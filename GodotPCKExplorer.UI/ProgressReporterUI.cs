using System;
using System.Windows.Forms;

namespace GodotPCKExplorer.UI
{
    class ProgressReporterUI : IProgressReporter
    {
        int prev_progress = 0;
        DateTime prev_time = DateTime.Now;

        public void Log(string txt)
        {
            Program.Log(txt);
        }

        public void Log(Exception ex)
        {
            Program.Log(ex);
        }

        public void LogProgress(string operation, int percent)
        {
            if (((DateTime.Now - prev_time).TotalSeconds > 1) || (prev_progress != percent && percent - prev_progress >= 5))
            {
                Program.LogProgress(operation, $"{Math.Max(Math.Min(percent, 100), 0)}%");

                prev_progress = percent;
                prev_time = DateTime.Now;
            }
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
