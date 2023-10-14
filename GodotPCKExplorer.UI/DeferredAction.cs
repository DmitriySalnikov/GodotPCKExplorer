namespace GodotPCKExplorer.UI
{
    public class DeferredAction : IDisposable
    {
        System.Threading.Timer? close_timer = null;
        Action action;
        readonly int delay = 1000;

        public DeferredAction(Action action, int delay = 1000)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
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
            close_timer = new System.Threading.Timer(CallAction, null, delay, Timeout.Infinite);
        }

        void CallAction(object? obj)
        {
            close_timer?.Dispose();
            close_timer = null;

            action();
        }

        ~DeferredAction()
        {
            Dispose();
        }

        public void Dispose()
        {
            close_timer?.Dispose();
            close_timer = null;
        }
    }
}
