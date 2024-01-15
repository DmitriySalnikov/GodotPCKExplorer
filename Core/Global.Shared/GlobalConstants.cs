namespace GodotPCKExplorer.GlobalShared
{
    public class GlobalConstants
    {
        public static readonly string ProjectName = "GodotPCKExplorer";
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName);
    }
}
