using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GodotPCKExplorer
{
    public class PCKEncryptedReader : IDisposable
    {
        [ThreadStatic]
        static byte[]? temp_encryption_buffer;

        public BinaryReader? Stream;
        public byte[] Key;

        readonly long start_position;
        readonly long data_start_position;
        public byte[] MD5;
        public long HeaderSize;
        public long DataSize;
        public long DataSizeEncoded;
        int DataSizeDelta;
        public byte[] StartIV;

        public PCKEncryptedReader(BinaryReader binReader, byte[] key)
        {
            Stream = binReader;
            Key = key;
            start_position = Stream.BaseStream.Position;

            MD5 = binReader.ReadBytes(16);
            DataSize = binReader.ReadInt64();
            StartIV = binReader.ReadBytes(mbedTLS.CHUNK_SIZE);

            data_start_position = Stream.BaseStream.Position;

            HeaderSize = (int)(data_start_position - start_position);
            DataSizeEncoded = PCKUtils.AlignAddress(DataSize, mbedTLS.CHUNK_SIZE);
            DataSizeDelta = (int)(DataSizeEncoded - DataSize);
        }

        public void Reset()
        {
            if (Stream != null)
            {
                Stream.BaseStream.Position = data_start_position;
            }
        }

        public IEnumerable<ReadOnlyMemory<byte>> ReadEncryptedBlocks()
        {
            if (Stream == null)
            {
                yield break;
            }

            if (PCKUtils.BUFFER_MAX_SIZE % mbedTLS.CHUNK_SIZE != 0)
                throw new ArgumentException($"{nameof(PCKUtils.BUFFER_MAX_SIZE)} must be a multiple of {mbedTLS.CHUNK_SIZE}.");

            temp_encryption_buffer ??= new byte[PCKUtils.BUFFER_MAX_SIZE];
            var output_buffer = new Memory<byte>(temp_encryption_buffer, 0, PCKUtils.BUFFER_MAX_SIZE);

            byte[] iv = StartIV.ToArray();
            long end_position = Stream.BaseStream.Position + DataSizeEncoded;

            using var mtls = new mbedTLS();
            mtls.set_key(Key);

            foreach (var block in PCKUtils.ReadStreamAsMemoryBlocks(Stream.BaseStream, data_start_position, end_position))
            {
                mtls.decrypt_cfb(iv, block, output_buffer);
                if (block.Length == PCKUtils.BUFFER_MAX_SIZE)
                {
                    yield return new ReadOnlyMemory<byte>(temp_encryption_buffer, 0, PCKUtils.BUFFER_MAX_SIZE);
                }
                else
                {
                    var dest_size = block.Length - DataSizeDelta;
                    yield return new ReadOnlyMemory<byte>(temp_encryption_buffer, 0, dest_size);
                }
            }
        }

        public void Dispose()
        {
            Stream = null;
        }
    }
}
