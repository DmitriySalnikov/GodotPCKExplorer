using GodotPCKExplorer.GlobalShared;
using Newtonsoft.Json;

namespace GodotPCKExplorer.UI
{
    class RecentFiles(string path, bool isEncrypted, string encryptionKey)
    {
        public string Path = path;
        public bool IsEncrypted = isEncrypted;
        public string EncryptionKey = encryptionKey;
    }

    class GUIConfig
    {
        [JsonIgnore]
        public static GUIConfig Instance { get; private set; } = new();

        [JsonIgnore]
        static string SaveFile = Path.Combine(GlobalConstants.AppDataPath, "settings.json");

        #region Packing

        public PCKVersion PackedVersion { get; set; } = new PCKVersion(2, 4, 1, 1);
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

        public void Save()
        {
            try
            {
                File.WriteAllText(SaveFile, JsonConvert.SerializeObject(this, Formatting.Indented));
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
                    Instance = JsonConvert.DeserializeObject<GUIConfig>(File.ReadAllText(SaveFile)) ?? new();

                Instance ??= new();
            }
            catch (Exception ex)
            {
                Program.Log(ex);
            }
        }

        #endregion

#if false
        // https://stackoverflow.com/questions/25749509/how-can-i-tell-json-net-to-ignore-properties-in-a-3rd-party-object
        public class ShouldSerializeContractResolver : DefaultContractResolver
        {
            public static ShouldSerializeContractResolver Instance { get; } = new ShouldSerializeContractResolver();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (property.DeclaringType == typeof(PCKVersion) && property.PropertyType != typeof(int))
                {
#if DEV_ENABLED
                    Console.WriteLine($"Ignoring {member.DeclaringType}.{member.Name} on save");
#endif
                    property.Ignored = true;
                }
                return property;
            }
        }
#endif
    }
}
