namespace GodotPCKExplorer.UI
{
	partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.packFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergePackIntoFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePackVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ripPackFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePackFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitExeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitExeInPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overwriteExported = new System.Windows.Forms.ToolStripMenuItem();
            this.checkMD5OnExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.integrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unregisterProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_filterButton = new System.Windows.Forms.ToolStripMenuItem();
            this.searchText = new ToolStripTextBoxWithPlaceholder();
            this.tsmi_match_case_filter = new System.Windows.Forms.ToolStripMenuItem();
            this.ofd_open_pack = new System.Windows.Forms.OpenFileDialog();
            this.fbd_extract_folder = new System.Windows.Forms.FolderBrowserDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_selected_size = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_version_and_stats = new System.Windows.Forms.ToolStripStatusLabel();
            this.ofd_remove_pck_from_exe = new System.Windows.Forms.OpenFileDialog();
            this.ofd_split_in_place = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cms_table_row = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySizeInBytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofd_rip_select_pck = new System.Windows.Forms.OpenFileDialog();
            this.sfd_rip_save_pack = new System.Windows.Forms.SaveFileDialog();
            this.sfd_split_new_file = new System.Windows.Forms.SaveFileDialog();
            this.ofd_split_exe_open = new System.Windows.Forms.OpenFileDialog();
            this.ofd_merge_pck = new System.Windows.Forms.OpenFileDialog();
            this.ofd_merge_target = new System.Windows.Forms.OpenFileDialog();
            this.ofd_change_version = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.cms_table_row.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.extractToolStripMenuItem,
            this.integrationToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem_filterButton,
            this.searchText,
            this.tsmi_match_case_filter});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ShowItemToolTips = true;
            this.menuStrip1.Size = new System.Drawing.Size(800, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.closeFileToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator1,
            this.packFolderToolStripMenuItem,
            this.mergePackIntoFileToolStripMenuItem,
            this.changePackVersionToolStripMenuItem,
            this.toolStripSeparator3,
            this.ripPackFromFileToolStripMenuItem,
            this.removePackFromFileToolStripMenuItem,
            this.splitExeToolStripMenuItem,
            this.splitExeInPlaceToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // closeFileToolStripMenuItem
            // 
            this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.closeFileToolStripMenuItem.Text = "Close File";
            this.closeFileToolStripMenuItem.Click += new System.EventHandler(this.closeFileToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Enabled = false;
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.recentToolStripMenuItem.Text = "Recent Files";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // packFolderToolStripMenuItem
            // 
            this.packFolderToolStripMenuItem.Name = "packFolderToolStripMenuItem";
            this.packFolderToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.packFolderToolStripMenuItem.Text = "Pack or Embed Folder";
            this.packFolderToolStripMenuItem.ToolTipText = resources.GetString("packFolderToolStripMenuItem.ToolTipText");
            this.packFolderToolStripMenuItem.Click += new System.EventHandler(this.packFolderToolStripMenuItem_Click);
            // 
            // mergePackIntoFileToolStripMenuItem
            // 
            this.mergePackIntoFileToolStripMenuItem.Name = "mergePackIntoFileToolStripMenuItem";
            this.mergePackIntoFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.mergePackIntoFileToolStripMenuItem.Text = "Merge Pack Into File";
            this.mergePackIntoFileToolStripMenuItem.ToolTipText = "Embed the entire \'.pck\' file into another file\r\n1. Select the \'.pck\' file\r\n2. Sel" +
    "ect the file to embed it in";
            this.mergePackIntoFileToolStripMenuItem.Click += new System.EventHandler(this.mergePackIntoFileToolStripMenuItem_Click);
            // 
            // changePackVersionToolStripMenuItem
            // 
            this.changePackVersionToolStripMenuItem.Name = "changePackVersionToolStripMenuItem";
            this.changePackVersionToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.changePackVersionToolStripMenuItem.Text = "Change Pack Version";
            this.changePackVersionToolStripMenuItem.Click += new System.EventHandler(this.changePackVersionToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(192, 6);
            // 
            // ripPackFromFileToolStripMenuItem
            // 
            this.ripPackFromFileToolStripMenuItem.Name = "ripPackFromFileToolStripMenuItem";
            this.ripPackFromFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.ripPackFromFileToolStripMenuItem.Text = "Rip Pack from File";
            this.ripPackFromFileToolStripMenuItem.ToolTipText = "Extract the whole embedded \'.pck\' file from the selected file\r\n1. Select the file" +
    " with embedded \'.pck\'\r\n2. Select the path for the new \'.pck\' file";
            this.ripPackFromFileToolStripMenuItem.Click += new System.EventHandler(this.ripPackFromFileToolStripMenuItem_Click);
            // 
            // removePackFromFileToolStripMenuItem
            // 
            this.removePackFromFileToolStripMenuItem.Name = "removePackFromFileToolStripMenuItem";
            this.removePackFromFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.removePackFromFileToolStripMenuItem.Text = "Remove Pack from File";
            this.removePackFromFileToolStripMenuItem.ToolTipText = "Remove the \'.pck\' package from the target file";
            this.removePackFromFileToolStripMenuItem.Click += new System.EventHandler(this.removePackFromFileToolStripMenuItem_Click);
            // 
            // splitExeToolStripMenuItem
            // 
            this.splitExeToolStripMenuItem.Name = "splitExeToolStripMenuItem";
            this.splitExeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.splitExeToolStripMenuItem.Text = "Split File";
            this.splitExeToolStripMenuItem.ToolTipText = "Split the target file into two in the new location\r\n1. Choose which file you want" +
    " to split\r\n2. Select a new name and path to the file\r\n3. \'.pck\' will be created " +
    "with the same name";
            this.splitExeToolStripMenuItem.Click += new System.EventHandler(this.splitExeToolStripMenuItem_Click);
            // 
            // splitExeInPlaceToolStripMenuItem
            // 
            this.splitExeInPlaceToolStripMenuItem.Name = "splitExeInPlaceToolStripMenuItem";
            this.splitExeInPlaceToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.splitExeInPlaceToolStripMenuItem.Text = "Split File in Place";
            this.splitExeInPlaceToolStripMenuItem.ToolTipText = "Split the target file into two: a target file without \'.pck\' and a separate \'.pck" +
    "\'";
            this.splitExeInPlaceToolStripMenuItem.Click += new System.EventHandler(this.splitExeInPlaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(192, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFileToolStripMenuItem,
            this.extractAllToolStripMenuItem,
            this.overwriteExported,
            this.checkMD5OnExportToolStripMenuItem});
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(55, 23);
            this.extractToolStripMenuItem.Text = "Extract";
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.extractFileToolStripMenuItem.Text = "Extract Selected";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.extractAllToolStripMenuItem.Text = "Extract All";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAllToolStripMenuItem_Click);
            // 
            // overwriteExported
            // 
            this.overwriteExported.CheckOnClick = true;
            this.overwriteExported.Name = "overwriteExported";
            this.overwriteExported.Size = new System.Drawing.Size(226, 22);
            this.overwriteExported.Text = "Overwrite exported files?";
            this.overwriteExported.Click += new System.EventHandler(this.overwriteExported_Click);
            // 
            // checkMD5OnExportToolStripMenuItem
            // 
            this.checkMD5OnExportToolStripMenuItem.CheckOnClick = true;
            this.checkMD5OnExportToolStripMenuItem.Name = "checkMD5OnExportToolStripMenuItem";
            this.checkMD5OnExportToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.checkMD5OnExportToolStripMenuItem.Text = "Check MD5 when exporting?";
            this.checkMD5OnExportToolStripMenuItem.ToolTipText = "Only for Godot 4+";
            this.checkMD5OnExportToolStripMenuItem.Click += new System.EventHandler(this.checkMD5OnExportToolStripMenuItem_Click);
            // 
            // integrationToolStripMenuItem
            // 
            this.integrationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem,
            this.unregisterProgramToolStripMenuItem,
            this.showConsoleToolStripMenuItem});
            this.integrationToolStripMenuItem.Name = "integrationToolStripMenuItem";
            this.integrationToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.integrationToolStripMenuItem.Text = "Options";
            // 
            // registerProgramToOpenPCKInExplorerToolStripMenuItem
            // 
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem.Name = "registerProgramToOpenPCKInExplorerToolStripMenuItem";
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem.Size = new System.Drawing.Size(293, 22);
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem.Text = "Register program to open PCK in explorer";
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem.Click += new System.EventHandler(this.registerProgramToOpenPCKInExplorerToolStripMenuItem_Click);
            // 
            // unregisterProgramToolStripMenuItem
            // 
            this.unregisterProgramToolStripMenuItem.Name = "unregisterProgramToolStripMenuItem";
            this.unregisterProgramToolStripMenuItem.Size = new System.Drawing.Size(293, 22);
            this.unregisterProgramToolStripMenuItem.Text = "Unregister program";
            this.unregisterProgramToolStripMenuItem.Click += new System.EventHandler(this.unregisterProgramToolStripMenuItem_Click);
            // 
            // showConsoleToolStripMenuItem
            // 
            this.showConsoleToolStripMenuItem.CheckOnClick = true;
            this.showConsoleToolStripMenuItem.Name = "showConsoleToolStripMenuItem";
            this.showConsoleToolStripMenuItem.Size = new System.Drawing.Size(293, 22);
            this.showConsoleToolStripMenuItem.Text = "Show console";
            this.showConsoleToolStripMenuItem.Click += new System.EventHandler(this.showConsoleToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 23);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripMenuItem_filterButton
            // 
            this.toolStripMenuItem_filterButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItem_filterButton.Name = "toolStripMenuItem_filterButton";
            this.toolStripMenuItem_filterButton.Size = new System.Drawing.Size(45, 23);
            this.toolStripMenuItem_filterButton.Text = "Filter";
            this.toolStripMenuItem_filterButton.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // searchText
            // 
            this.searchText.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchText.CueBanner = "Filter text (? and * allowed)";
            this.searchText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(200, 23);
            this.searchText.ToolTipText = resources.GetString("searchText.ToolTipText");
            // 
            // tsmi_match_case_filter
            // 
            this.tsmi_match_case_filter.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsmi_match_case_filter.CheckOnClick = true;
            this.tsmi_match_case_filter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tsmi_match_case_filter.Name = "tsmi_match_case_filter";
            this.tsmi_match_case_filter.Size = new System.Drawing.Size(33, 23);
            this.tsmi_match_case_filter.Text = "Aa";
            this.tsmi_match_case_filter.ToolTipText = "Match Case";
            this.tsmi_match_case_filter.Click += new System.EventHandler(this.tsmi_match_case_filter_Click);
            // 
            // ofd_open_pack
            // 
            this.ofd_open_pack.Title = "Select the file containing .pck";
            // 
            // fbd_extract_folder
            // 
            this.fbd_extract_folder.Tag = "";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.path,
            this.offset,
            this.size});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 27);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(800, 403);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.dataGridView1.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridView1_SortCompare);
            // 
            // path
            // 
            this.path.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.path.FillWeight = 70F;
            this.path.HeaderText = "Path";
            this.path.Name = "path";
            this.path.ReadOnly = true;
            // 
            // offset
            // 
            this.offset.FillWeight = 13F;
            this.offset.HeaderText = "Offset";
            this.offset.Name = "offset";
            this.offset.ReadOnly = true;
            // 
            // size
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.size.DefaultCellStyle = dataGridViewCellStyle1;
            this.size.FillWeight = 13F;
            this.size.HeaderText = "Size";
            this.size.Name = "size";
            this.size.ReadOnly = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tssl_selected_size,
            this.tssl_version_and_stats});
            this.statusStrip1.Location = new System.Drawing.Point(0, 430);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(612, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "*To select several separate rows, hold Control";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tssl_selected_size
            // 
            this.tssl_selected_size.Name = "tssl_selected_size";
            this.tssl_selected_size.Size = new System.Drawing.Size(72, 17);
            this.tssl_selected_size.Text = "selected size";
            // 
            // tssl_version_and_stats
            // 
            this.tssl_version_and_stats.Name = "tssl_version_and_stats";
            this.tssl_version_and_stats.Size = new System.Drawing.Size(101, 17);
            this.tssl_version_and_stats.Text = "version count size";
            // 
            // ofd_remove_pck_from_exe
            // 
            this.ofd_remove_pck_from_exe.Title = "Removing \'.pck\' from file";
            // 
            // ofd_split_in_place
            // 
            this.ofd_split_in_place.Title = "Select the file containing .pck to separate it";
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // cms_table_row
            // 
            this.cms_table_row.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPathToolStripMenuItem,
            this.copyOffsetToolStripMenuItem,
            this.copySizeToolStripMenuItem,
            this.copySizeInBytesToolStripMenuItem});
            this.cms_table_row.Name = "cms_table_row";
            this.cms_table_row.Size = new System.Drawing.Size(170, 92);
            // 
            // copyPathToolStripMenuItem
            // 
            this.copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
            this.copyPathToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.copyPathToolStripMenuItem.Text = "Copy Path";
            // 
            // copyOffsetToolStripMenuItem
            // 
            this.copyOffsetToolStripMenuItem.Name = "copyOffsetToolStripMenuItem";
            this.copyOffsetToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.copyOffsetToolStripMenuItem.Text = "Copy Offset";
            // 
            // copySizeToolStripMenuItem
            // 
            this.copySizeToolStripMenuItem.Name = "copySizeToolStripMenuItem";
            this.copySizeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.copySizeToolStripMenuItem.Text = "Copy Size";
            // 
            // copySizeInBytesToolStripMenuItem
            // 
            this.copySizeInBytesToolStripMenuItem.Name = "copySizeInBytesToolStripMenuItem";
            this.copySizeInBytesToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.copySizeInBytesToolStripMenuItem.Text = "Copy Size in bytes";
            // 
            // ofd_rip_select_pck
            // 
            this.ofd_rip_select_pck.Title = "Select the file containing .pck";
            // 
            // sfd_rip_save_pack
            // 
            this.sfd_rip_save_pack.DefaultExt = "pck";
            this.sfd_rip_save_pack.Filter = "Godot PCK|*.pck";
            this.sfd_rip_save_pack.Title = "Select the path to the new \'.pck\' file";
            // 
            // sfd_split_new_file
            // 
            this.sfd_split_new_file.DefaultExt = "pck";
            this.sfd_split_new_file.Filter = "Godot PCK|*.pck";
            this.sfd_split_new_file.Title = "Select the path to the new \'.pck\' file";
            // 
            // ofd_split_exe_open
            // 
            this.ofd_split_exe_open.Title = "Select the file containing .pck";
            // 
            // ofd_merge_pck
            // 
            this.ofd_merge_pck.Title = "Select the file containing .pck";
            // 
            // ofd_merge_target
            // 
            this.ofd_merge_target.Title = "Select the file to which \'.pck\' will be added";
            // 
            // ofd_change_version
            // 
            this.ofd_change_version.Title = "Select the file containing .pck";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(800, 452);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Godot PCK Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cms_table_row.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.FolderBrowserDialog fbd_extract_folder;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem packFolderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.DataGridViewTextBoxColumn path;
		private System.Windows.Forms.DataGridViewTextBoxColumn offset;
		private System.Windows.Forms.DataGridViewTextBoxColumn size;
		private System.Windows.Forms.ToolStripMenuItem integrationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem registerProgramToOpenPCKInExplorerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unregisterProgramToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tssl_version_and_stats;
        private System.Windows.Forms.ToolStripMenuItem overwriteExported;
        private ToolStripTextBoxWithPlaceholder searchText;
        private System.Windows.Forms.ToolStripStatusLabel tssl_selected_size;
        private System.Windows.Forms.ToolStripMenuItem mergePackIntoFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ripPackFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePackFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitExeInPlaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitExeToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofd_open_pack;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_filterButton;
        private System.Windows.Forms.OpenFileDialog ofd_remove_pck_from_exe;
        private System.Windows.Forms.OpenFileDialog ofd_split_in_place;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip cms_table_row;
        private System.Windows.Forms.ToolStripMenuItem copyPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySizeInBytesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofd_rip_select_pck;
        private System.Windows.Forms.SaveFileDialog sfd_rip_save_pack;
        private System.Windows.Forms.SaveFileDialog sfd_split_new_file;
        private System.Windows.Forms.OpenFileDialog ofd_split_exe_open;
        private System.Windows.Forms.OpenFileDialog ofd_merge_pck;
        private System.Windows.Forms.OpenFileDialog ofd_merge_target;
        private System.Windows.Forms.ToolStripMenuItem changePackVersionToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofd_change_version;
        private System.Windows.Forms.ToolStripMenuItem tsmi_match_case_filter;
        private System.Windows.Forms.ToolStripMenuItem showConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkMD5OnExportToolStripMenuItem;
    }
}

