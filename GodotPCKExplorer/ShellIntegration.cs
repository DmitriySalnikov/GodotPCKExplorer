using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using WinShellContextMenuRegistrator;

namespace GodotPCKExplorer
{
	static class ShellIntegration
	{
		static string AppPath = "this_app";
		static string BasePath = @"Software\Classes\";
		static string PckFile = ".pck";
		static string PckApp = "GodotPCKExplorer";

		static string PckExtractCommandName = "Extract Godot .pck file here";
		static string PckExtractCommandCodeName = "extract";
		static string PckExtractCMDArgs = "-e \"%1\" \"%1_extracted\"";

		static string PckOpenCommandName = "Open Godot .pck file";
		static string PckOpenCommandCodeName = "open";
		static string PckOpenCMDArgs = "-o \"%1\"";

		public static void Register()
		{
			AppPath = Application.ExecutablePath;

			try
			{
				ContextMenuRegistrator.RegisterContexMenuCommand(RegistryBranch.CURRENT_USER, AppPath, PckApp, PckOpenCommandName, PckOpenCommandCodeName, PckFile, PckOpenCMDArgs);
				ContextMenuRegistrator.RegisterContexMenuCommand(RegistryBranch.CURRENT_USER, AppPath, PckApp, PckExtractCommandName, PckExtractCommandCodeName, PckFile, PckExtractCMDArgs);
				ContextMenuRegistrator.RegisterContexMenuNameForApp(RegistryBranch.CURRENT_USER, PckApp, "Godot Engine PCK pack");
				MessageBox.Show("Successful registered .pck files");
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error");
			}
		}

		public static void Unregister()
		{
			RemoveKey(BasePath, PckApp);
			RemoveKey(BasePath, PckFile);

			MessageBox.Show("Successful unregistered .pck files");
		}

		private static void RemoveKey(string folder, string subKey)
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(folder, true))
			{
				if (key != null)
				{
					try
					{
						key.DeleteSubKeyTree(subKey);
					}
					catch
					{

					}
				}
			}
		}
	}
}
