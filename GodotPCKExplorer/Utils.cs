using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public enum MessageType
    {
        None,
        Info,
        Error,
        Warning
    }

    public class Utils
    {
        public const int PCK_MAGIC = 0x43504447;
        public const int PCK_DIR_ENCRYPTED = 1 << 0;
        public const int PCK_FILE_ENCRYPTED = 1 << 0;

        // Source: https://stackoverflow.com/a/14488941
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        // https://stackoverflow.com/a/321404/8980874
        public static byte[] StringToByteArray(string hex)
        {
            hex.Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // https://stackoverflow.com/a/10520086/8980874
        public static byte[] GetFileMD5(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var bytes = md5.ComputeHash(stream);
                    if (bytes.Length > 16)
                        throw new FormatException("Wrong size of MD5 hash");
                    return bytes;
                }
            }
        }

        // https://stackoverflow.com/a/30300521/8980874
        public static string WildCardToRegular(string value)
        {
            if (!(value.Contains("*") || value.Contains("?")))
                value = $"*{value}*";

            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        public static bool IsMatchWildCard(string input, string wildcard, bool matchCase)
        {
            if (matchCase)
                return Regex.IsMatch(input, WildCardToRegular(wildcard));
            else
                return Regex.IsMatch(input.ToLower(), WildCardToRegular(wildcard.ToLower()));
        }

        public static DialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, MessageBoxButtons boxButtons = MessageBoxButtons.OK)
        {
            Program.Log($"[{messageType}] \"{title}\": {text}");

            if (!Program.CMDMode)
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
                return MessageBox.Show(text, title, boxButtons, icon);
            }

#if DEV_ENABLED
            System.Diagnostics.Debugger.Break();
#endif
            return DialogResult.OK;
        }

        public static DialogResult ShowMessage(Exception ex, string title, MessageType messageType = MessageType.None, MessageBoxButtons boxButtons = MessageBoxButtons.OK)
        {
            Program.Log(ex);
            return ShowMessage(ex.Message, title, messageType, boxButtons);
        }

        public static void CommandLog(string text, string title, bool showHelp, MessageType messageType = MessageType.None)
        {
            if (showHelp)
                ShowMessage(text + "\n\n" + Properties.Resources.HelpText, title, messageType);
            else
                ShowMessage(text, title, messageType);
        }

        public static void CommandLog(Exception ex, string title, bool showHelp, MessageType messageType = MessageType.None)
        {
            Program.Log(ex);
            CommandLog(ex.Message, title, showHelp, messageType);
        }

        static public List<PCKPacker.FileToPack> ScanFoldersForFiles(string folder)
        {
            if (!Directory.Exists(folder))
                return new List<PCKPacker.FileToPack>();

            folder = Path.GetFullPath(folder);
            var files = new List<PCKPacker.FileToPack>();
            var cancel = false;

            ScanFoldersForFilesAdvanced(folder, files, ref folder, ref cancel);
            if (cancel)
                files.Clear();
            GC.Collect();

            return files;
        }

        static public void ScanFoldersForFilesAdvanced(string folder, List<PCKPacker.FileToPack> files, ref string basePath, ref bool cancel, BackgroundWorker backgroundWorker = null)
        {
            IEnumerable<string> dirEnums;
            try
            {
                dirEnums = Directory.EnumerateDirectories(folder);
            }
            catch (Exception ex)
            {
                cancel = ShowMessage($"{ex.Message}\nThe directory will be skipped!", "Warning", MessageType.Warning, MessageBoxButtons.OKCancel) == DialogResult.Cancel;
                return;
            }

            // Scan folders
            foreach (string d in dirEnums)
            {
                if (cancel || (backgroundWorker != null && backgroundWorker.CancellationPending))
                    return;

                backgroundWorker?.ReportProgress(0, $"Found files: {files.Count}");
                ScanFoldersForFilesAdvanced(d, files, ref basePath, ref cancel, backgroundWorker);
            }
            dirEnums = null;

            IEnumerable<string> filesEnum;
            try
            {
                filesEnum = Directory.EnumerateFiles(folder);
            }
            catch (Exception ex)
            {
                cancel = ShowMessage($"{ex.Message}\nThe directory will be skipped!", "Warning", MessageType.Warning, MessageBoxButtons.OKCancel) == DialogResult.Cancel;
                return;
            }

            // Scan files
            foreach (string f in filesEnum)
            {
                if (cancel || (backgroundWorker != null && backgroundWorker.CancellationPending))
                    return;

                backgroundWorker?.ReportProgress(0, $"Found files: {files.Count}");
                try
                {
                    var inf = new FileInfo(f);
                    files.Add(new PCKPacker.FileToPack(f, f.Replace(basePath + Path.DirectorySeparatorChar, "res://").Replace("\\", "/"), inf.Length));
                }
                catch (Exception ex)
                {
                    cancel = ShowMessage($"{ex.Message}\nThe file will be skipped!", "Warning", MessageType.Warning, MessageBoxButtons.OKCancel) == DialogResult.Cancel;
                }
            }
        }

        public static string GetShortPath(string name, uint chars)
        {
            var short_name = "";
            if (name.Length > chars)
            {
                short_name = name.Substring(name.Length - (int)chars);
                var slash_pos = short_name.IndexOf(Path.DirectorySeparatorChar);
                if (slash_pos == -1)
                    short_name = "..." + short_name;
                else
                {
                    short_name = "..." + short_name.Substring(slash_pos);
                }
            }
            else
            {
                short_name = name;
            }
            return short_name;
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static long AlignAddress(long p_n, int p_alignment)
        {
            if (p_alignment == 0)
                return p_n;

            long rest = p_n % p_alignment;
            if (rest == 0)
                return p_n;
            else
                return p_n + (p_alignment - rest);
        }

        public static void AddPadding(BinaryWriter p_file, int p_bytes)
        {
            p_file.Write(new byte[p_bytes]);
        }

    }
}
