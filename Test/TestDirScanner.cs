using System.IO;
using FullSys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestConCommand
{
    [TestClass]
    public class TestDirTree
    {
        public string Root1Name { get { return @"Targets\D1"; } }

        [TestMethod]
        public void TestStack1()
        {
            var d1 = @"Targets\D1";
            var di1 = new DirectoryInfo (d1);

            var loc = new DirLocation.Nodes (di1);
            Assert.AreEqual (-1, loc.Depth);
            loc.Advance();
            Assert.AreEqual (0, loc.Depth);
            loc.Advance();
            Assert.AreEqual (1, loc.Depth);
        }

        [TestMethod]
        public void TestScan1()
        {
            bool hasDirNope = false;
            bool hasDirD1E1 = false, hasEmpty = false;
            var d1 = @"Targets\D1";
            foreach (DirLocation.Nodes loc in new DirScanner (new DirectoryInfo (d1)))
            {
                string dn = loc.Top.Name;
                hasDirNope |= dn == "NonexistentDir";
                hasDirD1E1 |= dn == "E1";
                hasEmpty |= dn == "E5";
            }

            Assert.IsFalse (hasDirNope);
            Assert.IsTrue (hasDirD1E1);
            Assert.IsTrue (hasEmpty);
        }
    }
}
