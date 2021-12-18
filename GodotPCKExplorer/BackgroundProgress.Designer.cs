namespace GodotPCKExplorer
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bg_worker = new System.ComponentModel.BackgroundWorker();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.l_status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(364, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // bg_worker
            // 
            this.bg_worker.WorkerReportsProgress = true;
            this.bg_worker.WorkerSupportsCancellation = true;
            this.bg_worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.bg_worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(301, 41);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 2;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // l_status
            // 
            this.l_status.AutoSize = true;
            this.l_status.Location = new System.Drawing.Point(12, 46);
            this.l_status.Name = "l_status";
            this.l_status.Size = new System.Drawing.Size(35, 13);
            this.l_status.TabIndex = 3;
            this.l_status.Text = "label1";
            // 
            // BackgroundProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 71);
            this.Controls.Add(this.l_status);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(404, 110);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(404, 110);
            this.Name = "BackgroundProgress";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Please wait";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BackgroundProgress_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button btn_cancel;
		public System.ComponentModel.BackgroundWorker bg_worker;
        private System.Windows.Forms.Label l_status;
    }
}