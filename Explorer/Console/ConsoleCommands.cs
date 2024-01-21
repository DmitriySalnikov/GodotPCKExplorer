#if CONSOLE_BUILD
using GodotPCKExplorer.Cmd;
using System.Diagnostics;
using System.Reflection;
#else
using GodotPCKExplorer.UI;
#endif

// ExitCode = 1: Exception
// ExitCode = 2: Error
// ExitCode = 3: Error Help
// ExitCode = 4: Error Failed Command

namespace GodotPCKExplorer.Cmd
{
    public static class ConsoleCommands
    {
        static bool runWithArgs = false;
        private static readonly string[] encyptionTypes = ["both", "index", "files"];

        // TODO add CMD tests?..
        public static bool RunCommand(string[] args)
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
                LogEx(ex);
            }

            var help = () => { Program.Log("Please specify which action you want to run."); HelpCommand(); };

            if (args.Length > 0)
            {
                IterateCommands(
#if CONSOLE_BUILD
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
#endif
                 () => { if (args[0] == "-h" || args[0] == "/?" || args[0] == "--help") HelpCommand(); },
#if CONSOLE_BUILD
                 () => { if (args[0] == "-o") OpenPCKCommand(args); },
#else
                 () => { if (args[0] == "-o" || args.Length == 1) OpenPCKCommand(args); },
#endif
                 () => { help(); }
                 );
            }
#if CONSOLE_BUILD
            else
            {
                help();
            }
#endif

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

            Program.Log("Help:");
            Program.LogHelp();
            return;

        }

        static void LogErr(string err)
        {
#if CONSOLE_BUILD
            Program.ExitCode = 2;
#endif
            Program.Log(err);
        }

        static void LogErrHelp(string err)
        {
#if CONSOLE_BUILD
            Program.ExitCode = 3;
#endif
            Program.Log(err);
            Program.LogHelp();
        }

        static void LogEx(Exception ex)
        {
#if CONSOLE_BUILD
            Program.ExitCode = 1;
#endif
            Program.Log(ex);
        }

        static void SetResult(bool res)
        {
#if CONSOLE_BUILD
            Program.ExitCode = res ? 0 : 4;
#endif
        }

        static bool ValidateEncryptionKey(string key)
        {
            if (!PCKUtils.HexStringValidate(key, 256 / 8))
            {
                LogErr("Invalid encryption key provided!");
                return false;
            }
            return true;
        }

#if CONSOLE_BUILD
        static void InfoPCKCommand(string[] args, bool list_files)
        {
            runWithArgs = true;

            string filePath;
            string? encKey = null;

            try
            {
                if (args.Length >= 2 && args.Length <= 3)
                {
                    filePath = Path.GetFullPath(args[1]);

                    if (args.Length == 3)
                    {
                        encKey = args[2];

                        if (!ValidateEncryptionKey(encKey))
                            return;
                    }
                }
                else
                {
                    LogErrHelp("Path to file not specified! Or incorrect number of arguments specified!");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            var res = PCKActions.PrintInfo(filePath, list_files, encKey);
            SetResult(res);
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
                    filePath = Path.GetFullPath(args[1]);
                    dirPath = Path.GetFullPath(args[2]);

                    if (args.Length > 3)
                    {
                        encKey = args[3];

                        if (!ValidateEncryptionKey(encKey))
                            return;
                    }
                }
                else
                {
                    LogErrHelp($"Invalid number of arguments! Expected 3 or 4, but got {args.Length}");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            var res = PCKActions.Extract(filePath, dirPath, overwriteExisting, encKey: encKey);
            SetResult(res);
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
            string prefix = "";
            string? encKey = null;
            string encType = "both";

            try
            {
                if (args.Length >= 4 && args.Length <= 7)
                {
                    dirPath = Path.GetFullPath(args[1]);
                    filePath = Path.GetFullPath(args[2]);
                    strVer = args[3];

                    if (args.Length > 4)
                    {
                        prefix = args[4];

                        if (args.Length > 5)
                        {
                            encKey = args[5];

                            if (!ValidateEncryptionKey(encKey))
                                return;

                            if (args.Length > 6)
                            {
                                encType = args[6];

                                if (!encyptionTypes.Contains(encType))
                                {
                                    LogErrHelp($"Invalid encryption type: {encType}");
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    LogErrHelp($"Invalid number of arguments! Expected from 4 to 6, but got {args.Length}");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
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

            var res = PCKActions.Pack(dirPath, filePath, strVer, prefix, 16, embed, encKey, encIndex, encFiles);
            SetResult(res);
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
                    exeFile = Path.GetFullPath(args[1]);
                    if (args.Length == 3)
                        outFile = Path.GetFullPath(args[2]);

                    if (args.Length > 4)
                    {
                        LogErrHelp($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        return;
                    }
                }
                else
                {
                    LogErrHelp($"Path to file or directory not specified!");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            var res = PCKActions.Rip(exeFile, outFile);
            SetResult(res);
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
                    pckFile = Path.GetFullPath(args[1]);
                    exeFile = Path.GetFullPath(args[2]);
                }
                else
                {
                    LogErrHelp($"Invalid number of arguments! Expected 3, but got {args.Length}");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            var res = PCKActions.Merge(pckFile, exeFile);
            SetResult(res);
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
                    exeFile = Path.GetFullPath(args[1]);

                    if (args.Length == 3)
                        pairName = Path.GetFullPath(args[2]);

                    if (args.Length > 3)
                    {
                        LogErrHelp($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        return;
                    }
                }
                else
                {
                    LogErrHelp($"Path to file not specified!");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            var res = PCKActions.Split(exeFile, pairName);
            SetResult(res);
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
                    LogErrHelp($"Invalid number of arguments! Expected 3, but got {args.Length}");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            var res = PCKActions.ChangeVersion(pckFile, strVer);
            SetResult(res);
        }
#endif

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
                LogEx(ex);
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
                    LogEx(ex);
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
                            LogEx(ex);
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
                    LogErr($"Specified file does not exists! '{path}'");
                }
            }
            else
            {
                LogErr($"No path specified");
            }
        }
    }
}
