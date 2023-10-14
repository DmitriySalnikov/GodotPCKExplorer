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
            // btn_ok
            // 
            btn_ok.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_ok.Location = new Point(14, 63);
            btn_ok.Margin = new Padding(4, 3, 4, 3);
            btn_ok.Name = "btn_ok";
            btn_ok.Size = new Size(420, 27);
            btn_ok.TabIndex = 4;
            btn_ok.Text = "Confirm";
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
            tb_key.Size = new Size(419, 23);
            tb_key.TabIndex = 5;
            // 
            // OpenWithPCKEncryption
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(448, 96);
            Controls.Add(tb_key);
            Controls.Add(btn_ok);
            Controls.Add(label1);
            Margin = new Padding(4, 3, 4, 3);
            MaximumSize = new Size(1192, 135);
            MinimumSize = new Size(338, 135);
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
    }
}