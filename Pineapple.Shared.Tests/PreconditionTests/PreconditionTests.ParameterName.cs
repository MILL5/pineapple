using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckParameterNameNullTest()
        {
            CheckIsNotNull(NullString, AnObject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckParameterNameEmptyTest()
        {
            CheckIsNotNull(string.Empty, AnObject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckParameterNameWhitespaceTest()
        {
            CheckIsNotNull(WhitespaceString, AnObject);
        }

        [TestMethod]
        public void CheckParameterNameTest()
        {
            CheckIsNotNull(nameof(AnObject), AnObject);
        }
    }
}
