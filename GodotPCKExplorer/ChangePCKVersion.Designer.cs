namespace GodotPCKExplorer
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
            this.cb_ver = new System.Windows.Forms.ComboBox();
            this.nud_revision = new System.Windows.Forms.NumericUpDown();
            this.nud_minor = new System.Windows.Forms.NumericUpDown();
            this.nud_major = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.l_path = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.l_version = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nud_revision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_minor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_major)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_ver
            // 
            this.cb_ver.FormattingEnabled = true;
            this.cb_ver.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cb_ver.Location = new System.Drawing.Point(29, 92);
            this.cb_ver.MaxLength = 2;
            this.cb_ver.Name = "cb_ver";
            this.cb_ver.Size = new System.Drawing.Size(70, 21);
            this.cb_ver.TabIndex = 17;
            this.cb_ver.Text = "2";
            // 
            // nud_revision
            // 
            this.nud_revision.Location = new System.Drawing.Point(256, 93);
            this.nud_revision.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_revision.Name = "nud_revision";
            this.nud_revision.Size = new System.Drawing.Size(67, 20);
            this.nud_revision.TabIndex = 16;
            // 
            // nud_minor
            // 
            this.nud_minor.Location = new System.Drawing.Point(182, 93);
            this.nud_minor.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_minor.Name = "nud_minor";
            this.nud_minor.Size = new System.Drawing.Size(65, 20);
            this.nud_minor.TabIndex = 15;
            // 
            // nud_major
            // 
            this.nud_major.Location = new System.Drawing.Point(108, 93);
            this.nud_major.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nud_major.Name = "nud_major";
            this.nud_major.Size = new System.Drawing.Size(65, 20);
            this.nud_major.TabIndex = 14;
            this.nud_major.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Pack Version:";
            // 
            // l_path
            // 
            this.l_path.AutoSize = true;
            this.l_path.Location = new System.Drawing.Point(12, 9);
            this.l_path.Name = "l_path";
            this.l_path.Size = new System.Drawing.Size(325, 26);
            this.l_path.TabIndex = 18;
            this.l_path.Text = "File path:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Godot Major:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(179, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Godot Minor:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(253, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Godot Patch:";
            // 
            // l_version
            // 
            this.l_version.AutoSize = true;
            this.l_version.Location = new System.Drawing.Point(12, 39);
            this.l_version.Name = "l_version";
            this.l_version.Size = new System.Drawing.Size(325, 26);
            this.l_version.TabIndex = 22;
            this.l_version.Text = "Original Version:\r\nddddddddddddddddddddddddddddddddddddddddddddddddddddd";
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(124, 119);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(109, 23);
            this.btn_ok.TabIndex = 23;
            this.btn_ok.Text = "Change Version";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // ChangePCKVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 154);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.l_version);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.l_path);
            this.Controls.Add(this.cb_ver);
            this.Controls.Add(this.nud_revision);
            this.Controls.Add(this.nud_minor);
            this.Controls.Add(this.nud_major);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ChangePCKVersion";
            this.Text = "Change PCK Version";
            ((System.ComponentModel.ISupportInitialize)(this.nud_revision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_minor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_major)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_ver;
        private System.Windows.Forms.NumericUpDown nud_revision;
        private System.Windows.Forms.NumericUpDown nud_minor;
        private System.Windows.Forms.NumericUpDown nud_major;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label l_path;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label l_version;
        private System.Windows.Forms.Button btn_ok;
    }
}