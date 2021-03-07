using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Pineapple.Common.Preconditions;
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Pineapple.Tests
{
	public partial class PreconditionTests
	{
		private const int SomeInt = 32;
		private const int SomeLong = 33;
		private const int SomeUShort = 34;
		private const int SomeUInt = 35;
		private const int SomeULong = 36;

		#region CheckIsNotGreaterThan

		[TestMethod]
		public void CheckIsNotGreaterThanIntTest()
		{
			CheckIsNotGreaterThan(nameof(SomeInt), SomeInt, SomeInt);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotGreaterThanIntFalseTest()
		{
			CheckIsNotGreaterThan(nameof(SomeInt), SomeInt+1, SomeInt);
		}

		public void CheckIsNotGreaterThanLongTest()
		{
			CheckIsNotGreaterThan(nameof(SomeLong), SomeLong, SomeLong);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotGreaterThanLongFalseTest()
		{
			CheckIsNotGreaterThan(nameof(SomeLong), SomeLong+1, SomeLong);
		}

		[TestMethod]
		public void CheckIsNotGreaterThanUShortTest()
		{
			CheckIsNotGreaterThan(nameof(SomeUShort), SomeUShort, SomeUShort);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotGreaterThanUShortFalseTest()
		{
			CheckIsNotGreaterThan(nameof(SomeUShort), SomeUShort + 1, SomeUShort);
		}

		[TestMethod]
		public void CheckIsNotGreaterThanUIntTest()
		{
			CheckIsNotGreaterThan(nameof(SomeUInt), SomeUInt, SomeUInt);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotGreaterThanUIntFalseTest()
		{
			CheckIsNotGreaterThan(nameof(SomeUInt), SomeUInt+1, SomeUInt);
		}

		public void CheckIsNotGreaterThanULongTest()
		{
			CheckIsNotGreaterThan(nameof(SomeULong), SomeULong+1, SomeULong);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotGreaterThanULongFalseTest()
		{
			CheckIsNotGreaterThan(nameof(SomeULong), SomeULong + 1, SomeULong);
		}

		#endregion CheckIsNotGreaterThan

		#region CheckIsNotLessThan

		[TestMethod]
		public void CheckIsNotLessThanIntTest()
		{
			CheckIsNotLessThan(nameof(SomeInt), SomeInt, SomeInt);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanIntFalseTest()
		{
			CheckIsNotLessThan(nameof(SomeInt), SomeInt-1, SomeInt);
		}

		public void CheckIsNotLessThanLongTest()
		{
			CheckIsNotLessThan(nameof(SomeLong), SomeLong, SomeLong);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanLongFalseTest()
		{
			CheckIsNotLessThan(nameof(SomeLong), SomeLong-1, SomeLong);
		}

		[TestMethod]
		public void CheckIsNotLessThanUShortTest()
		{
			CheckIsNotLessThan(nameof(SomeUShort), SomeUShort, SomeUShort);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanUShortFalseTest()
		{
			CheckIsNotLessThan(nameof(SomeUShort), SomeUShort-1, SomeUShort);
		}

		[TestMethod]
		public void CheckIsNotLessThanUIntTest()
		{
			CheckIsNotLessThan(nameof(SomeUInt), SomeUInt, SomeUInt);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanUIntFalseTest()
		{
			CheckIsNotLessThan(nameof(SomeUInt), SomeUInt-1, SomeUInt);
		}

		public void CheckIsNotLessThanULongTest()
		{
			CheckIsNotLessThan(nameof(SomeULong), SomeULong, SomeULong);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanULongFalseTest()
		{
			CheckIsNotLessThan(nameof(SomeULong), SomeULong-1, SomeULong);
		}

		#endregion CheckIsNotLessThan

		#region CheckIsNotLessThanOrEqualTo

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToIntTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeInt), SomeInt+1, SomeInt);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanOrEqualToIntFalseTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeInt), SomeInt, SomeInt);
		}

		public void CheckIsNotLessThanOrEqualToLongTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeLong), SomeLong+1, SomeLong);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanOrEqualToLongFalseTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeLong), SomeLong, SomeLong);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToUShortTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeUShort), SomeUShort+1, SomeUShort);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanOrEqualToUShortFalseTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeUShort), SomeUShort, SomeUShort);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToUIntTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeUInt), SomeUInt+1, SomeUInt);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanOrEqualToUIntFalseTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeUInt), SomeUInt, SomeUInt);
		}

		public void CheckIsNotLessThanOrEqualToULongTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeULong), SomeULong+1, SomeULong);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CheckIsNotLessThanOrEqualToULongFalseTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeULong), SomeULong, SomeULong);
		}

		#endregion CheckIsNotLessThanOrEqualTo

	}
}
