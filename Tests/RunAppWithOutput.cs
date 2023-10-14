using System.Diagnostics;

namespace Tests
{
    internal class RunAppWithOutput : IDisposable
    {
        readonly Process process;
        readonly Timer timer;

        public RunAppWithOutput(string name, string args, int closeDelay)
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                Arguments = args,
                FileName = name,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            process.Start();

            timer = new Timer((s) =>
            {
                if (!process.HasExited)
                    Kill();
            }, null, closeDelay, -1);
        }

        public RunAppWithOutput(string name, string args) : this(name, args, UtilMethodsTests.ExecutableRunDelay) { }

        void Kill()
        {
            if (!OperatingSystem.IsWindows())
            {
                var p = Process.Start("pkill", $"-TERM -P {process.Id}");
                p.WaitForExit();
                process.Kill();
            }
            else
            {
                process.Kill();
            }
        }

        public string GetConsoleText()
        {
            if (!process.HasExited)
                process.WaitForExit();
            return process.StandardOutput.ReadToEnd();
        }

        public string GetConsoleError()
        {
            if (!process.HasExited)
                process.WaitForExit();
            return process.StandardError.ReadToEnd();
        }

        public void Dispose()
        {
            timer.Dispose();
            if (!process.HasExited)
                Kill();
            process.Dispose();
        }
    }
}
