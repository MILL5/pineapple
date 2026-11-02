using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Data;

namespace Pineapple.Tests
{
    [TestClass]
    public class RowVersionConversionTests
    {
        [TestMethod]
        public void RoundTripsBigEndianBytes()
        {
            var bytes = new byte[] { 0, 0, 0, 0, 0, 0, 0x07, 0xd1 };
            var version = (RowVersion)bytes;

            Assert.AreEqual("00000000000007d1", version.ToString());
            CollectionAssert.AreEqual(bytes, (byte[])version);
        }

        [TestMethod]
        public void RejectsWrongLength()
        {
            Assert.ThrowsExactly<ArgumentException>(() => _ = (RowVersion)new byte[] { 1, 2, 3 });
        }
    }
}
