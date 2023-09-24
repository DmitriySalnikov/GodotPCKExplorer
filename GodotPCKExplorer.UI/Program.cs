using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GodotPCKExplorer;

namespace GodotPCKExplorer.UI
{
    internal static class Program
    {
        public static string AppName = "GodotPCKExplorer";
        public static string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

        public static bool CMDMode = false;
        public static Form1 mainForm = null;

        static Logger logger;
        static bool runWithArgs = false;

        // https://stackoverflow.com/a/3571628/8980874
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static bool IsStylesInited = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Init();

            RunCommandInternal(args, false);

            if (!runWithArgs)
            {
                // run..
                CMDMode = false;

                mainForm = new Form1();
                Application.Run(mainForm);
            }

            //Environment.ExitCode = 0;
            return;
        }

        public static void Init()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);

            // InvariantCulture for console and UI
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Application.EnableVisualStyles();

            if (!IsStylesInited)
            {
                Application.SetCompatibleTextRenderingDefault(false);
                IsStylesInited = true;
            }

            PCKActions.Init(new ProgressReporterUI());

            ShowConsole();
            CMDMode = true;
            Log("");
        }

        public static void Cleanup()
        {
            logger?.Dispose();
            logger = null;

            mainForm?.Dispose();
            mainForm = null;
        }

        #region Logs
        class ProgressReporterUI : IProgressReporter
        {
            int prev_progress = 0;
            DateTime prev_time = DateTime.Now;

            public void Log(string txt)
            {
                Program.Log(txt);
            }

            public void Log(Exception ex)
            {
                Program.Log(ex);
            }

            public void LogProgress(string operation, int percent)
            {
                if (((DateTime.Now - prev_time).TotalSeconds > 1) || (prev_progress != percent && percent - prev_progress >= 5))
                {
                    Program.LogProgress(operation, $"{Math.Max(Math.Min(percent, 100), 0)}%");

                    prev_progress = percent;
                    prev_time = DateTime.Now;
                }

            }

            public void LogProgress(string operation, string str)
            {
                Program.LogProgress(operation, str);
            }

            public PCKDialogResult ShowMessage(string text, string title, GodotPCKExplorer.MessageType messageType = GodotPCKExplorer.MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
            {
                if (!CMDMode)
                {
                    return (PCKDialogResult)Program.ShowMessage(text, title, (MessageType)messageType, (MessageBoxButtons)boxButtons);
                }

                Program.Log(text);
                return PCKDialogResult.OK;
            }

            public PCKDialogResult ShowMessage(Exception ex, GodotPCKExplorer.MessageType messageType = GodotPCKExplorer.MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
            {
                if (!CMDMode)
                {
                    return (PCKDialogResult)Program.ShowMessage(ex, (MessageType)messageType, (MessageBoxButtons)boxButtons);
                }

                Program.Log(ex);
                return PCKDialogResult.OK;
            }
        }

        public static void LogProgress(string operation, string txt)
        {
            if (logger == null)
                logger = new Logger("log.txt");

            logger.Write($"[Progress] {operation}: {txt}");
        }

        public static void Log(string txt)
        {
            if (logger == null)
                logger = new Logger("log.txt");

            logger.Write(txt);
        }

        public static void Log(Exception ex)
        {
            if (logger == null)
                logger = new Logger("log.txt");

            logger.Write(ex);
        }

        public static DialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, MessageBoxButtons boxButtons = MessageBoxButtons.OK, DialogResult defaultRes = DialogResult.OK)
        {
            Log($"[MSG {messageType}] \"{title}\": {text}");

            if (!CMDMode)
            {
                MessageBoxIcon icon = MessageBoxIcon.None;
                switch (messageType)
                {
                    case MessageType.Info:
                        icon = MessageBoxIcon.Information;
                        break;
                    case MessageType.Error:
                        icon = MessageBoxIcon.Error;
                        break;
                    case MessageType.Warning:
                        icon = MessageBoxIcon.Warning;
                        break;
                }

#if DEV_ENABLED
                System.Diagnostics.Debugger.Break();
#endif
                var res = MessageBox.Show(text, title, boxButtons, icon);
                Log($"[MSG {messageType}] \"{title}\": Result = '{res}'");
                return res;
            }

#if DEV_ENABLED
            System.Diagnostics.Debugger.Break();
#endif
            Log($"[MSG {messageType}] \"{title}\": Skipped message box with result = '{defaultRes}'");
            return defaultRes;
        }

        public static DialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, MessageBoxButtons boxButtons = MessageBoxButtons.OK)
        {
            var res = ShowMessage(ex.Message, ex.GetType().Name, messageType, boxButtons);
            Log(ex);
            return res;
        }

        public static void LogHelp()
        {
            Log("\n" + Properties.Resources.HelpText);
        }

        #endregion

        static public void RunCommand(string[] args)
        {
            RunCommandInternal(args, true);
        }

        static void RunCommandInternal(string[] args, bool restore_params)
        {
            var old_run_with_args = runWithArgs;
            var old_cmd_mode = CMDMode;

            // Skip exe path
            try
            {
                if (args.Length > 0)
                    if (Path.GetFullPath(args[0]) == Application.ExecutablePath)
                        args = args.Skip(1).ToArray();
            }
            catch (Exception ex)
            {
                Log(ex);
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
                 () => { if (args[0] == "-o" || args.Length == 1) OpenPCKCommand(args); }
                 );

            if (restore_params)
            {
                runWithArgs = old_run_with_args;
                CMDMode = old_cmd_mode;
            }
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

        static public void ShowConsole()
        {
            if (!Utils.IsRunningOnMono())
                ShowWindow(GetConsoleWindow(), SW_SHOW);
        }

        static public void HideConsole()
        {
            if (!Utils.IsRunningOnMono())
                ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        static void HelpCommand()
        {
            runWithArgs = true;
            Log("Help");
            LogHelp();
            return;

        }

        static bool TestEncryptionKey(string key)
        {
            if (!PCKUtils.HexStringValidate(key, 256 / 8))
            {
                Log("Invalid encryption key provided!");
                return false;
            }
            return true;
        }

        public static bool OpenPCKRun(string path, string encKey = null)
        {
            CMDMode = false;
            if (File.Exists(path))
            {
                if (mainForm == null)
                {
                    mainForm = new Form1();
                    mainForm.OpenFile(path, encKey);

                    Application.Run(mainForm);
                }
                else
                {
                    mainForm.OpenFile(path, encKey);
                }
                return true;
            }
            else
            {
                Log($"Specified file does not exists! '{path}'");
            }
            return false;
        }

        public static void ClosePCK()
        {
            mainForm?.CloseFile();
        }

        static void OpenPCKCommand(string[] args)
        {
            string path = null;
            string encKey = null;

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
                runWithArgs = true;

                CMDMode = false;
                OpenPCKRun(path, encKey);
            }
        }

        static void InfoPCKCommand(string[] args, bool list_files)
        {
            runWithArgs = true;

            string filePath = "";
            string encKey = null;

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
                    Log("Path to file not specified! Or incorrect number of arguments specified!");
                    LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            PCKActions.InfoPCKRun(filePath, list_files, encKey);
        }

        static void ExtractPCKCommand(string[] args, bool overwriteExisting = true)
        {
            runWithArgs = true;

            string filePath = "";
            string dirPath = "";
            string encKey = null;

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
                    Log($"Invalid number of arguments! Expected 3 or 4, but got {args.Length}");
                    LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            PCKActions.ExtractPCKRun(filePath, dirPath, overwriteExisting, encKey: encKey);
        }

        static void ExtractSkipExistingPCKCommand(string[] args)
        {
            ExtractPCKCommand(args, false);
        }

        static void PackPCKCommand(string[] args, bool embed)
        {
            runWithArgs = true;

            string dirPath = "";
            string filePath = "";
            string strVer = "";
            uint alignment = 16;
            string encKey = null;
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
                                    Log($"Invalid encryption type: {encType}");
                                    LogHelp();
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log($"Invalid number of arguments! Expected from 4 to 6, but got {args.Length}");
                    LogHelp();
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

            PCKActions.PackPCKRun(dirPath, filePath, strVer, alignment, embed, encKey, encIndex, encFiles);
        }

        static void RipPCKCommand(string[] args)
        {
            runWithArgs = true;

            string exeFile = "";
            string outFile = null;
            try
            {
                if (args.Length >= 2)
                {
                    exeFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    if (args.Length == 3)
                        outFile = Path.GetFullPath(args[2].Replace("\"", ""));

                    if (args.Length > 4)
                    {
                        Log($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        LogHelp();
                        return;
                    }
                }
                else
                {
                    Log($"Path to file or directory not specified!");
                    LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            PCKActions.RipPCKRun(exeFile, outFile);
        }

        static void MergePCKCommand(string[] args)
        {
            runWithArgs = true;

            string pckFile = "";
            string exeFile = "";
            try
            {
                if (args.Length == 3)
                {
                    pckFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    exeFile = Path.GetFullPath(args[2].Replace("\"", ""));
                }
                else
                {
                    Log($"Invalid number of arguments! Expected 3, but got {args.Length}");
                    LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            PCKActions.MergePCKRun(pckFile, exeFile);
        }

        static void SplitPCKCommand(string[] args)
        {
            runWithArgs = true;

            string exeFile = "";
            string pairName = null;
            try
            {
                if (args.Length >= 2)
                {
                    exeFile = Path.GetFullPath(args[1].Replace("\"", ""));

                    if (args.Length == 3)
                        pairName = Path.GetFullPath(args[2].Replace("\"", ""));

                    if (args.Length > 3)
                    {
                        Log($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}");
                        LogHelp();
                        return;
                    }
                }
                else
                {
                    Log($"Path to file not specified!");
                    LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            PCKActions.SplitPCKRun(exeFile, pairName);
        }

        static void ChangeVersionPCKCommand(string[] args)
        {
            runWithArgs = true;

            string pckFile = "";
            string strVer = "";

            try
            {
                if (args.Length >= 3 && args.Length <= 4)
                {
                    pckFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    strVer = args[2];
                }
                else
                {
                    Log($"Invalid number of arguments! Expected 3, but got {args.Length}");
                    LogHelp();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                return;
            }

            PCKActions.ChangePCKVersion(pckFile, strVer);
        }
    }
}