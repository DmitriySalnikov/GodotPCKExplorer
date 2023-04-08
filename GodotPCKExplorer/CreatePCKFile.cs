using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public partial class CreatePCKFile : Form
    {
        Dictionary<string, PCKPacker.FileToPack> files = new Dictionary<string, PCKPacker.FileToPack>();
        Font MatchCaseNormal = null;
        Font MatchCaseStrikeout = null;

        public CreatePCKFile()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;

            tb_folder_path.Text = GUIConfig.Instance.FolderPath;
            SetFolderPath(tb_folder_path.Text);

            MatchCaseNormal = btn_match_case.Font;
            MatchCaseStrikeout = new Font(btn_match_case.Font, FontStyle.Strikeout);
            UpdateMatchCaseFilterButton();

            var ver = GUIConfig.Instance.PackedVersion;

            cb_ver.SelectedItem = ver.PackVersion.ToString();
            nud_major.Value = ver.Major;
            nud_minor.Value = ver.Minor;
            nud_revision.Value = ver.Revision;

            cb_embed.Checked = GUIConfig.Instance.EmbedPCK;

            nud_alignment.Value = GUIConfig.Instance.PCKAlignment;
            cb_enable_encryption.Checked = GUIConfig.Instance.EncryptPCK;
        }

        public void SetFolderPath(string path)
        {
            var bp = new BackgroundProgress();
            var bw = bp.bg_worker;
            var are = new AutoResetEvent(false);

            var filesScan = new List<PCKPacker.FileToPack>();

            bw.DoWork += (s, e) =>
            {
                try
                {
                    if (!Directory.Exists(path))
                        return;

                    var folder = Path.GetFullPath(path);
                    var cancel = false;

                    bw.ReportProgress(0);
                    Utils.ScanFoldersForFilesAdvanced(folder, filesScan, ref folder, ref cancel, bw);
                    if (cancel || bw.CancellationPending)
                        filesScan.Clear();
                }
                finally
                {
                    are.Set();
                }
            };

            bp.UnknowPercents = true;
            bw.RunWorkerAsync();
            bp.ShowDialog();
            are.WaitOne();

            GC.Collect();
            files = filesScan.ToDictionary((f) => f.OriginalPath);

            UpdateTableContent();
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
            l_total_count.Text = $"Files count: {files.Count}";
        }

        void UpdateTableContent()
        {
            dataGridView1.Rows.Clear();
            foreach (var f in files)
            {
                if (string.IsNullOrEmpty(searchText.Text) ||
                    (!string.IsNullOrEmpty(searchText.Text) && Utils.IsMatchWildCard(f.Key, searchText.Text, GUIConfig.Instance.MatchCaseFilterPackingForm)))
                {
                    var tmpRow = new DataGridViewRow();
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Key });
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

                    dataGridView1.Rows.Add(tmpRow);
                }
            }
        }

        void UpdateMatchCaseFilterButton()
        {
            if (GUIConfig.Instance.MatchCaseFilterPackingForm)
                btn_match_case.Font = MatchCaseNormal;
            else
                btn_match_case.Font = MatchCaseStrikeout;
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
                bool p_res = PCKActions.PackPCKRun(files.Values, file, ver.ToString(), (uint)nud_alignment.Value, cb_embed.Checked);

                GUIConfig.Instance.PackedVersion = ver;
                GUIConfig.Instance.EmbedPCK = cb_embed.Checked;
                GUIConfig.Instance.FolderPath = tb_folder_path.Text;
                GUIConfig.Instance.PCKAlignment = (uint)nud_alignment.Value;
                GUIConfig.Instance.EncryptPCK = cb_enable_encryption.Checked;
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

        private void tb_folder_path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetFolderPath(tb_folder_path.Text);
                e.Handled = true;
            }
        }

        private void btn_browse_Click(object sender, EventArgs e)
        {
            if (fbd_pack_folder.ShowDialog() == DialogResult.OK)
            {
                tb_folder_path.Text = Path.GetFullPath(fbd_pack_folder.SelectedPath);
                SetFolderPath(tb_folder_path.Text);
            }
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            SetFolderPath(tb_folder_path.Text);
        }

        private void textBoxWithPlaceholder1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                UpdateTableContent();
            }
        }

        private void btn_filter_Click(object sender, EventArgs e)
        {
            UpdateTableContent();
        }

        private void btn_match_case_Click(object sender, EventArgs e)
        {
            GUIConfig.Instance.MatchCaseFilterPackingForm = !GUIConfig.Instance.MatchCaseFilterPackingForm;
            GUIConfig.Instance.Save();
            UpdateMatchCaseFilterButton();
            UpdateTableContent();
        }

        private void btn_generate_key_Click(object sender, EventArgs e)
        {
            new CreatePCKEncryption().ShowDialog();
        }
    }
}
