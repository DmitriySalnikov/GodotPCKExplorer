using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
						dataGridView1.Rows.Add(f.Value.FilePath, f.Value.Offset, f.Value.Size);
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
			var res = folderBrowserDialog1.ShowDialog();
			if (res == DialogResult.OK)
			{
				string basePath = folderBrowserDialog1.SelectedPath;

				foreach (DataGridViewRow i in dataGridView1.SelectedRows)
				{
					string path = (string)(i.Cells[0].Value);
					if (path != null)
						pckReader.Files[path].ExtractFile(basePath);
				}
			}
		}

		private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var res = folderBrowserDialog1.ShowDialog();
			if (res == DialogResult.OK)
			{
				string basePath = folderBrowserDialog1.SelectedPath;

				foreach (DataGridViewRow i in dataGridView1.Rows)
				{
					string path = (string)(i.Cells[0].Value);
					if (path != null)
						pckReader.Files[path].ExtractFile(basePath);
				}
			}
		}

		private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}
	}
}
