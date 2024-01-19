using GodotPCKExplorer;
using System.Drawing;
using System.Text;

namespace PCKBruteforcer
{
    public class Bruteforcer
    {
        public static readonly int ProgressMaximum = 10000;

        public static readonly Color ProgressDefaultColor = Color.LimeGreen;
        public static readonly Color ProgressFoundColor = Color.Gold;
        public static readonly Color ProgressErrorColor = Color.IndianRed;

        public double ReportUpdateInterval = 0.25;

        struct StartData(string exe, string pck, bool encIndex, long startPos, long endPos, CancellationTokenSource ct, int threadIdx, string? testFileName, PCKFile? testFile, MemoryStream? memStr)
        {
            public string exe = exe;
            public string pck = pck;
            public bool encIndex = encIndex;
            public long startPos = startPos;
            public long endPos = endPos;
            public CancellationTokenSource ct = ct;
            public Action<int, long>? reportProgress = null;
            public int threadIdx = threadIdx;
            public string? testFileName = testFileName;
            public PCKFile? testFile = testFile;
            public MemoryStream? memStr = memStr;
        }

        public struct ResultData(bool result, string key, long address)
        {
            public bool found = result;
            public string key = key;
            public long address = address;
        }

        public struct ReportData(TimeSpan elapsedTime, TimeSpan remainingTime, double progressPercent, ThreadProgressData[] threadsData)
        {
            public TimeSpan ElapsedTime = elapsedTime;
            public TimeSpan RemainingTime = remainingTime;
            public double ProgressPercent = progressPercent;
            public ThreadProgressData[] ThreadsData = threadsData;
        }

        public struct ThreadProgressData(int progress, string text, Color color)
        {
            public int progress = progress;
            public string text = text;
            public Color color = color;
        }

        readonly Action? disablePCKLogs_cb;
        readonly Action? enablePCKLogs_cb;
        readonly Action<string>? setOutputText_cb;
        readonly Action<string>? log_cb;
        readonly Action<Exception>? logException_cb;
        readonly Action<ReportData>? reportProgress_cb;
        readonly Action<ResultData>? foundAlert_cb;
        readonly Action? finished_cb;

        void DisablePCKLogs()
        {
            disablePCKLogs_cb?.Invoke();
        }

        void EnablePCKLogs()
        {
            enablePCKLogs_cb?.Invoke();
        }

        void SetOutputText(string text)
        {
            setOutputText_cb?.Invoke(text);
        }

        void Log(string text)
        {
            log_cb?.Invoke(text);
        }

        void Log(Exception ex)
        {
            logException_cb?.Invoke(ex);
        }

        void ReportProgress(ReportData data)
        {
            reportProgress_cb?.Invoke(data);
        }

        void FoundAlert(ResultData res)
        {
            foundAlert_cb?.Invoke(res);
        }

        void Finished()
        {
            finished_cb?.Invoke();
        }

        public Bruteforcer(Action? disablePCKLogs_cb = null, Action? enablePCKLogs_cb = null, Action<string>? setOutputText_cb = null, Action<string>? log_cb = null, Action<Exception>? logException_cb = null, Action<ReportData>? reportProgress_cb = null, Action<ResultData>? foundAlert_cb = null, Action? finished_cb = null)
        {
            this.disablePCKLogs_cb = disablePCKLogs_cb;
            this.enablePCKLogs_cb = enablePCKLogs_cb;
            this.setOutputText_cb = setOutputText_cb;
            this.log_cb = log_cb;
            this.logException_cb = logException_cb;
            this.reportProgress_cb = reportProgress_cb;
            this.foundAlert_cb = foundAlert_cb;
            this.finished_cb = finished_cb;
        }

        public void Start(string exe, string pck, long startAdr, long endAdr, int threadsCount, bool inMemory, CancellationTokenSource ct)
        {
            if (!PCKActions.ValidateMbedTLS())
            {
                Log("Encryption library not loaded or loaded incorrectly! Unable to continue.");
                return;
            }

            Thread.CurrentThread.Name = $"{nameof(Bruteforcer)} main";
            bool use_custom_output = false;
            DateTime start_time = DateTime.UtcNow;
            double max_time = TimeSpan.FromHours(999).TotalSeconds;

            try
            {
                if (startAdr > endAdr)
                {
                    use_custom_output = true;
                    SetOutputText("The start address cannot be greater than the end address.");
                    return;
                }

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
                            SetOutputText($"The size of the file that will be used to test the encryption key is greater than 100MB:\r\n{pck_in_memory_file.Size / (1024 * 1024):F2}MB\r\nThis will slow down the process a lot, but it's the smallest file.");
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
                        using var encReader = new PCKEncryptedReader(fileReader, []);
                        // Restore position to header start
                        fileReader.BaseStream.Position = pck_in_memory_file.Offset;

                        encMemChunk = new byte[(int)encReader.DataSizeEncoded + encReader.HeaderSize];
                        fileReader.Read(encMemChunk, 0, encMemChunk.Length);
                    }
                }
                else
                {
                    use_custom_output = true;
                    SetOutputText("There is nothing to decrypt.");
                    return;
                }

