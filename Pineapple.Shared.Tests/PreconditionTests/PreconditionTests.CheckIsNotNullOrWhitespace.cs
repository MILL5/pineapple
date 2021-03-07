using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsNotNullOrWhitespaceNullStringTest()
        {
            CheckIsNotNullOrWhitespace(nameof(NullString), NullString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotNullOrWhitespaceEmptyStringTest()
        {
            CheckIsNotNullOrWhitespace(nameof(EmptyString), EmptyString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotNullOrWhitespaceWhitespaceStringTest()
        {
            CheckIsNotNullOrWhitespace(nameof(WhitespaceString), WhitespaceString);
        }
    }
}
