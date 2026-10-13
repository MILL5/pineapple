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
		public void CheckIsNotGreaterThanIntFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotGreaterThan(nameof(SomeInt), SomeInt+1, SomeInt);
			});
		}

		public void CheckIsNotGreaterThanLongTest()
		{
			CheckIsNotGreaterThan(nameof(SomeLong), SomeLong, SomeLong);
		}

		[TestMethod]
		public void CheckIsNotGreaterThanLongFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotGreaterThan(nameof(SomeLong), SomeLong+1, SomeLong);
			});
		}

		[TestMethod]
		public void CheckIsNotGreaterThanUShortTest()
		{
			CheckIsNotGreaterThan(nameof(SomeUShort), SomeUShort, SomeUShort);
		}

		[TestMethod]
		public void CheckIsNotGreaterThanUShortFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotGreaterThan(nameof(SomeUShort), SomeUShort + 1, SomeUShort);
			});
		}

		[TestMethod]
		public void CheckIsNotGreaterThanUIntTest()
		{
			CheckIsNotGreaterThan(nameof(SomeUInt), SomeUInt, SomeUInt);
		}

		[TestMethod]
		public void CheckIsNotGreaterThanUIntFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotGreaterThan(nameof(SomeUInt), SomeUInt+1, SomeUInt);
			});
		}

		public void CheckIsNotGreaterThanULongTest()
		{
			CheckIsNotGreaterThan(nameof(SomeULong), SomeULong+1, SomeULong);
		}

		[TestMethod]
		public void CheckIsNotGreaterThanULongFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotGreaterThan(nameof(SomeULong), SomeULong + 1, SomeULong);
			});
		}

		#endregion CheckIsNotGreaterThan

		#region CheckIsNotLessThan

		[TestMethod]
		public void CheckIsNotLessThanIntTest()
		{
			CheckIsNotLessThan(nameof(SomeInt), SomeInt, SomeInt);
		}

		[TestMethod]
		public void CheckIsNotLessThanIntFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThan(nameof(SomeInt), SomeInt-1, SomeInt);
			});
		}

		public void CheckIsNotLessThanLongTest()
		{
			CheckIsNotLessThan(nameof(SomeLong), SomeLong, SomeLong);
		}

		[TestMethod]
		public void CheckIsNotLessThanLongFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThan(nameof(SomeLong), SomeLong-1, SomeLong);
			});
		}

		[TestMethod]
		public void CheckIsNotLessThanUShortTest()
		{
			CheckIsNotLessThan(nameof(SomeUShort), SomeUShort, SomeUShort);
		}

		[TestMethod]
		public void CheckIsNotLessThanUShortFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThan(nameof(SomeUShort), SomeUShort-1, SomeUShort);
			});
		}

		[TestMethod]
		public void CheckIsNotLessThanUIntTest()
		{
			CheckIsNotLessThan(nameof(SomeUInt), SomeUInt, SomeUInt);
		}

		[TestMethod]
		public void CheckIsNotLessThanUIntFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThan(nameof(SomeUInt), SomeUInt-1, SomeUInt);
			});
		}

		public void CheckIsNotLessThanULongTest()
		{
			CheckIsNotLessThan(nameof(SomeULong), SomeULong, SomeULong);
		}

		[TestMethod]
		public void CheckIsNotLessThanULongFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThan(nameof(SomeULong), SomeULong-1, SomeULong);
			});
		}

		#endregion CheckIsNotLessThan

		#region CheckIsNotLessThanOrEqualTo

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToIntTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeInt), SomeInt+1, SomeInt);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToIntFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThanOrEqualTo(nameof(SomeInt), SomeInt, SomeInt);
			});
		}

		public void CheckIsNotLessThanOrEqualToLongTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeLong), SomeLong+1, SomeLong);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToLongFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThanOrEqualTo(nameof(SomeLong), SomeLong, SomeLong);
			});
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToUShortTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeUShort), SomeUShort+1, SomeUShort);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToUShortFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThanOrEqualTo(nameof(SomeUShort), SomeUShort, SomeUShort);
			});
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToUIntTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeUInt), SomeUInt+1, SomeUInt);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToUIntFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThanOrEqualTo(nameof(SomeUInt), SomeUInt, SomeUInt);
			});
		}

		public void CheckIsNotLessThanOrEqualToULongTest()
		{
			CheckIsNotLessThanOrEqualTo(nameof(SomeULong), SomeULong+1, SomeULong);
		}

		[TestMethod]
		public void CheckIsNotLessThanOrEqualToULongFalseTest()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				CheckIsNotLessThanOrEqualTo(nameof(SomeULong), SomeULong, SomeULong);
			});
		}

		#endregion CheckIsNotLessThanOrEqualTo

	}
}
