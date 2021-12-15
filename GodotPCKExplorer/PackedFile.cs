using System;
using System.IO;

namespace GodotPCKExplorer
{
    public class PackedFile
    {
        private BinaryReader reader;
        public string FilePath;
        public long Offset;
        public long OffsetPosition;
        public long Size;
        public byte[] MD5;

        public PackedFile(BinaryReader reader, string path, long offset, long offsetPosition, long size, byte[] _MD5)
        {
            this.reader = reader;
            FilePath = path;
            Offset = offset;
            OffsetPosition = offsetPosition;
            Size = size;
            MD5 = _MD5;
        }

        public delegate void VoidInt(int progress);
        public event VoidInt OnProgress;

        public bool ExtractFile(string basePath, bool overwriteExisting = true)
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
            catch (Exception e)
            {
                Utils.ShowMessage(e.Message, "Error");
                return false;
            }

            const int buf_max = 65536;

            try
            {
                if (Size > 0)
                {
                    reader.BaseStream.Seek(Offset, SeekOrigin.Begin);
                    long to_write = Size;

                    while (to_write > 0)
                    {
                        var read = reader.ReadBytes(Math.Min(buf_max, (int)to_write));
                        file.Write(read);
                        to_write -= read.Length;

                        OnProgress?.Invoke(100 - (int)((double)to_write / Size * 100));
                    }
                }
            }
            catch (Exception e)
            {
                Utils.ShowMessage(e.Message, "Error");
                file.Close();
                try
                {
                    File.Delete(path);
                }
                catch
                {

                }
            }

            file.Close();
            return true;
        }
    }
}
