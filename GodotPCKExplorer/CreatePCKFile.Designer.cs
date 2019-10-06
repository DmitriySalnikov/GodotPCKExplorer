namespace GodotPCKExplorer
{
	partial class CreatePCKFile
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.filePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.btn_create = new System.Windows.Forms.Button();
			this.l_total_size = new System.Windows.Forms.Label();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.nud_major = new System.Windows.Forms.NumericUpDown();
			this.nud_minor = new System.Windows.Forms.NumericUpDown();
			this.nud_revision = new System.Windows.Forms.NumericUpDown();
			this.nud_ver = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_major)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_minor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_revision)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_ver)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.filePath,
            this.size});
			this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dataGridView1.Location = new System.Drawing.Point(12, 12);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(776, 397);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserDeletedRow);
			// 
			// filePath
			// 
			this.filePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.filePath.FillWeight = 85F;
			this.filePath.HeaderText = "File Path";
			this.filePath.Name = "filePath";
			this.filePath.ReadOnly = true;
			// 
			// size
			// 
			this.size.FillWeight = 15F;
			this.size.HeaderText = "Size";
			this.size.Name = "size";
			this.size.ReadOnly = true;
			// 
			// btn_create
			// 
			this.btn_create.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_create.Location = new System.Drawing.Point(712, 415);
			this.btn_create.Name = "btn_create";
			this.btn_create.Size = new System.Drawing.Size(75, 20);
			this.btn_create.TabIndex = 1;
			this.btn_create.Text = "Create";
			this.btn_create.UseVisualStyleBackColor = true;
			this.btn_create.Click += new System.EventHandler(this.btn_create_Click);
			// 
			// l_total_size
			// 
			this.l_total_size.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.l_total_size.AutoSize = true;
			this.l_total_size.Location = new System.Drawing.Point(12, 420);
			this.l_total_size.Name = "l_total_size";
			this.l_total_size.Size = new System.Drawing.Size(35, 13);
			this.l_total_size.TabIndex = 2;
			this.l_total_size.Text = "label1";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.Filter = "Godot PCK|*.pck";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(203, 419);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(327, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Godot PCK version and Godot Engine version(major, minor, revision)";
			// 
			// nud_major
			// 
			this.nud_major.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nud_major.Location = new System.Drawing.Point(580, 415);
			this.nud_major.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nud_major.Name = "nud_major";
			this.nud_major.Size = new System.Drawing.Size(38, 20);
			this.nud_major.TabIndex = 8;
			this.nud_major.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// nud_minor
			// 
			this.nud_minor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nud_minor.Location = new System.Drawing.Point(624, 415);
			this.nud_minor.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nud_minor.Name = "nud_minor";
			this.nud_minor.Size = new System.Drawing.Size(38, 20);
			this.nud_minor.TabIndex = 9;
			this.nud_minor.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// nud_revision
			// 
			this.nud_revision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nud_revision.Location = new System.Drawing.Point(668, 415);
			this.nud_revision.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nud_revision.Name = "nud_revision";
			this.nud_revision.Size = new System.Drawing.Size(38, 20);
			this.nud_revision.TabIndex = 10;
			// 
			// nud_ver
			// 
			this.nud_ver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nud_ver.Location = new System.Drawing.Point(536, 415);
			this.nud_ver.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nud_ver.Name = "nud_ver";
			this.nud_ver.Size = new System.Drawing.Size(38, 20);
			this.nud_ver.TabIndex = 11;
			this.nud_ver.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// CreatePCKFile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.nud_ver);
			this.Controls.Add(this.nud_revision);
			this.Controls.Add(this.nud_minor);
			this.Controls.Add(this.nud_major);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.l_total_size);
			this.Controls.Add(this.btn_create);
			this.Controls.Add(this.dataGridView1);
			this.MinimumSize = new System.Drawing.Size(816, 489);
			this.Name = "CreatePCKFile";
			this.Text = "Create PCK File";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_major)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_minor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_revision)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nud_ver)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn filePath;
		private System.Windows.Forms.DataGridViewTextBoxColumn size;
		private System.Windows.Forms.Button btn_create;
		private System.Windows.Forms.Label l_total_size;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nud_major;
		private System.Windows.Forms.NumericUpDown nud_minor;
		private System.Windows.Forms.NumericUpDown nud_revision;
		private System.Windows.Forms.NumericUpDown nud_ver;
	}
}