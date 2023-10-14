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
            ((System.ComponentModel.ISupportInitialize)nud_revision).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_minor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nud_major).BeginInit();
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
            cb_ver.Size = new Size(81, 23);
            cb_ver.TabIndex = 17;
            cb_ver.Text = "2";
            // 
            // nud_revision
            // 
            nud_revision.Location = new Point(299, 107);
            nud_revision.Margin = new Padding(4, 3, 4, 3);
            nud_revision.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_revision.Name = "nud_revision";
            nud_revision.Size = new Size(78, 23);
            nud_revision.TabIndex = 16;
            // 
            // nud_minor
            // 
            nud_minor.Location = new Point(212, 107);
            nud_minor.Margin = new Padding(4, 3, 4, 3);
            nud_minor.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_minor.Name = "nud_minor";
            nud_minor.Size = new Size(76, 23);
            nud_minor.TabIndex = 15;
            // 
            // nud_major
            // 
            nud_major.Location = new Point(126, 107);
            nud_major.Margin = new Padding(4, 3, 4, 3);
            nud_major.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            nud_major.Name = "nud_major";
            nud_major.Size = new Size(76, 23);
            nud_major.TabIndex = 14;
            nud_major.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 89);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 13;
            label1.Text = "Pack Version:";
            // 
            // l_path
            // 
            l_path.AutoSize = true;
            l_path.Location = new Point(14, 10);
            l_path.Margin = new Padding(4, 0, 4, 0);
            l_path.Name = "l_path";
            l_path.Size = new Size(378, 30);
            l_path.TabIndex = 18;
            l_path.Text = "File path:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(122, 89);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(77, 15);
            label2.TabIndex = 19;
            label2.Text = "Godot Major:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(209, 89);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 20;
            label3.Text = "Godot Minor:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(295, 89);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(76, 15);
            label4.TabIndex = 21;
            label4.Text = "Godot Patch:";
            // 
            // l_version
            // 
            l_version.AutoSize = true;
            l_version.Location = new Point(14, 45);
            l_version.Margin = new Padding(4, 0, 4, 0);
            l_version.Name = "l_version";
            l_version.Size = new Size(378, 30);
            l_version.TabIndex = 22;
            l_version.Text = "Original Version:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // btn_ok
            // 
            btn_ok.Location = new Point(145, 137);
            btn_ok.Margin = new Padding(4, 3, 4, 3);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(127, 27);
            btn_ok.TabIndex = 23;
            btn_ok.Text = "Change Version";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // ChangePCKVersion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(407, 178);
            Controls.Add(btn_ok);
            Controls.Add(l_version);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(l_path);
            Controls.Add(cb_ver);
            Controls.Add(nud_revision);
            Controls.Add(nud_minor);
            Controls.Add(nud_major);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(423, 217);
            MinimumSize = new Size(423, 217);
            Name = "ChangePCKVersion";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Change PCK Version";
            ((System.ComponentModel.ISupportInitialize)nud_revision).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_minor).EndInit();
            ((System.ComponentModel.ISupportInitialize)nud_major).EndInit();
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
    }
}