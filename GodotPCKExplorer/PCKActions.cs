using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public class PCKActions
    {
        public static bool HelpRun()
        {
            Program.CommandLog("Help", "Help text", true);
            return true;
        }

        // TODO: add encryption key everywhere

        public static void Init()
        {
            Program.Init();
        }

        public static bool OpenPCKRun(string path)
        {
            Program.CMDMode = false;
            if (File.Exists(path))
            {
                if (Program.mainForm == null)
                {
                    Program.mainForm = new Form1();
                    Program.mainForm.OpenFile(path);

                    Program.HideConsole();
                    Application.Run(Program.mainForm);
                }
                else
                {
                    Program.mainForm.OpenFile(path);
                }
                return true;
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{path}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static void ClosePCK()
        {
            Program.mainForm?.CloseFile();
        }

        public static void CleanupApp()
        {
            Program.Cleanup();
        }

        public static bool InfoPCKRun(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var pckReader = new PCKReader())
                {
                    if (pckReader.OpenFile(filePath))
                        Program.CommandLog($"\"{pckReader.PackPath}\"\nPack version {pckReader.PCK_VersionPack}. Godot version {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}\nVersion string for this program: {pckReader.PCK_VersionPack}.{pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}", "Pack Info", false, MessageType.Info);
                    else
                        return false;
                }
                return true;
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{filePath}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static bool ChangePCKVersion(string filePath, string strVersion)
        {
            if (File.Exists(filePath))
            {
                var newVersion = new PCKVersion(strVersion);
                if (!newVersion.IsValid)
                {
                    Program.CommandLog("The version is specified incorrectly.", "Error", true, MessageType.Error);
                    return false;
                }

                long pckStartPosition = 0;
                using (var pck = new PCKReader())
                {
                    if (pck.OpenFile(filePath))
                        pckStartPosition = pck.PCK_StartPosition;
                    else
                        return false;
                }

                try
                {
                    using (var bw = new BinaryWriter(File.Open(filePath, FileMode.Open)))
                    {
                        using (var br = new BinaryReader(bw.BaseStream))
                        {
                            bw.BaseStream.Seek(pckStartPosition, SeekOrigin.Begin);
                            if (br.ReadInt32() != Utils.PCK_MAGIC)
                            {
                                Program.ShowMessage("Not a Godot PCK file!", "Error", MessageType.Error);
                                return false;
                            }

                            bw.Write((int)newVersion.PackVersion);
                            bw.Write((int)newVersion.Major);
                            bw.Write((int)newVersion.Minor);
                            bw.Write((int)newVersion.Revision);
                        }
                    }
                    Program.ShowMessage("Version changed", "Progress", MessageType.Info);
                }
                catch (Exception ex)
                {
                    Program.CommandLog(ex, "Error", false, MessageType.Error);
                    return false;
                }
                return true;
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{filePath}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static bool ExtractPCKRun(string filePath, string dirPath, bool overwriteExisting = true, IEnumerable<string> files = null)
        {
            if (File.Exists(filePath))
            {
                using (var pckReader = new PCKReader())
                {
                    if (pckReader.OpenFile(filePath))
                    {
                        if (files != null)
                            return pckReader.ExtractFiles(files, dirPath, overwriteExisting);
                        else
                            return pckReader.ExtractAllFiles(dirPath, overwriteExisting);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{filePath}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static bool PackPCKRun(string dirPath, string filePath, string strVer, uint alignment = 16, bool embed = false)
        {
            if (Directory.Exists(dirPath))
            {
                return PackPCKRun(Utils.ScanFoldersForFiles(dirPath), filePath, strVer, alignment, embed);
            }
            else
            {
                Program.CommandLog($"Specified directory does not exists! '{dirPath}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static bool PackPCKRun(IEnumerable<PCKPacker.FileToPack> files, string filePath, string strVer, uint alignment = 16, bool embed = false)
        {
            if (!files.Any())
            {
                Program.CommandLog("No files to pack", "Error", false, MessageType.Error);
                return false;
            }

            var pckPacker = new PCKPacker();
            var ver = new PCKVersion(strVer);

            if (!ver.IsValid)
            {
                Program.CommandLog("The version is specified incorrectly.", "Error", true, MessageType.Error);
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
                    Program.CommandLog("Unable to create a backup copy of the file:\n" + ex.Message, "Error", false, MessageType.Error);
                    Program.Log(ex.StackTrace);
                    return false;
                }
            }

            if (pckPacker.PackFiles(filePath, files, alignment, ver, embed))
            {
                return true;
            }
            else
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
                    Program.Log("[Error] Can't restore backup file.\n" + ex.Message);
                    Program.Log(ex.StackTrace);
                }
            }
            return false;
        }

        public static bool RipPCKRun(string exeFile, string outFile = null, bool removeBackup = false, bool show_message = true)
        {
            if (File.Exists(exeFile))
            {
                bool res = false;
                long to_write = 0;

                using (var pckReader = new PCKReader())
                {
                    res = pckReader.OpenFile(exeFile, false);
                    if (!res)
                    {
                        Program.CommandLog($"The file does not contain '.pck' inside", "Error", false, MessageType.Error);
                        return false;
                    }
                    to_write = pckReader.PCK_StartPosition;

                    if (!pckReader.PCK_Embedded)
                    {
                        Program.CommandLog($"The selected file is a regular '.pck'.\nOperation is not required..", "Error", false, MessageType.Error);
                        return false;
                    }

                    if (outFile == exeFile)
                    {
                        Program.CommandLog($"The path to the new file cannot be equal to the original file", "Error", false, MessageType.Error);
                        return false;
                    }

                    // rip pck
                    if (outFile != null)
                        if (pckReader.RipPCKFileFromExe(outFile))
                        {
                            if (show_message)
                                Program.CommandLog($"Extracting the '.pck' file from another file is complete.", "Progress", false, MessageType.Info);
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
                        Program.CommandLog("Unable to create a backup copy of the file:\n" + ex.Message, "Error", false, MessageType.Error);
                        Program.Log(ex.StackTrace);
                        return false;
                    }

                    BinaryReader exeOld = null;
                    BinaryWriter exeNew = null;

                    // copy only exe part
                    try
                    {
                        exeOld = new BinaryReader(File.OpenRead(oldExeFile));
                        exeNew = new BinaryWriter(File.OpenWrite(exeFile));
                        while (to_write > 0)
                        {
                            var read = exeOld.ReadBytes(Math.Min(Utils.BUFFER_MAX_SIZE, (int)to_write));
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
                            Program.Log("[Error] Can't restore backup file.\n" + e.Message);
                            Program.Log(e.StackTrace);
                        }

                        Program.CommandLog(ex, "Error", false, MessageType.Error);
                        return false;
                    }
                    if (show_message)
                        Program.CommandLog($"Removing '.pck' from another file is completed. The original file is renamed to \"{oldExeFile}\"", "Progress", false, MessageType.Info);

                    // remove backup
                    try
                    {
                        if (removeBackup)
                            File.Delete(oldExeFile);
                    }
                    catch (Exception ex)
                    {
                        Program.CommandLog(ex, "Error", false, MessageType.Error);
                    }
                }

                return res;
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{exeFile}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static bool MergePCKRun(string pckFile, string exeFile, bool removeBackup = false)
        {
            if (File.Exists(pckFile))
            {
                using (var pckReader = new PCKReader())
                {
                    bool res = pckReader.OpenFile(pckFile);

                    if (!res)
                    {
                        pckReader.Close();
                        Program.CommandLog($"Unable to open PCK file", "Error", false, MessageType.Error);
                        return false;
                    }

                    if (exeFile == pckFile)
                    {
                        pckReader.Close();
                        Program.CommandLog($"The path to the new file cannot be equal to the original file", "Error", false, MessageType.Error);
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
                        Program.CommandLog("Unable to create a backup copy of the file:\n" + ex.Message, "Error", false, MessageType.Error);
                        Program.Log(ex.StackTrace);
                        return false;
                    }

                    // merge
                    if (exeFile != null)
                        if (pckReader.MergePCKFileIntoExe(exeFile))
                            Program.CommandLog($"Merging '.pck' into another file is complete.", "Progress", false, MessageType.Info);
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
                                Program.Log("[Error] Can't restore backup file.\n" + ex.Message);
                                Program.Log(ex.StackTrace);
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
                        Program.CommandLog(ex, "Error", false, MessageType.Error);
                    }

                    return res;
                }
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{pckFile}'", "Error", false, MessageType.Error);
            }
            return false;
        }

        public static bool SplitPCKRun(string exeFile, string newExeName = null, bool removeBackup = true)
        {
            if (File.Exists(exeFile))
            {
                var name = exeFile;
                if (newExeName != null)
                {
                    name = newExeName;

                    if (name == exeFile)
                    {
                        Program.CommandLog($"The new pair cannot be named the same as the original file: {newExeName}", "Error", false, MessageType.Error);
                        return false;
                    }

                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(name)))
                            Directory.CreateDirectory(Path.GetDirectoryName(name));

                        if (File.Exists(name))
                            File.Delete(name);

                        Program.Log($"[Info] Progress: Copying the original file to {name}");
                        File.Copy(exeFile, name);
                    }
                    catch (Exception ex)
                    {
                        Program.CommandLog(ex, "Error", false, MessageType.Error);
                        return false;
                    }

                }

                var pckName = Path.ChangeExtension(name, ".pck");
                if (RipPCKRun(name, pckName, false, false))
                {
                    if (RipPCKRun(name, null, removeBackup, false))
                    {
                        Program.CommandLog($"Split finished. Original file: \"{exeFile}\".\nNew files: \"{name}\" and \"{pckName}\"", "Progress", false, MessageType.Info);
                        return true;
                    }
                }

                // remove new broken files
                if (newExeName != null)
                {
                    Program.Log($"Attempt to delete a new pair of broken files: \"{name}\" \"{pckName}\"");
                    try
                    {
                        if (File.Exists(name))
                            File.Delete(name);
                    }
                    catch (Exception ex)
                    {
                        Program.CommandLog(ex, "Error", false, MessageType.Error);
                    }

                    try
                    {
                        if (File.Exists(pckName))
                            File.Delete(pckName);

                    }
                    catch (Exception ex)
                    {
                        Program.CommandLog(ex, "Error", false, MessageType.Error);
                    }
                }

                return false;
            }
            else
            {
                Program.CommandLog($"Specified file does not exists! '{exeFile}'", "Error", false, MessageType.Error);
            }
            return false;
        }

    }
}
