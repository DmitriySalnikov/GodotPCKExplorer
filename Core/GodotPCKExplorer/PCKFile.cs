using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GodotPCKExplorer
{
    public sealed class PCKFile
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

        public bool ExtractFile(string basePath, out string extractPath, out bool skippedExisted, bool overwriteExisting = true, byte[]? encKey = null, PCKExtractNoEncryptionKeyMode noKeyMode = PCKExtractNoEncryptionKeyMode.Cancel, CancellationToken? cancellationToken = null)
        {
            string path = extractPath = Path.GetFullPath(Path.Combine(basePath, FilePath.Replace(PCKUtils.PathPrefixRes, "").Replace(PCKUtils.PathPrefixUser, "@@user@@/")));
            string op = "Extracting file";

            skippedExisted = false;
            string dir = Path.GetDirectoryName(path);
            BinaryWriter file;

            if (File.Exists(path) && !overwriteExisting)
            {
                skippedExisted = true;
                return true;
            }

            if (IsEncrypted && encKey == null)
            {
                switch (noKeyMode)
                {
                    case PCKExtractNoEncryptionKeyMode.Cancel:
                        PCKActions.progress?.ShowMessage($"Failed to extract the packed file.\nThe file is encrypted, but the decryption key was not specified.", "Error", MessageType.Error);
                        return false;
                    case PCKExtractNoEncryptionKeyMode.Skip:
                        PCKActions.progress?.LogProgress(op, $"The file is encrypted, but it will be skipped according to the settings.");
                        return false;
                    case PCKExtractNoEncryptionKeyMode.AsIs:
                        // Rename file to mark it as encrypted
                        path = extractPath = Path.ChangeExtension(path, Path.GetExtension(path) + ".encrypted");
                        if (File.Exists(path) && !overwriteExisting)
                        {
                            skippedExisted = true;
                            return true;
                        }
                        break;
                }
            }

            try
            {
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

                    bool write_raw_file()
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
                        return true;
                    }

                    if (IsEncrypted)
                    {
                        if (encKey == null)
                        {
                            if (noKeyMode == PCKExtractNoEncryptionKeyMode.AsIs)
                            {
                                PCKActions.progress?.LogProgress(op, $"The file is encrypted, but it will be extracted without decryption according to the settings.");

                                // Read encryption header, add combine actual size and header
                                long pos = reader.BaseStream.Position;
                                using (var r = new PCKEncryptedReader(reader, new byte[0]))
                                {
                                    to_write = PCKEncryptedReader.EncryptionHeaderSize + r.DataSizeEncoded;
                                }
                                reader.BaseStream.Seek(pos, SeekOrigin.Begin);

                                write_raw_file();
                                OnProgress?.Invoke(100);
                                return false;
                            }
                        }
                        else
                        {
                            using var r = new PCKEncryptedReader(reader, encKey);
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
                        if (!write_raw_file())
                            return false;
                        OnProgress?.Invoke(100);
                    }
                    file.Close();
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

        public bool CheckMD5(string path)
        {
            if (PackVersion > 1)
            {
                var exp_md5 = PCKUtils.GetFileMD5(path);
                if (!exp_md5.SequenceEqual(MD5))
                {
                    PCKActions.progress?.ShowMessage($"The MD5 of the exported file is not equal to the MD5 specified in the PCK.\n{PCKUtils.ByteArrayToHexString(MD5, " ")} != {PCKUtils.ByteArrayToHexString(exp_md5, " ")}", "Error", MessageType.Error);
                    return false;
                }
            }
            return true;
        }
    }
}
