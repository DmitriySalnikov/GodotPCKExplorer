namespace GodotPCKExplorer.GlobalShared
{
    public static class GlobalConstants
    {
        public static readonly string ProjectName = "GodotPCKExplorer";
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName);
    }
}
