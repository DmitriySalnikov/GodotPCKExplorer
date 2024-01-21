using System.Diagnostics;

namespace GodotPCKExplorer.UI
{
    public partial class OpenWithPCKEncryption : Form
    {
        public string EncryptionKey = "";

        public OpenWithPCKEncryption(string key = "")
        {
            InitializeComponent();

            tb_key.Text = key;
            DialogResult = DialogResult.Cancel;
        }

        private void btn_ok_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_key.Text) && !PCKUtils.HexStringValidate(tb_key.Text, 256 / 8))
            {
                Program.ShowMessage($"The key\n\"{tb_key.Text}\"\nis not a valid key!\nThe key must be valid AES 256 bits.", "Invalid key");
                return;
            }

            EncryptionKey = tb_key.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/DmitriySalnikov/GodotPCKExplorer/blob/master/Bruteforcer/README.md") { UseShellExecute = true });
        }
    }
}
