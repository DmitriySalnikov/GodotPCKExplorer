using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace GodotPCKUnpacker
{
	class PCKReader
	{
		BinaryReader fileStream = null;
		public Dictionary<string, PackedFile> Files = new Dictionary<string, PackedFile>();
		public int PCK_VersionMajor = 0;
		public int PCK_VersionMinor = 0;
		public int PCK_VersionLast = 0;

		public bool OpenFile(string p_path)
		{
			if(fileStream != null && fileStream.BaseStream != null)
			{
				fileStream.Close();
				fileStream = null;

				Files.Clear();
			}

			try
			{
				fileStream = new BinaryReader(File.OpenRead(p_path));
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error");
				return false;
			}

			int magic = fileStream.ReadInt32();

			if (magic != 0x43504447)
			{
				//maybe at the end.... self contained exe
				fileStream.BaseStream.Seek(4, SeekOrigin.End);
				magic = fileStream.ReadInt32();
				if (magic != 0x43504447)
				{
					fileStream.Close();
					MessageBox.Show("Error: Not Godot PCK file!", "Error");
					return false;
				}
				fileStream.BaseStream.Seek(-12, SeekOrigin.Current);

				long ds = fileStream.ReadInt64();
				fileStream.BaseStream.Seek(-ds - 8, SeekOrigin.Current);

				magic = fileStream.ReadInt32();
				if (magic != 0x43504447)
				{
					fileStream.Close();
					MessageBox.Show("Error: Not Godot PCK file!", "Error");
					return false;
				}
			}

			PCK_VersionLast = fileStream.ReadInt32();
			PCK_VersionMajor = fileStream.ReadInt32();
			PCK_VersionMinor = fileStream.ReadInt32();
			fileStream.ReadInt32(); // ver_rev

			//if (version != PACK_VERSION)
			//{
			//	fileStream.close();
			//	memdelete(f);
			//	ERR_FAIL_V_MSG(false, "Pack version unsupported: " + itos(version) + ".");
			//}
			//if (ver_major > VERSION_MAJOR || (ver_major == VERSION_MAJOR && ver_minor > VERSION_MINOR))
			//{
			//	fileStream.close();
			//	memdelete(f);
			//	ERR_FAIL_V_MSG(false, "Pack created with a newer version of the engine: " + itos(ver_major) + "." + itos(ver_minor) + ".");
			//}

			for (int i = 0; i < 16; i++)
			{
				//reserved
				fileStream.ReadInt32();
			}

			int file_count = fileStream.ReadInt32();

			for (int i = 0; i < file_count; i++)
			{
				string path = Encoding.UTF8.GetString(fileStream.ReadBytes(fileStream.ReadInt32())).Replace("\0", "");
				long ofs = fileStream.ReadInt64();
				long size = fileStream.ReadInt64();
				byte[] md5 = fileStream.ReadBytes(16);

				Files.Add(path, new PackedFile(fileStream, path, ofs, size, md5));
			};

			//fileStream.Close();
			return true;
		}
	}
}
