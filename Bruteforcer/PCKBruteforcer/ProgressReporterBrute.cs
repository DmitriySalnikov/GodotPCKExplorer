using GodotPCKExplorer;
using static System.Net.Mime.MediaTypeNames;

namespace PCKBruteforcer
{
    public class ProgressReporterBrute : BasicPCKProgressReporter, IPCKProgressReporter
    {
        [ThreadStatic]
        public static bool DisableLogs;

        public new void LogProgress(string operation, int number, string? customPrefix = null)
        {
            if (DisableLogs) return;
            base.LogProgress(operation, number, customPrefix);
        }

        public new void LogProgress(string operation, string str)
        {
            if (DisableLogs) return;
            base.LogProgress(operation, str);
        }

        public new void Log(string txt)
        {
            if (DisableLogs) return;
            base.Log(txt);
        }

        public new void Log(Exception ex)
        {
            if (DisableLogs) return;
            base.Log(ex);
        }

        public new PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (DisableLogs) return PCKDialogResult.OK;
            return base.ShowMessage(text, title, messageType, boxButtons);
        }

        public new PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (DisableLogs) return PCKDialogResult.OK;
            return base.ShowMessage(ex, messageType, boxButtons);
        }
    }

}
