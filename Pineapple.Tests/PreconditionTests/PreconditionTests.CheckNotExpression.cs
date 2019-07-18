using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionTrueTest()
        {
            CheckIsNotCondition(nameof(SomeString), true, () => "Condition was met.");
        }

        [TestMethod]
        public void CheckIsNotConditionFalseTest()
        {
            CheckIsNotCondition(nameof(SomeString), false, () => "Condition was not met.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsNotConditionTrueNullMessageTest()
        {
            CheckIsNotCondition(nameof(SomeString), true, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsNotConditionTrueNullMessageFromFuncTest()
        {
            CheckIsNotCondition(nameof(SomeString), true, () => null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionTrueEmptyMessageFromFuncTest()
        {
            CheckIsNotCondition(nameof(SomeString), true, () => EmptyString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionTrueWhitespaceMessageFromFuncTest()
        {
            CheckIsNotCondition(nameof(SomeString), true, () => WhitespaceString);
        }
    }
}
