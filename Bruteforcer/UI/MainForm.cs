using GodotPCKExplorer;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Media;

namespace PCKBruteforcer.UI
{
    internal partial class BruteforcerMainForm : Form
    {
        [DllImport("user32.dll")]
        static extern int FlashWindow(IntPtr Hwnd, bool Revert);

        readonly List<Control> browse_buttons = new();
        CancellationTokenSource? cancellationToken = null;
        Bruteforcer? bruteforcer;
        Task? bgTask = null;

        readonly ProgressReporterBrute progress;
        readonly List<ProgressBarEx> progressBars = new();
        static readonly int ProgressRowHeight = 20;

        public BruteforcerMainForm()
        {
            InitializeComponent();
            Icon = Resources.icon;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            progress = new ProgressReporterBrute();

            PCKActions.Init(progress);
            browse_buttons.AddRange(new Control[] { btn_exe, btn_pck, tb_exe, tb_pck, cb_inMemory, nud_threads, nud_from, nud_to });

            nud_threads.Maximum = Environment.ProcessorCount;
            nud_threads.Value = Environment.ProcessorCount - 1;
        }

        void SetControlsEnabled(bool enabled)
        {
            foreach (var b in browse_buttons)
            {
                b.Enabled = enabled;
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            StopTask();
        }

        private void btn_exe_Click(object? sender, EventArgs e)
        {
            if (File.Exists(tb_exe.Text))
                ofd_exe.FileName = tb_exe.Text;

            var res = ofd_exe.ShowDialog();

            if (res == DialogResult.OK)
            {
                tb_exe.Text = ofd_exe.FileName;

                var pck = Path.ChangeExtension(tb_exe.Text, ".pck");
                if (File.Exists(pck))
                {
                    tb_pck.Text = pck;
                }
                UpdateFileLengthAdresses();
            }
        }

        private void btn_pck_Click(object? sender, EventArgs e)
        {
            if (File.Exists(tb_pck.Text))
                ofd_pck.FileName = tb_pck.Text;

            var res = ofd_pck.ShowDialog();

            if (res == DialogResult.OK)
            {
                tb_pck.Text = ofd_pck.FileName;
            }
        }

        private void btn_start_Click(object? sender, EventArgs e)
        {
            if (bgTask == null)
            {
                if (!File.Exists(tb_exe.Text))
                {
                    tb_result.Text = "You need to specify the path to the .exe file";
                    return;
                }

                if (!File.Exists(tb_pck.Text))
                {
                    var is_embeded = false;
                    using (var reader = new PCKReader())
                    {
                        reader.OpenFile(tb_exe.Text);
                        is_embeded = reader.PCK_Embedded;
                        tb_pck.Text = tb_exe.Text;
                    }

                    if (!is_embeded)
                    {
                        tb_result.Text = "You need to specify the path to the .pck file";
                        return;
                    }
                }

                var dist = (long)(nud_to.Value - nud_from.Value);
                if (dist < 0)
                {
                    tb_result.Text = "The end address cannot be less than the start address";
                    return;
                }

                var threads = (int)nud_threads.Value;
                if (dist < threads)
                {
                    threads = Math.Max((int)dist, 1);
                }

                tb_result.Text = "";
                btn_start.Text = "Cancel";

                SetControlsEnabled(false);
                CreateProgressBars(threads);

                cancellationToken = new CancellationTokenSource();

                bruteforcer = new Bruteforcer(
                        disablePCKLogs_cb: DisablePCKLogs,
                        enablePCKLogs_cb: EnablePCKLogs,
                        setOutputText_cb: SetOutputText,
                        log_cb: (t) => Program.Log(t),
                        logException_cb: (ex) => Program.Log(ex),
                        reportProgress_cb: ReportProgress,
                        foundAlert_cb: FoundAlert,
                        finished_cb: Finished
                        );

                bgTask = Task.Run(() => bruteforcer.Start(tb_exe.Text, tb_pck.Text, (long)nud_from.Value, (long)nud_to.Value, threads, cb_inMemory.Checked, cancellationToken));
            }
            else
            {
                StopTask();
            }
        }

        void UpdateFileLengthAdresses()
        {
            if (File.Exists(tb_exe.Text))
            {
                var f = new FileInfo(tb_exe.Text);
                nud_from.Maximum = f.Length;
                nud_to.Value = nud_to.Maximum = f.Length;
            }
        }

        void CreateProgressBars(int count)
        {
            foreach (Control c in tlp_progressTable.Controls)
            {
                c.Dispose();
            }
            tlp_progressTable.Controls.Clear();
            tlp_progressTable.ColumnStyles.Clear();
            tlp_progressTable.RowStyles.Clear();
            progressBars.Clear();

            if (count > Environment.ProcessorCount * 2)
            {
                return;
            }

            int bars_columns = Math.Min(count, 8);
            int bars_rows = (int)Math.Ceiling((double)count / bars_columns);
            tlp_progressTable.ColumnCount = bars_columns;
            tlp_progressTable.RowCount = bars_rows + 1;
            tlp_progressTable.Height = bars_rows * ProgressRowHeight;

            for (int i = 0; i < bars_columns; i++)
            {
                tlp_progressTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)(1.0 / bars_columns)));
            }

