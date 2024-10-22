using System.Diagnostics;

namespace Tests
{
    internal class TUtils
    {
        // Returns true if success and false otherwise
        // permissions can be an int or a string. For example it can also be +x, -x etc..
        // https://stackoverflow.com/a/64625094
        public static bool Chmod(string filePath, string permissions = "700", bool recursive = false)
        {
            string cmd;
            if (recursive)
                cmd = $"chmod -R {permissions} {filePath}";
            else
                cmd = $"chmod {permissions} {filePath}";

            try
            {
                using Process proc = Process.Start("/bin/bash", $"-c \"{cmd}\"");
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public static void WaitForFileUnlock(string file, double waitForSec)
        {
            while (waitForSec > 0)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    using var w = fileInfo.OpenWrite();
                    break;
                }
                catch { }
                Thread.Sleep(100);
                waitForSec -= 0.1;
            }
        }

        public static void FixPermissions(string path)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    Console.WriteLine($"Fixing permissions for {path}");
                    Chmod(path, "777", false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}");
                Console.WriteLine(ex.Message);
            }
        }

        public static void CopyFile(string from, string to)
        {
            Console.WriteLine($"Copying {from} to {to}");
            File.Copy(from, to, true);
        }

        public static string RemoveTimestampFromLogs(string logs)
        {
            logs = logs.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
            var lines = logs.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                var split = lines[i].Split(['\t'], 2);
                if (split.Length == 2)
                    lines[i] = split[1];
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
