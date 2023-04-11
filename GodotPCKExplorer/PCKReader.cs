﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GodotPCKExplorer
{
    public class PCKReader : IDisposable
    {
        BinaryReader binReader = null;
        public Dictionary<string, PackedFile> Files = new Dictionary<string, PackedFile>();
        public string PackPath = "";
        public byte[] EncryptionKey = null;

        public int PCK_VersionPack = -1;
        public int PCK_VersionMajor = -1;
        public int PCK_VersionMinor = -1;
        public int PCK_VersionRevision = -1;
        public int PCK_Flags = -1;
        public long PCK_FileBase = 0;
        public long PCK_FileBaseAddressOffset = 0;
        public long PCK_StartPosition = 0;
        public long PCK_EndPosition = 0;
        public bool PCK_Embedded = false;

        Func<string> GetEncryptionKeyFunc = null;

        public PCKVersion PCK_Version { get { return new PCKVersion(PCK_VersionPack, PCK_VersionMajor, PCK_VersionMinor, PCK_VersionRevision); } }
        public bool IsOpened { get { return binReader != null; } }
        public bool IsEncrypted
        {
            get => (PCK_Flags & Utils.PCK_DIR_ENCRYPTED) != 0;
        }

        ~PCKReader()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            binReader?.Close();
            binReader = null;

            Files.Clear();
            PackPath = "";
            EncryptionKey = null;

            PCK_VersionPack = -1;
            PCK_VersionMajor = -1;
            PCK_VersionMinor = -1;
            PCK_VersionRevision = -1;
            PCK_Flags = -1;
            PCK_FileBaseAddressOffset = 0;
            PCK_FileBase = 0;
            PCK_StartPosition = 0;
            PCK_EndPosition = 0;
            PCK_Embedded = false;

            GetEncryptionKeyFunc = null;
        }

        void TryGetEncryptionKey()
        {
            // TODO: add test for this key before use!

            if (GetEncryptionKeyFunc != null)
                EncryptionKey = Utils.HexStringToByteArray(GetEncryptionKeyFunc());
            Program.Log($"Got encryption key: {Utils.ByteArrayToHexString(EncryptionKey)}");
        }

        public bool OpenFile(string p_path, bool show_not_pck_error = true, Func<string> get_encryption_key = null)
        {
            Close();

            try
            {
                p_path = Path.GetFullPath(p_path);
                binReader = new BinaryReader(File.OpenRead(p_path));
            }
            catch (Exception ex)
            {
                Program.ShowMessage(ex, "Error", MessageType.Error);
                return false;
            }

            GetEncryptionKeyFunc = get_encryption_key;

            try
            {
                int magic = binReader.ReadInt32(); // 0-3

                if (magic != Utils.PCK_MAGIC)
                {
                    //maybe at the end.... self contained exe
                    binReader.BaseStream.Seek(-4, SeekOrigin.End);
                    magic = binReader.ReadInt32();
                    if (magic != Utils.PCK_MAGIC)
                    {
                        binReader.Close();
                        if (show_not_pck_error)
                            Program.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);
                        return false;
                    }
                    binReader.BaseStream.Seek(-12, SeekOrigin.Current);

                    long ds = binReader.ReadInt64();
                    binReader.BaseStream.Seek(-ds - 8, SeekOrigin.Current);

                    magic = binReader.ReadInt32();
                    if (magic != Utils.PCK_MAGIC)
                    {
                        binReader.Close();
                        if (show_not_pck_error)
                            Program.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);

                        return false;
                    }
                    else
                    {
                        // If embedded PCK
                        PCK_Embedded = true;
                        PCK_StartPosition = binReader.BaseStream.Position - 4;
                        PCK_EndPosition = binReader.BaseStream.Length - 12;
                    }
                }
                else
                {
                    // If regular PCK
                    PCK_StartPosition = 0;
                    PCK_EndPosition = binReader.BaseStream.Length;
                }

                PCK_VersionPack = binReader.ReadInt32(); // 4-7
                PCK_VersionMajor = binReader.ReadInt32(); // 8-11
                PCK_VersionMinor = binReader.ReadInt32(); // 12-15
                PCK_VersionRevision = binReader.ReadInt32(); // 16-19

                PCK_Flags = 0;
                PCK_FileBase = 0;
                if (PCK_VersionPack == 2)
                {
                    PCK_Flags = binReader.ReadInt32(); // 20-23
                    PCK_FileBaseAddressOffset = binReader.BaseStream.Position;
                    PCK_FileBase = binReader.ReadInt64(); // 24-31

                    if (IsEncrypted && (EncryptionKey == null || EncryptionKey.Length != 32))
                    {
                        TryGetEncryptionKey();
                    }
                }

                binReader.ReadBytes(16 * sizeof(int)); // 32-95 reserved

                int file_count = binReader.ReadInt32();

                BinaryReader tmp_reader = binReader;

                if (IsEncrypted)
                    tmp_reader = Utils.ReadEncryptedBlockIntoMemoryStream(binReader, EncryptionKey);

                for (int i = 0; i < file_count; i++)
                {
                    int path_size = tmp_reader.ReadInt32();
                    string path = Encoding.UTF8.GetString(tmp_reader.ReadBytes(path_size)).Replace("\0", "");
                    long pos_of_ofs = tmp_reader.BaseStream.Position;
                    long ofs = tmp_reader.ReadInt64() + PCK_FileBase;
                    long size = tmp_reader.ReadInt64();
                    byte[] md5 = tmp_reader.ReadBytes(16);

                    int flags = 0;
                    if (PCK_VersionPack == 2)
                    {
                        flags = tmp_reader.ReadInt32();
                    }

                    Files.Add(path, new PackedFile(binReader, path, ofs, pos_of_ofs, size, md5, flags));
                };

                if (IsEncrypted)
                {
                    tmp_reader.Close();
                    tmp_reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                binReader.Close();
                binReader = null;

                Program.ShowMessage($"Can't read PCK file: {p_path}\n" + ex.Message, "Error", MessageType.Error);
                Program.Log(ex.StackTrace);
                return false;
            }

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
            var bw = bp.bg_worker;
            var are = new AutoResetEvent(false);

            bool result = true;
            int files_count = names.Count();

            if (!names.Any())
            {
                Program.CommandLog("The list of files to export is empty", "Error", false, MessageType.Error);
                return false;
            }

            bw.DoWork += (sender, ev) =>
            {
                try
                {
                    string basePath = folder;

                    int count = 0;
                    double one_file_in_progress_line = 1.0 / files_count;

                    foreach (var path in names)
                    {
                        if (bw.CancellationPending)
                        {
                            result = false;
                            return;
                        }

                        if (path != null)
                        {
                            if (!Files.ContainsKey(path))
                            {
                                var res = Program.ShowMessage($"File not found in PCK: {path}", "Error", MessageType.Error, System.Windows.Forms.MessageBoxButtons.OKCancel);
                                if (res == System.Windows.Forms.DialogResult.Cancel)
                                    bw.CancelAsync();

                                continue;
                            }

                            PackedFile.VoidInt upd = (p) =>
                            {
                                bw.ReportProgress((int)(((double)count / files_count * 100) + (p * one_file_in_progress_line)));
                            };
                            Files[path].OnProgress += upd;

                            if (Files[path].IsEncrypted && (EncryptionKey == null || EncryptionKey.Length != 32))
                                TryGetEncryptionKey();

                            if (!Files[path].ExtractFile(basePath, overwriteExisting, bw, EncryptionKey))
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
                }
                finally
                {
                    are.Set();
                }
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();
            are.WaitOne();

            return result;
        }

        public bool RipPCKFileFromExe(string outPath)
        {
            if (!PCK_Embedded)
            {
                Program.ShowMessage("The PCK file is not embedded.", "Error", MessageType.Error);
                return false;
            }

            var bp = new BackgroundProgress();
            var bw = bp.bg_worker;
            var are = new AutoResetEvent(false);
            bool result = true;

            bw.DoWork += (sender, ev) =>
            {
                try
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
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        result = false;
                        return;
                    }

                    long size = PCK_EndPosition - PCK_StartPosition;

                    try
                    {
                        if (size > 0)
                        {
                            binReader.BaseStream.Seek(PCK_StartPosition, SeekOrigin.Begin);
                            long to_write = size;

                            while (to_write > 0)
                            {
                                var read = binReader.ReadBytes(Math.Min(Utils.BUFFER_MAX_SIZE, (int)to_write));
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
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        file.Close();
                        try
                        {
                            File.Delete(outPath);
                        }
                        catch { }
                        return;
                    }

                    // Fix addresses
                    if (PCK_VersionPack < 2)
                    {
                        foreach (var p in Files.Values)
                        {
                            file.BaseStream.Seek(p.PositionOfOffsetValue - PCK_StartPosition, SeekOrigin.Begin);
                            file.Write(p.Offset - PCK_StartPosition);
                        }
                    }
                    else
                    {
                        file.BaseStream.Seek(PCK_FileBaseAddressOffset - PCK_StartPosition, SeekOrigin.Begin);
                        file.Write(PCK_FileBase - PCK_StartPosition);
                    }

                    file.Close();
                    bw.ReportProgress(100);
                }
                finally
                {
                    are.Set();
                }
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();
            are.WaitOne();

            return result;
        }

        public bool MergePCKFileIntoExe(string exePath)
        {
            var bp = new BackgroundProgress();
            var bw = bp.bg_worker;
            var are = new AutoResetEvent(false);
            bool result = true;

            bw.DoWork += (sender, ev) =>
            {
                try
                {
                    using (var p = new PCKReader())
                    {
                        if (p.OpenFile(exePath, false))
                        {
                            Program.CommandLog("File already contains '.pck' inside.", "Error", false, MessageType.Error);
                            result = false;
                            return;
                        }
                    }

                    BinaryWriter file;
                    try
                    {
                        file = new BinaryWriter(File.OpenWrite(exePath));
                        file.BaseStream.Seek(0, SeekOrigin.End);
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        result = false;
                        return;
                    }

                    var embed_start = file.BaseStream.Position;

                    // Ensure embedded PCK starts at a 64-bit multiple
                    try
                    {
                        Utils.AddPadding(file, file.BaseStream.Position % 8);
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        result = false;
                        return;
                    }

                    long pck_start = file.BaseStream.Position;
                    long size = PCK_EndPosition - PCK_StartPosition;

                    long offset_delta = pck_start - PCK_StartPosition;

                    try
                    {
                        if (size > 0)
                        {
                            binReader.BaseStream.Seek(PCK_StartPosition, SeekOrigin.Begin);
                            long to_write = size;

                            while (to_write > 0)
                            {
                                var read = binReader.ReadBytes(Math.Min(Utils.BUFFER_MAX_SIZE, (int)to_write));
                                file.Write(read);
                                to_write -= read.Length;

                                bw.ReportProgress(100 - (int)((double)to_write / size * 100));

                                if (bw.CancellationPending)
                                {
                                    result = false;
                                    return;
                                }
                            }

                            // Ensure embedded data ends at a 64-bit multiple
                            long embed_end = file.BaseStream.Position - embed_start + 12;
                            Utils.AddPadding(file, embed_end % 8);

                            long pck_size = file.BaseStream.Position - pck_start;
                            file.Write((long)pck_size);
                            file.Write((int)Utils.PCK_MAGIC);
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.ShowMessage(ex, "Error", MessageType.Error);
                        file.Close();
                        try
                        {
                            File.Delete(exePath);
                        }
                        catch { }
                        return;
                    }

                    // Fix addresses
                    if (PCK_VersionPack < 2)
                    {
                        foreach (var p in Files.Values)
                        {
                            file.BaseStream.Seek(p.PositionOfOffsetValue + offset_delta, SeekOrigin.Begin);
                            file.Write(p.Offset + offset_delta);
                        }
                    }
                    else
                    {
                        file.BaseStream.Seek(embed_start + PCK_FileBaseAddressOffset - PCK_StartPosition, SeekOrigin.Begin);
                        file.Write(embed_start + PCK_FileBase);
                    }

                    file.Close();
                    bw.ReportProgress(100);
                }
                finally
                {
                    are.Set();
                }
            };

            bw.RunWorkerAsync();
            bp.ShowDialog();
            are.WaitOne();

            return result;
        }
    }
}
