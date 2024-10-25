using GodotPCKExplorer.GlobalShared;
using System.Globalization;

namespace GodotPCKExplorer.Cmd
{
    internal class Program
    {
        static readonly Logger logger;

        static int Main(string[] args)
        {
            PCKActions.Init((s) => Log(s));
            ConsoleCommands.InitLogs(Log, Log);

            ConsoleCommands.RunCommand(args);
            Cleanup();

            if (ConsoleCommands.ExitCode.HasValue)
            {
                return ConsoleCommands.ExitCode.Value;
            }
            return 0;
        }

        static Program()
        {
            // InvariantCulture for console and UI
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (!Directory.Exists(GlobalConstants.AppDataPath))
                Directory.CreateDirectory(GlobalConstants.AppDataPath);

            logger = new Logger("log_c.txt");
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
            logger.Write(ex);
        }
    }
}
