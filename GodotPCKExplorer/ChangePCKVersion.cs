using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GodotPCKExplorer
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
                    if (pck.OpenFile(filePath))
                    {
                        FilePath = filePath;
                        var ver = pck.PCK_Version;
                        l_path.Text = $"File Path:\n{Utils.GetShortPath(filePath, 50)}";
                        l_version.Text = $"Original Version:\n{ver}";

                        cb_ver.Text = ver.PackVersion.ToString();
                        nud_major.Value = ver.Major;
                        nud_minor.Value = ver.Minor;
                        nud_revision.Value = ver.Revision;
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
                Utils.CommandLog($"Specified file does not exists! '{filePath}'", "Error", false);
                return;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (PCKActions.ChangePCKVersion(FilePath, $"{cb_ver.Text}.{nud_major.Value}.{nud_minor.Value}.{nud_revision.Value}"))
                Close();
        }
    }
}
