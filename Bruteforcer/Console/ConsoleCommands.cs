// ExitCode = 1: Exception
// ExitCode = 2: Error
// ExitCode = 3: Error Help
// ExitCode = 4: Error Failed Command

namespace PCKBruteforcer.Cmd
{
    public static class ConsoleCommands
    {
        static bool runWithArgs = false;
        private static readonly string[] yesVariants = ["y", "yes", "true"];

        public static bool RunCommand(string[] args)
        {
            runWithArgs = false;

            // Skip exe path
            try
            {
                if (args.Length > 0)
                    if (Path.GetFullPath(args[0]) == Path.GetFullPath(AppContext.BaseDirectory))
                        args = args.Skip(1).ToArray();
            }
            catch (Exception ex)
            {
                LogEx(ex);
            }

            var help = () => { Program.Log("Please specify which action you want to run."); HelpCommand(); };

            if (args.Length > 0)
                IterateCommands(
                 () => { if (args[0] == "-h" || args[0] == "/?" || args[0] == "--help") HelpCommand(); },
                 () => { if (args[0] == "-b") BruteforceCommand(args); },
                 help
                 );
            else
            {
                help();
            }

            return runWithArgs;
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

        static void HelpCommand()
        {
            runWithArgs = true;

            Program.Log("Help:");
            Program.LogHelp();
            return;

        }

        static void LogErr(string err)
        {
            Program.ExitCode = 2;
            Program.Log(err);
        }

        static void LogErrHelp(string err)
        {
            Program.ExitCode = 3;
            Program.Log(err);
            Program.LogHelp();
        }

        static void LogEx(Exception ex)
        {
            Program.ExitCode = 1;
            Program.Log(ex);
        }

        static void SetResult(bool res)
        {
            Program.ExitCode = res ? 0 : 4;
        }

        static void BruteforceCommand(string[] args)
        {
            runWithArgs = true;
            string exePath;
            string pckPath;
            long startAdr = -1;
            long endAdr = -1;
            int threads = -1;
            bool inMem = true;

            try
            {
                if (args.Length > 2)
                {
                    exePath = Path.GetFullPath(args[1]);
                    pckPath = Path.GetFullPath(args[2]);

                    if (args.Length > 3)
                    {
                        startAdr = long.Parse(args[3]);
                        if (args.Length > 4)
                        {
                            endAdr = long.Parse(args[4]);
                            if (args.Length > 5)
                            {
                                threads = int.Parse(args[5]);
                                if (args.Length > 6)
                                {
                                    inMem = yesVariants.Contains(args[6].ToLower());
                                }
                            }
                        }
                    }
                }
                else
                {
                    LogErrHelp($"Invalid number of arguments! Expected from 2 to 6, but got {args.Length}");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                return;
            }

            SetResult(Program.StartBruteforce(exePath, pckPath, startAdr, endAdr, threads, inMem));
        }
    }
}
