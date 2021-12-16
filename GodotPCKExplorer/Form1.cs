using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public partial class Form1 : Form
    {
        PCKReader pckReader = new PCKReader();
        string FormBaseTitle = "";
        long TotalOpenedSize = 0;

        public Form1()
        {
            GUIConfig.Load();

            InitializeComponent();
            Icon = Properties.Resources.icon;
            FormBaseTitle = Text;

            overwriteExported.Checked = GUIConfig.Instance.OverwriteExported;
            UpdateStatuStrip();
            UpdateRecentList();

            dataGridView1.SelectionChanged += (o, e) => UpdateStatuStrip();
            extractToolStripMenuItem.Enabled = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpAbout = new AboutBox1();
            tmpAbout.ShowDialog();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = ofd_open_pack.ShowDialog();

            if (res == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                OpenFile(ofd_open_pack.FileName);
            }
        }

        void UpdateRecentList()
        {
            recentToolStripMenuItem.DropDownItems.Clear();

            if (GUIConfig.Instance.RecentOpenedFiles.Count > 0)
            {
                recentToolStripMenuItem.Enabled = true;
                foreach (var f in GUIConfig.Instance.RecentOpenedFiles)
                {
                    recentToolStripMenuItem.DropDownItems.Add(
                        new ToolStripButton(f, null, (s, e) => OpenFile(f)));
                }

            }
            else
            {
                recentToolStripMenuItem.Enabled = false;
            }
        }

        public void OpenFile(string path)
        {
            CloseFile();

            path = Path.GetFullPath(path);
            if (pckReader.OpenFile(path))
            {
                foreach (var f in pckReader.Files)
                {
                    var tmpRow = new DataGridViewRow();
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.FilePath });
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.Offset });
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

                    dataGridView1.Rows.Add(tmpRow);

                    Text = $"\"{Utils.GetShortPath(pckReader.PackPath, 50)}\" Pack version: {pckReader.PCK_VersionPack}. Godot Version: {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}";
                }

                // update recent files
                var list = GUIConfig.Instance.RecentOpenedFiles;

                if (list.Contains(path))
                {
                    list.Remove(path);
                    list.Insert(0, path);
                }
                else
                {
                    list.Insert(0, path);
                    while (list.Count > 16)
                        list.RemoveAt(list.Count - 1);
                }

                TotalOpenedSize = 0;

                foreach (var f in pckReader.Files.Values)
                {
                    TotalOpenedSize += f.Size;
                }

                extractToolStripMenuItem.Enabled = true;

                GUIConfig.Instance.Save();
                UpdateRecentList();
                UpdateStatuStrip();
            }
            else
            {
                // update recent files
                var list = GUIConfig.Instance.RecentOpenedFiles;
                if (list.Contains(path))
                    list.Remove(path);
                GUIConfig.Instance.Save();
            }
        }

        public void CloseFile()
        {
            extractToolStripMenuItem.Enabled = false;

            dataGridView1.Rows.Clear();
            pckReader.Close();
            Text = FormBaseTitle;
            TotalOpenedSize = 0;

            UpdateStatuStrip();
        }

        void UpdateStatuStrip()
        {
            if (pckReader.IsOpened)
            {
                long size = 0;

                foreach (DataGridViewRow f in dataGridView1.SelectedRows)
                {
                    size += (long)f.Cells[2].Tag;
                }

                tssl_version_and_stats.Text = $"Version: {pckReader.PCK_VersionPack}.{pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}" +
                    $" Files count: {pckReader.Files.Count}" +
                    $" Total size: {Utils.SizeSuffix(TotalOpenedSize)}";

                if (dataGridView1.SelectedRows.Count > 0)
                    tssl_selected_size.Text = $"Selected: {dataGridView1.SelectedRows.Count} Size: {Utils.SizeSuffix(size)}";
                else
                    tssl_selected_size.Text = "";
            }
            else
            {
                tssl_selected_size.Text = "";
                tssl_version_and_stats.Text = "";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fbd_extract_folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                List<string> rows = new List<string>();
                foreach (DataGridViewRow i in dataGridView1.SelectedRows)
                    rows.Add((string)i.Cells[0].Value);

                pckReader.ExtractFiles(rows, fbd_extract_folder.SelectedPath, overwriteExported.Checked);
            }
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fbd_extract_folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                pckReader.ExtractFiles(pckReader.Files.Select((f) => f.Key), fbd_extract_folder.SelectedPath, overwriteExported.Checked);
            }
        }

        private void packFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fbd_pack_folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                var dlg = new CreatePCKFile();
                dlg.SetFolderPath(fbd_pack_folder.SelectedPath);
                dlg.ShowDialog();
            }
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
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

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    var pck = new PCKReader();

                    if (pck.OpenFile(files[0], false))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Effect == DragDropEffects.Copy)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    OpenFile(files[0]);
                }
            }
        }

        private void overwriteExported_Click(object sender, EventArgs e)
        {
            GUIConfig.Instance.OverwriteExported = overwriteExported.Checked;
            GUIConfig.Instance.Save();
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            // TODO search
            //dataGridView1.so
        }

        private void removePackFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ofd_remove_pck_from_exe.ShowDialog() == DialogResult.OK)
            {
                PCKActions.RipPCKRun(ofd_remove_pck_from_exe.FileName);
            }
        }
    }
}
