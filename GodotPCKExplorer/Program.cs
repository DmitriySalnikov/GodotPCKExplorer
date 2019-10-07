using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GodotPCKExplorer
{
	static class Program
	{
		public const int PCK_MAGIC = 0x43504447;
		const string quote_string_pattern = "\"(?:[^\"\\\\]|\\\\.)*\"";
		static Regex QuoteStringRegEx = new Regex(quote_string_pattern);
		const string version_string_pattern = @"([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})";
		static Regex VersionStringRegEx = new Regex(version_string_pattern);

		static string ValidCommands = $"Example of valid arguments:\nOpen file '-o \"C:\\Game.pck\"'\nExport files '-e \"C:\\Game.pck\" \"C:\\Export Directory\"'\nPack files (version can be ignored if you have saved {CreatePCKFile.GodotVersionSave}) '-p \"C:\\Directory with files\" \"C:\\GameNew.pck\" 1.3.2.0'";
		public static bool CMDMode = false;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//"C:\My\Projects\VS\GodotPCKExplorer\GodotPCKExplorer\bin\Debug\GodotPCKExplorer.exe" -o "C:\My\Projects\GE\C# Game.pck"
			string[] args = null;
			bool run_with_args = false;

			// -o Open pack
			{
				args = Environment.CommandLine.Split(new string[] { " -o " }, StringSplitOptions.None);
				if (args.Length > 1)
				{
					run_with_args = true;

					string path;
					try
					{
						var match = QuoteStringRegEx.Match(args[1]);
						if (match.Success)
						{
							path = Path.GetFullPath(match.Value.Replace("\"", ""));
						}
						else
						{
							Console.WriteLine($"Not valid file path: {args[1]}\n" + ValidCommands);
							return;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error in file path: {args[1]}\n{e.Message}" + ValidCommands);
						return;
					}

					if (path != null)
					{
						if (File.Exists(path))
						{
							var form = new Form1();
							form.OpenFile(path);

							Application.Run(form);
						}
						else
						{
							Console.WriteLine($"Specified file does not exists! {args[1]}\n" + ValidCommands);
							return;
						}
					}
				}
			}
			// -e Extract all from pack to folder
			{
				args = Environment.CommandLine.Split(new string[] { " -e " }, StringSplitOptions.None);
				if (args.Length > 1)
				{
					run_with_args = true;
					CMDMode = true;

					string filePath;
					string dirPath;
					try
					{
						var matches = QuoteStringRegEx.Matches(args[1]);
						if (matches.Count == 2)
						{
							filePath = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
							dirPath = Path.GetFullPath(matches[1].Value.Replace("\"", ""));
						}
						else
						{
							Console.WriteLine($"Error path to file or directory not specified!\n" + ValidCommands);
							return;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error: {e.Message}\n" + ValidCommands);
						return;
					}

					if (File.Exists(filePath))
					{
						var pckReader = new PCKReader();
						pckReader.OpenFile(filePath);
						pckReader.ExtractAllFiles(dirPath);
						pckReader.Close();
					}
					else
					{
						Console.WriteLine($"Specified file does not exists! '{filePath}'\n" + ValidCommands);
						return;
					}
				}
			}
			// -p Pack PCK from file
			{
				args = Environment.CommandLine.Split(new string[] { " -p " }, StringSplitOptions.None);
				if (args.Length > 1)
				{
					run_with_args = true;
					CMDMode = true;

					string dirPath;
					string filePath;
					try
					{
						var matches = QuoteStringRegEx.Matches(args[1]);
						if (matches.Count == 2)
						{
							dirPath = Path.GetFullPath(matches[0].Value.Replace("\"", ""));
							filePath = Path.GetFullPath(matches[1].Value.Replace("\"", ""));
						}
						else
						{
							Console.WriteLine($"Error path to file or directory not specified!\n" + ValidCommands);
							return;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine($"Error: {e.Message}\n" + ValidCommands);
						return;
					}

					if (Directory.Exists(dirPath))
					{
						var pckPacker = new PCKPacker();

						var files = new List<PCKPacker.FileToPack>();
						ScanFoldersForFiles(dirPath, files, ref dirPath);

						var ver = new PCKPacker.PCKVersion();

						var tmpCheck = VersionStringRegEx.Match(args[1].Split('\"').Last());
						if (tmpCheck.Success)
						{
							var digits = tmpCheck.Value.Split('.');
							ver.PackVersion = int.Parse(digits[0]);
							ver.Major = int.Parse(digits[1]);
							ver.Minor = int.Parse(digits[2]);
							ver.Revision = int.Parse(digits[3]);
						}
						else
						{
							if (File.Exists(CreatePCKFile.GodotVersionSave))
							{
								try
								{
									var reader = new BinaryReader(File.OpenRead(CreatePCKFile.GodotVersionSave));
									ver.PackVersion = reader.ReadByte();
									ver.Major = reader.ReadByte();
									ver.Minor = reader.ReadByte();
									ver.Revision = reader.ReadByte();
									reader.Close();
								}
								catch (Exception e)
								{
									Console.WriteLine($"Error! Can't read version from {CreatePCKFile.GodotVersionSave}\n{e.Message}\n" + ValidCommands);
									return;
								}
							}
							else
							{
								Console.WriteLine($"Can't find {CreatePCKFile.GodotVersionSave} please specify version in command line or one time use gui to create PCK file\n" + ValidCommands);
								return;
							}
						}

						pckPacker.PackFiles(filePath, files, 8, ver);
					}
					else
					{
						Console.WriteLine($"Specified directory does not exists! '{dirPath}'\n" + ValidCommands);
						return;
					}
				}
			}

			if (!run_with_args)
				// run..
				Application.Run(new Form1());
		}

		static void ScanFoldersForFiles(string folder, List<PCKPacker.FileToPack> files, ref string basePath)
		{
			foreach (string d in Directory.EnumerateDirectories(folder))
			{
				ScanFoldersForFiles(d, files, ref basePath);
			}

			foreach (string f in Directory.EnumerateFiles(folder))
			{
				var inf = new FileInfo(f);
				files.Add(new PCKPacker.FileToPack(f, f.Replace(basePath + "\\", "res://").Replace("\\", "/"), inf.Length));
			}
		}
	}
}