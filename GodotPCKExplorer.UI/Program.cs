using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GodotPCKExplorer.UI
{
    internal static class Program
    {
        public static readonly string AppName = "GodotPCKExplorer";
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

        static bool CMDMode = true;
        static MainForm? mainForm = null;
        static BackgroundProgress? progressBar = null;

        static readonly Logger logger;
        static bool IsConsoleVisible = false;

        static int prev_progress_percent = 0;
        static DateTime prev_progress_time = DateTime.UtcNow;

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

            Cleanup();
            // TODO return exitCode on errors
            //Environment.ExitCode = 0;
            return;
        }

        static Program()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);

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

            CMDMode = true;
            Log("");
            ShowConsole();
        }

        public static void Cleanup()
        {
            logger?.Dispose();

            mainForm?.Dispose();
            mainForm = null;
        }

        #region Logs
        public static void LogProgress(string operation, string txt)
        {
            logger.Write($"[Progress] {operation}: {txt}");
        }

        public static void LogProgress(string operation, int number, string? customPrefix = null)
        {
            if (((DateTime.UtcNow - prev_progress_time).TotalSeconds > 1) || (prev_progress_percent != number && Math.Abs(number - prev_progress_percent) >= 5))
            {
                if (customPrefix != null)
                    LogProgress(operation, $"{number}");
                else
                    LogProgress(operation, $"{Math.Max(Math.Min(number, 100), 0)}%");

                prev_progress_percent = number;
                prev_progress_time = DateTime.UtcNow;
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

            if (!IsConsoleVisible)
                Log("The console is displayed. The following logs will be duplicated into it!");
            logger.DuplicateToConsole = true;
            IsConsoleVisible = true;
        }

        public static void HideConsole()
        {
            if (!Utils.IsRunningOnMono())
                ShowWindow(GetConsoleWindow(), SW_HIDE);

            if (IsConsoleVisible)
                Log("The console is hidden. The following logs will only be written to a file!");
            logger.DuplicateToConsole = false;
            IsConsoleVisible = false;
        }

        public static void DoTaskWithProgressBar(Action<CancellationToken> work, Form? parentForm = null, [CallerFilePath] string _file = "", [CallerMemberName] string _func = "", [CallerLineNumber] int _line = 0)
        {
            void action(CancellationToken ct)
            {
                Thread.CurrentThread.Name = $"{Path.GetFileName(_file)}::{_func}::{_line}";

                // Do work
                try
                {
                    work(ct);
                }
                catch (Exception ex)
                {
                    ShowMessage(ex, MessageType.Error);
                }
            }

            progressBar = new BackgroundProgress(action);
            if (parentForm != null)
            {
                progressBar.ShowDialog(parentForm);
            }
            else
            {
                progressBar.ShowDialog();
            }

            progressBar?.Dispose();
            progressBar = null;
        }

        public static void OpenMainForm(string path, string? encKey = null)
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