using System;
using System.Globalization;
using System.Linq;

namespace GodotPCKExplorer
{
    public enum PCKMessageBoxIcon : byte
    {
        None = 0,
        Hand = 0x10,
        Question = 0x20,
        Exclamation = 48,
        Asterisk = 0x40,
        Stop = 0x10,
        Error = 0x10,
        Warning = 48,
        Information = 0x40
    }

    public enum PCKMessageBoxButtons : byte
    {
        OK,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel
    }

    public enum PCKDialogResult : byte
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public interface IPCKProgressReporter
    {
        /// <summary>
        /// Output percent or some number if prefix is not null
        /// </summary>
        /// <param name="operation">current operation name</param>
        /// <param name="number">number to print</param>
        /// <param name="customPrefix">number prefix. If null, output percentages from 0 to 100. Otherwise - any number with a prefix.</param>
        void LogProgress(string operation, int number, string? customPrefix = null);

        void LogProgress(string operation, string str);

        void Log(string txt);

        void Log(Exception ex);

        PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK);

        PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK);
    }

    public class BasicPCKProgressReporter : IPCKProgressReporter
    {
        int prev_progress = 0;
        DateTime prev_time = DateTime.UtcNow;
        public Action<string>? PrintLogText;

        public void LogProgress(string operation, int number, string? customPrefix = null)
        {
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
            Log($"[Progress] {operation}: {str}");
        }

        public void Log(string txt)
        {
            var isFirst = true;

            if (PrintLogText != null)
            {
                PrintLogText?.Invoke(txt);
            }
            else
            {
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
        }

        public void Log(Exception ex)
        {
            Log($"[!] {ex.GetType().Name}:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}");
        }

        public PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            Log($"[{messageType}] \"{title}\": {text}");

#if DEV_ENABLED
            System.Diagnostics.Debugger.Break();
#endif
            return PCKDialogResult.OK;
        }

        public PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            var res = ShowMessage(ex.Message, ex.GetType().Name, messageType, boxButtons);
            Log(ex);
            return res;
        }
    }
}
