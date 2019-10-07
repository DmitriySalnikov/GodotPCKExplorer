using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
	public partial class CreatePCKFile : Form
	{
		string BasePath = "C:/";
		public const string GodotVersionSave = "godot_version.save";
		Dictionary<string, PCKPacker.FileToPack> files = new Dictionary<string, PCKPacker.FileToPack>();

		public CreatePCKFile()
		{
			InitializeComponent();
			Icon = Properties.Resources.icon;

			if (File.Exists(GodotVersionSave))
			{
				try
				{
					var reader = new BinaryReader(File.OpenRead(GodotVersionSave));
					cb_ver.SelectedItem = reader.ReadByte().ToString();
					nud_major.Value = reader.ReadByte();
					nud_minor.Value = reader.ReadByte();
					nud_revision.Value = reader.ReadByte();
					reader.Close();
				}
				catch
				{

				}
			}
		}

		public void SetFolderPath(string path)
		{
			BasePath = path;

			ScanFoldersForFiles(BasePath);

			dataGridView1.Rows.Clear();
			foreach (var f in files)
			{
				var tmpRow = new DataGridViewRow();
				tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Key });
				tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

				dataGridView1.Rows.Add(tmpRow);
			}

			CalculatePCKSize();
		}

		void CalculatePCKSize()
		{
			long size = 0;

			foreach (var f in files.Values)
			{
				size += f.Size;
			}

			l_total_size.Text = $"Total size: ~{Utils.SizeSuffix(size)}";
		}

		void ScanFoldersForFiles(string folder)
		{
			foreach (string d in Directory.EnumerateDirectories(folder))
			{
				ScanFoldersForFiles(d);
			}

			foreach (string f in Directory.EnumerateFiles(folder))
			{
				var inf = new FileInfo(f);
				files.Add(f, new PCKPacker.FileToPack(f, f.Replace(BasePath + "\\", "res://").Replace("\\", "/"), inf.Length));
			}
		}

		private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			files.Remove((string)e.Row.Cells[0].Value);
			CalculatePCKSize();
		}

		private void btn_create_Click(object sender, EventArgs e)
		{
			var res = saveFileDialog1.ShowDialog();
			if (res == DialogResult.OK)
			{
				var packer = new PCKPacker();

				bool p_res = packer.PackFiles(saveFileDialog1.FileName, files.Values.ToList(), 8,
					new PCKPacker.PCKVersion(int.Parse((string)cb_ver.SelectedItem), (int)nud_major.Value, (int)nud_minor.Value, (int)nud_revision.Value)
					);

				try
				{
					if (File.Exists(GodotVersionSave))
					{
						File.Delete(GodotVersionSave);
					}
				
					var writer = new BinaryWriter(File.OpenWrite(GodotVersionSave));
					writer.Write((byte)int.Parse((string)cb_ver.SelectedItem));
					writer.Write((byte)nud_major.Value);
					writer.Write((byte)nud_minor.Value);
					writer.Write((byte)nud_revision.Value);
					writer.Close();
				}
				catch
				{

				}
			}
		}

		private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{
			if (e.Column.Index != 1)
			{
				e.Handled = false;
				return;
			}

			e.SortResult = (long)(dataGridView1.Rows[e.RowIndex1].Cells[1].Tag) > (long)(dataGridView1.Rows[e.RowIndex2].Cells[1].Tag) ? 1 : -1;
			e.Handled = true;
		}
	}
}
