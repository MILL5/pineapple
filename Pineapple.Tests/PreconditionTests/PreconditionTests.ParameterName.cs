using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        public void CheckParameterNameNullTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                CheckIsNotNull(NullString, AnObject);
            });
        }

        [TestMethod]
        public void CheckParameterNameEmptyTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotNull(string.Empty, AnObject);
            });
        }

        [TestMethod]
        public void CheckParameterNameWhitespaceTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotNull(WhitespaceString, AnObject);
            });
        }

        [TestMethod]
        public void CheckParameterNameTest()
        {
            CheckIsNotNull(nameof(AnObject), AnObject);
        }
    }
}
