namespace GodotPCKExplorer.UI.UIComponents
{
    partial class PCKVersionSelector
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            nud_revision = new NumericUpDown();
            nud_minor = new NumericUpDown();
            nud_major = new NumericUpDown();
            cb_ver = new ComboBox();
            red_panel = new Panel();
            tableLayoutPanel1.SuspendLayout();
            ((ISupportInitialize)nud_revision).BeginInit();
            ((ISupportInitialize)nud_minor).BeginInit();
            ((ISupportInitialize)nud_major).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.BackColor = Color.Firebrick;
            tableLayoutPanel1.BackgroundImageLayout = ImageLayout.None;
            tableLayoutPanel1.ColumnCount = 7;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 6F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 6F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 6F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(nud_revision, 6, 0);
            tableLayoutPanel1.Controls.Add(nud_minor, 4, 0);
            tableLayoutPanel1.Controls.Add(nud_major, 2, 0);
            tableLayoutPanel1.Controls.Add(cb_ver, 0, 0);
            tableLayoutPanel1.Location = new Point(1, 1);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(305, 23);
            tableLayoutPanel1.TabIndex = 22;
            // 
            // nud_revision
            // 
            nud_revision.Dock = DockStyle.Fill;
            nud_revision.Location = new Point(231, 0);
            nud_revision.Margin = new Padding(0);
            nud_revision.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_revision.Name = "nud_revision";
            nud_revision.Size = new Size(74, 23);
            nud_revision.TabIndex = 25;
            nud_revision.ValueChanged += nud_revision_ValueChanged;
            // 
            // nud_minor
            // 
            nud_minor.Dock = DockStyle.Fill;
            nud_minor.Location = new Point(154, 0);
            nud_minor.Margin = new Padding(0);
            nud_minor.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_minor.Name = "nud_minor";
            nud_minor.Size = new Size(71, 23);
            nud_minor.TabIndex = 24;
            nud_minor.ValueChanged += nud_minor_ValueChanged;
            // 
            // nud_major
            // 
            nud_major.Dock = DockStyle.Fill;
            nud_major.Location = new Point(77, 0);
            nud_major.Margin = new Padding(0);
            nud_major.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_major.Name = "nud_major";
            nud_major.Size = new Size(71, 23);
            nud_major.TabIndex = 23;
            nud_major.Value = new decimal(new int[] { 4, 0, 0, 0 });
            nud_major.ValueChanged += nud_major_ValueChanged;
            // 
            // cb_ver
            // 
            cb_ver.Dock = DockStyle.Fill;
            cb_ver.FormattingEnabled = true;
            cb_ver.Items.AddRange(new object[] { "1" });
            cb_ver.Location = new Point(0, 0);
            cb_ver.Margin = new Padding(0);
            cb_ver.MaxLength = 2;
            cb_ver.Name = "cb_ver";
            cb_ver.Size = new Size(71, 23);
            cb_ver.TabIndex = 26;
            cb_ver.Text = "1";
            cb_ver.SelectionChangeCommitted += cb_ver_SelectionChangeCommitted;
            cb_ver.Leave += cb_ver_Leave;
            // 
            // red_panel
            // 
            red_panel.BackColor = Color.Firebrick;
            red_panel.Dock = DockStyle.Fill;
            red_panel.Location = new Point(0, 0);
            red_panel.Margin = new Padding(0);
            red_panel.Name = "red_panel";
            red_panel.Size = new Size(307, 25);
            red_panel.TabIndex = 23;
            // 
            // PCKVersionSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(red_panel);
            Name = "PCKVersionSelector";
            Size = new Size(307, 25);
            tableLayoutPanel1.ResumeLayout(false);
            ((ISupportInitialize)nud_revision).EndInit();
            ((ISupportInitialize)nud_minor).EndInit();
            ((ISupportInitialize)nud_major).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private NumericUpDown nud_revision;
        private NumericUpDown nud_minor;
        private NumericUpDown nud_major;
        private ComboBox cb_ver;
        private Panel red_panel;
    }
}
