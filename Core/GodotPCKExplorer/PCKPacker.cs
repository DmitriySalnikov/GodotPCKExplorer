using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GodotPCKExplorer
{
    public class PCKPackerFile
    {
        public string Path;
        public long Size = 0;
        public long IndexOffsetPosition = 0;

        public byte[]? MD5 = null;
        public bool IsEncrypted = false;

        public PCKPackerFile(string path)
        {
            Path = path;
            IndexOffsetPosition = 0;
        }

        public virtual void CalculateMD5()
        {

        }

        public virtual IEnumerable<ReadOnlyMemory<byte>> ReadMemoryBlocks()
        {
            yield break;
        }
    }

    public sealed class PCKPackerRegularFile : PCKPackerFile
    {
        public string OriginalPath;

        public PCKPackerRegularFile(string o_path, string path) : base(path)
        {
            OriginalPath = o_path;
            Path = path;
            Size = new FileInfo(o_path).Length;
        }

        public override void CalculateMD5()
        {
            MD5 = PCKUtils.GetFileMD5(OriginalPath);
        }

        public override IEnumerable<ReadOnlyMemory<byte>> ReadMemoryBlocks()
        {
            using var stream = File.Open(OriginalPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(stream))
            {
                yield return block;
            }
        }
    }

    public static class PCKPacker
    {
        // 16 bytes for MD5
        // 8 bytes for size of data
        // 16 bytes for IV
        const int ENCRYPTED_HEADER_SIZE = 16 + 8 + mbedTLS.CHUNK_SIZE;

        [ThreadStatic]
        static byte[]? temp_encryption_output_buffer;

        static void CloseAndDeleteFile(BinaryWriter? writer, string out_pck)
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

        /// <summary>
        /// Create a new PCK file from existing files.
        /// </summary>
        /// <param name="outPck">Output file. It can be a new or an existing file.</param>
        /// <param name="embed">If enabled and an existing <see cref="outPck"/> is specified, then the PCK will be embedded into this file.</param>
        /// <param name="files">Enumeration of <see cref="PCKPackerRegularFile"/> files to be packed.</param>
        /// <param name="godotVersion">PCK file version.</param>
        /// <param name="packPathPrefix">The path prefix in the pack. For example, if the prefix is <c>test_folder/</c>, then the path <c>res://icon.png</c> is converted to <c>res://test_folder/icon.png</c>.</param>
        /// <param name="alignment">The address of each file will be aligned to this value.</param>
        /// <param name="encKey">Specify the encryption key if you want the file to be encrypted. To specify a <see cref="string"/>, look at <seealso cref="PCKUtils.HexStringToByteArray(string?)"/></param>
        /// <param name="encrypt_index">Whether to encrypt the index (list of contents).</param>
        /// <param name="encrypt_files">Whether to encrypt the contents of files.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool PackFiles(string outPck, bool embed, IEnumerable<PCKPackerFile> files, PCKVersion godotVersion, string packPathPrefix = "", uint alignment = 16, byte[]? encKey = null, bool encrypt_index = false, bool encrypt_files = false, CancellationToken? cancellationToken = null)
        {
            byte[]? EncryptionKey = encKey;
            bool EncryptIndex = encrypt_index;
            bool EncryptFiles = encrypt_files;
            packPathPrefix = packPathPrefix.Replace("\\", "/");

            const string baseOp = "Pack files";
            var op = baseOp;

            if (!godotVersion.IsValid())
            {
                PCKActions.progress?.ShowMessage("Incorrect version is specified!", "Error", MessageType.Error);
                return false;
            }

            if (embed)
            {
                if (!File.Exists(outPck))
                {
                    PCKActions.progress?.ShowMessage("Attempt to embed a package in a non-existent file", "Error", MessageType.Error);
                    return false;
                }
                else
                {
                    var pck = new PCKReader();
                    if (pck.OpenFile(outPck, false))
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
                        if (File.Exists(outPck))
                            File.Delete(outPck);
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
                    binWriter = new BinaryWriter(File.Open(outPck, FileMode.OpenOrCreate, FileAccess.ReadWrite));
                }
                catch (Exception ex)
                {
                    CloseAndDeleteFile(binWriter, outPck);
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
                        CloseAndDeleteFile(binWriter, outPck);
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
                                CloseAndDeleteFile(binWriter, outPck);
                                return false;
                            }

                            file_idx++;
                            string res_file = PCKUtils.GetResFilePath(file.Path, packPathPrefix);
                            var str = Encoding.UTF8.GetBytes(res_file).ToList();
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

                            file.IndexOffsetPosition = index_writer.BaseStream.Position;
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
                                file.CalculateMD5();
                                index_writer.Write(file.MD5);

                                // TODO allow to encrypt a specific files?
                                file.IsEncrypted = EncryptFiles;
                                index_writer.Write((int)(file.IsEncrypted ? 1 : 0));

                                PCKActions.progress?.LogProgress(op, $"Calculated MD5: {res_file}\n{PCKUtils.ByteArrayToHexString(file.MD5, " ")}");
                            }

                            PCKActions.progress?.LogProgress(op, (int)(((double)file_idx / files.Count()) * 100));
                        };

                        if (EncryptIndex)
                        {
                            // Later it will be encrypted and the data size will be aligned to 16 + encrypted header
                            PCKUtils.AddPadding(binWriter, PCKUtils.AlignAddress(index_writer.BaseStream.Length, mbedTLS.CHUNK_SIZE) + ENCRYPTED_HEADER_SIZE);
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
                            CloseAndDeleteFile(binWriter, outPck);
                            return false;
                        }

                        PCKActions.progress?.LogProgress(op, PCKUtils.GetResFilePath(file.Path, packPathPrefix));

                        // go back to store the file's offset
                        {
                            long pos = index_writer.BaseStream.Position;
                            index_writer.BaseStream.Seek(file.IndexOffsetPosition, SeekOrigin.Begin);

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
                        if (file.IsEncrypted)
                        {
                            var result_size = PackStreamEncrypted(binWriter, file.Size, file.ReadMemoryBlocks(), EncryptionKey ?? throw new NullReferenceException(nameof(EncryptionKey)), file.MD5 ?? throw new NullReferenceException(nameof(file.MD5)), () =>
                            {
                                PCKActions.progress?.LogProgress(op, (int)((double)binWriter.BaseStream.Position / total_size * 100)); // update progress bar
                                return !(cancellationToken?.IsCancellationRequested ?? false);
                            });

                            // canceled
                            if (result_size == -1 || Math.Abs(result_size - actual_file_size) > ENCRYPTED_HEADER_SIZE + mbedTLS.CHUNK_SIZE)
                            {
                                CloseAndDeleteFile(binWriter, outPck);
                                return false;
                            }

                            actual_file_size = result_size;
                        }
                        else
                        {
                            foreach (var block in file.ReadMemoryBlocks())
                            {
                                binWriter.Write(block.Span);
                                PCKActions.progress?.LogProgress(op, (int)((double)binWriter.BaseStream.Position / total_size * 100)); // update progress bar

                                // cancel packing
                                if (cancellationToken?.IsCancellationRequested ?? false)
                                {
                                    CloseAndDeleteFile(binWriter, outPck);
                                    return false;
                                }
                            }
                        }

                        // get offset of the next file and add some padding
                        offset = PCKUtils.AlignAddress(offset + actual_file_size, alignment);
                        PCKUtils.AddPadding(binWriter, offset - binWriter.BaseStream.Position, EncryptFiles); // fill random bytes between files

                        count += 1;
                    };

                    // TODO add PCK validation

                    // If the index is encrypted, then it must be written after all other operations in order to properly handle file offsets
                    if (EncryptIndex)
                    {
                        // Move to start of index
                        long pos = binWriter.BaseStream.Position;
                        binWriter.BaseStream.Seek(index_begin_pos, SeekOrigin.Begin);

                        PackStreamEncrypted(binWriter, index_writer.BaseStream.Length, PCKUtils.ReadStreamAsMemoryBlocks(index_writer.BaseStream), EncryptionKey ?? throw new NullReferenceException(nameof(EncryptionKey)), PCKUtils.GetStreamMD5(index_writer.BaseStream));
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
                    CloseAndDeleteFile(binWriter, outPck);
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

        static long PackStreamEncrypted(BinaryWriter binWriter, long dataSize, IEnumerable<ReadOnlyMemory<byte>> dataBlocks, byte[] key, byte[] md5, Func<bool>? onStep = null)
        {
            if (PCKUtils.BUFFER_MAX_SIZE % mbedTLS.CHUNK_SIZE != 0)
                throw new ArgumentException($"{nameof(PCKUtils.BUFFER_MAX_SIZE)} must be a multiple of {mbedTLS.CHUNK_SIZE}.");

            temp_encryption_output_buffer ??= new byte[PCKUtils.BUFFER_MAX_SIZE];
            var output_buffer = new Memory<byte>(temp_encryption_output_buffer, 0, PCKUtils.BUFFER_MAX_SIZE);

            binWriter.Write(md5);
            binWriter.Write((long)dataSize); // original size

            var iv = new byte[mbedTLS.CHUNK_SIZE];
            Random rnd = new Random();
            rnd.NextBytes(iv);

            binWriter.Write(iv);
            long total_size = 0;

            using var mtls = new mbedTLS();

            mtls.set_key(key);

            foreach (var block in dataBlocks)
            {
                mtls.encrypt_cfb(iv, block, output_buffer, out long out_size);
                binWriter.Write(output_buffer[..(int)out_size].Span);
                total_size += out_size;

                if (onStep != null)
                {
                    if (!onStep.Invoke())
                        return -1;
                }
            }

            return total_size + ENCRYPTED_HEADER_SIZE;
        }
    }
}
