using System;
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
        public int PCK_FileCount = -1;
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
            get => (PCK_Flags & PCKUtils.PCK_DIR_ENCRYPTED) != 0;
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
            PCK_FileCount = -1;
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
            PCKActions.progress?.Log("The package contains encrypted data. You need to specify the encryption key!");

            if (GetEncryptionKeyFunc != null)
                EncryptionKey = PCKUtils.HexStringToByteArray(GetEncryptionKeyFunc());

            if (EncryptionKey == null)
                throw new ArgumentNullException("Encryption key");

            if (EncryptionKey.Length != 256 / 8)
                throw new ArgumentOutOfRangeException("Encryption key");

            PCKActions.progress?.Log($"Got encryption key: {PCKUtils.ByteArrayToHexString(EncryptionKey)}");
        }

        public bool OpenFile(string p_path, bool show_not_pck_error = true, Func<string> get_encryption_key = null, bool log_names_progress = true, bool read_only_header_godot4 = false, CancellationToken? cancellationToken = null)
        {
            Close();

            try
            {
                p_path = Path.GetFullPath(p_path);
                binReader = new BinaryReader(File.OpenRead(p_path));
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                return false;
            }

            GetEncryptionKeyFunc = get_encryption_key;

            var op = "Open PCK";

            try
            {
                PCKActions.progress?.LogProgress(op, $"Opening: {p_path}");

                int magic = binReader.ReadInt32(); // 0-3

                if (magic != PCKUtils.PCK_MAGIC)
                {
                    //maybe at the end.... self contained exe
                    binReader.BaseStream.Seek(-4, SeekOrigin.End);
                    magic = binReader.ReadInt32();
                    if (magic != PCKUtils.PCK_MAGIC)
                    {
                        binReader.Close();
                        if (show_not_pck_error)
                            PCKActions.progress?.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);
                        return false;
                    }
                    binReader.BaseStream.Seek(-12, SeekOrigin.Current);

                    long ds = binReader.ReadInt64();
                    binReader.BaseStream.Seek(-ds - 8, SeekOrigin.Current);

                    magic = binReader.ReadInt32();
                    if (magic != PCKUtils.PCK_MAGIC)
                    {
                        binReader.Close();
                        if (show_not_pck_error)
                            PCKActions.progress?.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);

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
                }

                PCKActions.progress?.LogProgress(op, $"Version: {PCK_VersionPack}.{PCK_VersionMajor}.{PCK_VersionMinor}.{PCK_VersionRevision}, Flags: {PCK_Flags}");

                binReader.ReadBytes(16 * sizeof(int)); // 32-95 reserved

                PCK_FileCount = binReader.ReadInt32();
                PCKActions.progress?.LogProgress(op, $"File count: {PCK_FileCount}");

                if (read_only_header_godot4 && PCK_VersionPack == PCKUtils.PCK_VERSION_GODOT_4)
                {
                    PCKActions.progress?.LogProgress(op, "Completed without reading the file index!");
                    PackPath = p_path;
                    return true;
                }

                if (PCK_VersionPack == PCKUtils.PCK_VERSION_GODOT_4)
                {
                    if (IsEncrypted && (EncryptionKey == null || EncryptionKey.Length != 32))
                    {
                        TryGetEncryptionKey();
                    }
                }

                BinaryReader tmp_reader = binReader;

                if (IsEncrypted)
                    tmp_reader = PCKUtils.ReadEncryptedBlockIntoMemoryStream(binReader, EncryptionKey);

                for (int i = 0; i < PCK_FileCount; i++)
                {
                    int path_size = tmp_reader.ReadInt32();
                    string path = Encoding.UTF8.GetString(tmp_reader.ReadBytes(path_size)).Replace("\0", "");
                    long pos_of_ofs = tmp_reader.BaseStream.Position;
                    long ofs = tmp_reader.ReadInt64() + PCK_FileBase;
                    long size = tmp_reader.ReadInt64();
                    byte[] md5 = tmp_reader.ReadBytes(16);

                    int flags = 0;
                    if (PCK_VersionPack == PCKUtils.PCK_VERSION_GODOT_4)
                    {
                        flags = tmp_reader.ReadInt32();
                    }

                    if (log_names_progress)
                        PCKActions.progress?.LogProgress(op, $"{path} S: {size} F: {flags}");
                    Files.Add(path, new PackedFile(binReader, path, ofs, pos_of_ofs, size, md5, flags, PCK_VersionPack));
                };

                if (IsEncrypted)
                {
                    tmp_reader.Close();
                    tmp_reader.Dispose();
                }

                PCKActions.progress?.LogProgress(op, "Completed!");
                PackPath = p_path;
                return true;
            }
            catch (Exception ex)
            {
                binReader.Close();
                binReader = null;

                PCKActions.progress?.ShowMessage($"Can't read PCK file: {p_path}\n" + ex.Message, "Error", MessageType.Error);
                PCKActions.progress?.Log(ex.StackTrace);
                return false;
            }
            finally
            {
            }
        }

        public bool ExtractAllFiles(string folder, bool overwriteExisting = true, bool check_md5 = true, CancellationToken? cancellationToken = null)
        {
            return ExtractFiles(Files.Keys.ToList(), folder, overwriteExisting);
        }

        public bool ExtractFiles(IEnumerable<string> names, string folder, bool overwriteExisting = true, bool check_md5 = true, CancellationToken? cancellationToken = null)
        {
            var op = "Extract files";

            int files_count = names.Count();

            if (!names.Any())
            {
                PCKActions.progress?.CommandLog("The list of files to export is empty", "Error", MessageType.Error);
                return false;
            }

            try
            {
                PCKActions.progress?.LogProgress(op, "Started");
                PCKActions.progress?.LogProgress(op, $"Output folder: {folder}");

                string basePath = folder;

                int count = 0;
                double one_file_in_progress_line = 1.0 / files_count;

                foreach (var path in names)
                {
                    if (cancellationToken?.IsCancellationRequested ?? false)
                    {
                        return false;
                    }

                    if (path != null)
                    {
                        if (!Files.ContainsKey(path))
                        {
                            var res = PCKActions.progress?.ShowMessage($"File not found in PCK: {path}", "Error", MessageType.Error, PCKMessageBoxButtons.OKCancel);
                            if (res == PCKDialogResult.Cancel)
                                return false;

                            continue;
                        }

                        PCKActions.progress?.LogProgress(op, Files[path].FilePath);

                        PackedFile.VoidInt upd = (p) =>
                        {
                            PCKActions.progress?.LogProgress(op, (int)(((double)count / files_count * 100) + (p * one_file_in_progress_line)));
                        };
                        Files[path].OnProgress += upd;

                        if (Files[path].IsEncrypted && (EncryptionKey == null || EncryptionKey.Length != 32))
                            TryGetEncryptionKey();

                        if (!Files[path].ExtractFile(basePath, overwriteExisting, EncryptionKey, check_md5, cancellationToken))
                        {
                            Files[path].OnProgress -= upd;
                            return false;
                        }

                        Files[path].OnProgress -= upd;
                    }

                    count++;
                    PCKActions.progress?.LogProgress(op, (int)((double)count / files_count * 100));

                    if (cancellationToken?.IsCancellationRequested ?? false)
                    {
                        return false;
                    }
                }

                PCKActions.progress?.LogProgress(op, 100);
                PCKActions.progress?.LogProgress(op, "Completed!");
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

        public bool RipPCKFileFromExe(string outPath, CancellationToken? cancellationToken = null)
        {
            if (!PCK_Embedded)
            {
                PCKActions.progress?.ShowMessage("The PCK file is not embedded.", "Error", MessageType.Error);
                return false;
            }
            if (binReader == null)
            {
                PCKActions.progress?.ShowMessage("The PCK file is not open or has been closed.", "Error", MessageType.Error);
                return false;
            }

            var op = "Rip PCK file from EXE";


            try
            {
                PCKActions.progress?.LogProgress(op, "Started");

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
                    PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                    return false;
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
                            var read = binReader.ReadBytes(Math.Min(PCKUtils.BUFFER_MAX_SIZE, (int)to_write));
                            file.Write(read);
                            to_write -= read.Length;

                            PCKActions.progress?.LogProgress(op, 100 - (int)((double)to_write / size * 100));

                            if (cancellationToken?.IsCancellationRequested ?? false)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                    file.Close();
                    try
                    {
                        File.Delete(outPath);
                    }
                    catch { }
                    // TODO test?
                    return false;
                }

                // Fix addresses
                if (PCK_VersionPack < PCKUtils.PCK_VERSION_GODOT_4)
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

                PCKActions.progress?.LogProgress(op, 100);
                PCKActions.progress?.LogProgress(op, "Completed!");
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

        public bool MergePCKFileIntoExe(string exePath, CancellationToken? cancellationToken = null)
        {
            if (binReader == null)
            {
                PCKActions.progress?.ShowMessage("The PCK file is not open or has been closed.", "Error", MessageType.Error);
                return false;
            }

            var op = "Merge PCK into EXE";

            try
            {
                PCKActions.progress?.LogProgress(op, "Started");

                using (var p = new PCKReader())
                {
                    if (p.OpenFile(exePath, false, log_names_progress: false))
                    {
                        PCKActions.progress?.CommandLog("File already contains '.pck' inside.", "Error", MessageType.Error);
                        return false;
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
                    PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                    return false;
                }

                var embed_start = file.BaseStream.Position;

                // Ensure embedded PCK starts at a 64-bit multiple
                try
                {
                    PCKUtils.AddPadding(file, file.BaseStream.Position % 8);
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                    return false;
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
                            var read = binReader.ReadBytes(Math.Min(PCKUtils.BUFFER_MAX_SIZE, (int)to_write));
                            file.Write(read);
                            to_write -= read.Length;

                            PCKActions.progress?.LogProgress(op, 100 - (int)((double)to_write / size * 100));

                            if (cancellationToken?.IsCancellationRequested ?? false)
                            {
                                return false;
                            }
                        }

                        // Ensure embedded data ends at a 64-bit multiple
                        long embed_end = file.BaseStream.Position - embed_start + 12;
                        PCKUtils.AddPadding(file, embed_end % 8);

                        long pck_size = file.BaseStream.Position - pck_start;
                        file.Write((long)pck_size);
                        file.Write((int)PCKUtils.PCK_MAGIC);
                    }
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                    file.Close();
                    try
                    {
                        File.Delete(exePath);
                    }
                    catch { }
                    // TODO test?
                    return false;
                }

                // Fix addresses
                if (PCK_VersionPack < PCKUtils.PCK_VERSION_GODOT_4)
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

                PCKActions.progress?.LogProgress(op, 100);
                PCKActions.progress?.LogProgress(op, "Completed!");
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
    }
}
