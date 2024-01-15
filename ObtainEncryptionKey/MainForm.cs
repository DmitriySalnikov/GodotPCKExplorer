using GodotPCKExplorer;
using System.Globalization;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;

namespace ObtainPCKEncryptionKey
{
    // TODO make it cross-platform?
    internal partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        static extern int FlashWindow(IntPtr Hwnd, bool Revert);

        readonly List<Control> browse_buttons = new();
        CancellationTokenSource? cancellationToken = null;
        Task? bgTask = null;

        readonly ProgressReporterBrute progress;
        readonly List<ProgressBarEx> progressBars = new();
        readonly int ProgressMaximum = 10000;
        readonly int ProgressRowHeight = 20;
        readonly double update_interval = 0.25;

        static readonly Color progressDefaultColor = Color.LimeGreen;
        static readonly Color progressFoundColor = Color.Gold;
        static readonly Color progressErrorColor = Color.IndianRed;

        public MainForm()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            progress = new ProgressReporterBrute();

            PCKActions.Init(progress);
            browse_buttons.AddRange(new Control[] { btn_exe, btn_pck, tb_exe, tb_pck, cb_inMemory, nud_threads, nud_from, nud_to });

            nud_threads.Value = Environment.ProcessorCount - 1;
            nud_threads.Maximum = Environment.ProcessorCount;
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
                bgTask = Task.Run(() => Bruteforce(tb_exe.Text, tb_pck.Text, threads, cb_inMemory.Checked, (long)nud_from.Value, (long)nud_to.Value, cancellationToken));
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
                    Maximum = ProgressMaximum,
                    Step = 1,
                    Style = ProgressBarStyle.Continuous,
                    ProgressColor = progressDefaultColor,
                };

                tlp_progressTable.Controls.Add(tmp);
                progressBars.Add(tmp);
            }

            if (tlp_output.HorizontalScroll.Visible)
                Console.WriteLine("asdasdasdsdsd");
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

        struct StartData
        {
            public string exe;
            public string pck;
            public bool encIndex;
            public long startPos;
            public long endPos;
            public CancellationTokenSource ct;
            public Action<int, long>? reportProgress;
            public int threadIdx;
            public string? testFileName = null;
            public PCKFile? testFile = null;
            public MemoryStream? memStr = null;

            public StartData(string exe, string pck, bool encIndex, long startPos, long endPos, CancellationTokenSource ct, int threadIdx, string? testFileName, PCKFile? testFile, MemoryStream? memStr)
            {
                this.exe = exe;
                this.pck = pck;
                this.encIndex = encIndex;
                this.startPos = startPos;
                this.endPos = endPos;
                this.ct = ct;
                this.reportProgress = null;
                this.threadIdx = threadIdx;
                this.testFileName = testFileName;
                this.testFile = testFile;
                this.memStr = memStr;
            }
        }

        struct ResultData
        {
            public bool result;
            public string key;
            public long address;

            public ResultData(bool result, string key, long address)
            {
                this.result = result;
                this.key = key;
                this.address = address;
            }
        }

        class ThreadProgressData
        {
            public int progress;
            public string text;
            public Color color;

            public ThreadProgressData(int progress, string text, Color color)
            {
                this.progress = progress;
                this.text = text;
                this.color = color;
            }
        }

        void Bruteforce(string exe, string pck, int threadsCount, bool inMemory, long startAdr, long endAdr, CancellationTokenSource ct)
        {
            Thread.CurrentThread.Name = $"{nameof(Bruteforce)} main";

            bool use_custom_output = false;
            DateTime start_time = DateTime.UtcNow;
            double max_time = TimeSpan.FromHours(999).TotalSeconds;

            try
            {
                string? pck_in_memory_file_name = null;
                PCKFile? pck_in_memory_file = null;
                long pck_StartPosition;
                bool pck_encIndex;
                bool pck_embeded;
                long pck_FileBase;

                using (PCKReader pck_reader = new())
                {
                    pck_reader.OpenFile(pck,
                                     show_NotPCKError: true,
                                     readOnlyHeaderGodot4: false,
                                     logFileNamesProgress: false,
                                     cancellationToken: ct.Token);

                    if (pck_reader.PCK_VersionPack < PCKUtils.PCK_VERSION_GODOT_4)
                    {
                        use_custom_output = true;
                        SetOutputText("Unsupported PCK version.");
                        return;
                    }

                    if (!pck_reader.IsEncrypted)
                    {
                        use_custom_output = true;
                        SetOutputText("File is not encrypted.");
                        return;
                    }

                    pck_StartPosition = pck_reader.PCK_StartPosition;
                    pck_encIndex = pck_reader.IsEncryptedIndex;
                    pck_embeded = pck_reader.PCK_Embedded;
                    pck_FileBase = pck_reader.PCK_FileBase;

                    if (pck_reader.Files.Count > 0)
                    {
                        // First, find small files between 5KB and 1MB in size to have enough info for validation.
                        var files = pck_reader.Files.Where((f) => f.Value.Size >= 1024 * 5 && f.Value.Size < 1024 * 1024).OrderBy((f) => f.Value.Size);
                        if (files.Any())
                        {
                            pck_in_memory_file_name = files.First().Key;
                            pck_in_memory_file = files.First().Value;
                        }
                        else
                        {
                            // Then smaller files
                            files = pck_reader.Files.Where((f) => f.Value.Size > 512 && f.Value.Size < 1024 * 5).OrderByDescending((f) => f.Value.Size);
                            if (files.Any())
                            {
                                pck_in_memory_file_name = files.First().Key;
                                pck_in_memory_file = files.First().Value;
                            }
                            else
                            {
                                // Then first larger files but not too big
                                files = pck_reader.Files.Where((f) => f.Value.Size > 512 && f.Value.Size < 1024 * 1024 * 256).OrderBy((f) => f.Value.Size);
                                if (files.Any())
                                {
                                    pck_in_memory_file_name = files.First().Key;
                                    pck_in_memory_file = files.First().Value;
                                }
                                else
                                {
                                    // Any other file of any size
                                    pck_in_memory_file_name = files.First().Key;
                                    pck_in_memory_file = pck_reader.Files.OrderBy((f) => f.Value.Size).First().Value;
                                }
                            }
                        }

                        if (pck_in_memory_file.Size > 1024 * 1024 * 100)
                        {
                            SetOutputText($"The size of the file that will be used to test the encryption key is greater than 100MB:\r\n{pck_in_memory_file.Size / (1024 * 1024):F2}MB");
                        }
                    }
                    else if (!pck_reader.IsEncryptedIndex)
                    {
                        SetOutputText("PCK does not contain any encrypted files. Unable to decrypt.");
                        return;
                    }
                }

                byte[]? encMemChunk = null;
                if (pck_encIndex)
                {
                    if (inMemory)
                    {
                        using var fileReader = new BinaryReader(File.Open(pck, FileMode.Open, FileAccess.Read, FileShare.Read));
                        fileReader.BaseStream.Position = pck_StartPosition;
                        encMemChunk = new byte[pck_FileBase - pck_StartPosition];
                        fileReader.Read(encMemChunk, 0, encMemChunk.Length);
                    }
                }
                else if (pck_in_memory_file != null)
                {
                    if (inMemory)
                    {
                        using var fileReader = new BinaryReader(File.Open(pck, FileMode.Open, FileAccess.Read, FileShare.Read));
                        fileReader.BaseStream.Position = pck_in_memory_file.Offset;

                        // Read encrypted header
                        using var encReader = new PCKEncryptedReader(fileReader, Array.Empty<byte>());
                        // Restore position to header start
                        fileReader.BaseStream.Position = pck_in_memory_file.Offset;

                        encMemChunk = new byte[(int)encReader.DataSizeEncoded + encReader.HeaderSize];
                        fileReader.Read(encMemChunk, 0, encMemChunk.Length);
                    }
                }
                else
                {
                    SetOutputText("There is nothing to decrypt.");
                    return;
                }

                List<Thread> threads = new();
                ResultData[] thread_results = new ResultData[threadsCount];
                ThreadProgressData[] thread_progress = new ThreadProgressData[threadsCount];
                object task_progress_mutex = new();

                using (var exeReader = new BinaryReader(File.Open(exe, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    if (endAdr == startAdr)
                    {
                        endAdr = Math.Min(endAdr + 1, exeReader.BaseStream.Length);
                    }

                    long endPos = endAdr;

                    if (pck_embeded && endPos > pck_StartPosition)
                    {
                        if (startAdr > pck_StartPosition)
                        {
                            use_custom_output = true;
                            SetOutputText("The start address is greater than the start of the embedded PCK. Canceling.");
                            return;
                        }
                        else
                        {
                            SetOutputText("The end address is greater than the beginning of the embedded PCK.\r\nLimiting the address to the beginning of the PCK.");
                            endPos = pck_StartPosition;
                        }
                    }

                    var step = (endPos - startAdr) / threadsCount;

                    long start = startAdr;
                    long end = start + step;
                    for (int i = 0; i < threadsCount; i++)
                    {
                        progress.Log($"Thread {i} Start: {start} End: {end}");

                        thread_progress[i] = new ThreadProgressData(0, "Init", progressDefaultColor);

                        var args = new StartData(
                            exe,
                            pck,
                            pck_encIndex,
                            start,
                            end,
                            ct,
                            i,
                            pck_in_memory_file_name,
                            pck_in_memory_file,
                            encMemChunk != null ? new MemoryStream(encMemChunk) : null);

                        args.reportProgress = (percent, address) =>
                        {
                            lock (task_progress_mutex)
                            {
                                thread_progress[args.threadIdx].text = address.ToString();
                                thread_progress[args.threadIdx].progress = percent;
                            }
                        };

                        var th = new Thread(() =>
                        {
                            try
                            {
                                thread_results[args.threadIdx] = BruteforcePartial(args);
                            }
                            catch (Exception ex)
                            {
                                lock (task_progress_mutex)
                                {
                                    thread_results[args.threadIdx] = new ResultData();
                                    thread_progress[args.threadIdx].text = "Error";
                                    thread_progress[args.threadIdx].progress = ProgressMaximum;
                                    thread_progress[args.threadIdx].color = progressErrorColor;
                                }

                                progress.Log(ex);
                            }
                        });

                        threads.Add(th);
                        th.Start();

                        start += step;

                        if (i + 1 == threadsCount - 1)
                            end = endPos;
                        else
                            end = start + step;
                    }
                }

                var prev_delta_buf = new CircularBuffer(48);
                double prev_perc = 0;
                DateTime prev_time = DateTime.UtcNow;

                var pb_update_token = new CancellationTokenSource();
                var ui_update_time = DateTime.UtcNow;
                var pb_update = Task.Run(() =>
                {
                    bool finish_update = false;

                    while (!pb_update_token.Token.IsCancellationRequested || !finish_update)
                    {
                        if (pb_update_token.Token.IsCancellationRequested)
                            finish_update = true;

                        if ((DateTime.UtcNow - ui_update_time).TotalSeconds > update_interval || finish_update)
                        {
                            SafeCall(() =>
                            {
                                var perc = (thread_progress.Sum((p) => p.progress) / (double)thread_progress.Length) / ProgressMaximum * 100;
                                var elapsed = DateTime.UtcNow - start_time;

                                // Do not update on cancel, so as not to display the wrong time.
                                if (!finish_update)
                                {
                                    var remaining_sec = Math.Min((DateTime.UtcNow - prev_time).TotalSeconds * ((100 - perc) / Math.Max(prev_delta_buf.Avg(), 0.00000001)), max_time);
                                    l_estimatedTime.Text = $"Estimated time: {(elapsed + TimeSpan.FromSeconds(remaining_sec)):hh\\:mm\\:ss}";
                                }

                                prev_delta_buf.Push(perc - prev_perc);
                                prev_perc = perc;
                                prev_time = DateTime.UtcNow;

                                l_elapsedTime.Text = $"Elapsed time: {elapsed:hh\\:mm\\:ss}";
                                l_percents.Text = $"{perc:F2}%";

                                for (int idx = 0; idx < progressBars.Count; idx++)
                                {
                                    lock (task_progress_mutex)
                                    {
                                        progressBars[idx].ProgressText = thread_progress[idx].text;
                                        progressBars[idx].Value = thread_progress[idx].progress;
                                        progressBars[idx].ProgressColor = thread_progress[idx].color;
                                    }
                                }
                            });
                            ui_update_time = DateTime.UtcNow;
                        }
                    }
                });

                var threads_res = new ResultData();
                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Join();

                    ResultData res = thread_results[i];

                    if (res.result)
                    {
                        threads_res = res;

                        progress.Log($"Thread {i} found {res.key} at {res.address}");
                        lock (task_progress_mutex)
                        {
                            thread_progress[i].progress = ProgressMaximum;
                            thread_progress[i].color = progressFoundColor;
                        }
                    }
                    else
                    {
                        progress.Log($"Thread {i} nothing found");
                    }
                }

                if (threads_res.result)
                {
                    use_custom_output = true;
                    SetOutputText($"{threads_res.key}\r\nAt address: {threads_res.address}");

                    SystemSounds.Beep.Play();
                }
                else
                {
                    if (ct.IsCancellationRequested)
                    {
                        use_custom_output = true;
                        var str = "Cancelled";
                        SetOutputText(str);
                        progress.Log(str);
                    }
                }

                pb_update_token.Cancel();
                pb_update.Wait();
                pb_update.Dispose();

                SafeCall(() =>
                {
                    _ = FlashWindow(Handle, true);
                });
            }
            finally
            {
                if (!use_custom_output)
                    SetOutputText("No matching key found");

                ProgressReporterBrute.DisableLogs = false;
                SafeCall(() =>
                {
                    SetControlsEnabled(true);
                    StopTask();
                });
            }
        }

        ResultData BruteforcePartial(StartData args)
        {
            Thread.CurrentThread.Name = $"Bruteforce thread {args.threadIdx}";
            // Wait some time for the GUI update 
            Thread.Sleep(250);

            long hex_position = 0;
            int current_progress = 0;

            try
            {
                using var exeReader = new BinaryReader(File.Open(args.exe, FileMode.Open, FileAccess.Read, FileShare.Read));

                progress.Log($"Thread {args.threadIdx} Started on {args.startPos}");
                var exeStream = exeReader.BaseStream;
                exeStream.Position = args.startPos;

                double data_length = args.endPos - args.startPos;

                if (args.encIndex)
                {
                    using PCKReader pckReader = new();
                    using BinaryReader br = new(
                        args.memStr != null ? args.memStr : File.Open(args.pck, FileMode.Open, FileAccess.Read, FileShare.Read),
                        Encoding.UTF8, true);

                    while ((args.endPos - exeStream.Position) > 0)
                    {
                        if (args.ct.IsCancellationRequested)
                        {
                            return new ResultData();
                        }

                        hex_position = exeStream.Position;
                        current_progress = (int)((exeStream.Position - args.startPos) / data_length * ProgressMaximum);
                        args.reportProgress?.Invoke(current_progress, hex_position);

                        var hex = PCKUtils.ByteArrayToHexString(exeReader.ReadBytes(32));
                        if (hex.Length / 2 < 32)
                        {
                            return new ResultData();
                        }

                        br.BaseStream.Position = 0;
                        ProgressReporterBrute.DisableLogs = true;
                        bool res = pckReader.OpenFile(
                            br,
                            getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = hex },
                            disableExceptions: true,
                            cancellationToken: args.ct.Token);
                        pckReader.Close();
                        ProgressReporterBrute.DisableLogs = false;

                        if (res)
                        {
                            args.ct.Cancel();
                            return new ResultData(true, hex, hex_position);
                        }

                        // Move to next byte
                        exeStream.Seek(-32 + 1, SeekOrigin.Current);
                    }
                }
                else
                {
                    if (args.testFile == null)
                    {
                        throw new NullReferenceException(nameof(args.testFile));
                    }

                    using BinaryWriter memFile = new(new MemoryStream((int)PCKUtils.AlignAddress(args.testFile.Size, mbedTLS.CHUNK_SIZE)));
                    using BinaryReader br = new(
                        args.memStr != null ? args.memStr : File.Open(args.pck, FileMode.Open, FileAccess.Read, FileShare.Read),
                        Encoding.UTF8, true);

                    while ((args.endPos - exeStream.Position) > 0)
                    {
                        if (args.ct.IsCancellationRequested)
                        {
                            return new ResultData();
                        }

                        hex_position = exeStream.Position;
                        current_progress = (int)((exeStream.Position - args.startPos) / data_length * ProgressMaximum);
                        args.reportProgress?.Invoke(current_progress, hex_position);

                        var bytes = exeReader.ReadBytes(32);
                        if (bytes.Length < 32)
                        {
                            return new ResultData();
                        }

                        ProgressReporterBrute.DisableLogs = true;
                        br.BaseStream.Position = args.memStr != null ? 0 : args.testFile.Offset;
                        long fileSize = 0;
                        using (var r = new PCKEncryptedReader(br, bytes))
                        {
                            memFile.BaseStream.Position = 0;
                            foreach (var chunk in r.ReadEncryptedBlocks())
                            {
                                fileSize += chunk.Length;
                                memFile.Write(chunk.Span);
                            }
                        }
                        ProgressReporterBrute.DisableLogs = false;

                        if (args.testFile.MD5.SequenceEqual(PCKUtils.GetStreamMD5(memFile.BaseStream, 0, fileSize)))
                        {
                            args.ct.Cancel();
                            return new ResultData(true, PCKUtils.ByteArrayToHexString(bytes), hex_position);
                        }

                        // Move to next byte
                        exeStream.Seek(-32 + 1, SeekOrigin.Current);
                    }
                }
            }
            finally
            {
                args.reportProgress?.Invoke(current_progress, hex_position);
                args.memStr?.Dispose();
            }

            return new ResultData();
        }
    }
}
