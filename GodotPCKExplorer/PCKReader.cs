using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GodotPCKExplorer
{
    public class PCKReader
    {
        BinaryReader fileStream = null;
        public Dictionary<string, PackedFile> Files = new Dictionary<string, PackedFile>();
        public string PackPath = "";
        public int PCK_VersionPack = 0;
        public int PCK_VersionMajor = 0;
        public int PCK_VersionMinor = 0;
        public int PCK_VersionRevision = 0;
        public long PCK_StartPosition = 0;
        public long PCK_EndPosition = 0;
        public bool PCK_Embedded = false;

        ~PCKReader()
        {
            Close();
        }

        public void Close()
        {
            fileStream?.Close();
            fileStream = null;

            Files.Clear();
            PackPath = "";
            PCK_VersionPack = 0;
            PCK_VersionMajor = 0;
            PCK_VersionMinor = 0;
            PCK_VersionRevision = 0;
            PCK_StartPosition = 0;
            PCK_EndPosition = 0;
            PCK_Embedded = false;
        }

        public bool OpenFile(string p_path)
        {
            Close();

            try
            {
                p_path = Path.GetFullPath(p_path);
                fileStream = new BinaryReader(File.OpenRead(p_path));
            }
            catch (Exception e)
            {
                Utils.ShowMessage(e.Message, "Error");
                return false;
            }

            int magic = fileStream.ReadInt32();

            if (magic != Program.PCK_MAGIC)
            {
                //maybe at the end.... self contained exe
                fileStream.BaseStream.Seek(-4, SeekOrigin.End);
                magic = fileStream.ReadInt32();
                if (magic != Program.PCK_MAGIC)
                {
                    fileStream.Close();
                    Utils.ShowMessage("Not Godot PCK file!", "Error");
                    return false;
                }
                fileStream.BaseStream.Seek(-12, SeekOrigin.Current);

                long ds = fileStream.ReadInt64();
                fileStream.BaseStream.Seek(-ds - 8, SeekOrigin.Current);

                magic = fileStream.ReadInt32();
                if (magic != Program.PCK_MAGIC)
                {
                    fileStream.Close();
                    Utils.ShowMessage("Not Godot PCK file!", "Error");

                    return false;
                }
                else
                {
                    // If embedded PCK
                    PCK_Embedded = true;
                    PCK_StartPosition = fileStream.BaseStream.Position - 4;
                    PCK_EndPosition = fileStream.BaseStream.Length - 12;
                }
            }
            else
            {
                // If regular PCK
                PCK_StartPosition = 0;
                PCK_EndPosition = fileStream.BaseStream.Length;
            }

            PCK_VersionPack = fileStream.ReadInt32();
            PCK_VersionMajor = fileStream.ReadInt32();
            PCK_VersionMinor = fileStream.ReadInt32();
            PCK_VersionRevision = fileStream.ReadInt32();

            for (int i = 0; i < 16; i++)
            {
                //reserved
                fileStream.ReadInt32();
            }

            int file_count = fileStream.ReadInt32();

            for (int i = 0; i < file_count; i++)
            {
                string path = Encoding.UTF8.GetString(fileStream.ReadBytes(fileStream.ReadInt32())).Replace("\0", "");
                long ofs = fileStream.ReadInt64();
                long size = fileStream.ReadInt64();
                byte[] md5 = fileStream.ReadBytes(16);

                Files.Add(path, new PackedFile(fileStream, path, ofs, size, md5));
            };

            PackPath = p_path;
            return true;
        }

        public bool ExtractAllFiles(string folder, bool overwriteExisting = true)
        {
            return ExtractFiles(Files.Keys.ToList(), folder, overwriteExisting);
        }

        public bool ExtractFiles(List<string> names, string folder, bool overwriteExisting = true)
        {
            var bp = new BackgroundProgress();
            var bw = bp.backgroundWorker1;
            bool result = true;

            bw.DoWork += (sender, ev) =>
            {
                string basePath = folder;

                int count = 0;
                double one_file_in_progress_line = 1.0 / names.Count;
                foreach (var path in names)
                {
                    if (path != null)
                    {
                        PackedFile.VoidInt upd = (p) =>
                        {
                            bw.ReportProgress((int)(((double)count / names.Count * 100) + (p * one_file_in_progress_line)));
                        };
                        Files[path].OnProgress += upd;

                        if (!Files[path].ExtractFile(basePath, overwriteExisting))
                        {
                            Files[path].OnProgress -= upd;
                            result = false;
                            return;
                        }

                        Files[path].OnProgress -= upd;
                    }

                    count++;
                    bw.ReportProgress((int)((double)count / names.Count * 100));

                    if (bw.CancellationPending)
                    {
                        result = false;
                        return;
                    }
                }
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();

            if (result)
                Utils.ShowMessage("Completed!", "Progress");

            return result;
        }

        public bool RipPCKFileFromExe(string outPath)
        {
            if (!PCK_Embedded)
            {
                Utils.ShowMessage("The PCK file is not embedded.", "Error");
                return false;
            }

            var bp = new BackgroundProgress();
            var bw = bp.backgroundWorker1;
            bool result = true;

            bw.DoWork += (sender, ev) =>
            {
                string dir = Path.GetDirectoryName(outPath);
                BinaryWriter file;

                try
                {
                    if (File.Exists(outPath))
                        File.Delete(outPath);

                    Directory.CreateDirectory(dir);
                    file = new BinaryWriter(File.OpenWrite(outPath));
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    result = false;
                    return;
                }

                const int buf_max = 65536;

                try
                {
                    long size = PCK_EndPosition - PCK_StartPosition;
                    if (size > 0)
                    {
                        fileStream.BaseStream.Seek(PCK_StartPosition, SeekOrigin.Begin);
                        long to_write = size;

                        while (to_write > 0)
                        {
                            var read = fileStream.ReadBytes(Math.Min(buf_max, (int)to_write));
                            file.Write(read);
                            to_write -= read.Length;

                            bw.ReportProgress(100 - (int)((double)to_write / size * 100));

                            if (bw.CancellationPending)
                            {
                                result = false;
                                return;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    file.Close();
                    try
                    {
                        File.Delete(outPath);
                    }
                    catch { }
                    return;
                }

                // Update offsets
                file.Close();

                // TODO move first scan before cloning pck to get real size of file

                // First scan 
                BinaryReader fileReader;
                try
                {
                    fileReader = new BinaryReader(File.OpenRead(outPath));
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    result = false;
                    return;
                }

                // 21 = 1 Magic, 4 Version numbers + 16 reserved
                fileReader.BaseStream.Seek(21 * 4, SeekOrigin.Begin);
                var offsetMap = new Dictionary<long, long>();

                var filesCount = fileReader.ReadInt32();
                for (int i = 0; i < filesCount; i++)
                {
                    var strSize = fileReader.ReadInt32();
                    fileReader.BaseStream.Seek(strSize, SeekOrigin.Current);
                    offsetMap.Add(fileReader.BaseStream.Position, fileReader.ReadInt64());
                    // skip size and md5
                    fileReader.BaseStream.Seek(8 + 16, SeekOrigin.Current);
                }

                // Then modify
                fileReader.Close();

                try
                {
                    file = new BinaryWriter(File.OpenWrite(outPath));
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    result = false;
                    return;
                }

                foreach (var p in offsetMap)
                {
                    file.BaseStream.Seek(p.Key, SeekOrigin.Begin);
                    file.Write(p.Value - PCK_StartPosition);
                }

                file.Close();
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();

            if (result)
                Utils.ShowMessage("Completed!", "Progress");

            return result;
        }
    }
}
