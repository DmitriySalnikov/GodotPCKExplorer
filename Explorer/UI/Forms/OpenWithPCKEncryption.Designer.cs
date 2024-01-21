namespace GodotPCKExplorer.UI
{
    partial class OpenWithPCKEncryption
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
            btn_ok = new Button();
            tb_key = new TextBoxWithPlaceholder();
            linkLabel1 = new LinkLabel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(213, 15);
            label1.TabIndex = 0;
            label1.Text = "256-bit AES key in hexadecimal format:";
            // 
            // btn_ok
            // 
            btn_ok.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btn_ok.Location = new Point(12, 73);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(455, 27);
            btn_ok.TabIndex = 4;
            btn_ok.Text = "Confirm";
            btn_ok.UseVisualStyleBackColor = true;
            btn_ok.Click += btn_ok_Click;
            // 
            // tb_key
            // 
            tb_key.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_key.CueBanner = "04bf1d.., 04 BF 1D.., 04-BF-1D..";
            tb_key.Location = new Point(12, 27);
            tb_key.Name = "tb_key";
            tb_key.Size = new Size(455, 23);
            tb_key.TabIndex = 5;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(225, 55);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(242, 15);
            linkLabel1.TabIndex = 6;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Don't know the key? Try the PCK Bruteforcer.";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // OpenWithPCKEncryption
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(479, 106);
            Controls.Add(linkLabel1);
            Controls.Add(tb_key);
            Controls.Add(btn_ok);
            Controls.Add(label1);
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(1192, 145);
            MinimumSize = new Size(338, 145);
            Name = "OpenWithPCKEncryption";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter the encryption key";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btn_ok;
        private TextBoxWithPlaceholder tb_key;
        private LinkLabel linkLabel1;
    }
}