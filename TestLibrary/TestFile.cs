using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLibrary
{
    [TestClass]
    public class TestFile
    {
        [TestMethod]
        public void TestMethod1()
        {
            var files = Library.File.CollectFiles(@"", (path) => path.EndsWith(".cs"));
            foreach (var f in files)
            {
                Debug.WriteLine(f);
            }
        }
    }
}
