using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public partial class Form1 : Form
    {
        PCKReader pckReader = new PCKReader();
        string FormBaseTitle = "";

        public Form1()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;
            FormBaseTitle = Text;
            tssl_total_size.Text = "";
            tssl_files_count.Text = "";
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
                OpenFile(openFileDialog1.FileName);
            }
        }

        public void OpenFile(string path)
        {
            if (pckReader.OpenFile(path))
            {
                foreach (var f in pckReader.Files)
                {
                    var tmpRow = new DataGridViewRow();
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.FilePath });
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.Offset });
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

                    dataGridView1.Rows.Add(tmpRow);

                    Text = $"\"{pckReader.PackPath}\" Pack version {pckReader.PCK_VersionPack}. Godot version {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}";
                    CalculatePCKSize();
                }
            }
        }

        void CalculatePCKSize()
        {
            long size = 0;

            foreach (var f in pckReader.Files.Values)
            {
                size += f.Size;
            }

            tssl_files_count.Text = $"Files count: {pckReader.Files.Count}";
            tssl_total_size.Text = $"Total size: ~{Utils.SizeSuffix(size)}";
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
                List<string> rows = new List<string>();
                foreach (DataGridViewRow i in dataGridView1.SelectedRows)
                    rows.Add((string)i.Cells[0].Value);

                pckReader.ExtractFiles(rows, folderBrowserDialog1.SelectedPath);
            }
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = folderBrowserDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                List<string> rows = new List<string>();
                foreach (DataGridViewRow i in dataGridView1.Rows)
                    rows.Add((string)i.Cells[0].Value);

                pckReader.ExtractFiles(rows, folderBrowserDialog1.SelectedPath);
            }
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
            Text = FormBaseTitle;
            tssl_total_size.Text = "";
            tssl_files_count.Text = "";
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            pckReader.Close();
        }

        private void registerProgramToOpenPCKInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellIntegration.Register();
        }

        private void unregisterProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellIntegration.Unregister();
        }
    }
}
