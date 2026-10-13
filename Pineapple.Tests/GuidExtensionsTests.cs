using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Common;

namespace Pineapple.Tests
{
    [TestClass]
    public class GuidExtensionsTests
    {
        // SQL Server compares uniqueidentifier values by the last 6 bytes first.
        private static readonly Guid HighFirstGroup = new("ffffffff-ffff-ffff-ffff-000000000001");
        private static readonly Guid LowFirstGroup = new("00000000-0000-0000-0000-000000000002");

        [TestMethod]
        public void SqlCompareToUsesSqlServerByteOrder()
        {
            Assert.IsLessThan(0, HighFirstGroup.SqlCompareTo(LowFirstGroup));
            Assert.IsGreaterThan(0, LowFirstGroup.SqlCompareTo(HighFirstGroup));
            Assert.AreEqual(0, LowFirstGroup.SqlCompareTo(LowFirstGroup));
        }

        [TestMethod]
        public void ComparisonOperatorsUseSqlServerByteOrder()
        {
            Assert.IsTrue(LowFirstGroup.IsGreaterThan(HighFirstGroup));
            Assert.IsTrue(LowFirstGroup.IsGreaterThanOrEqual(LowFirstGroup));
            Assert.IsTrue(HighFirstGroup.IsLessThan(LowFirstGroup));
            Assert.IsTrue(HighFirstGroup.IsLessThanOrEqual(HighFirstGroup));
        }

        [TestMethod]
        public void OrderByGuidMatchesSqlServer()
        {
            var ids = new[] { LowFirstGroup, HighFirstGroup };

            CollectionAssert.AreEqual(new[] { HighFirstGroup, LowFirstGroup }, ids.OrderByGuid(x => x).ToArray());
            CollectionAssert.AreEqual(new[] { LowFirstGroup, HighFirstGroup }, ids.OrderByGuidDescending(x => x).ToArray());
        }
    }
}
