using System;
using System.Collections.Generic;
using System.IO;
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
        [ThreadStatic]
        static byte[]? temp_buffer;

        [ThreadStatic]
        static byte[]? md5_buffer;

        public enum PACK_VERSION
        {
            Godot_3 = 1,
            Godot_4 = 2,
            Godot_4_5 = 3,
        }

        public const int PCK_MAGIC = 0x43504447;

        public const int BUFFER_MAX_SIZE = 1024 * 1024;
        public const int UnknownProgressStatus = -1234;

        public const string PathPrefixRes = "res://";
        public const string PathPrefixUser = "user://";
        public const string PathExtractPrefixUser = "@@user@@/";
        public const string PathExtractTagRemoval = ".@@removal@@";

        [Flags]
        public enum PCK_FLAG
        {
            DIR_ENCRYPTED = 1 << 0,
            REL_FILEBASE = 1 << 1, // Added in 4.3 https://github.com/godotengine/godot/commit/7e65fd87253fecb630151bbc4c6ac31d5cfa01a0
        }
        [Flags]
        public enum PCK_FILE
        {
            FLAG_ENCRYPTED = 1 << 0,
            // before 4.4 set offset to 0 instead of flag. file_access_pack.h:199
            FLAG_REMOVAL = 1 << 1, // Added in 4.4 https://github.com/godotengine/godot/commit/d76fbb7a40c56fa4b10edc017dc33a2d668c5c0d
        }

        public static readonly Dictionary<int, VersionLimitRanges> VersionLimits = new Dictionary<int, VersionLimitRanges>()
        {
            {1, new VersionLimitRanges(1,0,0, 3,ushort.MaxValue,ushort.MaxValue)},
            {2, new VersionLimitRanges(4,0,0, 4,4,ushort.MaxValue)},
            {3, new VersionLimitRanges(4,5,0, ushort.MaxValue,ushort.MaxValue,ushort.MaxValue)},
        };

        static readonly Random rng = new Random();

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

            byte[] result = new byte[hex.Length / 2];
            int j = 0;

            for (int i = 0; i < hex.Length; i += 2)
            {
                result[j] = Convert.ToByte(hex.Substring(i, 2), 16);
                j++;
            }

            return result;
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
                return expected_size_in_bytes != 0 && expected_size_in_bytes == hex.Length / 2;

            return false;
        }

        public static byte[] GetFileMD5(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var bytes = md5.ComputeHash(stream);
            if (bytes.Length > 16)
                throw new FormatException("Wrong size of MD5 hash");
            return bytes;
        }

        public static byte[] GetStreamMD5(Stream stream, long from = 0, long to = 0)
        {
            if (to - from < 0)
                throw new ArgumentOutOfRangeException("The length of the range in the stream cannot be less than zero.");

            if (from < 0)
                throw new ArgumentOutOfRangeException("The starting address is less than zero.");

            if (to > stream.Length)
                throw new ArgumentOutOfRangeException("The end address is greater than the length of the stream.");

            if (to == 0)
                to = stream.Length;

            md5_buffer ??= new byte[BUFFER_MAX_SIZE];
            using var md5 = MD5.Create();

            stream.Position = from;
            while (stream.Position < to)
            {
                if (stream.Position + BUFFER_MAX_SIZE < to)
                {
                    _ = stream.Read(md5_buffer, 0, BUFFER_MAX_SIZE);
                    md5.TransformBlock(md5_buffer, 0, BUFFER_MAX_SIZE, null, 0);
                }
                else
                {
                    long range = to - stream.Position;
                    _ = stream.Read(md5_buffer, 0, (int)range);
                    md5.TransformBlock(md5_buffer, 0, (int)range, null, 0);
                }
            }

            md5.TransformFinalBlock(md5_buffer, 0, 0);
            var bytes = md5.Hash;
            if (bytes.Length > 16)
                throw new FormatException("Wrong size of MD5 hash");
            return bytes;
        }

        internal static IEnumerable<ReadOnlyMemory<byte>> ReadStreamAsMemoryBlocks(Stream stream, long from = 0, long to = 0)
        {
            if (to - from < 0)
                throw new ArgumentOutOfRangeException("The length of the range in the stream cannot be less than zero.");

            if (from < 0)
                throw new ArgumentOutOfRangeException("The starting address is less than zero.");

            if (to > stream.Length)
                throw new ArgumentOutOfRangeException("The end address is greater than the length of the stream.");

            if (to == 0)
                to = stream.Length;

            temp_buffer ??= new byte[BUFFER_MAX_SIZE];

            stream.Seek(from, SeekOrigin.Begin);
            while (stream.Position < to)
            {
                if (stream.Position + BUFFER_MAX_SIZE < to)
                {
                    _ = stream.Read(temp_buffer, 0, BUFFER_MAX_SIZE);
                    yield return new ReadOnlyMemory<byte>(temp_buffer, 0, BUFFER_MAX_SIZE);
                }
                else
                {
                    int range = (int)(to - stream.Position);
                    _ = stream.Read(temp_buffer, 0, range);
                    yield return new ReadOnlyMemory<byte>(temp_buffer, 0, range);
                }
            }
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

        public static void AddPaddingAlignedByAddress(BinaryWriter p_file, long p_address, uint p_alignment, bool randomFill = false)
        {
            AddPadding(p_file, AlignAddress(p_address, p_alignment) - p_address, randomFill);
        }

        public static string GetResFilePathInPCK(string path, PCKVersion version)
        {
            // removed res:// prefix in 4.4. editor_export_platform.cpp:227 https://github.com/godotengine/godot/commit/d76fbb7a40c56fa4b10edc017dc33a2d668c5c0d

            if (version.Major >= 4 && version.Minor >= 4)
            {
                return path;
            }
            else
            {
                // res:// + prefix/ + path.file
                return PathPrefixRes + path;
            }
        }

        public static List<PCKPackerRegularFile> GetListOfFilesToPack(string folder, PCKVersion version, string packPathPrefix = "", CancellationToken? cancellationToken = null)
        {
            if (!Directory.Exists(folder))
                return new List<PCKPackerRegularFile>();

            folder = Path.GetFullPath(folder);
            var files = new List<PCKPackerRegularFile>();
            var cancel = false;

            const string op = "Scan folder";
            PCKActions.progress?.LogProgress(op, $"Started scanning files in '{folder}'");
            PCKActions.progress?.LogProgress(op, PCKUtils.UnknownProgressStatus);

            GetListOfFilesToPackRecursive(folder, files, ref folder, ref version, ref packPathPrefix, ref cancel, cancellationToken);
            if (cancel)
                files.Clear();
            GC.Collect();

            PCKActions.progress?.LogProgress(op, "Scan completed!");

            return files;
        }

        static void GetListOfFilesToPackRecursive(string folder, List<PCKPackerRegularFile> files, ref string basePath, ref PCKVersion version, ref string packPathPrefix, ref bool cancel, CancellationToken? cancellationToken = null)
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

                GetListOfFilesToPackRecursive(d, files, ref basePath, ref version, ref packPathPrefix, ref cancel, cancellationToken);
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
                    var file = new PCKPackerRegularFile(f, basePath);
                    file.UpdateFileInfo(version, packPathPrefix);
                    files.Add(file);
                    PCKActions.progress?.LogProgress(op, f);
                    PCKActions.progress?.LogProgress(op, files.Count, "Found files: ");
                }
                catch (Exception ex)
                {
                    cancel = PCKActions.progress?.ShowMessage($"{ex.Message}\nThe file will be skipped!", "Warning", MessageType.Warning, PCKMessageBoxButtons.OKCancel) == PCKDialogResult.Cancel;
                }
            }
        }

        public readonly struct VersionLimitRanges
        {
            public readonly ushort MinMajor, MinMinor, MinRevision;
            public readonly ushort MaxMajor, MaxMinor, MaxRevision;

            public VersionLimitRanges(ushort minMajor, ushort minMinor, ushort minRevision, ushort maxMajor, ushort maxMinor, ushort maxRevision)
            {
                MinMajor = minMajor; MinMinor = minMinor; MinRevision = minRevision;
                MaxMajor = maxMajor; MaxMinor = maxMinor; MaxRevision = maxRevision;
            }
        }

        public static bool IsVersionInAllowedLimits(PCKVersion ver)
        {
            if (!ver.IsValid())
                return false;

            if (VersionLimits.TryGetValue(ver.Pack, out VersionLimitRanges ranges))
            {
                static ulong get_num(int major, int minor, int rev) { return ((ulong)major << 16 * 2) + ((ulong)minor << 16 * 1) + (ulong)rev; }

                ulong ver_num = get_num(ver.Major, ver.Minor, ver.Revision);
                ulong ver_min = get_num(ranges.MinMajor, ranges.MinMinor, ranges.MinRevision);
                ulong ver_max = get_num(ranges.MaxMajor, ranges.MaxMinor, ranges.MaxRevision);

                if (ver_num >= ver_min && ver_num <= ver_max)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
