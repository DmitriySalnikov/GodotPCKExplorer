namespace GodotPCKExplorer.UI
{
    public partial class ChangePCKVersion : Form
    {
        string FilePath = "";

        public ChangePCKVersion()
        {
            InitializeComponent();
        }

        public void ShowAndOpenFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var pck = new PCKReader())
                {
                    if (pck.OpenFile(filePath, logFileNamesProgress: false, readOnlyHeaderGodot4: true))
                    {
                        FilePath = filePath;
                        var ver = pck.PCK_Version;
                        l_path.Text = $"File Path:\n{Utils.GetShortPath(filePath, 50)}";
                        l_version.Text = $"Original Version:\n{ver}";

                        pckVersionSelector1.SetVersion(ver);
                    }
                    else
                    {
                        return;
                    }
                }
                ShowDialog();
                return;
            }
            else
            {
                Program.ShowMessage($"Specified file does not exists! '{filePath}'", "Error", MessageType.Error);
                return;
            }
        }

        private void btn_ok_Click(object? sender, EventArgs e)
        {
            var ver = pckVersionSelector1.GetVersion();
            if (!ver.IsValid())
                return;

            if (PCKActions.ChangeVersion(FilePath, $"{ver}"))
                Close();
        }
    }
}
