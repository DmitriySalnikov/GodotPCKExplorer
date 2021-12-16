using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public partial class CreatePCKFile : Form
    {
        Dictionary<string, PCKPacker.FileToPack> files = new Dictionary<string, PCKPacker.FileToPack>();

        public CreatePCKFile()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;

            var ver = GUIConfig.Instance.PackedVersion;

            cb_ver.SelectedItem = ver.PackVersion.ToString();
            nud_major.Value = ver.Major;
            nud_minor.Value = ver.Minor;
            nud_revision.Value = ver.Revision;

            cb_embed.Checked = GUIConfig.Instance.EmbedPCK;
        }

        public void SetFolderPath(string path)
        {
            files = Utils.ScanFoldersForFiles(path).ToDictionary((f) => f.OriginalPath);

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

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            files.Remove((string)e.Row.Cells[0].Value);
            CalculatePCKSize();
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            var ver = new PCKVersion(int.Parse((string)cb_ver.SelectedItem), (int)nud_major.Value, (int)nud_minor.Value, (int)nud_revision.Value);
            DialogResult res = DialogResult.No;
            string file = "";

            if (cb_embed.Checked)
            {
                res = ofd_pack_into.ShowDialog();
                file = ofd_pack_into.FileName;
            }
            else
            {
                res = sfd_save_pack.ShowDialog();
                file = sfd_save_pack.FileName;
            }

            if (res == DialogResult.OK)
            {
                bool p_res = PCKActions.PackPCKRun(files.Values, file, ver.ToString(), cb_embed.Checked);

                GUIConfig.Instance.PackedVersion = ver;
                GUIConfig.Instance.EmbedPCK = cb_embed.Checked;
                GUIConfig.Instance.Save();
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
