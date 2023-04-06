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

        long _align(long p_n, int p_alignment)
        {
            if (p_alignment == 0)
                return p_n;

            long rest = p_n % p_alignment;
            if (rest == 0)
                return p_n;
            else
                return p_n + (p_alignment - rest);
        }

        void _pad(BinaryWriter p_file, int p_bytes)
        {
            p_file.Write(new byte[p_bytes]);
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
                Utils.CommandLog(ex, "Error", false, MessageType.Error);
            }
        }

        public bool PackFiles(string out_pck, IEnumerable<FileToPack> files, int alignment, PCKVersion godotVersion, bool embed)
        {
            var bp = new BackgroundProgress();
            var bw = bp.bg_worker;
            var are = new AutoResetEvent(false);

            bool result = false;

            if (!godotVersion.IsValid)
            {
                bp.Dispose();
                Utils.ShowMessage("Incorrect version is specified!", "Error", MessageType.Error);
                return false;
            }

            if (embed)
            {
                if (!File.Exists(out_pck))
                {
                    Utils.CommandLog("Attempt to embed a package in a non-existent file", "Error", false, MessageType.Error);
                    return false;
                }
                else
                {
                    var pck = new PCKReader();
                    if (pck.OpenFile(out_pck, false))
                    {
                        pck.Close();
                        Utils.CommandLog("Attempt to embed a package in a file with an already embedded package or in a regular '.pck' file", "Error", false, MessageType.Error);
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
                            Utils.ShowMessage(ex, "Error", MessageType.Error);
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
                        Utils.ShowMessage(ex, "Error", MessageType.Error); result = false; return;
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
                            int pad = (int)(pck.BaseStream.Position % 8);
                            for (int i = 0; i < pad; i++)
                            {
                                pck.Write((byte)0);
                            }
                        }
                        catch (Exception ex)
                        {
                            CloseAndDeleteFile(pck, out_pck);
                            Utils.ShowMessage(ex, "Error", MessageType.Error); result = false; return;
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

                        if (godotVersion.PackVersion == 2)
                        {
                            pck.Write((int)0); // TODO: pack_flags (is pack encrypted)
                            pck.Write((long)0); // TODO: file_base (where begins the chunk of actual data)
                        }

                        _pad(pck, 16 * sizeof(int)); // reserved

                        // write the index
                        pck.Write((int)files.Count());

                        long total_size = 0;
                        // write pck header
                        foreach (var file in files)
                        {
                            var str = Encoding.UTF8.GetBytes(file.Path).ToList();
                            pck.Write((int)str.Count); // write str size because of original function "store_pascal_string" store size and after actual data
                            pck.Write(str.ToArray());
                            file.OffsetPosition = pck.BaseStream.Position;
                            pck.Write((long)0); // offset
                            pck.Write((long)file.Size); // size

                            total_size += file.Size; // for progress bar

                            // # empty md5
                            if (godotVersion.PackVersion < 2)
                            {
                                pck.Write((int)0);
                                pck.Write((int)0);
                                pck.Write((int)0);
                                pck.Write((int)0);
                            }
                            else
                            {
                                pck.Write(Utils.GetFileMD5(file.Path));

                                pck.Write((int)0); // TODO: add flags (encrypted or not)
                            }
                        };

                        total_size += pck.BaseStream.Position;

                        long ofs = pck.BaseStream.Position;
                        ofs = _align(ofs, alignment);

                        _pad(pck, (int)(ofs - pck.BaseStream.Position));

                        const int buf_max = 65536;

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
                                var read = src.ReadBytes(buf_max);
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
                            pck.Write((long)ofs);
                            pck.BaseStream.Seek(pos, SeekOrigin.Begin);

                            ofs = _align(ofs + file.Size, alignment);
                            _pad(pck, (int)(ofs - pos));

                            src.Close();

                            count += 1;
                        };

                        if (embed)
                        {
                            // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:1073
                            // Ensure embedded data ends at a 64-bit multiple
                            long embed_end = pck.BaseStream.Position - embed_start + 12;
                            long pad = embed_end % 8;
                            for (long i = 0; i < pad; i++)
                            {
                                pck.Write((byte)0);
                            }

                            long pck_size = pck.BaseStream.Position - pck_start;
                            pck.Write((long)pck_size);
                            pck.Write((int)Utils.PCK_MAGIC);
                        }

                        bw.ReportProgress(100);
                    }
                    catch (Exception ex)
                    {
                        Utils.ShowMessage(ex, "Error", MessageType.Error);
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
            //    Utils.ShowMessage("Complete!", "Progress");

            return result;
        }
    }
}
