using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLibrary
{
    [TestClass]
    public class TestFile
    {
        private static readonly string TESTEX = ".libtest";
        private static readonly string TESTSUBDIR = "testsub";
        private static string subDirPath = "";

        [TestInitialize]
        public void TestInitialize()
        {
            subDirPath = $@"{Path.GetTempPath()}\{TESTSUBDIR}";
            Directory.CreateDirectory(subDirPath);
        }
        [TestCleanup]
        public void TestCleanup()
        {
            var info = new DirectoryInfo(Path.GetTempPath());
            foreach (var f in info.EnumerateFiles())
            {
                if (f.FullName.EndsWith(TESTEX))
                {
                    File.Delete(f.FullName);
                }
            }
            Directory.Delete(subDirPath);
        }

        [TestMethod]
        public void TestMethod1()
        {
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                var tmpPath1 = Path.GetTempFileName();
                var tmpPath2 = Path.ChangeExtension(tmpPath1, TESTEX);
                File.Move(tmpPath1, tmpPath2);
            }
            var (files, err) = Files.CollectFiles(Path.GetTempPath(), (path) => path.EndsWith(TESTEX));
            Assert.AreEqual(count, files.Count);
            Assert.AreEqual(0, err.Count);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var tmpFiles = new List<string>();
            int count = 10;
            for (int i = 0; i < count; i++)
            {
                var tmpPath1 = Path.GetTempFileName();
                var tmpPath2 = Path.ChangeExtension(tmpPath1, TESTEX);
                var tmpPath3 = $@"{subDirPath}\{Path.GetFileName(tmpPath2)}";
                File.Move(tmpPath1, tmpPath3);
                tmpFiles.Add(tmpPath3);
            }
            var (files, errs) = Files.CollectFiles(Path.GetTempPath(), (path) => path.EndsWith(TESTEX));
            Assert.AreEqual(count, files.Count);
            Assert.AreEqual(0, errs.Count);
            tmpFiles.ForEach(f => File.Delete(f));
        }

        [TestMethod]
        public void TestMethod3()
        {
            var (files, errs) = Files.CollectFiles(@"C:\Config.Msi", (_) => false);
            Assert.AreNotEqual(0, errs.Count);
        }
    }
}
