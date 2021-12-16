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
        
        public bool IsOpened { get { return fileStream != null; } }

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

        public bool OpenFile(string p_path, bool show_not_pck_error = true)
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
                    if (show_not_pck_error)
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
                    if (show_not_pck_error)
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
                long ofs_p = fileStream.BaseStream.Position;
                long ofs = fileStream.ReadInt64();
                long size = fileStream.ReadInt64();
                byte[] md5 = fileStream.ReadBytes(16);

                Files.Add(path, new PackedFile(fileStream, path, ofs, ofs_p, size, md5));
            };

            PackPath = p_path;
            return true;
        }

        public bool ExtractAllFiles(string folder, bool overwriteExisting = true)
        {
            return ExtractFiles(Files.Keys.ToList(), folder, overwriteExisting);
        }

        public bool ExtractFiles(IEnumerable<string> names, string folder, bool overwriteExisting = true)
        {
            var bp = new BackgroundProgress();
            var bw = bp.backgroundWorker1;
            bool result = true;
            int files_count = names.Count();

            if (!names.Any())
            {
                Utils.CommandLog("The list of files to export is empty", "Error", false);
                return false;
            }

            bw.DoWork += (sender, ev) =>
            {
                string basePath = folder;

                int count = 0;
                double one_file_in_progress_line = 1.0 / files_count;
                foreach (var path in names)
                {
                    if (path != null)
                    {
                        if (!Files.ContainsKey(path))
                        {
                            Utils.CommandLog($"File not found in PCK: {path}", "Error", false);
                            continue;
                        }

                        PackedFile.VoidInt upd = (p) =>
                        {
                            bw.ReportProgress((int)(((double)count / files_count * 100) + (p * one_file_in_progress_line)));
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
                    bw.ReportProgress((int)((double)count / files_count * 100));

                    if (bw.CancellationPending)
                    {
                        result = false;
                        return;
                    }
                }
                bw.ReportProgress(100);
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

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    file = new BinaryWriter(File.OpenWrite(outPath));
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error"); result = false; return;
                }

                const int buf_max = 65536;
                long size = PCK_EndPosition - PCK_StartPosition;

                try
                {
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

                foreach (var p in Files.Values)
                {
                    file.BaseStream.Seek(p.OffsetPosition - PCK_StartPosition, SeekOrigin.Begin);
                    file.Write(p.Offset - PCK_StartPosition);
                }

                file.Close();
                bw.ReportProgress(100);
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();

            if (result)
                Utils.ShowMessage("Completed!", "Progress");

            return result;
        }

        public bool MergePCKFileIntoExe(string exePath)
        {
            var bp = new BackgroundProgress();
            var bw = bp.backgroundWorker1;
            bool result = true;

            bw.DoWork += (sender, ev) =>
            {
                BinaryWriter file;

                {
                    Console.WriteLine("Checking file whether it already contains '.pck'");
                    var p = new PCKReader();
                    if (p.OpenFile(exePath, false))
                    {
                        p.Close();
                        Utils.CommandLog("File already contains '.pck' inside.", "Error", false);
                        result = false;
                        return;
                    }
                    p.Close();
                }

                try
                {
                    file = new BinaryWriter(File.OpenWrite(exePath));
                    file.BaseStream.Seek(0, SeekOrigin.End);
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error"); result = false; return;
                }

                var embed_start = file.BaseStream.Position;

                // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:994
                // Ensure embedded PCK starts at a 64-bit multiple
                try
                {
                    int pad = (int)(file.BaseStream.Position % 8);
                    for (int i = 0; i < pad; i++)
                    {
                        file.Write((byte)0);
                    }
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error"); result = false; return;
                }

                const int buf_max = 65536;
                long pck_start = file.BaseStream.Position;
                long size = PCK_EndPosition - PCK_StartPosition;

                long offset_delta = pck_start - PCK_StartPosition;

                try
                {
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

                        // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:1073
                        // Ensure embedded data ends at a 64-bit multiple
                        long embed_end = file.BaseStream.Position - embed_start + 12;
                        long pad = embed_end % 8;
                        for (long i = 0; i < pad; i++)
                        {
                            file.Write((byte)0);
                        }

                        long pck_size = file.BaseStream.Position - pck_start;
                        file.Write((long)pck_size);
                        file.Write((int)Program.PCK_MAGIC);
                    }
                }
                catch (Exception e)
                {
                    Utils.ShowMessage(e.Message, "Error");
                    file.Close();
                    try
                    {
                        File.Delete(exePath);
                    }
                    catch { }
                    return;
                }


                foreach (var p in Files.Values)
                {
                    file.BaseStream.Seek(p.OffsetPosition + offset_delta, SeekOrigin.Begin);
                    file.Write(p.Offset + offset_delta);
                }

                file.Close();
                bw.ReportProgress(100);
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();

            if (result)
                Utils.ShowMessage("Completed!", "Progress");

            return result;
        }
    }
}
