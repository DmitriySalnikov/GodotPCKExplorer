using System;
using System.Threading;

namespace GodotPCKExplorer.UI
{
    public class DeferredAction : IDisposable
    {
        Timer close_timer = null;
        Action action = null;
        int delay = 1000;

        public DeferredAction(Action action, int delay = 1000)
        {
            this.action = action ?? throw new ArgumentNullException("action");
            this.delay = delay;
        }

        public bool IsTimerActive()
        {
            return close_timer != null;
        }

        public void Cancel()
        {
            close_timer?.Dispose();
            close_timer = null;
        }

        public void CallDeferred()
        {
            close_timer?.Dispose();
            close_timer = new Timer(CallAction, null, delay, Timeout.Infinite);
        }

        void CallAction(object obj)
        {
            close_timer?.Dispose();
            close_timer = null;

            if (action != null)
            {
                action();
            }
        }

        ~DeferredAction()
        {
            Dispose();
        }

        public void Dispose()
        {
            close_timer?.Dispose();
            close_timer = null;
            action = null;
        }
    }
}
