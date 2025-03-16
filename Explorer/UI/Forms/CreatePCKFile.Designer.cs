namespace GodotPCKExplorer.UI
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatePCKFile));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            btn_create = new Button();
            l_total_size = new Label();
            sfd_save_pack = new SaveFileDialog();
            label1 = new Label();
            nud_major = new NumericUpDown();
            nud_minor = new NumericUpDown();
            nud_revision = new NumericUpDown();
            cb_ver = new ComboBox();
            label2 = new Label();
            cb_embed = new CheckBox();
            ofd_pack_into = new OpenFileDialog();
            tb_folder_path = new TextBox();
            label3 = new Label();
            btn_browse = new Button();
            fbd_pack_folder = new FolderBrowserDialog();
            btn_refresh = new Button();
            btn_filter = new Button();
            btn_match_case = new Button();
            l_total_count = new Label();
            searchText = new TextBoxWithPlaceholder();
            toolTip1 = new ToolTip(components);
            btn_clearFilter = new Button();
            cb_enable_encryption = new CheckBox();
            nud_alignment = new NumericUpDown();
            label4 = new Label();
            btn_generate_key = new Button();
            cb_packFiltered = new CheckBox();
            cb_previewPaths = new CheckBox();
            label5 = new Label();
            tb_prefix = new TextBox();
            label6 = new Label();
            tb_patch_target = new TextBox();
            cb_enable_patching = new CheckBox();
            btn_browse_patch_target = new Button();
            ofd_patch_target = new OpenFileDialog();
            filePath = new DataGridViewTextBoxColumn();
            size = new DataGridViewTextBoxColumn();
            patch = new DataGridViewTextBoxColumn();
            removal = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_major).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_minor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_revision).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_alignment).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { filePath, size, patch, removal });
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.Location = new Point(0, 138);
            dataGridView1.Margin = new Padding(0, 3, 0, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(850, 307);
            dataGridView1.TabIndex = 0;
            dataGridView1.SortCompare += dataGridView1_SortCompare;
            dataGridView1.UserDeletedRow += dataGridView1_UserDeletedRow;
            // 
            // btn_create
            // 
            btn_create.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_create.Font = new Font("Microsoft Sans Serif", 12F);
            btn_create.Location = new Point(760, 452);
            btn_create.Margin = new Padding(4, 3, 4, 3);
            btn_create.Name = "btn_create";
            btn_create.Size = new Size(88, 79);
            btn_create.TabIndex = 1;
            btn_create.Text = "Pack";
            btn_create.UseVisualStyleBackColor = true;
            btn_create.Click += btn_create_Click;
            // 
            // l_total_size
            // 
            l_total_size.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            l_total_size.AutoSize = true;
            l_total_size.Location = new Point(14, 513);
            l_total_size.Margin = new Padding(4, 0, 4, 0);
            l_total_size.Name = "l_total_size";
            l_total_size.Size = new Size(55, 15);
            l_total_size.TabIndex = 2;
            l_total_size.Text = "Total Size";
            // 
            // sfd_save_pack
            // 
            sfd_save_pack.DefaultExt = "pck";
            sfd_save_pack.Filter = "Godot PCK|*.pck";
            sfd_save_pack.Title = "Select the path to the new '.pck' file";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(165, 512);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(369, 15);
            label1.TabIndex = 7;
            label1.Text = "Godot PCK version and Godot Engine version(major, minor, revision)";
            // 
            // nud_major
            // 
            nud_major.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            nud_major.Location = new Point(605, 508);
            nud_major.Margin = new Padding(4, 3, 4, 3);
            nud_major.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_major.Name = "nud_major";
            nud_major.Size = new Size(44, 23);
            nud_major.TabIndex = 8;
            nud_major.Value = new decimal(new int[] { 4, 0, 0, 0 });
            nud_major.ValueChanged += nud_major_ValueChanged;
            // 
            // nud_minor
            // 
            nud_minor.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            nud_minor.Location = new Point(657, 508);
            nud_minor.Margin = new Padding(4, 3, 4, 3);
            nud_minor.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_minor.Name = "nud_minor";
            nud_minor.Size = new Size(44, 23);
            nud_minor.TabIndex = 9;
            nud_minor.ValueChanged += nud_minor_ValueChanged;
            // 
            // nud_revision
            // 
            nud_revision.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            nud_revision.Location = new Point(708, 508);
            nud_revision.Margin = new Padding(4, 3, 4, 3);
            nud_revision.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_revision.Name = "nud_revision";
            nud_revision.Size = new Size(44, 23);
            nud_revision.TabIndex = 10;
            nud_revision.ValueChanged += nud_revision_ValueChanged;
            // 
            // cb_ver
            // 
            cb_ver.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cb_ver.FormattingEnabled = true;
            cb_ver.Items.AddRange(new object[] { "1", "2" });
            cb_ver.Location = new Point(553, 508);
            cb_ver.Margin = new Padding(4, 3, 4, 3);
            cb_ver.MaxLength = 2;
            cb_ver.Name = "cb_ver";
            cb_ver.Size = new Size(44, 23);
            cb_ver.TabIndex = 12;
            cb_ver.Text = "2";
            cb_ver.SelectionChangeCommitted += cb_ver_SelectionChangeCommitted;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 120);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(632, 15);
            label2.TabIndex = 13;
            label2.Text = "*You can delete files from the table by selecting them and pressing Delete. To select several separate rows, hold Control";
            // 
            // cb_embed
            // 
            cb_embed.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cb_embed.AutoSize = true;
            cb_embed.CheckAlign = ContentAlignment.MiddleRight;
            cb_embed.Location = new Point(561, 483);
            cb_embed.Margin = new Padding(4, 3, 4, 3);
            cb_embed.Name = "cb_embed";
            cb_embed.Size = new Size(191, 19);
            cb_embed.TabIndex = 14;
            cb_embed.Text = "Embed .pck into executable file";
            cb_embed.UseVisualStyleBackColor = true;
            // 
            // ofd_pack_into
            // 
            ofd_pack_into.Title = "Select the file to embed the package into";
            // 
            // tb_folder_path
            // 
            tb_folder_path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_folder_path.Location = new Point(166, 7);
            tb_folder_path.Margin = new Padding(4, 3, 4, 3);
            tb_folder_path.Name = "tb_folder_path";
            tb_folder_path.Size = new Size(480, 23);
            tb_folder_path.TabIndex = 15;
            tb_folder_path.KeyDown += tb_folder_path_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 10);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(144, 15);
            label3.TabIndex = 16;
            label3.Text = "Path to the folder to pack:";
            // 
            // btn_browse
            // 
            btn_browse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_browse.Location = new Point(657, 6);
            btn_browse.Margin = new Padding(4, 3, 4, 3);
            btn_browse.Name = "btn_browse";
            btn_browse.Size = new Size(88, 23);
            btn_browse.TabIndex = 17;
            btn_browse.Text = "Browse...";
            btn_browse.UseVisualStyleBackColor = true;
            btn_browse.Click += btn_browse_Click;
            // 
            // fbd_pack_folder
            // 
            fbd_pack_folder.Tag = "";
            // 
            // btn_refresh
            // 
            btn_refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_refresh.Location = new Point(749, 7);
            btn_refresh.Margin = new Padding(4, 3, 4, 3);
            btn_refresh.Name = "btn_refresh";
            btn_refresh.Size = new Size(88, 23);
            btn_refresh.TabIndex = 18;
            btn_refresh.Text = "Refresh List";
            btn_refresh.UseVisualStyleBackColor = true;
            btn_refresh.Click += btn_refresh_Click;
            // 
            // btn_filter
            // 
            btn_filter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_filter.Location = new Point(749, 36);
            btn_filter.Margin = new Padding(4, 3, 4, 3);
            btn_filter.Name = "btn_filter";
            btn_filter.Size = new Size(88, 23);
            btn_filter.TabIndex = 18;
            btn_filter.Text = "Filter";
            btn_filter.UseVisualStyleBackColor = true;
            btn_filter.Click += btn_filter_Click;
            // 
            // btn_match_case
            // 
            btn_match_case.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_match_case.FlatStyle = FlatStyle.Popup;
            btn_match_case.Font = new Font("Microsoft Sans Serif", 8.25F);
            btn_match_case.Location = new Point(453, 36);
            btn_match_case.Margin = new Padding(2, 3, 2, 3);
            btn_match_case.Name = "btn_match_case";
            btn_match_case.Size = new Size(30, 23);
            btn_match_case.TabIndex = 21;
            btn_match_case.Text = "Aa";
            btn_match_case.UseVisualStyleBackColor = true;
            btn_match_case.Click += btn_match_case_Click;
            // 
            // l_total_count
            // 
            l_total_count.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            l_total_count.AutoSize = true;
            l_total_count.Location = new Point(14, 487);
            l_total_count.Margin = new Padding(4, 0, 4, 0);
            l_total_count.Name = "l_total_count";
            l_total_count.Size = new Size(66, 15);
            l_total_count.TabIndex = 22;
            l_total_count.Text = "Files Count";
            // 
            // searchText
            // 
            searchText.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            searchText.CueBanner = "Filter text (? and * allowed)";
            searchText.Location = new Point(487, 36);
            searchText.Margin = new Padding(2, 3, 0, 3);
            searchText.Name = "searchText";
            searchText.Size = new Size(239, 23);
            searchText.TabIndex = 20;
            toolTip1.SetToolTip(searchText, resources.GetString("searchText.ToolTip"));
            searchText.KeyDown += textBoxWithPlaceholder1_KeyDown;
            // 
            // btn_clearFilter
            // 
            btn_clearFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_clearFilter.Location = new Point(726, 36);
            btn_clearFilter.Margin = new Padding(0, 0, 2, 0);
            btn_clearFilter.Name = "btn_clearFilter";
            btn_clearFilter.Size = new Size(19, 23);
            btn_clearFilter.TabIndex = 29;
            btn_clearFilter.Text = "X";
            toolTip1.SetToolTip(btn_clearFilter, "Clear filter");
            btn_clearFilter.UseVisualStyleBackColor = true;
            btn_clearFilter.Click += btn_clearFilter_Click;
            // 
            // cb_enable_encryption
            // 
            cb_enable_encryption.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cb_enable_encryption.AutoSize = true;
            cb_enable_encryption.CheckAlign = ContentAlignment.MiddleRight;
            cb_enable_encryption.Location = new Point(529, 456);
            cb_enable_encryption.Margin = new Padding(4, 3, 4, 3);
            cb_enable_encryption.Name = "cb_enable_encryption";
            cb_enable_encryption.Size = new Size(121, 19);
            cb_enable_encryption.TabIndex = 28;
            cb_enable_encryption.Text = "Enable encryption";
            toolTip1.SetToolTip(cb_enable_encryption, "When patching, encryption is applied only to files that will be added to the PCK, and not copied from another Pack.");
            cb_enable_encryption.UseVisualStyleBackColor = true;
            // 
            // nud_alignment
            // 
            nud_alignment.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            nud_alignment.Location = new Point(503, 481);
            nud_alignment.Margin = new Padding(4, 3, 4, 3);
            nud_alignment.Maximum = new decimal(new int[] { 512, 0, 0, 0 });
            nud_alignment.Name = "nud_alignment";
            nud_alignment.Size = new Size(44, 23);
            nud_alignment.TabIndex = 23;
            nud_alignment.Value = new decimal(new int[] { 512, 0, 0, 0 });
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(301, 483);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(194, 15);
            label4.TabIndex = 24;
            label4.Text = "Alignment between files inside PCK";
            // 
            // btn_generate_key
            // 
            btn_generate_key.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_generate_key.Location = new Point(657, 452);
            btn_generate_key.Margin = new Padding(4, 3, 4, 3);
            btn_generate_key.Name = "btn_generate_key";
            btn_generate_key.Size = new Size(96, 27);
            btn_generate_key.TabIndex = 27;
            btn_generate_key.Text = "Encryption";
            btn_generate_key.UseVisualStyleBackColor = true;
            btn_generate_key.Click += btn_generate_key_Click;
            // 
            // cb_packFiltered
            // 
            cb_packFiltered.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cb_packFiltered.AutoSize = true;
            cb_packFiltered.CheckAlign = ContentAlignment.MiddleRight;
            cb_packFiltered.Location = new Point(404, 456);
            cb_packFiltered.Margin = new Padding(4, 3, 4, 3);
            cb_packFiltered.Name = "cb_packFiltered";
            cb_packFiltered.Size = new Size(117, 19);
            cb_packFiltered.TabIndex = 28;
            cb_packFiltered.Text = "Pack only filtered";
            cb_packFiltered.UseVisualStyleBackColor = true;
            // 
            // cb_previewPaths
            // 
            cb_previewPaths.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cb_previewPaths.AutoSize = true;
            cb_previewPaths.CheckAlign = ContentAlignment.MiddleRight;
            cb_previewPaths.Location = new Point(14, 456);
            cb_previewPaths.Margin = new Padding(4, 3, 4, 3);
            cb_previewPaths.Name = "cb_previewPaths";
            cb_previewPaths.Size = new Size(99, 19);
            cb_previewPaths.TabIndex = 28;
            cb_previewPaths.Text = "Preview paths";
            cb_previewPaths.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(14, 39);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(108, 15);
            label5.TabIndex = 16;
            label5.Text = "Path prefix in pack:";
            // 
            // tb_prefix
            // 
            tb_prefix.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_prefix.Location = new Point(166, 36);
            tb_prefix.Margin = new Padding(4, 3, 20, 3);
            tb_prefix.Name = "tb_prefix";
            tb_prefix.Size = new Size(265, 23);
            tb_prefix.TabIndex = 15;
            tb_prefix.KeyDown += tb_prefix_KeyDown;
            tb_prefix.Leave += tb_prefix_Leave;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(14, 68);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(57, 15);
            label6.TabIndex = 31;
            label6.Text = "Patching:";
            // 
            // tb_patch_target
            // 
            tb_patch_target.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_patch_target.Location = new Point(165, 94);
            tb_patch_target.Margin = new Padding(4, 3, 4, 3);
            tb_patch_target.Name = "tb_patch_target";
            tb_patch_target.ReadOnly = true;
            tb_patch_target.Size = new Size(480, 23);
            tb_patch_target.TabIndex = 33;
            // 
            // cb_enable_patching
            // 
            cb_enable_patching.AutoSize = true;
            cb_enable_patching.CheckAlign = ContentAlignment.MiddleRight;
            cb_enable_patching.Location = new Point(14, 97);
            cb_enable_patching.Margin = new Padding(4, 3, 4, 3);
            cb_enable_patching.Name = "cb_enable_patching";
            cb_enable_patching.Size = new Size(118, 19);
            cb_enable_patching.TabIndex = 34;
            cb_enable_patching.Text = "Enabled patching";
            cb_enable_patching.UseVisualStyleBackColor = true;
            // 
            // btn_browse_patch_target
            // 
            btn_browse_patch_target.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_browse_patch_target.Location = new Point(657, 94);
            btn_browse_patch_target.Margin = new Padding(4, 3, 4, 3);
            btn_browse_patch_target.Name = "btn_browse_patch_target";
            btn_browse_patch_target.Size = new Size(88, 23);
            btn_browse_patch_target.TabIndex = 17;
            btn_browse_patch_target.Text = "Browse...";
            btn_browse_patch_target.UseVisualStyleBackColor = true;
            btn_browse_patch_target.Click += btn_browse_patch_target_Click;
            // 
            // ofd_patch_target
            // 
            ofd_patch_target.Title = "Select the file containing .pck";
            // 
            // filePath
            // 
            filePath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            filePath.FillWeight = 85F;
            filePath.HeaderText = "File Path";
            filePath.Name = "filePath";
            filePath.ReadOnly = true;
            // 
            // size
            // 
            size.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            size.DefaultCellStyle = dataGridViewCellStyle1;
            size.HeaderText = "Size";
            size.Name = "size";
            size.ReadOnly = true;
            size.Width = 52;
            // 
            // patch
            // 
            patch.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            patch.DefaultCellStyle = dataGridViewCellStyle2;
            patch.HeaderText = "Patch";
            patch.MaxInputLength = 1;
            patch.MinimumWidth = 30;
            patch.Name = "patch";
            patch.Width = 30;
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
            removal.Width = 30;
            // 
            // CreatePCKFile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(850, 538);
            Controls.Add(cb_enable_patching);
            Controls.Add(tb_patch_target);
            Controls.Add(label6);
            Controls.Add(btn_clearFilter);
            Controls.Add(cb_previewPaths);
            Controls.Add(cb_packFiltered);
            Controls.Add(cb_enable_encryption);
            Controls.Add(btn_generate_key);
            Controls.Add(label4);
            Controls.Add(nud_alignment);
            Controls.Add(l_total_count);
            Controls.Add(btn_match_case);
            Controls.Add(searchText);
            Controls.Add(btn_filter);
            Controls.Add(btn_refresh);
            Controls.Add(btn_browse_patch_target);
            Controls.Add(btn_browse);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(tb_prefix);
            Controls.Add(tb_folder_path);
            Controls.Add(cb_embed);
            Controls.Add(label2);
            Controls.Add(cb_ver);
            Controls.Add(nud_revision);
            Controls.Add(nud_minor);
            Controls.Add(nud_major);
            Controls.Add(label1);
            Controls.Add(l_total_size);
            Controls.Add(btn_create);
            Controls.Add(dataGridView1);
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(866, 350);
            Name = "CreatePCKFile";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pack or Embed folder";
            FormClosed += CreatePCKFile_FormClosed;
            Shown += CreatePCKFile_Shown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_major).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_minor).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_revision).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_alignment).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button btn_create;
        private Label l_total_size;
        private SaveFileDialog sfd_save_pack;
        private Label label1;
        private NumericUpDown nud_major;
        private NumericUpDown nud_minor;
        private NumericUpDown nud_revision;
        private ComboBox cb_ver;
        private Label label2;
        private CheckBox cb_embed;
        private OpenFileDialog ofd_pack_into;
        private TextBox tb_folder_path;
        private Label label3;
        private Button btn_browse;
        private FolderBrowserDialog fbd_pack_folder;
        private Button btn_refresh;
        private Button btn_filter;
        private TextBoxWithPlaceholder searchText;
        private Button btn_match_case;
        private Label l_total_count;
        private ToolTip toolTip1;
        private NumericUpDown nud_alignment;
        private Label label4;
        private Button btn_generate_key;
        private CheckBox cb_enable_encryption;
        private Button btn_clearFilter;
        private CheckBox cb_packFiltered;
        private CheckBox cb_previewPaths;
        private Label label5;
        private TextBox tb_prefix;
        private Label label6;
        private TextBox tb_patch_target;
        private CheckBox cb_enable_patching;
        private Button btn_browse_patch_target;
        private OpenFileDialog ofd_patch_target;
        private DataGridViewTextBoxColumn filePath;
        private DataGridViewTextBoxColumn size;
        private DataGridViewTextBoxColumn patch;
        private DataGridViewTextBoxColumn removal;
    }
}