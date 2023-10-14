using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace GodotPCKExplorer
{
    public enum PCKMessageBoxIcon : byte
    {
        None = 0,
        Hand = 0x10,
        Question = 0x20,
        Exclamation = 48,
        Asterisk = 0x40,
        Stop = 0x10,
        Error = 0x10,
        Warning = 48,
        Information = 0x40
    }

    public enum PCKMessageBoxButtons : byte
    {
        OK,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel
    }

    public enum PCKDialogResult : byte
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public interface IProgressReporter
    {
        /// <summary>
        /// Output percent or some number if prefix is not null
        /// </summary>
        /// <param name="operation">current operation name</param>
        /// <param name="number">number to print</param>
        /// <param name="customPrefix">number prefix. If null, output percentages from 0 to 100. Otherwise - any number with a prefix.</param>
        void LogProgress(string operation, int number, string? customPrefix = null);

        void LogProgress(string operation, string str);

        void Log(string txt);

        void Log(Exception ex);

        PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK);

        PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK);
    }

    public class ProgressReporter : IProgressReporter
    {
        int prev_progress = 0;
        DateTime prev_time = DateTime.UtcNow;

        public void LogProgress(string operation, int number, string? customPrefix = null)
        {
            if (((DateTime.UtcNow - prev_time).TotalSeconds > 1) || (prev_progress != number && Math.Abs(number - prev_progress) >= 5))
            {
                if (customPrefix != null)
                    Log($"[Progress] {operation}: {customPrefix}{number}");
                else
                    Log($"[Progress] {operation}: {Math.Max(Math.Min(number, 100), 0)}%");

                prev_progress = number;
                prev_time = DateTime.UtcNow;
            }
        }

        public void LogProgress(string operation, string str)
        {
            Log($"[Progress] {operation}: {str}");
        }

        public void Log(string txt)
        {
            var isFirst = true;
            txt = string.Join("\n",
                txt.Split('\n').
                Select((t) =>
                {
                    var res = $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]{(isFirst ? "\t" : "-\t")}{t}";
                    isFirst = false;
                    return res;
                }));

            Console.WriteLine(txt);
        }

        public void Log(Exception ex)
        {
            Log($"Exception:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}");
        }

        public PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            Log($"[{messageType}] \"{title}\": {text}");

#if DEV_ENABLED
            System.Diagnostics.Debugger.Break();
