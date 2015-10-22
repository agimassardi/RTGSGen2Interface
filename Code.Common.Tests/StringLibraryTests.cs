using Microsoft.VisualStudio.TestTools.UnitTesting;
using Code.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Common.Tests
{
    [TestClass()]
    public class StringLibraryTests
    {
        [TestMethod()]
        public void AddLeadingZeroTestNumberIsValid()
        {
            int number = 13003;
            var expected = "013003";

            var actual = number.AddLeadingZero(6);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AddLeadingZeroTestNumberIsNull()
        {
            int? number = null;
            var expected = "000000";

            var actual = number.AddLeadingZero(6);

            Assert.AreEqual(expected, actual);
        }
    }
}