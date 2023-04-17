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

            public byte[] md5 = null;
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
        public byte[] EncryptionKey = null;
        public bool EncryptIndex = false;
        public bool EncryptFiles = false;

        public PCKPacker(byte[] encKey = null, bool encrypt_index = false, bool encrypt_files = false)
        {
            EncryptionKey = encKey;
            EncryptIndex = encrypt_index;
            EncryptFiles = encrypt_files;
        }

        void CloseAndDeleteFile(BinaryWriter writer, string out_pck)
        {
            writer?.Close();

            try
            {
                File.Delete(out_pck);
            }
            catch (Exception ex)
            {
                Program.CommandLog(ex, "Error", false, MessageType.Error);
            }
        }

        public bool PackFiles(string out_pck, IEnumerable<FileToPack> files, uint alignment, PCKVersion godotVersion, bool embed)
        {
            var bp = new BackgroundProgress();
            var bw = bp.bg_worker;
            var are = new AutoResetEvent(false);

            bool result = false;

            if (!godotVersion.IsValid)
            {
                bp.Dispose();
                Program.ShowMessage("Incorrect version is specified!", "Error", MessageType.Error);
                return false;
            }

            if (embed)
            {
                if (!File.Exists(out_pck))
                {
                    Program.CommandLog("Attempt to embed a package in a non-existent file", "Error", false, MessageType.Error);
                    return false;
                }
                else
                {
                    var pck = new PCKReader();
                    if (pck.OpenFile(out_pck, false))
                    {
                        pck.Close();
                        Program.CommandLog("Attempt to embed a package in a file with an already embedded package or in a regular '.pck' file", "Error", false, MessageType.Error);
                        return false;
                    }
                }
            }

            bw.DoWork += (sender, ev) =>
            {
                var op = "Pack files";
                var lpr = new LogProgressReporter(op);

                try
                {
                    Program.LogProgress(op, "Starting.");

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
                            Program.ShowMessage(ex, "Error", MessageType.Error);
                            result = false;
                            return;
                        }
                    }

                    BinaryWriter binWriter = null;
                    try
                    {
                        binWriter = new BinaryWriter(File.Open(out_pck, FileMode.OpenOrCreate, FileAccess.ReadWrite));
                    }
                    catch (Exception ex)
                    {
                        CloseAndDeleteFile(binWriter, out_pck);
                        Program.ShowMessage(ex, "Error", MessageType.Error); result = false; return;
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
                            Utils.AddPadding(binWriter, binWriter.BaseStream.Position % 8);
                        }
                        catch (Exception ex)
                        {
                            CloseAndDeleteFile(binWriter, out_pck);
                            Program.ShowMessage(ex, "Error", MessageType.Error); result = false; return;
                        }

                    }

                    long pck_start = binWriter.BaseStream.Position;

                    try
                    {
                        Program.LogProgress(op, "Writing the file index");

                        binWriter.Write(Utils.PCK_MAGIC);
                        binWriter.Write(godotVersion.PackVersion);
                        binWriter.Write(godotVersion.Major);
                        binWriter.Write(godotVersion.Minor);
                        binWriter.Write(godotVersion.Revision);

                        long file_base_address = -1;

                        if (godotVersion.PackVersion == Utils.PCK_VERSION_GODOT_4)
                        {
                            binWriter.Write((int)(EncryptIndex ? 1 : 0));
                            file_base_address = binWriter.BaseStream.Position;
                            binWriter.Write((long)0);
                        }

                        Utils.AddPadding(binWriter, 16 * sizeof(int)); // reserved

                        // write the files count
                        binWriter.Write((int)files.Count());

                        var index_writer = binWriter;
                        long index_begin_pos = binWriter.BaseStream.Position;

                        long total_size = 0;

                        {
                            if (EncryptIndex)
                                index_writer = new BinaryWriter(new MemoryStream());

                            // write pck index
                            foreach (var file in files)
                            {
                                var str = Encoding.UTF8.GetBytes(file.Path).ToList();
                                var str_len = str.Count;

                                // Godot 4's PCK uses padding for some reason...
                                if (godotVersion.PackVersion == Utils.PCK_VERSION_GODOT_4)
                                    str_len = (int)Utils.AlignAddress(str_len, 4); // align with 4

                                // store pascal string (size, data)
                                index_writer.Write(str_len);
                                index_writer.Write(str.ToArray());

                                // Add padding for string
                                if (godotVersion.PackVersion == Utils.PCK_VERSION_GODOT_4)
                                    Utils.AddPadding(index_writer, str_len - str.Count);

                                file.OffsetPosition = index_writer.BaseStream.Position;
                                index_writer.Write((long)0); // offset for later use
                                index_writer.Write((long)file.Size); // size

                                total_size += file.Size; // for progress bar

                                if (godotVersion.PackVersion < Utils.PCK_VERSION_GODOT_4)
                                {
                                    // # empty md5
                                    Utils.AddPadding(index_writer, 16 * sizeof(byte));
                                }
                                else
                                {
                                    file.md5 = Utils.GetFileMD5(file.OriginalPath);
                                    index_writer.Write(file.md5);

                                    file.is_encrypted = EncryptFiles;
                                    index_writer.Write((int)(file.is_encrypted ? 1 : 0));
                                }
                            };

                            if (EncryptIndex)
                            {
                                // Later it will be encrypted and the data size will be aligned to 16 + encrypted header
                                Utils.AddPadding(binWriter, Utils.AlignAddress(index_writer.BaseStream.Length, 16) + ENCRYPTED_HEADER_SIZE);
                            }
                        }

                        // approximate size of the output file for displaying progress
                        total_size += binWriter.BaseStream.Position;

                        // file_base or individual offset
                        long offset = binWriter.BaseStream.Position;
                        offset = Utils.AlignAddress(offset, alignment);

                        Utils.AddPadding(binWriter, offset - binWriter.BaseStream.Position);

                        long file_base = offset;
                        if (godotVersion.PackVersion == Utils.PCK_VERSION_GODOT_4)
                        {
                            // update actual address of file_base in the header
                            binWriter.BaseStream.Seek(file_base_address, SeekOrigin.Begin);
                            binWriter.Write(file_base);
                            binWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
                        }

                        // write actual files data
                        Program.LogProgress(op, "Writing the content of files");

                        int count = 0;
                        foreach (var file in files)
                        {
                            // cancel packing
                            if (bw.CancellationPending)
                            {
                                CloseAndDeleteFile(binWriter, out_pck);
                                result = false;
                                return;
                            }
                            Program.LogProgress(op, file.OriginalPath);

                            // go back to store the file's offset
                            {
                                long pos = index_writer.BaseStream.Position;
                                index_writer.BaseStream.Seek(file.OffsetPosition, SeekOrigin.Begin);

                                if (godotVersion.PackVersion < Utils.PCK_VERSION_GODOT_4)
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
                                actual_file_size = PackEncryptedBlock(binWriter, File.ReadAllBytes(file.OriginalPath), EncryptionKey);

                                Utils.ReportProgress((int)((double)binWriter.BaseStream.Position / total_size * 100), bw, lpr); // update progress bar
                            }
                            else
                            {
                                using (BinaryReader src = new BinaryReader(File.OpenRead(file.OriginalPath)))
                                {
                                    long to_write = file.Size;
                                    while (to_write > 0)
                                    {
                                        var read = src.ReadBytes(Utils.BUFFER_MAX_SIZE);
                                        binWriter.Write(read);
                                        to_write -= read.Length;

                                        Utils.ReportProgress((int)((double)binWriter.BaseStream.Position / total_size * 100), bw, lpr); // update progress bar

                                        // cancel packing
                                        if (bw.CancellationPending)
                                        {
                                            CloseAndDeleteFile(binWriter, out_pck);
                                            result = false;
                                            return;
                                        }
                                    };
                                }
                            }

                            // get offset of the next file and add some padding
                            offset = Utils.AlignAddress(offset + actual_file_size, alignment);
                            Utils.AddPadding(binWriter, offset - binWriter.BaseStream.Position);

                            count += 1;
                        };

                        // If the index is encrypted, then it must be written after all other operations in order to properly handle file offsets
                        if (EncryptIndex)
                        {
                            long pos = binWriter.BaseStream.Position;
                            binWriter.BaseStream.Seek(index_begin_pos, SeekOrigin.Begin);

                            PackEncryptedBlock(binWriter, (MemoryStream)index_writer.BaseStream, EncryptionKey);
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
                            Utils.AddPadding(binWriter, embed_end % 8);

                            long pck_size = binWriter.BaseStream.Position - pck_start;
                            binWriter.Write((long)pck_size);
                            binWriter.Write((int)Utils.PCK_MAGIC);
                        }

                        Utils.ReportProgress(100, bw, lpr);
                        Program.LogProgress(op, "Completed!");
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        CloseAndDeleteFile(binWriter, out_pck); result = false; return;
                    }

                    binWriter.Close();
                    result = true;
                    return;
                }
                catch (Exception ex)
                {
                    Program.Log(ex);
                }
                finally
                {
                    lpr.Dispose();
                    are.Set();
                }
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();
            are.WaitOne();

            if (result)
                Program.Log("Pack complete!");

            return result;
        }

        // TODO: encrypt data in parts, not all data at once
        long PackEncryptedBlock(BinaryWriter binWriter, MemoryStream stream, byte[] key)
        {
            var data = stream.ToArray();
            return PackEncryptedBlock(binWriter, data, key);
        }

        long PackEncryptedBlock(BinaryWriter binWriter, byte[] data, byte[] key)
        {
            var md5 = new byte[16];
            using (var md5_crypt = MD5.Create())
                md5 = md5_crypt.ComputeHash(data);

            binWriter.Write(md5);
            binWriter.Write((long)data.Length);

            var iv = new byte[16];
            Random rnd = new Random();
            rnd.NextBytes(iv);

            binWriter.Write(iv);

            using (var mtls = new mbedTLS())
            {
                mtls.set_key(key);
                mtls.encrypt_cfb(iv.ToArray(), data, out byte[] output);
                binWriter.Write(output);

                return output.Length + ENCRYPTED_HEADER_SIZE;
            }
        }
    }
}
