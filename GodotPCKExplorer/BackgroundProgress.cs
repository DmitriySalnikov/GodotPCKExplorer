using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public partial class BackgroundProgress : Form
    {
        public bool UnknowPercents { get; set; } = false;

        DateTime prevUpdateTime = DateTime.Now;
        int prevPercent = 0;

        public BackgroundProgress()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;
            l_status.Text = "";
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!bg_worker.CancellationPending && !UnknowPercents)
            {
                var prct = Math.Max(0, Math.Min(100, e.ProgressPercentage));
                if ((DateTime.Now - prevUpdateTime).TotalSeconds > 2 || (prct - prevPercent) > 10)
                {
                    prevUpdateTime = DateTime.Now;
                    if (Program.CMDMode)
                    {
                        Program.Log($"{prct}%");
                        prevPercent = prct;
                    }

                    l_status.Text = $"Progress: {prct}%";
                }
                progressBar1.Value = prct;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Marquee;

                if (e.UserState != null && e.UserState is string)
                {
                    if ((DateTime.Now - prevUpdateTime).TotalSeconds > 0.25)
                    {
                        prevUpdateTime = DateTime.Now;
                        l_status.Text = e.UserState as string;
                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            bg_worker.CancelAsync();
        }

        private void BackgroundProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            bg_worker.CancelAsync();
        }
    }
}
