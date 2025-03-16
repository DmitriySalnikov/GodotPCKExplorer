using GodotPCKExplorer.GlobalShared;

namespace GodotPCKExplorer.UI
{
    public partial class ExplorerMainForm : Form
    {
        readonly PCKReader pckReader = new();
        readonly string FormBaseTitle = "";
        readonly Font MatchCaseNormal;
        readonly Font MatchCaseStrikeout;
        readonly VersionCheckerGitHub versionCheckerGitHub = new("DmitriySalnikov", "GodotPCKExplorer", GlobalConstants.ProjectName, ShowMessageBoxForVersionCheck);
        readonly List<ToolStripMenuItem> noEncKeyModeMenus = [];

        string onLoadOpenPCKFile = "";
        string? onLoadOpenPCKFileEncKey = null;

        long TotalOpenedSize = 0;

        public ExplorerMainForm(string openFile = "", string? encKey = null)
        {
            onLoadOpenPCKFile = openFile;
            onLoadOpenPCKFileEncKey = encKey;

            InitializeComponent();
            Icon = Properties.Resources.icon;
            FormBaseTitle = Text;

            MatchCaseNormal = tsmi_match_case_filter.Font;
            MatchCaseStrikeout = new Font(tsmi_match_case_filter.Font, FontStyle.Strikeout);

            overwriteExported.Checked = GUIConfig.Instance.ExtractOverwrite;
            checkMD5OnExportToolStripMenuItem.Checked = GUIConfig.Instance.ExtractCheckMD5;

            noEncKeyModeMenus.Add(ifNoEncKeyMode_Cancel);
            noEncKeyModeMenus.Add(ifNoEncKeyMode_Skip);
            noEncKeyModeMenus.Add(ifNoEncKeyMode_AsIs);
            UpdateIfNoEncKeyModeMenus();

            showConsoleToolStripMenuItem.Checked = GUIConfig.Instance.MainFormShowConsole;
            UpdateRecentList();

            UpdateShellReigstrationButton();
            UpdateShowConsole();

            UpdateStatuStrip();
            UpdateMatchCaseFilterButton();

            UpdateListOfPCKContent();

            dataGridView1.SelectionChanged += (o, e) => UpdateStatuStrip();
            extractToolStripMenuItem.Enabled = false;

            copyPathToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) { var text = dataGridView1.Rows[(int)cms_table_row.Tag].Cells[0].Value.ToString(); if (text != null) Clipboard.SetText(text); } };
            copyOffsetToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) { var text = dataGridView1.Rows[(int)cms_table_row.Tag].Cells[1].Value.ToString(); if (text != null) Clipboard.SetText(text); } };
            copySizeToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) { var text = dataGridView1.Rows[(int)cms_table_row.Tag].Cells[2].Value.ToString(); if (text != null) Clipboard.SetText(text); } };
            copySizeInBytesToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) { var text = dataGridView1.Rows[(int)cms_table_row.Tag].Cells[2].Tag.ToString(); if (text != null) Clipboard.SetText(text); } };
            copyMD5ToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) { var text = dataGridView1.Rows[(int)cms_table_row.Tag].Cells[1].Tag.ToString(); if (text != null) Clipboard.SetText(text); } };

            searchText.KeyDown += new KeyEventHandler(searchText_KeyDown);

            versionCheckerGitHub.VersionSkippedByUser += (s, e) =>
            {
                GUIConfig.Instance.SkipVersion = e.SkippedVersion.ToString();
                GUIConfig.Instance.Save();
            };

            CheckVersion(true);
        }

        private void aboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var tmpAbout = new AboutBox1();
            tmpAbout.ShowDialog(this);
        }

        private void checkForUpdates_tstb_Click(object sender, EventArgs e)
        {
            CheckVersion(false, false);
        }

        private void openFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var res = ofd_open_pack.ShowDialog(this);

            if (res == DialogResult.OK)
            {
                OpenFile(ofd_open_pack.FileName);
            }
        }

        internal void CheckVersion(bool isSilent = true, bool useSkipVersion = true)
        {
            if (useSkipVersion)
            {
                // Get SkipVersion for updater
                try
                {
                    if (!string.IsNullOrWhiteSpace(GUIConfig.Instance.SkipVersion))
                        versionCheckerGitHub.SkipVersion = new Version(GUIConfig.Instance.SkipVersion);
                }
                catch (Exception ex)
                {
                    Program.Log(ex);
                }
            }
            else
            {
                versionCheckerGitHub.SkipVersion = default;
            }

            versionCheckerGitHub.CheckForUpdates(isSilent);
        }

        static VersionCheckerGitHub.MSGDialogResult ShowMessageBoxForVersionCheck(VersionCheckerGitHub.MSGType type, Dictionary<string, string> customData, Exception? ex)
        {
            string text = "";
            string caption = "";
            MessageType msg_type = MessageType.None;
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            switch (type)
            {
                case VersionCheckerGitHub.MSGType.InfoUpdateAvailable:
                    text = $"Current version: {customData["current_version"]}\nNew version: {customData["new_version"]}\nWould you like to go to the download page?\n\nSelect \"No\" to skip this version.";
                    caption = $"A new version of {GlobalConstants.ProjectName} is available";
                    msg_type = MessageType.Info;
                    buttons = MessageBoxButtons.YesNoCancel;
                    break;
                case VersionCheckerGitHub.MSGType.InfoUsingLatestVersion:
                    text = $"You are using the latest version: {customData["current_version"]}";
                    msg_type = MessageType.Info;
                    break;
                case VersionCheckerGitHub.MSGType.FailedToRequestInfo:
                    text = $"Failed to request info about the new version.\n{ex?.Message}";
                    caption = $"Error";
                    msg_type = MessageType.Error;
                    break;
                case VersionCheckerGitHub.MSGType.FailedToGetInfo:
                    text = $"Failed to get info about the new version.\nCode: {customData["respose"]}";
                    caption = $"Error";
                    msg_type = MessageType.Error;
                    break;
                case VersionCheckerGitHub.MSGType.FailedToProcessData:
                    text = $"Failed to check for update.\n{ex?.Message}";
                    caption = $"Error";
                    msg_type = MessageType.Error;
                    break;
                case VersionCheckerGitHub.MSGType.FailedAlreadyRequesting:
                    text = $"Failed to request data. Already requesting.";
                    caption = $"Error";
                    msg_type = MessageType.Error;
                    break;
            }

            return (VersionCheckerGitHub.MSGDialogResult)Program.ShowMessage(text, caption, msg_type, buttons, DialogResult.Cancel);
        }

        static void UpdateShowConsole()
        {
            if (GUIConfig.Instance.MainFormShowConsole)
            {
                Program.ShowConsole();
            }
            else
            {
                Program.HideConsole();
            }
        }

        void UpdateShellReigstrationButton()
        {
            if (ShellIntegration.IsRegistered())
            {
                registerProgram_ToolStripMenuItem.Text = "Unregister the program in Explorer";
                registerProgram_ToolStripMenuItem.Checked = true;
            }
            else
            {
                registerProgram_ToolStripMenuItem.Text = "Register the program to open PCK in Explorer";
                registerProgram_ToolStripMenuItem.Checked = false;
            }
        }

        string GetEncryptionStatusString()
        {
            var enc_text = "";
            if (pckReader.IsEncrypted)
            {
                if (pckReader.IsEncryptedIndex != pckReader.IsEncryptedFiles)
                {
                    if (pckReader.IsEncryptedIndex)
                        enc_text = "Encrypted Index";
                    else
                        enc_text = "Encrypted Files";
                }
                else
                {
                    enc_text = "Encrypted";
                }
            }
            if (enc_text != "")
                enc_text = " " + enc_text;
            return enc_text;
        }

        PCKReaderEncryptionKeyResult GetEncryptionKey(string? path = null)
        {
            PCKReaderEncryptionKeyResult res = new();
            RecentFiles? item = GUIConfig.Instance.MainFormRecentOpenedFiles.FirstOrDefault((i) => i.Path == path);

            using var d = new OpenWithPCKEncryption(item?.EncryptionKey ?? "");
            var dlg_res = d.ShowDialog(this);

            if (dlg_res == DialogResult.Cancel)
            {
                res.IsCancelled = true;
            }

            res.Key = d.EncryptionKey;
            return res;
        }

        void UpdateIfNoEncKeyModeMenus()
        {
            for (int i = 0; i < noEncKeyModeMenus.Count; i++)
            {
                var menu = noEncKeyModeMenus[i];
                if (i == (int)GUIConfig.Instance.ExtractIfNoEncryptionKeyMode)
                {
                    menu.Checked = true;
                }
                else
                {
                    menu.Checked = false;
                }
            }
        }

        void UpdateRecentList(string? path = null, bool remove = false, bool isEncrypted = false, bool updateKey = false, string? encryptionKey = null)
        {
            if (path != null)
            {
                if (!remove)
                {
                    var list = GUIConfig.Instance.MainFormRecentOpenedFiles;
                    var item = list.FirstOrDefault((i) => i.Path == path);

                    // Move to top if already exists
                    if (item != null)
                    {
                        item.IsEncrypted = isEncrypted;

                        // Override only if not null
                        if (updateKey)
                            item.EncryptionKey = encryptionKey ?? "";

                        list.Remove(item);
                        list.Insert(0, item);
                    }
                    else
                    {
                        list.Insert(0, new RecentFiles(path, isEncrypted, encryptionKey ?? ""));
                        while (list.Count > 16)
                            list.RemoveAt(list.Count - 1);
                    }
                }
                else
                {
                    var list = GUIConfig.Instance.MainFormRecentOpenedFiles;

                    var item = list.FirstOrDefault((i) => i.Path == path);
                    if (item != null)
                        list.Remove(item);
                }

                GUIConfig.Instance.Save();
            }

            recentToolStripMenuItem.DropDownItems.Clear();

            if (GUIConfig.Instance.MainFormRecentOpenedFiles.Count > 0)
            {
                recentToolStripMenuItem.Enabled = true;
                foreach (var f in GUIConfig.Instance.MainFormRecentOpenedFiles)
                {
                    recentToolStripMenuItem.DropDownItems.Add(
                        new ToolStripButton(f.IsEncrypted ? f.Path + " 🔑" : f.Path, null, (s, e) => OpenFile(f.Path, f.EncryptionKey)) { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });
                }
            }
            else
            {
                recentToolStripMenuItem.Enabled = false;
            }
        }

        public void OpenFile(string path, string? encKey = null)
        {
            CloseFile();
            bool enc_dialog_canceled = false;

            path = Path.GetFullPath(path);
            string? encryption_key = null;

            PCKReaderEncryptionKeyResult get_enc_key()
            {
                if (!string.IsNullOrWhiteSpace(encKey))
                {
                    encryption_key = encKey;
                    return new PCKReaderEncryptionKeyResult() { Key = encKey };
                }
                else
                {
                    var res = GetEncryptionKey(path);
                    encryption_key = res.Key;
                    return res;
                }
            }

            if (pckReader.OpenFile(path, getEncryptionKey: () => InvokeRequired ? Invoke(get_enc_key) : get_enc_key()))
            {
                var enc_text = GetEncryptionStatusString();
                Text = $"\"{Utils.GetShortPath(pckReader.PackPath, 50)}\" Pack version: {pckReader.PCK_VersionPack}. Godot Version: {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}{enc_text}";

                TotalOpenedSize = 0;

                foreach (var f in pckReader.Files.Values)
                {
                    TotalOpenedSize += f.Size;
                }

                searchText.Text = "";
                extractToolStripMenuItem.Enabled = true;

                GUIConfig.Instance.Save();
                UpdateRecentList(path, false, pckReader.IsEncrypted, encryption_key != null, encryption_key);
                UpdateStatuStrip();
                UpdateListOfPCKContent();
            }
            else
            {
                if (enc_dialog_canceled)
                    return;

                GUIConfig.Instance.Save();
                UpdateRecentList(path, true);
            }
        }

        public void CloseFile()
        {
            extractToolStripMenuItem.Enabled = false;

            pckReader.Close();

            Text = FormBaseTitle;
            TotalOpenedSize = 0;

            UpdateListOfPCKContent();
            UpdateStatuStrip();
        }

        public void UpdateListOfPCKContent()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns["encrypted"].Visible = pckReader.IsEncryptedFiles;
            dataGridView1.Columns["removal"].Visible = pckReader.IsRemovalFiles;

            if (pckReader.IsOpened)
            {
                List<DataGridViewRow> tmp_rows = [];

                foreach (var f in pckReader.Files)
                {
                    if (string.IsNullOrWhiteSpace(searchText.Text) ||
                        (!string.IsNullOrWhiteSpace(searchText.Text) && Utils.IsMatchWildCard(f.Key, searchText.Text, GUIConfig.Instance.MainFormMatchCaseFilter)))
                    {
                        var tmpRow = new DataGridViewRow();
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.FilePath });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.Offset, Tag = PCKUtils.ByteArrayToHexString(f.Value.MD5, "") });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.IsEncrypted ? "*" : string.Empty, Tag = f.Value.IsEncrypted });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.IsRemoval ? "*" : string.Empty, Tag = f.Value.IsRemoval });

                        tmp_rows.Add(tmpRow);
                    }
                }

                dataGridView1.Rows.AddRange([.. tmp_rows]);
            }

            extractFilteredToolStripMenuItem.Enabled = !string.IsNullOrWhiteSpace(searchText.Text);
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

                var enc_text = GetEncryptionStatusString();
                tssl_version_and_stats.Text = $"Version: {pckReader.PCK_VersionPack} {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}" +
                    $" Files count: {pckReader.Files.Count}" +
                    $" Total size: {Utils.SizeSuffix(TotalOpenedSize)}" +
                    enc_text;

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

        void UpdateMatchCaseFilterButton()
        {
            if (GUIConfig.Instance.MainFormMatchCaseFilter)
                tsmi_match_case_filter.Font = MatchCaseNormal;
            else
                tsmi_match_case_filter.Font = MatchCaseStrikeout;
        }

        private void ExplorerMainForm_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(onLoadOpenPCKFile))
            {
                OpenFile(onLoadOpenPCKFile, onLoadOpenPCKFileEncKey);
                onLoadOpenPCKFile = "";
                onLoadOpenPCKFileEncKey = null;
            }
        }

        private void exitToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void extractFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var res = fbd_extract_folder.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                var selectedRows = new List<string>();
                foreach (DataGridViewRow i in dataGridView1.SelectedRows)
                    selectedRows.Add((string)i.Cells[0].Value);

                ExtractFilesFromPCK(selectedRows);
            }
        }

        private void extractAllToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var res = fbd_extract_folder.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                ExtractFilesFromPCK(pckReader.Files.Select((f) => f.Key));
            }
        }

        private void extractFilteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fbd_extract_folder.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                var filteredRows = new List<string>();
                foreach (DataGridViewRow i in dataGridView1.Rows)
                    filteredRows.Add((string)i.Cells[0].Value);

                ExtractFilesFromPCK(filteredRows);
            }
        }

        void ExtractFilesFromPCK(IEnumerable<string> files)
        {
            var path = pckReader.PackPath;
            List<string> extractedFiles = [];
            List<string> failedFiles = [];

            bool extract_result = false;
            Program.DoTaskWithProgressBar((t) =>
            {
                extract_result = pckReader.ExtractFiles(
                    names: files,
                    extractedFiles: out extractedFiles,
                    failedFiles: out failedFiles,
                    folder: fbd_extract_folder.SelectedPath,
                    overwriteExisting: overwriteExported.Checked,
                    checkMD5: GUIConfig.Instance.ExtractCheckMD5,
                    getEncryptionKey: () => InvokeRequired ? Invoke(() => GetEncryptionKey(path)) : GetEncryptionKey(path),
                    noKeyMode: GUIConfig.Instance.ExtractIfNoEncryptionKeyMode,
                    cancellationToken: t);
            }, this);

            if (failedFiles.Count > 0)
            {
                string failed = "Failed";
                if (extract_result)
                {
                    switch (GUIConfig.Instance.ExtractIfNoEncryptionKeyMode)
                    {
                        case PCKExtractNoEncryptionKeyMode.Skip:
                            failed = "Skipped";
                            break;
                        case PCKExtractNoEncryptionKeyMode.AsIs:
                            failed = "Extracted As Is";
                            break;
                    }
                }
                Program.ShowMessage($"Not all files have been extracted.\nExtracted: {extractedFiles.Count}\n{failed}: {failedFiles.Count}", "Extraction result", MessageType.Info);
            }

            UpdateRecentList(path, false, pckReader.IsEncrypted, true, PCKUtils.ByteArrayToHexString(pckReader.ReceivedEncryptionKey));
        }

        private void packFolderToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var dlg = new CreatePCKFile();
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.ShowDialog(this);
        }

        private void closeFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CloseFile();
        }

        private void dataGridView1_SortCompare(object? sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 2)
            {
                e.Handled = false;
                return;
            }

            e.SortResult = ((long)(dataGridView1.Rows[e.RowIndex1].Cells[2].Tag)).CompareTo((long)(dataGridView1.Rows[e.RowIndex2].Cells[2].Tag));
            e.Handled = true;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            CloseFile();
        }

        private void registerProgramToOpenPCKInExplorerToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ShellIntegration.IsRegistered())
            {
                ShellIntegration.Unregister();
            }
            else
            {
                ShellIntegration.Register();
            }
            UpdateShellReigstrationButton();
        }

        private void showConsoleToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            GUIConfig.Instance.MainFormShowConsole = showConsoleToolStripMenuItem.Checked;
            GUIConfig.Instance.Save();
            UpdateShowConsole();
        }

        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length == 1)
                {
                    using var pck = new PCKReader();

                    if (File.Exists(files[0]))
                    {
                        var old_enabled = Program.IsMessageBoxesEnabled();
                        Program.DisableMessageBoxes();

                        try
                        {
                            if (pck.OpenFile(files[0], false, logFileNamesProgress: false))
                            {
                                e.Effect = DragDropEffects.Copy;
                                return;
                            }

                            if (pck.IsEncrypted)
                            {
                                e.Effect = DragDropEffects.Link;
                                return;
                            }
                        }
                        finally
                        {
                            if (old_enabled)
                                Program.EnableMessageBoxes();
                        }
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        static readonly DragDropEffects[] allowedEffects = [DragDropEffects.Copy, DragDropEffects.Link];
        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop) && allowedEffects.Contains(e.Effect))
            {
                string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length == 1)
                {
                    OpenFile(files[0]);
                }
            }
        }

        private void overwriteExported_Click(object? sender, EventArgs e)
        {
            GUIConfig.Instance.ExtractOverwrite = overwriteExported.Checked;
            GUIConfig.Instance.Save();
        }

        private void checkMD5OnExportToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            GUIConfig.Instance.ExtractCheckMD5 = checkMD5OnExportToolStripMenuItem.Checked;
            GUIConfig.Instance.Save();
        }

        private void ifNoEncKeyMode_Shared_Click(object sender, EventArgs e)
        {
            var idx = noEncKeyModeMenus.IndexOf((ToolStripMenuItem)sender);
            if (idx != -1)
            {
                GUIConfig.Instance.ExtractIfNoEncryptionKeyMode = (PCKExtractNoEncryptionKeyMode)idx;
                UpdateIfNoEncKeyModeMenus();
            }
            else
            {
                throw new NotImplementedException("Invalid mode index!");
            }
        }

        private void ripPackFromFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ofd_rip_select_pck.ShowDialog(this) == DialogResult.OK)
            {
                using (var pck = new PCKReader())
                {
                    if (!pck.OpenFile(ofd_rip_select_pck.FileName, logFileNamesProgress: false))
                    {
                        return;
                    }
                    else if (!pck.PCK_Embedded)
                    {
                        Program.ShowMessage("The selected file must contain an embedded '.pck' file", "Error", MessageType.Error);
                        return;
                    }
                }

                if (sfd_rip_save_pack.ShowDialog(this) == DialogResult.OK)
                {
                    Program.DoTaskWithProgressBar((t) => PCKActions.Rip(ofd_rip_select_pck.FileName, sfd_rip_save_pack.FileName, cancellationToken: t),
                        this);
                }
            }
        }

        private void splitExeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ofd_split_exe_open.ShowDialog(this) == DialogResult.OK)
            {
                using (var pck = new PCKReader())
                {
                    if (!pck.OpenFile(ofd_split_exe_open.FileName, logFileNamesProgress: false))
                    {
                        return;
                    }
                    else if (!pck.PCK_Embedded)
                    {
                        Program.ShowMessage("The selected file must contain an embedded '.pck' file", "Error", MessageType.Error);
                        return;
                    }
                }

                sfd_split_new_file.Filter = $"Original file extension|*{Path.GetExtension(ofd_split_exe_open.FileName)}|All Files|*.*";
                if (sfd_split_new_file.ShowDialog(this) == DialogResult.OK)
                {
                    Program.DoTaskWithProgressBar((t) => PCKActions.Split(ofd_split_exe_open.FileName, sfd_split_new_file.FileName, cancellationToken: t),
                        this);
                }
            }
        }

        private void mergePackIntoFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ofd_merge_pck.ShowDialog(this) == DialogResult.OK)
            {
                using (var pck = new PCKReader())
                {
                    if (!pck.OpenFile(ofd_merge_pck.FileName, logFileNamesProgress: false))
                    {
                        return;
                    }
                }

                if (ofd_merge_target.ShowDialog(this) == DialogResult.OK)
                {
                    Program.DoTaskWithProgressBar((t) => PCKActions.Merge(ofd_merge_pck.FileName, ofd_merge_target.FileName, cancellationToken: t), this);
                }
            }
        }

        private void removePackFromFileToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ofd_remove_pck_from_exe.ShowDialog(this) == DialogResult.OK)
            {
                Program.DoTaskWithProgressBar((t) => PCKActions.Remove(ofd_remove_pck_from_exe.FileName, cancellationToken: t), this);
            }
        }

        private void splitExeInPlaceToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ofd_split_in_place.ShowDialog(this) == DialogResult.OK)
            {
                Program.DoTaskWithProgressBar((t) => PCKActions.Split(ofd_split_in_place.FileName, null, false, cancellationToken: t), this);
            }
        }

        private void searchText_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                UpdateListOfPCKContent();
            }
        }

        private void toolStripMenuItem_clearFilter_Click(object sender, EventArgs e)
        {
            searchText.Text = string.Empty;
            UpdateListOfPCKContent();
        }

        private void toolStripMenuItem_filter_Click(object? sender, EventArgs e)
        {
            UpdateListOfPCKContent();
        }

        private void dataGridView1_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                if (dataGridView1.SelectedRows.Count <= 1)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                }
                cms_table_row.Show(MousePosition);
                cms_table_row.Tag = e.RowIndex;
            }
        }

        private void changePackVersionToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (ofd_change_version.ShowDialog(this) == DialogResult.OK)
            {
                var cv = new ChangePCKVersion();
                cv.ShowAndOpenFile(ofd_change_version.FileName);
                cv.Dispose();
            }
        }

        private void tsmi_match_case_filter_Click(object? sender, EventArgs e)
        {
            GUIConfig.Instance.MainFormMatchCaseFilter = !GUIConfig.Instance.MainFormMatchCaseFilter;
            GUIConfig.Instance.Save();
            UpdateMatchCaseFilterButton();
            UpdateListOfPCKContent();
        }
    }
}
