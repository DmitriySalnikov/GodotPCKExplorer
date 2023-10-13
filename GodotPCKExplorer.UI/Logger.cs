using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GodotPCKExplorer.UI
{
    // https://stackoverflow.com/a/57837393
    public static class FastConsole
    {
        static readonly BufferedStream str;

        static FastConsole()
        {
            Console.OutputEncoding = Encoding.Unicode;  // crucial

            // avoid special "ShadowBuffer" for hard-coded size 0x14000 in 'BufferedStream' 
            str = new BufferedStream(Console.OpenStandardOutput(), 256000);
        }

        public static void WriteLine(string s)
        {
            Write(s);
            Write("\r\n");
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

    public class Logger : IDisposable
    {
        readonly string saveFile;
        TextWriter logWriter = null;
        DeferredAction flushFileAction = null;
        readonly object dataLock = new object();

        DeferredAction flushFastConsole = null;
        DateTime timeFlushConsole = DateTime.UtcNow;

        public Logger(string saveFile)
        {
            this.saveFile = Path.Combine(Program.AppDataPath, saveFile);

            try
            {
                string dir_name = Path.GetDirectoryName(this.saveFile);
                if (!Directory.Exists(dir_name))
                    Directory.CreateDirectory(dir_name);

                if (File.Exists(this.saveFile))
                    File.Delete(this.saveFile);
                logWriter = new StreamWriter(File.Open(this.saveFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read));
                flushFileAction = new DeferredAction(() => logWriter.Flush(), 500);
                flushFastConsole = new DeferredAction(() => FastConsole.Flush(), 500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The log file (\"{this.saveFile}\") cannot be opened for writing. {ex.Message}");
            }

            Write($"// Time format - {CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern} {CultureInfo.InvariantCulture.DateTimeFormat.LongTimePattern}");
        }

        public void Dispose()
        {
            flushFileAction?.Dispose();
            flushFileAction = null;

            flushFastConsole?.Dispose();
            flushFastConsole = null;

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

        public void Write(string txt)
        {
            var isFirst = true;
            txt = string.Join("\n",
                txt.Split('\n').
                Select((t) =>
                {
                    var res = $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]{(isFirst ? "\t" : "-\t")}{t}";
                    isFirst = false;
                    return res;
                }));

            lock (dataLock)
            {
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

        public void Write(Exception ex)
        {
            Write($"⚠ {ex.GetType().Name}:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}");
        }
    }
}
