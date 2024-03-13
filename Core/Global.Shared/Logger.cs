using System.Globalization;
using System.Text;

namespace GodotPCKExplorer.GlobalShared
{
    // https://stackoverflow.com/a/57837393
    public static class FastConsole
    {
        static readonly BufferedStream str;

        static FastConsole()
        {
            Console.OutputEncoding = Encoding.Unicode;  // crucial

            // avoid special "ShadowBuffer" for hard-coded size 0x14000 in 'BufferedStream' 
            str = new BufferedStream(Console.OpenStandardOutput(), 128 * 1024);
        }

        public static void WriteLine(string s)
        {
            Write(s);
            Write(Environment.NewLine);
        }

        public static void Write(string s)
        {
            // avoid endless 'GetByteCount' dithering in 'Encoding.Unicode.GetBytes(s)'
            var rgb = new byte[s.Length << 1];
            Encoding.Unicode.GetBytes(s, 0, s.Length, rgb, 0);

            lock (str)   // (optional, can omit if appropriate)
                str.Write(rgb, 0, rgb.Length);
        }

        public static void Flush() { lock (str) str.Flush(); }
    };

    public sealed class Logger : IDisposable
    {
        public bool DuplicateToConsole = true;

        bool disposed = false;
        readonly string saveFile;
        TextWriter? logWriter = null;
        DeferredAction? flushFileAction = null;
        readonly object dataLock = new();

        readonly DeferredAction flushFastConsole;
        DateTime timeFlushConsole = DateTime.UtcNow;

        public Logger(string saveFile)
        {
            this.saveFile = Path.Combine(GlobalConstants.AppDataPath, saveFile);
            flushFastConsole = new DeferredAction(() => FastConsole.Flush(), 500);

            try
            {
                string dir_name = Path.GetDirectoryName(this.saveFile) ?? string.Empty;
                if (!Directory.Exists(dir_name))
                    Directory.CreateDirectory(dir_name);

                if (File.Exists(this.saveFile))
                    File.Delete(this.saveFile);
                logWriter = new StreamWriter(File.Open(this.saveFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read));
                flushFileAction = new DeferredAction(() => logWriter.Flush(), 500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The log file (\"{this.saveFile}\") cannot be opened for writing.");
                Write(ex);
            }

            Write($"// Time format - {CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.InvariantCulture.DateTimeFormat.LongTimePattern}");
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            // Force print remaining part of output
            FastConsole.Flush();

            flushFileAction?.Dispose();
            flushFileAction = null;

            flushFastConsole.Dispose();

            if (logWriter != null)
            {
                try
                {
                    logWriter.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                logWriter.Close();
                logWriter.Dispose();
            }
            logWriter = null;
        }

        public void Flush()
        {
            FastConsole.Flush();
            flushFastConsole.Cancel();
            timeFlushConsole = DateTime.UtcNow;
            logWriter?.Flush();
        }

        public void Write(string txt)
        {
            var isFirst = true;
            txt = string.Join(Environment.NewLine,
                txt.Split(["\r\n", "\n", "\r"], StringSplitOptions.None)
                .Select((t) =>
                {
                    var res = $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]{(isFirst ? "\t" : "-\t")}{t}";
                    isFirst = false;
                    return res;
                }));

            lock (dataLock)
            {
                if (DuplicateToConsole)
                    FastConsole.WriteLine(txt);

                // Force write or continue buffering
                if ((DateTime.UtcNow - timeFlushConsole).TotalMilliseconds > 500)
                {
                    FastConsole.Flush();
                    flushFastConsole.Cancel();
                    timeFlushConsole = DateTime.UtcNow;
                }
                else
                {
                    flushFastConsole.CallDeferred();
                }

                if (logWriter != null)
                {
                    logWriter.WriteLine(txt);
                    flushFileAction?.CallDeferred();
                }
            }
        }

        public void WriteError(string txt)
        {
            Write("❗ Error: " + txt);
        }

        public void Write(Exception ex)
        {
            Write($"⚠ {ex.GetType().Name}:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}");
        }
    }
}
