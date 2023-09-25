using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace GodotPCKExplorer.UI
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

        public static long AlignAddress(long p_n, uint p_alignment)
        {
            if (p_alignment == 0)
                return p_n;

            long rest = p_n % p_alignment;
            if (rest == 0)
                return p_n;
            else
                return p_n + (p_alignment - rest);
        }

        public static void AddPadding(BinaryWriter p_file, long p_bytes)
        {
            if (p_bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(p_bytes));

            if (p_bytes != 0)
                p_file.Write(new byte[p_bytes]);
        }
    }
}
