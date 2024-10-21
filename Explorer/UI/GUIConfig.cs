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

        public PCKVersion PackedVersion { get; set; } = new PCKVersion(2, 4, 1, 1);
        public bool EmbedPCK { get; set; } = false;
        public string FolderPath { get; set; } = "";
        public string PackPathPrefix { get; set; } = "";
        public bool MatchCaseFilterPackingForm { get; set; } = false;
        public uint PCKAlignment { get; set; } = 16;
        public bool PackOnlyFiltered { get; set; } = true;
        public bool PreviewPaths { get; set; } = false;

        #region Encryption

        public bool EncryptPCK { get; set; } = false;
        public string EncryptionKey { get; set; } = "";
        public bool EncryptIndex { get; set; } = false;
        public bool EncryptFiles { get; set; } = false;

        #endregion

        #endregion

        #region Extract

        public bool OverwriteExtracted { get; set; } = false;
        public bool CheckMD5Extracted { get; set; } = true;
        public PCKExtractNoEncryptionKeyMode IfNoEncryptionKeyMode { get; set; } = PCKExtractNoEncryptionKeyMode.Cancel;

        #endregion

        #region Main Window

        public List<RecentFiles> RecentOpenedFiles { get; set; } = [];
        public bool MatchCaseFilterMainForm { get; set; } = false;
        public bool ShowConsole { get; set; } = false;
        public string SkipVersion { get; set; } = "";

        #endregion

        #region Save/Load

        GUIConfig()
        {
            Instance ??= this;
        }

        public GUIConfig(
            PCKVersion packedVersion,
            bool embedPCK,
            string folderPath,
            string packPathPrefix,
            bool matchCaseFilterPackingForm,
            uint pckAlignment,
            bool packOnlyFiltered,
            bool previewPaths,

            bool encryptPCK,
            string encryptionKey,
            bool encryptIndex,
            bool encryptFiles,

            bool overwriteExtracted,
            bool checkMD5Extracted,
            PCKExtractNoEncryptionKeyMode ifNoEncryptionKeyMode,

            List<RecentFiles> recentOpenedFiles,
            bool matchCaseFilterMainForm,
            bool showConsole,
            string skipVersion
            )
        {
            PackedVersion = packedVersion;
            EmbedPCK = embedPCK;
            FolderPath = folderPath;
            PackPathPrefix = packPathPrefix;
            MatchCaseFilterPackingForm = matchCaseFilterPackingForm;
            PCKAlignment = pckAlignment;
            PackOnlyFiltered = packOnlyFiltered;
            PreviewPaths = previewPaths;

            EncryptPCK = encryptPCK;
            EncryptionKey = encryptionKey;
            EncryptIndex = encryptIndex;
            EncryptFiles = encryptFiles;

            OverwriteExtracted = overwriteExtracted;
            CheckMD5Extracted = checkMD5Extracted;
            IfNoEncryptionKeyMode = ifNoEncryptionKeyMode;

            RecentOpenedFiles = recentOpenedFiles;
            MatchCaseFilterMainForm = matchCaseFilterMainForm;
            ShowConsole = showConsole;
            SkipVersion = skipVersion;
        }

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
