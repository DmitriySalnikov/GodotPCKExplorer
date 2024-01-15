using GodotPCKExplorer.GlobalShared;

namespace PCKBruteforcer.UI
{
    internal static class Program
    {
        internal static int? ExitCode;
        static readonly Logger logger;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BruteforcerMainForm());
        }

        static Program()
        {
            if (!Directory.Exists(GlobalConstants.AppDataPath))
                Directory.CreateDirectory(GlobalConstants.AppDataPath);

            logger = new Logger("log_bf.txt");
        }

        public static void Cleanup()
        {
            logger.Dispose();
        }

        public static void Log(string txt)
        {
            logger.Write(txt);
        }

        public static void Log(Exception ex)
        {
            ExitCode = 2;
            logger.Write(ex);
        }

        public static void LogHelp()
        {
            Log("\nThere is no HelpText");
        }
    }
}
