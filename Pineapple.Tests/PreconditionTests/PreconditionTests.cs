using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Pineapple.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public partial class PreconditionTests
    {
        private const string NullString = null;
        private readonly string EmptyString = string.Empty;
        private const string WhitespaceString = "      ";
        private const string SomeString = "Abc123";
        private const string BadUri = "/\\\as&!@#!@D";
        private const string RelativeUri = "/hello/world";
        private const string AbsoluteUri = "https://www.google.com/hello/world";

        private static readonly object NullObject = null;
        private static readonly object AnObject = new object();

        #region CheckIsNotCondition using Func<bool> Tests
        [TestMethod]
        public void CheckIsNotConditionFuncTrueTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), () => true, () => "Condition was met.");
            });
        }

        [TestMethod]
        public void CheckIsNotConditionFuncFalseTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => false, () => "Condition was not met.");
        }

        [TestMethod]
        public void CheckIsNotConditionFuncTrueNullMessageTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), () => true, "Some condition was met");
            });
        }

        [TestMethod]
        public void CheckIsNotConditionFuncTrueNullMessageFromFuncTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), () => true, () => null);
            });
        }

        [TestMethod]
        public void CheckIsNotConditionFuncTrueEmptyMessageFromFuncTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), () => true, () => EmptyString);
            });
        }

        [TestMethod]
        public void CheckIsNotConditionFuncTrueWhitespaceMessageFromFuncTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsNotCondition(nameof(SomeString), () => true, () => WhitespaceString);
            });
        }

        #endregion

        #region CheckNotExpression Tests
        [TestMethod]
        public void CheckNotExpressionFalseTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                bool value = true;

                CheckNotExpression(nameof(SomeString), value);
            });
        }

        [TestMethod]
        public void CheckNotExpressionTrueTest()
        {
            bool value = false;

            CheckNotExpression(nameof(SomeString), value);
        }
        #endregion

        #region CheckIsNotOnSynchronizationContext Tests
        public class TestSynchronizationContext : SynchronizationContext
        {

        }

        [TestMethod]
        public void CheckIsNotOnSynchronizationContextWithoutContextTest()
        {
            SynchronizationContext saved = SynchronizationContext.Current;

            SynchronizationContext.SetSynchronizationContext(null);

            try
            {
                CheckIsNotOnSynchronizationContext();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(saved);
            }
        }

        [TestMethod]
        public void CheckIsNotOnSynchronizationContextWithContextTest()
        {
            Assert.ThrowsExactly<Exception>(() =>
            {
                SynchronizationContext saved = SynchronizationContext.Current;

                SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());

                try
                {
                    CheckIsNotOnSynchronizationContext();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(saved);
                }
            });
        }

        #endregion

        #region CheckIsWellFormedUri Tests

        [TestMethod]
        public void CheckIsWellFormedUriDefaultTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsWellFormedUri(nameof(BadUri), BadUri);
            });
        }

        [TestMethod]
        public void CheckIsWellFormedUriRelativeTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsWellFormedUri(nameof(BadUri), BadUri, UriKind.Relative);
            });
        }

        [TestMethod]
        public void CheckIsWellFormedUriRelativeOrAbsoluteTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsWellFormedUri(nameof(BadUri), BadUri, UriKind.RelativeOrAbsolute);
            });
        }

        [TestMethod]
        public void CheckIsWellFormedRelativeUriDefaultTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsWellFormedUri(nameof(RelativeUri), RelativeUri);
            });
        }

        [TestMethod]
        public void CheckIsWellFormedRelativeUriRelativeTest()
        {
            CheckIsWellFormedUri(nameof(RelativeUri), RelativeUri, UriKind.Relative);
        }

        [TestMethod]
        public void CheckIsWellFormedRelativeUriRelativeOrAbsoluteTest()
        {
            CheckIsWellFormedUri(nameof(RelativeUri), RelativeUri, UriKind.RelativeOrAbsolute);
        }

        [TestMethod]
        public void CheckIsWellFormedAbsoluteUriDefaultTest()
        {
            CheckIsWellFormedUri(nameof(AbsoluteUri), AbsoluteUri);
        }

        [TestMethod]
        public void CheckIsWellFormedAbsoluteUriRelativeTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                CheckIsWellFormedUri(nameof(AbsoluteUri), AbsoluteUri, UriKind.Relative);
            });
        }

        [TestMethod]
        public void CheckIsWellFormedAbsoluteUriRelativeOrAbsoluteTest()
        {
            CheckIsWellFormedUri(nameof(AbsoluteUri), AbsoluteUri, UriKind.RelativeOrAbsolute);
        }
        #endregion
    }
}
