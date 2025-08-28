namespace GodotPCKExplorer.UI
{
    partial class ChangePCKVersion
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
            cb_ver = new ComboBox();
            nud_revision = new NumericUpDown();
            nud_minor = new NumericUpDown();
            nud_major = new NumericUpDown();
            label1 = new Label();
            l_path = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            l_version = new Label();
            btn_ok = new Button();
            label5 = new Label();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            ((ISupportInitialize)nud_revision).BeginInit();
            ((ISupportInitialize)nud_minor).BeginInit();
            ((ISupportInitialize)nud_major).BeginInit();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // cb_ver
            // 
            cb_ver.FormattingEnabled = true;
            cb_ver.Items.AddRange(new object[] { "1", "2" });
            cb_ver.Location = new Point(34, 106);
            cb_ver.Margin = new Padding(4, 3, 4, 3);
            cb_ver.MaxLength = 2;
            cb_ver.Name = "cb_ver";
            cb_ver.Size = new Size(79, 23);
            cb_ver.TabIndex = 17;
            cb_ver.Text = "2";
            // 
            // nud_revision
            // 
            nud_revision.Location = new Point(264, 17);
            nud_revision.Margin = new Padding(4, 3, 4, 3);
            nud_revision.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_revision.Name = "nud_revision";
            nud_revision.Size = new Size(79, 23);
            nud_revision.TabIndex = 16;
            // 
            // nud_minor
            // 
            nud_minor.Location = new Point(177, 16);
            nud_minor.Margin = new Padding(4, 3, 4, 3);
            nud_minor.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_minor.Name = "nud_minor";
            nud_minor.Size = new Size(79, 23);
            nud_minor.TabIndex = 15;
            // 
            // nud_major
            // 
            nud_major.Location = new Point(91, 18);
            nud_major.Margin = new Padding(4, 3, 4, 3);
            nud_major.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_major.Name = "nud_major";
            nud_major.Size = new Size(78, 23);
            nud_major.TabIndex = 14;
            nud_major.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 13;
            label1.Text = "Pack Version:";
            // 
            // l_path
            // 
            l_path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            l_path.AutoSize = true;
            l_path.Location = new Point(13, 48);
            l_path.Margin = new Padding(4, 0, 4, 0);
            l_path.Name = "l_path";
            l_path.Size = new Size(378, 30);
            l_path.TabIndex = 18;
            l_path.Text = "File path:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(91, 0);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(77, 15);
            label2.TabIndex = 19;
            label2.Text = "Godot Major:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(178, 0);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 20;
            label3.Text = "Godot Minor:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(264, 0);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(76, 15);
            label4.TabIndex = 21;
            label4.Text = "Godot Patch:";
            // 
            // l_version
            // 
            l_version.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            l_version.AutoSize = true;
            l_version.Location = new Point(13, 78);
            l_version.Margin = new Padding(4, 0, 4, 0);
            l_version.Name = "l_version";
            l_version.Size = new Size(378, 30);
            l_version.TabIndex = 22;
            l_version.Text = "Original Version:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // btn_ok
            // 
            btn_ok.Location = new Point(110, 56);
            btn_ok.Margin = new Padding(4, 3, 4, 3);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(128, 27);
            btn_ok.TabIndex = 23;
            btn_ok.Text = "Change Version";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label5.Location = new Point(13, 9);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(491, 30);
            label5.TabIndex = 24;
            label5.Text = "Important note: Changing the version only affects the initial PCK check in Godot.\r\nAll the content remains unchanged, which can lead to errors in other versions of the engine.";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(nud_major);
            panel1.Controls.Add(btn_ok);
            panel1.Controls.Add(nud_minor);
            panel1.Controls.Add(nud_revision);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(cb_ver);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(71, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(349, 83);
            panel1.TabIndex = 25;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel1, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 114);
            tableLayoutPanel1.Margin = new Padding(3, 0, 3, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(491, 89);
            tableLayoutPanel1.TabIndex = 26;
            // 
            // ChangePCKVersion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 212);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(label5);
            Controls.Add(l_version);
            Controls.Add(l_path);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(531, 251);
            MinimumSize = new Size(531, 251);
            Name = "ChangePCKVersion";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Change PCK Version";
            ((ISupportInitialize)nud_revision).EndInit();
            ((ISupportInitialize)nud_minor).EndInit();
            ((ISupportInitialize)nud_major).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cb_ver;
        private NumericUpDown nud_revision;
        private NumericUpDown nud_minor;
        private NumericUpDown nud_major;
        private Label label1;
        private Label l_path;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label l_version;
        private Button btn_ok;
        private Label label5;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
    }
}