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
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.integrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unregisterProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.folderBrowserDialog_pack_folder = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_files_count = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_total_size = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.integrationToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.closeFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.packFolderToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // closeFileToolStripMenuItem
            // 
            this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.closeFileToolStripMenuItem.Text = "Close File";
            this.closeFileToolStripMenuItem.Click += new System.EventHandler(this.closeFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // packFolderToolStripMenuItem
            // 
            this.packFolderToolStripMenuItem.Name = "packFolderToolStripMenuItem";
            this.packFolderToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.packFolderToolStripMenuItem.Text = "Pack folder to PCK";
            this.packFolderToolStripMenuItem.Click += new System.EventHandler(this.packFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(169, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFileToolStripMenuItem,
            this.extractAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.extractFileToolStripMenuItem.Text = "Extract Selected";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.extractAllToolStripMenuItem.Text = "Extract All";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAllToolStripMenuItem_Click);
            // 
            // integrationToolStripMenuItem
            // 
            this.integrationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerProgramToOpenPCKInExplorerToolStripMenuItem,
            this.unregisterProgramToolStripMenuItem});
            this.integrationToolStripMenuItem.Name = "integrationToolStripMenuItem";
            this.integrationToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Godot PCK files and Executables|*.pck;*.exe|Godot PCK files|*.pck|Executables|*.e" +
    "xe|All files|*.*";
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 24);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(800, 406);
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tssl_files_count,
            this.tssl_total_size});
            this.statusStrip1.Location = new System.Drawing.Point(0, 430);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(639, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "*To select several separate rows, hold Control";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tssl_files_count
            // 
            this.tssl_files_count.Name = "tssl_files_count";
            this.tssl_files_count.Size = new System.Drawing.Size(62, 17);
            this.tssl_files_count.Text = "files count";
            // 
            // tssl_total_size
            // 
            this.tssl_total_size.Name = "tssl_total_size";
            this.tssl_total_size.Size = new System.Drawing.Size(53, 17);
            this.tssl_total_size.Text = "total size";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 452);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Godot PCK Explorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
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
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extractFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem packFolderToolStripMenuItem;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_pack_folder;
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
        private System.Windows.Forms.ToolStripStatusLabel tssl_files_count;
        private System.Windows.Forms.ToolStripStatusLabel tssl_total_size;
    }
}

