namespace GodotPCKExplorer.UI
{
    public partial class CreatePCKFile : Form
    {
        Dictionary<string, PCKPackerFile> filesToPack = [];
        readonly Font MatchCaseNormal;
        readonly Font MatchCaseStrikeout;

        readonly List<Control> patchModeControls = [];

        string currentSelectedDir = "";
        string currentSelectedPrefix = "";
        PCKReader pckReader = new();

        DataGridViewCellStyle styleNormalFile = new DataGridViewCellStyle();
        DataGridViewCellStyle styleRealFile = new DataGridViewCellStyle();
        DataGridViewCellStyle stylePCKFile = new DataGridViewCellStyle();
        DataGridViewCellStyle styleNormalFileSize = new DataGridViewCellStyle();
        DataGridViewCellStyle styleRealFileSize = new DataGridViewCellStyle();
        DataGridViewCellStyle stylePCKFileSize = new DataGridViewCellStyle();

        public CreatePCKFile()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;

            {
                styleNormalFile = dataGridView1.DefaultCellStyle.Clone();
                styleRealFile = dataGridView1.DefaultCellStyle.Clone();
                stylePCKFile = dataGridView1.DefaultCellStyle.Clone();

                styleRealFile.ForeColor = SystemColors.ControlText;
                styleRealFile.Font = new Font(styleRealFile.Font, FontStyle.Bold);

                stylePCKFile.ForeColor = SystemColors.ControlDarkDark;

                styleNormalFileSize = styleNormalFile.Clone();
                styleNormalFileSize.Alignment = DataGridViewContentAlignment.MiddleRight;
                styleRealFileSize = styleRealFile.Clone();
                styleRealFileSize.Alignment = DataGridViewContentAlignment.MiddleRight;
                stylePCKFileSize = stylePCKFile.Clone();
                stylePCKFileSize.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            tb_folder_path.Text = currentSelectedDir = GUIConfig.Instance.PackFolderPath;
            tb_prefix.Text = currentSelectedPrefix = GUIConfig.Instance.PackPathPrefix;

            MatchCaseNormal = btn_match_case.Font;
            MatchCaseStrikeout = new Font(btn_match_case.Font, FontStyle.Strikeout);
            UpdateMatchCaseFilterButton();

            cb_packFiltered.Checked = GUIConfig.Instance.PackOnlyFiltered;
            cb_previewPaths.Checked = GUIConfig.Instance.PackPreviewPaths;

            var ver = GUIConfig.Instance.PackVersion;
            cb_ver.SelectedItem = ver.PackVersion.ToString();
            nud_major.Value = ver.Major;
            nud_minor.Value = ver.Minor;
            nud_revision.Value = ver.Revision;

            cb_embed.Checked = GUIConfig.Instance.PackEmbedPCK;

            nud_alignment.Value = GUIConfig.Instance.PackPCKAlignment;
            cb_enable_encryption.Checked = GUIConfig.Instance.PackEncryptPCK;

            cb_enable_patching.Checked = GUIConfig.Instance.PackPatchingEnabled;
            tb_patch_target.Text = GUIConfig.Instance.PackPatchingTarget;
            patchModeControls.Add(tb_patch_target);
            patchModeControls.Add(btn_browse_patch_target);
            UpdatePatchModeControls();
            CalculatePCKSize();

            cb_packFiltered.CheckedChanged += cb_packFiltered_CheckedChanged;
            cb_previewPaths.CheckedChanged += cb_previewPaths_CheckedChanged;
            cb_enable_patching.CheckedChanged += cb_enable_patching_CheckedChanged;
        }

        private void CreatePCKFile_Shown(object sender, EventArgs e)
        {
            SetFolderPath(tb_folder_path.Text);
        }

        public void SetFolderPath(string path)
        {
            currentSelectedDir = path;
            RegenerateFilesToPackList();
        }

        void RegenerateFilesToPackList()
        {
            if (cb_enable_patching.Checked && !string.IsNullOrWhiteSpace(tb_patch_target.Text))
            {
                PCKReaderEncryptionKeyResult getEncKey()
                {
                    return new PCKReaderEncryptionKeyResult() { Key = GUIConfig.Instance.PackEncryptionKey ?? "" };
                }

                if (!pckReader.OpenFile(tb_patch_target.Text, getEncryptionKey: getEncKey))
                {
                    Program.ShowMessage("The PCK file could not be opened.\nIf it contains encrypted data, then specify the encryption key in the \"Encryption Setting\" menu.", "Error", MessageType.Error);
                }
            }
            else
            {
                pckReader.Close();
            }

            var ver = GetPCKVersion();
            var files_scan = new List<PCKPackerRegularFile>();
            if (Directory.Exists(currentSelectedDir))
                Program.DoTaskWithProgressBar((t) => files_scan = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(currentSelectedDir), ver, tb_prefix.Text, cancellationToken: t), this);

