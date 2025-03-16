using System.Reflection;
using System.Collections;
using System.IO.Compression;
using GodotPCKExplorer;
using GodotPCKExplorer.Cmd;
using PCKBruteforcer;

namespace Tests
{
    [TestFixture, NonParallelizable, SingleThreaded]
    [TestFixtureSource(typeof(MyFixtureData), nameof(MyFixtureData.FixtureParams))]
    public class UtilMethodsTests
    {
        static string ExecutableExtension
        {
            get => OperatingSystem.IsWindows() ? ".exe" : "";
        }

        int GodotVersionMajor = 0;
        int GodotVersion = 0;

        string ZipFilePath
        {
            get => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", $"Test{GodotVersion}.zip");
        }

        static string PlatformFolder
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return "win";
                else if (OperatingSystem.IsLinux())
                    return "linux";
                else if (OperatingSystem.IsMacOS())
                    return "mac";
                return "";
            }
        }

        static readonly string DefaultGodotArgs = "--headless --quit";

        string test_folder_name;
        string binaries_base;
        string binaries;

        List<string> OriginalTestFiles = [];

        public UtilMethodsTests(int major, int version)
        {
            GodotVersionMajor = major;
            GodotVersion = version;
        }

        static void Title(string name)
        {
            Console.WriteLine($"===================={name}====================");
        }

        static string Exe(string name)
        {
            return name + ExecutableExtension;
        }

        static PCKVersion GetPCKVersion(string pack)
        {
            Console.WriteLine($"Getting version");
            string console_output = "";
            var ver = new PCKVersion();
            using (var output = new ConsoleOutputRedirect())
            {
                Assert.That(PCKActions.PrintInfo(pack), Is.True);
                console_output = TUtils.RemoveTimestampFromLogs(output.GetOuput());
                var lines = console_output.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
                foreach (var l in lines)
                {
                    if (l.StartsWith("Version string for this program: "))
                    {
                        var parts = l.Split(':');
                        ver = new PCKVersion(parts[^1]);
                    }
                }
            }

            Console.WriteLine(console_output);
            Console.WriteLine($"Got version: {ver}");
            return ver;
        }

        [SetUp]
        public void GodotPCKInit()
        {
            test_folder_name = $"{GodotVersion}_{TestContext.CurrentContext.Test.MethodName}";
            binaries_base = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "TestBinaries", test_folder_name);
            binaries = Path.Combine(binaries_base, PlatformFolder);

            if (!PCKActions.IsInited)
                PCKActions.Init(new ProgressReporterTests($"{TestContext.CurrentContext.Test.MethodName ?? "!NoTest!"}({GodotVersion})"));
            ConsoleCommands.InitLogs(t => Console.WriteLine($"[Console ({test_folder_name})] {t}"), ex => Console.WriteLine($"[Console ({test_folder_name}) ⚠] {ex.GetType().Name}:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}"));

            if (!Directory.Exists(binaries_base))
                Directory.CreateDirectory(binaries_base);

            ClearBinaries();

            var zip = ZipFile.OpenRead(ZipFilePath);
            foreach (var f in zip.Entries)
            {
                try
                {
                    var file = Path.Combine(binaries_base, f.FullName);
                    Directory.CreateDirectory(Path.GetDirectoryName(file) ?? "");

                    if (f.FullName.StartsWith(PlatformFolder))
                    {
                        f.ExtractToFile(file, false);
                    }
                }
                catch { }
            }
        }

        [TearDown]
        public void GodotPCKClear()
        {
            PCKActions.Cleanup();
            ClearBinaries();
        }

        private void ClearBinaries()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    foreach (var d in Directory.GetDirectories(binaries_base))
                        Directory.Delete(d, true);
                    foreach (var f in Directory.GetFiles(binaries_base, "*", SearchOption.AllDirectories))
                        if (!OriginalTestFiles.Contains(f))
                            File.Delete(f);
                }
                catch { }
            }
        }

        void CompareRealFilesAndPCK(string folder, string origPck, string newPck, bool compareMD5Raw, string? encKey = null, Func<string, string>? realFileNameChange = null)
        {
            string[] original_files = null!;
            PCKVersion ver;
            {
                using var pckReader = new PCKReader();
                pckReader.OpenFile(origPck, getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = encKey });
                ver = pckReader.PCK_Version;
                original_files = [.. pckReader.Files.Select(f => f.Value.FilePath).Order()];
            }

            // Compare all names
            Assert.That(PCKUtils.GetListOfFilesToPack(folder, ver).Select(f => f.Path).Order().SequenceEqual(original_files.Select(f => realFileNameChange == null ? f : realFileNameChange(f))), Is.True);

            if (GodotVersionMajor < 4)
                return;

            // Test MD5 for all files
            var new_md5s = PCKUtils.GetListOfFilesToPack(folder, ver).OrderBy(f => f.Path).Select(f => { f.CalculateMD5(); return PCKUtils.ByteArrayToHexString(f.MD5); }).ToArray();
            string[] orig_md5s;
            {
                PCKReaderFile[] ordered_files = null!;
                using var pckReader = new PCKReader();
                pckReader.OpenFile(newPck, getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = encKey });
                ordered_files = [.. pckReader.Files.Select(f => f.Value).OrderBy(f => f.FilePath)];

                if (compareMD5Raw)
                {
                    orig_md5s = ordered_files.Select(f =>
                    {
                        if (f.IsRemoval)
                        {
                            return PCKUtils.ByteArrayToHexString(new byte[16]);
                        }

                        return PCKUtils.ByteArrayToHexString(PCKUtils.GetStreamMD5(pckReader.ReaderStream!.BaseStream, f.Offset, f.Offset + f.ActualSize));
                    }).ToArray();
                }
                else
                {
                    orig_md5s = ordered_files.Select(f => PCKUtils.ByteArrayToHexString(f.MD5)).ToArray();
                }
            }
            Assert.That(new_md5s.SequenceEqual(orig_md5s), Is.True);
        }

        [Test]
        public void TestInfoCommand()
        {
            Title("Info PCK");
            Assert.That(PCKActions.PrintInfo(Path.Combine(binaries, "Test.pck")), Is.True);
            Title("Info EXE");
            Assert.That(PCKActions.PrintInfo(Path.Combine(binaries, Exe("TestEmbedded")), true), Is.True);
            Title("Wrong path");
            Assert.That(PCKActions.PrintInfo(Path.Combine(binaries, "WrongPath/Test.pck")), Is.False);
        }

        [Test]
        public void TestPatchingCommand()
        {
            TestPatchingCommandBody(false);
        }

        public void TestPatchingCommandBody(bool encrypted)
        {
            string extractTestPath = Path.Combine(binaries, "ExtractTest");
            string testEXE = Path.Combine(binaries, Exe("Test"));
            string testPCK = Path.Combine(binaries, "Test.pck");
            string extractCheckPatch = Path.Combine(binaries, "ExtractCheckPatch");
            string patchFolder = Path.Combine(binaries, "justForPatch");
            string checkPatchPck = Path.Combine(binaries, "TestCheckPatch.pck");

            var rnd = new Random();
            var ver = GetPCKVersion(testPCK);
            var verStr = ver.ToString();
            string encKey = encrypted ? "7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA" : "";
            string wrong_encKey = encrypted ? "8FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA" : "";

            Title("Extract");
            Assert.That(PCKActions.Extract(testPCK, extractTestPath, true), Is.True);
            Assert.That(PCKActions.Extract(testPCK, extractCheckPatch, true), Is.True);

            Title("Check Patched Prefixed Files");
            {
                var files = PCKUtils.GetListOfFilesToPack(extractCheckPatch, ver);
                var rndFile = files[rnd.Next(files.Count)];
                var filePath = rndFile.OriginalPath;

                using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    var bytes = new byte[256];
                    rnd.NextBytes(bytes);
                    file.Write(bytes);
                }
                Directory.CreateDirectory(patchFolder);
                TUtils.CopyFile(filePath, Path.Combine(patchFolder, Path.GetFileName(filePath)));
                var prefix = (Path.GetDirectoryName(filePath) ?? "").Replace(extractCheckPatch, "").TrimStart(Path.DirectorySeparatorChar);
                if (!string.IsNullOrWhiteSpace(prefix))
                    prefix += Path.DirectorySeparatorChar;

                // pack with prefix from another folder
                Assert.That(PCKActions.Pack(patchFolder, checkPatchPck, verStr, pckToPatch: testPCK, packPathPrefix: prefix, encKey: encKey, encFiles: encrypted), Is.True);
                CompareRealFilesAndPCK(extractCheckPatch, testPCK, checkPatchPck, false, encKey: encKey);

                // Same PCK
                Assert.That(PCKActions.Pack(patchFolder, testPCK, verStr, pckToPatch: testPCK), Is.False);

                if (encrypted)
                {
                    using var pck = new PCKReader();
                    Assert.That(pck.OpenFile(checkPatchPck), Is.True);
                    Assert.That(pck.Files.Count(f => f.Value.IsEncrypted), Is.EqualTo(1));
                }

                // test MD5 of not touched PCK files
                string[] getMd5s(string md5PCK)
                {
                    PCKReaderFile[] ordered_files = null!;
                    using var pckReader = new PCKReader();
                    pckReader.OpenFile(md5PCK, getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = encKey });
                    ordered_files = [.. pckReader.Files.Where(f => f.Value.FilePath != rndFile.Path).Select(f => f.Value).OrderBy(f => f.FilePath)];

                    return ordered_files.Select(f =>
                    {
                        return PCKUtils.ByteArrayToHexString(PCKUtils.GetStreamMD5(pckReader.ReaderStream!.BaseStream, f.Offset, f.Offset + f.ActualSize));
                    }).ToArray();
                }

                string[] orig_md5s = getMd5s(testPCK);
                string[] new_md5s = getMd5s(checkPatchPck);
                Assert.That(new_md5s.SequenceEqual(orig_md5s), Is.True);
            }
        }

        [Test]
        public void TestExtractScanPackCommand()
        {
            string extractTestPath = Path.Combine(binaries, "ExtractTest");
            string testEXE = Path.Combine(binaries, Exe("Test"));
            string testPCK = Path.Combine(binaries, "Test.pck");
            string testEmbedPack = Path.Combine(binaries, Exe("TestPack"));
            string selectedFilesPck = Path.Combine(binaries, "SelecetedFiles.pck");
            string extractTestSelectedPath = Path.Combine(binaries, "ExtractTestSelected");
            string newPckPath = Path.Combine(binaries, "out.pck");
            string extractTestSelectedWrongPath = Path.Combine(binaries, "ExtractTestSelectedWrong");
            string overwritePath = extractTestPath + "Overwrite";
            string out_exe = Path.ChangeExtension(newPckPath, ExecutableExtension);
            string extractTestPrefix = Path.Combine(binaries, "ExtractTestPrefix");
            string testPrefixPCK = Path.Combine(binaries, "TestPrefix.pck");

            var ver = GetPCKVersion(testPCK);
            var verStr = ver.ToString();

            Title("Extract");
            Assert.That(PCKActions.Extract(testPCK, extractTestPath, true), Is.True);

            Title("Extract Wrong Path");
            Assert.That(PCKActions.Extract(Path.Combine(binaries, "WrongPath/Test.pck"), extractTestPath, true), Is.False);

            Title("Compare content with folder");
            var list_of_files = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestPath), ver);
            {
                using var pck = new PCKReader();

                Assert.That(pck.OpenFile(testPCK), Is.True);
                Assert.That(list_of_files, Has.Count.EqualTo(pck.Files.Count));

                foreach (var f in list_of_files)
                    Assert.That(pck.Files.ContainsKey(f.Path), Is.True);
            }

            // select at least one file
            var rnd = new Random();
            var first = false;
            var seleceted_files = list_of_files.Where(f =>
            {
                if (!first)
                {
                    first = true;
                    return true;
                }
                return rnd.NextDouble() > 0.5;
            }).ToList();

            Title("Extract only seleceted files and compare");
            var export_files = seleceted_files.Select((s) => s.Path);
            {
                Assert.That(PCKActions.Extract(testPCK, extractTestSelectedPath, true, export_files), Is.True);

                var exportedSelectedList = PCKUtils.GetListOfFilesToPack(extractTestSelectedPath, ver);
                Assert.That(seleceted_files, Has.Count.EqualTo(exportedSelectedList.Count));

                foreach (var f in export_files)
                    Assert.That(exportedSelectedList.FindIndex((l) => l.Path == f), Is.Not.EqualTo(-1));
            }

            Title("Extract only seleceted wrong files and compare");
            {
                var wrong_selected = export_files.ToList();
                for (int i = 0; i < wrong_selected.Count; i++)
                    wrong_selected[i] = wrong_selected[i] + "WrongFile";

                Assert.That(PCKActions.Extract(testPCK, extractTestSelectedWrongPath, true, wrong_selected), Is.True);
                Assert.That(Directory.Exists(extractTestSelectedWrongPath), Is.False);
            }

            Title("Extract empty list");
            {
                Assert.That(PCKActions.Extract(testPCK, extractTestSelectedWrongPath, true, Array.Empty<string>()), Is.False);
            }

            Title("Extract without overwrite");
            {
                Assert.That(PCKActions.Extract(testPCK, overwritePath, true), Is.True);
                var files = Directory.GetFiles(overwritePath);
                File.Delete(files[0]);
                File.WriteAllText(files[0], "Test");
                File.Delete(files[1]);

                Assert.That(PCKActions.Extract(testPCK, overwritePath, false), Is.True);

                Assert.That(File.ReadAllText(files[0]), Is.EqualTo("Test"));
                Assert.That(File.Exists(files[1]), Is.True);
            }

            Title("Pack new PCK");
            {
                Assert.That(PCKActions.Pack(extractTestPath, newPckPath, verStr), Is.True);

                if (OperatingSystem.IsWindows())
                {
                    Title("Locked file");
                    string locked_file = Path.Combine(extractTestPath, "out.lock");
                    using var f = new LockedFile(locked_file);
                    Assert.That(PCKActions.Pack(extractTestPath, locked_file, verStr), Is.False);
                }
            }

            Title("Pack and Extract PCK with prefix");
            {
                Assert.That(PCKActions.Pack(extractTestPath, testPrefixPCK, verStr, packPathPrefix: "test_prefix_like_mod_folder/"), Is.True);
                Assert.That(PCKActions.Extract(testPrefixPCK, extractTestPrefix), Is.True);

                var list_of_files_with_prefix = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestPrefix), ver);
                {
                    using var pck = new PCKReader();

                    Assert.That(pck.OpenFile(testPrefixPCK), Is.True);
                    Assert.That(list_of_files_with_prefix, Has.Count.EqualTo(pck.Files.Count));

                    foreach (var f in list_of_files_with_prefix)
                        Assert.That(pck.Files.ContainsKey(f.Path), Is.True);
                }
            }

            Title("Wrong version and directory");
            {
                Assert.That(PCKActions.Pack(extractTestPath, newPckPath, "1234"), Is.False);
                Assert.That(PCKActions.Pack(extractTestPath, newPckPath, "123.33.2.1"), Is.False);
                Assert.That(PCKActions.Pack(extractTestPath, newPckPath, "-1.0.2.1"), Is.False);
                Assert.That(PCKActions.Pack(extractTestPath + "WrongPath", newPckPath, verStr), Is.False);
            }

            // Compare new PCK content with alredy existing trusted list of files 'list_of_files'
            Title("Compare files to original list");
            {
                using var pck = new PCKReader();
                Assert.That(pck.OpenFile(newPckPath), Is.True);
                foreach (var f in pck.Files.Keys)
                    Assert.That(list_of_files.FindIndex((l) => l.Path == f), Is.Not.EqualTo(-1));
            }

            Title("Pack embedded");
            {
                TUtils.CopyFile(testEXE, testEmbedPack);
                Assert.That(PCKActions.Pack(extractTestPath, testEmbedPack, verStr, embed: true), Is.True);
                Assert.That(File.Exists(Path.ChangeExtension(testEmbedPack, Exe(".old"))), Is.True);
            }

            Title("Pack embedded again");
            {
                Assert.That(PCKActions.Pack(extractTestPath, testEmbedPack, verStr, embed: true), Is.False);
                Assert.That(File.Exists(Path.ChangeExtension(testEmbedPack, Exe(".old"))), Is.False);
            }

            Title("Pack only selected files");

            Assert.That(PCKActions.Pack(seleceted_files, selectedFilesPck, verStr), Is.True);

            Title("Compare selected to pack content with new pck");
            {
                using var pck = new PCKReader();
                Assert.That(pck.OpenFile(selectedFilesPck), Is.True);
                Assert.That(seleceted_files, Has.Count.EqualTo(pck.Files.Count));

                foreach (var f in pck.Files.Keys)
                    Assert.That(seleceted_files.FindIndex((l) => l.Path == f), Is.Not.EqualTo(-1));
            }

            Title("Pack and Extract PCK with User dir");
            {
                string extractTestPathUserOrig = Path.Combine(binaries, "ExtractTestUser");
                string extractTestUserPath = Path.Combine(binaries, "ExtractUser");
                string extractTestUserPck = Path.Combine(binaries, "TestUser.pck");

                Assert.That(PCKActions.Extract(testPCK, extractTestPathUserOrig, true), Is.True);

                var user_test_file = "test.empty";
                var user_test_file_dir_path = Path.Combine(extractTestPathUserOrig, PCKUtils.PathExtractPrefixUser, user_test_file).Replace("\\", "/");

                var user_test_wrong_file = "test2.empty";
                var user_test_wrong_file_local_path = Path.Combine("somefolder", PCKUtils.PathExtractPrefixUser, user_test_wrong_file).Replace("\\", "/");
                var user_test_wrong_file_dir_path = Path.Combine(extractTestPathUserOrig, user_test_wrong_file_local_path).Replace("\\", "/");
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(user_test_file_dir_path)!);
                    Directory.CreateDirectory(Path.GetDirectoryName(user_test_wrong_file_dir_path)!);
                    using var f = File.Create(user_test_file_dir_path);
                    using var f2 = File.Create(user_test_wrong_file_dir_path);
                }


                Assert.That(PCKActions.Pack(extractTestPathUserOrig, extractTestUserPck, verStr), Is.True);
                Assert.That(PCKActions.Extract(extractTestUserPck, extractTestUserPath), Is.True);

                var list_of_files_user = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestUserPath), ver);
                {
                    using var pck = new PCKReader();

                    Assert.That(pck.OpenFile(extractTestUserPck), Is.True);
                    Assert.That(list_of_files_user, Has.Count.EqualTo(pck.Files.Count)); // same count

                    Assert.That(list_of_files_user.Find(i => i.Path == PCKUtils.PathPrefixUser + user_test_file) != null, Is.True);
                    Assert.That(pck.Files.ContainsKey(PCKUtils.PathPrefixUser + user_test_file), Is.True);

                    // check for no removal files
                    Assert.That(pck.IsRemovalFiles, Is.False);

                    // only @@user@@ in root allowed
                    Assert.That(list_of_files_user.Find(i => i.Path == PCKUtils.PathPrefixUser + user_test_wrong_file) != null, Is.False);
                    Assert.That(pck.Files.ContainsKey(PCKUtils.PathPrefixUser + user_test_wrong_file), Is.False);

                    // check for wrong files in res://somefolder/@@user@@
                    var user_wrong_res_path = PCKUtils.GetResFilePathInPCK(user_test_wrong_file_local_path, ver);
                    Assert.That(list_of_files_user.Find(i => i.Path == user_wrong_res_path) != null, Is.True);
                    Assert.That(pck.Files.ContainsKey(user_wrong_res_path), Is.True);

                    foreach (var f in list_of_files_user)
                        Assert.That(pck.Files.ContainsKey(f.Path), Is.True);
                }
            }

            if (GodotVersionMajor >= 4 && ver.Minor >= 4)
            {
                Title("Removal files");

                string extractTestPathRemovalOrig = Path.Combine(binaries, "ExtractTestRemoval");
                string extractTestRemovalPath = Path.Combine(binaries, "ExtractRemoval");
                string extractTestRemovalPck = Path.Combine(binaries, "TestRemoval.pck");
                string extractTestRemovalPck2 = Path.Combine(binaries, "TestRemoval2.pck");

                Assert.That(PCKActions.Extract(testPCK, extractTestPathRemovalOrig, true), Is.True);

                var removal_test_file = "test.empty" + PCKUtils.PathExtractTagRemoval;
                var removal_test_file_dir_path = Path.Combine(extractTestPathRemovalOrig, removal_test_file).Replace("\\", "/");

                var removal_test_wrong_file = "test2.empty";
                var removal_test_wrong_file_local_path = Path.Combine("somefolder" + PCKUtils.PathExtractTagRemoval, removal_test_wrong_file).Replace("\\", "/");
                var removal_test_wrong_file_dir_path = Path.Combine(extractTestPathRemovalOrig, removal_test_wrong_file_local_path).Replace("\\", "/");
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(removal_test_file_dir_path)!);
                    Directory.CreateDirectory(Path.GetDirectoryName(removal_test_wrong_file_dir_path)!);
                    using var f = File.Create(removal_test_file_dir_path);
                    using var f2 = File.Create(removal_test_wrong_file_dir_path);
                }

                var removal_test_file_object = new PCKPackerRegularFile(removal_test_file_dir_path, extractTestPathRemovalOrig);
                removal_test_file_object.UpdateFileInfo(ver); // It is also testing the conversion of \\ to /
                removal_test_file_object.CalculateMD5(); // should be 16 zeros.

                Assert.That(PCKActions.Pack(extractTestPathRemovalOrig, extractTestRemovalPck, verStr), Is.True);
                Assert.That(PCKActions.Extract(extractTestRemovalPck, extractTestRemovalPath), Is.True);
                Assert.That(PCKActions.Pack(extractTestRemovalPath, extractTestRemovalPck2, verStr), Is.True);

                var list_of_files_removal = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestRemovalPath), ver);
                {
                    using var pck = new PCKReader();

                    Assert.That(pck.OpenFile(extractTestRemovalPck), Is.True);
                    Assert.That(list_of_files_removal, Has.Count.EqualTo(pck.Files.Count)); // same count

                    var res_file_path = PCKUtils.GetResFilePathInPCK(removal_test_file_object.Path, ver);
                    var res_wrong_file_path = PCKUtils.GetResFilePathInPCK(removal_test_wrong_file_local_path, ver);

                    Assert.That(list_of_files_removal.Find(i => i.Path == res_file_path) != null, Is.True);
                    Assert.That(list_of_files_removal.Find(i => i.Path == res_wrong_file_path) != null, Is.True);
                    Assert.That(pck.Files.ContainsKey(res_file_path), Is.True);
                    Assert.That(pck.Files.ContainsKey(res_wrong_file_path), Is.True); // regular file with .@@removal@@ tag

                    Assert.That(pck.IsRemovalFiles, Is.True);
                    Assert.That(pck.Files.Count(f => f.Value.IsRemoval), Is.EqualTo(1));

                    Assert.That(list_of_files_removal.Find(i => i.Path == res_file_path)!.IsRemoval, Is.True);
                    Assert.That(list_of_files_removal.Find(i => i.Path == res_wrong_file_path)!.IsRemoval, Is.False);
                    Assert.That(pck.Files[res_file_path].IsRemoval, Is.True);
                    Assert.That(pck.Files[res_wrong_file_path].IsRemoval, Is.False);

                    Assert.That(pck.Files[res_file_path].MD5.SequenceEqual(new byte[16]), Is.True);
                    Assert.That(pck.Files[res_file_path].MD5.SequenceEqual(removal_test_file_object.MD5!), Is.True);

                    CompareRealFilesAndPCK(extractTestPathRemovalOrig, extractTestRemovalPck, extractTestRemovalPck2, true);
                }
            }

            Title("Good run");

            TUtils.CopyFile(testEXE, out_exe);

            using (var r = new RunGodotWithOutput(out_exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.True);

            // test embed pack
            using (var r = new RunGodotWithOutput(testEmbedPack, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.True);

            Title("Run without PCK");
            if (File.Exists(newPckPath))
                File.Delete(newPckPath);

            using (var r = new RunGodotWithOutput(out_exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.False);
        }

        [Test]
        public void TestMergePCK()
        {
            string testEXE = Path.Combine(binaries, Exe("Test"));
            string testPCK = Path.Combine(binaries, "Test.pck");
            string newEXE = Path.Combine(binaries, Exe("TestMerge"));
            string newEXE1Byte = Path.Combine(binaries, Exe("TestMerge1Byte"));
            string newEXE_old = Path.Combine(binaries, Exe("TestMerge.old"));

            TUtils.CopyFile(testEXE, newEXE);

            Title("Merge");
            {
                Assert.That(PCKActions.Merge(testPCK, newEXE), Is.True);
                Assert.That(File.Exists(newEXE_old), Is.True);
            }

            Title("Again");
            {
                Assert.That(PCKActions.Merge(testPCK, newEXE), Is.False);

                File.Delete(newEXE);
                TUtils.CopyFile(testEXE, newEXE);
            }

            Title("Merge without backup");
            {
                Assert.That(PCKActions.Merge(testPCK, newEXE, true), Is.True);
                Assert.That(File.Exists(newEXE_old), Is.False);

                File.Delete(newEXE);
                TUtils.CopyFile(testEXE, newEXE);
            }

            if (OperatingSystem.IsWindows())
            {
                Title("Locked backup");
                // creates new (old + ExecutableExtension) 0kb
                using (var l = new LockedFile(newEXE_old, false))
                    Assert.That(PCKActions.Merge(testPCK, newEXE, true), Is.False);

                Title("Locked pck file");
                using (var l = new LockedFile(testPCK, false))
                    Assert.That(PCKActions.Merge(testPCK, newEXE), Is.False);
            }

            Title("Wrong Files");
            {
                Assert.That(PCKActions.Merge(testPCK + "Wrong", newEXE), Is.False);
                Assert.That(PCKActions.Merge(testPCK, newEXE + "Wrong", true), Is.False);
            }

            Title("Same File");
            Assert.That(PCKActions.Merge(testPCK, testPCK, true), Is.False);

            Title("-1 Byte");
            var nf = new BinaryWriter(File.OpenWrite(newEXE1Byte));
            var o = new BinaryReader(File.OpenRead(testEXE));
            nf.Write(o.ReadBytes((int)o.BaseStream.Length - 1), 0, (int)o.BaseStream.Length - 1);
            nf.Close();
            o.Close();

            // The result is good... but thats not 64bit multiple :/
            Assert.That(PCKActions.Merge(testPCK, newEXE1Byte, true), Is.True);

            Title("Bad run");
            using (var r = new RunGodotWithOutput(newEXE, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.False);

            Title("Good runs");
            File.Delete(newEXE);
            TUtils.CopyFile(testEXE, newEXE);
            Assert.That(PCKActions.Merge(testPCK, newEXE), Is.True);
            using (var r = new RunGodotWithOutput(newEXE, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.True);

            using (var r = new RunGodotWithOutput(newEXE1Byte, DefaultGodotArgs))
                if (GodotVersionMajor == 3)
                    Assert.That(r.IsSuccess(), Is.True);
                else if (GodotVersionMajor >= 4)
                    Assert.That(r.IsSuccess(), Is.False);
        }

        [Test]
        public void TestRipPCK()
        {
            string new_exe = Path.Combine(binaries, Exe("TestRip"));
            string new_exe_remove = Path.Combine(binaries, Exe("TestRipRemove"));
            string new_exe_old = Path.Combine(binaries, Exe("TestRip.old"));
            string new_exe_remove_old = Path.Combine(binaries, Exe("TestRipRemove.old"));
            string new_pck = Path.Combine(binaries, "TestRip.pck");
            string locked_exe_str = Path.Combine(binaries, Exe("TestLockedRip"));

            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), new_exe);
            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), new_exe_remove);

            Title("Rip embedded");
            Assert.That(PCKActions.Rip(new_exe, new_pck), Is.True);

            Title("Rip wrong files");
            {
                Assert.That(PCKActions.Rip(Path.Combine(binaries, Exe("Test")), new_pck), Is.False);
                Assert.That(PCKActions.Rip(new_pck, new_pck), Is.False);

                if (OperatingSystem.IsWindows())
                {
                    Title("Locked file");
                    string locked_file = Path.Combine(binaries, "test.lock");
                    using var f = new LockedFile(locked_file);
                    Assert.That(PCKActions.Rip(new_exe, locked_file), Is.False);
                }
            }

            Title("Rip PCK from exe");
            {
                Assert.That(PCKActions.Rip(new_exe, null, true), Is.True);
                Assert.That(PCKActions.Remove(new_exe_remove, true), Is.True);
                Assert.That(File.Exists(new_exe_old), Is.False);
                Assert.That(File.Exists(new_exe_remove_old), Is.False);
            }

            Title("Rip PCK from PCK");
            Assert.That(PCKActions.Rip(new_pck, null, true), Is.False);
            Assert.That(PCKActions.Remove(new_exe_remove, true), Is.False);

            Title("Good run");
            using (var r = new RunGodotWithOutput(new_exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.True);

            Title("Run without PCK");
            if (File.Exists(new_pck))
                File.Delete(new_pck);
            using (var r = new RunGodotWithOutput(new_exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.False);

            Title("Rip locked");

            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), locked_exe_str);

            using (var locked_exe = File.OpenWrite(locked_exe_str))
                Assert.That(PCKActions.Rip(locked_exe_str), Is.False);

            Title("Rip and remove .old");
            {
                Assert.That(PCKActions.Rip(locked_exe_str, null, true), Is.True);
                Assert.That(File.Exists(Path.ChangeExtension(locked_exe_str, Exe(".old"))), Is.False);
            }
        }

        [Test]
        public void TestSplitPCK()
        {
            string exe = Path.Combine(binaries, Exe("TestSplit"));
            string pck = Path.Combine(binaries, "TestSplit.pck");
            string new_exe = Path.Combine(binaries, "SplitFolder", Exe("Split"));
            string new_pck = Path.Combine(binaries, "SplitFolder", "Split.pck");

            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), exe);

            Title("Split with custom pair name and check files");
            {
                Assert.That(PCKActions.Split(exe, new_exe), Is.True);
                Assert.That(File.Exists(new_exe), Is.True);
                Assert.That(File.Exists(new_pck), Is.True);
                Assert.That(File.Exists(Path.ChangeExtension(new_exe, Exe(".old"))), Is.False);
            }

            Title("Can't copy with same name");
            Assert.That(PCKActions.Split(exe, exe), Is.False);

            Title("Split with same name");
            {
                Assert.That(PCKActions.Split(exe), Is.True);
                Assert.That(File.Exists(exe), Is.True);
                Assert.That(File.Exists(pck), Is.True);
                Assert.That(File.Exists(Path.ChangeExtension(new_exe, Exe(".old"))), Is.False);
            }

            Title("Already splitted");
            Assert.That(PCKActions.Split(exe), Is.False);

            Title("Good runs");
            using (var r = new RunGodotWithOutput(exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.True);

            using (var r = new RunGodotWithOutput(new_exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.True);

            Title("Bad runs");
            foreach (var f in new string[] { pck, new_pck })
                if (File.Exists(f))
                    File.Delete(f);

            using (var r = new RunGodotWithOutput(exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.False);

            using (var r = new RunGodotWithOutput(new_exe, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.False);

            if (OperatingSystem.IsWindows())
            {
                Title("Split with locked output");
                foreach (var f in new string[] { new_exe, new_pck })
                    if (File.Exists(f))
                        File.Delete(f);
                TUtils.CopyFile(Path.Combine(binaries, "Test.pck"), new_pck);

                using (var l = new LockedFile(new_pck, false))
                    Assert.That(PCKActions.Split(Path.Combine(binaries, Exe("TestEmbedded")), new_exe), Is.False);

                {
                    Assert.That(File.Exists(new_exe), Is.False);
                    Assert.That(File.Exists(new_pck), Is.True);
                }
            }
        }

        [Test]
        public void TestChangePCKVersion()
        {
            string exe = Path.Combine(binaries, Exe("TestVersion"));
            string pck = Path.Combine(binaries, "TestVersion.pck");
            string exeEmbedded = Path.Combine(binaries, Exe("TestVersionEmbedded"));

            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe);
            TUtils.CopyFile(Path.Combine(binaries, "Test.pck"), pck);
            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), exeEmbedded);

            var origVersion = GetPCKVersion(pck);
            var newVersion = origVersion;
            newVersion.Major += 1;
            newVersion.Minor += 1;
            newVersion.Revision += 2;

            Title("Regular pck test runs");
            {
                Assert.That(PCKActions.ChangeVersion(pck, newVersion.ToString()), Is.True);
                Assert.That(GetPCKVersion(pck), Is.EqualTo(newVersion));
                using (var r = new RunGodotWithOutput(exe, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.False);

                Assert.That(PCKActions.ChangeVersion(pck, origVersion.ToString()), Is.True);
                Assert.That(GetPCKVersion(pck), Is.EqualTo(origVersion));

                using (var r = new RunGodotWithOutput(exe, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);
            }

            Title("Embedded test runs");
            {
                Assert.That(PCKActions.ChangeVersion(exeEmbedded, newVersion.ToString()), Is.True);
                Assert.That(GetPCKVersion(exeEmbedded), Is.EqualTo(newVersion));

                using (var r = new RunGodotWithOutput(exeEmbedded, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.False);

                Assert.That(PCKActions.ChangeVersion(exeEmbedded, origVersion.ToString()), Is.True);
                Assert.That(GetPCKVersion(exeEmbedded), Is.EqualTo(origVersion));

                using (var r = new RunGodotWithOutput(exeEmbedded, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);
            }
        }

        [Test]
        public void TestEncryption()
        {
            if (GodotVersionMajor == 3)
                Assert.Inconclusive("Not applicable!");

            string enc_key = "7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA";
            string wrong_enc_key = "8FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA";
            string ver = $"{(GodotVersionMajor == 4 ? 2 : 1)}.{GodotVersionMajor}.0.2";

            string extracted = Path.Combine(binaries, "EncryptedExport");
            string exe = Path.Combine(binaries, Exe("TestEncrypted"));
            string pck = Path.Combine(binaries, "TestEncrypted.pck");
            string exe_new = Path.Combine(binaries, Exe("TestEncryptedNew"));
            string pck_new = Path.Combine(binaries, "TestEncryptedNew.pck");
            string pck_new_files = Path.Combine(binaries, "TestEncryptedNewOnlyFiles.pck");
            string pck_new_wrong_key = Path.Combine(binaries, "TestEncryptedWrongKey.pck");
            string exe_new_wrong_key = Path.Combine(binaries, Exe("TestEncryptedWrongKey"));
            string exe_new_files = Path.Combine(binaries, Exe("TestEncryptedNewOnlyFiles"));
            string exe_ripped = Path.Combine(binaries, Exe("TestEncryptedRipped"));
            string pck_ripped = Path.Combine(binaries, "TestEncryptedRipped.pck");
            string exe_embedded = Path.Combine(binaries, Exe("TestEncryptedEmbedded"));

            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe);
            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe_embedded);
            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe_new);
            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe_new_files);
            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe_ripped);
            TUtils.CopyFile(Path.Combine(binaries, Exe("Test")), exe_new_wrong_key);

            string[] original_files = null!;
            {
                using var pckReader = new PCKReader();
                pckReader.OpenFile(pck, getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = enc_key });
                original_files = [.. pckReader.Files.Select(f => f.Value.FilePath).Order()];
            }

            Title("PCK info");
            Assert.That(PCKActions.PrintInfo(pck, true, enc_key), Is.True);
            Title("PCK info wrong");
            Assert.That(PCKActions.PrintInfo(pck, true, wrong_enc_key), Is.False);

            Title("Merge PCK");
            Assert.That(PCKActions.Merge(pck, exe_embedded), Is.True);

            Title("Rip PCK");
            Assert.That(PCKActions.Rip(exe_embedded, pck_ripped), Is.True);

            Title("Extract PCK");
            Assert.That(PCKActions.Extract(pck, extracted, true, encKey: enc_key), Is.True);
            Title("Extract PCK wrong");
            Assert.That(PCKActions.Extract(pck, extracted + "Wrong", true, encKey: wrong_enc_key), Is.False);

            Title("Pack PCK");
            Assert.That(PCKActions.Pack(extracted, pck_new, ver, encIndex: true, encFiles: true, encKey: enc_key), Is.True);
            Title("Pack PCK only files");
            Assert.That(PCKActions.Pack(extracted, pck_new_files, ver, encIndex: false, encFiles: true, encKey: enc_key), Is.True);
            Title("Pack PCK wrong");
            Assert.That(PCKActions.Pack(extracted, pck_new_wrong_key, ver, encIndex: true, encFiles: true, encKey: wrong_enc_key), Is.True);
            Title("Pack PCK no key");
            Assert.That(PCKActions.Pack(extracted, pck_new_wrong_key, ver, encIndex: true, encFiles: true), Is.False);

            Title("Extract PCK. Encrypted only files");
            Assert.That(PCKActions.Extract(pck_new_files, extracted, true, encKey: enc_key), Is.True);
            Title("Extract PCK. Encrypted only files. Wrong key");
            Assert.That(PCKActions.Extract(pck_new_files, extracted, true), Is.False);
            Title("Extract PCK no Key skip");
            Assert.That(PCKActions.Extract(pck_new_files, extracted + "Skip", true, encKey: "", noKeyMode: PCKExtractNoEncryptionKeyMode.Skip), Is.True);
            Assert.That(Directory.Exists(extracted + "Skip"), Is.False); // If the files are not extracted, the folder is not created
            Title("Extract PCK no Key encrypted");
            Assert.That(PCKActions.Extract(pck_new_files, extracted + "Encrypted", true, encKey: "", noKeyMode: PCKExtractNoEncryptionKeyMode.AsIs), Is.True);
            CompareRealFilesAndPCK(extracted + "Encrypted", pck, pck_new_files, true, enc_key, f => f + ".encrypted");

            Title("Patching");
            TestPatchingCommandBody(true);

            Title("PCK good test runs");

            {
                using (var r = new RunGodotWithOutput(exe, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);

                using (var r = new RunGodotWithOutput(exe_embedded, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);

                using (var r = new RunGodotWithOutput(exe_ripped, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);

                using (var r = new RunGodotWithOutput(exe_new, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);

                using (var r = new RunGodotWithOutput(exe_new_files, DefaultGodotArgs))
                    Assert.That(r.IsSuccess(), Is.True);
            }

            Title("PCK bad test runs");

            using (var r = new RunGodotWithOutput(exe_new_wrong_key, DefaultGodotArgs))
                Assert.That(r.IsSuccess(), Is.False);
        }

        [Test]
        public void TestConsoleCommands()
        {
            //if (GodotVersionMajor == 3)
            //    Assert.Inconclusive("Not applicable!");

            string enc_key = "7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA";
            string wrong_enc_key = "8FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA";

            string extractTestPath = Path.Combine(binaries, "ExtractTest");
            string testEXE = Path.Combine(binaries, Exe("Test"));
            string testEmbedEXE = Path.Combine(binaries, Exe("TestPack"));
            string testPCK = Path.Combine(binaries, "Test.pck");
            string encPCK = Path.Combine(binaries, "TestEncrypted.pck");
            string encOnlyFilesPCK = GodotVersionMajor > 3 ? Path.Combine(binaries, "TestEncryptedFiles.pck") : "";

            string testPCKVersion = Path.Combine(binaries, "TestVer.pck");

            string testEXERip = Path.Combine(binaries, Exe("TestRip"));
            string testPCKRip = Path.Combine(binaries, "TestRip.pck");
            string testEXERipRemove = Path.Combine(binaries, Exe("TestRipRemove"));

            string testEXESplit = Path.Combine(binaries, Exe("TestSplitPair"));

            string testExtractFolder = Path.Combine(binaries, "TestExtract");

            string testPCKPack = Path.Combine(binaries, "TestPCKPack.pck");
            string testPCKPackEnc = Path.Combine(binaries, "TestPCKPackEnc.pck");

            var ver = GetPCKVersion(testPCK);
            var verStr = ver.ToString();

            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), testEmbedEXE);

            {
                string onlyFilesFolder = Path.Combine(binaries, "TestExtractOnlyFiles");
                Assert.That(PCKActions.Extract(testPCK, onlyFilesFolder, true), Is.True);
                if (GodotVersionMajor > 3)
                    Assert.That(PCKActions.Pack(onlyFilesFolder, encOnlyFilesPCK, verStr, encIndex: false, encFiles: true, encKey: enc_key), Is.True);
            }

            bool RunCommand(params string[] args)
            {
                return ConsoleCommands.RunCommand(args) && ConsoleCommands.ExitCode == 0;
            }

            Title("-i");
            {
                Assert.That(RunCommand("-i", testPCK), Is.True);
                Assert.That(RunCommand("-i"), Is.False);
                Assert.That(RunCommand("-i", "some/fake/path.pck"), Is.False);
                Assert.That(RunCommand("-i", testPCK, "extra arg"), Is.False);
            }

            Title("-l");
            {
                Assert.That(RunCommand("-l", testPCK), Is.True);
                Assert.That(RunCommand("-l"), Is.False);
                Assert.That(RunCommand("-l", "some/fake/path.pck"), Is.False);
                Assert.That(RunCommand("-l", testPCK, "extra arg"), Is.False);

                if (GodotVersionMajor > 3)
                {
                    Assert.That(RunCommand("-l", encPCK, enc_key), Is.True);
                    Assert.That(RunCommand("-l", encPCK, wrong_enc_key), Is.False);
                    Assert.That(RunCommand("-l", encPCK), Is.False);
                    Assert.That(RunCommand("-l", encPCK, enc_key, "extra arg"), Is.False);
                }
            }

            Title("-c");
            {
                TUtils.CopyFile(testPCK, testPCKVersion);
                var newVer = GetPCKVersion(testPCKVersion);
                newVer.Revision += 1;
                Assert.That(RunCommand("-c", testPCKVersion, newVer.ToString()), Is.True);
                Assert.That(GetPCKVersion(testPCKVersion), Is.EqualTo(newVer));
                Assert.That(RunCommand("-c", testPCKVersion, "-1.0.0,1"), Is.False);
                Assert.That(RunCommand("-c", testPCKVersion, "1.0.-.1"), Is.False);
                Assert.That(RunCommand("-c", "some/fake/path.pck", "1.0.-.1"), Is.False);
                Assert.That(RunCommand("-c", "some/fake/path.pck", newVer.ToString(), "extra arg"), Is.False);
            }

            Title("-r");
            {
                TUtils.CopyFile(testEmbedEXE, testEXERip);
                TUtils.CopyFile(testEmbedEXE, testEXERipRemove);

                Assert.That(RunCommand("-r", testEXERip, testPCKRip), Is.True);
                Assert.That(RunCommand("-r", testEXERipRemove), Is.True);

                {
                    using var pckReader = new PCKReader();
                    Assert.That(pckReader.OpenFile(testEXERip), Is.True);
                    Assert.That(pckReader.OpenFile(testPCKRip), Is.True);

                    Assert.That(pckReader.OpenFile(testEXERipRemove), Is.False);
                }

                Assert.That(RunCommand("-r", testEXERipRemove), Is.False);
                Assert.That(RunCommand("-r", testEXERipRemove + "testWrongPath"), Is.False);

                Assert.That(RunCommand("-r", testEXERip, testPCKRip, "extra arg"), Is.False);
            }

            Title("-m");
            {
                Assert.That(RunCommand("-m", testPCKRip, testEXERipRemove, "extra arg"), Is.False);

                Assert.That(RunCommand("-m", testPCKRip, testEXERipRemove), Is.True);
                Assert.That(RunCommand("-m", testPCKRip, testPCKRip), Is.False);
                Assert.That(RunCommand("-m", testPCKRip, testEXERipRemove), Is.False);

                {
                    using var pckReader = new PCKReader();
                    Assert.That(pckReader.OpenFile(testEXERipRemove), Is.True);
                }
            }

            Title("-s");
            {
                Assert.That(RunCommand("-s", testEXERipRemove, "kek", "extra arg"), Is.False);
                var splitExeCopy = Path.ChangeExtension(testEXERipRemove, ".copy" + Exe(""));
                TUtils.CopyFile(testEXERipRemove, splitExeCopy);

                Assert.That(RunCommand("-s", testEXERipRemove), Is.True);
                Assert.That(RunCommand("-s", splitExeCopy, testEXESplit), Is.True);

                {
                    using var pckReader = new PCKReader();
                    Assert.That(pckReader.OpenFile(testEXERipRemove), Is.False);
                    Assert.That(pckReader.OpenFile(Path.ChangeExtension(testEXERipRemove, ".pck")), Is.True);

                    Assert.That(pckReader.OpenFile(testEXESplit), Is.False);
                    Assert.That(pckReader.OpenFile(Path.ChangeExtension(testEXESplit, ".pck")), Is.True);
                }
            }

            Title("-e, -es");
            {
                Assert.That(RunCommand("-e", testPCK, testExtractFolder, "", "extra arg"), Is.False);

                Assert.That(RunCommand("-e", testPCK, testExtractFolder), Is.True);
                Assert.That(RunCommand("-e", testPCK, testPCK), Is.False);
                Assert.That(RunCommand("-e", testEXE, testPCK), Is.False);

                if (GodotVersionMajor > 3)
                {
                    Assert.That(RunCommand("-e", encPCK, testExtractFolder + "Enc"), Is.False);
                    Assert.That(RunCommand("-e", encPCK, testExtractFolder + "Enc", wrong_enc_key), Is.False);
                    Assert.That(RunCommand("-e", encPCK, testExtractFolder + "Enc", enc_key), Is.True);

                    CompareRealFilesAndPCK(testExtractFolder + "Enc", encPCK, encPCK, false, enc_key);

                    // "skip" and "encrypted" does not works if Index is encrypted

                    // skip
                    Assert.That(RunCommand("-e", encPCK, testExtractFolder + "EncSkip", "skip"), Is.False);
                    Assert.That(RunCommand("-e", encPCK, testExtractFolder + "EncEncrypted", "encrypted"), Is.False);

                    Assert.That(RunCommand("-e", encOnlyFilesPCK, testExtractFolder + "EncSkip", "skip"), Is.True);
                    Assert.That(PCKUtils.GetListOfFilesToPack(testExtractFolder + "EncSkip", ver), Has.Count.EqualTo(0));

                    // encrypted
                    Assert.That(RunCommand("-e", encOnlyFilesPCK, testExtractFolder + "EncEncrypted", "encrypted"), Is.True);
                    CompareRealFilesAndPCK(testExtractFolder + "EncEncrypted", encOnlyFilesPCK, encOnlyFilesPCK, true, enc_key, f => f + ".encrypted");
                }
            }

            // just testing the arguments. Everything else is tested in a separate test.
            Title("-p, -pe");
            {
                Assert.That(RunCommand("-p", testExtractFolder, testPCKPack, verStr, "", enc_key, "both", "extra arg"), Is.False);

                Assert.That(RunCommand("-p", testExtractFolder, testPCKPack, verStr, ""), Is.True);
                Assert.That(RunCommand("-p", testExtractFolder, testExtractFolder, verStr, ""), Is.False);
                Assert.That(RunCommand("-p", testExtractFolder, testPCKPack, "", ""), Is.False);
                if (GodotVersionMajor > 3)
                {
                    Assert.That(RunCommand("-p", testExtractFolder, testPCKPackEnc, verStr, "", enc_key, "both"), Is.True);
                    CompareRealFilesAndPCK(testExtractFolder, testPCK, testPCKPackEnc, false, enc_key);

                    Assert.That(RunCommand("-p", testExtractFolder, testPCKPackEnc, verStr, "", "wrong key", "both"), Is.False);
                }
            }

            Title("-pc, -pce");
            {
                Assert.That(RunCommand("-pc", testPCK, testExtractFolder, testPCKPack, verStr, "", enc_key, "both", "extra arg"), Is.False);

                Assert.That(RunCommand("-pc", testPCK, testExtractFolder, testPCKPack, verStr, ""), Is.True);
                Assert.That(RunCommand("-pc", testPCK, testExtractFolder, testExtractFolder, verStr, ""), Is.False);
                Assert.That(RunCommand("-pc", testPCK, testExtractFolder, testPCKPack, "", ""), Is.False);
                if (GodotVersionMajor > 3)
                {
                    Assert.That(RunCommand("-pc", testPCK, testExtractFolder, testPCKPackEnc, verStr, "", enc_key, "both"), Is.True);
                    CompareRealFilesAndPCK(testExtractFolder, testPCK, testPCKPackEnc, false, enc_key);

                    Assert.That(RunCommand("-pc", testPCK, testExtractFolder, testPCKPackEnc, verStr, "", "wrong key", "both"), Is.False);
                }
            }
        }
    }

    public class MyFixtureData
    {
        public static IEnumerable FixtureParams
        {
            get
            {
                yield return new TestFixtureData(3, 3);
                yield return new TestFixtureData(4, 422);
                yield return new TestFixtureData(4, 440);
            }
        }
    }
}
