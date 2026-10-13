using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        public void CheckIsNotNullNullObjectTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                CheckIsNotNull(nameof(NullObject), NullObject);
            });
        }

        [TestMethod]
        public void CheckIsNotAnObjectTest()
        {
            CheckIsNotNull(nameof(NullObject), AnObject);
        }

        [TestMethod]
        public void CheckIsNotNullNullStringTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                CheckIsNotNull(nameof(NullString), NullString);
            });
        }

        [TestMethod]
        public void CheckIsNotNullSomeStringTest()
        {
            CheckIsNotNull(nameof(NullString), SomeString);
        }
    }
}
