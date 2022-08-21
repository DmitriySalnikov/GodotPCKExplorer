using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public static class Program
    {
        public const int PCK_MAGIC = 0x43504447;

        public static bool CMDMode = false;
        public static bool ForceConsoleMode = false;
        static bool runWithArgs = false;
        public static Form1 mainForm = null;

        // https://stackoverflow.com/a/3571628/8980874
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ShowConsole();
            CMDMode = true;
            Console.WriteLine("");

            RunCommandInternal(args, false);

            if (!runWithArgs)
            {
                // run..
                CMDMode = false;

                HideConsole();

                mainForm = new Form1();
                Application.Run(mainForm);
            }

            //Environment.ExitCode = 0;
            return;
        }

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
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
            }

            if (args.Length > 0)
                IterateCommands(
                 () => { if (args[0] == "-h" || args[0] == "/?" || args[0] == "--help") HelpCommand(); },
                 () => { if (args[0] == "-i") InfoPCKCommand(args); },
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
            PCKActions.HelpRun();
            return;

        }

        static void OpenPCKCommand(string[] args)
        {
            string path = null;

            try
            {
                if (args.Length == 2)
                {
                    path = Path.GetFullPath(args[1]);
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
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
                catch (Exception e)
                {
                    Utils.CommandLog(e.Message, "Error", false);
                    return;
                }
            }

            if (path != null)
            {
                runWithArgs = true;

                CMDMode = false;
                PCKActions.OpenPCKRun(path);
            }
        }

        static void InfoPCKCommand(string[] args)
        {
            runWithArgs = true;

            string filePath = "";
            try
            {
                if (args.Length == 2)
                {
                    filePath = Path.GetFullPath(args[1].Replace("\"", ""));
                }
                else
                {
                    Utils.CommandLog("Path to file not specified! Or incorrect number of arguments specified!", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
                return;
            }

            PCKActions.InfoPCKRun(filePath);
        }

        static void ExtractPCKCommand(string[] args, bool overwriteExisting = true)
        {
            runWithArgs = true;

            string filePath = "";
            string dirPath = "";
            try
            {
                if (args.Length == 3)
                {
                    filePath = Path.GetFullPath(args[1].Replace("\"", ""));
                    dirPath = Path.GetFullPath(args[2].Replace("\"", ""));
                }
                else
                {
                    Utils.CommandLog($"Invalid number of arguments! Expected 3, but got {args.Length}", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
                return;
            }

            PCKActions.ExtractPCKRun(filePath, dirPath, overwriteExisting);
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

            try
            {
                if (args.Length == 4)
                {
                    dirPath = Path.GetFullPath(args[1].Replace("\"", ""));
                    filePath = Path.GetFullPath(args[2].Replace("\"", ""));
                    strVer = args[3];
                }
                else
                {
                    Utils.CommandLog($"Invalid number of arguments! Expected 4, but got {args.Length}", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
                return;
            }

            PCKActions.PackPCKRun(dirPath, filePath, strVer, embed);
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
                        Utils.CommandLog($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}", "Error", true);
                        return;
                    }
                }
                else
                {
                    Utils.CommandLog($"Path to file or directory not specified!", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
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
                    Utils.CommandLog($"Invalid number of arguments! Expected 3, but got {args.Length}", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
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
                        Utils.CommandLog($"Invalid number of arguments! Expected 2 or 3, but got {args.Length}", "Error", true);
                        return;
                    }
                }
                else
                {
                    Utils.CommandLog($"Path to file not specified!", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
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
                if (args.Length == 3)
                {
                    pckFile = Path.GetFullPath(args[1].Replace("\"", ""));
                    strVer = args[2];
                }
                else
                {
                    Utils.CommandLog($"Invalid number of arguments! Expected 3, but got {args.Length}", "Error", true);
                    return;
                }
            }
            catch (Exception e)
            {
                Utils.CommandLog(e.Message, "Error", false);
                return;
            }

            PCKActions.ChangePCKVersion(pckFile, strVer);
        }
    }
}