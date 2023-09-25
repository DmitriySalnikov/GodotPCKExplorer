using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace GodotPCKExplorer.UI
{
    public partial class BackgroundProgress : Form
    {
        public bool UnknowPercents { get; set; } = false;

        DateTime prevUpdateTime = DateTime.Now;
        float delta_time = 1 / 30; // 30 fps
        int prevPercent = 0;
        CancellationTokenSource cancellationTokenSource;

        public BackgroundProgress(CancellationTokenSource token)
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;
            l_status.Text = "";
            cancellationTokenSource = token;
        }

        public void ReportProgress(string operation, int percent)
        {
            if (percent != PCKUtils.UnknownProgressStatus)
            {
                if (Text != operation)
                {
                    Text = operation;
                }

                progressBar1.Style = ProgressBarStyle.Continuous;
                var prct = Math.Max(0, Math.Min(100, percent));
                if ((DateTime.Now - prevUpdateTime).TotalSeconds > delta_time || (prct - prevPercent) >= 5)
                {
                    prevUpdateTime = DateTime.Now;

                    prevPercent = prct;
                    l_status.Text = $"Progress: {prct}%";
                }
                progressBar1.Value = prct;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private void BackgroundProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
