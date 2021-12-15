using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

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
            File.Delete(out_pck);
        }

        public bool PackFiles(string out_pck, IEnumerable<FileToPack> files, int alignment, PCKVersion godotVersion)
        {
            var bp = new BackgroundProgress();
            var bw = bp.backgroundWorker1;
            bool result = false;

            if (!godotVersion.IsValid)
            {
                bp.Dispose();
                Utils.ShowMessage("Incorrect version is specified!", "Error");
                return false;
            }

            bw.DoWork += (sender, ev) =>
            {

                try
                {
                    if (File.Exists(out_pck))
                        File.Delete(out_pck);
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    result = false;
                    return;
                }

                BinaryWriter writer = null;
                try
                {
                    writer = new BinaryWriter(File.OpenWrite(out_pck));
                }
                catch (Exception e)
                {
                    writer?.Close();
                    Utils.ShowMessage(e.Message, "Error");
                    result = false;
                    return;
                }

                try
                {
                    writer.Write(Program.PCK_MAGIC);
                    writer.Write(godotVersion.PackVersion);
                    writer.Write(godotVersion.Major);
                    writer.Write(godotVersion.Minor);
                    writer.Write(godotVersion.Revision);

                    for (int i = 0; i < 16; i++)
                    {
                        writer.Write((int)0); // reserved
                    };

                    // write the index

                    writer.Write((int)files.Count());

                    long total_size = 0;
                    // write pck header
                    foreach (var file in files)
                    {
                        var str = Encoding.UTF8.GetBytes(file.Path).ToList();
                        writer.Write((int)str.Count); // write str size beacause of original function "store_pascal_string" store size and after actual data
                        writer.Write(str.ToArray());
                        file.OffsetPosition = writer.BaseStream.Position;
                        writer.Write((long)0); // offset
                        writer.Write((long)file.Size); // size

                        total_size += file.Size; // for progress bar

                        // # empty md5
                        writer.Write((int)0);
                        writer.Write((int)0);
                        writer.Write((int)0);
                        writer.Write((int)0);
                    };

                    total_size += writer.BaseStream.Position;

                    long ofs = writer.BaseStream.Position;
                    ofs = _align(ofs, alignment);

                    _pad(writer, (int)(ofs - writer.BaseStream.Position));

                    const int buf_max = 65536;

                    // write actual files data
                    int count = 0;
                    foreach (var file in files)
                    {
                        // cancel packing
                        if (bw.CancellationPending)
                        {
                            CloseAndDeleteFile(writer, out_pck);
                            result = false;
                            return;
                        }

                        BinaryReader src = new BinaryReader(File.OpenRead(file.OriginalPath));
                        long to_write = file.Size;
                        while (to_write > 0)
                        {
                            var read = src.ReadBytes(buf_max);
                            writer.Write(read);
                            to_write -= read.Length;

                            bw.ReportProgress((int)((double)writer.BaseStream.Position / total_size * 100)); // update progress bar

                            // cancel packing
                            if (bw.CancellationPending)
                            {
                                src.Close();
                                CloseAndDeleteFile(writer, out_pck);
                                result = false;
                                return;
                            }
                        };

                        long pos = writer.BaseStream.Position;
                        writer.BaseStream.Seek(file.OffsetPosition, SeekOrigin.Begin); // go back to store the writer's offset
                        writer.Write((long)ofs);
                        writer.BaseStream.Seek(pos, SeekOrigin.Begin);

                        ofs = _align(ofs + file.Size, alignment);
                        _pad(writer, (int)(ofs - pos));

                        src.Close();

                        count += 1;
                    };
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    CloseAndDeleteFile(writer, out_pck);
                    result = false;
                    return;
                }

                writer.Close();
                result = true;
                return;
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();

            if (result)
                Utils.ShowMessage("Complete!", "Progress");

            return result;
        }
    }
}
