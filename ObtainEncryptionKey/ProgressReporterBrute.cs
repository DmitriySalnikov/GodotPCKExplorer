using System.Globalization;
using GodotPCKExplorer;

namespace ObtainPCKEncryptionKey
{
    internal class ProgressReporterBrute : IPCKProgressReporter
    {
        [ThreadStatic]
        public static bool DisableLogs;

        int prev_progress = 0;
        DateTime prev_time = DateTime.UtcNow;

        public void LogProgress(string operation, int number, string? customPrefix = null)
        {
            if (DisableLogs) return;

            if (((DateTime.UtcNow - prev_time).TotalSeconds > 1) || (prev_progress != number && Math.Abs(number - prev_progress) >= 5))
            {
                if (customPrefix != null)
                    Log($"[Progress] {operation}: {customPrefix}{number}");
                else
                    Log($"[Progress] {operation}: {Math.Max(Math.Min(number, 100), 0)}%");

                prev_progress = number;
                prev_time = DateTime.UtcNow;
            }
        }

        public void LogProgress(string operation, string str)
        {
            if (DisableLogs) return;

            Log($"[Progress] {operation}: {str}");
        }

        public void Log(string txt)
        {
            if (DisableLogs) return;

            var isFirst = true;
            txt = string.Join("\n",
                txt.Split('\n').
                Select((t) =>
                {
                    var res = $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]{(isFirst ? "\t" : "-\t")}{t}";
                    isFirst = false;
                    return res;
                }));

            Console.WriteLine(txt);
        }

        public void Log(Exception ex)
        {
            if (DisableLogs) return;

            Log($"Exception:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}");
        }

        public PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (DisableLogs) return PCKDialogResult.OK;

            Log($"[{messageType}] \"{title}\": {text}");

#if DEV_ENABLED
            System.Diagnostics.Debugger.Break();
#endif
            return PCKDialogResult.OK;
        }

        public PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (DisableLogs) return PCKDialogResult.OK;

            var res = ShowMessage(ex.Message, ex.GetType().Name, messageType, boxButtons);
            Log(ex);
            return res;
        }
    }

}
