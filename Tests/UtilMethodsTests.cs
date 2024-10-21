using System.Reflection;
using System.Collections;
using System.IO.Compression;
using GodotPCKExplorer;
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

        string binaries_base;
        string binaries;

        List<string> OriginalTestFiles = [];

        public UtilMethodsTests(int version)
        {
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
            if (!PCKActions.IsInited)
                PCKActions.Init(new ProgressReporterTests($"{TestContext.CurrentContext.Test.MethodName ?? "!NoTest!"}({GodotVersion})"));

            binaries_base = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "TestBinaries", $"{GodotVersion}_{TestContext.CurrentContext.Test.MethodName}");
            binaries = Path.Combine(binaries_base, PlatformFolder);

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

        /* TODO add UI tests?..
        [Test]
        public void TestOpenCommand()
        {
            Application.Idle += (sender, e) => Application.Exit();
            Title("Open");
            Assert.IsTrue(PCKActions.OpenPCKRun(Path.Combine(binaries, "Test.pck")));
            PCKActions.ClosePCK();

            Title("Wrong Path");
            Assert.IsFalse(PCKActions.OpenPCKRun(Path.Combine(binaries, "WrongPath/Test.pck")));
            PCKActions.ClosePCK();
        }
        */

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
            string extractTestUserPck = Path.Combine(binaries, "TestUser.pck");
            string extractTestUserPath = Path.Combine(binaries, "ExtractUser");

            Title("Extract");
            Assert.That(PCKActions.Extract(testPCK, extractTestPath, true), Is.True);

            Title("Extract Wrong Path");
            Assert.That(PCKActions.Extract(Path.Combine(binaries, "WrongPath/Test.pck"), extractTestPath, true), Is.False);

            Title("Compare content with folder");
            var list_of_files = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestPath));
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

                var exportedSelectedList = PCKUtils.GetListOfFilesToPack(extractTestSelectedPath);
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
            string ver = GetPCKVersion(testPCK).ToString();
            {
                Assert.That(PCKActions.Pack(extractTestPath, newPckPath, ver), Is.True);

                if (OperatingSystem.IsWindows())
                {
                    Title("Locked file");
                    string locked_file = Path.Combine(extractTestPath, "out.lock");
                    using var f = new LockedFile(locked_file);
                    Assert.That(PCKActions.Pack(extractTestPath, locked_file, ver), Is.False);
                }
            }

            Title("Pack and Extract PCK with prefix");
            {
                Assert.That(PCKActions.Pack(extractTestPath, testPrefixPCK, ver, packPathPrefix: "test_prefix_like_mod_folder/"), Is.True);
                Assert.That(PCKActions.Extract(testPrefixPCK, extractTestPrefix), Is.True);

                var list_of_files_with_prefix = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestPrefix));
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
                Assert.That(PCKActions.Pack(extractTestPath + "WrongPath", newPckPath, ver), Is.False);
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
                Assert.That(PCKActions.Pack(extractTestPath, testEmbedPack, ver, embed: true), Is.True);
                Assert.That(File.Exists(Path.ChangeExtension(testEmbedPack, Exe(".old"))), Is.True);
            }

            Title("Pack embedded again");
            {
                Assert.That(PCKActions.Pack(extractTestPath, testEmbedPack, ver, embed: true), Is.False);
                Assert.That(File.Exists(Path.ChangeExtension(testEmbedPack, Exe(".old"))), Is.False);
            }

            Title("Pack only selected files");

            Assert.That(PCKActions.Pack(seleceted_files, selectedFilesPck, ver), Is.True);

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
                var user_test_file = "test.empty";
                var user_test_file_dir_path = Path.Combine(extractTestPath, "@@user@@", user_test_file).Replace("\\", "/");

                var user_test_wrong_file = "test2.empty";
                var user_test_wrong_file_local_path = Path.Combine("somefolder", "@@user@@", user_test_wrong_file).Replace("\\", "/");
                var user_test_wrong_file_dir_path = Path.Combine(extractTestPath, user_test_wrong_file_local_path).Replace("\\", "/");
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(user_test_file_dir_path)!);
                    Directory.CreateDirectory(Path.GetDirectoryName(user_test_wrong_file_dir_path)!);
                    using var f = File.Create(user_test_file_dir_path);
                    using var f2 = File.Create(user_test_wrong_file_dir_path);
                }
                Assert.That(PCKActions.Pack(extractTestPath, extractTestUserPck, ver), Is.True);
                Assert.That(PCKActions.Extract(extractTestUserPck, extractTestUserPath), Is.True);

                var list_of_files_user = PCKUtils.GetListOfFilesToPack(Path.GetFullPath(extractTestUserPath));
                {
                    using var pck = new PCKReader();

                    Assert.That(pck.OpenFile(extractTestUserPck), Is.True);
                    Assert.That(list_of_files_user, Has.Count.EqualTo(pck.Files.Count));

                    Assert.That(list_of_files_user.Find(i => i.Path == PCKUtils.PathPrefixUser + user_test_file) != null, Is.True);
                    Assert.That(pck.Files.ContainsKey(PCKUtils.PathPrefixUser + user_test_file), Is.True);


                    // only @@user@@ in root allowed
                    Assert.That(list_of_files_user.Find(i => i.Path == PCKUtils.PathPrefixUser + user_test_wrong_file) != null, Is.False);
                    Assert.That(pck.Files.ContainsKey(PCKUtils.PathPrefixUser + user_test_wrong_file), Is.False);
                    // check for wrong files in res://somefolder/@@user@@
                    Assert.That(list_of_files_user.Find(i => i.Path == PCKUtils.PathPrefixRes + user_test_wrong_file_local_path) != null, Is.True);
                    Assert.That(pck.Files.ContainsKey(PCKUtils.PathPrefixRes + user_test_wrong_file_local_path), Is.True);

                    foreach (var f in list_of_files_user)
                        Assert.That(pck.Files.ContainsKey(f.Path), Is.True);
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
                if (GodotVersion == 3)
                    Assert.That(r.IsSuccess(), Is.True);
                else if (GodotVersion == 4)
                    Assert.That(r.IsSuccess(), Is.False);
        }

        [Test]
        public void TestRipPCK()
        {
            string new_exe = Path.Combine(binaries, Exe("TestRip"));
            string new_exe_old = Path.Combine(binaries, Exe("TestRip.old"));
            string new_pck = Path.Combine(binaries, "TestRip.pck");
            string locked_exe_str = Path.Combine(binaries, Exe("TestLockedRip"));

            TUtils.CopyFile(Path.Combine(binaries, Exe("TestEmbedded")), new_exe);

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
                Assert.That(File.Exists(new_exe_old), Is.False);
            }

            Title("Rip PCK from PCK");
            Assert.That(PCKActions.Rip(new_pck, null, true), Is.False);

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
            };

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
            };
        }

        [Test]
        public void TestEncryption()
        {
            if (GodotVersion == 3)
                Assert.Inconclusive("Not applicable!");

            string enc_key = "7FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA";
            string wrong_enc_key = "8FDBF68B69B838194A6F1055395225BBA3F1C5689D08D71DCD620A7068F61CBA";
            string ver = "2.4.0.2";

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
            Title("Extract PCK. Encrypted only files");
            Assert.That(PCKActions.Extract(pck_new_files, extracted, true), Is.False);
            Title("Extract PCK no Key skip");
            Assert.That(PCKActions.Extract(pck_new_files, extracted + "Skip", true, encKey: "", noKeyMode: PCKExtractNoEncryptionKeyMode.Skip), Is.True);
            Assert.That(Directory.Exists(extracted + "Skip"), Is.False); // If the files are not extracted, the folder is not created
            Title("Extract PCK no Key encrypted");
            Assert.That(PCKActions.Extract(pck_new_files, extracted + "Encrypted", true, encKey: "", noKeyMode: PCKExtractNoEncryptionKeyMode.AsIs), Is.True);
            // AsIs creates .encrypted copies
            Assert.That(PCKUtils.GetListOfFilesToPack(extracted + "Encrypted").Select(f => f.Path).Order().SequenceEqual(original_files.Select(f => f + ".encrypted")), Is.True);
            // Test MD5 for all files
            var new_md5s = PCKUtils.GetListOfFilesToPack(extracted + "Encrypted").OrderBy(f => f.Path).Select(f => { f.CalculateMD5(); return PCKUtils.ByteArrayToHexString(f.MD5); }).ToArray();
            string[] orig_md5s;
            {
                PCKFile[] ordered_files = null!;
                using var pckReader = new PCKReader();
                pckReader.OpenFile(pck_new_files, getEncryptionKey: () => new PCKReaderEncryptionKeyResult() { Key = enc_key });
                ordered_files = [.. pckReader.Files.Select(f => f.Value).OrderBy(f => f.FilePath)];

                orig_md5s = ordered_files.Select(f =>
                {
                    pckReader.ReaderStream!.BaseStream.Seek(f.Offset, SeekOrigin.Begin);
                    using var enc_file = new PCKEncryptedReader(pckReader.ReaderStream!, []);
                    return PCKUtils.ByteArrayToHexString(PCKUtils.GetStreamMD5(pckReader.ReaderStream!.BaseStream, f.Offset, f.Offset + PCKEncryptedReader.EncryptionHeaderSize + enc_file.DataSizeEncoded));
                }).ToArray();
            }
            Assert.That(new_md5s.SequenceEqual(orig_md5s), Is.True);

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
    }

    public class MyFixtureData
    {
        public static IEnumerable FixtureParams
        {
            get
            {
                yield return new TestFixtureData(3);
                yield return new TestFixtureData(4);
            }
        }
    }
}
