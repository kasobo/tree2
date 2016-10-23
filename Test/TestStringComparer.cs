using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FullSys;
using System.Collections.Generic;

namespace TestConCommand
{
    [TestClass]
    public class TestSorting
    {
        [TestMethod]
        public void TestCompare1()
        {
            string a1 = "aa", a2 = "aa";
            string b1 = "ab", b2 = "ba";
            string c1 = "a23d", c2 = "a4cd";
            string d1 = "CD2", d2 = "CD10";

            IComparer<string> comp = new NaturalCompareStringWin32();

            Assert.IsTrue (comp.Compare (a1, a2) == 0);
            Assert.IsTrue (comp.Compare (b1, b2) < 0);
            Assert.IsTrue (comp.Compare (b2, b1) > 0);
            Assert.IsTrue (comp.Compare (c1, c2) > 0);
            Assert.IsTrue (comp.Compare (d1, d2) < 0);
            Assert.IsTrue (comp.Compare (d2, d1) > 0);
        }
    }
}
