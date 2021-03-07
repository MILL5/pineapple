using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Diagnostics;

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]

    public class TimingScopeTests
    {
        public class TestLogger : ILogger
        {
            private LogLevel _currentLevel;

            private class NoopDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }

            public TestLogger(LogLevel currentLevel)
            {
                _currentLevel = currentLevel;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                LogCalled = true;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                IsEnabledCalled = true;
                var enabled = logLevel.Equals(_currentLevel);
                return enabled;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                BeginScopeCalled = true;
                return new NoopDisposable();
            }

            public bool LogCalled { get; set; }

            public bool IsEnabledCalled { get; set; }

            public bool BeginScopeCalled { get; set; }
        }

        [TestMethod]
        public void TimingScopeOneSecondLoggingEnabledTest()
        {
            var logger = new TestLogger(LogLevel.Debug);
            using (new TimingScope("Five Seconds", logger))
            {
                Thread.Sleep(1000);
            }

            Assert.IsTrue(logger.IsEnabledCalled, "Did not check the log level.");
            Assert.IsTrue(logger.LogCalled, "Nothing was logged.");
            Assert.IsTrue(logger.BeginScopeCalled, "Did not initiate a logging begin scope.");
        }

        [TestMethod]
        public void TimingScopeOneSecondLoggingDisabledTest()
        {
            var logger = new TestLogger(LogLevel.None);
            using (new TimingScope("Five Seconds", logger))
            {
                Thread.Sleep(1000);
            }

            Assert.IsTrue(logger.IsEnabledCalled, "Did not check the log level.");
            Assert.IsFalse(logger.LogCalled, "Should not have logged.");
            Assert.IsFalse(logger.BeginScopeCalled, "Should not have initiated a logging begin scope.");
        }
    }
}
