using GodotPCKExplorer.GlobalShared;
using System.Globalization;

namespace GodotPCKExplorer.Cmd
{
    internal class Program
    {
        internal static int? ExitCode;
        static readonly Logger logger;

        static int Main(string[] args)
        {
            PCKActions.Init((s) => Log(s));
            ConsoleCommands.RunCommand(args);
            Cleanup();

            if (ExitCode.HasValue)
            {
                return ExitCode.Value;
            }
            return 0;
        }

        static Program()
        {
            // InvariantCulture for console and UI
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (!Directory.Exists(GlobalConstants.AppDataPath))
                Directory.CreateDirectory(GlobalConstants.AppDataPath);

            logger = new Logger("log_c.txt");
        }

        public static void Cleanup()
        {
            logger.Dispose();
        }

        public static void Log(string txt)
        {
            logger.Write(txt);
        }

        public static void Log(Exception ex)
        {
            logger.Write(ex);
        }

        public static void LogHelp()
        {
            string helpText = @"Godot can embed '.pck' files into other files.
Therefore, GodotPCKExplorer can open both '.pck' and files with embedded 'pck'.

Paths and other arguments must be without spaces or inside quotes: ""some path""
The PACK_VERSION in the version can be 1 for Godot 3 or 2 for Godot 4.
Encryption only works with '.pck' for Godot 4.

{} - Optional arguments

Examples of valid commands:
";

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                helpText += @"-o	Open pack file using UI
	-o [path to pack] {[encryption key]}
	-o C:/Game.exe
	-o C:/Game.pck

";
            }

            helpText += @"-i	Show pack file info
	-i [path to pack]
	-i C:/Game.exe
	-i C:/Game.pck
	
-l	Show pack file info with a list of packed files
	-l [path to pack] {[encryption key]}
	-l C:/Game.exe
	-l C:/Game.pck

-e	Extract content from a pack to a folder. Automatically overwrites existing files
	Instead of the key, you can specify ""skip"" or ""encrypted"" to skip or extract files without decryption, respectively
	-e [path to pack] [path to output folder] {[encryption key]}
	-e C:/Game.exe ""C:/Path with Spaces"" 
	-e C:/Game.pck Output_dir 7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA
	-e C:/Game.pck Output_dir skip
	-e C:/Game.pck Output_dir encrypted

-es	Extract like -e but Skip existing files

-p	Pack content of folder into .pck file
	The version should be in this format: PACK_VERSION.GODOT_MINOR._MAJOR._PATCH
	-p [folder] [output pack file] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-p ""C:/Directory with files"" C:/Game_New.pck 1.3.2.0
	-p ""C:/Directory with files"" C:/Game_New.pck 2.4.0.1 ""some/prefix/dir/""
	-p ""C:/Directory with files"" C:/Game_New.pck 2.4.0.1 """" 7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA files

-pe	Pack Embedded. Equal to -p, but embed '.pck' into target file
	-pe [folder] [exe to pack into] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-pe ""C:/Directory with files"" C:/Game.exe 1.3.2.0 ""mod_folder/""

-pc	Copy the contents of '.pck' file into a new '.pck' file and patch it with the contents of the folder. Similar to -p
	Encryption and prefix are applied only to the contents of the folder.
	-pc [input pck] [folder] [output pack file] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-pc C:/Game.pck ""C:/Directory with modified files"" C:/Game_New.pck 2.4.3.0 ""some/prefix/dir/""

-pce	Equal to -pc, but embed '.pck' into target file
	-pce [input pck] [folder] [exe to pack into] [version] {[path prefix]} {[encryption key]} {[encryption: both|index|files]}
	-pce C:/Game.pck ""C:/Directory with files"" C:/Game.exe 2.4.3.0 ""mod_folder/""

-m	Merge pack into target file. So you can copy the '.pck' from one file to another
	-m [path to pack] [file to merge into]
	-m C:/Game.pck C:/Game.exe
	-m C:/GameEmbedded.exe C:/Game.exe

-r	Rip '.pck' from file
	If the output file is not specified, it will just be deleted from the original file
	Otherwise, it will be extracted without changing the original file
	-r [path to file] {[output pack file]}
	-r C:/Game.exe C:/Game.pck
	-r C:/Game.exe

-s	Split file with embedded '.pck' into two separated files
	-s [path to file] {[path to the new file (this name will also be used for '.pck')]}
	-s C:/Game.exe ""C:/Out Folder/NewGameSplitted.exe""
	-s C:/Game.exe

-c	Change version of the '.pck'
	-c [path to pck] [new version]
	-c C:/Game.pck 1.3.4.1
	-c C:/Game.exe 2.4.0.2
";

            Log("\n" + helpText);
        }
    }
}
