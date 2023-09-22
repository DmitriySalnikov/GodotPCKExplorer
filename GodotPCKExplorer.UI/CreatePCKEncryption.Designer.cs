namespace GodotPCKExplorer.UI
{
    partial class CreatePCKEncryption
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
            this.label1 = new System.Windows.Forms.Label();
            this.cb_encrypt_index = new System.Windows.Forms.CheckBox();
            this.cb_encrypt_files = new System.Windows.Forms.CheckBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.tb_key = new TextBoxWithPlaceholder();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "256-bit AES key in hexadecimal format:";
            // 
            // cb_encrypt_index
            // 
            this.cb_encrypt_index.AutoSize = true;
            this.cb_encrypt_index.Location = new System.Drawing.Point(12, 55);
            this.cb_encrypt_index.Name = "cb_encrypt_index";
            this.cb_encrypt_index.Size = new System.Drawing.Size(138, 17);
            this.cb_encrypt_index.TabIndex = 2;
            this.cb_encrypt_index.Text = "Encrypt Index (files info)";
            this.cb_encrypt_index.UseVisualStyleBackColor = true;
            // 
            // cb_encrypt_files
            // 
            this.cb_encrypt_files.AutoSize = true;
            this.cb_encrypt_files.Location = new System.Drawing.Point(12, 78);
            this.cb_encrypt_files.Name = "cb_encrypt_files";
            this.cb_encrypt_files.Size = new System.Drawing.Size(83, 17);
            this.cb_encrypt_files.TabIndex = 3;
            this.cb_encrypt_files.Text = "Encrypt files";
            this.cb_encrypt_files.UseVisualStyleBackColor = true;
            // 
            // btn_ok
            // 
            this.btn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ok.Location = new System.Drawing.Point(12, 101);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(360, 23);
            this.btn_ok.TabIndex = 4;
            this.btn_ok.Text = "Apply";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // tb_key
            // 
            this.tb_key.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_key.CueBanner = "04bf1d.., 04 BF 1D.., 04-BF-1D..";
            this.tb_key.Location = new System.Drawing.Point(12, 29);
            this.tb_key.Name = "tb_key";
            this.tb_key.Size = new System.Drawing.Size(294, 20);
            this.tb_key.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(312, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Generate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CreatePCKEncryption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 130);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tb_key);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.cb_encrypt_files);
            this.Controls.Add(this.cb_encrypt_index);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(1024, 169);
            this.MinimumSize = new System.Drawing.Size(292, 169);
            this.Name = "CreatePCKEncryption";
            this.Text = "Encryption Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_encrypt_index;
        private System.Windows.Forms.CheckBox cb_encrypt_files;
        private System.Windows.Forms.Button btn_ok;
        private TextBoxWithPlaceholder tb_key;
        private System.Windows.Forms.Button button1;
    }
}