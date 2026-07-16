using System.Diagnostics;

namespace GodotPCKExplorer.UI
{
    public partial class OpenWithPCKEncryption : Form
    {
        public string EncryptionKey = "";

        public OpenWithPCKEncryption(string key = "")
        {
            InitializeComponent();
            ThemeManager.Apply(this);

            tb_key.Text = key;
            DialogResult = DialogResult.Cancel;
        }

        void CheckKeyAndClose()
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

        private void btn_ok_Click(object? sender, EventArgs e)
        {
            CheckKeyAndClose();
        }

        private void tb_key_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CheckKeyAndClose();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/DmitriySalnikov/GodotPCKExplorer/blob/master/Bruteforcer/README.md") { UseShellExecute = true });
        }
    }
}
