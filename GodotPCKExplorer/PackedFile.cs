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

		public bool ExtractFile(string basePath)
		{
			string path = basePath + "/" + FilePath.Replace("res://", "");
			string dir = Path.GetDirectoryName(path);
			FileStream file;

			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}

				Directory.CreateDirectory(dir);
				file = File.OpenWrite(path);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error");
				return false;
			}

			if (Size > 0)
			{
				reader.BaseStream.Seek(Offset, SeekOrigin.Begin);
				file.Write(reader.ReadBytes((int)Size), 0, (int)Size);
			}

			file.Close();
			return true;
		}
	}
}
