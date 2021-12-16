namespace GodotPCKExplorer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.packFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.integrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unregisterProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofd_open_pack = new System.Windows.Forms.OpenFileDialog();
            this.fbd_extract_folder = new System.Windows.Forms.FolderBrowserDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fbd_pack_folder = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tssl_version_and_stats = new System.Windows.Forms.ToolStripStatusLabel();
            this.overwriteExported = new System.Windows.Forms.ToolStripMenuItem();
            this.searchText = new ToolStripTextBoxWithPlaceholder();
            this.tssl_selected_size = new System.Windows.Forms.ToolStripStatusLabel();
            this.mergePackIntoFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ripPackFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePackFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitExeInPlaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitExeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ofd_remove_pck_from_exe = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.extractToolStripMenuItem,
            this.integrationToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem1,
            this.searchText});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
            // 
            // packFolderToolStripMenuItem
            // 
            this.packFolderToolStripMenuItem.Name = "packFolderToolStripMenuItem";
            this.packFolderToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.packFolderToolStripMenuItem.Text = "Pack or Embed folder";
            this.packFolderToolStripMenuItem.Click += new System.EventHandler(this.packFolderToolStripMenuItem_Click);
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
            this.overwriteExported});
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(55, 23);
            this.extractToolStripMenuItem.Text = "Extract";
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.extractFileToolStripMenuItem.Text = "Extract Selected";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.extractAllToolStripMenuItem.Text = "Extract All";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAllToolStripMenuItem_Click);
            // 
            // integrationToolStripMenuItem
            // 
            this.integrationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem,
            this.unregisterProgramToolStripMenuItem});
            this.integrationToolStripMenuItem.Name = "integrationToolStripMenuItem";
            this.integrationToolStripMenuItem.Size = new System.Drawing.Size(77, 23);
            this.integrationToolStripMenuItem.Text = "Integration";
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
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 23);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ofd_open_pack
            // 
            this.ofd_open_pack.Filter = "Godot PCK files and Executables|*.pck;*.exe|Godot PCK files|*.pck|Executables|*.e" +
    "xe|All files|*.*";
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.size.DefaultCellStyle = dataGridViewCellStyle2;
            this.size.FillWeight = 13F;
            this.size.HeaderText = "Size";
            this.size.Name = "size";
            this.size.ReadOnly = true;
            // 
            // fbd_pack_folder
            // 
            this.fbd_pack_folder.Tag = "";
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
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Enabled = false;
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.recentToolStripMenuItem.Text = "Recent Files";
            // 
            // tssl_version_and_stats
            // 
            this.tssl_version_and_stats.Name = "tssl_version_and_stats";
            this.tssl_version_and_stats.Size = new System.Drawing.Size(101, 17);
            this.tssl_version_and_stats.Text = "version count size";
            // 
            // overwriteExported
            // 
            this.overwriteExported.CheckOnClick = true;
            this.overwriteExported.Name = "overwriteExported";
            this.overwriteExported.Size = new System.Drawing.Size(204, 22);
            this.overwriteExported.Text = "Overwrite exported files?";
            this.overwriteExported.Click += new System.EventHandler(this.overwriteExported_Click);
            // 
            // searchText
            // 
            this.searchText.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchText.CueBanner = "Filter text";
            this.searchText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(150, 23);
            this.searchText.TextChanged += new System.EventHandler(this.searchText_TextChanged);
            // 
            // tssl_selected_size
            // 
            this.tssl_selected_size.Name = "tssl_selected_size";
            this.tssl_selected_size.Size = new System.Drawing.Size(72, 17);
            this.tssl_selected_size.Text = "selected size";
            // 
            // mergePackIntoFileToolStripMenuItem
            // 
            this.mergePackIntoFileToolStripMenuItem.Name = "mergePackIntoFileToolStripMenuItem";
            this.mergePackIntoFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.mergePackIntoFileToolStripMenuItem.Text = "Merge pack into exe";
            // 
            // ripPackFromFileToolStripMenuItem
            // 
            this.ripPackFromFileToolStripMenuItem.Name = "ripPackFromFileToolStripMenuItem";
            this.ripPackFromFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.ripPackFromFileToolStripMenuItem.Text = "Rip pack from exe";
            // 
            // removePackFromFileToolStripMenuItem
            // 
            this.removePackFromFileToolStripMenuItem.Name = "removePackFromFileToolStripMenuItem";
            this.removePackFromFileToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.removePackFromFileToolStripMenuItem.Text = "Remove pack from exe";
            this.removePackFromFileToolStripMenuItem.Click += new System.EventHandler(this.removePackFromFileToolStripMenuItem_Click);
            // 
            // splitExeInPlaceToolStripMenuItem
            // 
            this.splitExeInPlaceToolStripMenuItem.Name = "splitExeInPlaceToolStripMenuItem";
            this.splitExeInPlaceToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.splitExeInPlaceToolStripMenuItem.Text = "Split exe in place";
            // 
            // splitExeToolStripMenuItem
            // 
            this.splitExeToolStripMenuItem.Name = "splitExeToolStripMenuItem";
            this.splitExeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.splitExeToolStripMenuItem.Text = "Split exe";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(192, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(45, 23);
            this.toolStripMenuItem1.Text = "Filter";
            // 
            // ofd_remove_pck_from_exe
            // 
            this.ofd_remove_pck_from_exe.Title = "Select the file containing .pck";
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
		private System.Windows.Forms.FolderBrowserDialog fbd_pack_folder;
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
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog ofd_remove_pck_from_exe;
    }
}

