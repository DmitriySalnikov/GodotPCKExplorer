using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            public FileToPack(string o_path, string path, long size)
            {
                OriginalPath = o_path;
                Path = path;
                Size = size;
                OffsetPosition = 0;
            }
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
                try
                {
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

                    BinaryWriter pck = null;
                    try
                    {
                        pck = new BinaryWriter(File.OpenWrite(out_pck));
                    }
                    catch (Exception ex)
                    {
                        CloseAndDeleteFile(pck, out_pck);
                        Program.ShowMessage(ex, "Error", MessageType.Error); result = false; return;
                    }

                    long embed_start = 0;
                    if (embed)
                    {
                        pck.BaseStream.Seek(0, SeekOrigin.End);
                        embed_start = pck.BaseStream.Position;

                        // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:994
                        // Ensure embedded PCK starts at a 64-bit multiple
                        try
                        {
                            Utils.AddPadding(pck, (uint)pck.BaseStream.Position % 8);
                        }
                        catch (Exception ex)
                        {
                            CloseAndDeleteFile(pck, out_pck);
                            Program.ShowMessage(ex, "Error", MessageType.Error); result = false; return;
                        }

                    }

                    long pck_start = pck.BaseStream.Position;

                    try
                    {
                        pck.Write(Utils.PCK_MAGIC);
                        pck.Write(godotVersion.PackVersion);
                        pck.Write(godotVersion.Major);
                        pck.Write(godotVersion.Minor);
                        pck.Write(godotVersion.Revision);

                        long file_base_address = -1;

                        if (godotVersion.PackVersion == 2)
                        {
                            pck.Write((int)0); // TODO: pack_flags (is pack encrypted)
                            file_base_address = pck.BaseStream.Position;
                            pck.Write((long)0);
                        }

                        Utils.AddPadding(pck, 16 * sizeof(int)); // reserved

                        // write the index
                        pck.Write((int)files.Count());


                        long total_size = 0;
                        // write pck header
                        foreach (var file in files)
                        {
                            var str = Encoding.UTF8.GetBytes(file.Path).ToList();
                            var str_len = str.Count;

                            // Godot 4's PCK uses padding for some reason...
                            if (godotVersion.PackVersion == 2)
                                str_len = (int)Utils.AlignAddress(str_len, 4); // align with 4

                            // store pascal string (size, data)
                            pck.Write(str_len);
                            pck.Write(str.ToArray());

                            // Add padding for string
                            if (godotVersion.PackVersion == 2)
                                Utils.AddPadding(pck, (uint)(str_len - str.Count));

                            file.OffsetPosition = pck.BaseStream.Position;
                            pck.Write((long)0); // offset
                            pck.Write((long)file.Size); // size

                            total_size += file.Size; // for progress bar

                            if (godotVersion.PackVersion < 2)
                            {
                                // # empty md5
                                Utils.AddPadding(pck, 16 * sizeof(byte));
                            }
                            else
                            {
                                pck.Write(Utils.GetFileMD5(file.OriginalPath));

                                pck.Write((int)0); // TODO: add flags (encrypted or not)
                            }
                        };

                        total_size += pck.BaseStream.Position;

                        long offset = pck.BaseStream.Position;
                        offset = Utils.AlignAddress(offset, alignment);

                        Utils.AddPadding(pck, (uint)(offset - pck.BaseStream.Position));

                        long file_base = offset;
                        if (godotVersion.PackVersion == 2)
                        {
                            // update actual address of file_base in the header
                            pck.Seek((int)file_base_address, SeekOrigin.Begin);
                            pck.Write(file_base);
                            pck.Seek((int)offset, SeekOrigin.Begin);
                        }

                        // write actual files data
                        int count = 0;
                        foreach (var file in files)
                        {
                            // cancel packing
                            if (bw.CancellationPending)
                            {
                                CloseAndDeleteFile(pck, out_pck);
                                result = false;
                                return;
                            }

                            BinaryReader src = new BinaryReader(File.OpenRead(file.OriginalPath));
                            long to_write = file.Size;
                            while (to_write > 0)
                            {
                                var read = src.ReadBytes(Utils.BUFFER_MAX_SIZE);
                                pck.Write(read);
                                to_write -= read.Length;

                                bw.ReportProgress((int)((double)pck.BaseStream.Position / total_size * 100)); // update progress bar

                                // cancel packing
                                if (bw.CancellationPending)
                                {
                                    src.Close();
                                    CloseAndDeleteFile(pck, out_pck);
                                    result = false;
                                    return;
                                }
                            };

                            long pos = pck.BaseStream.Position;
                            pck.BaseStream.Seek(file.OffsetPosition, SeekOrigin.Begin); // go back to store the pck's offset

                            if (godotVersion.PackVersion < 2)
                            {
                                pck.Write((long)offset);
                            }
                            else
                            {
                                pck.Write((long)offset - file_base);
                            }

                            pck.BaseStream.Seek(pos, SeekOrigin.Begin);

                            offset = Utils.AlignAddress(offset + file.Size, alignment);
                            Utils.AddPadding(pck, (uint)(offset - pos));

                            src.Close();

                            count += 1;
                        };

                        if (embed)
                        {
                            // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:1073
                            // Ensure embedded data ends at a 64-bit multiple
                            long embed_end = pck.BaseStream.Position - embed_start + 12;
                            Utils.AddPadding(pck, (uint)embed_end % 8);

                            long pck_size = pck.BaseStream.Position - pck_start;
                            pck.Write((long)pck_size);
                            pck.Write((int)Utils.PCK_MAGIC);
                        }

                        bw.ReportProgress(100);
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        CloseAndDeleteFile(pck, out_pck); result = false; return;
                    }

                    pck.Close();
                    result = true;
                    return;
                }
                finally
                {
                    are.Set();
                }
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();
            are.WaitOne();

            //if (result)
            //    Program.ShowMessage("Complete!", "Progress");

            return result;
        }
    }
}
