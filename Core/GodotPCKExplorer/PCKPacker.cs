using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GodotPCKExplorer
{
    public class PCKPackerFile
    {
        public string Path { get; protected set; }
        public long Size { get; protected set; } = 0;
        public long IndexOffsetPosition = 0;

        public byte[]? MD5 { get; protected set; } = null;
        public bool IsEncrypted = false;
        public bool IsRemoval = false;

        public PCKPackerFile(string path)
        {
            Path = path;
            IndexOffsetPosition = 0;
        }

        public virtual void CalculateMD5()
        {

        }

        public virtual IEnumerable<ReadOnlyMemory<byte>> ReadMemoryBlocks()
        {
            yield break;
        }
    }

    public sealed class PCKPackerRegularFile : PCKPackerFile
    {
        public string OriginalPath { get; }
        readonly long originalSize = 0;
        readonly string rootFolder;

        public PCKPackerRegularFile(string o_path, string base_path) : base("")
        {
            OriginalPath = o_path.Replace('\\', '/');
            originalSize = new FileInfo(OriginalPath).Length;

            rootFolder = base_path.Replace('\\', '/');
            if (!rootFolder.EndsWith('/'))
            {
                rootFolder += '/';
            }
        }

        public void UpdateFileInfo(PCKVersion version, string prefix = "")
        {
            // added Removal flag in 4.4. file_access_pack.h:53 https://github.com/godotengine/godot/commit/d76fbb7a40c56fa4b10edc017dc33a2d668c5c0d

            prefix = prefix.Replace("\\", "/");
            var file_path = OriginalPath.Replace(rootFolder, "").Replace("\\", "/");
            bool is_user = file_path.StartsWith(PCKUtils.PathPrefixUser);

            // paths with user://
            if (file_path.StartsWith(PCKUtils.PathExtractPrefixUser) || is_user)
            {
                Path = PCKUtils.PathPrefixUser + file_path.Replace(PCKUtils.PathExtractPrefixUser, "");
            }
            // regular paths with res://
            else
            {
                Path = PCKUtils.GetResFilePathInPCK(prefix + file_path, version);
            }

            string file_name = System.IO.Path.GetFileName(Path);

            if (file_name.Contains(PCKUtils.PathExtractTagRemoval))
            {
                Path = Path.Replace(PCKUtils.PathExtractTagRemoval, "");
                IsRemoval = true;
            }
            else
            {
                IsRemoval = false;
            }

            if (IsRemoval)
            {
                Size = 0;
            }
            else
            {
                Size = originalSize;
            }
        }

        public override void CalculateMD5()
        {
            if (IsRemoval)
            {
                // Fill with 0
                MD5 = new byte[16];
            }
            else
            {
                MD5 = PCKUtils.GetFileMD5(OriginalPath);
            }
        }

        public override IEnumerable<ReadOnlyMemory<byte>> ReadMemoryBlocks()
        {
            if (IsRemoval)
            {
                // If something unexpected happens, prevent the Removal file from being written.
                yield break;
            }

            using var stream = File.Open(OriginalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(stream))
            {
                yield return block;
            }
        }
    }

    public sealed class PCKPackerPCKFile : PCKPackerFile
    {
        public PCKReaderFile OriginalFile;

        public PCKPackerPCKFile(PCKReaderFile o_file) : base(o_file.FilePath)
        {
            OriginalFile = o_file;
            Size = o_file.Size;
            IsEncrypted = o_file.IsEncrypted;
            IsRemoval = o_file.IsRemoval;
        }

        public override void CalculateMD5()
        {
            MD5 = OriginalFile.MD5;
        }

        public override IEnumerable<ReadOnlyMemory<byte>> ReadMemoryBlocks()
        {
            foreach (var block in OriginalFile.ReadMemoryBlocks())
            {
                yield return block;
            }
        }
    }

    public static class PCKPacker
    {
        // 16 bytes for MD5
        // 8 bytes for size of data
        // 16 bytes for IV
        internal const int ENCRYPTED_HEADER_SIZE = 16 + 8 + mbedTLS.CHUNK_SIZE;

        [ThreadStatic]
        static byte[]? temp_encryption_output_buffer;

        static void CloseAndDeleteFile(BinaryWriter? writer, string out_pck)
        {
            writer?.Close();

            try
            {
                File.Delete(out_pck);
            }
            catch (Exception ex)
            {
                PCKActions.progress?.ShowMessage(ex, MessageType.Error);
            }
        }

        /// <summary>
        /// Create a new PCK file from existing files.
        /// </summary>
        /// <param name="outPck">Output file. It can be a new or an existing file.</param>
        /// <param name="embed">If enabled and an existing <see cref="outPck"/> is specified, then the PCK will be embedded into this file.</param>
        /// <param name="files">Enumeration of <see cref="PCKPackerRegularFile"/> files to be packed. Must be in a valid "res://" or "user://" format.</param>
        /// <param name="godotVersion">PCK file version.</param>
        /// <param name="alignment">The address of each file will be aligned to this value.</param>
        /// <param name="encKey">Specify the encryption key if you want the file to be encrypted. To specify a <see cref="string"/>, look at <seealso cref="PCKUtils.HexStringToByteArray(string?)"/></param>
        /// <param name="encryptIndex">Whether to encrypt the index (list of contents).</param>
        /// <param name="cancellationToken">Cancellation token to interrupt the extraction process.</param>
        /// <returns><c>true</c> if successful</returns>
        public static bool PackFiles(string outPck, bool embed, IEnumerable<PCKPackerFile> files, PCKVersion godotVersion, uint alignment = 16, byte[]? encKey = null, bool encryptIndex = false, CancellationToken? cancellationToken = null)
        {
            var start_time = DateTime.UtcNow;
            byte[]? EncryptionKey = encKey;

            const string baseOp = "Pack files";
            var op = baseOp;

            if (!godotVersion.IsValid())
            {
                PCKActions.progress?.ShowMessage("Incorrect version is specified!", "Error", MessageType.Error);
                return false;
            }

            bool encryptFiles = files.Any(f => f is PCKPackerRegularFile && f.IsEncrypted);

            if (godotVersion.PackVersion == (int)PCKUtils.PACK_VERSION.Godot_3)
            {
                if (EncryptionKey != null || encryptIndex || encryptFiles)
                {
                    PCKActions.progress?.ShowMessage("Encryption is not supported for PCK files for Godot 3 (pack version 1).", "Error", MessageType.Error);
                    return false;
                }
            }

            if (embed)
            {
                if (!File.Exists(outPck))
                {
                    PCKActions.progress?.ShowMessage("Attempt to embed a package in a non-existent file", "Error", MessageType.Error);
                    return false;
                }
                else
                {
                    var pck = new PCKReader();
                    if (pck.OpenFile(outPck, false))
                    {
                        pck.Close();
                        PCKActions.progress?.ShowMessage("Attempt to embed a package in a file with an already embedded package or in a regular '.pck' file", "Error", MessageType.Error);
                        return false;
                    }
                }
            }

            if (EncryptionKey == null)
            {
                if (encryptIndex || encryptFiles)
                {
                    PCKActions.progress?.ShowMessage("The encryption key is not specified, although the encryption mode is activated.", "Error", MessageType.Error);
                    return false;
                }
            }

            try
            {
                PCKActions.progress?.LogProgress(op, "Starting.");
                PCKActions.progress?.LogProgress(op, $"Version: {godotVersion}");
                PCKActions.progress?.LogProgress(op, $"Alignment: {alignment}");
                if (encryptIndex || encryptFiles)
                {
                    PCKActions.progress?.LogProgress(op, $"Encryption key: {PCKUtils.ByteArrayToHexString(EncryptionKey)}");
                    PCKActions.progress?.LogProgress(op, $"Encrypt Index: {encryptIndex}");
                    PCKActions.progress?.LogProgress(op, $"Encrypt Files: {encryptFiles}");
                }

                // delete if not embbeding
                if (!embed)
                {
                    try
                    {
                        if (File.Exists(outPck))
                            File.Delete(outPck);
                    }
                    catch (Exception ex)
                    {
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }
                }

                BinaryWriter? binWriter = null;
                try
                {
                    binWriter = new BinaryWriter(File.Open(outPck, FileMode.OpenOrCreate, FileAccess.ReadWrite));
                }
                catch (Exception ex)
                {
                    CloseAndDeleteFile(binWriter, outPck);
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    return false;
                }

                long embed_start = 0;
                if (embed)
                {
                    binWriter.BaseStream.Seek(0, SeekOrigin.End);
                    embed_start = binWriter.BaseStream.Position;

                    // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:994
                    // Ensure embedded PCK starts at a 64-bit multiple
                    try
                    {
                        PCKUtils.AddPadding(binWriter, binWriter.BaseStream.Position % 8);
                    }
                    catch (Exception ex)
                    {
                        CloseAndDeleteFile(binWriter, outPck);
                        PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                        return false;
                    }

                }

                long pck_start = binWriter.BaseStream.Position;

                try
                {
                    PCKActions.progress?.LogProgress(op, "Writing the file index");

                    binWriter.Write(PCKUtils.PCK_MAGIC);
                    binWriter.Write(godotVersion.PackVersion);
                    binWriter.Write(godotVersion.Major);
                    binWriter.Write(godotVersion.Minor);
                    binWriter.Write(godotVersion.Revision);

                    long file_base_address = -1;
                    int pack_flags = 0;

                    if (godotVersion.PackVersion == (int)PCKUtils.PACK_VERSION.Godot_4)
                    {
                        if (encryptIndex)
                            pack_flags |= (int)PCKUtils.PCK_FLAG.DIR_ENCRYPTED;

                        // https://github.com/godotengine/godot/commit/7e65fd87253fecb630151bbc4c6ac31d5cfa01a0 4.3+
                        if (embed && godotVersion.Major >= 4 && godotVersion.Minor >= 3)
                            pack_flags |= (int)PCKUtils.PCK_FLAG.REL_FILEBASE;

                        binWriter.Write(pack_flags); // pack_flags

                        file_base_address = binWriter.BaseStream.Position;
                        binWriter.Write((long)0); // file_base
                    }

                    PCKUtils.AddPadding(binWriter, 16 * sizeof(int)); // reserved

                    // write the files count
                    binWriter.Write((int)files.Count());

                    var index_writer = binWriter;
                    long index_begin_pos = binWriter.BaseStream.Position;

                    long total_size = 0;

                    {
                        if (encryptIndex)
                            index_writer = new BinaryWriter(new MemoryStream());

                        // Multi-threaded MD5 pre-calculation
                        if (godotVersion.PackVersion >= (int)PCKUtils.PACK_VERSION.Godot_4)
                        {
                            Parallel.ForEach(files, (f) =>
                            {
                                f.CalculateMD5();
                                PCKActions.progress?.LogProgress(op, $"Calculated MD5: {f.Path}\n{PCKUtils.ByteArrayToHexString(f.MD5, " ")}");
                            });
                        }

                        op = baseOp + ", writing an index";
                        // write pck index
                        int file_idx = 0;
                        foreach (var file in files)
                        {
                            // cancel packing
                            if (cancellationToken?.IsCancellationRequested ?? false)
                            {
                                CloseAndDeleteFile(binWriter, outPck);
                                return false;
                            }

                            file_idx++;
                            var str = Encoding.UTF8.GetBytes(file.Path).ToList();
                            var str_len = str.Count;

                            // Godot 4's PCK uses padding for some reason...
                            if (godotVersion.PackVersion == (int)PCKUtils.PACK_VERSION.Godot_4)
                                str_len = (int)PCKUtils.AlignAddress(str_len, 4); // align with 4

                            // store pascal string (size, data)
                            index_writer.Write(str_len);
                            index_writer.Write(str.ToArray());

                            // Add padding for string
                            if (godotVersion.PackVersion == (int)PCKUtils.PACK_VERSION.Godot_4)
                                PCKUtils.AddPadding(index_writer, str_len - str.Count);

                            file.IndexOffsetPosition = index_writer.BaseStream.Position;
                            index_writer.Write((long)0); // offset for later use
                            index_writer.Write((long)file.Size); // size

                            total_size += file.Size; // for progress bar

                            if (godotVersion.PackVersion < (int)PCKUtils.PACK_VERSION.Godot_4)
                            {
                                // # empty md5
                                PCKUtils.AddPadding(index_writer, 16 * sizeof(byte));
                            }
                            else
                            {
                                index_writer.Write(file.MD5);

                                int flags = 0;
                                if (file.IsEncrypted)
                                    flags |= (int)PCKUtils.PCK_FILE.FLAG_ENCRYPTED;

                                if (file.IsRemoval)
                                    flags |= (int)PCKUtils.PCK_FILE.FLAG_REMOVAL;

                                index_writer.Write(flags);
                            }

                            PCKActions.progress?.LogProgress(op, (int)(((double)file_idx / files.Count()) * 100));
                        }

                        if (encryptIndex)
                        {
                            // Later it will be encrypted and the data size will be aligned to 16 + encrypted header
                            PCKUtils.AddPadding(binWriter, PCKUtils.AlignAddress(index_writer.BaseStream.Length, mbedTLS.CHUNK_SIZE) + ENCRYPTED_HEADER_SIZE);
                        }
                    }

                    // approximate size of the output file for displaying progress
                    total_size += binWriter.BaseStream.Position;

                    // file_base or individual offset
                    long offset = binWriter.BaseStream.Position;
                    offset = PCKUtils.AlignAddress(offset, alignment);

                    // end of index
                    PCKUtils.AddPadding(binWriter, offset - binWriter.BaseStream.Position, encryptIndex); // fill random bytes between index and files

                    long file_base = offset;
                    if (godotVersion.PackVersion == (int)PCKUtils.PACK_VERSION.Godot_4)
                    {
                        // update actual address of file_base in the header
                        long file_base_store = file_base;
                        if ((pack_flags & (int)PCKUtils.PCK_FLAG.REL_FILEBASE) != 0)
                            file_base_store -= pck_start;

                        binWriter.BaseStream.Seek(file_base_address, SeekOrigin.Begin);
                        binWriter.Write(file_base_store);
                        binWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
                    }

                    // write actual files data
                    PCKActions.progress?.LogProgress(op, "Writing the content of files");
                    op = baseOp + ", writing file contents";

                    int count = 0;
                    foreach (var file in files)
                    {
                        // cancel packing
                        if (cancellationToken?.IsCancellationRequested ?? false)
                        {
                            CloseAndDeleteFile(binWriter, outPck);
                            return false;
                        }

                        PCKActions.progress?.LogProgress(op, file.Path);

                        if (file.IsRemoval)
                        {
                            PCKActions.progress?.LogProgress(op, "- The file is marked as Removal. Skipping.");
                            continue;
                        }

                        // go back to store the file's offset
                        {
                            long pos = index_writer.BaseStream.Position;
                            index_writer.BaseStream.Seek(file.IndexOffsetPosition, SeekOrigin.Begin);

                            if (godotVersion.PackVersion < (int)PCKUtils.PACK_VERSION.Godot_4)
                            {
                                index_writer.Write((long)offset);
                            }
                            else
                            {
                                index_writer.Write((long)offset - file_base);
                            }

                            index_writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                        }

                        long actual_file_size = file.Size;
                        bool ignore_encryption = false;

                        if (file is PCKPackerPCKFile pck_file)
                        {
                            actual_file_size = pck_file.OriginalFile.ActualSize;
                            ignore_encryption = true; // Ignore any encryption for PCKFiles
                        }

                        if (file.IsEncrypted && !ignore_encryption)
                        {
                            var result_size = PackStreamEncrypted(binWriter, file.Size, file.ReadMemoryBlocks(), EncryptionKey ?? throw new NullReferenceException(nameof(EncryptionKey)), file.MD5 ?? throw new NullReferenceException(nameof(file.MD5)), () =>
                            {
                                PCKActions.progress?.LogProgress(op, (int)((double)binWriter.BaseStream.Position / total_size * 100)); // update progress bar
                                return !(cancellationToken?.IsCancellationRequested ?? false);
                            });

                            // canceled
                            if (result_size == -1 || Math.Abs(result_size - actual_file_size) > ENCRYPTED_HEADER_SIZE + mbedTLS.CHUNK_SIZE)
                            {
                                CloseAndDeleteFile(binWriter, outPck);
                                return false;
                            }

                            actual_file_size = result_size;
                        }
                        else
                        {
                            foreach (var block in file.ReadMemoryBlocks())
                            {
                                binWriter.Write(block.Span);
                                PCKActions.progress?.LogProgress(op, (int)((double)binWriter.BaseStream.Position / total_size * 100)); // update progress bar

                                // cancel packing
                                if (cancellationToken?.IsCancellationRequested ?? false)
                                {
                                    CloseAndDeleteFile(binWriter, outPck);
                                    return false;
                                }
                            }
                        }

                        // get offset of the next file and add some padding
                        offset = PCKUtils.AlignAddress(offset + actual_file_size, alignment);
                        PCKUtils.AddPadding(binWriter, offset - binWriter.BaseStream.Position, encryptFiles); // fill random bytes between files

                        count += 1;
                    }

                    // TODO add PCK validation

                    // If the index is encrypted, then it must be written after all other operations in order to properly handle file offsets
                    if (encryptIndex)
                    {
                        // Move to start of index
                        long pos = binWriter.BaseStream.Position;
                        binWriter.BaseStream.Seek(index_begin_pos, SeekOrigin.Begin);

                        PackStreamEncrypted(binWriter, index_writer.BaseStream.Length, PCKUtils.ReadStreamAsMemoryBlocks(index_writer.BaseStream), EncryptionKey ?? throw new NullReferenceException(nameof(EncryptionKey)), PCKUtils.GetStreamMD5(index_writer.BaseStream));
                        index_writer.Close();
                        index_writer.Dispose();
                        index_writer = null;

                        binWriter.BaseStream.Seek(pos, SeekOrigin.Begin);
                    }

                    if (embed)
                    {
                        // Godot's 779a5e56218b7fa2ab34ab22ab5b1b2aaa19346f editor_export.cpp:1073
                        // Ensure embedded data ends at a 64-bit multiple
                        long embed_end = binWriter.BaseStream.Position - embed_start + 12;
                        PCKUtils.AddPadding(binWriter, embed_end % 8);

                        long pck_size = binWriter.BaseStream.Position - pck_start;
                        binWriter.Write((long)pck_size);
                        binWriter.Write((int)PCKUtils.PCK_MAGIC);
                    }

                    PCKActions.progress?.LogProgress(op, 100);
                    PCKActions.progress?.LogProgress(op, "Completed!");
                }
                catch (Exception ex)
                {
                    PCKActions.progress?.ShowMessage(ex, MessageType.Error);
                    CloseAndDeleteFile(binWriter, outPck);
                    return false;
                }

                binWriter.Close();
                PCKActions.progress?.Log($"Pack complete! Time spent: {(DateTime.UtcNow - start_time).TotalSeconds:F2}s.");
                return true;
            }
            catch (Exception ex)
            {
                PCKActions.progress?.Log(ex);
                return false;
            }
            finally
            {

            }
        }

        static long PackStreamEncrypted(BinaryWriter binWriter, long dataSize, IEnumerable<ReadOnlyMemory<byte>> dataBlocks, byte[] key, byte[] md5, Func<bool>? onStep = null)
        {
            if (PCKUtils.BUFFER_MAX_SIZE % mbedTLS.CHUNK_SIZE != 0)
                throw new ArgumentException($"{nameof(PCKUtils.BUFFER_MAX_SIZE)} must be a multiple of {mbedTLS.CHUNK_SIZE}.");

            temp_encryption_output_buffer ??= new byte[PCKUtils.BUFFER_MAX_SIZE];
            var output_buffer = new Memory<byte>(temp_encryption_output_buffer, 0, PCKUtils.BUFFER_MAX_SIZE);

            binWriter.Write(md5);
            binWriter.Write((long)dataSize); // original size

            var iv = new byte[mbedTLS.CHUNK_SIZE];
            Random rnd = new Random();
            rnd.NextBytes(iv);

            binWriter.Write(iv);
            long total_size = 0;

            using var mtls = new mbedTLS();

            mtls.set_key(key);

            foreach (var block in dataBlocks)
            {
                mtls.encrypt_cfb(iv, block, output_buffer, out long out_size);
                binWriter.Write(output_buffer[..(int)out_size].Span);
                total_size += out_size;

                if (onStep != null)
                {
                    if (!onStep.Invoke())
                        return -1;
                }
            }

            return total_size + ENCRYPTED_HEADER_SIZE;
        }
    }
}
