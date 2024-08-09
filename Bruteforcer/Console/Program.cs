using GodotPCKExplorer;
using GodotPCKExplorer.GlobalShared;
using System.Globalization;

namespace PCKBruteforcer.Cmd
{
    internal class Program
    {
        internal static int? ExitCode;
        static readonly Logger logger;
        internal static object lock_obj = new();

        [STAThread]
        static int Main(string[] args)
        {
            PCKActions.Init(new ProgressReporterBrute());

            ConsoleCommands.RunCommand(args);
            logger.Dispose();

            if (ExitCode.HasValue)
            {
                return ExitCode.Value;
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

            logger = new Logger("log_bfc.txt");
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

        public static void LogHelp()
        {
            Log("\n" + HelpText);
        }

        static readonly string HelpText = @"PCK Bruteforcer allows you to find the encryption key hidden in Godot executables.
This program does not guarantee that the key will be found,
as the developer may have used a custom build of Godot and hidden the key differently.
Encryption is only verified with PCK for Godot 4.

Paths and other arguments must be without spaces or inside quotes: ""some path""

{} - Optional arguments

Examples of valid commands:
-b	Start bruteforce
	-b [path to game executable] [path to pack] {start byte, -1 is auto} {end byte, -1} {threads, -1} {load encrypted in memory}
	-b C:/Game.exe C:/Game.pck
	-b C:/Game.exe C:/Game.pck 50535488 57535488 4 false
";

        public static bool StartBruteforce(string exe, string pck, long startAdr = -1, long endAdr = -1, int threadsCount = -1, bool inMemory = true)
        {
            FileInfo exeInfo = new FileInfo(exe);
            if (startAdr == -1)
            {
                startAdr = 0;
            }

            if (endAdr == -1)
            {
                endAdr = exeInfo.Length;
            }

            if (threadsCount == -1)
            {
                threadsCount = Environment.ProcessorCount - 1;
            }

            var cancellationToken = new CancellationTokenSource();
            var resultData = new Bruteforcer.ResultData();

            var bruteforcer = new Bruteforcer(
                    disablePCKLogs_cb: () => ProgressReporterBrute.DisableLogs = true,
                    enablePCKLogs_cb: () => ProgressReporterBrute.DisableLogs = false,
                    setOutputText_cb: (t) => { if (!ProgressReporterBrute.DisableLogs) Log(t); },
                    log_cb: (t) => Log(t),
                    logException_cb: (ex) => Log(ex),
                    reportProgress_cb: (d) => Log($"{d.ProgressPercent:F2}%, Estimated: {(d.RemainingTime + d.ElapsedTime):hh\\:mm\\:ss}, Remaining: {d.RemainingTime:hh\\:mm\\:ss}, Elapsed: {d.ElapsedTime:hh\\:mm\\:ss}"),
                    foundAlert_cb: (res) => { ProgressReporterBrute.DisableLogs = true; ExitCode = 0; resultData = res; },
                    finished_cb: null
                    )
            {
                ReportUpdateInterval = 1,
            };

            bruteforcer.Start(exe, pck, startAdr, endAdr, threadsCount, inMemory, cancellationToken);

            ProgressReporterBrute.DisableLogs = false;
            Log("Finished.");

            if (resultData.found)
            {
                Log($"Encryption key found: {resultData.key}");
            }
            else
            {
                Log("Encryption key not found.");
            }

            return true;
        }
    }
}
