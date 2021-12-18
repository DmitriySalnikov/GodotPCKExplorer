using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GodotPCKExplorer
{
    class GUIConfig
    {
        [JsonIgnore]
        static public GUIConfig Instance { get; private set; } = null;
        
        [JsonIgnore]
        static string SaveFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "settings.json");

        #region Packing

        public PCKVersion PackedVersion { get; set; } = new PCKVersion(1, 3, 4, 0);
        public bool EmbedPCK { get; set; } = false;
        public string FolderPath { get; set; } = "";
        public bool MatchCaseFilterPackingForm { get; set; } = true;

        #endregion

        #region Extract

        public bool OverwriteExtracted { get; set; } = true;

        #endregion

        #region Main Window

        public List<string> RecentOpenedFiles { get; set; } = new List<string>();
        public bool MatchCaseFilterMainForm { get; set; } = true;

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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}
