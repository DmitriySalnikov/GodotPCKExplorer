using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GodotPCKExplorer
{
    class PCKEncryptedReader : IDisposable
    {
        public BinaryReader Stream;
        public byte[] Key;

        long start_position;
        long data_start_position;
        public byte[] MD5;
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
            StartIV = binReader.ReadBytes(16);

            data_start_position = Stream.BaseStream.Position;

            DataSizeEncoded = PCKUtils.AlignAddress(DataSize, 16);
            DataSizeDelta = (int)(DataSizeEncoded - DataSize);
        }

        public void Reset()
        {
            Stream.BaseStream.Position = data_start_position;
        }

        public IEnumerable<byte[]> ReadEncryptedBlocks()
        {
            byte[] iv = StartIV.ToArray();
            byte[] temp_encryption_buffer = new byte[PCKUtils.BUFFER_MAX_SIZE];
            long end_position = Stream.BaseStream.Position + DataSizeEncoded;

            using (var mtls = new mbedTLS())
            {
                mtls.set_key(Key);

                while (Stream.BaseStream.Position < end_position)
                {
                    if ((end_position - Stream.BaseStream.Position) > temp_encryption_buffer.Length)
                    {
                        var size = Stream.BaseStream.Read(temp_encryption_buffer, 0, temp_encryption_buffer.Length);
                        mtls.decrypt_cfb(iv, temp_encryption_buffer, temp_encryption_buffer.Length, out byte[] output);
                        yield return output;
                    }
                    else
                    {
                        byte[] buffer = new byte[end_position - Stream.BaseStream.Position];
                        var size = Stream.BaseStream.Read(buffer, 0, buffer.Length);
                        mtls.decrypt_cfb(iv, buffer, (int)(buffer.Length - DataSizeDelta), out byte[] output);
                        yield return output;
                    }
                }
            }
        }

        public void Dispose()
        {
            Stream = null;
            Key = null;
            MD5 = null;
            StartIV = null;
        }
    }
}
