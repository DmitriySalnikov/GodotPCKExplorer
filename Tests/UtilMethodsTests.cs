using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using GodotPCKExplorer;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class UtilMethodsTests
    {
        const string binaries = "TestBinaries";
        const string pck_error = "Error: Couldn't load project data at path \".\". Is the .pck file missing?";

        void Title(string name)
        {
            Console.WriteLine($"===================={name}====================");
        }

        [TestInitialize]
        public void GodotPCKInit()
        {
            Program.CMDMode = true;
            Program.SkipReadKey = true;

            if (!Directory.Exists(binaries))
                System.IO.Compression.ZipFile.ExtractToDirectory("../../Test.zip", binaries);
        }

        [TestMethod]
        [Ignore]
        public void ClearBinaries()
        {
            Directory.Delete(binaries, true);
            Assert.IsFalse(Directory.Exists(binaries));
        }

        [TestMethod]
        public void TestHelpCommand()
        {
            Assert.IsTrue(Utils.HelpRun());
        }

        [TestMethod]
        public void TestOpenCommand()
        {
            Application.Idle += (sender, e) => Application.Exit();
            Title("Open");
            Assert.IsTrue(Utils.OpenPCKRun(Path.Combine(binaries, "Test.pck")));

            Title("Wrong Path");
            Assert.IsFalse(Utils.OpenPCKRun(Path.Combine(binaries, "WrongPath/Test.pck")));
        }

        [TestMethod]
        public void TestInfoCommand()
        {
            Title("Info PCK");
            Assert.IsTrue(Utils.InfoPCKRun(Path.Combine(binaries, "Test.pck")));
            Title("Info EXE");
            Assert.IsTrue(Utils.InfoPCKRun(Path.Combine(binaries, "Test.exe")));
            Title("Wrong path");
            Assert.IsFalse(Utils.InfoPCKRun(Path.Combine(binaries, "WrongPath/Test.pck")));
        }

        [TestMethod]
        public void TestExtractScanPackCommand()
        {
            string exportTestPath = Path.Combine(binaries, "ExportTest");
            string testPCK = Path.Combine(binaries, "Test.pck");
            if (Directory.Exists(exportTestPath))
                Directory.Delete(exportTestPath, true);

            Title("Extract");
            Assert.IsTrue(Utils.ExtractPCKRun(testPCK, exportTestPath, true));

            Title("Extract Wrong Path");
            Assert.IsFalse(Utils.ExtractPCKRun(Path.Combine(binaries, "WrongPath/Test.pck"), exportTestPath, true));

            Title("Compare content with folder");
            var new_list = new List<PCKPacker.FileToPack>();
            {

                var pck = new PCKReader();
                Assert.IsTrue(pck.OpenFile(Path.Combine(binaries, "Test.pck")));
                var files = pck.Files;

                var abs_path = Path.GetFullPath(exportTestPath);
                Utils.ScanFoldersForFiles(abs_path, new_list, ref abs_path);

                Assert.AreEqual(files.Count, new_list.Count);

                foreach (var f in new_list)
                    Assert.IsTrue(files.ContainsKey(f.Path));

                pck.Close();
            }

            Title("Extract without overwrite");
            {
                var overwritePath = exportTestPath + "Overwrite";
                if (Directory.Exists(overwritePath))
                    Directory.Delete(overwritePath, true);

                Assert.IsTrue(Utils.ExtractPCKRun(testPCK, overwritePath, true));
                var files = Directory.GetFiles(overwritePath);
                File.Delete(files[0]);
                File.WriteAllText(files[0], "Test");
                File.Delete(files[1]);

                Assert.IsTrue(Utils.ExtractPCKRun(testPCK, overwritePath, false));

                Assert.AreEqual("Test", File.ReadAllText(files[0]));
                Assert.IsTrue(File.Exists(files[1]));
            }


            Title("Get original version");
            string ver = "";
            using (var output = new ConsoleOutputRedirect())
            {
                Assert.IsTrue(Utils.InfoPCKRun(testPCK));
                var lines = output.GetOuput().Replace("\r", "").Split('\n');
                ver = lines[lines.Length - 2].Split(':')[1]; ;
            }

            Title("Pack new PCK");
            string newPckPath = Path.Combine(exportTestPath, "out.pck");
            Assert.IsTrue(Utils.PackPCKRun(exportTestPath, newPckPath, ver));

            Title("Locked file");
            string locked_file = Path.Combine(exportTestPath, "out.lock");
            using (var f = new LockedFile(locked_file))
                Assert.IsFalse(Utils.PackPCKRun(exportTestPath, locked_file, ver));

            Title("Wrong version and directory");
            Assert.IsFalse(Utils.PackPCKRun(exportTestPath, newPckPath, "1234"));
            Assert.IsFalse(Utils.PackPCKRun(exportTestPath + "WrongPath", newPckPath, ver));

            // Compare new PCK content with alredy existing trusted list of files 'new_list'
            Title("Compare files to original list");
            {
                var pck = new PCKReader();
                Assert.IsTrue(pck.OpenFile(Path.Combine(binaries, "Test.pck")));
                foreach (var f in pck.Files.Keys)
                    Assert.IsTrue(new_list.FindIndex((l) => l.Path == f) != -1);

                pck.Close();
            }

            Title("Good run");
            var out_exe = Path.ChangeExtension(newPckPath, ".exe");
            if (File.Exists(out_exe))
                File.Delete(out_exe);
            File.Copy(Path.Combine(binaries, "Test.exe"), out_exe);

            using (var r = new RunAppWithOutput(out_exe, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            Title("Run without PCK");
            if (File.Exists(newPckPath))
                File.Delete(newPckPath);

            using (var r = new RunAppWithOutput(out_exe, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));
        }

        [TestMethod]
        public void TestRipPCK()
        {
            string new_exe = Path.Combine(binaries, "TestRip.exe");
            string new_exe_old = Path.Combine(binaries, "TestRip.old.exe");
            string new_pck = Path.Combine(binaries, "TestRip.pck");

            foreach (var f in new string[] { new_exe, new_pck, new_exe_old })
                if (File.Exists(f))
                    File.Delete(f);

            File.Copy(Path.Combine(binaries, "TestEmbedded.exe"), new_exe);

            Title("Rip embedded");
            Assert.IsTrue(Utils.RipPCKRun(new_exe, new_pck));

            Title("Rip wrong files");
            Assert.IsFalse(Utils.RipPCKRun(Path.Combine(binaries, "Test.exe"), new_pck));
            Assert.IsFalse(Utils.RipPCKRun(new_pck, new_pck));

            Title("Locked file");
            string locked_file = Path.Combine(binaries, "test.lock");
            using (var f = new LockedFile(locked_file))
                Assert.IsFalse(Utils.RipPCKRun(new_exe, locked_file));


            Title("Rip PCK from exe");
            Assert.IsTrue(Utils.RipPCKRun(new_exe));

            Title("Good run");
            using (var r = new RunAppWithOutput(new_exe, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            Title("Run without PCK");
            if (File.Exists(new_pck))
                File.Delete(new_pck);
            using (var r = new RunAppWithOutput(new_exe, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));

            Title("Rip locked");
            string locked_exe_str = Path.Combine(binaries, "TestLockedRip.exe");
            if (File.Exists(locked_exe_str))
                File.Delete(locked_exe_str);
            File.Copy(Path.Combine(binaries, "TestEmbedded.exe"), locked_exe_str);

            using (var locked_exe = File.OpenWrite(locked_exe_str))
            {
                Assert.IsFalse(Utils.RipPCKRun(locked_exe_str));
            }

            Title("Rip and remove .old");
            Assert.IsTrue(Utils.RipPCKRun(locked_exe_str, null, true));
            Assert.IsFalse(File.Exists(Path.ChangeExtension(locked_exe_str, ".old.exe")));
        }

        [TestMethod]
        public void TestSplitPCK()
        {
            string exe = Path.Combine(binaries, "TestSplit.exe");
            string pck = Path.Combine(binaries, "TestSplit.pck");
            string new_exe = Path.Combine(binaries, "Split.exe");
            string new_pck = Path.Combine(binaries, "Split.pck");

            foreach (var f in new string[] { exe, pck, new_exe, new_pck })
                if (File.Exists(f))
                    File.Delete(f);

            File.Copy(Path.Combine(binaries, "TestEmbedded.exe"), exe);

            Title("Split with custom pair name and check files");
            Assert.IsTrue(Utils.SplitPCKRun(exe, "Split"));
            Assert.IsTrue(File.Exists(new_exe));
            Assert.IsTrue(File.Exists(new_pck));
            Assert.IsFalse(File.Exists(Path.ChangeExtension(new_exe, ".old.exe")));

            Title("Can't copy with same name");
            Assert.IsFalse(Utils.SplitPCKRun(exe, "TestSplit"));

            Title("Split with same name");
            Assert.IsTrue(Utils.SplitPCKRun(exe));
            Assert.IsTrue(File.Exists(exe));
            Assert.IsTrue(File.Exists(pck));
            Assert.IsFalse(File.Exists(Path.ChangeExtension(new_exe, ".old.exe")));

            Title("Already splitted");
            Assert.IsFalse(Utils.SplitPCKRun(exe));

            Title("Good runs");
            using (var r = new RunAppWithOutput(exe, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            using (var r = new RunAppWithOutput(new_exe, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            Title("Bad runs");
            foreach (var f in new string[] { pck, new_pck })
                if (File.Exists(f))
                    File.Delete(f);

            using (var r = new RunAppWithOutput(exe, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));

            using (var r = new RunAppWithOutput(new_exe, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));
        }
    }
}
