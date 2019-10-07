using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
	public class PackedFile
	{
		private BinaryReader reader;
		public string FilePath;
		public long Offset;
		public long Size;
		public byte[] MD5;

		public PackedFile(BinaryReader _reader, string _Path, long _Offset, long _Size, byte[] _MD5)
		{
			reader = _reader;
			FilePath = _Path;
			Offset = _Offset;
			Size = _Size;
			MD5 = _MD5;
		}

		public delegate void VoidInt(int progress);
		public event VoidInt OnProgress;

		public bool ExtractFile(string basePath)
		{
			string path = basePath + "/" + FilePath.Replace("res://", "");
			string dir = Path.GetDirectoryName(path);
			BinaryWriter file;

			try
			{
				if (File.Exists(path))
				{
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