#endif
            return PCKDialogResult.OK;
        }

        public PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            var res = ShowMessage(ex.Message, ex.GetType().Name, messageType, boxButtons);
            Log(ex);
            return res;
        }
    }

    public static class PCKActions
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr dllToUnload);

        public static bool IsMbedTLSLibLoaded
        {
            get => mbedTLSLibPtr != IntPtr.Zero;
        }
        static IntPtr mbedTLSLibPtr = IntPtr.Zero;

        internal static IProgressReporter progress;

        static PCKActions()
        {
            progress = new ProgressReporter();
        }

        public static bool HelpRun()
        {
            return true;
        }

        public static void Init(IProgressReporter? progressReporter = null)
        {
            if (progressReporter != null)
            {
                progress = progressReporter;
            }

            LoadNativeLibs();
        }

        // https://stackoverflow.com/a/30646096/8980874
        static void LoadNativeLibs()
        {
            var myPath = new Uri(typeof(PCKActions).Assembly.CodeBase).LocalPath;
            var myFolder = Path.GetDirectoryName(myPath);
            var subFolder = Path.Combine(myFolder, "mbedTLS", (Environment.Is64BitProcess ? "x64" : "x86"));

            if (mbedTLSLibPtr == IntPtr.Zero)
            {
                mbedTLSLibPtr = LoadLibrary(Path.Combine(subFolder, "mbedTLS_AES.dll"));
                if (mbedTLSLibPtr == IntPtr.Zero)
                {
                    PCKActions.progress?.Log("Failed to load 'mbedTLS_AES.dll'");
                }
            }
        }

        public static void Cleanup()
        {
            if (IsMbedTLSLibLoaded)
            {
                FreeLibrary(mbedTLSLibPtr);
                mbedTLSLibPtr = IntPtr.Zero;
            }
        }

        public static bool InfoPCKRun(string filePath, bool list_files = false, string? encKey = null, CancellationToken? cancellationToken = null)
        {
            PCKActions.progress?.Log("PCK Info started");
            PCKActions.progress?.Log($"Input file: {filePath}");

            if (File.Exists(filePath))
            {
                using var pckReader = new PCKReader();
                if (pckReader.OpenFile(filePath, get_encryption_key: () => encKey, read_only_header_godot4: !list_files, cancellationToken: cancellationToken))
                {
                    var enc_text = "";
                    if (pckReader.IsEncrypted)
                    {
                        if (pckReader.IsEncryptedIndex != pckReader.IsEncryptedFiles)
                        {
                            if (pckReader.IsEncryptedIndex)
                                enc_text = "Encrypted Index";
                            else
                                enc_text = "Encrypted Files";
                        }
                        else
                        {
                            enc_text = "Encrypted";
                        }
                    }
                    if (enc_text != "")
                        enc_text = " " + enc_text;

                    PCKActions.progress?.ShowMessage(
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
                PCKActions.progress?.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
            }
            return false;
        }

        public static bool ChangePCKVersion(string filePath, string strVersion)
        {
            PCKActions.progress?.Log($"Change PCK Version started for");
            PCKActions.progress?.Log($"Input file: {filePath}");

            if (File.Exists(filePath))
            {
                var newVersion = new PCKVersion(strVersion);
                if (!newVersion.IsValid)
                {
                    PCKActions.progress?.ShowMessage("The version is specified incorrectly.", "Error", MessageType.Error);
                    return false;
                }

                long pckStartPosition = 0;
                using (var pck = new PCKReader())
                {
                    if (pck.OpenFile(filePath, log_names_progress: false))
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
                        PCKActions.progress?.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);
                        return false;
                    }

                    bw.Write((int)newVersion.PackVersion);
                    bw.Write((int)newVersion.Major);
                    bw.Write((int)newVersion.Minor);
                    bw.Write((int)newVersion.Revision);

                    PCKActions.progress?.ShowMessage($"Version changed. New version: {newVersion}", "Progress", MessageType.Info);
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    return false;
                }
                return true;
            }
            else
            {
                PCKActions.progress?.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
            }
            return false;
        }

        public static bool ExtractPCKRun(string filePath, string dirPath, bool overwriteExisting = true, IEnumerable<string>? files = null, bool check_md5 = true, string? encKey = null, CancellationToken? cancellationToken = null)
        {
            PCKActions.progress?.Log("Extract PCK started");
            PCKActions.progress?.Log($"Input file: {filePath}");
            PCKActions.progress?.Log($"Output directory: {dirPath}");

            if (File.Exists(filePath))
            {
                using var pckReader = new PCKReader();

                if (pckReader.OpenFile(filePath, get_encryption_key: () => encKey))
                {
                    if (files != null)
                        return pckReader.ExtractFiles(files, dirPath, overwriteExisting, check_md5, cancellationToken);
                    else
                        return pckReader.ExtractAllFiles(dirPath, overwriteExisting, check_md5, cancellationToken);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                PCKActions.progress?.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
            }
            return false;
        }

        public static bool PackPCKRun(string dirPath, string filePath, string strVer, uint alignment = 16, bool embed = false, string? encKey = null, bool encIndex = false, bool encFiles = false, CancellationToken? cancellationToken = null)
        {
            if (Directory.Exists(dirPath))
            {
                return PackPCKRun(PCKUtils.ScanFoldersForFiles(dirPath), filePath, strVer, alignment, embed, encKey, encIndex, encFiles, cancellationToken);
            }
            else
            {
                PCKActions.progress?.ShowMessage($"Specified directory does not exists! '{dirPath}'", "Error", MessageType.Error);
            }
            return false;
        }

        public static bool PackPCKRun(IEnumerable<PCKPacker.FileToPack> files, string filePath, string strVer, uint alignment = 16, bool embed = false, string? encKey = null, bool encIndex = false, bool encFiles = false, CancellationToken? cancellationToken = null)
        {
            if (!files.Any())
            {
                PCKActions.progress?.ShowMessage("No files to pack", "Error", MessageType.Error);
                return false;
            }

            PCKActions.progress?.Log("Pack PCK started");
            PCKActions.progress?.Log($"Output file: {filePath}");

            var pckPacker = new PCKPacker(PCKUtils.HexStringToByteArray(encKey), encIndex, encFiles);
            var ver = new PCKVersion(strVer);

            if (!ver.IsValid)
            {
                PCKActions.progress?.ShowMessage($"The version '{ver}' is specified incorrectly. Negative values are not allowed.", "Error", MessageType.Error);
                return false;
            }

            // create backup
            var oldPCKFile = Path.ChangeExtension(filePath, "old" + Path.GetExtension(filePath));
            if (embed)
            {
                try
                {
                    if (File.Exists(oldPCKFile))
                        File.Delete(oldPCKFile);
                    File.Copy(filePath, oldPCKFile);
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage("Unable to create a backup copy of the file:\n" + ex.Message, "Error", MessageType.Error);
                    PCKActions.progress?.Log(ex.StackTrace);
                    return false;
                }
            }

            if (pckPacker.PackFiles(filePath, files, alignment, ver, embed, cancellationToken))
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
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        File.Move(oldPCKFile, filePath);
                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.Log("[Error] Can't restore backup file.\n" + ex.Message);
                        PCKActions.progress?.Log(ex.StackTrace);
                    }
                }
            }
            return false;
        }

        public static bool RipPCKRun(string exeFile, string? outFile = null, bool removeBackup = false, bool show_message = true, CancellationToken? cancellationToken = null)
        {
            PCKActions.progress?.Log("Rip PCK started");
            PCKActions.progress?.Log($"Input file: {exeFile}");
            PCKActions.progress?.Log($"Output file: {outFile}");

            if (File.Exists(exeFile))
            {
                bool res = false;
                long to_write = 0;

                using (var pckReader = new PCKReader())
                {
                    res = pckReader.OpenFile(exeFile, false, log_names_progress: false, read_only_header_godot4: true, cancellationToken: cancellationToken);
                    if (!res)
                    {
                        PCKActions.progress?.ShowMessage($"The file does not contain '.pck' inside", "Error", MessageType.Error);
                        return false;
                    }
                    to_write = pckReader.PCK_StartPosition;

                    if (!pckReader.PCK_Embedded)
                    {
                        PCKActions.progress?.ShowMessage($"The selected file is a regular '.pck'.\nOperation is not required..", "Error", MessageType.Error);
                        return false;
                    }

                    if (outFile == exeFile)
                    {
                        PCKActions.progress?.ShowMessage($"The path to the new file cannot be equal to the original file", "Error", MessageType.Error);
                        return false;
                    }

                    // rip pck
                    if (outFile != null)
                        if (pckReader.RipPCKFileFromExe(outFile))
                        {
                            if (show_message)
                                PCKActions.progress?.ShowMessage($"Extracting the '.pck' file from another file is complete.", "Progress", MessageType.Info);
                        }
                        else
                        {
                            return false;
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
                        PCKActions.progress?.ShowMessage("Unable to create a backup copy of the file:\n" + ex.Message, "Error", MessageType.Error);
                        PCKActions.progress?.Log(ex.StackTrace);
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
                            PCKActions.progress?.Log("[Error] Can't restore backup file.\n" + e.Message);
                            PCKActions.progress?.Log(e.StackTrace);
                        }

                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }
                    if (show_message)
                        PCKActions.progress?.ShowMessage($"Removing '.pck' from another file is completed. The original file is renamed to \"{oldExeFile}\"", "Progress", MessageType.Info);

                    // remove backup
                    try
                    {
                        if (removeBackup)
                            File.Delete(oldExeFile);
                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    }
                }

                return res;
            }
            else
            {
                PCKActions.progress?.ShowMessage($"Specified file does not exists! '{exeFile}'", "Error", MessageType.Error);
            }
            return false;
        }

        public static bool MergePCKRun(string pckFile, string exeFile, bool removeBackup = false, CancellationToken? cancellationToken = null)
        {
            PCKActions.progress?.Log("Merge PCK started");
            PCKActions.progress?.Log($"Input file: {pckFile}");
            PCKActions.progress?.Log($"Output file: {exeFile}");

            if (File.Exists(pckFile))
            {
                using var pckReader = new PCKReader();

                bool res = pckReader.OpenFile(pckFile, log_names_progress: false, read_only_header_godot4: true, cancellationToken: cancellationToken);

                if (!res)
                {
                    pckReader.Close();
                    PCKActions.progress?.ShowMessage($"Unable to open PCK file", "Error", MessageType.Error);
                    return false;
                }

                if (exeFile == pckFile)
                {
                    pckReader.Close();
                    PCKActions.progress?.ShowMessage($"The path to the new file cannot be equal to the original file", "Error", MessageType.Error);
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
                    PCKActions.progress?.ShowMessage("Unable to create a backup copy of the file:\n" + ex.Message, "Error", MessageType.Error);
                    PCKActions.progress?.Log(ex.StackTrace);
                    return false;
                }

                // merge
                if (exeFile != null)
                    if (pckReader.MergePCKFileIntoExe(exeFile, cancellationToken))
                        PCKActions.progress?.ShowMessage($"Merging '.pck' into another file is complete.", "Progress", MessageType.Info);
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
                            PCKActions.progress?.Log("[Error] Can't restore backup file.\n" + ex.Message);
                            PCKActions.progress?.Log(ex.StackTrace);
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
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                }

                return res;
            }
            else
            {
                PCKActions.progress?.ShowMessage($"Specified file does not exists! '{pckFile}'", "Error", MessageType.Error);
            }
            return false;
        }

        public static bool SplitPCKRun(string exeFile, string? newExeName = null, bool removeBackup = true, CancellationToken? cancellationToken = null)
        {
            PCKActions.progress?.Log("Split PCK started");
            PCKActions.progress?.Log($"Input file: {exeFile}");

            if (File.Exists(exeFile))
            {
                var name = exeFile;
                if (newExeName != null)
                {
                    name = newExeName;

                    if (name == exeFile)
                    {
                        PCKActions.progress?.ShowMessage($"The new pair cannot be named the same as the original file: {newExeName}", "Error", MessageType.Error);
                        return false;
                    }

                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(name)))
                            Directory.CreateDirectory(Path.GetDirectoryName(name));

                        if (File.Exists(name))
                            File.Delete(name);

                        PCKActions.progress?.Log($"[Info] Progress: Copying the original file to {name}");
                        File.Copy(exeFile, name);
                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }

                }

                var pckName = Path.ChangeExtension(name, ".pck");
                if (RipPCKRun(name, pckName, false, false, cancellationToken))
                {
                    if (RipPCKRun(name, null, removeBackup, false, cancellationToken))
                    {
                        PCKActions.progress?.ShowMessage($"Split finished. Original file: \"{exeFile}\".\nNew files: \"{name}\" and \"{pckName}\"", "Progress", MessageType.Info);
                        return true;
                    }
                }

                // remove new broken files
                if (newExeName != null)
                {
                    PCKActions.progress?.Log($"Attempt to delete a new pair of broken files: \"{name}\" \"{pckName}\"");
                    try
                    {
                        if (File.Exists(name))
                            File.Delete(name);
                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    }

                    try
                    {
                        if (File.Exists(pckName))
                            File.Delete(pckName);

                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    }
                }

                return false;
            }
            else
            {
                PCKActions.progress?.ShowMessage($"Specified file does not exists! '{exeFile}'", "Error", MessageType.Error);
            }
            return false;
        }

    }
}