            filesToPack = [];

            // Fill with PCK files
            foreach (var file in pckReader.Files)
                filesToPack[file.Key] = new PCKPackerPCKFile(file.Value);

            // Replace by files from folder
            foreach (var file in files_scan)
                filesToPack[file.Path] = file;

            UpdateTableContent();
        }

        IEnumerable<PCKPackerFile> GetFilesList()
        {
            if (cb_packFiltered.Checked)
            {
                var filteredRows = new List<PCKPackerFile>();
                foreach (DataGridViewRow i in dataGridView1.Rows)
                {
                    string file = (string)i.Cells[0].Tag;
                    if (filesToPack.TryGetValue(file, out PCKPackerFile? value))
                        filteredRows.Add(value);
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

        void UpdatePCKVersionOfOpenedFiles()
        {
            var ver = GetPCKVersion();

            foreach (var p in filesToPack)
            {
                if (p.Value is PCKPackerRegularFile rf)
                {
                    rf.UpdateFileInfo(ver, tb_prefix.Text);
                }
            }

            UpdateTableContent();
        }

        void UpdateTableContent()
        {
            bool preview = cb_previewPaths.Checked;
            bool patch_enabled = cb_enable_patching.Checked && pckReader.IsOpened;
            bool contains_removal = filesToPack.Any(f => f.Value.IsRemoval);

            DataGridViewCellStyle current_styleRealFile = styleNormalFile;
            DataGridViewCellStyle current_stylePCKFile = styleNormalFile;

            DataGridViewCellStyle current_styleRealFileSize = styleNormalFileSize;
            DataGridViewCellStyle current_stylePCKFileSize = styleNormalFileSize;

            if (patch_enabled)
            {
                current_styleRealFile = styleRealFile;
                current_stylePCKFile = stylePCKFile;

                current_styleRealFileSize = styleRealFileSize;
                current_stylePCKFileSize = stylePCKFileSize;

                dataGridView1.Columns["patch"].Visible = true;
            }
            else
            {
                dataGridView1.Columns["patch"].Visible = false;
            }

            dataGridView1.Columns["removal"].Visible = contains_removal;

            dataGridView1.Rows.Clear();
            List<DataGridViewRow> tmp_rows = [];

            foreach (var f in filesToPack)
            {
                if (string.IsNullOrWhiteSpace(searchText.Text) ||
                    (!string.IsNullOrWhiteSpace(searchText.Text) && Utils.IsMatchWildCard(f.Key, searchText.Text, GUIConfig.Instance.MatchCaseFilterPackingForm)))
                {
                    if (patch_enabled && !preview)
                    {
                        if (f.Value is not PCKPackerRegularFile)
                            continue;
                    }

                    var tmpRow = new DataGridViewRow();
                    if (f.Value is PCKPackerRegularFile rf)
                    {
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = preview ? f.Value.Path : rf.OriginalPath, Tag = rf.OriginalPath, Style = current_styleRealFile });
                        if (pckReader.Files.TryGetValue(rf.Path, out var orig_file))
                            tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = $"{Utils.SizeSuffix(orig_file.Size)} -> {Utils.SizeSuffix(f.Value.Size)}", Tag = f.Value.Size, Style = current_styleRealFileSize });
                        else
                            tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size, Style = current_styleRealFileSize });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = "*", Tag = true });
                    }
                    else
                    {
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.Path, Tag = f.Value.Path, Style = current_stylePCKFile });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size, Style = current_stylePCKFileSize });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = "", Tag = false });
                    }

                    tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.IsRemoval ? "*" : string.Empty, Tag = f.Value.IsRemoval });

                    tmp_rows.Add(tmpRow);
                }
            }

            dataGridView1.Rows.AddRange([.. tmp_rows]);

            CalculatePCKSize();
        }

        void UpdateMatchCaseFilterButton()
        {
            if (GUIConfig.Instance.MatchCaseFilterPackingForm)
                btn_match_case.Font = MatchCaseNormal;
            else
                btn_match_case.Font = MatchCaseStrikeout;
        }

        void UpdatePatchModeControls()
        {
            foreach (var c in patchModeControls)
            {
                c.Enabled = cb_enable_patching.Checked;
            }
        }

        PCKVersion GetPCKVersion()
        {
            if (!int.TryParse(cb_ver.Text, out int pack_ver))
            {
                Program.ShowMessage("Incorrect package version format.", "Error", MessageType.Error);
                return new PCKVersion();
            }

            return new PCKVersion(pack_ver, (int)nud_major.Value, (int)nud_minor.Value, (int)nud_revision.Value);
        }

        private void CreatePCKFile_FormClosed(object sender, FormClosedEventArgs e)
        {
            pckReader.Close();
        }

        private void dataGridView1_UserDeletedRow(object? sender, DataGridViewRowEventArgs e)
        {
            filesToPack.Remove((string)e.Row.Cells[0].Value);
            CalculatePCKSize();
        }

        private void btn_create_Click(object? sender, EventArgs e)
        {
            var ver = GetPCKVersion();
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

                // TODO temp. Encrypt every real file if needed
                foreach (var f in filesToPack)
                    if (f.Value is PCKPackerRegularFile rf)
                        rf.IsEncrypted = cb_enable_encryption.Checked && GUIConfig.Instance.PackEncryptFiles;

                Program.DoTaskWithProgressBar((t) =>
                {
                    if (cb_enable_encryption.Checked)
                    {
                        p_res = PCKActions.Pack(
                            GetFilesList(),
                            file,
                            ver.ToString(),
                            (uint)nud_alignment.Value,
                            cb_embed.Checked,
                            GUIConfig.Instance.PackEncryptionKey,
                            GUIConfig.Instance.PackEncryptIndex,
                            t
                            );
                    }
                    else
                    {
                        p_res = PCKActions.Pack(
                            GetFilesList(),
                            file,
                            ver.ToString(),
                            (uint)nud_alignment.Value,
                            cb_embed.Checked,
                            null,
                            false,
                            t
                            );
                    }
                }, this);

                GUIConfig.Instance.PackVersion = ver;
                GUIConfig.Instance.PackEmbedPCK = cb_embed.Checked;
                GUIConfig.Instance.PackFolderPath = tb_folder_path.Text;
                GUIConfig.Instance.PackPathPrefix = prefix;
                GUIConfig.Instance.PackPCKAlignment = (uint)nud_alignment.Value;
                GUIConfig.Instance.PackOnlyFiltered = cb_packFiltered.Checked;
                GUIConfig.Instance.PackPreviewPaths = cb_previewPaths.Checked;
                GUIConfig.Instance.PackEncryptPCK = cb_enable_encryption.Checked;
                GUIConfig.Instance.PackPatchingEnabled = cb_enable_patching.Checked;
                GUIConfig.Instance.PackPatchingTarget = tb_patch_target.Text;

                GUIConfig.Instance.Save();
            }
        }

        private void dataGridView1_SortCompare(object? sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 1 && e.Column.Index != 2)
            {
                e.Handled = false;
                return;
            }

            if (e.Column.Index == 1)
            {
                e.SortResult = ((long)(dataGridView1.Rows[e.RowIndex1].Cells[1].Tag)).CompareTo((long)(dataGridView1.Rows[e.RowIndex2].Cells[1].Tag));
                e.Handled = true;
            }
            else if (e.Column.Index == 2)
            {
                e.SortResult = ((bool)(dataGridView1.Rows[e.RowIndex1].Cells[2].Tag)).CompareTo((bool)(dataGridView1.Rows[e.RowIndex2].Cells[2].Tag));
                e.Handled = true;
            }
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
                currentSelectedPrefix = tb_prefix.Text;
                RegenerateFilesToPackList();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void tb_prefix_Leave(object sender, EventArgs e)
        {
            if (currentSelectedPrefix != tb_prefix.Text)
            {
                currentSelectedPrefix = tb_prefix.Text;
                RegenerateFilesToPackList();
            }
        }

        private void btn_browse_Click(object? sender, EventArgs e)
        {
            if (Directory.Exists(tb_folder_path.Text))
                fbd_pack_folder.SelectedPath = tb_folder_path.Text;

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

        private void cb_packFiltered_CheckedChanged(object? sender, EventArgs e)
        {
            CalculatePCKSize();
        }

        private void cb_previewPaths_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateTableContent();
        }

        private void cb_enable_patching_CheckedChanged(object? sender, EventArgs e)
        {
            UpdatePatchModeControls();
            RegenerateFilesToPackList();
        }

        private void btn_browse_patch_target_Click(object sender, EventArgs e)
        {
            if (File.Exists(tb_patch_target.Text))
                ofd_patch_target.FileName = tb_patch_target.Text;

            if (ofd_patch_target.ShowDialog(this) == DialogResult.OK)
            {
                tb_patch_target.Text = Path.GetFullPath(ofd_patch_target.FileName);
                RegenerateFilesToPackList();
            }
        }

        private void cb_ver_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdatePCKVersionOfOpenedFiles();
        }

        private void nud_major_ValueChanged(object sender, EventArgs e)
        {
            UpdatePCKVersionOfOpenedFiles();
        }

        private void nud_minor_ValueChanged(object sender, EventArgs e)
        {
            UpdatePCKVersionOfOpenedFiles();
        }

        private void nud_revision_ValueChanged(object sender, EventArgs e)
        {
            UpdatePCKVersionOfOpenedFiles();
        }
    }
}
