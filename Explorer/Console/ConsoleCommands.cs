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
        public static int? ExitCode { get; private set; } = null;
        static bool runWithArgs = false;
        private static readonly string[] encyptionTypes = ["both", "index", "files"];

        static Action<string>? _log = null;
        static Action<Exception>? _logEx = null;

        public static void InitLogs(Action<string> log, Action<Exception> logEx)
        {
            _log = log;
            _logEx = logEx;
        }

        public static bool RunCommand(params string[] args)
        {
            runWithArgs = false;
            ExitCode = null;

            // Skip exe path
            try
            {
                if (args.Length > 0)
                    if (args[0].StartsWith(AppDomain.CurrentDomain.BaseDirectory) || args[0].StartsWith(AppDomain.CurrentDomain.FriendlyName))
                        args = args.Skip(1).ToArray();
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            var help = () => { Log("Please specify which action you want to run."); HelpCommand(); };

            if (args.Length > 0)
            {
                IterateCommands(
#if CONSOLE_BUILD
                 () => { if (args[0] == "-i") InfoPCKCommand(args, false); },
                 () => { if (args[0] == "-l") InfoPCKCommand(args, true); },
                 () => { if (args[0] == "-e") ExtractPCKCommand(args); },
                 () => { if (args[0] == "-es") ExtractSkipExistingPCKCommand(args); },
                 () => { if (args[0] == "-p") PackPCKCommand(args, false, false); },
                 () => { if (args[0] == "-pe") PackPCKCommand(args, false, true); },
                 () => { if (args[0] == "-pc") PackPCKCommand(args, true, false); },
                 () => { if (args[0] == "-pce") PackPCKCommand(args, true, true); },
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

            if (!ExitCode.HasValue)
                SetResult(runWithArgs);

            return runWithArgs;
        }

        static void LogHelpText()
        {
            string helpText = @"Godot can embed '.pck' files into other files.
Therefore, GodotPCKExplorer can open both '.pck' and files with embedded 'pck'.

Paths and other arguments must be without spaces or inside quotes: ""some path""
The PACK_VERSION in the version can be 1 for Godot 3 or 2 for Godot 4.
Encryption only works with '.pck' for Godot 4.

{} - Optional arguments

Examples of valid commands:
";

#if !CONSOLE_BUILD
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                helpText += @"-o	Open pack file using UI
	-o [path to pack] {[encryption key]}
	-o C:/Game.exe
	-o C:/Game.pck

";
            }
#endif

#if CONSOLE_BUILD
            helpText += @"-i	Show pack file info
	-i [path to pack]
	-i C:/Game.exe
	-i C:/Game.pck
	
-l	Show pack file info with a list of packed files
	-l [path to pack] {[encryption key]}
	-l C:/Game.exe
	-l C:/Game.pck

-e	Extract content from a pack to a folder. Automatically overwrites existing files
	Instead of the key, you can specify ""skip"" or ""encrypted"" to skip or extract files without decryption, respectively
	-e [path to pack] [path to output folder] {[encryption key]}
	-e C:/Game.exe ""C:/Path with Spaces"" 
	-e C:/Game.pck Output_dir 7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA
	-e C:/Game.pck Output_dir skip
	-e C:/Game.pck Output_dir encrypted

-es	Extract like -e but Skip existing files

-p	Pack content of folder into .pck file
	The version should be in this format: PACK_VERSION.GODOT_MINOR._MAJOR._PATCH
	-p [folder] [output pack file] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-p ""C:/Directory with files"" C:/Game_New.pck 1.3.2.0
	-p ""C:/Directory with files"" C:/Game_New.pck 2.4.0.1 ""some/prefix/dir/""
	-p ""C:/Directory with files"" C:/Game_New.pck 2.4.0.1 """" 7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA files

-pe	Pack Embedded. Equal to -p, but embed '.pck' into target file
	-pe [folder] [exe to pack into] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-pe ""C:/Directory with files"" C:/Game.exe 1.3.2.0 ""mod_folder/""

-pc	Copy the contents of '.pck' file into a new '.pck' file and patch it with the contents of the folder. Similar to -p
	Encryption and prefix are applied only to the contents of the folder.
	-pc [input pck] [folder] [output pack file] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-pc C:/Game.pck ""C:/Directory with modified files"" C:/Game_New.pck 2.4.3.0 ""some/prefix/dir/""

-pce	Equal to -pc, but embed '.pck' into target file
	-pce [input pck] [folder] [exe to pack into] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-pce C:/Game.pck ""C:/Directory with files"" C:/Game.exe 2.4.3.0 ""mod_folder/""

-m	Merge pack into target file. So you can copy the '.pck' from one file to another
	-m [path to pack] [file to merge into]
	-m C:/Game.pck C:/Game.exe
	-m C:/GameEmbedded.exe C:/Game.exe

-r	Rip '.pck' from file
	If the output file is not specified, it will just be deleted from the original file
	Otherwise, it will be extracted without changing the original file
	-r [path to file] {[output pack file]}
	-r C:/Game.exe C:/Game.pck
	-r C:/Game.exe

-s	Split file with embedded '.pck' into two separated files
	-s [path to file] {[path to the new file (this name will also be used for '.pck')]}
	-s C:/Game.exe ""C:/Out Folder/NewGameSplitted.exe""
	-s C:/Game.exe

-c	Change version of the '.pck'
	-c [path to pck] [new version]
	-c C:/Game.pck 1.3.4.1
	-c C:/Game.exe 2.4.0.2
";
#endif

            Log("\n" + helpText);
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

            Log("Help:");
            LogHelpText();
            return;

        }

        static void LogErr(string err)
        {
#if CONSOLE_BUILD
            ExitCode = 2;
#endif
            Log(err);
        }

        static void LogErrHelp(string err)
        {
#if CONSOLE_BUILD
            ExitCode = 3;
#endif
            Log(err);
            LogErrHelpSeparator();
            LogHelpText();
        }

        static void LogErrHelpSeparator()
        {
            Log("");
            Log("-------------------------------------------------------------");
            Log("");
        }

        static void Log(string txt)
        {
            _log?.Invoke(txt);
        }

        static void Log(Exception ex)
        {
#if CONSOLE_BUILD
            ExitCode = 1;
#endif
            _logEx?.Invoke(ex);
        }

        static void SetResult(bool res)
        {
#if CONSOLE_BUILD
            ExitCode = res ? 0 : 4;
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
                    LogErrHelp("Path to the file not specified! Or incorrect number of arguments specified!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
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
            PCKExtractNoEncryptionKeyMode noKeyMode = PCKExtractNoEncryptionKeyMode.Cancel;

            try
            {
                if (args.Length >= 3 && args.Length <= 4)
                {
                    filePath = Path.GetFullPath(args[1]);
                    dirPath = Path.GetFullPath(args[2]);

                    if (args.Length > 3)
                    {
                        encKey = args[3];

                        if (encKey == "skip" || encKey == "encrypted")
                        {
                            if (encKey == "skip")
                            {
                                noKeyMode = PCKExtractNoEncryptionKeyMode.Skip;
                            }
                            else
                            {
                                noKeyMode = PCKExtractNoEncryptionKeyMode.AsIs;
                            }
                            encKey = null;
                        }
                        else
                        {
                            if (!ValidateEncryptionKey(encKey))
                                return;
                        }
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
                Log(ex);
                return;
            }

            var res = PCKActions.Extract(filePath, dirPath, overwriteExisting, encKey: encKey, noKeyMode: noKeyMode);
            SetResult(res);
        }

        static void ExtractSkipExistingPCKCommand(string[] args)
        {
            ExtractPCKCommand(args, false);
        }

        static void PackPCKCommand(string[] args, bool patch, bool embed)
        {
            runWithArgs = true;

            string patchPck = "";
            string dirPath;
            string filePath;
            string strVer;
            string prefix = "";
            string? encKey = null;
            string encType = "both";

            try
            {
                int min_args = patch ? 5 : 4;
                int max_args = patch ? 8 : 7;

                if (args.Length >= min_args && args.Length <= max_args)
                {
                    int idx = 1;

                    if (patch)
                        patchPck = Path.GetFullPath(args[idx++]);

                    dirPath = Path.GetFullPath(args[idx++]);
                    filePath = Path.GetFullPath(args[idx++]);
                    strVer = args[idx++];

                    if (args.Length > idx)
                    {
                        prefix = args[idx++];

                        if (args.Length > idx)
                        {
                            encKey = args[idx++];

                            if (!ValidateEncryptionKey(encKey))
                                return;

                            if (args.Length > idx)
                            {
                                encType = args[idx++];

                                if (!encyptionTypes.Contains(encType))
                                {
                                    LogErr($"Invalid encryption type: {encType}. Only \"both\", \"index\" and \"files\" can be used as encryption mode.");
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    LogErrHelp($"Invalid number of arguments! Expected from {min_args} to {max_args}, but got {args.Length}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            var encIndex = false;
            var encFiles = false;

            if (encKey != null)
            {
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
            }

            var res = PCKActions.Pack(dirPath, filePath, strVer, pckToPatch: patchPck, packPathPrefix: prefix, alignment: 16, embed: embed, encKey: encKey, encIndex: encIndex, encFiles: encFiles);
            SetResult(res);
        }

        static void RipPCKCommand(string[] args)
        {
            runWithArgs = true;

            string exeFile;
            string? outFile = null;

            try
            {
                int idx = 1;
                if (args.Length > idx)
                {
                    exeFile = Path.GetFullPath(args[idx++]);
                    if (args.Length > idx)
                    {
                        outFile = Path.GetFullPath(args[idx++]);

                        if (args.Length > idx)
                        {
                            LogErrHelp($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                            return;
                        }
                    }
                }
                else
                {
                    LogErrHelp($"Path to the file or directory not specified!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
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
                Log(ex);
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
                    LogErrHelp($"Path to the file not specified!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
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
                Log(ex);
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
                Log(ex);
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
                    Log(ex);
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
                        var exe_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName.Split('.')[0]);
                        try
                        {
                            // !! Sync with UI !!
                            var proc = Process.Start(exe_path + ".UI.exe", $"-o \"{path}\" {encKey ?? ""}");
                            //proc.WaitForExit();
                            //ExitCode = proc.ExitCode;

                            SetResult(true);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Log(ex);
                            return;
                        }
                    }
                    else
                    {
                        ExitCode = 1;
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
