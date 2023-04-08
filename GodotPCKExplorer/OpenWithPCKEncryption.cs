using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    public partial class OpenWithPCKEncryption : Form
    {
        public string EncryptionKey = "";

        public OpenWithPCKEncryption(string key = "")
        {
            InitializeComponent();

            tb_key.Text = key;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_key.Text) && !Utils.HexStringValidate(tb_key.Text, 256 / 8))
            {
                Utils.ShowMessage($"The key\n\"{tb_key.Text}\"\nis not a valid key!\nThe key must be valid AES 256 bits.", "Invalid key");
                return;
            }

            EncryptionKey = tb_key.Text;

            Close();
        }
    }
}
