using Microsoft.Win32;
using WinShellContextMenuRegistrator;

namespace GodotPCKExplorer.UI
{
    static class ShellIntegration
    {
        static string AppPath = "this_app";
        const string BasePath = @"Software\Classes\";
        const string PckFile = ".pck";
        const string PckApp = "GodotPCKExplorer";

        const string PckExtractCommandName = "Extract Godot .pck file here";
        const string PckExtractCommandCodeName = "extract";
        const string PckExtractCMDArgs = "-e \"%1\" \"%1_extracted\"";

        const string PckOpenCommandName = "Open Godot .pck file";
        const string PckOpenCommandCodeName = "open";
        const string PckOpenCMDArgs = "-o \"%1\"";

        public static void Register()
        {
            AppPath = Application.ExecutablePath;

            try
            {
                ContextMenuRegistrator.RegisterContexMenuCommand(RegistryBranch.CURRENT_USER, AppPath, PckApp, PckOpenCommandName, PckOpenCommandCodeName, PckFile, PckOpenCMDArgs);
                ContextMenuRegistrator.RegisterContexMenuCommand(RegistryBranch.CURRENT_USER, AppPath, PckApp, PckExtractCommandName, PckExtractCommandCodeName, PckFile, PckExtractCMDArgs);
                ContextMenuRegistrator.RegisterContexMenuNameForApp(RegistryBranch.CURRENT_USER, PckApp, "Godot Engine PCK");
                MessageBox.Show("Successful registered .pck files");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public static void Unregister()
        {
            RemoveKey(BasePath, PckApp);
            RemoveKey(BasePath, PckFile);

            MessageBox.Show("Successful unregistered .pck files");
        }

        public static bool IsRegistered()
        {
            return HasKey(BasePath, PckApp) || HasKey(BasePath, PckFile);
        }

        private static void RemoveKey(string folder, string subKey)
        {
            using var key = Registry.CurrentUser.OpenSubKey(folder, true);

            if (key != null)
            {
                try
                {
                    key.DeleteSubKeyTree(subKey);
                }
                catch { }
            }
        }

        private static bool HasKey(string folder, string subKey)
        {
            using var key = Registry.CurrentUser.OpenSubKey(folder, true);
            if (key != null)
            {
                return key.GetSubKeyNames().Contains(subKey);
            }
            return false;
        }
    }
}
