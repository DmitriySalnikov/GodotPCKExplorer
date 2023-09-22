using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GodotPCKExplorer.UI
{
    class RecentFiles
    {
        public string Path;
        public string EncryptionKey;

        public RecentFiles(string path, string encKey)
        {
            Path = path;
            EncryptionKey = encKey;
        }
    }

    class GUIConfig
    {
        [JsonIgnore]
        static public GUIConfig Instance { get; private set; } = null;

        [JsonIgnore]
        static string SaveFile = Path.Combine(Program.AppDataPath, "settings.json");

        #region Packing

        public PCKVersion PackedVersion { get; set; } = new PCKVersion(1, 3, 4, 0);
        public bool EmbedPCK { get; set; } = false;
        public string FolderPath { get; set; } = "";
        public bool MatchCaseFilterPackingForm { get; set; } = false;
        public uint PCKAlignment { get; set; } = 16;

        #region Encryption

        public bool EncryptPCK { get; set; } = false;
        public string EncryptionKey { get; set; } = "";
        public bool EncryptIndex { get; set; } = false;
        public bool EncryptFiles { get; set; } = false;

        #endregion

        #endregion

        #region Extract

        public bool OverwriteExtracted { get; set; } = true;
        public bool CheckMD5Extracted { get; set; } = true;

        #endregion

        #region Main Window

        public List<RecentFiles> RecentOpenedFiles { get; set; } = new List<RecentFiles>();
        public bool MatchCaseFilterMainForm { get; set; } = false;
        public bool ShowConsole { get; set; } = false;

        #endregion

        #region Save/Load

        GUIConfig()
        {
            if (Instance == null)
                Instance = this;
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(SaveFile, Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        static public void Load()
        {
            try
            {
                if (File.Exists(SaveFile))
                    Instance = Newtonsoft.Json.JsonConvert.DeserializeObject<GUIConfig>(File.ReadAllText(SaveFile));

                if (Instance == null)
                    Instance = new GUIConfig();
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        #endregion
    }
}
