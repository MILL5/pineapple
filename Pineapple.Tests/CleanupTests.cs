using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CleanupTests
    {
        [TestMethod]
        public void CleanupSafeMethodTest()
        {
            bool success = false;

            SafeMethod(() => { success = true; });

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodExceptionTest()
        {
            bool success = false;

            SafeMethod(() =>
            {
                success = true;
                throw new Exception();
            });

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodAggregateExceptionTest()
        {
            bool success = false;

            SafeMethod(() =>
            {
                success = true;
                throw new AggregateException();
            });

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodWithContinueTest()
        {
            bool success = false;

            SafeMethod(() => {  })
                .Continue(() => success = true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodExceptionContinueTest()
        {
            bool success = false;

            SafeMethod(() => throw new Exception())
                .Continue(() => success = true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodAggregateExceptionContinueTest()
        {
            bool success = false;

            SafeMethod(() => throw new AggregateException())
                .Continue(() => success = true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodWithOnSuccessTest()
        {
            bool success = false;

            SafeMethod(() => { })
                .OnSuccess(() => success = true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodWithOnFailureTest()
        {
            bool success = true;

            SafeMethod(() => { })
                .OnFailure(() => success = false);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodExceptionWithOnFailureTest()
        {
            bool success = false;

            SafeMethod(() => throw new Exception())
                .OnFailure(() => success = true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodAggregateExceptionOnFailureTest()
        {
            bool success = false;

            SafeMethod(() => throw new AggregateException())
                .OnFailure(() => success = true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodExceptionWithOnSuccessTest()
        {
            bool success = true;

            SafeMethod(() => throw new Exception())
                .OnSuccess(() => success = false);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void CleanupSafeMethodAggregateExceptionOnSuccessTest()
        {
            bool success = true;

            SafeMethod(() => throw new AggregateException())
                .OnSuccess(() => success = false);

            Assert.IsTrue(success);
        }

    }
}
