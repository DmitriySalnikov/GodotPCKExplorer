using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
            Program.skipReadKey = true;

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
            string testEXE = Path.Combine(binaries, "Test.exe");
            string testPCK = Path.Combine(binaries, "Test.pck");
            string testEmbedPack = Path.Combine(binaries, "TestPack.exe");
            string selectedFilesPck = Path.Combine(binaries, "SelecetedFiles.pck");
            string exportTestSelectedPath = Path.Combine(binaries, "ExportTestSelected");
            string newPckPath = Path.Combine(binaries, "out.pck");
            string exportTestSelectedWrongPath = Path.Combine(binaries, "ExportTestSelectedWrong");
            string overwritePath = exportTestPath + "Overwrite";
            string out_exe = Path.ChangeExtension(newPckPath, ".exe");

            foreach (var d in new string[] { overwritePath, exportTestSelectedPath, exportTestPath, exportTestSelectedWrongPath })
                if (Directory.Exists(d))
                    Directory.Delete(d, true);

            foreach (var f in new string[] { selectedFilesPck, testEmbedPack, out_exe })
                if (File.Exists(f))
                    File.Delete(f);

            Title("Extract");
            Assert.IsTrue(Utils.ExtractPCKRun(testPCK, exportTestPath, true));

            Title("Extract Wrong Path");
            Assert.IsFalse(Utils.ExtractPCKRun(Path.Combine(binaries, "WrongPath/Test.pck"), exportTestPath, true));

            Title("Compare content with folder");
            var list_of_files = Utils.ScanFoldersForFiles(Path.GetFullPath(exportTestPath));
            {
                var pck = new PCKReader();
                Assert.IsTrue(pck.OpenFile(testPCK));
                Assert.AreEqual(pck.Files.Count, list_of_files.Count);

                foreach (var f in list_of_files)
                    Assert.IsTrue(pck.Files.ContainsKey(f.Path));

                pck.Close();
            }

            Title("Extract without overwrite");


            Assert.IsTrue(Utils.ExtractPCKRun(testPCK, exportTestPath, true));

            // select at least one file
            var rnd = new Random();
            var first = false;
            var seleceted_files = list_of_files.Where((f) =>
            {
                if (!first)
                {
                    first = true;
                    return true;
                }
                return rnd.NextDouble() > 0.5;
            }).ToList();

            Title("Extract only seleceted files and compare");
            {
                var export_files = seleceted_files.Select((s) => s.Path);

                Assert.IsTrue(Utils.ExtractPCKRun(testPCK, exportTestSelectedPath, true, export_files));

                var exportedSelectedList = Utils.ScanFoldersForFiles(exportTestSelectedPath);
                Assert.AreEqual(exportedSelectedList.Count, seleceted_files.Count);

                foreach (var f in export_files)
                    Assert.IsTrue(exportedSelectedList.FindIndex((l) => l.Path == f) != -1);


                Title("Extract only seleceted wrong files and compare");
                var wrong_selected = export_files.ToList();
                for (int i = 0; i < wrong_selected.Count; i++)
                    wrong_selected[i] = wrong_selected[i] + "WrongFile";

                Assert.IsTrue(Utils.ExtractPCKRun(testPCK, exportTestSelectedWrongPath, true, wrong_selected));
                Assert.IsFalse(Directory.Exists(exportTestSelectedWrongPath));

                Title("Extract empty list");
                Assert.IsFalse(Utils.ExtractPCKRun(testPCK, exportTestSelectedWrongPath, true, new string[] { }));
            }


            Title("Extract without overwrite");
            {
                Assert.IsTrue(Utils.ExtractPCKRun(testPCK, overwritePath, true));
                var files = Directory.GetFiles(overwritePath);
                File.Delete(files[0]);
                File.WriteAllText(files[0], "Test");
                File.Delete(files[1]);

                Assert.IsTrue(Utils.ExtractPCKRun(testPCK, overwritePath, false));

                Assert.AreEqual("Test", File.ReadAllText(files[0]));
                Assert.IsTrue(File.Exists(files[1]));
            }


            string ver = "";
            {
                Title("Get original version");
                string console_output = "";
                using (var output = new ConsoleOutputRedirect())
                {
                    Assert.IsTrue(Utils.InfoPCKRun(testPCK));
                    console_output = output.GetOuput();
                    var lines = console_output.Replace("\r", "").Split('\n');
                    ver = lines[lines.Length - 2].Split(':')[1];
                }
                Console.WriteLine(console_output);
                Console.WriteLine($"Found version: {ver}");
            }


            {
                Title("Pack new PCK");

                Assert.IsTrue(Utils.PackPCKRun(exportTestPath, newPckPath, ver));

                Title("Locked file");
                string locked_file = Path.Combine(exportTestPath, "out.lock");
                using (var f = new LockedFile(locked_file))
                    Assert.IsFalse(Utils.PackPCKRun(exportTestPath, locked_file, ver));

                Title("Wrong version and directory");
                Assert.IsFalse(Utils.PackPCKRun(exportTestPath, newPckPath, "1234"));
                Assert.IsFalse(Utils.PackPCKRun(exportTestPath, newPckPath, "123.33.2.1"));
                Assert.IsFalse(Utils.PackPCKRun(exportTestPath, newPckPath, "-1.0.2.1"));
                Assert.IsFalse(Utils.PackPCKRun(exportTestPath + "WrongPath", newPckPath, ver));

                // Compare new PCK content with alredy existing trusted list of files 'list_of_files'
                Title("Compare files to original list");
                {
                    var pck = new PCKReader();
                    Assert.IsTrue(pck.OpenFile(testPCK));
                    foreach (var f in pck.Files.Keys)
                        Assert.IsTrue(list_of_files.FindIndex((l) => l.Path == f) != -1);

                    pck.Close();
                }
            }

            {
                Title("Pack embedded");

                File.Copy(testEXE, testEmbedPack);

                Assert.IsTrue(Utils.PackPCKRun(exportTestPath, testEmbedPack, ver, true));
                Assert.IsTrue(File.Exists(Path.ChangeExtension(testEmbedPack, ".old.exe")));

                Title("Pack embedded again");
                Assert.IsFalse(Utils.PackPCKRun(exportTestPath, testEmbedPack, ver, true));
                Assert.IsFalse(File.Exists(Path.ChangeExtension(testEmbedPack, ".old.exe")));
            }


            {
                Title("Pack only selected files");

                Assert.IsTrue(Utils.PackPCKRun(seleceted_files, selectedFilesPck, ver));

                Title("Compare selected to pack content with new pck");
                {
                    var pck = new PCKReader();
                    Assert.IsTrue(pck.OpenFile(selectedFilesPck));
                    Assert.AreEqual(pck.Files.Count, seleceted_files.Count);

                    foreach (var f in pck.Files.Keys)
                        Assert.IsTrue(seleceted_files.FindIndex((l) => l.Path == f) != -1);

                    pck.Close();
                }
            }

            Title("Good run");

            File.Copy(testEXE, out_exe);

            using (var r = new RunAppWithOutput(out_exe, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            // test embed pack
            using (var r = new RunAppWithOutput(testEmbedPack, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            Title("Run without PCK");
            if (File.Exists(newPckPath))
                File.Delete(newPckPath);

            using (var r = new RunAppWithOutput(out_exe, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));
        }


        [TestMethod]
        public void TestMergePCK()
        {
            string testEXE = Path.Combine(binaries, "Test.exe");
            string testPCK = Path.Combine(binaries, "Test.pck");
            string newEXE = Path.Combine(binaries, "TestMerge.exe");
            string newEXE_old = Path.Combine(binaries, "TestMerge.old.exe");

            foreach (var f in new string[] { newEXE_old, newEXE })
                if (File.Exists(f))
                    File.Delete(f);

            File.Copy(testEXE, newEXE);

            Title("Merge");
            Assert.IsTrue(Utils.MergePCKRun(testPCK, newEXE));
            Assert.IsTrue(File.Exists(newEXE_old));

            Title("Again");
            Assert.IsFalse(Utils.MergePCKRun(testPCK, newEXE));

            File.Delete(newEXE);
            File.Copy(testEXE, newEXE);

            Title("Merge without backup");
            Assert.IsTrue(Utils.MergePCKRun(testPCK, newEXE, true));
            Assert.IsFalse(File.Exists(newEXE_old));

            Title("Locked backup");
            File.Delete(newEXE);
            File.Copy(testEXE, newEXE);
            // creates new old.exe 0kb
            using (var l = new LockedFile(newEXE_old, false))
                Assert.IsFalse(Utils.MergePCKRun(testPCK, newEXE, true));

            Title("Locked pck file");
            using (var l = new LockedFile(testPCK, false))
                Assert.IsFalse(Utils.MergePCKRun(testPCK, newEXE));

            Title("Wrong Files");
            Assert.IsFalse(Utils.MergePCKRun(testPCK + "Wrong", newEXE));
            Assert.IsFalse(Utils.MergePCKRun(testPCK, newEXE + "Wrong", true));

            Title("Same File");
            Assert.IsFalse(Utils.MergePCKRun(testPCK, testPCK, true));

            Title("Bad run");
            using (var r = new RunAppWithOutput(newEXE, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));

            Title("Good run");
            File.Delete(newEXE);
            File.Copy(testEXE, newEXE);
            Assert.IsTrue(Utils.MergePCKRun(testPCK, newEXE));
            using (var r = new RunAppWithOutput(newEXE, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

        }


        [TestMethod]
        public void TestRipPCK()
        {
            string new_exe = Path.Combine(binaries, "TestRip.exe");
            string new_exe_old = Path.Combine(binaries, "TestRip.old.exe");
            string new_pck = Path.Combine(binaries, "TestRip.pck");
            string locked_exe_str = Path.Combine(binaries, "TestLockedRip.exe");

            foreach (var f in new string[] { new_exe, new_pck, new_exe_old, locked_exe_str })
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
            Assert.IsTrue(Utils.RipPCKRun(new_exe, null, true));
            Assert.IsFalse(File.Exists(new_exe_old));

            Title("Good run");
            using (var r = new RunAppWithOutput(new_exe, "", 1000))
                Assert.IsFalse(r.GetConsoleText().Contains(pck_error));

            Title("Run without PCK");
            if (File.Exists(new_pck))
                File.Delete(new_pck);
            using (var r = new RunAppWithOutput(new_exe, "", 1000))
                Assert.IsTrue(r.GetConsoleText().Contains(pck_error));

            Title("Rip locked");

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
            string new_exe = Path.Combine(binaries, "SplitFolder", "Split.exe");
            string new_pck = Path.Combine(binaries, "SplitFolder", "Split.pck");

            foreach (var f in new string[] { exe, pck, new_exe, new_pck })
                if (File.Exists(f))
                    File.Delete(f);

            File.Copy(Path.Combine(binaries, "TestEmbedded.exe"), exe);

            Title("Split with custom pair name and check files");
            Assert.IsTrue(Utils.SplitPCKRun(exe, new_exe));
            Assert.IsTrue(File.Exists(new_exe));
            Assert.IsTrue(File.Exists(new_pck));
            Assert.IsFalse(File.Exists(Path.ChangeExtension(new_exe, ".old.exe")));

            Title("Can't copy with same name");
            Assert.IsFalse(Utils.SplitPCKRun(exe, exe));

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

            Title("Split with locked output");
            foreach (var f in new string[] { new_exe, new_pck })
                if (File.Exists(f))
                    File.Delete(f);
            File.Copy(Path.Combine(binaries, "Test.pck"), new_pck);

            using (var l = new LockedFile(new_pck, false))
                Assert.IsFalse(Utils.SplitPCKRun(Path.Combine(binaries, "TestEmbedded.exe"), new_exe));

            Assert.IsFalse(File.Exists(new_exe));
            Assert.IsTrue(File.Exists(new_pck));
        }
    }
}
