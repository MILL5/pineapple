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
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionFuncTrueTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => true, () => "Condition was met.");
        }

        [TestMethod]
        public void CheckIsNotConditionFuncFalseTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => false, () => "Condition was not met.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionFuncTrueNullMessageTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => true, "Some condition was met");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CheckIsNotConditionFuncTrueNullMessageFromFuncTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => true, () => null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionFuncTrueEmptyMessageFromFuncTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => true, () => EmptyString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsNotConditionFuncTrueWhitespaceMessageFromFuncTest()
        {
            CheckIsNotCondition(nameof(SomeString), () => true, () => WhitespaceString);
        }

        #endregion

        #region CheckNotExpression Tests
        [TestMethod] [ExpectedException(typeof(ArgumentException))]
        public void CheckNotExpressionFalseTest()
        {
            bool value = true;

            CheckNotExpression(nameof(SomeString), value);
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
        [ExpectedException(typeof(Exception))]
        public void CheckIsNotOnSynchronizationContextWithContextTest()
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
        }

        #endregion

        #region CheckIsWellFormedUri Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsWellFormedUriDefaultTest()
        {
            CheckIsWellFormedUri(nameof(BadUri), BadUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsWellFormedUriRelativeTest()
        {
            CheckIsWellFormedUri(nameof(BadUri), BadUri, UriKind.Relative);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsWellFormedUriRelativeOrAbsoluteTest()
        {
            CheckIsWellFormedUri(nameof(BadUri), BadUri, UriKind.RelativeOrAbsolute);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsWellFormedRelativeUriDefaultTest()
        {
            CheckIsWellFormedUri(nameof(RelativeUri), RelativeUri);
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
        [ExpectedException(typeof(ArgumentException))]
        public void CheckIsWellFormedAbsoluteUriRelativeTest()
        {
            CheckIsWellFormedUri(nameof(AbsoluteUri), AbsoluteUri, UriKind.Relative);
        }

        [TestMethod]
        public void CheckIsWellFormedAbsoluteUriRelativeOrAbsoluteTest()
        {
            CheckIsWellFormedUri(nameof(AbsoluteUri), AbsoluteUri, UriKind.RelativeOrAbsolute);
        }
        #endregion
    }
}
