namespace GodotPCKExplorer.UI
{
    public partial class CreatePCKFile : Form
    {
        Dictionary<string, PCKPackerRegularFile> filesToPack = [];
        readonly Font MatchCaseNormal;
        readonly Font MatchCaseStrikeout;

        public CreatePCKFile()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;

            tb_folder_path.Text = GUIConfig.Instance.FolderPath;
            tb_prefix.Text = GUIConfig.Instance.PackPathPrefix;
            SetFolderPath(tb_folder_path.Text);

            MatchCaseNormal = btn_match_case.Font;
            MatchCaseStrikeout = new Font(btn_match_case.Font, FontStyle.Strikeout);
            UpdateMatchCaseFilterButton();

            cb_packFiltered.Checked = GUIConfig.Instance.PackOnlyFiltered;
            cb_previewPaths.Checked = GUIConfig.Instance.PreviewPaths;

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
            var filesScan = new List<PCKPackerRegularFile>();

            if (Directory.Exists(path))
                Program.DoTaskWithProgressBar((t) => filesScan = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(path), cancellationToken: t),
                    this);

            GC.Collect();

            if (filesScan != null)
                filesToPack = filesScan.ToDictionary((f) => f.OriginalPath);
            else
                filesToPack = [];

            UpdateTableContent();
        }

        IEnumerable<PCKPackerRegularFile> GetFilesList()
        {
            if (cb_packFiltered.Checked)
            {
                var filteredRows = new List<PCKPackerRegularFile>();
                foreach (DataGridViewRow i in dataGridView1.Rows)
                {
                    string file = (string)i.Cells[0].Tag;
                    if (filesToPack.ContainsKey(file))
                        filteredRows.Add(filesToPack[file]);
                }

                return filteredRows;
            }
            else
            {
                return filesToPack.Values;
            }
        }

        void CalculatePCKSize()
        {
            long size = 0;

            var files = GetFilesList();
            foreach (var f in files)
            {
                size += f.Size;
            }

            l_total_size.Text = $"Total size: ~{Utils.SizeSuffix(size)}";
            l_total_count.Text = $"Files count: {filesToPack.Count}";
        }

        void UpdateTableContent()
        {
            bool preview = cb_previewPaths.Checked;
            string prefix = tb_prefix.Text;

            dataGridView1.Rows.Clear();
            foreach (var f in filesToPack)
            {
                if (string.IsNullOrWhiteSpace(searchText.Text) ||
                    (!string.IsNullOrWhiteSpace(searchText.Text) && Utils.IsMatchWildCard(f.Key, searchText.Text, GUIConfig.Instance.MatchCaseFilterPackingForm)))
                {
                    var tmpRow = new DataGridViewRow();
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = preview ? PCKUtils.GetResFilePath(f.Value.Path, prefix) : f.Value.OriginalPath, Tag = f.Value.OriginalPath });
                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

                    dataGridView1.Rows.Add(tmpRow);
                }
            }
            CalculatePCKSize();
        }

        void UpdateMatchCaseFilterButton()
        {
            if (GUIConfig.Instance.MatchCaseFilterPackingForm)
                btn_match_case.Font = MatchCaseNormal;
            else
                btn_match_case.Font = MatchCaseStrikeout;
        }

        private void dataGridView1_UserDeletedRow(object? sender, DataGridViewRowEventArgs e)
        {
            filesToPack.Remove((string)e.Row.Cells[0].Value);
            CalculatePCKSize();
        }

        private void btn_create_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(cb_ver.Text, out int pack_ver))
            {
                Program.ShowMessage("Incorrect package version format.", "Error", MessageType.Error);
                return;
            }

            var ver = new PCKVersion(pack_ver, (int)nud_major.Value, (int)nud_minor.Value, (int)nud_revision.Value);
            DialogResult res = DialogResult.No;
            string file = "";
            string prefix = tb_prefix.Text;

            if (cb_embed.Checked)
            {
                res = ofd_pack_into.ShowDialog(this);
                file = ofd_pack_into.FileName;
            }
            else
            {
                res = sfd_save_pack.ShowDialog(this);
                file = sfd_save_pack.FileName;
            }

            if (res == DialogResult.OK)
            {
                bool p_res = false;
                Program.DoTaskWithProgressBar((t) =>
                {
                    p_res = PCKActions.Pack(
                        GetFilesList(),
                        file,
                        ver.ToString(),
                        prefix,
                        (uint)nud_alignment.Value,
                        cb_embed.Checked,
                        GUIConfig.Instance.EncryptionKey,
                        GUIConfig.Instance.EncryptIndex && cb_enable_encryption.Checked,
                        GUIConfig.Instance.EncryptFiles && cb_enable_encryption.Checked,
                        t
                        );
                }, this);

                GUIConfig.Instance.PackedVersion = ver;
                GUIConfig.Instance.EmbedPCK = cb_embed.Checked;
                GUIConfig.Instance.FolderPath = tb_folder_path.Text;
                GUIConfig.Instance.PackPathPrefix = prefix;
                GUIConfig.Instance.PCKAlignment = (uint)nud_alignment.Value;
                GUIConfig.Instance.PackOnlyFiltered = cb_packFiltered.Checked;
                GUIConfig.Instance.PreviewPaths = cb_previewPaths.Checked;
                GUIConfig.Instance.EncryptPCK = cb_enable_encryption.Checked;
                GUIConfig.Instance.Save();
            }
        }

        private void dataGridView1_SortCompare(object? sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 1)
            {
                e.Handled = false;
                return;
            }

            e.SortResult = (long)(dataGridView1.Rows[e.RowIndex1].Cells[1].Tag) > (long)(dataGridView1.Rows[e.RowIndex2].Cells[1].Tag) ? 1 : -1;
            e.Handled = true;
        }

        private void tb_folder_path_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetFolderPath(tb_folder_path.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void tb_prefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateTableContent();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void tb_prefix_Leave(object sender, EventArgs e)
        {
            UpdateTableContent();
        }

        private void btn_browse_Click(object? sender, EventArgs e)
        {
            if (fbd_pack_folder.ShowDialog(this) == DialogResult.OK)
            {
                tb_folder_path.Text = Path.GetFullPath(fbd_pack_folder.SelectedPath);
                SetFolderPath(tb_folder_path.Text);
            }
        }

        private void btn_refresh_Click(object? sender, EventArgs e)
        {
            SetFolderPath(tb_folder_path.Text);
        }

        private void textBoxWithPlaceholder1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                UpdateTableContent();
            }
        }

        private void btn_clearFilter_Click(object sender, EventArgs e)
        {
            searchText.Text = string.Empty;
            UpdateTableContent();
        }

        private void btn_filter_Click(object? sender, EventArgs e)
        {
            UpdateTableContent();
        }

        private void btn_match_case_Click(object? sender, EventArgs e)
        {
            GUIConfig.Instance.MatchCaseFilterPackingForm = !GUIConfig.Instance.MatchCaseFilterPackingForm;
            GUIConfig.Instance.Save();
            UpdateMatchCaseFilterButton();
            UpdateTableContent();
        }

        private void btn_generate_key_Click(object? sender, EventArgs e)
        {
            using var tmp = new CreatePCKEncryption();
            tmp.ShowDialog(this);
        }

        private void cb_packFiltered_CheckedChanged(object sender, EventArgs e)
        {
            CalculatePCKSize();
        }

        private void cb_previewPaths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableContent();
        }
    }
}
