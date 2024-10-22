using GodotPCKExplorer.GlobalShared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GodotPCKExplorer.UI
{
    class RecentFiles(string path, bool isEncrypted, string encryptionKey)
    {
        public string Path { get; set; } = path;
        public bool IsEncrypted { get; set; } = isEncrypted;
        public string EncryptionKey { get; set; } = encryptionKey;
    }

    class GUIConfig
    {
        [JsonIgnore]
        public static GUIConfig Instance { get; private set; } = new();

        [JsonIgnore]
        static readonly string SaveFile = Path.Combine(GlobalConstants.AppDataPath, "settings.json");

        #region Packing

        public PCKVersion PackVersion { get; set; } = new PCKVersion(2, 4, 1, 1);
        public bool PackEmbedPCK { get; set; } = false;
        public string PackFolderPath { get; set; } = "";
        public string PackPathPrefix { get; set; } = "";
        public bool MatchCaseFilterPackingForm { get; set; } = false;
        public uint PackPCKAlignment { get; set; } = 16;
        public bool PackOnlyFiltered { get; set; } = false;
        public bool PackPreviewPaths { get; set; } = true;
        public bool PackPatchingEnabled { get; set; } = false;
        public string PackPatchingTarget { get; set; } = "";

        #region Encryption

        public bool PackEncryptPCK { get; set; } = false;
        public string PackEncryptionKey { get; set; } = "";
        public bool PackEncryptIndex { get; set; } = false;
        public bool PackEncryptFiles { get; set; } = false;

        #endregion

        #endregion

        #region Extract

        public bool ExtractOverwrite { get; set; } = false;
        public bool ExtractCheckMD5 { get; set; } = true;
        public PCKExtractNoEncryptionKeyMode ExtractIfNoEncryptionKeyMode { get; set; } = PCKExtractNoEncryptionKeyMode.Cancel;

        #endregion

        #region Main Window

        public List<RecentFiles> MainFormRecentOpenedFiles { get; set; } = [];
        public bool MainFormMatchCaseFilter { get; set; } = false;
        public bool MainFormShowConsole { get; set; } = false;
        public string SkipVersion { get; set; } = "";

        #endregion

        #region Save/Load

        [JsonIgnore]
        readonly JsonSerializerOptions serializerOptions = new() { WriteIndented = true, IncludeFields = true };
        public void Save()
        {
            try
            {
                File.WriteAllText(SaveFile, JsonSerializer.Serialize(this, serializerOptions));
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(SaveFile))
                    Instance = JsonSerializer.Deserialize<GUIConfig>(File.ReadAllText(SaveFile)) ?? new();

                Instance ??= new();
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        #endregion
    }
}
