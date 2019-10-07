using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
	public partial class BackgroundProgress : Form
	{


		public BackgroundProgress()
		{
			InitializeComponent();
			Icon = Properties.Resources.icon;
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (!backgroundWorker1.CancellationPending)
			{
				progressBar1.Value = e.ProgressPercentage;
			}
			else
			{
				progressBar1.Style = ProgressBarStyle.Marquee;
			}
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			backgroundWorker1.CancelAsync();
		}

		private void BackgroundProgress_FormClosing(object sender, FormClosingEventArgs e)
		{
			backgroundWorker1.CancelAsync();
		}
	}
}
