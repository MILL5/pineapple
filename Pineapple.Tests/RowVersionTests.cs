using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pineapple.Tests
{
    [TestClass]
    public class RowVersionTests
    {
        [TestMethod]
        public void RowVersionCompareMinMax()
        {
            var min = RowVersion.MinValue;
            var max = RowVersion.MaxValue;

            Assert.AreNotEqual(min, max);
            Assert.IsTrue(min < max);
            Assert.IsTrue(min <= max);
            Assert.IsTrue(max > min);
            Assert.IsTrue(max >= min);
            Assert.IsTrue(max != min);
            Assert.IsFalse(max == min);
        }

        [TestMethod]
        public void RowVersionCompareMin()
        {
            var min = RowVersion.MinValue;
            var other = new RowVersion(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });

            Assert.AreEqual(min, other);
            Assert.IsTrue(min == other);
            Assert.IsTrue(min <= other);
            Assert.IsTrue(min >= other);
            Assert.IsFalse(min < other);
            Assert.IsFalse(min > other);
            Assert.IsFalse(min != other);
        }

        [TestMethod]
        public void RowVersionCompareMax()
        {
            var max = RowVersion.MaxValue;
            var other = new RowVersion(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 });

            Assert.AreEqual(max, other);
            Assert.IsTrue(max == other);
            Assert.IsTrue(max <= other);
            Assert.IsTrue(max >= other);
            Assert.IsFalse(max < other);
            Assert.IsFalse(max > other);
            Assert.IsFalse(max != other);
        }

        [TestMethod]
        public void RowVersionSelectMax()
        {
            var min = RowVersion.MinValue;
            var max = RowVersion.MaxValue;
            var other = new RowVersion(new byte[] { 255, 4, 255, 4, 255, 4, 255, 4 });

            var list = new List<RowVersion> { min, max, other };

            var foundMax = list.Max();

            Assert.AreEqual(max, foundMax);
            Assert.IsTrue(max == foundMax);
            Assert.IsTrue(max <= foundMax);
            Assert.IsTrue(max >= foundMax);
            Assert.IsFalse(max < foundMax);
            Assert.IsFalse(max > foundMax);
            Assert.IsFalse(max != foundMax);
        }

        [TestMethod]
        public void RowVersionSelectMin()
        {
            var min = RowVersion.MinValue;
            var max = RowVersion.MaxValue;
            var other = new RowVersion(new byte[] { 255, 4, 255, 4, 255, 4, 255, 4 });

            var list = new List<RowVersion> { min, max, other };

            var foundMin = list.Min();

            Assert.AreEqual(min, foundMin);
            Assert.IsTrue(min == foundMin);
            Assert.IsTrue(min <= foundMin);
            Assert.IsTrue(min >= foundMin);
            Assert.IsFalse(min < foundMin);
            Assert.IsFalse(min > foundMin);
            Assert.IsFalse(min != foundMin);
        }
    }
}
