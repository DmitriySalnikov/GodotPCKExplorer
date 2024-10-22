using System.Security.Cryptography;

namespace GodotPCKExplorer.UI
{
    public partial class CreatePCKEncryption : Form
    {
        public CreatePCKEncryption()
        {
            InitializeComponent();

            tb_key.Text = GUIConfig.Instance.PackEncryptionKey;
            cb_encrypt_index.Checked = GUIConfig.Instance.PackEncryptIndex;
            cb_encrypt_files.Checked = GUIConfig.Instance.PackEncryptFiles;
        }

        private void btn_ok_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_key.Text) && !PCKUtils.HexStringValidate(tb_key.Text, 256 / 8))
            {
                Program.ShowMessage($"The key\n\"{tb_key.Text}\"\nis not a valid key!\nThe key must be valid AES 256 bits.", "Invalid key");
                return;
            }

            GUIConfig.Instance.PackEncryptionKey = tb_key.Text;
            GUIConfig.Instance.PackEncryptIndex = cb_encrypt_index.Checked;
            GUIConfig.Instance.PackEncryptFiles = cb_encrypt_files.Checked;
            GUIConfig.Instance.Save();

            Close();
        }

        private void button1_Click(object? sender, EventArgs e)
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            tb_key.Text = PCKUtils.ByteArrayToHexString(aes.Key);
        }
    }
}
