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
            label1 = new Label();
            cb_encrypt_index = new CheckBox();
            cb_encrypt_files = new CheckBox();
            btn_ok = new Button();
            tb_key = new TextBoxWithPlaceholder();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 15);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(213, 15);
            label1.TabIndex = 0;
            label1.Text = "256-bit AES key in hexadecimal format:";
            // 
            // cb_encrypt_index
            // 
            cb_encrypt_index.AutoSize = true;
            cb_encrypt_index.Location = new Point(14, 63);
            cb_encrypt_index.Margin = new Padding(4, 3, 4, 3);
            cb_encrypt_index.Name = "cb_encrypt_index";
            cb_encrypt_index.Size = new Size(160, 19);
            cb_encrypt_index.TabIndex = 2;
            cb_encrypt_index.Text = "Encrypt Index ( files info )";
            cb_encrypt_index.UseVisualStyleBackColor = true;
            // 
            // cb_encrypt_files
            // 
            cb_encrypt_files.AutoSize = true;
            cb_encrypt_files.Location = new Point(14, 90);
            cb_encrypt_files.Margin = new Padding(4, 3, 4, 3);
            cb_encrypt_files.Name = "cb_encrypt_files";
            cb_encrypt_files.Size = new Size(90, 19);
            cb_encrypt_files.TabIndex = 3;
            cb_encrypt_files.Text = "Encrypt files";
            cb_encrypt_files.UseVisualStyleBackColor = true;
            // 
            // btn_ok
            // 
            btn_ok.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_ok.Location = new Point(14, 117);
            btn_ok.Margin = new Padding(4, 3, 4, 3);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(552, 27);
            btn_ok.TabIndex = 4;
            btn_ok.Text = "Apply";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // tb_key
            // 
            tb_key.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_key.CueBanner = "04bf1d.., 04 BF 1D.., 04-BF-1D..";
            tb_key.Location = new Point(14, 33);
            tb_key.Margin = new Padding(4, 3, 4, 3);
            tb_key.Name = "tb_key";
            tb_key.Size = new Size(474, 23);
            tb_key.TabIndex = 5;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(496, 31);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(70, 27);
            button1.TabIndex = 6;
            button1.Text = "Generate";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // CreatePCKEncryption
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(580, 150);
            Controls.Add(button1);
            Controls.Add(tb_key);
            Controls.Add(btn_ok);
            Controls.Add(cb_encrypt_files);
            Controls.Add(cb_encrypt_index);
            Controls.Add(label1);
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(1192, 189);
            MinimumSize = new Size(292, 189);
            Name = "CreatePCKEncryption";
            Text = "Encryption Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private CheckBox cb_encrypt_index;
        private CheckBox cb_encrypt_files;
        private Button btn_ok;
        private TextBoxWithPlaceholder tb_key;
        private Button button1;
    }
}