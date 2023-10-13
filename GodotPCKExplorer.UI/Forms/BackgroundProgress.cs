using System;
using System.Threading;
using System.Windows.Forms;

namespace GodotPCKExplorer.UI
{
    public partial class BackgroundProgress : Form
    {
        public bool UnknowPercents { get; set; } = false;

        DateTime prevUpdateTime = DateTime.UtcNow;
        float delta_time = 1 / 30; // 30 fps
        int prevPercent = 0;

        Thread work = null;
        Action<CancellationToken> work_action;
        CancellationTokenSource cancellationTokenSource;

        public BackgroundProgress(Action<CancellationToken> action)
        {
            InitializeComponent();

            work_action = action;
            Icon = Properties.Resources.icon;
            l_status.Text = "";
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void BackgroundProgress_Shown(object sender, EventArgs e)
        {
            work = new Thread(new ThreadStart(() =>
            {
                work_action(cancellationTokenSource.Token);
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    BeginInvoke(new Action(Close));
                }
            }));
            work.Start();
        }

        public void ReportProgress(string operation, int number, string customPrefix = null)
        {
            if (number != PCKUtils.UnknownProgressStatus && customPrefix == null)
            {
                if (Text != operation)
                {
                    Text = operation;
                }

                progressBar1.Style = ProgressBarStyle.Continuous;
                var prct = Math.Max(0, Math.Min(100, number));
                if ((DateTime.UtcNow - prevUpdateTime).TotalSeconds > delta_time || (prct - prevPercent) >= 5)
                {
                    prevUpdateTime = DateTime.UtcNow;

                    prevPercent = prct;
                    l_status.Text = $"Progress: {prct}%";
                }
                progressBar1.Value = prct;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Marquee;

                if (customPrefix != null)
                {
                    if ((DateTime.UtcNow - prevUpdateTime).TotalSeconds > delta_time)
                    {
                        prevUpdateTime = DateTime.UtcNow;
                        l_status.Text = customPrefix + number.ToString();
                    }
                }
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            Close();
        }

        private void BackgroundProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel();
            if (work != null)
            {
                if (work.IsAlive)
                    work.Join();
            }
            work = null;

            work_action = null;
            cancellationTokenSource.Dispose();
        }
    }
}
