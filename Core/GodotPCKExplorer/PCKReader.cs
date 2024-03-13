using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GodotPCKExplorer
{
    public struct PCKReaderEncryptionKeyResult
    {
        public string? Key;
        public byte[]? KeyBytes;
        public bool IsCancelled;
    }

    public sealed class PCKReader : IDisposable
    {
        BinaryReader? binReader = null;
        public Dictionary<string, PCKFile> Files = new Dictionary<string, PCKFile>();
        public string PackPath = "";

        public int PCK_FileCount = -1;
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

        public byte[]? ReceivedEncryptionKey { get; set; } = null;

        public PCKVersion PCK_Version { get { return new PCKVersion(PCK_VersionPack, PCK_VersionMajor, PCK_VersionMinor, PCK_VersionRevision); } }
        public bool IsOpened { get { return binReader != null; } }

        public bool IsEncrypted
        {
            get => IsEncryptedIndex || IsEncryptedFiles;
        }
        public bool IsEncryptedIndex
        {
            get => PCK_Flags != -1 && (PCK_Flags & PCKUtils.PCK_DIR_ENCRYPTED) != 0;
        }
        public bool IsEncryptedFiles
        {
            get => Files.Count != 0 && Files.First().Value.IsEncrypted;
        }

        ~PCKReader()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Close the PCK file and reset the cached values
        /// </summary>
        public void Close()
        {
            binReader?.Close();
            binReader = null;

            Files.Clear();
            PackPath = "";

            PCK_FileCount = -1;
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

            ReceivedEncryptionKey = null;
        }

        byte[]? TryGetEncryptionKey(string operation, Func<PCKReaderEncryptionKeyResult>? getEncryptionKeyFunc = null, bool disableExceptions = false)
        {
            PCKActions.progress?.LogProgress(operation, "The package contains encrypted data. You need to specify the encryption key!");
            byte[]? key = null;

            if (getEncryptionKeyFunc != null)
            {
                var res = getEncryptionKeyFunc();
                if (!res.IsCancelled)
                {
                    if (res.KeyBytes != null)
                    {
                        key = res.KeyBytes;
                    }
                    else
                    {
                        key = PCKUtils.HexStringToByteArray(res.Key);
                    }
                }
                else
                {
                    var errStr = "The encryption key retrieval was canceled.";
                    if (disableExceptions)
                    {
                        PCKActions.progress?.LogProgress(operation, errStr);
                        return null;
                    }
                    else
                        throw new OperationCanceledException(errStr);
                }
            }

            if (key == null)
            {
                var errStr = "The Encryption Key is null.";
                if (disableExceptions)
                {
                    PCKActions.progress?.LogProgress(operation, errStr);
                    return null;
                }
                else
                    throw new ArgumentNullException(errStr);
            }

            if (key.Length != 256 / 8)
            {
                var errStr = "The Encryption Key has the wrong format";
                if (disableExceptions)
                {
                    PCKActions.progress?.LogProgress(operation, errStr);
                    return null;
                }
                else
                    throw new ArgumentOutOfRangeException(errStr);
            }

            ReceivedEncryptionKey = key;
            PCKActions.progress?.LogProgress(operation, $"Got encryption key: {PCKUtils.ByteArrayToHexString(key)}");
            return key;
        }

        /// <summary>
        /// Open a PCK file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="show_NotPCKError">Show or hide errors that the selected file is not a PCK</param>
        /// <param name="readOnlyHeaderGodot4">In the PCK file for Godot 4, it is possible to read only index (list of contents).</param>
        /// <param name="logFileNamesProgress">Output a list of all the contents of the PCK file.</param>
        /// <param name="disableExceptions">Disable throwing exceptions, useful mainly for debugging.</param>
        /// <param name="getEncryptionKey">Function for obtaining the encryption key. It will be called only if the file has an encrypted index (list of contents).</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns></returns>
        public bool OpenFile(string path, bool show_NotPCKError = true, bool readOnlyHeaderGodot4 = false, bool logFileNamesProgress = true, bool disableExceptions = false, Func<PCKReaderEncryptionKeyResult>? getEncryptionKey = null, CancellationToken? cancellationToken = null)
        {
            BinaryReader reader;

            try
            {
                path = Path.GetFullPath(path);
                reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                return false;
            }

            return OpenFile(reader, show_NotPCKError, readOnlyHeaderGodot4, logFileNamesProgress, disableExceptions, getEncryptionKey, cancellationToken);
        }

        /// <summary>
        /// Open a PCK file
        /// </summary>
        /// <param name="fileReader">Any Binary Stream that contains a PCK file. The reading will occur from the current position of the stream.</param>
        /// <param name="show_NotPCKError">Show or hide errors that the selected file is not a PCK</param>
        /// <param name="readOnlyHeaderGodot4">In the PCK file for Godot 4, it is possible to read only index (list of contents).</param>
        /// <param name="logFileNamesProgress">Output a list of all the contents of the PCK file.</param>
        /// <param name="disableExceptions">Disable throwing exceptions, useful mainly for debugging.</param>
        /// <param name="getEncryptionKey">Function for obtaining the encryption key. It will be called only if the file has an encrypted index (list of contents).</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns></returns>
        public bool OpenFile(BinaryReader fileReader, bool show_NotPCKError = true, bool readOnlyHeaderGodot4 = false, bool logFileNamesProgress = true, bool disableExceptions = false, Func<PCKReaderEncryptionKeyResult>? getEncryptionKey = null, CancellationToken? cancellationToken = null)
        {
            var op = "Open PCK";

            Close();

            string file_path = "Data stream";

            if (fileReader == null)
            {
                PCKActions.progress?.ShowMessage("The File stream was null", "Error", MessageType.Error);
                return false;
            }

            if (fileReader.BaseStream is FileStream fs)
            {
                file_path = fs.Name;
            }

            try
            {
                PCKActions.progress?.LogProgress(op, $"Opening: {file_path}");
                PCKActions.progress?.LogProgress(op, PCKUtils.UnknownProgressStatus);

                int magic = fileReader.ReadInt32(); // 0-3

                if (magic != PCKUtils.PCK_MAGIC)
                {
                    //maybe at the end.... self contained exe
                    fileReader.BaseStream.Seek(-4, SeekOrigin.End);
                    magic = fileReader.ReadInt32();
                    if (magic != PCKUtils.PCK_MAGIC)
                    {
                        fileReader.Close();
                        if (show_NotPCKError)
                            PCKActions.progress?.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);
                        return false;
                    }
                    fileReader.BaseStream.Seek(-12, SeekOrigin.Current);

                    long ds = fileReader.ReadInt64();
                    fileReader.BaseStream.Seek(-ds - 8, SeekOrigin.Current);

                    magic = fileReader.ReadInt32();
                    if (magic != PCKUtils.PCK_MAGIC)
                    {
                        fileReader.Close();
                        if (show_NotPCKError)
                            PCKActions.progress?.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);

                        return false;
                    }
                    else
                    {
                        // If embedded PCK
                        PCK_Embedded = true;
                        PCK_StartPosition = fileReader.BaseStream.Position - 4;
                        PCK_EndPosition = fileReader.BaseStream.Length - 12;
                    }
                }
                else
                {
                    // If regular PCK
                    PCK_StartPosition = 0;
                    PCK_EndPosition = fileReader.BaseStream.Length;
                }

                PCK_VersionPack = fileReader.ReadInt32(); // 4-7
                PCK_VersionMajor = fileReader.ReadInt32(); // 8-11
                PCK_VersionMinor = fileReader.ReadInt32(); // 12-15
                PCK_VersionRevision = fileReader.ReadInt32(); // 16-19

                PCK_Flags = 0;
                PCK_FileBase = 0;

                if (PCK_VersionPack == PCKUtils.PCK_VERSION_GODOT_4)
                {
                    PCK_Flags = fileReader.ReadInt32(); // 20-23
                    PCK_FileBaseAddressOffset = fileReader.BaseStream.Position;
                    PCK_FileBase = fileReader.ReadInt64(); // 24-31
                }

                PCKActions.progress?.LogProgress(op, $"Version: {PCK_VersionPack}.{PCK_VersionMajor}.{PCK_VersionMinor}.{PCK_VersionRevision}, Flags: {PCK_Flags}");

                fileReader.ReadBytes(16 * sizeof(int)); // 32-95 reserved

                PCK_FileCount = fileReader.ReadInt32();
                PCKActions.progress?.LogProgress(op, $"File count: {PCK_FileCount}");

                if (readOnlyHeaderGodot4 && PCK_VersionPack == PCKUtils.PCK_VERSION_GODOT_4)
                {
                    PCKActions.progress?.LogProgress(op, 100);
                    PCKActions.progress?.LogProgress(op, "Completed without reading the file index!");
                    PackPath = file_path;
                    binReader = fileReader;
                    return true;
                }

                byte[]? encryption_key = null;
                if (PCK_VersionPack == PCKUtils.PCK_VERSION_GODOT_4)
                {
                    if (IsEncrypted)
                    {
                        // Throw Exception on error or return null
                        encryption_key = TryGetEncryptionKey(op, getEncryptionKey, disableExceptions);

                        if (encryption_key == null)
                        {
                            PCKActions.progress?.LogProgress(op, "No Encryption Key received. No further work with PCK is possible.");
                            return false;
                        }
                    }
                }

                BinaryReader tmp_reader = fileReader;

                if (IsEncryptedIndex)
                {
                    // Index should be read as a single buffer here
                    var mem = new MemoryStream();
                    using (var reader = new PCKEncryptedReader(fileReader, encryption_key ?? throw new NullReferenceException(nameof(encryption_key))))
                    {
                        foreach (var chunk in reader.ReadEncryptedBlocks())
                        {
                            mem.Write(chunk.Span);

                            if (cancellationToken?.IsCancellationRequested ?? false)
                            {
                                if (disableExceptions)
                                {
                                    PCKActions.progress?.LogProgress(op, "Operation Canceled.");
                                    return false;
                                }
                                else
                                    throw new OperationCanceledException();
                            }
                        }

                        // Test MD5 of decoded data
                        mem.Position = 0;
                        byte[] dec_md5;
                        using (var md5_crypto = MD5.Create())
                        {
                            dec_md5 = md5_crypto.ComputeHash(mem);
                        }

                        if (!reader.MD5.SequenceEqual(dec_md5))
                        {
                            if (disableExceptions)
                            {
                                PCKActions.progress?.LogProgress(op, "Operation Canceled. The decrypted index data has an incorrect MD5 hash sum.");
                                return false;
                            }
                            else
                                throw new CryptographicException("The decrypted index data has an incorrect MD5 hash sum.");
                        }
                    }
                    mem.Position = 0;
                    tmp_reader = new BinaryReader(mem);
                }

                List<PCKFile> tmp_files = new List<PCKFile>();
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

                    if (logFileNamesProgress)
                    {
                        PCKActions.progress?.LogProgress(op, $"{path}\nSize: {size} Flags: {flags}");
                        PCKActions.progress?.LogProgress(op, (int)(((double)i / PCK_FileCount) * 100));
                    }

                    tmp_files.Add(new PCKFile(fileReader, path, ofs, pos_of_ofs, size, md5, flags, PCK_VersionPack));

                    if (cancellationToken?.IsCancellationRequested ?? false)
                    {
                        if (disableExceptions)
                        {
                            PCKActions.progress?.LogProgress(op, "Operation Canceled");
                            return false;
                        }
                        else
                            throw new OperationCanceledException();
                    }
                };

                // In some rare cases, the PCK may contain duplicate files
                // So it is necessary to mark them as duplicates,
                // because those lower on the list will overwrite those higher up.
                {
                    Dictionary<string, int> duplicates = new Dictionary<string, int>();
                    for (int i = tmp_files.Count - 1; i >= 0; i--)
                    {
                        string tmp_path = tmp_files[i].FilePath;
                        if (!duplicates.ContainsKey(tmp_path))
                        {
                            duplicates.Add(tmp_path, 0);
                        }
                        else
                        {
                            duplicates[tmp_path]++;
                        }

                        if (duplicates[tmp_path] == 0)
                        {
                            continue;
                        }

                        string new_path = Path.ChangeExtension(tmp_path, $"duplicate_{duplicates[tmp_path]}" + Path.GetExtension(tmp_path));
                        tmp_files[i].FilePath = new_path;

                        if (logFileNamesProgress)
                        {
                            PCKActions.progress?.LogProgress(op, $"Duplicate file found. It will be renamed '{tmp_path}' -> '{new_path}'");
                        }
                    }

                    foreach (PCKFile file in tmp_files)
                    {
                        Files.Add(file.FilePath, file);
                    }
                }

                if (IsEncryptedIndex)
                {
                    tmp_reader.Close();
                    tmp_reader.Dispose();
                }

                PCKActions.progress?.LogProgress(op, "Completed!");
                PCKActions.progress?.LogProgress(op, 100);
                PackPath = file_path;
                binReader = fileReader;
                return true;
            }
            catch (Exception ex)
            {
                fileReader.Close();
                binReader = null;

                PCKActions.progress?.ShowMessage($"Can't read PCK file: {file_path}\n" + ex.Message, "Error", MessageType.Error);
                PCKActions.progress?.Log(ex);
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Extract files from an open PCK
        /// </summary>
        /// <param name="folder">Extraction Folder</param>
        /// <param name="overwriteExisting">Overwrite existing files</param>
        /// <param name="checkMD5">Whether to check the MD5 of exported files</param>
        /// <param name="getEncryptionKey">If the Encryption Key has not been received after opening the PCK, this function will be called. This function will also be called if the previous attempt to decrypt the file failed.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public bool ExtractAllFiles(string folder, bool overwriteExisting = true, bool checkMD5 = true, Func<PCKReaderEncryptionKeyResult>? getEncryptionKey = null, CancellationToken? cancellationToken = null)
        {
            return ExtractFiles(Files.Keys.ToList(), folder, overwriteExisting, checkMD5, getEncryptionKey, cancellationToken);
        }

        /// <summary>
        /// Extract files from an open PCK
        /// </summary>
        /// <param name="names">File names inside PCK</param>
        /// <param name="folder">Extraction Folder</param>
        /// <param name="overwriteExisting">Overwrite existing files</param>
        /// <param name="checkMD5">Whether to check the MD5 of exported files</param>
        /// <param name="getEncryptionKey">If the Encryption Key has not been received after opening the PCK, this function will be called. This function will also be called if the previous attempt to decrypt the file failed.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public bool ExtractFiles(IEnumerable<string> names, string folder, bool overwriteExisting = true, bool checkMD5 = true, Func<PCKReaderEncryptionKeyResult>? getEncryptionKey = null, CancellationToken? cancellationToken = null)
        {
            var op = "Extract files";

            int files_count = names.Count();

            if (!names.Any())
            {
                PCKActions.progress?.ShowMessage("The list of files to export is empty", "Error", MessageType.Error);
                return false;
            }

            try
            {
                PCKActions.progress?.LogProgress(op, "Started");
                PCKActions.progress?.LogProgress(op, $"Output folder: {folder}");

                string basePath = folder;
                byte[]? encryption_key = null;

                int count = 0;
                double one_file_in_progress_line = 1.0 / files_count;

                foreach (var path in names)
                {
                    if (cancellationToken?.IsCancellationRequested ?? false)
                        return false;

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

                        void upd(int p)
                        {
                            PCKActions.progress?.LogProgress(op, (int)(((double)count / files_count * 100) + (p * one_file_in_progress_line)));
                        }
                        Files[path].OnProgress += upd;

                        if (Files[path].IsEncrypted)
                        {
                            if (encryption_key == null)
                            {
                                if (ReceivedEncryptionKey == null)
                                {
                                    // Throw Exception on error or return null
                                    encryption_key = TryGetEncryptionKey(op, getEncryptionKey);

                                    if (encryption_key == null)
                                    {
                                        ReceivedEncryptionKey = null;
                                        PCKActions.progress?.LogProgress(op, "No Encryption Key received. No further work with PCK is possible.");
                                        return false;
                                    }
                                }
                                else
                                {
                                    PCKActions.progress?.LogProgress(op, $"A cached Encryption Key is used: {PCKUtils.ByteArrayToHexString(ReceivedEncryptionKey)}.");
                                    encryption_key = ReceivedEncryptionKey;
                                }
                            }
                        }

                        if (!Files[path].ExtractFile(basePath, overwriteExisting, encryption_key, checkMD5, cancellationToken))
                        {
                            ReceivedEncryptionKey = null;
                            Files[path].OnProgress -= upd;
                            return false;
                        }

                        Files[path].OnProgress -= upd;
                    }

                    count++;
                    PCKActions.progress?.LogProgress(op, (int)((double)count / files_count * 100));

                    if (cancellationToken?.IsCancellationRequested ?? false)
                        return false;
                }

                PCKActions.progress?.LogProgress(op, 100);
                PCKActions.progress?.LogProgress(op, "Completed!");
                return true;
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Create a copy of the currently opened PCK file as a separate file.
        /// </summary>
        /// <param name="outPath">Output file name</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
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
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
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
                                return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    file.Close();
                    try
                    {
                        File.Delete(outPath);
                    }
                    catch { }
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

        /// <summary>
        /// Merge the currently open PCK file into any other file.
        /// </summary>
        /// <param name="exePath">The path to any file. Usually <c>.exe</c> or unix binary file.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
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
                    if (p.OpenFile(exePath, false, logFileNamesProgress: false))
                    {
                        PCKActions.progress?.ShowMessage("File already contains '.pck' inside.", "Error", MessageType.Error);
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
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
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
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
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
                                return false;
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
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    file.Close();
                    try
                    {
                        File.Delete(exePath);
                    }
                    catch { }
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
