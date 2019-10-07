using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
	public class PCKReader
	{
		BinaryReader fileStream = null;
		public Dictionary<string, PackedFile> Files = new Dictionary<string, PackedFile>();
		public string PackPath = "";
		public int PCK_VersionPack = 0;
		public int PCK_VersionMajor = 0;
		public int PCK_VersionMinor = 0;
		public int PCK_VersionRevision = 0;

		public void Close()
		{
			if (fileStream != null && fileStream.BaseStream != null)
			{
				fileStream.Close();
				fileStream = null;

			}

			Files.Clear();
			PackPath = "";
			PCK_VersionPack = 0;
			PCK_VersionMajor = 0;
			PCK_VersionMinor = 0;
			PCK_VersionRevision = 0;
		}

		public bool OpenFile(string p_path)
		{
			if (fileStream != null && fileStream.BaseStream != null)
			{
				fileStream.Close();
				fileStream = null;

				Files.Clear();
			}

			try
			{
				p_path = Path.GetFullPath(p_path);
				fileStream = new BinaryReader(File.OpenRead(p_path));
			}
			catch (Exception e)
			{
				Utils.ShowMessage(e.Message, "Error");
				return false;
			}

			int magic = fileStream.ReadInt32();

			if (magic != Program.PCK_MAGIC)
			{
				//maybe at the end.... self contained exe
				fileStream.BaseStream.Seek(4, SeekOrigin.End);
				magic = fileStream.ReadInt32();
				if (magic != Program.PCK_MAGIC)
				{
					fileStream.Close();
					Utils.ShowMessage("Not Godot PCK file!", "Error");
					return false;
				}
				fileStream.BaseStream.Seek(-12, SeekOrigin.Current);

				long ds = fileStream.ReadInt64();
				fileStream.BaseStream.Seek(-ds - 8, SeekOrigin.Current);

				magic = fileStream.ReadInt32();
				if (magic != Program.PCK_MAGIC)
				{
					fileStream.Close();
					Utils.ShowMessage("Not Godot PCK file!", "Error");

					return false;
				}
			}

			PCK_VersionPack = fileStream.ReadInt32();
			PCK_VersionMajor = fileStream.ReadInt32();
			PCK_VersionMinor = fileStream.ReadInt32();
			PCK_VersionRevision = fileStream.ReadInt32();

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

			PackPath = p_path;
			return true;
		}

		public bool ExtractAllFiles(string folder)
		{
			return ExtractFiles(Files.Keys.ToList(), folder);
		}

		public bool ExtractFiles(List<string> names, string folder)
		{
			var bp = new BackgroundProgress();
			var bw = bp.backgroundWorker1;
			bool result = true;

			bw.DoWork += (sender, ev) =>
			{
				string basePath = folder;

				int count = 0;
				double one_file_in_progress_line = 1.0 / names.Count;
				foreach (var path in names)
				{
					if (path != null)
					{
						PackedFile.VoidInt upd = (p) =>
						{
							bw.ReportProgress((int)(((double)count / names.Count * 100) + (p * one_file_in_progress_line)));
						};
						Files[path].OnProgress += upd;

						if (!Files[path].ExtractFile(basePath))
						{
							Files[path].OnProgress -= upd;
							result = false;
							return;
						}

						Files[path].OnProgress -= upd;
					}

					count++;
					bw.ReportProgress((int)((double)count / names.Count * 100));

					if (bw.CancellationPending)
					{
						result = false;
						return;
					}
				}
			};

			bw.RunWorkerAsync();
			bp.ShowDialog();

			if (result)
				Utils.ShowMessage("Complete!", "Progress");

			return result;
		}
	}
}
