using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GodotPCKExplorer
{
    public class PackedFile
    {
        private BinaryReader reader;
        /// <summary>
        /// The name of the file in the package hierarchy.
        /// </summary>
        public string FilePath;
        /// <summary>
        /// File offset inside the package.
        /// </summary>
        public long Offset;
        /// <summary>
        /// Required for manipulating addresses.
        /// </summary>
        public long PositionOfOffsetValue;
        /// <summary>
        /// File size inside the package.
        /// </summary>
        public long Size;
        /// <summary>
        /// Hash to check the correctness of the file.
        /// </summary>
        public byte[] MD5;
        /// <summary>
        /// Individual file flags.
        /// Now only: 0 or 1 (Encrypted).
        /// </summary>
        public int Flags;

        public int PackVersion;

        public PackedFile(BinaryReader reader, string path, long contentOffset, long positionOfOffsetValue, long size, byte[] MD5, int flags, int pack_version)
        {
            this.reader = reader;
            FilePath = path;
            Offset = contentOffset;
            PositionOfOffsetValue = positionOfOffsetValue;
            Size = size;
            this.MD5 = MD5;
            Flags = flags;

            PackVersion = pack_version;
        }

        public delegate void VoidInt(int progress);
        public event VoidInt OnProgress;

        public bool IsEncrypted
        {
            get => (Flags & PCKUtils.PCK_FILE_ENCRYPTED) != 0;
        }

        public bool ExtractFile(string basePath, bool overwriteExisting = true, byte[] encKey = null, bool check_md5 = true, CancellationToken? cancellationToken = null)
        {
            string path = basePath + "/" + FilePath.Replace("res://", "");
            string dir = Path.GetDirectoryName(path);
            BinaryWriter file;

            try
            {
                if (File.Exists(path))
                {
                    if (!overwriteExisting)
                        return true;

                    File.Delete(path);
                }

                Directory.CreateDirectory(dir);
                file = new BinaryWriter(File.OpenWrite(path));
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error);
                return false;
            }

            try
            {
                if (Size > 0)
                {
                    reader.BaseStream.Seek(Offset, SeekOrigin.Begin);

                    BinaryReader tmp_reader = reader;

                    if (IsEncrypted)
                        tmp_reader = PCKUtils.ReadEncryptedBlockIntoMemoryStream(reader, encKey);

                    long to_write = Size;

                    while (to_write > 0)
                    {
                        var read = tmp_reader.ReadBytes(Math.Min(PCKUtils.BUFFER_MAX_SIZE, (int)to_write));
                        file.Write(read);
                        to_write -= read.Length;

                        OnProgress?.Invoke(100 - (int)((double)to_write / Size * 100));
                    }

                    if (IsEncrypted)
                    {
                        tmp_reader.Close();
                        tmp_reader.Dispose();
                    }
                    file.Close();

                    if (check_md5 && PackVersion > 1)
                    {
                        var exp_md5 = PCKUtils.GetFileMD5(path);
                        if (!exp_md5.SequenceEqual(MD5))
                        {
                            PCKActions.progress?.ShowMessage($"The MD5 of the exported file is not equal to the MD5 specified in the PCK.\n{PCKUtils.ByteArrayToHexString(MD5, " ")} != {PCKUtils.ByteArrayToHexString(exp_md5, " ")}", "Error", MessageType.Error);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var res = PCKActions.progress?.ShowMessage(ex, "Error", MessageType.Error, PCKMessageBoxButtons.OKCancel);
                file.Close();
                try
                {
                    File.Delete(path);
                }
                catch { }

                if (res == PCKDialogResult.Cancel)
                {
                    // TODO test cancel in process
                    return false;
                }
            }

            return true;
        }
    }
}
