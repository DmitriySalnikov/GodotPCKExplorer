using System;
using System.IO;
using System.Linq;

namespace GodotPCKExplorer.UI
{
    internal static class ConsoleCommands
    {
        static bool runWithArgs = false;

        // TODO add CMD tests?..
        public static bool RunCommand(string[] args)
        {
            return RunCommandInternal(args);
        }

        public static bool RunCommandInternal(string[] args)
        {
            runWithArgs = false;

            // Skip exe path
            try
            {
                if (args.Length > 0)
                    if (Path.GetFullPath(args[0]) == AppContext.BaseDirectory)
                        args = args.Skip(1).ToArray();
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }

            if (args.Length > 0)
                IterateCommands(
                 () => { if (args[0] == "-h" || args[0] == "/?" || args[0] == "--help") HelpCommand(); },
                 () => { if (args[0] == "-i") InfoPCKCommand(args, false); },
                 () => { if (args[0] == "-l") InfoPCKCommand(args, true); },
                 () => { if (args[0] == "-e") ExtractPCKCommand(args); },
                 () => { if (args[0] == "-es") ExtractSkipExistingPCKCommand(args); },
                 () => { if (args[0] == "-p") PackPCKCommand(args, false); },
                 () => { if (args[0] == "-pe") PackPCKCommand(args, true); },
                 () => { if (args[0] == "-m") MergePCKCommand(args); },
                 () => { if (args[0] == "-r") RipPCKCommand(args); },
                 () => { if (args[0] == "-s") SplitPCKCommand(args); },
                 () => { if (args[0] == "-c") ChangeVersionPCKCommand(args); },
                 () => { if (args[0] == "-o" || args.Length == 1) OpenPCKCommand(args); },
                 () => { HelpCommand(); }
                 );

            return runWithArgs;
        }

        static void IterateCommands(params Action[] commands)
        {
            foreach (var cmd in commands)
            {
                cmd();
                if (runWithArgs)
                    return;
            }
        }

        static void HelpCommand()
        {
            runWithArgs = true;

            Program.Log("Help");
            Program.LogHelp();
            return;

        }

        static bool TestEncryptionKey(string key)
        {
            if (!PCKUtils.HexStringValidate(key, 256 / 8))
            {
                Program.Log("Invalid encryption key provided!");
                return false;
            }
            return true;
        }

