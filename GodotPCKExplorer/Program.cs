using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace GodotPCKExplorer
{
    public static class Program
    {
        public const int PCK_MAGIC = 0x43504447;
        const string quote_string_pattern = @"(("".*"")|([^""\s]+))";
        static Regex QuoteStringRegEx = new Regex(quote_string_pattern);

        public static bool CMDMode = false;
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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ShowConsole();
            CMDMode = true;
            Console.WriteLine("");

            RunCommandInternal(Environment.CommandLine, false);

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

        static public void RunCommand(string args)
        {
            RunCommandInternal(args, true);
        }

        static void RunCommandInternal(string args, bool restore_params)
        {
            var old_run_with_args = runWithArgs;
            var old_cmd_mode = CMDMode;

            IterateCommands(
             () => HelpCommand(args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)),
             () => OpenPCKCommand(SplitArgs(" -o ")),
             () => InfoPCKCommand(SplitArgs(" -i ")),
             () => ExtractPCKCommand(SplitArgs(" -e ")),
             () => ExtractSkipExistingPCKCommand(SplitArgs(" -es ")),
             () => PackPCKCommand(SplitArgs(" -p "), false),
             () => PackPCKCommand(SplitArgs(" -pe "), true),
             () => MergePCKCommand(SplitArgs(" -m ")),
             () => RipPCKCommand(SplitArgs(" -r ")),
             () => SplitPCKCommand(SplitArgs(" -s "))
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

        static string SplitArgs(string separator)
        {
            var args = Environment.CommandLine.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length > 1)
            {
                return args[1];
            }
            return "";
        }

        static public void ShowConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_SHOW);
        }

        static public void HideConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        static void HelpCommand(string[] args)
        {
            if (args.Length > 1)
            {
                foreach (var a in args)
                {
                    if (a.Contains("-h") || a.Contains("/?") || a.Contains("--help"))
                    {
                        runWithArgs = true;
                        Utils.HelpRun();
                        return;
                    }
                }
            }
            return;
        }

        static void OpenPCKCommand(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                string path = null;
                try
                {
                    var match = QuoteStringRegEx.Match(args);
                    if (match.Success)
                    {
                        path = Path.GetFullPath(match.Value.Replace("\"", ""));
                    }
                    else
                    {
                        Utils.CommandLog(args[1].ToString(), "Not valid file path", true);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Utils.CommandLog($"{args[1]}\n{e.Message}", "Error in file path", false);
                    return;
                }

                if (path == null)
                {
                    var s = Environment.CommandLine.Split(new string[] { "\" \"" }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        var strings = QuoteStringRegEx.Matches(Environment.CommandLine);
                        if (strings.Count == 2)
                        {
                            path = Path.GetFullPath(strings[1].Value.Replace("\"", ""));
                        }
                    }
                }

                if (path == null)
                {
                    try
                    {
                        path = Path.GetFullPath(Environment.CommandLine.Replace(Application.ExecutablePath, "").Replace("\"", ""));
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
                    Utils.OpenPCKRun(path);
                }
            }

            return;
        }

        static void InfoPCKCommand(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                runWithArgs = true;

                string filePath = "";
                try
                {
                    var matches = QuoteStringRegEx.Matches(args);
                    if (matches.Count == 1)
                    {
                        filePath = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
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

                Utils.InfoPCKRun(filePath);
            }

            return;
        }

        static void ExtractPCKCommand(string args, bool overwriteExisting = true)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                runWithArgs = true;

                string filePath = "";
                string dirPath = "";
                try
                {
                    var matches = QuoteStringRegEx.Matches(args);
                    if (matches.Count == 2)
                    {
                        filePath = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
                        dirPath = Path.GetFullPath(matches[1].Value.Replace("\"", ""));
                    }
                    else
                    {
                        Utils.CommandLog($"Incorrect number of arguments specified!", "Error", true);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Utils.CommandLog(e.Message, "Error", false);
                    return;
                }

                Utils.ExtractPCKRun(filePath, dirPath, overwriteExisting);
            }

            return;
        }

        static void ExtractSkipExistingPCKCommand(string args)
        {
            ExtractPCKCommand(args, false);
        }

        static void PackPCKCommand(string args, bool embed)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                runWithArgs = true;

                string dirPath = "";
                string filePath = "";
                string strVer = "";

                try
                {
                    var matches = QuoteStringRegEx.Matches(args);
                    if (matches.Count == 3)
                    {
                        dirPath = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
                        filePath = Path.GetFullPath(matches[1].Value.Replace("\"", ""));
                        strVer = matches[2].Value;
                    }
                    else
                    {
                        Utils.CommandLog($"Incorrect number of arguments specified!", "Error", true);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Utils.CommandLog(e.Message, "Error", false);
                    return;
                }

                Utils.PackPCKRun(dirPath, filePath, strVer, embed);
            }

            return;
        }

        static void RipPCKCommand(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                runWithArgs = true;

                string exeFile = "";
                string outFile = null;
                try
                {
                    var matches = QuoteStringRegEx.Matches(args);
                    if (matches.Count >= 1)
                    {
                        exeFile = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
                        if (matches.Count == 2)
                            outFile = Path.GetFullPath(matches[1].Value.Replace("\"", ""));

                        if (matches.Count > 2)
                        {
                            Utils.CommandLog($"Invalid number of arguments!", "Error", true);
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

                Utils.RipPCKRun(exeFile, outFile);
            }

            return;
        }

        static void MergePCKCommand(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                runWithArgs = true;

                string pckFile = "";
                string exeFile = "";
                try
                {
                    var matches = QuoteStringRegEx.Matches(args);
                    if (matches.Count == 2)
                    {
                        pckFile = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
                        exeFile = Path.GetFullPath(matches[1].Value.Replace("\"", ""));
                    }
                    else
                    {
                        Utils.CommandLog($"Invalid number of arguments!", "Error", true);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Utils.CommandLog(e.Message, "Error", false);
                    return;
                }

                Utils.MergePCKRun(pckFile, exeFile);
            }

            return;
        }

        static void SplitPCKCommand(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                runWithArgs = true;

                string exeFile = "";
                string pairName = null;
                try
                {
                    var matches = QuoteStringRegEx.Matches(args);
                    if (matches.Count >= 1)
                    {
                        exeFile = Path.GetFullPath(matches[0].Value.Replace("\"", ""));

                        if (matches.Count == 2)
                            pairName = Path.GetFullPath(matches[1].Value.Replace("\"", ""));

                        if (matches.Count > 2)
                        {
                            Utils.CommandLog($"Invalid number of arguments!", "Error", true);
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

                Utils.SplitPCKRun(exeFile, pairName);
            }

            return;
        }
    }
}