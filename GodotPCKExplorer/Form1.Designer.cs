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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
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
            this.saveFileToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openFileToolStripMenuItem
			// 
			this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
			this.openFileToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.openFileToolStripMenuItem.Text = "Open File";
			this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
			// 
			// saveFileToolStripMenuItem
			// 
			this.saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
			this.saveFileToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.saveFileToolStripMenuItem.Text = "Save File";
			this.saveFileToolStripMenuItem.Click += new System.EventHandler(this.saveFileToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFileToolStripMenuItem,
            this.extractAllToolStripMenuItem,
            this.addFileToolStripMenuItem,
            this.addFolderToolStripMenuItem});
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
			// addFileToolStripMenuItem
			// 
			this.addFileToolStripMenuItem.Name = "addFileToolStripMenuItem";
			this.addFileToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.addFileToolStripMenuItem.Text = "Add Files";
			// 
			// addFolderToolStripMenuItem
			// 
			this.addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
			this.addFolderToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.addFolderToolStripMenuItem.Text = "Add Folder";
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
			this.openFileDialog1.Filter = "Godot PCK files|*.pck|All files|*.*";
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.path,
            this.offset,
            this.size});
			this.dataGridView1.Location = new System.Drawing.Point(12, 27);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(776, 411);
			this.dataGridView1.TabIndex = 2;
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
			this.offset.FillWeight = 15F;
			this.offset.HeaderText = "Offset";
			this.offset.Name = "offset";
			this.offset.ReadOnly = true;
			// 
			// size
			// 
			this.size.FillWeight = 15F;
			this.size.HeaderText = "Size";
			this.size.Name = "size";
			this.size.ReadOnly = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
		private System.Windows.Forms.ToolStripMenuItem addFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addFolderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn path;
		private System.Windows.Forms.DataGridViewTextBoxColumn offset;
		private System.Windows.Forms.DataGridViewTextBoxColumn size;
		private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveFileToolStripMenuItem;
	}
}

