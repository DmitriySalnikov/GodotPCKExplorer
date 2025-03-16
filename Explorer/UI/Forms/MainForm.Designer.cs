namespace GodotPCKExplorer.UI
{
    partial class ExplorerMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerMainForm));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            closeFileToolStripMenuItem = new ToolStripMenuItem();
            recentToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            packFolderToolStripMenuItem = new ToolStripMenuItem();
            mergePackIntoFileToolStripMenuItem = new ToolStripMenuItem();
            changePackVersionToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            ripPackFromFileToolStripMenuItem = new ToolStripMenuItem();
            removePackFromFileToolStripMenuItem = new ToolStripMenuItem();
            splitExeToolStripMenuItem = new ToolStripMenuItem();
            splitExeInPlaceToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            extractToolStripMenuItem = new ToolStripMenuItem();
            extractFileToolStripMenuItem = new ToolStripMenuItem();
            extractFilteredToolStripMenuItem = new ToolStripMenuItem();
            extractAllToolStripMenuItem = new ToolStripMenuItem();
            overwriteExported = new ToolStripMenuItem();
            checkMD5OnExportToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            ifNoEncKeyMode_Cancel = new ToolStripMenuItem();
            ifNoEncKeyMode_Skip = new ToolStripMenuItem();
            ifNoEncKeyMode_AsIs = new ToolStripMenuItem();
            integrationToolStripMenuItem = new ToolStripMenuItem();
            registerProgram_ToolStripMenuItem = new ToolStripMenuItem();
            showConsoleToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem_filterButton = new ToolStripMenuItem();
            toolStripMenuItem_clearFilter = new ToolStripMenuItem();
            searchText = new ToolStripTextBoxWithPlaceholder();
            tsmi_match_case_filter = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            checkUpdates_toolStripMenuItem1 = new ToolStripMenuItem();
            about_toolStripMenuItem1 = new ToolStripMenuItem();
            ofd_open_pack = new OpenFileDialog();
            fbd_extract_folder = new FolderBrowserDialog();
            dataGridView1 = new DataGridView();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            tssl_selected_size = new ToolStripStatusLabel();
            tssl_version_and_stats = new ToolStripStatusLabel();
            ofd_remove_pck_from_exe = new OpenFileDialog();
            ofd_split_in_place = new OpenFileDialog();
            toolTip1 = new ToolTip(components);
            cms_table_row = new ContextMenuStrip(components);
            copyPathToolStripMenuItem = new ToolStripMenuItem();
            copyOffsetToolStripMenuItem = new ToolStripMenuItem();
            copySizeToolStripMenuItem = new ToolStripMenuItem();
            copySizeInBytesToolStripMenuItem = new ToolStripMenuItem();
            copyMD5ToolStripMenuItem = new ToolStripMenuItem();
            ofd_rip_select_pck = new OpenFileDialog();
            sfd_rip_save_pack = new SaveFileDialog();
            sfd_split_new_file = new SaveFileDialog();
            ofd_split_exe_open = new OpenFileDialog();
            ofd_merge_pck = new OpenFileDialog();
            ofd_merge_target = new OpenFileDialog();
            ofd_change_version = new OpenFileDialog();
            path = new DataGridViewTextBoxColumn();
            offset = new DataGridViewTextBoxColumn();
            size = new DataGridViewTextBoxColumn();
            encrypted = new DataGridViewTextBoxColumn();
            removal = new DataGridViewTextBoxColumn();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            statusStrip1.SuspendLayout();
            cms_table_row.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, extractToolStripMenuItem, integrationToolStripMenuItem, toolStripMenuItem_filterButton, toolStripMenuItem_clearFilter, searchText, tsmi_match_case_filter, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.ShowItemToolTips = true;
            menuStrip1.Size = new Size(800, 27);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, closeFileToolStripMenuItem, recentToolStripMenuItem, toolStripSeparator1, packFolderToolStripMenuItem, mergePackIntoFileToolStripMenuItem, changePackVersionToolStripMenuItem, toolStripSeparator3, ripPackFromFileToolStripMenuItem, removePackFromFileToolStripMenuItem, splitExeToolStripMenuItem, splitExeInPlaceToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 23);
            fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(195, 22);
            openFileToolStripMenuItem.Text = "Open File";
            openFileToolStripMenuItem.Click += openFileToolStripMenuItem_Click;
            // 
            // closeFileToolStripMenuItem
            // 
            closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            closeFileToolStripMenuItem.Size = new Size(195, 22);
            closeFileToolStripMenuItem.Text = "Close File";
            closeFileToolStripMenuItem.Click += closeFileToolStripMenuItem_Click;
            // 
            // recentToolStripMenuItem
            // 
            recentToolStripMenuItem.Enabled = false;
            recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            recentToolStripMenuItem.Size = new Size(195, 22);
            recentToolStripMenuItem.Text = "Recent Files";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(192, 6);
            // 
            // packFolderToolStripMenuItem
            // 
            packFolderToolStripMenuItem.Name = "packFolderToolStripMenuItem";
            packFolderToolStripMenuItem.Size = new Size(195, 22);
            packFolderToolStripMenuItem.Text = "Pack or Embed Folder";
            packFolderToolStripMenuItem.ToolTipText = resources.GetString("packFolderToolStripMenuItem.ToolTipText");
            packFolderToolStripMenuItem.Click += packFolderToolStripMenuItem_Click;
            // 
            // mergePackIntoFileToolStripMenuItem
            // 
            mergePackIntoFileToolStripMenuItem.Name = "mergePackIntoFileToolStripMenuItem";
            mergePackIntoFileToolStripMenuItem.Size = new Size(195, 22);
            mergePackIntoFileToolStripMenuItem.Text = "Merge Pack Into File";
            mergePackIntoFileToolStripMenuItem.ToolTipText = "Embed the entire '.pck' file into another file\r\n1. Select the '.pck' file\r\n2. Select the file to embed it in";
            mergePackIntoFileToolStripMenuItem.Click += mergePackIntoFileToolStripMenuItem_Click;
            // 
            // changePackVersionToolStripMenuItem
            // 
            changePackVersionToolStripMenuItem.Name = "changePackVersionToolStripMenuItem";
            changePackVersionToolStripMenuItem.Size = new Size(195, 22);
            changePackVersionToolStripMenuItem.Text = "Change Pack Version";
            changePackVersionToolStripMenuItem.Click += changePackVersionToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(192, 6);
            // 
            // ripPackFromFileToolStripMenuItem
            // 
            ripPackFromFileToolStripMenuItem.Name = "ripPackFromFileToolStripMenuItem";
            ripPackFromFileToolStripMenuItem.Size = new Size(195, 22);
            ripPackFromFileToolStripMenuItem.Text = "Rip Pack from File";
            ripPackFromFileToolStripMenuItem.ToolTipText = "Extract the whole embedded '.pck' file from the selected file\r\n1. Select the file with embedded '.pck'\r\n2. Select the path for the new '.pck' file";
            ripPackFromFileToolStripMenuItem.Click += ripPackFromFileToolStripMenuItem_Click;
            // 
            // removePackFromFileToolStripMenuItem
            // 
            removePackFromFileToolStripMenuItem.Name = "removePackFromFileToolStripMenuItem";
            removePackFromFileToolStripMenuItem.Size = new Size(195, 22);
            removePackFromFileToolStripMenuItem.Text = "Remove Pack from File";
            removePackFromFileToolStripMenuItem.ToolTipText = "Remove the '.pck' package from the target file";
            removePackFromFileToolStripMenuItem.Click += removePackFromFileToolStripMenuItem_Click;
            // 
            // splitExeToolStripMenuItem
            // 
            splitExeToolStripMenuItem.Name = "splitExeToolStripMenuItem";
            splitExeToolStripMenuItem.Size = new Size(195, 22);
            splitExeToolStripMenuItem.Text = "Split File";
            splitExeToolStripMenuItem.ToolTipText = "Split the target file into two in the new location\r\n1. Choose which file you want to split\r\n2. Select a new name and path to the file\r\n3. '.pck' will be created with the same name";
            splitExeToolStripMenuItem.Click += splitExeToolStripMenuItem_Click;
            // 
            // splitExeInPlaceToolStripMenuItem
            // 
            splitExeInPlaceToolStripMenuItem.Name = "splitExeInPlaceToolStripMenuItem";
            splitExeInPlaceToolStripMenuItem.Size = new Size(195, 22);
            splitExeInPlaceToolStripMenuItem.Text = "Split File in Place";
            splitExeInPlaceToolStripMenuItem.ToolTipText = "Split the target file into two: a target file without '.pck' and a separate '.pck'";
            splitExeInPlaceToolStripMenuItem.Click += splitExeInPlaceToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(192, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(195, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // extractToolStripMenuItem
            // 
            extractToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractFileToolStripMenuItem, extractFilteredToolStripMenuItem, extractAllToolStripMenuItem, overwriteExported, checkMD5OnExportToolStripMenuItem, toolStripMenuItem1 });
            extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            extractToolStripMenuItem.Size = new Size(55, 23);
            extractToolStripMenuItem.Text = "Extract";
            // 
            // extractFileToolStripMenuItem
            // 
            extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            extractFileToolStripMenuItem.Size = new Size(245, 22);
            extractFileToolStripMenuItem.Text = "Extract Selected";
            extractFileToolStripMenuItem.Click += extractFileToolStripMenuItem_Click;
            // 
            // extractFilteredToolStripMenuItem
            // 
            extractFilteredToolStripMenuItem.Name = "extractFilteredToolStripMenuItem";
            extractFilteredToolStripMenuItem.Size = new Size(245, 22);
            extractFilteredToolStripMenuItem.Text = "Extract Filtered";
            extractFilteredToolStripMenuItem.Click += extractFilteredToolStripMenuItem_Click;
            // 
            // extractAllToolStripMenuItem
            // 
            extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            extractAllToolStripMenuItem.Size = new Size(245, 22);
            extractAllToolStripMenuItem.Text = "Extract All";
            extractAllToolStripMenuItem.Click += extractAllToolStripMenuItem_Click;
            // 
            // overwriteExported
            // 
            overwriteExported.CheckOnClick = true;
            overwriteExported.Name = "overwriteExported";
            overwriteExported.Size = new Size(245, 22);
            overwriteExported.Text = "Overwrite exported files?";
            overwriteExported.Click += overwriteExported_Click;
            // 
            // checkMD5OnExportToolStripMenuItem
            // 
            checkMD5OnExportToolStripMenuItem.CheckOnClick = true;
            checkMD5OnExportToolStripMenuItem.Name = "checkMD5OnExportToolStripMenuItem";
            checkMD5OnExportToolStripMenuItem.Size = new Size(245, 22);
            checkMD5OnExportToolStripMenuItem.Text = "Check MD5 when exporting?";
            checkMD5OnExportToolStripMenuItem.ToolTipText = "Only for Godot 4+";
            checkMD5OnExportToolStripMenuItem.Click += checkMD5OnExportToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { ifNoEncKeyMode_Cancel, ifNoEncKeyMode_Skip, ifNoEncKeyMode_AsIs });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(245, 22);
            toolStripMenuItem1.Text = "If no encryption key is specified?";
            // 
            // ifNoEncKeyMode_Cancel
            // 
            ifNoEncKeyMode_Cancel.Name = "ifNoEncKeyMode_Cancel";
            ifNoEncKeyMode_Cancel.Size = new Size(176, 22);
            ifNoEncKeyMode_Cancel.Text = "Cancel extraction";
            ifNoEncKeyMode_Cancel.Click += ifNoEncKeyMode_Shared_Click;
            // 
            // ifNoEncKeyMode_Skip
            // 
            ifNoEncKeyMode_Skip.Name = "ifNoEncKeyMode_Skip";
            ifNoEncKeyMode_Skip.Size = new Size(176, 22);
            ifNoEncKeyMode_Skip.Text = "Skip encrypted files";
            ifNoEncKeyMode_Skip.Click += ifNoEncKeyMode_Shared_Click;
            // 
            // ifNoEncKeyMode_AsIs
            // 
            ifNoEncKeyMode_AsIs.Name = "ifNoEncKeyMode_AsIs";
            ifNoEncKeyMode_AsIs.Size = new Size(176, 22);
            ifNoEncKeyMode_AsIs.Text = "Extract as is";
            ifNoEncKeyMode_AsIs.Click += ifNoEncKeyMode_Shared_Click;
            // 
            // integrationToolStripMenuItem
            // 
            integrationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { registerProgram_ToolStripMenuItem, showConsoleToolStripMenuItem });
            integrationToolStripMenuItem.Name = "integrationToolStripMenuItem";
            integrationToolStripMenuItem.Size = new Size(61, 23);
            integrationToolStripMenuItem.Text = "Options";
            // 
            // registerProgram_ToolStripMenuItem
            // 
            registerProgram_ToolStripMenuItem.Name = "registerProgram_ToolStripMenuItem";
            registerProgram_ToolStripMenuItem.Size = new Size(313, 22);
            registerProgram_ToolStripMenuItem.Text = "Register the program to open PCK in Explorer";
            registerProgram_ToolStripMenuItem.Click += registerProgramToOpenPCKInExplorerToolStripMenuItem_Click;
            // 
            // showConsoleToolStripMenuItem
            // 
            showConsoleToolStripMenuItem.CheckOnClick = true;
            showConsoleToolStripMenuItem.Name = "showConsoleToolStripMenuItem";
            showConsoleToolStripMenuItem.Size = new Size(313, 22);
            showConsoleToolStripMenuItem.Text = "Show console";
            showConsoleToolStripMenuItem.Click += showConsoleToolStripMenuItem_Click;
            // 
            // toolStripMenuItem_filterButton
            // 
            toolStripMenuItem_filterButton.Alignment = ToolStripItemAlignment.Right;
            toolStripMenuItem_filterButton.Name = "toolStripMenuItem_filterButton";
            toolStripMenuItem_filterButton.Size = new Size(45, 23);
            toolStripMenuItem_filterButton.Text = "Filter";
            toolStripMenuItem_filterButton.Click += toolStripMenuItem_filter_Click;
            // 
            // toolStripMenuItem_clearFilter
            // 
            toolStripMenuItem_clearFilter.Alignment = ToolStripItemAlignment.Right;
            toolStripMenuItem_clearFilter.Name = "toolStripMenuItem_clearFilter";
            toolStripMenuItem_clearFilter.Padding = new Padding(0);
            toolStripMenuItem_clearFilter.Size = new Size(18, 23);
            toolStripMenuItem_clearFilter.Text = "X";
            toolStripMenuItem_clearFilter.ToolTipText = "Clear filter";
            toolStripMenuItem_clearFilter.Click += toolStripMenuItem_clearFilter_Click;
            // 
            // searchText
            // 
            searchText.Alignment = ToolStripItemAlignment.Right;
            searchText.CueBanner = "Filter text (? and * allowed)";
            searchText.Name = "searchText";
            searchText.Size = new Size(233, 23);
            searchText.ToolTipText = resources.GetString("searchText.ToolTipText");
            // 
            // tsmi_match_case_filter
            // 
            tsmi_match_case_filter.Alignment = ToolStripItemAlignment.Right;
            tsmi_match_case_filter.CheckOnClick = true;
            tsmi_match_case_filter.Font = new Font("Segoe UI", 9F);
            tsmi_match_case_filter.Name = "tsmi_match_case_filter";
            tsmi_match_case_filter.Size = new Size(33, 23);
            tsmi_match_case_filter.Text = "Aa";
            tsmi_match_case_filter.ToolTipText = "Match Case";
            tsmi_match_case_filter.Click += tsmi_match_case_filter_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkUpdates_toolStripMenuItem1, about_toolStripMenuItem1 });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 23);
            helpToolStripMenuItem.Text = "Help";
            // 
            // checkUpdates_toolStripMenuItem1
            // 
            checkUpdates_toolStripMenuItem1.Name = "checkUpdates_toolStripMenuItem1";
            checkUpdates_toolStripMenuItem1.Size = new Size(171, 22);
            checkUpdates_toolStripMenuItem1.Text = "Check for Updates";
            checkUpdates_toolStripMenuItem1.Click += checkForUpdates_tstb_Click;
            // 
            // about_toolStripMenuItem1
            // 
            about_toolStripMenuItem1.Name = "about_toolStripMenuItem1";
            about_toolStripMenuItem1.Size = new Size(171, 22);
            about_toolStripMenuItem1.Text = "About";
            about_toolStripMenuItem1.Click += aboutToolStripMenuItem_Click;
            // 
            // ofd_open_pack
            // 
            ofd_open_pack.Title = "Select the file containing .pck";
            // 
            // fbd_extract_folder
            // 
            fbd_extract_folder.Tag = "";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { path, offset, size, encrypted, removal });
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.Location = new Point(0, 31);
            dataGridView1.Margin = new Padding(4, 0, 4, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(800, 395);
            dataGridView1.TabIndex = 2;
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            dataGridView1.SortCompare += dataGridView1_SortCompare;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, tssl_selected_size, tssl_version_and_stats });
            statusStrip1.Location = new Point(0, 430);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 15;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(610, 17);
            toolStripStatusLabel1.Spring = true;
            toolStripStatusLabel1.Text = "*To select several separate rows, hold Control";
            toolStripStatusLabel1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tssl_selected_size
            // 
            tssl_selected_size.Name = "tssl_selected_size";
            tssl_selected_size.Size = new Size(72, 17);
            tssl_selected_size.Text = "selected size";
            // 
            // tssl_version_and_stats
            // 
            tssl_version_and_stats.Name = "tssl_version_and_stats";
            tssl_version_and_stats.Size = new Size(101, 17);
            tssl_version_and_stats.Text = "version count size";
            // 
            // ofd_remove_pck_from_exe
            // 
            ofd_remove_pck_from_exe.Title = "Removing '.pck' from file";
            // 
            // ofd_split_in_place
            // 
            ofd_split_in_place.Title = "Select the file containing .pck to separate it";
            // 
            // toolTip1
            // 
            toolTip1.ShowAlways = true;
            // 
            // cms_table_row
            // 
            cms_table_row.Items.AddRange(new ToolStripItem[] { copyPathToolStripMenuItem, copyOffsetToolStripMenuItem, copySizeToolStripMenuItem, copySizeInBytesToolStripMenuItem, copyMD5ToolStripMenuItem });
            cms_table_row.Name = "cms_table_row";
            cms_table_row.Size = new Size(170, 114);
            // 
            // copyPathToolStripMenuItem
            // 
            copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
            copyPathToolStripMenuItem.Size = new Size(169, 22);
            copyPathToolStripMenuItem.Text = "Copy Path";
            // 
            // copyOffsetToolStripMenuItem
            // 
            copyOffsetToolStripMenuItem.Name = "copyOffsetToolStripMenuItem";
            copyOffsetToolStripMenuItem.Size = new Size(169, 22);
            copyOffsetToolStripMenuItem.Text = "Copy Offset";
            // 
            // copySizeToolStripMenuItem
            // 
            copySizeToolStripMenuItem.Name = "copySizeToolStripMenuItem";
            copySizeToolStripMenuItem.Size = new Size(169, 22);
            copySizeToolStripMenuItem.Text = "Copy Size";
            // 
            // copySizeInBytesToolStripMenuItem
            // 
            copySizeInBytesToolStripMenuItem.Name = "copySizeInBytesToolStripMenuItem";
            copySizeInBytesToolStripMenuItem.Size = new Size(169, 22);
            copySizeInBytesToolStripMenuItem.Text = "Copy Size in bytes";
            // 
            // copyMD5ToolStripMenuItem
            // 
            copyMD5ToolStripMenuItem.Name = "copyMD5ToolStripMenuItem";
            copyMD5ToolStripMenuItem.Size = new Size(169, 22);
            copyMD5ToolStripMenuItem.Text = "Copy MD5";
            // 
            // ofd_rip_select_pck
            // 
            ofd_rip_select_pck.Title = "Select the file containing .pck";
            // 
            // sfd_rip_save_pack
            // 
            sfd_rip_save_pack.DefaultExt = "pck";
            sfd_rip_save_pack.Filter = "Godot PCK|*.pck";
            sfd_rip_save_pack.Title = "Select the path to the new '.pck' file";
            // 
            // sfd_split_new_file
            // 
            sfd_split_new_file.DefaultExt = "pck";
            sfd_split_new_file.Filter = "Godot PCK|*.pck";
            sfd_split_new_file.Title = "Select the path to the new '.pck' file";
            // 
            // ofd_split_exe_open
            // 
            ofd_split_exe_open.Title = "Select the file containing .pck";
            // 
            // ofd_merge_pck
            // 
            ofd_merge_pck.Title = "Select the file containing .pck";
            // 
            // ofd_merge_target
            // 
            ofd_merge_target.Title = "Select the file to which '.pck' will be added";
            // 
            // ofd_change_version
            // 
            ofd_change_version.Title = "Select the file containing .pck";
            // 
            // path
            // 
            path.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            path.FillWeight = 70F;
            path.HeaderText = "Path";
            path.Name = "path";
            path.ReadOnly = true;
            // 
            // offset
            // 
            offset.FillWeight = 13F;
            offset.HeaderText = "Offset";
            offset.Name = "offset";
            offset.ReadOnly = true;
            // 
            // size
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            size.DefaultCellStyle = dataGridViewCellStyle1;
            size.FillWeight = 13F;
            size.HeaderText = "Size";
            size.Name = "size";
            size.ReadOnly = true;
            // 
            // encrypted
            // 
            encrypted.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            encrypted.DefaultCellStyle = dataGridViewCellStyle2;
            encrypted.HeaderText = "Encrypted";
            encrypted.MaxInputLength = 1;
            encrypted.MinimumWidth = 30;
            encrypted.Name = "encrypted";
            encrypted.ReadOnly = true;
            encrypted.Width = 30;
            // 
            // removal
            // 
            removal.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            removal.DefaultCellStyle = dataGridViewCellStyle3;
            removal.HeaderText = "Removal";
            removal.MaxInputLength = 1;
            removal.MinimumWidth = 30;
            removal.Name = "removal";
            removal.ReadOnly = true;
            removal.Width = 30;
            // 
            // ExplorerMainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnablePreventFocusChange;
            ClientSize = new Size(800, 452);
            Controls.Add(statusStrip1);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ExplorerMainForm";
            Text = "Godot PCK Explorer";
            FormClosing += Form1_FormClosing;
            Shown += ExplorerMainForm_Shown;
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            cms_table_row.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem extractToolStripMenuItem;
        private ToolStripMenuItem extractFileToolStripMenuItem;
        private FolderBrowserDialog fbd_extract_folder;
        private DataGridView dataGridView1;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private ToolStripMenuItem packFolderToolStripMenuItem;
        private ToolStripMenuItem closeFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem integrationToolStripMenuItem;
        private ToolStripMenuItem registerProgram_ToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripMenuItem recentToolStripMenuItem;
        private ToolStripStatusLabel tssl_version_and_stats;
        private ToolStripMenuItem overwriteExported;
        private ToolStripTextBoxWithPlaceholder searchText;
        private ToolStripStatusLabel tssl_selected_size;
        private ToolStripMenuItem mergePackIntoFileToolStripMenuItem;
        private ToolStripMenuItem ripPackFromFileToolStripMenuItem;
        private ToolStripMenuItem removePackFromFileToolStripMenuItem;
        private ToolStripMenuItem splitExeInPlaceToolStripMenuItem;
        private ToolStripMenuItem splitExeToolStripMenuItem;
        private OpenFileDialog ofd_open_pack;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItem_filterButton;
        private OpenFileDialog ofd_remove_pck_from_exe;
        private OpenFileDialog ofd_split_in_place;
        private ToolTip toolTip1;
        private ContextMenuStrip cms_table_row;
        private ToolStripMenuItem copyPathToolStripMenuItem;
        private ToolStripMenuItem copyOffsetToolStripMenuItem;
        private ToolStripMenuItem copySizeToolStripMenuItem;
        private ToolStripMenuItem copySizeInBytesToolStripMenuItem;
        private OpenFileDialog ofd_rip_select_pck;
        private SaveFileDialog sfd_rip_save_pack;
        private SaveFileDialog sfd_split_new_file;
        private OpenFileDialog ofd_split_exe_open;
        private OpenFileDialog ofd_merge_pck;
        private OpenFileDialog ofd_merge_target;
        private ToolStripMenuItem changePackVersionToolStripMenuItem;
        private OpenFileDialog ofd_change_version;
        private ToolStripMenuItem tsmi_match_case_filter;
        private ToolStripMenuItem showConsoleToolStripMenuItem;
        private ToolStripMenuItem checkMD5OnExportToolStripMenuItem;
        private ToolStripMenuItem copyMD5ToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem about_toolStripMenuItem1;
        private ToolStripMenuItem checkUpdates_toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem_clearFilter;
        private ToolStripMenuItem extractFilteredToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem ifNoEncKeyMode_Cancel;
        private ToolStripMenuItem ifNoEncKeyMode_Skip;
        private ToolStripMenuItem ifNoEncKeyMode_AsIs;
        private DataGridViewTextBoxColumn path;
        private DataGridViewTextBoxColumn offset;
        private DataGridViewTextBoxColumn size;
        private DataGridViewTextBoxColumn encrypted;
        private DataGridViewTextBoxColumn removal;
    }
}

