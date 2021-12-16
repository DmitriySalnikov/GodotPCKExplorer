using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public class PCKActions
    {
        public static bool HelpRun()
        {
            Utils.CommandLog("Help", "Help text", true);
            return true;
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
                Utils.CommandLog($"Specified file does not exists! '{path}'", "Error", false);
            }
            return false;
        }

        public static bool InfoPCKRun(string filePath)
        {
            if (File.Exists(filePath))
            {
                var pckReader = new PCKReader();
                if (pckReader.OpenFile(filePath))
                    Utils.CommandLog($"\"{pckReader.PackPath}\"\nPack version {pckReader.PCK_VersionPack}. Godot version {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}\nVersion string for this program: {pckReader.PCK_VersionPack}.{pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}", "Pack Info", false);

                pckReader.Close();
                return true;
            }
            else
            {
                Utils.CommandLog($"Specified file does not exists! '{filePath}'", "Error", false);
            }
            return false;
        }

        public static bool ExtractPCKRun(string filePath, string dirPath, bool overwriteExisting = true, IEnumerable<string> files = null)
        {
            if (File.Exists(filePath))
            {
                var pckReader = new PCKReader();
                bool res;
                if (pckReader.OpenFile(filePath))
                {
                    if (files != null)
                        res = pckReader.ExtractFiles(files, dirPath, overwriteExisting);
                    else
                        res = pckReader.ExtractAllFiles(dirPath, overwriteExisting);
                }
                else
                {
                    return false;
                }

                pckReader.Close();
                return res;
            }
            else
            {
                Utils.CommandLog($"Specified file does not exists! '{filePath}'", "Error", false);
            }
            return false;
        }

        public static bool PackPCKRun(string dirPath, string filePath, string strVer, bool embed = false)
        {
            if (Directory.Exists(dirPath))
            {
                return PackPCKRun(Utils.ScanFoldersForFiles(dirPath), filePath, strVer, embed);
            }
            else
            {
                Utils.CommandLog($"Specified directory does not exists! '{dirPath}'", "Error", false);
            }
            return false;
        }

        public static bool PackPCKRun(IEnumerable<PCKPacker.FileToPack> files, string filePath, string strVer, bool embed = false)
        {
            if (!files.Any())
            {
                Utils.CommandLog("No files to pack", "Error", false);
                return false;
            }

            var pckPacker = new PCKPacker();
            var ver = new PCKVersion(strVer);

            if (!ver.IsValid)
            {
                Utils.CommandLog("The version is specified incorrectly.", "Error", true);
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
                catch (Exception e)
                {
                    Utils.CommandLog("Unable to create a backup copy of the file:\n" + e.Message, "Error", false);
                    return false;
                }
            }

            if (pckPacker.PackFiles(filePath, files, 8, ver, embed))
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
                    Console.WriteLine("Error: Can't restore backup file.\n" + ex.Message);
                }
            }
            return false;
        }

        public static bool RipPCKRun(string exeFile, string outFile = null, bool removeBackup = false)
        {
            if (File.Exists(exeFile))
            {
                var pckReader = new PCKReader();
                bool res = pckReader.OpenFile(exeFile, false);
                long to_write = pckReader.PCK_StartPosition;

                if (!res)
                {
                    Utils.CommandLog($"The file does not contain '.pck' inside", "Error", false);
                    return false;
                }

                if (!pckReader.PCK_Embedded)
                {
                    Utils.CommandLog($"The selected file does not contain an embedded '.pck'.", "Error", false);
                    return false;
                }

                if (outFile == exeFile)
                {
                    Utils.CommandLog($"The path to the new file cannot be equal to the original file", "Error", false);
                    return false;
                }

                // rip pck
                if (outFile != null)
                    if (pckReader.RipPCKFileFromExe(outFile))
                        Utils.CommandLog($"Extracting '.pck' from '.exe' completed.", "Progress", false);
                    else
                    {
                        pckReader.Close();
                        return false;
                    }

                pckReader.Close();

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
                    catch (Exception e)
                    {
                        Utils.CommandLog("Unable to create a backup copy of the file:\n" + e.Message, "Error", false);
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
                            var read = exeOld.ReadBytes(Math.Min(65536, (int)to_write));
                            exeNew.Write(read);
                            to_write -= read.Length;
                        }
                        exeOld.Close();
                        exeNew.Close();
                    }
                    catch (Exception e)
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
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: Can't restore backup file.\n" + ex.Message);
                        }

                        Utils.CommandLog(e.Message, "Error", false);
                        return false;
                    }
                    Utils.CommandLog($"Removing '.pck' from '.exe' completed. Original file renamed to \"{oldExeFile}\"", "Progress", false);

                    // remove backup
                    try
                    {
                        if (removeBackup)
                            File.Delete(oldExeFile);
                    }
                    catch (Exception e)
                    {
                        Utils.CommandLog(e.Message, "Error", false);
                    }
                }

                return res;
            }
            else
            {
                Utils.CommandLog($"Specified file does not exists! '{exeFile}'", "Error", false);
            }
            return false;
        }

        public static bool MergePCKRun(string pckFile, string exeFile, bool removeBackup = false)
        {
            if (File.Exists(pckFile))
            {
                var pckReader = new PCKReader();
                bool res = pckReader.OpenFile(pckFile);

                if (!res)
                {
                    pckReader.Close();
                    Utils.CommandLog($"Unable to open PCK file", "Error", false);
                    return false;
                }

                if (exeFile == pckFile)
                {
                    pckReader.Close();
                    Utils.CommandLog($"The path to the new file cannot be equal to the original file", "Error", false);
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
                catch (Exception e)
                {
                    pckReader.Close();
                    Utils.CommandLog("Unable to create a backup copy of the file:\n" + e.Message, "Error", false);
                    return false;
                }

                // merge
                if (exeFile != null)
                    if (pckReader.MergePCKFileIntoExe(exeFile))
                        Utils.CommandLog($"Merging '.pck' into '.exe' completed.", "Progress", false);
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
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: Can't restore backup file.\n" + e.Message);
                        }

                        return false;
                    }

                pckReader.Close();

                // remove backup
                try
                {
                    if (removeBackup)
                        File.Delete(oldExeFile);
                }
                catch (Exception e)
                {
                    Utils.CommandLog(e.Message, "Error", false);
                }

                return res;
            }
            else
            {
                Utils.CommandLog($"Specified file does not exists! '{pckFile}'", "Error", false);
            }
            return false;
        }

        public static bool SplitPCKRun(string exeFile, string newExeName = null)
        {
            if (File.Exists(exeFile))
            {
                var name = exeFile;
                if (newExeName != null)
                {
                    name = newExeName;

                    if (name == exeFile)
                    {
                        Utils.CommandLog($"The new pair cannot be named the same as the original file: {newExeName}", "Error", false);
                        return false;
                    }

                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(name)))
                            Directory.CreateDirectory(Path.GetDirectoryName(name));

                        if (File.Exists(name))
                            File.Delete(name);

                        Utils.CommandLog($"Copying the original file to {name}", "Progress", false);
                        File.Copy(exeFile, name);
                    }
                    catch (Exception e)
                    {
                        Utils.CommandLog(e.Message, "Error", false);
                        return false;
                    }

                }

                var pckName = Path.ChangeExtension(name, ".pck");
                if (RipPCKRun(name, pckName))
                {
                    if (RipPCKRun(name, null, true))
                    {
                        Utils.CommandLog($"Split finished. Original file: \"{exeFile}\".\nNew files: \"{name}\" and \"{pckName}\"", "Progress", false);
                        return true;
                    }
                }

                if (newExeName != null)
                {
                    try
                    {
                        if (File.Exists(name))
                            File.Delete(name);

                    }
                    catch (Exception e)
                    {
                        Utils.CommandLog(e.Message, "Error", false);
                    }

                    try
                    {
                        if (File.Exists(pckName))
                            File.Delete(pckName);

                    }
                    catch (Exception e)
                    {
                        Utils.CommandLog(e.Message, "Error", false);
                    }
                }

                return false;
            }
            else
            {
                Utils.CommandLog($"Specified file does not exists! '{exeFile}'", "Error", false);
            }
            return false;
        }

    }
}
