using GodotPCKExplorer;

namespace PCKBruteforcer
{
    public sealed class ProgressReporterBrute : BasicPCKProgressReporter, IPCKProgressReporter
    {
        [ThreadStatic]
        public static bool DisableLogs;

        public override void LogProgress(string operation, int number, string? customPrefix = null)
        {
            if (DisableLogs) return;
            base.LogProgress(operation, number, customPrefix);
        }

        public override void LogProgress(string operation, string str)
        {
            if (DisableLogs) return;
            base.LogProgress(operation, str);
        }

        public override void Log(string txt)
        {
            if (DisableLogs) return;
            base.Log(txt);
        }

        public override void Log(Exception ex)
        {
            if (DisableLogs) return;
            base.Log(ex);
        }

        public override PCKDialogResult ShowMessage(string text, string title, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (DisableLogs) return PCKDialogResult.OK;
            return base.ShowMessage(text, title, messageType, boxButtons);
        }

        public override PCKDialogResult ShowMessage(Exception ex, MessageType messageType = MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (DisableLogs) return PCKDialogResult.OK;
            return base.ShowMessage(ex, messageType, boxButtons);
        }
    }
}