                List<Thread> threads = [];
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
                            SetOutputText($"The end address is greater than the beginning of the embedded PCK.\r\nLimiting the address to the beginning of the PCK: {pck_StartPosition}");
                            endPos = pck_StartPosition;
                        }
                    }

                    var step = (endPos - startAdr) / threadsCount;

                    long start = startAdr;
                    long end = start + step;
                    for (int i = 0; i < threadsCount; i++)
                    {
                        Log($"Thread {i} Start: {start} End: {end}");

                        thread_progress[i] = new ThreadProgressData(0, "Init", ProgressDefaultColor);

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
                                var data = thread_progress[args.threadIdx];

                                data.text = address.ToString();
                                data.progress = percent;

                                thread_progress[args.threadIdx] = data;
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
                                    thread_progress[args.threadIdx] = new ThreadProgressData(ProgressMaximum, "Error", ProgressErrorColor);
                                }

                                Log(ex);
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
                double prev_percent = 0;
                DateTime prev_time = DateTime.UtcNow;

                var pb_update_token = new CancellationTokenSource();
                var ui_update_time = DateTime.UtcNow;
                var pb_update = Task.Run(() =>
                {
                    bool finish_update = false;
                    TimeSpan remaining_time = TimeSpan.Zero;
                    ThreadProgressData[] safe_thread_progress = new ThreadProgressData[threadsCount];

                    while (!pb_update_token.Token.IsCancellationRequested || !finish_update)
                    {
                        if (pb_update_token.Token.IsCancellationRequested)
                            finish_update = true;

                        if ((DateTime.UtcNow - ui_update_time).TotalSeconds > ReportUpdateInterval || finish_update)
                        {
                            var perc = (thread_progress.Sum((p) => p.progress) / (double)thread_progress.Length) / ProgressMaximum * 100;
                            var elapsed = DateTime.UtcNow - start_time;

                            // Do not update on cancel, so as not to display the wrong time.
                            if (!finish_update)
                            {
                                double remaining_sec = Math.Min((DateTime.UtcNow - prev_time).TotalSeconds * ((100 - perc) / Math.Max(prev_delta_buf.Avg(), 0.00000001)), max_time);
                                remaining_time = TimeSpan.FromSeconds(remaining_sec);
                            }

                            prev_delta_buf.Push(perc - prev_percent);
                            prev_percent = perc;
                            prev_time = DateTime.UtcNow;

                            safe_thread_progress = [.. thread_progress];

                            ReportProgress(new ReportData(elapsed, remaining_time, perc, safe_thread_progress));

                            ui_update_time = DateTime.UtcNow;
                        }
                    }
                });

                var threads_res = new ResultData();
                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Join();

                    ResultData res = thread_results[i];

                    if (res.found)
                    {
                        threads_res = res;

                        Log($"Thread {i} found {res.key} at {res.address}");
                        lock (task_progress_mutex)
                        {
                            thread_progress[i] = new ThreadProgressData(ProgressMaximum, thread_progress[i].text, ProgressFoundColor);
                        }
                    }
                    else
                    {
                        Log($"Thread {i} nothing found");
                    }
                }

                if (threads_res.found)
                {
                    use_custom_output = true;
                    FoundAlert(threads_res);
                    SetOutputText($"{threads_res.key}\r\nAt address: {threads_res.address}");
                }
                else
                {
                    if (ct.IsCancellationRequested)
                    {
                        use_custom_output = true;
                        var str = "Cancelled";
                        SetOutputText(str);
                        Log(str);
                    }
                }

                pb_update_token.Cancel();
                pb_update.Wait();
                pb_update.Dispose();

            }
            finally
            {
                EnablePCKLogs();
                if (!use_custom_output)
                    SetOutputText("No matching key found");

                Finished();
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

                Log($"Thread {args.threadIdx} Started on {args.startPos}");
                var exeStream = exeReader.BaseStream;
                exeStream.Position = args.startPos;

                double data_length = args.endPos - args.startPos;

                if (args.encIndex)
                {
                    using PCKReader pckReader = new();
                    using BinaryReader br = new(
                        args.memStr != null ? args.memStr : File.Open(args.pck, FileMode.Open, FileAccess.Read, FileShare.Read),
                        Encoding.UTF8, args.memStr != null);

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

                        br.BaseStream.Position = 0;
                        DisablePCKLogs();
                        bool res = pckReader.OpenFile(
                            br,
                            getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { KeyBytes = bytes },
                            disableExceptions: true,
                            cancellationToken: args.ct.Token);
                        pckReader.Close();
                        EnablePCKLogs();

                        if (res)
                        {
                            args.ct.Cancel();
                            return new ResultData(true, PCKUtils.ByteArrayToHexString(bytes), hex_position);
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

                        DisablePCKLogs();
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
                        EnablePCKLogs();

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
