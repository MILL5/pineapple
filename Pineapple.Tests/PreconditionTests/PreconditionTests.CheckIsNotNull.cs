using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsNotNullNullObjectTest()
        {
            CheckIsNotNull(nameof(NullObject), NullObject);
        }

        [TestMethod]
        public void CheckIsNotAnObjectTest()
        {
            CheckIsNotNull(nameof(NullObject), AnObject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsNotNullNullStringTest()
        {
            CheckIsNotNull(nameof(NullString), NullString);
        }

        [TestMethod]
        public void CheckIsNotNullSomeStringTest()
        {
            CheckIsNotNull(nameof(NullString), SomeString);
        }
    }
}
