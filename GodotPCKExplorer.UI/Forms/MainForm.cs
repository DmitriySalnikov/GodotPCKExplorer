﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GodotPCKExplorer.UI
{
    // TODO some actions with encoded PCK is not possible in UI
    public partial class MainForm : Form
    {
        PCKReader pckReader = new PCKReader();
        string FormBaseTitle = "";
        long TotalOpenedSize = 0;
        Font MatchCaseNormal = null;
        Font MatchCaseStrikeout = null;
        VersionCheckerGitHub versionCheckerGitHub = new VersionCheckerGitHub("DmitriySalnikov", "GodotPCKExplorer", Program.AppName, ShowMessageBoxForVersionCheck);

        public MainForm()
        {
            GUIConfig.Load();

            InitializeComponent();
            Icon = Properties.Resources.icon;
            FormBaseTitle = Text;

            MatchCaseNormal = tsmi_match_case_filter.Font;
            MatchCaseStrikeout = new Font(tsmi_match_case_filter.Font, FontStyle.Strikeout);

            overwriteExported.Checked = GUIConfig.Instance.OverwriteExtracted;
            checkMD5OnExportToolStripMenuItem.Checked = GUIConfig.Instance.CheckMD5Extracted;

            showConsoleToolStripMenuItem.Checked = GUIConfig.Instance.ShowConsole;

            UpdateShowConsole();
            UpdateStatuStrip();
            UpdateRecentList();
            UpdateListOfPCKContent();
            UpdateMatchCaseFilterButton();

            dataGridView1.SelectionChanged += (o, e) => UpdateStatuStrip();
            extractToolStripMenuItem.Enabled = false;

            copyPathToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) Clipboard.SetText(dataGridView1.Rows[(int)cms_table_row.Tag].Cells[0].Value as string); };
            copyOffsetToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) Clipboard.SetText(dataGridView1.Rows[(int)cms_table_row.Tag].Cells[1].Value.ToString()); };
            copySizeToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) Clipboard.SetText(dataGridView1.Rows[(int)cms_table_row.Tag].Cells[2].Value.ToString()); };
            copySizeInBytesToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) Clipboard.SetText(dataGridView1.Rows[(int)cms_table_row.Tag].Cells[2].Tag.ToString()); };
            copyMD5ToolStripMenuItem.Click += (o, e) => { if (cms_table_row.Tag != null) Clipboard.SetText(dataGridView1.Rows[(int)cms_table_row.Tag].Cells[1].Tag.ToString()); };

            if (Utils.IsRunningOnMono())
            {
                // Recreate filter text for mono support
                menuStrip1.Items.Remove(searchText);
                searchText.Dispose();
                searchText = null;

                searchText = new ToolStripTextBoxWithPlaceholder();

                searchText.Alignment = ToolStripItemAlignment.Right;
                searchText.AutoSize = false;
                searchText.Font = new Font("Segoe UI", 9F);
                searchText.Name = "searchTextLinux";
                searchText.Size = new Size(200, 23);
                searchText.ToolTipText = "Filter text (? and * allowed)\n" + new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetString("searchText.ToolTipText");
                menuStrip1.Items.Insert(menuStrip1.Items.IndexOf(tsmi_match_case_filter) + 1, searchText);

                searchText.Text = "Filter text (? and * allowed)";
                // HACK to get some size of the text field.
                // Without this, the Textbox's text field will have a width of 1 pixel.
                var t = new Timer(components);
                t.Interval = 1;
                t.Tick += (s, e) =>
                {
                    t.Dispose();
                    t = null;
                    searchText.Text = "";
                };
                t.Start();

                integrationToolStripMenuItem.Visible = false;
            }

            searchText.KeyDown += new KeyEventHandler(searchText_KeyDown);
            CheckVersion(true);
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
                OpenFile(ofd_open_pack.FileName);
            }
        }

        internal void CheckVersion(bool isSilent = true)
        {
            versionCheckerGitHub.VersionSkippedByUser += (s, e) =>
            {
                GUIConfig.Instance.SkipVersion = e.SkippedVersion.ToString();
                GUIConfig.Instance.Save();
            };

            // Get SkipVersion for updater
            try
            {
                versionCheckerGitHub.SkipVersion = new Version(GUIConfig.Instance.SkipVersion);
            }
            catch (Exception ex)
            {
                Program.Log("The SkipVersion string for the updater could not be parsed.");
                Program.Log(ex.Message);
            }

            versionCheckerGitHub.CheckForUpdates(isSilent);
        }

        static VersionCheckerGitHub.MSGDialogResult ShowMessageBoxForVersionCheck(VersionCheckerGitHub.MSGType type, Dictionary<string, string> customData, Exception ex)
        {
            string text = "";
            string caption = "";
            MessageType msg_type = MessageType.None;
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            switch (type)
            {
                case VersionCheckerGitHub.MSGType.InfoUpdateAvailable:
                    text = $"Current version: {customData["current_version"]}\nNew version: {customData["new_version"]}\nWould you like to go to the download page?\n\nSelect \"No\" to skip this version.";
                    caption = $"A new version of {Program.AppName} is available";
                    msg_type = MessageType.Info;
                    buttons = MessageBoxButtons.YesNoCancel;
                    break;
                case VersionCheckerGitHub.MSGType.InfoUsingLatestVersion:
                    text = $"You are using the latest version: {customData["current_version"]}";
                    msg_type = MessageType.Info;
                    break;
                case VersionCheckerGitHub.MSGType.FailedToRequestInfo:
                    text = $"Failed to request info about the new version.\n{ex.Message}";
                    msg_type = MessageType.Error;
                    break;
                case VersionCheckerGitHub.MSGType.FailedToGetInfo:
                    text = $"Failed to get info about the new version.\n{ex.Message}";
                    msg_type = MessageType.Error;
                    break;
                case VersionCheckerGitHub.MSGType.FailedToProcessData:
                    text = $"Failed to check for update.\n{ex.Message}";
                    msg_type = MessageType.Error;
                    break;
            }

            return (VersionCheckerGitHub.MSGDialogResult)Program.ShowMessage(text, caption, msg_type, buttons, DialogResult.Cancel);
        }

        void UpdateShowConsole()
        {
            if (GUIConfig.Instance.ShowConsole)
            {
                Program.ShowConsole();
            }
            else
            {
                Program.HideConsole();
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
                        new ToolStripButton(f.Path, null, (s, e) => OpenFile(f.Path, f.EncryptionKey)));
                }

            }
            else
            {
                recentToolStripMenuItem.Enabled = false;
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

        public void OpenFile(string path, string encKey = null)
        {
            CloseFile();
            bool enc_dialog_canceled = false;

            Func<string> get_enc_key = () =>
            {
                if (!string.IsNullOrWhiteSpace(encKey))
                {
                    return encKey;
                }
                else
                {
                    var item = GUIConfig.Instance.RecentOpenedFiles.FirstOrDefault((i) => i.Path == path);

                    using (var d = new OpenWithPCKEncryption(item?.EncryptionKey ?? ""))
                    {
                        var res = d.ShowDialog();

                        if (res == DialogResult.Cancel)
                        {
                            enc_dialog_canceled = true;
                        }

                        if (item != null && !string.IsNullOrWhiteSpace(d.EncryptionKey))
                        {
                            item.EncryptionKey = d.EncryptionKey;
                            GUIConfig.Instance.Save();
                        }
                        return d.EncryptionKey;
                    }
                }
            };

            path = Path.GetFullPath(path);
            if (pckReader.OpenFile(path, get_encryption_key: get_enc_key))
            {
                var enc_text = GetEncryptionStatusString();
                Text = $"\"{Utils.GetShortPath(pckReader.PackPath, 50)}\" Pack version: {pckReader.PCK_VersionPack}. Godot Version: {pckReader.PCK_VersionMajor}.{pckReader.PCK_VersionMinor}.{pckReader.PCK_VersionRevision}{enc_text}";

                // update recent files
                var list = GUIConfig.Instance.RecentOpenedFiles;
                var item = list.FirstOrDefault((i) => i.Path == path);

                // Move to top
                if (item != null)
                {
                    var str = PCKUtils.ByteArrayToHexString(pckReader.EncryptionKey);
                    if (str != "")
                        item.EncryptionKey = str;
                    list.Remove(item);
                    list.Insert(0, item);
                }
                else
                {
                    list.Insert(0, new RecentFiles(path, PCKUtils.ByteArrayToHexString(pckReader.EncryptionKey)));
                    while (list.Count > 16)
                        list.RemoveAt(list.Count - 1);
                }

                TotalOpenedSize = 0;

                foreach (var f in pckReader.Files.Values)
                {
                    TotalOpenedSize += f.Size;
                }

                searchText.Text = "";
                extractToolStripMenuItem.Enabled = true;

                GUIConfig.Instance.Save();
                UpdateRecentList();
                UpdateStatuStrip();
                UpdateListOfPCKContent();
            }
            else
            {
                if (enc_dialog_canceled)
                    return;

                // update recent files
                var list = GUIConfig.Instance.RecentOpenedFiles;

                var item = list.FirstOrDefault((i) => i.Path == path);
                if (item != null)
                    list.Remove(item);
                GUIConfig.Instance.Save();
                UpdateRecentList();
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
            if (pckReader.IsOpened)
            {
                foreach (var f in pckReader.Files)
                {
                    if (string.IsNullOrEmpty(searchText.Text) ||
                        (!string.IsNullOrEmpty(searchText.Text) && Utils.IsMatchWildCard(f.Key, searchText.Text, GUIConfig.Instance.MatchCaseFilterMainForm)))
                    {
                        var tmpRow = new DataGridViewRow();
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.FilePath });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = f.Value.Offset, Tag = PCKUtils.ByteArrayToHexString(f.Value.MD5, " ") });
                        tmpRow.Cells.Add(new DataGridViewTextBoxCell() { Value = Utils.SizeSuffix(f.Value.Size), Tag = f.Value.Size });

                        dataGridView1.Rows.Add(tmpRow);
                    }
                }
            }
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
            if (GUIConfig.Instance.MatchCaseFilterMainForm)
                tsmi_match_case_filter.Font = MatchCaseNormal;
            else
                tsmi_match_case_filter.Font = MatchCaseStrikeout;
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

                Program.DoTaskWithProgressBar((t) => pckReader.ExtractFiles(rows, fbd_extract_folder.SelectedPath, overwriteExported.Checked, GUIConfig.Instance.CheckMD5Extracted, cancellationToken: t));
            }
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fbd_extract_folder.ShowDialog();
            if (res == DialogResult.OK)
            {
                Program.DoTaskWithProgressBar((t) => pckReader.ExtractFiles(pckReader.Files.Select((f) => f.Key), fbd_extract_folder.SelectedPath, overwriteExported.Checked, GUIConfig.Instance.CheckMD5Extracted, cancellationToken: t));
            }
        }

        private void packFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new CreatePCKFile();
            dlg.ShowDialog();
            dlg.Dispose();
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
            CloseFile();
        }

        private void registerProgramToOpenPCKInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellIntegration.Register();
        }

        private void unregisterProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShellIntegration.Unregister();
        }

        private void showConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUIConfig.Instance.ShowConsole = showConsoleToolStripMenuItem.Checked;
            GUIConfig.Instance.Save();
            UpdateShowConsole();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    var pck = new PCKReader();

                    if (File.Exists(files[0]))
                    {
                        var old_enabled = Program.IsMessageBoxesEnabled();
                        Program.DisableMessageBoxes();

                        try
                        {
                            if (pck.OpenFile(files[0], false, log_names_progress: false))
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

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && new DragDropEffects[] { DragDropEffects.Copy, DragDropEffects.Link }.Contains(e.Effect))
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
            GUIConfig.Instance.OverwriteExtracted = overwriteExported.Checked;
            GUIConfig.Instance.Save();
        }

        private void checkMD5OnExportToolStripMenuItem_Click(object sender, EventArgs e)
        {

            GUIConfig.Instance.CheckMD5Extracted = checkMD5OnExportToolStripMenuItem.Checked;
            GUIConfig.Instance.Save();
        }

        private void ripPackFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_rip_select_pck.ShowDialog() == DialogResult.OK)
            {
                using (var pck = new PCKReader())
                {
                    if (!pck.OpenFile(ofd_rip_select_pck.FileName, log_names_progress: false))
                    {
                        return;
                    }
                    else if (!pck.PCK_Embedded)
                    {
                        Program.ShowMessage("The selected file must contain an embedded '.pck' file", "Error", MessageType.Error);
                        return;
                    }
                }

                if (sfd_rip_save_pack.ShowDialog() == DialogResult.OK)
                {
                    Program.DoTaskWithProgressBar((t) => PCKActions.RipPCKRun(ofd_rip_select_pck.FileName, sfd_rip_save_pack.FileName, cancellationToken: t));
                }
            }
        }

        private void splitExeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_split_exe_open.ShowDialog() == DialogResult.OK)
            {
                using (var pck = new PCKReader())
                    if (!pck.OpenFile(ofd_split_exe_open.FileName, log_names_progress: false))
                    {
                        return;
                    }
                    else if (!pck.PCK_Embedded)
                    {
                        Program.ShowMessage("The selected file must contain an embedded '.pck' file", "Error", MessageType.Error);
                        return;
                    }

                sfd_split_new_file.Filter = $"Original file extension|*{Path.GetExtension(ofd_split_exe_open.FileName)}|All Files|*.*";
                if (sfd_split_new_file.ShowDialog() == DialogResult.OK)
                {
                    Program.DoTaskWithProgressBar((t) => PCKActions.SplitPCKRun(ofd_split_exe_open.FileName, sfd_split_new_file.FileName, cancellationToken: t));
                }
            }
        }

        private void mergePackIntoFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_merge_pck.ShowDialog() == DialogResult.OK)
            {
                using (var pck = new PCKReader())
                    if (!pck.OpenFile(ofd_merge_pck.FileName, log_names_progress: false))
                    {
                        return;
                    }

                if (ofd_merge_target.ShowDialog() == DialogResult.OK)
                {
                    Program.DoTaskWithProgressBar((t) => PCKActions.MergePCKRun(ofd_merge_pck.FileName, ofd_merge_target.FileName, cancellationToken: t));
                }
            }
        }

        private void removePackFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_remove_pck_from_exe.ShowDialog() == DialogResult.OK)
            {
                Program.DoTaskWithProgressBar((t) => PCKActions.RipPCKRun(ofd_remove_pck_from_exe.FileName, cancellationToken: t));
            }
        }

        private void splitExeInPlaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_split_in_place.ShowDialog() == DialogResult.OK)
            {
                Program.DoTaskWithProgressBar((t) => PCKActions.SplitPCKRun(ofd_split_in_place.FileName, null, false, cancellationToken: t));
            }
        }

        private void searchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                UpdateListOfPCKContent();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateListOfPCKContent();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
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

        private void changePackVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd_change_version.ShowDialog() == DialogResult.OK)
            {
                var cv = new ChangePCKVersion();
                cv.ShowAndOpenFile(ofd_change_version.FileName);
                cv.Dispose();
            }
        }

        private void tsmi_match_case_filter_Click(object sender, EventArgs e)
        {
            GUIConfig.Instance.MatchCaseFilterMainForm = !GUIConfig.Instance.MatchCaseFilterMainForm;
            GUIConfig.Instance.Save();
            UpdateMatchCaseFilterButton();
            UpdateListOfPCKContent();
        }
    }
}