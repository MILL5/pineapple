using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Health;

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CheckTests
    {
        [TestMethod]
        public void CheckIsAliveTest()
        {
            Assert.IsTrue(Check.IsAlive("www.google.com", 80));
        }

        [TestMethod]
        public void CheckIsAliveFailTest()
        {
            Assert.IsFalse(Check.IsAlive("not-a-real-server.google.com", 80));
        }

        [TestMethod]
        public void CheckIsUrlAvailableTest()
        {
            Assert.IsTrue(Check.IsUrlAvailable("http://www.google.com", 10000));
        }

        [TestMethod]
        public void CheckIsUrlAvailableFailTest()
        {
            Assert.IsFalse(Check.IsUrlAvailable("http://not-a-real-server.not-a-real-domain.com", 10000));
        }

        [TestMethod]
        public void CheckIsRunningVisualStudioTest()
        {
            Assert.IsTrue(Check.IsRunning("devenv"));
        }

        [TestMethod]
        public void CheckIsRunningVisualStudioBadNameTest()
        {
            Assert.IsFalse(Check.IsRunning("devenv.exe"));
        }

        [TestMethod]
        public void CheckIsRunningTestConsoleTest()
        {
            Assert.IsTrue(Check.IsRunning("vstest.console"));
        }

        [TestMethod]
        public void CheckIsRunningFailTest()
        {
            Assert.IsFalse(Check.IsRunning("not-a-real-process"));
        }
    }
}
