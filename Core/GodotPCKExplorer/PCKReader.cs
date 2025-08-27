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

    public enum PCKExtractNoEncryptionKeyMode
    {
        /// <summary>
        /// Cancel the extraction when encrypted data is encountered
        /// </summary>
        Cancel,
        /// <summary>
        /// Skip encrypted files
        /// </summary>
        Skip,
        /// <summary>
        /// Extract as is, without decryption
        /// </summary>
        AsIs
    }

    public sealed class PCKReader : IDisposable
    {
        BinaryReader? binReader = null;
        public Dictionary<string, PCKReaderFile> Files = new Dictionary<string, PCKReaderFile>();
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
        public bool PCK_ContainsEncryptedFiles = false;
        public bool PCK_ContainsRemovalFiles = false;

        public byte[]? ReceivedEncryptionKey { get; private set; } = null;

        public PCKVersion PCK_Version { get; private set; } = new PCKVersion(-1, -1, -1, -1);
        public bool IsOpened { get { return binReader != null; } }

        public bool IsRelativeFileBase
        {
            get => PCK_Flags != -1 && (PCK_Flags & (int)PCKUtils.PCK_FLAG.REL_FILEBASE) != 0;
        }

        public bool IsEncrypted
        {
            get => IsEncryptedIndex || IsEncryptedFiles;
        }
        public bool IsEncryptedIndex
        {
            get => PCK_Flags != -1 && (PCK_Flags & (int)PCKUtils.PCK_FLAG.DIR_ENCRYPTED) != 0;
        }
        public bool IsEncryptedFiles
        {
            get => PCK_ContainsEncryptedFiles;
        }
        public bool IsRemovalFiles
        {
            get => PCK_ContainsRemovalFiles;
        }
        public BinaryReader? ReaderStream
        {
            get => binReader;
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
            PCK_Version = new PCKVersion(-1, -1, -1, -1);
            PCK_Flags = -1;
            PCK_FileBaseAddressOffset = 0;
            PCK_FileBase = 0;
            PCK_StartPosition = 0;
            PCK_EndPosition = 0;
            PCK_Embedded = false;
            PCK_ContainsEncryptedFiles = false;
            PCK_ContainsRemovalFiles = false;

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
                PCKActions.progress?.LogProgress(operation, errStr);
                return null;
                /*
                if (disableExceptions)
                {
                    PCKActions.progress?.LogProgress(operation, errStr);
                    return null;
                }
                else
                    throw new ArgumentNullException(errStr);
                */
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
            BinaryReader? reader = null;

            try
            {
                path = Path.GetFullPath(path);
                reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch (Exception ex)
            {
                reader?.Dispose();
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                return false;
            }

            var res = OpenFile(reader, show_NotPCKError, readOnlyHeaderGodot4, logFileNamesProgress, disableExceptions, getEncryptionKey, cancellationToken);
            if (!res)
                reader?.Dispose();

            return res;
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

                PCK_Version = new PCKVersion(PCK_VersionPack, PCK_VersionMajor, PCK_VersionMinor, PCK_VersionRevision);

                PCK_Flags = 0;
                PCK_FileBase = 0;

                if (PCK_VersionPack == (int)PCKUtils.PACK_VERSION.Godot_4)
                {
                    PCK_Flags = fileReader.ReadInt32(); // 20-23
                    PCK_FileBaseAddressOffset = fileReader.BaseStream.Position;
                    PCK_FileBase = fileReader.ReadInt64(); // 24-31

                    if (IsRelativeFileBase)
                    {
                        PCK_FileBase += PCK_StartPosition;
                    }
                }

                PCKActions.progress?.LogProgress(op, $"Version: {PCK_VersionPack}.{PCK_VersionMajor}.{PCK_VersionMinor}.{PCK_VersionRevision}, Flags: {PCK_Flags}");

                fileReader.ReadBytes(16 * sizeof(int)); // 32-95 reserved

                PCK_FileCount = fileReader.ReadInt32();
                PCKActions.progress?.LogProgress(op, $"File count: {PCK_FileCount}");

                if (readOnlyHeaderGodot4 && PCK_VersionPack == (int)PCKUtils.PACK_VERSION.Godot_4)
                {
                    PCKActions.progress?.LogProgress(op, 100);
                    PCKActions.progress?.LogProgress(op, "Completed without reading the file index!");
                    PackPath = file_path;
                    binReader = fileReader;
                    return true;
                }

                byte[]? encryption_key = null;
                if (PCK_VersionPack == (int)PCKUtils.PACK_VERSION.Godot_4)
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
                    using (var reader = new PCKEncryptedFileReader(fileReader, encryption_key ?? throw new NullReferenceException(nameof(encryption_key))))
                    {
                        foreach (var chunk in reader.ReadDencryptedBlocks())
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
                                {
                                    throw new OperationCanceledException();
                                }
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
                            {
                                throw new CryptographicException("The decrypted index data has an incorrect MD5 hash sum.");
                            }
                        }
                    }
                    mem.Position = 0;
                    tmp_reader = new BinaryReader(mem);
                }

                List<PCKReaderFile> tmp_files = new List<PCKReaderFile>();
                for (int i = 0; i < PCK_FileCount; i++)
                {
                    int path_size = tmp_reader.ReadInt32();
                    string path = Encoding.UTF8.GetString(tmp_reader.ReadBytes(path_size)).Replace("\0", "");
                    long pos_of_ofs = tmp_reader.BaseStream.Position;
                    long ofs = tmp_reader.ReadInt64() + PCK_FileBase;
                    long size = tmp_reader.ReadInt64();
                    byte[] md5 = tmp_reader.ReadBytes(16);

                    int flags = 0;
                    if (PCK_VersionPack == (int)PCKUtils.PACK_VERSION.Godot_4)
                    {
                        flags = tmp_reader.ReadInt32();
                    }

                    if (logFileNamesProgress)
                    {
                        PCKActions.progress?.LogProgress(op, $"{path}\nSize: {size} Flags: {flags}");
                        PCKActions.progress?.LogProgress(op, (int)(((double)i / PCK_FileCount) * 100));
                    }

                    tmp_files.Add(new PCKReaderFile(fileReader, path, ofs, pos_of_ofs, size, md5, flags, PCK_Version));

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
                }

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

                    foreach (PCKReaderFile file in tmp_files)
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
                PCK_ContainsEncryptedFiles = Files.Any((f) => f.Value.IsEncrypted);
                PCK_ContainsRemovalFiles = Files.Any((f) => f.Value.IsRemoval);
                return true;
            }
            catch (Exception ex)
            {
                fileReader.Close();
                binReader = null;
                Close();

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
        public bool ExtractAllFiles(out List<string> extractedFiles, out List<string> failedFiles, string folder, bool overwriteExisting = true, bool checkMD5 = true, Func<PCKReaderEncryptionKeyResult>? getEncryptionKey = null, PCKExtractNoEncryptionKeyMode noKeyMode = PCKExtractNoEncryptionKeyMode.Cancel, CancellationToken? cancellationToken = null)
        {
            return ExtractFiles(Files.Keys.ToList(), out extractedFiles, out failedFiles, folder, overwriteExisting, checkMD5, getEncryptionKey, noKeyMode, cancellationToken);
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
        public bool ExtractFiles(IEnumerable<string> names, out List<string> extractedFiles, out List<string> failedFiles, string folder, bool overwriteExisting = true, bool checkMD5 = true, Func<PCKReaderEncryptionKeyResult>? getEncryptionKey = null, PCKExtractNoEncryptionKeyMode noKeyMode = PCKExtractNoEncryptionKeyMode.Cancel, CancellationToken? cancellationToken = null)
        {
            var start_time = DateTime.UtcNow;

            var op = "Extract files";
            extractedFiles = new List<string>();
            failedFiles = new List<string>();

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

                if (IsEncryptedFiles && names.Any(name => Files.TryGetValue(name, out var file) && file.IsEncrypted))
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

                                switch (noKeyMode)
                                {
                                    case PCKExtractNoEncryptionKeyMode.Cancel:
                                        // Add all of the following files as failed
                                        failedFiles.AddRange(names.Skip(count));
                                        PCKActions.progress?.ShowMessage("No Encryption Key received.\nNo further work with PCK is possible.", "Error", MessageType.Error, PCKMessageBoxButtons.OK);
                                        return false;
                                    case PCKExtractNoEncryptionKeyMode.Skip:
                                        PCKActions.progress?.LogProgress(op, "No Encryption Key received. All encrypted files will be skipped.");
                                        break;
                                    case PCKExtractNoEncryptionKeyMode.AsIs:
                                        PCKActions.progress?.LogProgress(op, "No Encryption Key received. All encrypted files will be extracted without decryption.");
                                        break;
                                    default:
                                        throw new NotImplementedException("Invalid 'no key' mode!");
                                }
                            }
                        }
                        else
                        {
                            PCKActions.progress?.LogProgress(op, $"A cached Encryption Key is used: {PCKUtils.ByteArrayToHexString(ReceivedEncryptionKey)}.");
                            encryption_key = ReceivedEncryptionKey;
                        }
                    }
                }

                // TODO Multithreading would greatly speed up extraction.
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

                        PCKReaderFile file = Files[path];
                        PCKActions.progress?.LogProgress(op, file.FilePath);

                        void upd(int p)
                        {
                            PCKActions.progress?.LogProgress(op, (int)(((double)count / files_count * 100) + (p * one_file_in_progress_line)));
                        }
                        file.OnProgress += upd;

                        if (file.ExtractFile(basePath, out string extractedPath, out bool skippedExisted, overwriteExisting, encryption_key, noKeyMode, cancellationToken))
                        {
                            if (checkMD5 && !skippedExisted)
                            {
                                if (file.CheckMD5(extractedPath))
                                {
                                    extractedFiles.Add(extractedPath);
                                }
                                else
                                {
                                    try
                                    {
                                        File.Delete(extractedPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        PCKActions.progress?.LogProgress(op, $"Failed to delete a file with an incorrect MD5: {ex.Message}");
                                    }

                                    failedFiles.AddRange(names.Skip(count));
                                    ReceivedEncryptionKey = null;
                                    file.OnProgress -= upd;
                                    return false;
                                }
                            }
                            else
                            {
                                extractedFiles.Add(extractedPath);
                            }
                        }
                        else
                        {
                            if (!file.IsEncrypted || (file.IsEncrypted && noKeyMode == PCKExtractNoEncryptionKeyMode.Cancel))
                            {
                                failedFiles.AddRange(names.Skip(count));
                                ReceivedEncryptionKey = null;
                                file.OnProgress -= upd;
                                return false;
                            }
                            else
                            {
                                failedFiles.Add(path);
                            }
                        }

                        file.OnProgress -= upd;
                    }

                    count++;
                    PCKActions.progress?.LogProgress(op, (int)((double)count / files_count * 100));

                    if (cancellationToken?.IsCancellationRequested ?? false)
                        return false;
                }

                PCKActions.progress?.LogProgress(op, 100);
                PCKActions.progress?.LogProgress(op, $"Completed!  Time spent: {(DateTime.UtcNow - start_time).TotalSeconds:F2}s.");
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
                    long written = 0;
                    foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(binReader.BaseStream, PCK_StartPosition, PCK_EndPosition))
                    {
                        file.Write(block.Span);
                        written += block.Length;
                        PCKActions.progress?.LogProgress(op, (int)((double)written / size * 100));

                        if (cancellationToken?.IsCancellationRequested ?? false)
                            return false;
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
                if (PCK_VersionPack < (int)PCKUtils.PACK_VERSION.Godot_4)
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

            BinaryWriter? file = null;
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
                    long written = 0;
                    foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(binReader.BaseStream, PCK_StartPosition, PCK_EndPosition))
                    {
                        file.Write(block.Span);
                        written += block.Length;
                        PCKActions.progress?.LogProgress(op, (int)((double)written / size * 100));

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
                if (PCK_VersionPack < (int)PCKUtils.PACK_VERSION.Godot_4)
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
                file?.Dispose();
            }
        }
    }

    public sealed class PCKReaderFile
    {
        internal readonly BinaryReader StreamReader;
        /// <summary>
        /// The name of the file in the package hierarchy.
        /// </summary>
        public string FilePath;
        /// <summary>
        /// File offset inside the package.
        /// </summary>
        public long Offset;
        /// <summary>
        /// Required for manipulating addresses.
        /// </summary>
        public long PositionOfOffsetValue;
        /// <summary>
        /// Original file size.
        /// </summary>
        public long Size;
        /// <summary>
        /// Actual file size inside the package. Including encrypted header.
        /// This size is calculated at the first request.
        /// </summary>
        public long ActualSize
        {
            get
            {
                if (_actualSize == -2)
                {
                    if (IsEncrypted)
                    {   // PCKEncryptedFileReader moves the position of the stream, so it needs to be moved back
                        long pos = StreamReader.BaseStream.Position;
                        using var er = new PCKEncryptedFileReader(this, new byte[0]);
                        StreamReader.BaseStream.Seek(pos, SeekOrigin.Begin);
                        _actualSize = er.TotalFileSize;
                    }
                    else
                    {
                        _actualSize = Size;
                    }
                }

                return _actualSize;
            }
        }
        long _actualSize = -2;
        /// <summary>
        /// Hash to check the correctness of the file.
        /// </summary>
        public byte[] MD5;
        /// <summary>
        /// Individual file flags.
        /// Now only: 0 or 1 (Encrypted).
        /// </summary>
        public int Flags;
        /// <summary>
        /// Pack Version
        /// </summary>
        public PCKVersion Version;

        public PCKReaderFile(BinaryReader reader, string path, long contentOffset, long positionOfOffsetValue, long size, byte[] MD5, int flags, PCKVersion pack_version)
        {
            this.StreamReader = reader;
            FilePath = path;
            Offset = contentOffset;
            PositionOfOffsetValue = positionOfOffsetValue;
            Size = size;
            this.MD5 = MD5;
            Flags = flags;

            Version = pack_version;
        }

        public delegate void VoidInt(int progress);
        public event VoidInt? OnProgress;

        public bool IsEncrypted
        {
            get => (Flags & (int)PCKUtils.PCK_FILE.FLAG_ENCRYPTED) != 0;
        }

        public bool IsRemoval
        {
            get
            {
                if (Version.Major >= 4 && Version.Minor >= 4)
                {
                    return (Flags & (int)PCKUtils.PCK_FILE.FLAG_REMOVAL) != 0;
                }

                return false;
            }
        }

        public bool ExtractFile(string basePath, out string extractPath, out bool skippedExisted, bool overwriteExisting = true, byte[]? encKey = null, PCKExtractNoEncryptionKeyMode noKeyMode = PCKExtractNoEncryptionKeyMode.Cancel, CancellationToken? cancellationToken = null)
        {
            string file_name = FilePath.Replace(PCKUtils.PathPrefixRes, "").Replace(PCKUtils.PathPrefixUser, PCKUtils.PathExtractPrefixUser);
            if (IsRemoval)
            {
                file_name += PCKUtils.PathExtractTagRemoval;
            }

            string path = extractPath = Path.GetFullPath(Path.Combine(basePath, file_name));
            string op = "Extracting file";

            skippedExisted = false;
            string dir = Path.GetDirectoryName(path);
            BinaryWriter file;

            if (File.Exists(path) && !overwriteExisting)
            {
                skippedExisted = true;
                return true;
            }

            if (IsEncrypted && encKey == null)
            {
                switch (noKeyMode)
                {
                    case PCKExtractNoEncryptionKeyMode.Cancel:
                        PCKActions.progress?.ShowMessage($"Failed to extract the packed file.\nThe file is encrypted, but the decryption key was not specified.", "Error", MessageType.Error);
                        return false;
                    case PCKExtractNoEncryptionKeyMode.Skip:
                        PCKActions.progress?.LogProgress(op, $"The file is encrypted, but it will be skipped according to the settings.");
                        return false;
                    case PCKExtractNoEncryptionKeyMode.AsIs:
                        // Rename file to mark it as encrypted
                        path = extractPath = Path.ChangeExtension(path, Path.GetExtension(path) + ".encrypted");
                        if (File.Exists(path) && !overwriteExisting)
                        {
                            skippedExisted = true;
                            return true;
                        }
                        break;
                }
            }

            try
            {
                Directory.CreateDirectory(dir);
                file = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                return false;
            }

            try
            {
                if (Size > 0 && !IsRemoval)
                {
                    StreamReader.BaseStream.Seek(Offset, SeekOrigin.Begin);

                    bool write_raw_file()
                    {
                        long written = 0;
                        foreach (var block in ReadMemoryBlocks())
                        {
                            file.Write(block.Span);
                            written += block.Length;
                            OnProgress?.Invoke((int)((double)written / ActualSize * 100));

                            if (cancellationToken?.IsCancellationRequested ?? false)
                                return false;
                        }
                        return true;
                    }

                    if (IsEncrypted)
                    {
                        if (encKey == null)
                        {
                            if (noKeyMode == PCKExtractNoEncryptionKeyMode.AsIs)
                            {
                                PCKActions.progress?.LogProgress(op, $"The file is encrypted, but it will be extracted without decryption according to the settings.");

                                write_raw_file();
                                OnProgress?.Invoke(100);
                                return false;
                            }
                        }
                        else
                        {
                            using var r = new PCKEncryptedFileReader(StreamReader, encKey);
                            long written = 0;
                            foreach (var block in r.ReadDencryptedBlocks())
                            {
                                file.Write(block.Span);
                                written += block.Length;
                                OnProgress?.Invoke((int)((double)written / Size * 100));

                                if (cancellationToken?.IsCancellationRequested ?? false)
                                    return false;
                            }
                        }

                        OnProgress?.Invoke(100);
                    }
                    else
                    {
                        if (!write_raw_file())
                            return false;
                        OnProgress?.Invoke(100);
                    }
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                var res = PCKActions.progress?.ShowMessage(ex, MessageType.Error, PCKMessageBoxButtons.OKCancel);
                file.Close();

                try
                {
                    File.Delete(path);
                }
                catch { }

                if (res == PCKDialogResult.Cancel)
                {
                    return false;
                }
            }
            finally
            {
                file.Close();
            }

            return true;
        }

        public bool CheckMD5(string path)
        {
            // Do not check the MD5 for the Removal file.
            if (IsRemoval)
                return true;

            if (Version.PackVersion >= 2)
            {
                var exp_md5 = PCKUtils.GetFileMD5(path);
                if (!exp_md5.SequenceEqual(MD5))
                {
                    PCKActions.progress?.ShowMessage($"The MD5 of the exported file is not equal to the MD5 specified in the PCK.\n{PCKUtils.ByteArrayToHexString(MD5, " ")} != {PCKUtils.ByteArrayToHexString(exp_md5, " ")}", "Error", MessageType.Error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Read file's memory as is
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReadOnlyMemory<byte>> ReadMemoryBlocks()
        {
            foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(StreamReader.BaseStream, Offset, Offset + ActualSize))
            {
                yield return block;
            }
        }
    }

    public sealed class PCKEncryptedFileReader : IDisposable
    {
        [ThreadStatic]
        static byte[]? temp_encryption_buffer;

        public BinaryReader? Stream;
        public byte[] Key = new byte[16];

        readonly long start_position = -1;
        readonly long data_start_position = -1;
        public byte[] MD5 = new byte[16];
        public long HeaderSize = -1;
        public long DataSize = -1;
        public long DataSizeEncoded = -1;
        public long TotalFileSize = -1;
        int DataSizeDelta = -1;
        public byte[] StartIV = new byte[16];

        public PCKEncryptedFileReader(PCKReaderFile file, byte[] key) : this(file.StreamReader, key, file.Offset)
        {

        }

        public PCKEncryptedFileReader(BinaryReader binReader, byte[] key, long startOffset = -1)
        {
            if (startOffset >= 0)
                binReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);

            if (startOffset >= binReader.BaseStream.Length)
                return;

            Stream = binReader;
            Key = key;
            start_position = Stream.BaseStream.Position;

            if (start_position + 16 + 8 + mbedTLS.CHUNK_SIZE >= binReader.BaseStream.Length)
                return;

            // Update EncryptionHeaderSize if needed
            MD5 = binReader.ReadBytes(16);
            DataSize = binReader.ReadInt64();
            StartIV = binReader.ReadBytes(mbedTLS.CHUNK_SIZE);

            data_start_position = Stream.BaseStream.Position;

            HeaderSize = (int)(data_start_position - start_position);
            DataSizeEncoded = PCKUtils.AlignAddress(DataSize, mbedTLS.CHUNK_SIZE);
            DataSizeDelta = (int)(DataSizeEncoded - DataSize);
            TotalFileSize = HeaderSize + DataSizeEncoded;

            if (data_start_position + DataSizeEncoded > binReader.BaseStream.Length)
                throw new IndexOutOfRangeException("The end of the encrypted file goes beyond the boundaries of the open PCK file.");
        }

        public IEnumerable<ReadOnlyMemory<byte>> ReadDencryptedBlocks()
        {
            if (Stream == null)
            {
                yield break;
            }

            if (PCKUtils.BUFFER_MAX_SIZE % mbedTLS.CHUNK_SIZE != 0)
                throw new ArgumentException($"{nameof(PCKUtils.BUFFER_MAX_SIZE)} must be a multiple of {mbedTLS.CHUNK_SIZE}.");

            temp_encryption_buffer ??= new byte[PCKUtils.BUFFER_MAX_SIZE];
            var output_buffer = new Memory<byte>(temp_encryption_buffer, 0, PCKUtils.BUFFER_MAX_SIZE);

            byte[] iv = StartIV.ToArray();

            using var mtls = new mbedTLS();
            mtls.set_key(Key);

            foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(Stream.BaseStream, data_start_position, data_start_position + DataSizeEncoded))
            {
                mtls.decrypt_cfb(iv, block, output_buffer);
                if (block.Length == PCKUtils.BUFFER_MAX_SIZE)
                {
                    yield return new ReadOnlyMemory<byte>(temp_encryption_buffer, 0, PCKUtils.BUFFER_MAX_SIZE);
                }
                else
                {
                    var dest_size = block.Length - DataSizeDelta;
                    yield return new ReadOnlyMemory<byte>(temp_encryption_buffer, 0, dest_size);
                }
            }
        }

        public void Dispose()
        {
            Stream = null;
        }
    }
}
