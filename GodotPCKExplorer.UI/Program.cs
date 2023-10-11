using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodotPCKExplorer.UI
{
    internal static class Program
    {
        public static readonly string AppName = "GodotPCKExplorer";
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

        static bool CMDMode = true;
        static MainForm mainForm = null;
        static BackgroundProgress progressBar = null;

        static Logger logger;

        static int prev_progress_percent = 0;
        static DateTime prev_progress_time = DateTime.Now;

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

            if (!ConsoleCommands.RunCommandInternal(args))
            {
                // run..
                CMDMode = false;

                mainForm = new MainForm();
                Application.Run(mainForm);
            }

            //Environment.ExitCode = 0;
            return;
        }

        static Program()
        {
            logger = new Logger("log.txt");
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
        public static void LogProgress(string operation, string txt)
        {
            logger.Write($"[Progress] {operation}: {txt}");
        }

        public static void LogProgress(string operation, int number, string customPrefix = null)
        {
            if (((DateTime.Now - prev_progress_time).TotalSeconds > 1) || (prev_progress_percent != number && Math.Abs(number - prev_progress_percent) >= 5))
            {
                if (customPrefix != null)
                    LogProgress(operation, $"{number}");
                else
                    LogProgress(operation, $"{Math.Max(Math.Min(number, 100), 0)}%");

                prev_progress_percent = number;
                prev_progress_time = DateTime.Now;
            }

            // Always update ProgressBar
            if (progressBar != null && progressBar.Created)
            {
                if (progressBar.InvokeRequired)
                {
                    progressBar.BeginInvoke(new Action(() =>
                    {
                        progressBar.ReportProgress(operation, number, customPrefix);
                    }));
                }
                else
                {
                    progressBar.ReportProgress(operation, number, customPrefix);
                }
            }
        }

        public static void Log(string txt)
        {
            logger.Write(txt);
        }

        public static void Log(Exception ex)
        {
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

        public static void EnableMessageBoxes()
        {
            CMDMode = false;
        }

        public static void DisableMessageBoxes()
        {
            CMDMode = true;
        }

        public static bool IsMessageBoxesEnabled()
        {
            return !CMDMode;
        }

        public static void ShowConsole()
        {
            if (!Utils.IsRunningOnMono())
                ShowWindow(GetConsoleWindow(), SW_SHOW);
        }

        public static void HideConsole()
        {
            if (!Utils.IsRunningOnMono())
                ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        public static void DoTaskWithProgressBar(Action<CancellationToken> work, [CallerFilePath] string _file = "", [CallerMemberName] string _func = "", [CallerLineNumber] int _line = 0)
        {
            var token = new CancellationTokenSource();
            progressBar = new BackgroundProgress(token);

            var task = Task.Run(() =>
            {
                Thread.CurrentThread.Name = $"{Path.GetFileName(_file)}::{_func}::{_line}";

                // Do work
                try
                {
                    work(token.Token);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex, MessageType.Error);
                }

                // Force close window
                token.Cancel();

                while (progressBar == null || !progressBar.Visible)
                {
                    Thread.Sleep(1);
                }

                progressBar.Invoke(new Action(() =>
                {
                    progressBar.Close();
                    progressBar?.Dispose();
                    progressBar = null;
                }
                ));
            });

            // Wait until the task is completed
            // or until the window closes
            progressBar?.ShowDialog();
            progressBar?.Dispose();
            progressBar = null;
            task.Wait();
        }

        public static void OpenMainForm(string path, string encKey = null)
        {
            if (mainForm == null)
            {
                mainForm = new MainForm();
                mainForm.OpenFile(path, encKey);

                Application.Run(mainForm);
            }
            else
            {
                mainForm.OpenFile(path, encKey);
            }
        }

        public static void CloseMainForm()
        {
            mainForm?.CloseFile();
        }
    }
}