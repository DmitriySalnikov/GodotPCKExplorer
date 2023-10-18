using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GodotPCKExplorer
{
    public class PCKFile
    {
        private readonly BinaryReader reader;
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
        /// <summary>
        /// Pack Version
        /// </summary>
        public int PackVersion;

        public PCKFile(BinaryReader reader, string path, long contentOffset, long positionOfOffsetValue, long size, byte[] MD5, int flags, int pack_version)
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
        public event VoidInt? OnProgress;

        public bool IsEncrypted
        {
            get => (Flags & PCKUtils.PCK_FILE_ENCRYPTED) != 0;
        }

        public bool ExtractFile(string basePath, bool overwriteExisting = true, byte[]? encKey = null, bool check_md5 = true, CancellationToken? cancellationToken = null)
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
                file = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                return false;
            }

            try
            {
                if (Size > 0)
                {
                    reader.BaseStream.Seek(Offset, SeekOrigin.Begin);

                    BinaryReader tmp_reader = reader;

                    long to_write = Size;
                    if (IsEncrypted)
                    {
                        if (encKey == null)
                        {
                            PCKActions.progress?.ShowMessage($"Failed to extract the packed file.\nThe PCK file is encrypted, but the decryption key was not specified.", "Error", MessageType.Error);
                            return false;
                        }

                        using (var r = new PCKEncryptedReader(reader, encKey))
                        {
                            foreach (var chunk in r.ReadEncryptedBlocks())
                            {
                                file.Write(chunk.Span);
                                to_write -= chunk.Length;
                                OnProgress?.Invoke(100 - (int)((double)to_write / Size * 100));

                                if (cancellationToken?.IsCancellationRequested ?? false)
                                    return false;
                            }
                        }

                        OnProgress?.Invoke(100);
                    }
                    else
                    {
                        while (to_write > 0)
                        {
                            var read = tmp_reader.ReadBytes(Math.Min(PCKUtils.BUFFER_MAX_SIZE, (int)to_write));
                            file.Write(read);
                            to_write -= read.Length;

                            OnProgress?.Invoke(100 - (int)((double)to_write / Size * 100));

                            if (cancellationToken?.IsCancellationRequested ?? false)
                                return false;
                        }

                        OnProgress?.Invoke(100);
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
                var res = PCKActions.progress?.ShowMessage(ex, MessageType.Error, PCKMessageBoxButtons.OKCancel);
                file.Close();

                try
                {
                    File.Delete(path);
                }
                catch { }

                if (res == PCKDialogResult.Cancel)
                {
                    return false;
                }
            }
            finally
            {
                file.Close();
            }

            return true;
        }
    }
}
