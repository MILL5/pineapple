using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Tests
{
    public partial class PreconditionTests
    {
        [TestMethod]
        public void CheckIsNotConditionTrueTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), true, () => "Condition was met.");
            });
        }

        [TestMethod]
        public void CheckIsNotConditionFalseTest()
        {
            CheckIsNotCondition(nameof(SomeString), false, () => "Condition was not met.");
        }

        [TestMethod]
        public void CheckIsNotConditionTrueNullMessageTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), true, "Some condition was not met.");
            });
        }

        [TestMethod]
        public void CheckIsNotConditionTrueNullMessageFromFuncTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), true, () => null);
            });
        }

        [TestMethod]
        public void CheckIsNotConditionTrueEmptyMessageFromFuncTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), true, () => EmptyString);
            });
        }

        [TestMethod]
        public void CheckIsNotConditionTrueWhitespaceMessageFromFuncTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), true, () => WhitespaceString);
            });
        }
    }
}
