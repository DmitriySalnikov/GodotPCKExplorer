namespace GodotPCKExplorer.UI
{
    class ProgressReporterUI : IPCKProgressReporter
    {
        public void Log(string txt)
        {
            Program.Log(txt);
        }

        public void Log(Exception ex)
        {
            Program.Log(ex);
        }

        public void LogProgress(string operation, int number, string? customPrefix = null)
        {
            Program.LogProgress(operation, number, customPrefix);
        }

        public void LogProgress(string operation, string str)
        {
            Program.LogProgress(operation, str);
        }

        public PCKDialogResult ShowMessage(string text, string title, GodotPCKExplorer.MessageType messageType = GodotPCKExplorer.MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (Program.IsMessageBoxesEnabled())
            {
                return (PCKDialogResult)Program.ShowMessage(text, title, (MessageType)messageType, (MessageBoxButtons)boxButtons);
            }

            Program.Log(text);
            return PCKDialogResult.OK;
        }

        public PCKDialogResult ShowMessage(Exception ex, GodotPCKExplorer.MessageType messageType = GodotPCKExplorer.MessageType.None, PCKMessageBoxButtons boxButtons = PCKMessageBoxButtons.OK)
        {
            if (Program.IsMessageBoxesEnabled())
            {
                return (PCKDialogResult)Program.ShowMessage(ex, (MessageType)messageType, (MessageBoxButtons)boxButtons);
            }

            Program.Log(ex);
            return PCKDialogResult.OK;
        }
    }
}
