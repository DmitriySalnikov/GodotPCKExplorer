using System.Diagnostics;

namespace Tests
{
    internal class RunGodotWithOutput : IDisposable
    {
        readonly Process process;
        readonly Timer timer;
        readonly string fileName;
        readonly string pwd;
        readonly string uid;
        readonly string uid_file;

        public RunGodotWithOutput(string name, string godot_args, string user_args, int closeDelay)
        {
            byte[] bytes = new byte[16];
            Random.Shared.NextBytes(bytes);
            fileName = name;
            uid = Convert.ToHexString(bytes).Replace("-", "");
            pwd = Path.GetDirectoryName(name) ?? "";
            uid_file = Path.Combine(pwd, uid);
            var args = $"{godot_args} -- --test_result_file {uid} {user_args}";

            TUtils.FixPermissions(name);

            process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = name,
                    Arguments = args,
                    WorkingDirectory = pwd,
                    UseShellExecute = true,
                    CreateNoWindow = true
                }
            };

            Console.WriteLine($"Running {name} with [{args}]");
            process.Start();

            timer = new Timer((s) =>
            {
                if (!process.HasExited)
                    Kill();
            }, null, closeDelay, -1);
        }

        public RunGodotWithOutput(string name, string godot_args = "", string user_args = "") : this(name, godot_args, user_args, 60 * 1000) { }

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

        public bool IsSuccess()
        {
            if (!process.HasExited)
                process.WaitForExit();

            if (File.Exists(uid_file))
            {
                try
                {
                    if (File.ReadAllText(uid_file) == "SUCCESS")
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.GetType().Name}");
                    Console.WriteLine(ex.Message);
                }
            }
            return false;
        }

        public void Dispose()
        {
            timer.Dispose();
            if (!process.HasExited)
                Kill();
            process.Dispose();

            if (File.Exists(uid_file))
            {
                try
                {
                    File.Delete(uid_file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.GetType().Name}");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
