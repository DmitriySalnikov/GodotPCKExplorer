using System.Diagnostics;

namespace Tests
{
    internal class RunProcessWithOutput : IDisposable
    {
        readonly Process process;
        readonly Timer timer;
        readonly string fileName;
        readonly string pwd;

        public RunProcessWithOutput(string name, string proc_args, int closeDelay)
        {
            fileName = name;
            pwd = Path.GetDirectoryName(name) ?? "";

            TUtils.FixPermissions(name);

            process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = name,
                    Arguments = proc_args,
                    WorkingDirectory = pwd,
                    UseShellExecute = true,
                    CreateNoWindow = true
                }
            };

            Console.WriteLine($"Running {name} with [{proc_args}]");
            process.Start();

            timer = new Timer((s) =>
            {
                if (!process.HasExited)
                    Kill();
            }, null, closeDelay, -1);
        }

        public RunProcessWithOutput(string name, string proc_args = "") : this(name, proc_args, 60 * 1000) { }

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
                // Sometimes the process does not stop instantly
                // Kill it manually
                var p = Process.Start("taskkill", $"/F /PID {process.Id}");
                p.WaitForExit();
                process.Kill();

                TUtils.WaitForFileUnlock(fileName, 4);
            }
        }

        public int GetExitCode()
        {
            if (!process.HasExited)
                process.WaitForExit();

            return process.ExitCode;
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
