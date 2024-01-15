// https://github.com/DmitriySalnikov/WinShellContextMenuRegistrator

using Microsoft.Win32;

namespace WinShellContextMenuRegistrator
{
    /// <summary>
    /// Branch of Windows Registry HKEY_*
    /// </summary>
    public enum RegistryBranch
    {
        CLASSES_ROOT,
        CURRENT_USER,
        LOCAL_MACHINE,
        USERS,
        CURRENT_CONFIG,
    }

    public static class ContextMenuRegistrator
    {
        private static string GetBranchString(RegistryBranch branch)
        {
            string branchStr = "";
            switch (branch)
            {
                case RegistryBranch.CLASSES_ROOT: branchStr = "HKEY_CLASSES_ROOT"; break;
                case RegistryBranch.CURRENT_CONFIG: branchStr = "HKEY_CURRENT_CONFIG"; break;
                case RegistryBranch.CURRENT_USER: branchStr = "HKEY_CURRENT_USER"; break;
                case RegistryBranch.LOCAL_MACHINE: branchStr = "HKEY_LOCAL_MACHINE"; break;
                case RegistryBranch.USERS: branchStr = "HKEY_USERS"; break;
            }
            return branchStr;
        }

        /// <summary>
        /// Register Windows Context Menu Action.
        /// </summary>
        /// <param name="RegBranch">Branch of registry</param>
        /// <param name="AppPath">Path to your application</param>
        /// <param name="AppCodeName">Code name of your app in registry for create link between file format and app</param>
        /// <param name="AppCommandName">The name of the command that the user sees in the context menu</param>
        /// <param name="AppCommandCodeName">Code name of cammand  in registry. For example, "open" for default Open operation.</param>
        /// <param name="FileExtension">File extension. For example, ".jpg" or ".png".</param>
        /// <param name="AppCMDArguments">Command line arguments. For example, "-Extract \"%1\" \"%1_extracted\"", where %1 is file with your extension</param>
        /// <param name="AppIconPath">Command icon. For example, icons in "Create" menu</param>
        public static void RegisterContexMenuCommand(RegistryBranch RegBranch, string AppPath, string AppCodeName, string AppCommandName, string AppCommandCodeName, string FileExtension, string AppCMDArguments, string? AppIconPath = null)
        {
            string branchStr = GetBranchString(RegBranch);
            string iconPath = AppIconPath ?? AppPath;
            string classesPath = branchStr + @"\Software\Classes\";
            string commandPath = classesPath + AppCodeName + @"\shell\" + AppCommandCodeName;

            Registry.SetValue(classesPath + FileExtension, null, AppCodeName);
            Registry.SetValue(commandPath, null, AppCommandName);
            Registry.SetValue(commandPath, "Icon", iconPath);
            Registry.SetValue(commandPath + @"\command", null, string.Format("\"{0}\" {1}", AppPath, AppCMDArguments));
        }

        /// <summary>
        /// Register default name for program
        /// </summary>
        /// <param name="RegBranch">Branch of registry</param>
        /// <param name="AppCodeName">Code name of your app in registry</param>
        /// <param name="Name">Name what user see in context menu</param>
        public static void RegisterContexMenuNameForApp(RegistryBranch RegBranch, string AppCodeName, string Name)
        {
            string branchStr = GetBranchString(RegBranch);
            string classesPath = branchStr + @"\Software\Classes\";
            Registry.SetValue(classesPath + AppCodeName, null, Name);
        }

        /// <summary>
        /// Register default icon for program
        /// </summary>
        /// <param name="RegBranch">Branch of registry</param>
        /// <param name="AppCodeName">Code name of your app in registry</param>
        /// <param name="DefaultIconPath">Icon path</param>
        public static void RegisterContexMenuDefaultIcon(RegistryBranch RegBranch, string AppCodeName, string DefaultIconPath)
        {
            string branchStr = GetBranchString(RegBranch);
            string classesPath = branchStr + @"\Software\Classes\";
            Registry.SetValue(classesPath + AppCodeName + @"\DefaultIcon", null, DefaultIconPath);
        }

        /// <summary>
        /// Register create action for file extension
        /// </summary>
        /// <param name="RegBranch">Branch of registry</param>
        /// <param name="FileExtension">File extension </param>
        /// <param name="DefaultFilePath">Path to file with icon. Can be like "C:/icon.ico" or "C:/icon.ico,0"</param>
        public static void RegisterContexMenuCreateAction(RegistryBranch RegBranch, string FileExtension, string DefaultFilePath)
        {
            string branchStr = GetBranchString(RegBranch);
            string classesPath = branchStr + @"\Software\Classes\";
            Registry.SetValue(classesPath + FileExtension + @"\ShellNew", "FileName", DefaultFilePath);
        }
    }
}
