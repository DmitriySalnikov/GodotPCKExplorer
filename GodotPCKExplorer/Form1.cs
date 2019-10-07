using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GodotPCKExplorer.PackedFile;

namespace GodotPCKExplorer
{
	public partial class Form1 : Form
	{
		PCKReader pckReader = new PCKReader();

		public Form1()
		{
			InitializeComponent();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var tmpAbout = new AboutBox1();
			tmpAbout.ShowDialog();
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var res = openFileDialog1.ShowDialog();

			if (res == DialogResult.OK)
			{
				dataGridView1.Rows.Clear();

				if (pckReader.OpenFile(openFileDialog1.FileName))
				{
					foreach (var f in pckReader.Files)
					{
						var tmpRow = new DataGridViewRow();
						tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.FilePath });
						tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.Offset });
						tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

						dataGridView1.Rows.Add(tmpRow);
					}
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<DataGridViewRow> rows = new List<DataGridViewRow>();
			foreach (DataGridViewRow i in dataGridView1.SelectedRows)
				rows.Add(i);

			ExtractFiles(rows);
		}

		private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<DataGridViewRow> rows = new List<DataGridViewRow>();
			foreach (DataGridViewRow i in dataGridView1.Rows)
				rows.Add(i);

			ExtractFiles(rows);
		}

		private void ExtractFiles(List<DataGridViewRow> rows)
		{
			var bp = new BackgroundProgress();
			var bw = bp.backgroundWorker1;
			bool result = true;

			var res = folderBrowserDialog1.ShowDialog();

			bw.DoWork += (sender, ev) =>
			{
				if (res == DialogResult.OK)
				{
					string basePath = folderBrowserDialog1.SelectedPath;

					int count = 0;
					double one_file_in_progress_line = 1.0 / rows.Count;
					foreach (var i in rows)
					{
						string path = (string)(i.Cells[0].Value);
						if (path != null)
						{
							VoidInt upd = (p) =>
							{
								bw.ReportProgress((int)(((double)count / rows.Count * 100) + (p * one_file_in_progress_line)));
							};
							pckReader.Files[path].OnProgress += upd;

							if (!pckReader.Files[path].ExtractFile(basePath))
							{
								pckReader.Files[path].OnProgress -= upd;

								result = false;
								return;
							}
						}

						count++;
						bw.ReportProgress((int)((double)count / rows.Count * 100));

						if (bw.CancellationPending)
						{
							result = false;
							return;
						}
					}
				}
				else
				{
					result = false;
				}
			};

			bw.RunWorkerAsync();
			bp.ShowDialog();

			if (result)
				MessageBox.Show("Complete!");
		}

		private void packFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var res = folderBrowserDialog_pack_folder.ShowDialog();
			if (res == DialogResult.OK)
			{
				var dlg = new CreatePCKFile();
				dlg.SetFolderPath(folderBrowserDialog_pack_folder.SelectedPath);
				dlg.ShowDialog();
			}
		}

		private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dataGridView1.Rows.Clear();
			pckReader.Close();
		}

		private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{
			if (e.Column.Index != 2)
			{
				e.Handled = false;
				return;
			}

			e.SortResult = (long)(dataGridView1.Rows[e.RowIndex1].Cells[2].Tag) > (long)(dataGridView1.Rows[e.RowIndex2].Cells[2].Tag) ? 1 : -1;
			e.Handled = true;
		}
	}
}
