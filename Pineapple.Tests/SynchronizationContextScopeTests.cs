using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Threading;

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]

    public class SynchronizationContextScopeTests
    {

        [TestMethod]
        public void SynchronizationContextScopeTest()
        {
            var saved = SynchronizationContext.Current;

            var sc = new SynchronizationContext();
            using (new SynchronizationContextScope(sc))
            {
                Assert.IsFalse(SynchronizationContext.Current == null, "SynchronizationContext should be set.");
            }

            Assert.IsTrue(SynchronizationContext.Current == saved, "SynchronizationContext should be set to the previous context.");
        }

        [TestMethod]
        public void SuppressSynchronizationContextScopeTest()
        {
            var saved = SynchronizationContext.Current;

            var sc = new SynchronizationContext();
            using (new SynchronizationContextScope(sc))
            {
                using (new SuppressSynchronizationContextScope())
                {
                    Assert.IsTrue(SynchronizationContext.Current == null, "SynchronizationContext should not be set.");
                }
            }

            Assert.IsTrue(SynchronizationContext.Current == saved, "SynchronizationContext should be set to the initial context.");
        }
    }
}
