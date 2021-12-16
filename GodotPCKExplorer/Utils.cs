using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public class Utils
    {
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

        public static void ShowMessage(string text, string title)
        {
            if (Program.CMDMode)
                Console.WriteLine($"{title}: {text}");
            else
                MessageBox.Show(text, title);
        }

        public static void CommandLog(string text, string title, bool showHelp)
        {
            if (showHelp)
                ShowMessage(text + "\n\n" + Properties.Resources.HelpText, title);
            else
                ShowMessage(text, title);
        }

        static public List<PCKPacker.FileToPack> ScanFoldersForFiles(string folder)
        {
            var files = new List<PCKPacker.FileToPack>();
            ScanFoldersForFilesInternal(folder, files, ref folder);
            return files;
        }

        static void ScanFoldersForFilesInternal(string folder, List<PCKPacker.FileToPack> files, ref string basePath)
        {
            foreach (string d in Directory.EnumerateDirectories(folder))
            {
                ScanFoldersForFilesInternal(d, files, ref basePath);
            }

            foreach (string f in Directory.EnumerateFiles(folder))
            {
                var inf = new FileInfo(f);
                files.Add(new PCKPacker.FileToPack(f, f.Replace(basePath + "\\", "res://").Replace("\\", "/"), inf.Length));
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
            return short_name;
        }
    }
}
