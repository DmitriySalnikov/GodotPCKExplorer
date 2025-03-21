﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using NativeLibraryLoader;

namespace GodotPCKExplorer
{
    public static class PCKActions
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr dllToUnload);

        public static bool IsInited { get; private set; } = false;

        /// <summary>
        /// Indicates whether the encryption library is loaded.
        /// </summary>
        public static bool IsMbedTLSLibLoaded
        {
            get => mbedTLSLib != null;
        }

        static NativeLibrary? mbedTLSLib = null;

        internal static IPCKProgressReporter progress;

        static PCKActions()
        {
            progress = new BasicPCKProgressReporter();
            LoadNativeLibs();
        }

        /// <summary>
        /// Initialize the library to work with PCK.
        /// </summary>
        public static void Init()
        {
            LoadNativeLibs();
            IsInited = true;
        }

        /// <summary>
        /// Initialize the library to work with PCK.
        /// </summary>
        /// <param name="progressReporter">Custom class for processing logs.</param>
        public static void Init(IPCKProgressReporter progressReporter)
        {
            progress = progressReporter;
            LoadNativeLibs();
            IsInited = true;
        }

        /// <summary>
        /// Initialize the library to work with PCK.
        /// </summary>
        /// <param name="logPrinter">Custom action for printing log texts.</param>
        public static void Init(Action<string> logPrinter)
        {
            var rep = new BasicPCKProgressReporter
            {
                PrintLogText = logPrinter
            };
            progress = rep;
            LoadNativeLibs();
            IsInited = true;
        }

        static void LoadNativeLibs()
        {
            var myPath = new Uri(AppContext.BaseDirectory).LocalPath;
            var myFolder = Path.GetDirectoryName(myPath);
            var platform = "NotImplemented";
            var lib = "mbedTLS_AES";
            var arch = "ARCH";
            var extension = "EXT";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = "win";
                extension = ".dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = "linux";
                extension = ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platform = "mac";
                extension = ".dylib";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                arch = "universal";
            }
            else
            {
                if (Environment.Is64BitProcess)
                    arch = "x64";
                else
                    arch = "x86";
            }

            var lib_path = Path.Combine(myFolder, "mbedTLS", $"{platform}_{arch}", $"{lib}{extension}");

            if (!IsMbedTLSLibLoaded)
            {
                try
                {
                    mbedTLSLib = new NativeLibrary(lib_path);
                    mbedTLS.LoadMethods(mbedTLSLib);
                }
                catch (Exception ex)
                {
                    progress.Log(ex);
                }
            }
        }

        /// <summary>
        /// Clear the runtime before closing.
        /// </summary>
        public static void Cleanup()
        {
            mbedTLS.UnloadMethods();
            mbedTLSLib?.Dispose();
            mbedTLSLib = null;
            IsInited = false;
        }

        /// <summary>
        /// Check if the mbedTLS library is loaded correctly.
        /// </summary>
        /// <returns>Whether you can use the library.</returns>
        public static bool ValidateMbedTLS()
        {
            if (mbedTLSLib == null)
            {
                return false;
            }

            try
            {
                using var mbedtls = new mbedTLS();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Show a message about the PCK and output the information to the logs.
        /// </summary>
        /// <param name="filePath">Path to the PCK file.</param>
        /// <param name="listFiles">Output a list of contents.</param>
        /// <param name="encKey">The encryption key, if the PCK is encrypted.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool PrintInfo(string filePath, bool listFiles = false, string? encKey = null, CancellationToken? cancellationToken = null)
        {
            progress?.Log("PCK Info started");
            progress?.Log($"Input file: {filePath}");

            if (File.Exists(filePath))
            {
                using var pckReader = new PCKReader();
                if (pckReader.OpenFile(
                    path: filePath,
                    logFileNamesProgress: listFiles,
                    getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = encKey ?? "" },
                    cancellationToken: cancellationToken))
                {
                    progress?.ShowMessage(
                        $"Pack version {pckReader.PCK_VersionPack}. Godot version {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}\n" +
                        $"Version string for this program: {pckReader.PCK_VersionPack}.{pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}\n" +
                        $"File count: {pckReader.PCK_FileCount}" +
                        (pckReader.IsEncryptedIndex ? "\nThe file index is encrypted" : "") +
                        (pckReader.IsEncryptedFiles ? "\nFiles are encrypted" : ""), "Pack Info", MessageType.Info);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                progress?.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
            }
            return false;
        }

        /// <summary>
        /// Change the PCK file version if it is not encrypted.
        /// </summary>
        /// <param name="filePath">Path to the PCK file.</param>
        /// <param name="strVersion">A new version of the file. Format: [pack version].[godot major].[godot minor].[godot patch] e.g. <c>2.4.1.1</c></param>
        /// <returns><c>true</c> if successful</returns>
        public static bool ChangeVersion(string filePath, string strVersion)
        {
            progress?.Log($"Change PCK Version started for");
            progress?.Log($"Input file: {filePath}");

            if (File.Exists(filePath))
            {
                var newVersion = new PCKVersion(strVersion);
                if (!newVersion.IsValid())
                {
                    progress?.ShowMessage("The version is specified incorrectly.", "Error", MessageType.Error);
                    return false;
                }

                long pckStartPosition = 0;
                using (var pck = new PCKReader())
                {
                    if (pck.OpenFile(filePath, logFileNamesProgress: false))
                        pckStartPosition = pck.PCK_StartPosition;
                    else
                        return false;
                }

                try
                {
                    using var bw = new BinaryWriter(File.Open(filePath, FileMode.Open));
                    using var br = new BinaryReader(bw.BaseStream);

                    bw.BaseStream.Seek(pckStartPosition, SeekOrigin.Begin);
                    if (br.ReadInt32() != PCKUtils.PCK_MAGIC)
                    {
                        progress?.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);
                        return false;
                    }

                    bw.Write((int)newVersion.PackVersion);
                    bw.Write((int)newVersion.Major);
                    bw.Write((int)newVersion.Minor);
                    bw.Write((int)newVersion.Revision);

                    progress?.ShowMessage($"Version changed. New version: {newVersion}", "Progress", MessageType.Info);
                }
                catch (Exception ex)
                {
                    progress?.ShowMessage(ex, MessageType.Error);
                    return false;
                }
                return true;
            }
            else
            {
                progress?.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
            }
            return false;
        }

        /// <summary>
        /// Extract files from PCK file.
        /// </summary>
        /// <param name="filePath">Path to the PCK file.</param>
        /// <param name="dirPath">The directory where the files will be extracted.</param>
        /// <param name="overwriteExisting">Whether to overwrite existing files.</param>
        /// <param name="files">List of files to extract. Format: res://[path to file]</param>
        /// <param name="check_md5">Whether to check MD5 after extraction.</param>
        /// <param name="encKey">The encryption key, if the PCK is encrypted.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Extract(string filePath, string dirPath, bool overwriteExisting = true, IEnumerable<string>? files = null, bool check_md5 = true, string? encKey = null, PCKExtractNoEncryptionKeyMode noKeyMode = PCKExtractNoEncryptionKeyMode.Cancel, CancellationToken? cancellationToken = null)
        {
            progress?.Log("Extract PCK started");
            progress?.Log($"Input file: {filePath}");
            progress?.Log($"Output directory: {dirPath}");

            if (File.Exists(filePath))
            {
                using var pckReader = new PCKReader();
                PCKReaderEncryptionKeyResult getEncKey()
                {
                    return new PCKReaderEncryptionKeyResult() { Key = encKey ?? "" };
                }

                if (pckReader.OpenFile(filePath, getEncryptionKey: getEncKey))
                {
                    List<string> extractedFiles;
                    List<string> failedFiles;
                    if (files != null)
                        return pckReader.ExtractFiles(
                            names: files,
                            extractedFiles: out extractedFiles,
                            failedFiles: out failedFiles,
                            folder: dirPath,
                            overwriteExisting: overwriteExisting,
                            checkMD5: check_md5,
                            getEncryptionKey: getEncKey,
                            noKeyMode: noKeyMode,
                            cancellationToken: cancellationToken);
                    else
                        return pckReader.ExtractAllFiles(
                            extractedFiles: out extractedFiles,
                            failedFiles: out failedFiles,
                            folder: dirPath,
                            overwriteExisting: overwriteExisting,
                            checkMD5: check_md5,
                            getEncryptionKey: getEncKey,
                            noKeyMode: noKeyMode,
                            cancellationToken: cancellationToken);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                progress?.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
            }
            return false;
        }

        /// <summary>
        /// Pack the files into a new PCK file.
        /// </summary>
        /// <param name="dirPath">The directory from which the files will be recursively packed.</param>
        /// <param name="outFilePath">The path to the new PCK file.</param>
        /// <param name="strVer">A version of the file. Format: [pack version].[godot major].[godot minor].[godot patch] e.g. <c>2.4.1.1</c></param>
        /// <param name="pckToPatch">The path to the PCK file that will be used to create the patched PCK.</param>
        /// <param name="packPathPrefix">The path prefix in the pack. For example, if the prefix is <c>test_folder/</c>, then the path <c>res://icon.png</c> is converted to <c>res://test_folder/icon.png</c>. Not applied to files from <see cref="pckToPatch"/></param>
        /// <param name="alignment">The address of each file will be aligned to this value.</param>
        /// <param name="embed">If enabled and an existing <see cref="filePath"/> is specified, then the PCK will be embedded into this file.</param>
        /// <param name="encKey">The encryption key if you want to encrypt a new PCK file.</param>
        /// <param name="encIndex">Whether to encrypt the index (list of contents).</param>
        /// <param name="encFiles">Whether to encrypt the contents of files.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Pack(string dirPath, string outFilePath, string strVer, string pckToPatch = "", string packPathPrefix = "", uint alignment = 16, bool embed = false, string? encKey = null, bool encIndex = false, bool encFiles = false, CancellationToken? cancellationToken = null)
        {
            if (Directory.Exists(dirPath))
            {
                var ver = new PCKVersion(strVer);
                if (!ver.IsValid())
                {
                    progress?.ShowMessage($"The version '{ver}' is specified incorrectly. Negative values are not allowed.", "Error", MessageType.Error);
                    return false;
                }

                if (outFilePath == pckToPatch)
                {
                    progress?.ShowMessage($"The path to the new file cannot be equal to the patch file", "Error", MessageType.Error);
                    return false;
                }

                using var pckReader = new PCKReader();
                PCKReaderEncryptionKeyResult getEncKey()
                {
                    return new PCKReaderEncryptionKeyResult() { Key = encKey ?? "" };
                }

                Dictionary<string, PCKPackerFile> files = new Dictionary<string, PCKPackerFile>();
                if (!string.IsNullOrWhiteSpace(pckToPatch))
                {
                    if (pckReader.OpenFile(pckToPatch, getEncryptionKey: getEncKey, cancellationToken: cancellationToken))
                    {
                        // fill with original pck files
                        foreach (var f in pckReader.Files)
                            files[f.Key] = new PCKPackerPCKFile(f.Value);
                    }
                    else
                    {
                        progress?.ShowMessage($"The file that needs to be patched could not be opened.", "Error", MessageType.Error);
                        return false;
                    }
                }

                var scanFiles = PCKUtils.GetListOfFilesToPack(dirPath, ver, packPathPrefix, cancellationToken);
                if (encFiles)
                    scanFiles.ForEach(f => f.IsEncrypted = true);

                // add new files or replace the originals from pck
                foreach (var f in scanFiles)
                    files[f.Path] = f;

                return Pack(files.Values, outFilePath, strVer, alignment, embed, encKey, encIndex, cancellationToken);
            }
            else
            {
                progress?.ShowMessage($"Specified directory does not exists! '{dirPath}'", "Error", MessageType.Error);
            }
            return false;
        }

        /// <summary>
        /// Pack the files into a new PCK file.
        /// </summary>
        /// <param name="files">A list of files to be packed. Must be in a valid "res://" or "user://" format. See <see cref="PCKPackerPCKFile"/> to copy files from another PCK file.</param>
        /// <param name="outFilePath">The path to the new PCK file.</param>
        /// <param name="strVer">A version of the file. Format: [pack version].[godot major].[godot minor].[godot patch] e.g. <c>2.4.1.1</c></param>
        /// <param name="alignment">The address of each file will be aligned to this value.</param>
        /// <param name="embed">If enabled and an existing <see cref="filePath"/> is specified, then the PCK will be embedded into this file.</param>
        /// <param name="encKey">The encryption key if you want to encrypt a new PCK file.</param>
        /// <param name="encIndex">Whether to encrypt the index (list of contents).</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Pack(IEnumerable<PCKPackerFile> files, string outFilePath, string strVer, uint alignment = 16, bool embed = false, string? encKey = null, bool encIndex = false, CancellationToken? cancellationToken = null)
        {
            if (!files.Any())
            {
                progress?.ShowMessage("No files to pack", "Error", MessageType.Error);
                return false;
            }

            progress?.Log("Pack PCK started");
            progress?.Log($"Output file: {outFilePath}");

            var ver = new PCKVersion(strVer);

            if (!ver.IsValid())
            {
                progress?.ShowMessage($"The version '{ver}' is specified incorrectly. Negative values are not allowed.", "Error", MessageType.Error);
                return false;
            }

            // create backup
            var oldPCKFile = Path.ChangeExtension(outFilePath, "old" + Path.GetExtension(outFilePath));
            if (embed)
            {
                try
                {
                    if (File.Exists(oldPCKFile))
                        File.Delete(oldPCKFile);
                    File.Copy(outFilePath, oldPCKFile);
                }
                catch (Exception ex)
                {
                    progress?.ShowMessage("Unable to create a backup copy of the file:\n" + ex.Message, "Error", MessageType.Error);
                    progress?.Log(ex.StackTrace);
                    return false;
                }
            }

            if (PCKPacker.PackFiles(outFilePath, embed, files, ver, alignment, PCKUtils.HexStringToByteArray(encKey), encIndex, cancellationToken))
            {
                return true;
            }
            else
            {
                if (embed)
                {
                    // restore backup
                    try
                    {
                        if (File.Exists(outFilePath))
                            File.Delete(outFilePath);
                        File.Move(oldPCKFile, outFilePath);
                    }
                    catch (Exception ex)
                    {
                        progress?.Log("[Error] Can't restore backup file.\n" + ex.Message);
                        progress?.Log(ex.StackTrace);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Make a copy of the PCK file as a separate file.
        /// </summary>
        /// <param name="exeFile">EXE or any other file that contains PCK inside.</param>
        /// <param name="outFile">The path to the new PCK file.</param>
        /// <param name="removeBackup">Whether to delete the backup if the operation completes successfully.</param>
        /// <param name="showMessage">Whether to show messages when operations are completed successfully.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Rip(string exeFile, string? outFile = null, bool removeBackup = false, bool showMessage = true, CancellationToken? cancellationToken = null)
        {
            progress?.Log("Rip PCK started");
            progress?.Log($"Input file: {exeFile}");
            progress?.Log($"Output file: {outFile}");

            if (File.Exists(exeFile))
            {
                bool res = false;
                long to_write = 0;

                using (var pckReader = new PCKReader())
                {
                    res = pckReader.OpenFile(exeFile, false, readOnlyHeaderGodot4: true, logFileNamesProgress: false, cancellationToken: cancellationToken);
                    if (!res)
                    {
                        progress?.ShowMessage($"The file does not contain '.pck' inside", "Error", MessageType.Error);
                        return false;
                    }
                    to_write = pckReader.PCK_StartPosition;

                    if (!pckReader.PCK_Embedded)
                    {
                        progress?.ShowMessage($"The selected file is a regular '.pck'.\nOperation is not required..", "Error", MessageType.Error);
                        return false;
                    }

                    if (outFile == exeFile)
                    {
                        progress?.ShowMessage($"The path to the new file cannot be equal to the original file", "Error", MessageType.Error);
                        return false;
                    }

                    // rip pck
                    if (outFile != null)
                    {
                        if (pckReader.RipPCKFileFromExe(outFile))
                        {
                            if (showMessage)
                                progress?.ShowMessage($"Extracting the '.pck' file from another file is complete.", "Progress", MessageType.Info);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (res && outFile == null)
                {
                    // create backup
                    var oldExeFile = Path.ChangeExtension(exeFile, "old" + Path.GetExtension(exeFile));
                    try
                    {
                        if (File.Exists(oldExeFile))
                            File.Delete(oldExeFile);
                        File.Move(exeFile, oldExeFile);
                    }
                    catch (Exception ex)
                    {
                        progress?.ShowMessage("Unable to create a backup copy of the file:\n" + ex.Message, "Error", MessageType.Error);
                        progress?.Log(ex.StackTrace);
                        return false;
                    }

                    BinaryReader? exeOld = null;
                    BinaryWriter? exeNew = null;

                    // copy only exe part
                    try
                    {
                        exeOld = new BinaryReader(File.OpenRead(oldExeFile));
                        exeNew = new BinaryWriter(File.OpenWrite(exeFile));
                        while (to_write > 0)
                        {
                            var read = exeOld.ReadBytes(Math.Min(PCKUtils.BUFFER_MAX_SIZE, (int)to_write));
                            exeNew.Write(read);
                            to_write -= read.Length;
                        }
                        exeOld.Close();
                        exeNew.Close();
                    }
                    catch (Exception ex)
                    {
                        exeOld?.Close();
                        exeNew?.Close();

                        // restore backup
                        try
                        {
                            if (File.Exists(exeFile))
                                File.Delete(exeFile);
                            File.Move(oldExeFile, exeFile);
                        }
                        catch (Exception e)
                        {
                            progress?.Log("[Error] Can't restore backup file.\n" + e.Message);
                            progress?.Log(e.StackTrace);
                        }

                        progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }
                    if (showMessage)
                        progress?.ShowMessage($"Removing '.pck' from another file is completed. The original file is renamed to \"{oldExeFile}\"", "Progress", MessageType.Info);

                    // remove backup
                    try
                    {
                        if (removeBackup)
                            File.Delete(oldExeFile);
                    }
                    catch (Exception ex)
                    {
                        progress?.ShowMessage(ex, MessageType.Error);
                    }
                }

                return res;
            }
            else
            {
                progress?.ShowMessage($"Specified file does not exists! '{exeFile}'", "Error", MessageType.Error);
            }
            return false;
        }

        /// <summary>
        /// Remove the PCK part from the file.
        /// </summary>
        /// <param name="exeFile">EXE or any other file that contains PCK inside.</param>
        /// <param name="removeBackup">Whether to delete the backup if the operation completes successfully.</param>
        /// <param name="showMessage">Whether to show messages when operations are completed successfully.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Remove(string exeFile, bool removeBackup = false, bool showMessage = true, CancellationToken? cancellationToken = null)
        {
            return Rip(exeFile, null, removeBackup, showMessage, cancellationToken);
        }

        /// <summary>
        /// Merge an existing PCK with an EXE or any other file.
        /// </summary>
        /// <param name="pckFile">Path to the PCK file.</param>
        /// <param name="exeFile">EXE or any other file without PCK inside.</param>
        /// <param name="removeBackup">Whether to delete the backup if the operation completes successfully.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Merge(string pckFile, string exeFile, bool removeBackup = false, CancellationToken? cancellationToken = null)
        {
            progress?.Log("Merge PCK started");
            progress?.Log($"Input file: {pckFile}");
            progress?.Log($"Output file: {exeFile}");

            if (File.Exists(pckFile))
            {
                using var pckReader = new PCKReader();

                bool res = pckReader.OpenFile(pckFile, readOnlyHeaderGodot4: true, logFileNamesProgress: false, cancellationToken: cancellationToken);
                if (!res)
                {
                    pckReader.Close();
                    progress?.ShowMessage($"Unable to open PCK file", "Error", MessageType.Error);
                    return false;
                }

                if (exeFile == pckFile)
                {
                    pckReader.Close();
                    progress?.ShowMessage($"The path to the new file cannot be equal to the original file", "Error", MessageType.Error);
                    return false;
                }

                // create backup
                var oldExeFile = Path.ChangeExtension(exeFile, "old" + Path.GetExtension(exeFile));
                try
                {
                    if (File.Exists(oldExeFile))
                        File.Delete(oldExeFile);
                    File.Copy(exeFile, oldExeFile);
                }
                catch (Exception ex)
                {
                    pckReader.Close();
                    progress?.ShowMessage("Unable to create a backup copy of the file:\n" + ex.Message, "Error", MessageType.Error);
                    progress?.Log(ex.StackTrace);
                    return false;
                }

                // merge
                if (exeFile != null)
                    if (pckReader.MergePCKFileIntoExe(exeFile, cancellationToken))
                        progress?.ShowMessage($"Merging '.pck' into another file is complete.", "Progress", MessageType.Info);
                    else
                    {
                        pckReader.Close();

                        // restore backup
                        try
                        {
                            if (File.Exists(exeFile))
                                File.Delete(exeFile);
                            File.Move(oldExeFile, exeFile);
                        }
                        catch (Exception ex)
                        {
                            progress?.Log("[Error] Can't restore backup file.\n" + ex.Message);
                            progress?.Log(ex.StackTrace);
                        }

                        return false;
                    }

                // remove backup
                try
                {
                    if (removeBackup)
                        File.Delete(oldExeFile);
                }
                catch (Exception ex)
                {
                    progress?.ShowMessage(ex, MessageType.Error);
                }

                return res;
            }
            else
            {
                progress?.ShowMessage($"Specified file does not exists! '{pckFile}'", "Error", MessageType.Error);
            }
            return false;
        }

        /// <summary>
        /// Split the EXE and PCK into 2 separate files.
        /// </summary>
        /// <param name="exeFile">EXE or any other file that contains PCK inside.</param>
        /// <param name="newExeName">The path to the new EXE file. PCK will have the same name.</param>
        /// <param name="removeBackup">Whether to delete the backup if the operation completes successfully.</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool Split(string exeFile, string? newExeName = null, bool removeBackup = true, CancellationToken? cancellationToken = null)
        {
            progress?.Log("Split PCK started");
            progress?.Log($"Input file: {exeFile}");

            if (File.Exists(exeFile))
            {
                var name = exeFile;
                if (newExeName != null)
                {
                    name = newExeName;

                    if (name == exeFile)
                    {
                        progress?.ShowMessage($"The new pair cannot be named the same as the original file: {newExeName}", "Error", MessageType.Error);
                        return false;
                    }

                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(name)))
                            Directory.CreateDirectory(Path.GetDirectoryName(name));

                        if (File.Exists(name))
                            File.Delete(name);

                        progress?.Log($"[Info] Progress: Copying the original file to {name}");
                        File.Copy(exeFile, name);
                    }
                    catch (Exception ex)
                    {
                        progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }

                }

                var pckName = Path.ChangeExtension(name, ".pck");
                if (Rip(name, pckName, false, false, cancellationToken))
                {
                    if (Remove(name, removeBackup, false, cancellationToken))
                    {
                        progress?.ShowMessage($"Split finished. Original file: \"{exeFile}\".\nNew files: \"{name}\" and \"{pckName}\"", "Progress", MessageType.Info);
                        return true;
                    }
                }

                // remove new broken files
                if (newExeName != null)
                {
                    progress?.Log($"Attempt to delete a new pair of broken files: \"{name}\" \"{pckName}\"");
                    try
                    {
                        if (File.Exists(name))
                            File.Delete(name);
                    }
                    catch (Exception ex)
                    {
                        progress?.ShowMessage(ex, MessageType.Error);
                    }

                    try
                    {
                        if (File.Exists(pckName))
                            File.Delete(pckName);

                    }
                    catch (Exception ex)
                    {
                        progress?.ShowMessage(ex, MessageType.Error);
                    }
                }

                return false;
            }
            else
            {
                progress?.ShowMessage($"Specified file does not exists! '{exeFile}'", "Error", MessageType.Error);
            }
            return false;
        }

    }
}