        static void OpenPCKCommand(string[] args)
        {
            string? path = null;
            string? encKey = null;

            try
            {
                if (args.Length >= 2)
                {
                    path = Path.GetFullPath(args[1]);

                    if (args.Length == 3)
                    {
                        encKey = args[2];

                        if (!TestEncryptionKey(encKey))
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            if (path == null)
            {
                try
                {
                    if (args.Length == 1)
                    {
                        path = Path.GetFullPath(args[0]);
                    }
                }
                catch (Exception ex)
                {
                    Program.Log(ex);
                    return;
                }
            }

            if (path != null)
            {
                if (File.Exists(path))
                {
                    runWithArgs = false;
                    // force enable
                    Program.EnableMessageBoxes();
                    Program.OpenMainForm(path, encKey);
                    runWithArgs = true;
                }
                else
                {
                    Program.Log($"Specified file does not exists! '{path}'");
                }
            }
        }

        static void InfoPCKCommand(string[] args, bool list_files)
        {
            runWithArgs = true;

            string filePath;
            string? encKey = null;

            try
            {
                if (args.Length >= 2 && args.Length <= 3)
                {
                    filePath = Path.GetFullPath(args[1].Replace("\"", ""));

                    if (args.Length == 3)
                    {
                        encKey = args[2];

                        if (!TestEncryptionKey(encKey))
                            return;
                    }
                }
                else
                {
                    Program.Log("Path to file not specified! Or incorrect number of arguments specified!");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            PCKActions.PrintInfo(filePath, list_files, encKey);
        }

        static void ExtractPCKCommand(string[] args, bool overwriteExisting = true)
        {
            runWithArgs = true;

            string filePath;
            string dirPath;
            string? encKey = null;

            try
            {
                if (args.Length >= 3 && args.Length <= 4)
                {
                    filePath = Path.GetFullPath(args[1].Replace("\"", ""));
                    dirPath = Path.GetFullPath(args[2].Replace("\"", ""));

                    if (args.Length > 3)
                    {
                        encKey = args[3];

                        if (!TestEncryptionKey(encKey))
                            return;
                    }
                }
                else
                {
                    Program.Log($"Invalid number of arguments! Expected 3 or 4, but got {args.Length}");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            PCKActions.Extract(filePath, dirPath, overwriteExisting, encKey: encKey);
        }

        static void ExtractSkipExistingPCKCommand(string[] args)
        {
            ExtractPCKCommand(args, false);
        }

        static void PackPCKCommand(string[] args, bool embed)
        {
            runWithArgs = true;

            string dirPath;
            string filePath;
            string strVer;
            uint alignment = 16;
            string? encKey = null;
            string encType = "both";

            try
            {
                if (args.Length >= 4 && args.Length <= 7)
                {
                    dirPath = Path.GetFullPath(args[1].Replace("\"", ""));
                    filePath = Path.GetFullPath(args[2].Replace("\"", ""));
                    strVer = args[3];

                    if (args.Length > 4)
                    {
                        alignment = uint.Parse(args[4]);

                        if (args.Length > 5)
                        {
                            encKey = args[5];

                            if (!TestEncryptionKey(encKey))
                                return;

                            if (args.Length > 6)
                            {
                                encType = args[6];

                                if (!new string[] { "both", "index", "files" }.Contains(encType))
                                {
                                    Program.Log($"Invalid encryption type: {encType}");
                                    Program.LogHelp();
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Program.Log($"Invalid number of arguments! Expected from 4 to 6, but got {args.Length}");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            var encIndex = false;
            var encFiles = false;

            switch (encType)
            {
                case "both":
                    encIndex = true;
                    encFiles = true;
                    break;
                case "index":
                    encIndex = true;
                    break;
                case "files":
                    encFiles = true;
                    break;
            }

            PCKActions.Pack(dirPath, filePath, strVer, alignment, embed, encKey, encIndex, encFiles);
        }

        static void RipPCKCommand(string[] args)
        {
            runWithArgs = true;

            string exeFile;
            string? outFile = null;

            try
            {
                if (args.Length >= 2)
                {
                    exeFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    if (args.Length == 3)
                        outFile = Path.GetFullPath(args[2].Replace("\"", ""));

                    if (args.Length > 4)
                    {
                        Program.Log($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        Program.LogHelp();
                        return;
                    }
                }
                else
                {
                    Program.Log($"Path to file or directory not specified!");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            PCKActions.Rip(exeFile, outFile);
        }

        static void MergePCKCommand(string[] args)
        {
            runWithArgs = true;

            string pckFile;
            string exeFile;

            try
            {
                if (args.Length == 3)
                {
                    pckFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    exeFile = Path.GetFullPath(args[2].Replace("\"", ""));
                }
                else
                {
                    Program.Log($"Invalid number of arguments! Expected 3, but got {args.Length}");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            PCKActions.Merge(pckFile, exeFile);
        }

        static void SplitPCKCommand(string[] args)
        {
            runWithArgs = true;

            string exeFile;
            string? pairName = null;
            try
            {
                if (args.Length >= 2)
                {
                    exeFile = Path.GetFullPath(args[1].Replace("\"", ""));

                    if (args.Length == 3)
                        pairName = Path.GetFullPath(args[2].Replace("\"", ""));

                    if (args.Length > 3)
                    {
                        Program.Log($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        Program.LogHelp();
                        return;
                    }
                }
                else
                {
                    Program.Log($"Path to file not specified!");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            PCKActions.Split(exeFile, pairName);
        }

        static void ChangeVersionPCKCommand(string[] args)
        {
            runWithArgs = true;

            string pckFile;
            string strVer;

            try
            {
                if (args.Length >= 3 && args.Length <= 4)
                {
                    pckFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    strVer = args[2];
                }
                else
                {
                    Program.Log($"Invalid number of arguments! Expected 3, but got {args.Length}");
                    Program.LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.Log(ex);
                return;
            }

            PCKActions.ChangeVersion(pckFile, strVer);
        }
    }
}
