using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GodotPCKExplorer.UI
{
    internal static class Program
    {
        public static readonly string AppName = "GodotPCKExplorer";
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

        static bool CMDMode = true;
        static Form1 mainForm = null;

        static Logger logger;

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

        public static void OpenMainForm(string path, string encKey = null)
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
        }

        public static void CloseMainForm()
        {
            mainForm?.CloseFile();
        }
    }
}