using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace GodotPCKExplorer
{
    public enum MessageType
    {
        None,
        Info,
        Error,
        Warning
    }

    public static class PCKUtils
    {
        static readonly Random rng = new Random();
        public const int PCK_VERSION_GODOT_3 = 1;
        public const int PCK_VERSION_GODOT_4 = 2;
        public const int PCK_MAGIC = 0x43504447;
        public const int PCK_DIR_ENCRYPTED = 1 << 0;
        public const int PCK_FILE_ENCRYPTED = 1 << 0;
        public const int BUFFER_MAX_SIZE = 1024 * 1024;
        public const int UnknownProgressStatus = -1234;
        public static string ByteArrayToHexString(byte[]? data, string sepChar = "")
        {
            if (data != null)
                return BitConverter.ToString(data).Replace("-", sepChar);

            return "";
        }

        // https://stackoverflow.com/a/321404/8980874
        // does the same thing as here https://github.com/godotengine/godot/blob/cfab3d2f57976913a03a891b30eaa0a5da4ff64f/core/io/pck_packer.cpp#L61
        public static byte[]? HexStringToByteArray(string? hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return null;

            hex = hex.Replace("-", "").Replace(" ", "").Replace("\r", "").Replace("\n", "");

            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool HexStringValidate(string hex, uint expected_size_in_bytes = 0)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return false;

            hex = hex.Replace("-", "").Replace(" ", "").Replace("\r", "").Replace("\n", "");

            var matches = Regex.Matches(hex, "[0-9A-Fa-f]+");
            if (matches.Count != 1)
                return false;
            if (matches[0].Length == hex.Length)
            {
                if (expected_size_in_bytes != 0)
                    return expected_size_in_bytes == hex.Length / 2;
                else
                    return false;
            }

            return false;
        }

        // https://stackoverflow.com/a/10520086/8980874
        public static byte[] GetFileMD5(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var bytes = md5.ComputeHash(stream);
            if (bytes.Length > 16)
                throw new FormatException("Wrong size of MD5 hash");
            return bytes;
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

        public static void AddPadding(BinaryWriter p_file, long p_bytes, bool randomFill = false)
        {
            if (p_bytes < 0)
                throw new ArgumentOutOfRangeException(nameof(p_bytes));

            if (p_bytes != 0)
            {
                if (randomFill)
                {
                    var buf = new byte[p_bytes];
                    rng.NextBytes(buf);
                    p_file.Write(buf);
                }
                else
                {
                    p_file.Write(new byte[p_bytes]);
                }
            }
        }

        public static List<PCKPacker.FileToPack> ScanFoldersForFiles(string folder, CancellationToken? cancellationToken = null)
        {
            if (!Directory.Exists(folder))
                return new List<PCKPacker.FileToPack>();

            folder = Path.GetFullPath(folder);
            var files = new List<PCKPacker.FileToPack>();
            var cancel = false;

            const string op = "Scan folder";
            PCKActions.progress?.LogProgress(op, $"Started scanning files in '{folder}'");
            PCKActions.progress?.LogProgress(op, PCKUtils.UnknownProgressStatus);

            ScanFoldersForFilesAdvanced(folder, files, ref folder, ref cancel, cancellationToken);
            if (cancel)
                files.Clear();
            GC.Collect();

            PCKActions.progress?.LogProgress(op, "Scan completed!");

            return files;
        }

        public static void ScanFoldersForFilesAdvanced(string folder, List<PCKPacker.FileToPack> files, ref string basePath, ref bool cancel, CancellationToken? cancellationToken = null)
        {
            const string op = "Scan folder";
            IEnumerable<string> dirEnums;
            try
            {
                dirEnums = Directory.EnumerateDirectories(folder);
            }
            catch (Exception ex)
            {
                cancel = PCKActions.progress?.ShowMessage($"{ex.Message}\nThe directory will be skipped!", "Warning", MessageType.Warning, PCKMessageBoxButtons.OKCancel) == PCKDialogResult.Cancel;
                return;
            }

            // Scan folders
            foreach (string d in dirEnums)
            {
                if (cancel || (cancellationToken?.IsCancellationRequested ?? false))
                    return;

                ScanFoldersForFilesAdvanced(d, files, ref basePath, ref cancel, cancellationToken);
            }

            IEnumerable<string> filesEnum;
            try
            {
                filesEnum = Directory.EnumerateFiles(folder);
            }
            catch (Exception ex)
            {
                cancel = PCKActions.progress?.ShowMessage($"{ex.Message}\nThe directory will be skipped!", "Warning", MessageType.Warning, PCKMessageBoxButtons.OKCancel) == PCKDialogResult.Cancel;
                return;
            }

            // Scan files
            foreach (string f in filesEnum)
            {
                if (cancel || (cancellationToken?.IsCancellationRequested ?? false))
                    return;

                try
                {
                    var inf = new FileInfo(f);
                    files.Add(new PCKPacker.FileToPack(f, f.Replace(basePath + Path.DirectorySeparatorChar, "res://").Replace("\\", "/"), inf.Length));
                    PCKActions.progress?.LogProgress(op, f);
                    PCKActions.progress?.LogProgress(op, files.Count, "Found files: ");
                }
                catch (Exception ex)
                {
                    cancel = PCKActions.progress?.ShowMessage($"{ex.Message}\nThe file will be skipped!", "Warning", MessageType.Warning, PCKMessageBoxButtons.OKCancel) == PCKDialogResult.Cancel;
                }
            }
        }
    }
}