            for (int i = 0; i < bars_rows; i++)
            {
                tlp_progressTable.RowStyles.Add(new RowStyle(SizeType.Absolute, ProgressRowHeight));
            }
            // dummy row
            tlp_progressTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            for (int i = 0; i < count; i++)
            {
                var tmp = new ProgressBarEx()
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0),
                    Width = 1,
                    Maximum = Bruteforcer.ProgressMaximum,
                    Step = 1,
                    Style = ProgressBarStyle.Continuous,
                    ProgressColor = Bruteforcer.ProgressDefaultColor,
                };

                tlp_progressTable.Controls.Add(tmp);
                progressBars.Add(tmp);
            }

            tlp_output.PerformLayout();
        }

        void StopTask()
        {
            cancellationToken?.Cancel();
            bgTask?.Wait();

            bgTask?.Dispose();
            bgTask = null;
            cancellationToken?.Dispose();
            cancellationToken = null;
            bruteforcer = null;

            btn_start.Text = "Try to get the key";
        }

        void SafeCall(Action action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        void SetOutputText(string text)
        {
            SafeCall(() => tb_result.Text = text);
        }

        void DisablePCKLogs()
        {
            ProgressReporterBrute.DisableLogs = true;
        }

        void EnablePCKLogs()
        {
            ProgressReporterBrute.DisableLogs = false;
        }

        void ReportProgress(Bruteforcer.ReportData data)
        {
            SafeCall(() =>
            {
                l_estimatedTime.Text = $"Estimated: {(data.RemainingTime + data.ElapsedTime):hh\\:mm\\:ss}";
                l_remaningTime.Text = $"Remaining: {data.RemainingTime:hh\\:mm\\:ss}";
                l_elapsedTime.Text = $"Elapsed: {data.ElapsedTime:hh\\:mm\\:ss}";
                l_percents.Text = $"{data.ProgressPercent:F2}%";

                for (int idx = 0; idx < progressBars.Count; idx++)
                {
                    progressBars[idx].ProgressText = data.ThreadsData[idx].text;
                    progressBars[idx].Value = data.ThreadsData[idx].progress;
                    progressBars[idx].ProgressColor = data.ThreadsData[idx].color;
                }
            });
        }

        void FoundAlert(Bruteforcer.ResultData res)
        {
            SystemSounds.Beep.Play();
        }

        void Finished()
        {
            SafeCall(() =>
            {
                _ = FlashWindow(Handle, true);
                SetControlsEnabled(true);
                StopTask();
            });
        }
    }
}
