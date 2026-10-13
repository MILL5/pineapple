using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        public void CheckIsNotNullOrWhitespaceNullStringTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                CheckIsNotNullOrWhitespace(nameof(NullString), NullString);
            });
        }

        [TestMethod]
        public void CheckIsNotNullOrWhitespaceEmptyStringTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotNullOrWhitespace(nameof(EmptyString), EmptyString);
            });
        }

        [TestMethod]
        public void CheckIsNotNullOrWhitespaceWhitespaceStringTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotNullOrWhitespace(nameof(WhitespaceString), WhitespaceString);
            });
        }
    }
}
