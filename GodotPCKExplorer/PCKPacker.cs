using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GodotPCKExplorer
{
    public class PCKPacker
    {
        public class FileToPack
        {
            public string Path;
            public string OriginalPath;
            public long Size;
            public long OffsetPosition;

            public byte[]? md5 = null;
            public bool is_encrypted = false;

            public FileToPack(string o_path, string path, long size)
            {
                OriginalPath = o_path;
                Path = path;
                Size = size;
                OffsetPosition = 0;
            }
        }

        // 16 bytes for MD5
        // 8 bytes for size of data
        // 16 bytes for IV
        const int ENCRYPTED_HEADER_SIZE = 16 + 8 + 16;
        public byte[]? EncryptionKey = null;
        public bool EncryptIndex = false;
        public bool EncryptFiles = false;

        [ThreadStatic]
        static byte[]? temp_encryption_buffer;

        public PCKPacker(byte[]? encKey = null, bool encrypt_index = false, bool encrypt_files = false)
        {
            EncryptionKey = encKey;
            EncryptIndex = encrypt_index;
            EncryptFiles = encrypt_files;
        }

        void CloseAndDeleteFile(BinaryWriter? writer, string out_pck)
        {
            writer?.Close();

            try
            {
                File.Delete(out_pck);
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
            }
        }

        public bool PackFiles(string out_pck, IEnumerable<FileToPack> files, uint alignment, PCKVersion godotVersion, bool embed, CancellationToken? cancellationToken = null)
        {
            const string baseOp = "Pack files";
            var op = baseOp;

            if (!godotVersion.IsValid)
            {
                PCKActions.progress?.ShowMessage("Incorrect version is specified!", "Error", MessageType.Error);
                return false;
            }

            if (embed)
            {
                if (!File.Exists(out_pck))
                {
                    PCKActions.progress?.ShowMessage("Attempt to embed a package in a non-existent file", "Error", MessageType.Error);
                    return false;
                }
                else
                {
                    var pck = new PCKReader();
                    if (pck.OpenFile(out_pck, false))
                    {
                        pck.Close();
                        PCKActions.progress?.ShowMessage("Attempt to embed a package in a file with an already embedded package or in a regular '.pck' file", "Error", MessageType.Error);
                        return false;
                    }
                }
            }

            if (EncryptionKey == null)
            {
                if (EncryptIndex || EncryptFiles)
                {
                    // TODO add test
                    PCKActions.progress?.ShowMessage("The encryption key is not specified, although the encryption mode is activated.", "Error", MessageType.Error);
                    return false;
                }
            }


            try
            {
                PCKActions.progress?.LogProgress(op, "Starting.");
                PCKActions.progress?.LogProgress(op, $"Version: {godotVersion}");
                PCKActions.progress?.LogProgress(op, $"Alignment: {alignment}");
                if (EncryptIndex || EncryptFiles)
                {
                    PCKActions.progress?.LogProgress(op, $"Encryption key: {PCKUtils.ByteArrayToHexString(EncryptionKey)}");
                    PCKActions.progress?.LogProgress(op, $"Encrypt Index: {EncryptIndex}");
                    PCKActions.progress?.LogProgress(op, $"Encrypt Files: {EncryptFiles}");
                }

                // delete if not embbeding
                if (!embed)
                {
                    try
                    {
                        if (File.Exists(out_pck))
                            File.Delete(out_pck);
                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }
                }

                BinaryWriter? binWriter = null;
                try
                {
                    binWriter = new BinaryWriter(File.Open(out_pck, FileMode.OpenOrCreate, FileAccess.ReadWrite));
                }
                catch (Exception ex)
                {
                    CloseAndDeleteFile(binWriter, out_pck);
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    return false;
                }

                long embed_start = 0;
                if (embed)
                {
                    binWriter.BaseStream.Seek(0, SeekOrigin.End);
                    embed_start = binWriter.BaseStream.Position;

                    // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:994
                    // Ensure embedded PCK starts at a 64-bit multiple
                    try
                    {
                        PCKUtils.AddPadding(binWriter, binWriter.BaseStream.Position % 8);
                    }
                    catch (Exception ex)
                    {
                        CloseAndDeleteFile(binWriter, out_pck);
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }

                }

                long pck_start = binWriter.BaseStream.Position;

                try
                {
                    PCKActions.progress?.LogProgress(op, "Writing the file index");

                    binWriter.Write(PCKUtils.PCK_MAGIC);
                    binWriter.Write(godotVersion.PackVersion);
                    binWriter.Write(godotVersion.Major);
                    binWriter.Write(godotVersion.Minor);
                    binWriter.Write(godotVersion.Revision);

                    long file_base_address = -1;

                    if (godotVersion.PackVersion == PCKUtils.PCK_VERSION_GODOT_4)
                    {
                        binWriter.Write((int)(EncryptIndex ? 1 : 0));
                        file_base_address = binWriter.BaseStream.Position;
                        binWriter.Write((long)0);
                    }

                    PCKUtils.AddPadding(binWriter, 16 * sizeof(int)); // reserved

                    // write the files count
                    binWriter.Write((int)files.Count());

                    var index_writer = binWriter;
                    long index_begin_pos = binWriter.BaseStream.Position;

                    long total_size = 0;

                    {
                        if (EncryptIndex)
                            index_writer = new BinaryWriter(new MemoryStream());

                        op = baseOp + ", writing an index";
                        // write pck index
                        int file_idx = 0;
                        foreach (var file in files)
                        {
                            // cancel packing
                            if (cancellationToken?.IsCancellationRequested ?? false)
                            {
                                CloseAndDeleteFile(binWriter, out_pck);
                                return false;
                            }

                            file_idx++;
                            var str = Encoding.UTF8.GetBytes(file.Path).ToList();
                            var str_len = str.Count;

                            // Godot 4's PCK uses padding for some reason...
                            if (godotVersion.PackVersion == PCKUtils.PCK_VERSION_GODOT_4)
                                str_len = (int)PCKUtils.AlignAddress(str_len, 4); // align with 4

                            // store pascal string (size, data)
                            index_writer.Write(str_len);
                            index_writer.Write(str.ToArray());

                            // Add padding for string
                            if (godotVersion.PackVersion == PCKUtils.PCK_VERSION_GODOT_4)
                                PCKUtils.AddPadding(index_writer, str_len - str.Count);

                            file.OffsetPosition = index_writer.BaseStream.Position;
                            index_writer.Write((long)0); // offset for later use
                            index_writer.Write((long)file.Size); // size

                            total_size += file.Size; // for progress bar

                            if (godotVersion.PackVersion < PCKUtils.PCK_VERSION_GODOT_4)
                            {
                                // # empty md5
                                PCKUtils.AddPadding(index_writer, 16 * sizeof(byte));
                            }
                            else
                            {
                                file.md5 = PCKUtils.GetFileMD5(file.OriginalPath);
                                index_writer.Write(file.md5);

                                file.is_encrypted = EncryptFiles;
                                index_writer.Write((int)(file.is_encrypted ? 1 : 0));

                                PCKActions.progress?.LogProgress(op, $"Calculated MD5: {file.OriginalPath}\n{PCKUtils.ByteArrayToHexString(file.md5, " ")}");
                            }

                            PCKActions.progress?.LogProgress(op, (int)(((double)file_idx / files.Count()) * 100));
                        };

                        if (EncryptIndex)
                        {
                            // Later it will be encrypted and the data size will be aligned to 16 + encrypted header
                            PCKUtils.AddPadding(binWriter, PCKUtils.AlignAddress(index_writer.BaseStream.Length, 16) + ENCRYPTED_HEADER_SIZE);
                        }
                    }

                    // approximate size of the output file for displaying progress
                    total_size += binWriter.BaseStream.Position;

                    // file_base or individual offset
                    long offset = binWriter.BaseStream.Position;
                    offset = PCKUtils.AlignAddress(offset, alignment);

                    // end of index
                    PCKUtils.AddPadding(binWriter, offset - binWriter.BaseStream.Position, EncryptIndex); // fill random bytes between index and files

                    long file_base = offset;
                    if (godotVersion.PackVersion == PCKUtils.PCK_VERSION_GODOT_4)
                    {
                        // update actual address of file_base in the header
                        binWriter.BaseStream.Seek(file_base_address, SeekOrigin.Begin);
                        binWriter.Write(file_base);
                        binWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
                    }

                    // write actual files data
                    PCKActions.progress?.LogProgress(op, "Writing the content of files");
                    op = baseOp + ", writing file contents";

                    int count = 0;
                    foreach (var file in files)
                    {
                        // cancel packing
                        if (cancellationToken?.IsCancellationRequested ?? false)
                        {
                            CloseAndDeleteFile(binWriter, out_pck);
                            return false;
                        }
                        PCKActions.progress?.LogProgress(op, file.OriginalPath);

                        // go back to store the file's offset
                        {
                            long pos = index_writer.BaseStream.Position;
                            index_writer.BaseStream.Seek(file.OffsetPosition, SeekOrigin.Begin);

                            if (godotVersion.PackVersion < PCKUtils.PCK_VERSION_GODOT_4)
                            {
                                index_writer.Write((long)offset);
                            }
                            else
                            {
                                index_writer.Write((long)offset - file_base);
                            }

                            index_writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                        }

                        long actual_file_size = file.Size;
                        if (file.is_encrypted)
                        {
                            using var stream = File.Open(file.OriginalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                            actual_file_size = PackStreamEncrypted(binWriter, stream, EncryptionKey ?? throw new NullReferenceException(nameof(EncryptionKey)), file.md5, () =>
                            {
                                PCKActions.progress?.LogProgress(op, (int)((double)binWriter.BaseStream.Position / total_size * 100)); // update progress bar
                                return !(cancellationToken?.IsCancellationRequested ?? false);
                            });

                            // canceled
                            if (actual_file_size == -1)
                            {
                                CloseAndDeleteFile(binWriter, out_pck);
                                return false;
                            }
                        }
                        else
                        {
                            using BinaryReader src = new BinaryReader(File.OpenRead(file.OriginalPath));

                            long to_write = file.Size;
                            while (to_write > 0)
                            {
                                var read = src.ReadBytes(PCKUtils.BUFFER_MAX_SIZE);
                                binWriter.Write(read);
                                to_write -= read.Length;

                                PCKActions.progress?.LogProgress(op, (int)((double)binWriter.BaseStream.Position / total_size * 100)); // update progress bar

                                // cancel packing
                                if (cancellationToken?.IsCancellationRequested ?? false)
                                {
                                    CloseAndDeleteFile(binWriter, out_pck);
                                    return false;
                                }
                            };
                        }

                        // get offset of the next file and add some padding
                        offset = PCKUtils.AlignAddress(offset + actual_file_size, alignment);
                        PCKUtils.AddPadding(binWriter, offset - binWriter.BaseStream.Position, EncryptFiles); // fill random bytes between files

                        count += 1;
                    };

                    // If the index is encrypted, then it must be written after all other operations in order to properly handle file offsets
                    if (EncryptIndex)
                    {
                        // Move to start of index
                        long pos = binWriter.BaseStream.Position;
                        binWriter.BaseStream.Seek(index_begin_pos, SeekOrigin.Begin);

                        // PackStreamEncrypted will generate MD5, so index_writer.Position must be moved to start
                        index_writer.BaseStream.Position = 0;

                        PackStreamEncrypted(binWriter, index_writer.BaseStream, EncryptionKey ?? throw new NullReferenceException(nameof(EncryptionKey)));
                        index_writer.Close();
                        index_writer.Dispose();
                        index_writer = null;

                        binWriter.BaseStream.Seek(pos, SeekOrigin.Begin);
                    }

                    if (embed)
                    {
                        // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:1073
                        // Ensure embedded data ends at a 64-bit multiple
                        long embed_end = binWriter.BaseStream.Position - embed_start + 12;
                        PCKUtils.AddPadding(binWriter, embed_end % 8);

                        long pck_size = binWriter.BaseStream.Position - pck_start;
                        binWriter.Write((long)pck_size);
                        binWriter.Write((int)PCKUtils.PCK_MAGIC);
                    }

                    PCKActions.progress?.LogProgress(op, 100);
                    PCKActions.progress?.LogProgress(op, "Completed!");
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    CloseAndDeleteFile(binWriter, out_pck);
                    return false;
                }

                binWriter.Close();
                PCKActions.progress?.Log("Pack complete!");
                return true;
            }
            catch (Exception ex)
            {
                PCKActions.progress?.Log(ex);
                return false;
            }
            finally
            {

            }
        }

        long PackStreamEncrypted(BinaryWriter binWriter, Stream stream, byte[] key, byte[]? md5 = null, Func<bool>? onStep = null)
        {
            temp_encryption_buffer ??= new byte[PCKUtils.BUFFER_MAX_SIZE];

            if (md5 == null)
            {
                long stream_position = stream.Position;
                using (var md5_crypt = MD5.Create())
                    md5 = md5_crypt.ComputeHash(stream);
                stream.Position = stream_position;
            }

            binWriter.Write(md5);
            binWriter.Write((long)stream.Length);

            var iv = new byte[16];
            Random rnd = new Random();
            rnd.NextBytes(iv);

            binWriter.Write(iv);
            long total_size = 0;

            using (var mtls = new mbedTLS())
            {
                mtls.set_key(key);

                while (stream.Position != stream.Length)
                {
                    if ((stream.Length - stream.Position) >= temp_encryption_buffer.Length)
                    {
                        var size = stream.Read(temp_encryption_buffer, 0, temp_encryption_buffer.Length);
                        mtls.encrypt_cfb(iv, temp_encryption_buffer, out byte[] output);
                        binWriter.Write(output);
                        total_size += output.Length;
                    }
                    else
                    {
                        byte[] data = new byte[stream.Length - stream.Position];
                        var size = stream.Read(data, 0, data.Length);
                        mtls.encrypt_cfb(iv, data, out byte[] output);
                        binWriter.Write(output);
                        total_size += output.Length;
                    }

                    if (onStep != null)
                    {
                        if (!onStep.Invoke())
                        {
                            return -1;
                        }
                    }
                }
            }

            return total_size + ENCRYPTED_HEADER_SIZE;
        }
    }
}
