namespace GodotPCKExplorer.UI
{
    partial class BackgroundProgress
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
            progressBar1 = new ProgressBar();
            btn_cancel = new Button();
            l_status = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(13, 12);
            progressBar1.Margin = new Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(360, 27);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 0;
            // 
            // btn_cancel
            // 
            btn_cancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_cancel.Location = new Point(287, 45);
            btn_cancel.Margin = new Padding(4, 3, 4, 3);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(88, 35);
            btn_cancel.TabIndex = 2;
            btn_cancel.Text = "Cancel";
            btn_cancel.UseVisualStyleBackColor = true;
            btn_cancel.Click += btn_cancel_Click;
            // 
            // l_status
            // 
            l_status.AutoSize = true;
            l_status.Location = new Point(14, 47);
            l_status.Margin = new Padding(4, 0, 4, 0);
            l_status.Name = "l_status";
            l_status.Size = new Size(38, 15);
            l_status.TabIndex = 3;
            l_status.Text = "label1";
            // 
            // label1
            // 
            label1.ForeColor = SystemColors.GrayText;
            label1.Location = new Point(13, 65);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(262, 35);
            label1.TabIndex = 4;
            label1.Text = "* To view logs, use the \"Options - Show console\" in the main window.";
            // 
            // BackgroundProgress
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(388, 105);
            Controls.Add(label1);
            Controls.Add(l_status);
            Controls.Add(btn_cancel);
            Controls.Add(progressBar1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new Size(2000, 144);
            MinimizeBox = false;
            MinimumSize = new Size(404, 144);
            Name = "BackgroundProgress";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Please wait";
            FormClosing += BackgroundProgress_FormClosing;
            Shown += BackgroundProgress_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar1;
        private Button btn_cancel;
        private Label l_status;
        private Label label1;
    }
}