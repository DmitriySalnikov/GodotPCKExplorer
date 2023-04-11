using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GodotPCKExplorer
{
    public class LogProgressReporter : IDisposable
    {
        string operation;
        Timer anti_spam_timer = null;
        bool can_write = true;
        int prev_progress = 0;

        public LogProgressReporter(string operation)
        {
            this.operation = operation;
            anti_spam_timer = new Timer(timer_callback);
        }

        void timer_callback(object state)
        {

        }

        public void Dispose()
        {
            anti_spam_timer.Dispose();
        }

        public void ReportProgress(int percent)
        {
            if (can_write || (prev_progress != percent && percent % 5 == 0))
            {
                Program.LogProgress(operation, $"{Math.Max(Math.Min(percent, 100), 0)}%");
                
                prev_progress = percent;
                can_write = false;
                anti_spam_timer.Change(1000, Timeout.Infinite);
            }
        }
    }
}
