using System.Runtime.InteropServices;

namespace GodotPCKExplorer.UI
{
    // https://stackoverflow.com/a/50920121/8980874

    [ToolboxBitmap(typeof(ToolStripTextBox))]
    public class TextBoxWithPlaceholder : TextBox
    {
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        public TextBoxWithPlaceholder()
        {
            this.HandleCreated += Control_HandleCreated;
        }

        private void Control_HandleCreated(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cueBanner))
                UpdateCueBanner();
        }

        string cueBanner = "";
        public string CueBanner
        {
            get { return cueBanner; }
            set
            {
                cueBanner = value;
                UpdateCueBanner();
            }
        }

        private void UpdateCueBanner()
        {
            SendMessage(this.Handle, EM_SETCUEBANNER, 0, cueBanner);
        }
    }
}