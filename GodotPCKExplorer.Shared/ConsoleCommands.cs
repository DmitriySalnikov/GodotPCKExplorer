#if CONSOLE_BUILD
using GodotPCKExplorer.Cmd;
#else
using GodotPCKExplorer.UI;
#endif
using GodotPCKExplorer;
using System.Diagnostics;
using System.Reflection;

// ExitCode = 1: Error
// ExitCode = 2: Exception

namespace GodotPCKExplorer.Shared

{
    public static class ConsoleCommands
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
#if CONSOLE_BUILD
                Program.ExitCode = 1;
#endif
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

        static bool ValidateEncryptionKey(string key)
        {
            if (!PCKUtils.HexStringValidate(key, 256 / 8))
            {
#if CONSOLE_BUILD
                Program.ExitCode = 1;
#endif
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

                        if (!ValidateEncryptionKey(encKey))
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
#if CONSOLE_BUILD
                    runWithArgs = true;
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        var split = Path.GetFileName(Assembly.GetExecutingAssembly().Location).Split('.');
                        try
                        {
                            // !! Sync with UI !!
                            var proc = Process.Start(split[0] + ".UI.exe", $"-o \"{path}\" {encKey ?? ""}");
                            proc.WaitForExit();

                            Program.ExitCode = proc.ExitCode;
                            return;
                        }
                        catch (Exception ex)
                        {
                            Program.Log(ex);
                            return;
                        }
                    }
                    else
                    {
                        Program.ExitCode = 1;
                        throw new NotImplementedException("The UI is only supported on Windows");
                    }
#else
                    runWithArgs = false;
                    // force enable
                    Program.EnableMessageBoxes();
                    // !! Sync with console !!
                    Program.OpenMainForm(path, encKey);
                    runWithArgs = true;
                    return;
#endif
                }
                else
                {
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
                    Program.Log($"Specified file does not exists! '{path}'");
                }
            }
            else
            {
#if CONSOLE_BUILD
                Program.ExitCode = 1;
#endif
                Program.Log($"No path specified");
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

                        if (!ValidateEncryptionKey(encKey))
                            return;
                    }
                }
                else
                {
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.PrintInfo(filePath, list_files, encKey);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
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

                        if (!ValidateEncryptionKey(encKey))
                            return;
                    }
                }
                else
                {
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.Extract(filePath, dirPath, overwriteExisting, encKey: encKey);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
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

                            if (!ValidateEncryptionKey(encKey))
                                return;

                            if (args.Length > 6)
                            {
                                encType = args[6];

                                if (!new string[] { "both", "index", "files" }.Contains(encType))
                                {
#if CONSOLE_BUILD
                                    Program.ExitCode = 1;
#endif
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
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.Pack(dirPath, filePath, strVer, alignment, embed, encKey, encIndex, encFiles);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
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
#if CONSOLE_BUILD
                        Program.ExitCode = 1;
#endif
                        Program.Log($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        Program.LogHelp();
                        return;
                    }
                }
                else
                {
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.Rip(exeFile, outFile);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
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
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.Merge(pckFile, exeFile);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
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
#if CONSOLE_BUILD
                        Program.ExitCode = 1;
#endif
                        Program.Log($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        Program.LogHelp();
                        return;
                    }
                }
                else
                {
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.Split(exeFile, pairName);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
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
#if CONSOLE_BUILD
                    Program.ExitCode = 1;
#endif
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

            var res = PCKActions.ChangeVersion(pckFile, strVer);
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 1;
#endif
        }
    }
}
