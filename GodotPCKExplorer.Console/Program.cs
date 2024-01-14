using GodotPCKExplorer.Shared;

namespace GodotPCKExplorer.Cmd
{
    internal class Program
    {
        internal static int? ExitCode;
        static readonly Logger logger;

        static int Main(string[] args)
        {
            ConsoleCommands.RunCommandInternal(args);
            Cleanup();

            if (ExitCode.HasValue)
            {
                return ExitCode.Value;
            }
            return 0;
        }

        static Program()
        {
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
            ExitCode = 2;
            logger.Write(ex);
        }

        public static void LogHelp()
        {
            Log("\n" + GlobalConstants.HelpText);
        }
    }
}
