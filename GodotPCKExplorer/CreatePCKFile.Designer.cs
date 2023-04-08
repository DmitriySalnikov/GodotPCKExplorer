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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatePCKFile));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btn_create = new System.Windows.Forms.Button();
            this.l_total_size = new System.Windows.Forms.Label();
            this.sfd_save_pack = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.nud_major = new System.Windows.Forms.NumericUpDown();
            this.nud_minor = new System.Windows.Forms.NumericUpDown();
            this.nud_revision = new System.Windows.Forms.NumericUpDown();
            this.cb_ver = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_embed = new System.Windows.Forms.CheckBox();
            this.ofd_pack_into = new System.Windows.Forms.OpenFileDialog();
            this.tb_folder_path = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_browse = new System.Windows.Forms.Button();
            this.fbd_pack_folder = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.btn_filter = new System.Windows.Forms.Button();
            this.btn_match_case = new System.Windows.Forms.Button();
            this.l_total_count = new System.Windows.Forms.Label();
            this.searchText = new TextBoxWithPlaceholder();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.nud_alignment = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_generate_key = new System.Windows.Forms.Button();
            this.cb_enable_encryption = new System.Windows.Forms.CheckBox();
            this.filePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_major)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_minor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_revision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_alignment)).BeginInit();
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 59);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(800, 344);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridView1_SortCompare);
            this.dataGridView1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserDeletedRow);
            // 
            // btn_create
            // 
            this.btn_create.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_create.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_create.Location = new System.Drawing.Point(722, 409);
            this.btn_create.Name = "btn_create";
            this.btn_create.Size = new System.Drawing.Size(75, 69);
            this.btn_create.TabIndex = 1;
            this.btn_create.Text = "Pack";
            this.btn_create.UseVisualStyleBackColor = true;
            this.btn_create.Click += new System.EventHandler(this.btn_create_Click);
            // 
            // l_total_size
            // 
            this.l_total_size.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.l_total_size.AutoSize = true;
            this.l_total_size.Location = new System.Drawing.Point(12, 462);
            this.l_total_size.Name = "l_total_size";
            this.l_total_size.Size = new System.Drawing.Size(54, 13);
            this.l_total_size.TabIndex = 2;
            this.l_total_size.Text = "Total Size";
            // 
            // sfd_save_pack
            // 
            this.sfd_save_pack.DefaultExt = "pck";
            this.sfd_save_pack.Filter = "Godot PCK|*.pck";
            this.sfd_save_pack.Title = "Select the path to the new \'.pck\' file";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(213, 460);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Godot PCK version and Godot Engine version(major, minor, revision)";
            // 
            // nud_major
            // 
            this.nud_major.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nud_major.Location = new System.Drawing.Point(590, 458);
            this.nud_major.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_major.Name = "nud_major";
            this.nud_major.Size = new System.Drawing.Size(38, 20);
            this.nud_major.TabIndex = 8;
            this.nud_major.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // nud_minor
            // 
            this.nud_minor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nud_minor.Location = new System.Drawing.Point(634, 458);
            this.nud_minor.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_minor.Name = "nud_minor";
            this.nud_minor.Size = new System.Drawing.Size(38, 20);
            this.nud_minor.TabIndex = 9;
            // 
            // nud_revision
            // 
            this.nud_revision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nud_revision.Location = new System.Drawing.Point(678, 458);
            this.nud_revision.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_revision.Name = "nud_revision";
            this.nud_revision.Size = new System.Drawing.Size(38, 20);
            this.nud_revision.TabIndex = 10;
            // 
            // cb_ver
            // 
            this.cb_ver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_ver.FormattingEnabled = true;
            this.cb_ver.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cb_ver.Location = new System.Drawing.Point(546, 457);
            this.cb_ver.MaxLength = 2;
            this.cb_ver.Name = "cb_ver";
            this.cb_ver.Size = new System.Drawing.Size(38, 21);
            this.cb_ver.TabIndex = 12;
            this.cb_ver.Text = "2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(356, 26);
            this.label2.TabIndex = 13;
            this.label2.Text = "*You can delete files from the table by selecting them and pressing Delete.\r\nTo s" +
    "elect several separate rows, hold Control";
            // 
            // cb_embed
            // 
            this.cb_embed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_embed.AutoSize = true;
            this.cb_embed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_embed.Location = new System.Drawing.Point(542, 435);
            this.cb_embed.Name = "cb_embed";
            this.cb_embed.Size = new System.Drawing.Size(174, 17);
            this.cb_embed.TabIndex = 14;
            this.cb_embed.Text = "Embed .pck into executable file";
            this.cb_embed.UseVisualStyleBackColor = true;
            // 
            // ofd_pack_into
            // 
            this.ofd_pack_into.Title = "Select the file to embed the package into";
            // 
            // tb_folder_path
            // 
            this.tb_folder_path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_folder_path.Location = new System.Drawing.Point(148, 6);
            this.tb_folder_path.Name = "tb_folder_path";
            this.tb_folder_path.Size = new System.Drawing.Size(478, 20);
            this.tb_folder_path.TabIndex = 15;
            this.tb_folder_path.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_folder_path_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Path to the folder to pack:";
            // 
            // btn_browse
            // 
            this.btn_browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browse.Location = new System.Drawing.Point(632, 6);
            this.btn_browse.Name = "btn_browse";
            this.btn_browse.Size = new System.Drawing.Size(75, 20);
            this.btn_browse.TabIndex = 17;
            this.btn_browse.Text = "Browse...";
            this.btn_browse.UseVisualStyleBackColor = true;
            this.btn_browse.Click += new System.EventHandler(this.btn_browse_Click);
            // 
            // fbd_pack_folder
            // 
            this.fbd_pack_folder.Tag = "";
            // 
            // btn_refresh
            // 
            this.btn_refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_refresh.Location = new System.Drawing.Point(713, 6);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(75, 20);
            this.btn_refresh.TabIndex = 18;
            this.btn_refresh.Text = "Refresh List";
            this.btn_refresh.UseVisualStyleBackColor = true;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // btn_filter
            // 
            this.btn_filter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_filter.Location = new System.Drawing.Point(713, 32);
            this.btn_filter.Name = "btn_filter";
            this.btn_filter.Size = new System.Drawing.Size(75, 20);
            this.btn_filter.TabIndex = 18;
            this.btn_filter.Text = "Filter";
            this.btn_filter.UseVisualStyleBackColor = true;
            this.btn_filter.Click += new System.EventHandler(this.btn_filter_Click);
            // 
            // btn_match_case
            // 
            this.btn_match_case.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_match_case.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_match_case.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_match_case.Location = new System.Drawing.Point(388, 30);
            this.btn_match_case.Name = "btn_match_case";
            this.btn_match_case.Size = new System.Drawing.Size(30, 24);
            this.btn_match_case.TabIndex = 21;
            this.btn_match_case.Text = "Aa";
            this.btn_match_case.UseVisualStyleBackColor = true;
            this.btn_match_case.Click += new System.EventHandler(this.btn_match_case_Click);
            // 
            // l_total_count
            // 
            this.l_total_count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.l_total_count.AutoSize = true;
            this.l_total_count.Location = new System.Drawing.Point(12, 439);
            this.l_total_count.Name = "l_total_count";
            this.l_total_count.Size = new System.Drawing.Size(59, 13);
            this.l_total_count.TabIndex = 22;
            this.l_total_count.Text = "Files Count";
            // 
            // searchText
            // 
            this.searchText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchText.CueBanner = "Filter text (? and * allowed)";
            this.searchText.Location = new System.Drawing.Point(424, 32);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(283, 20);
            this.searchText.TabIndex = 20;
            this.toolTip1.SetToolTip(this.searchText, resources.GetString("searchText.ToolTip"));
            this.searchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxWithPlaceholder1_KeyDown);
            // 
            // nud_alignment
            // 
            this.nud_alignment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nud_alignment.Location = new System.Drawing.Point(502, 434);
            this.nud_alignment.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nud_alignment.Name = "nud_alignment";
            this.nud_alignment.Size = new System.Drawing.Size(38, 20);
            this.nud_alignment.TabIndex = 23;
            this.nud_alignment.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 436);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Alignment between files inside PCK";
            // 
            // btn_generate_key
            // 
            this.btn_generate_key.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_generate_key.Location = new System.Drawing.Point(634, 409);
            this.btn_generate_key.Name = "btn_generate_key";
            this.btn_generate_key.Size = new System.Drawing.Size(82, 23);
            this.btn_generate_key.TabIndex = 27;
            this.btn_generate_key.Text = "Encryption";
            this.btn_generate_key.UseVisualStyleBackColor = true;
            this.btn_generate_key.Click += new System.EventHandler(this.btn_generate_key_Click);
            // 
            // cb_enable_encryption
            // 
            this.cb_enable_encryption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_enable_encryption.AutoSize = true;
            this.cb_enable_encryption.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_enable_encryption.Location = new System.Drawing.Point(517, 413);
            this.cb_enable_encryption.Name = "cb_enable_encryption";
            this.cb_enable_encryption.Size = new System.Drawing.Size(111, 17);
            this.cb_enable_encryption.TabIndex = 28;
            this.cb_enable_encryption.Text = "Enable encryption";
            this.cb_enable_encryption.UseVisualStyleBackColor = true;
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.size.DefaultCellStyle = dataGridViewCellStyle1;
            this.size.FillWeight = 15F;
            this.size.HeaderText = "Size";
            this.size.Name = "size";
            this.size.ReadOnly = true;
            // 
            // CreatePCKFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 484);
            this.Controls.Add(this.cb_enable_encryption);
            this.Controls.Add(this.btn_generate_key);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nud_alignment);
            this.Controls.Add(this.l_total_count);
            this.Controls.Add(this.btn_match_case);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.btn_filter);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.btn_browse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_folder_path);
            this.Controls.Add(this.cb_embed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_ver);
            this.Controls.Add(this.nud_revision);
            this.Controls.Add(this.nud_minor);
            this.Controls.Add(this.nud_major);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.l_total_size);
            this.Controls.Add(this.btn_create);
            this.Controls.Add(this.dataGridView1);
            this.MinimumSize = new System.Drawing.Size(816, 489);
            this.Name = "CreatePCKFile";
            this.Text = "Pack or Embed folder";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_major)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_minor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_revision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_alignment)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button btn_create;
		private System.Windows.Forms.Label l_total_size;
		private System.Windows.Forms.SaveFileDialog sfd_save_pack;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nud_major;
		private System.Windows.Forms.NumericUpDown nud_minor;
		private System.Windows.Forms.NumericUpDown nud_revision;
		private System.Windows.Forms.ComboBox cb_ver;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cb_embed;
        private System.Windows.Forms.OpenFileDialog ofd_pack_into;
        private System.Windows.Forms.TextBox tb_folder_path;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_browse;
        private System.Windows.Forms.FolderBrowserDialog fbd_pack_folder;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.Button btn_filter;
        private TextBoxWithPlaceholder searchText;
        private System.Windows.Forms.Button btn_match_case;
        private System.Windows.Forms.Label l_total_count;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown nud_alignment;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_generate_key;
        private System.Windows.Forms.CheckBox cb_enable_encryption;
        private System.Windows.Forms.DataGridViewTextBoxColumn filePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn size;
    }
}