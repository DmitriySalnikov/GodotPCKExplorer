using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public class Utils
    {
        const string version_string_pattern = @"([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})";
        static Regex VersionStringRegEx = new Regex(version_string_pattern);

        // Source: https://stackoverflow.com/a/14488941
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static void ShowMessage(string text, string title)
        {
            if (Program.CMDMode)
                Console.WriteLine($"{title}: {text}");
            else
                MessageBox.Show(text, title);
        }

        public static void CommandLog(string text, string title, bool showHelp)
        {
            if (showHelp)
                ShowMessage(text + "\n\n" + Properties.Resources.HelpText, title);
            else
                ShowMessage(text, title);
        }

        static public void ScanFoldersForFiles(string folder, List<PCKPacker.FileToPack> files, ref string basePath)
        {
            foreach (string d in Directory.EnumerateDirectories(folder))
            {
                ScanFoldersForFiles(d, files, ref basePath);
            }

            foreach (string f in Directory.EnumerateFiles(folder))
            {
                var inf = new FileInfo(f);
                files.Add(new PCKPacker.FileToPack(f, f.Replace(basePath + "\\", "res://").Replace("\\", "/"), inf.Length));
            }
        }

        public static void ConsoleWaitKey()
        {
            if (Program.CMDMode && !Program.SkipReadKey)
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        #region Console Commands

        public static bool HelpRun()
        {
            CommandLog("Help", "Help text", true);
            ConsoleWaitKey();
            return true;
        }

        public static bool OpenPCKRun(string path)
        {
            Program.CMDMode = false;
            if (File.Exists(path))
            {
                var form = new Form1();
                form.OpenFile(path);

                Program.HideConsole();
                Application.Run(form);
                return true;
            }
            else
            {
                CommandLog($"Specified file does not exists! '{path}'", "Error", false);
                ConsoleWaitKey();
            }
            return false;
        }

        public static bool InfoPCKRun(string filePath)
        {
            if (File.Exists(filePath))
            {
                var pckReader = new PCKReader();
                if (pckReader.OpenFile(filePath))
                    CommandLog($"\"{pckReader.PackPath}\"\nPack version {pckReader.PCK_VersionPack}. Godot version {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}\nVersion string for this program: {pckReader.PCK_VersionPack}.{pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}", "Pack Info", false);

                pckReader.Close();
                return true;
            }
            else
            {
                CommandLog($"Specified file does not exists! '{filePath}'", "Error", false);
                ConsoleWaitKey();
            }
            return false;
        }

        public static bool ExtractPCKRun(string filePath, string dirPath, bool overwriteExisting = true)
        {
            if (File.Exists(filePath))
            {
                var pckReader = new PCKReader();
                pckReader.OpenFile(filePath);
                pckReader.ExtractAllFiles(dirPath, overwriteExisting);
                pckReader.Close();
                return true;
            }
            else
            {
                CommandLog($"Specified file does not exists! '{filePath}'", "Error", false);
                ConsoleWaitKey();
            }
            return false;
        }

        public static bool PackPCKRun(string dirPath, string filePath, string strVer)
        {
            if (Directory.Exists(dirPath))
            {
                var pckPacker = new PCKPacker();

                var files = new List<PCKPacker.FileToPack>();
                ScanFoldersForFiles(dirPath, files, ref dirPath);

                var ver = new PCKPacker.PCKVersion();

                var tmpCheck = VersionStringRegEx.Match(strVer);
                if (tmpCheck.Success)
                {
                    var digits = tmpCheck.Value.Split('.');
                    ver.PackVersion = int.Parse(digits[0]);
                    ver.Major = int.Parse(digits[1]);
                    ver.Minor = int.Parse(digits[2]);
                    ver.Revision = int.Parse(digits[3]);
                }
                else
                {
                    CommandLog("The version is specified incorrectly.", "Error", true);
                    ConsoleWaitKey();
                    return false;
                }

                return pckPacker.PackFiles(filePath, files, 8, ver); ;
            }
            else
            {
                CommandLog($"Specified directory does not exists! '{dirPath}'", "Error", false);
                ConsoleWaitKey();
            }
            return false;
        }

        public static bool RipPCKRun(string exeFile, string outFile = null, bool removeBackup = false)
        {
            if (File.Exists(exeFile))
            {
                var pckReader = new PCKReader();
                bool res = pckReader.OpenFile(exeFile);
                long to_write = pckReader.PCK_StartPosition;

                if (outFile == exeFile)
                {
                    CommandLog($"The path to the new file cannot be equal to the original file", "Error", false);
                    ConsoleWaitKey();
                    return false;
                }

                if (outFile != null)
                    if (pckReader.RipPCKFileFromExe(outFile))
                        CommandLog($"Extracting '.pck' from '.exe' completed.", "Progress", false);
                    else
                    {
                        pckReader.Close();
                        return false;
                    }

                pckReader.Close();

                if (res && outFile == null)
                {
                    var oldExeFile = Path.ChangeExtension(exeFile, "old" + Path.GetExtension(exeFile));
                    try
                    {
                        if (File.Exists(oldExeFile))
                            File.Delete(oldExeFile);
                        File.Move(exeFile, oldExeFile);
                    }
                    catch (Exception e)
                    {
                        CommandLog(e.Message, "Error", false);
                        ConsoleWaitKey();
                        return false;
                    }

                    BinaryReader exeOld = null;
                    BinaryWriter exeNew = null;

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
                        CommandLog(e.Message, "Error", false);
                        ConsoleWaitKey();
                        return false;
                    }
                    CommandLog($"Removing '.pck' from '.exe' completed. Original file renamed to \"{oldExeFile}\"", "Progress", false);

                    try
                    {
                        if (removeBackup)
                            File.Delete(oldExeFile);
                    }
                    catch (Exception e)
                    {
                        CommandLog(e.Message, "Error", false);
                    }
                }

                return res;
            }
            else
            {
                CommandLog($"Specified file does not exists! '{exeFile}'", "Error", false);
                ConsoleWaitKey();
            }
            return false;
        }

        public static bool SplitPCKRun(string exeFile, string newPairName = null)
        {
            if (File.Exists(exeFile))
            {
                var name = exeFile;
                if (newPairName != null)
                {
                    name = Path.Combine(Path.GetDirectoryName(exeFile), newPairName + Path.GetExtension(exeFile));

                    if (name == exeFile)
                    {
                        CommandLog($"The new pair cannot be named the same as the original file: {newPairName}", "Error", false);
                        ConsoleWaitKey();
                        return false;
                    }

                    try
                    {
                        if (File.Exists(name))
                            File.Delete(name);

                        CommandLog($"Copying the original file to {name}", "Progress", false);
                        File.Copy(exeFile, name);
                    }
                    catch (Exception e)
                    {
                        CommandLog(e.Message, "Error", false);
                        return false;
                    }

                }

                var pckName = Path.ChangeExtension(name, ".pck");
                if (RipPCKRun(name, pckName))
                    if (RipPCKRun(name, null, true))
                    {
                        CommandLog($"Split finished. Original file: \"{exeFile}\".\nNew files: \"{name}\" and \"{pckName}\"", "Progress", false);
                        return true;
                    }
                return false;
            }
            else
            {
                CommandLog($"Specified file does not exists! '{exeFile}'", "Error", false);
                ConsoleWaitKey();
            }
            return false;
        }

        #endregion //Console Commands

    }
}
