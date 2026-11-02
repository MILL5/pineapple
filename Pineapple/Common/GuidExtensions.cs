using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

using static Pineapple.Common.Preconditions;

namespace Pineapple.Common
{
    public static class GuidExtensions
    {
        // SqlGuid compares using SQL Server's uniqueidentifier byte order (bytes 10-15, 8-9, 6-7, 4-5, 0-3).
        private sealed class SqlGuidComparer : IComparer<Guid>
        {
            public int Compare(Guid x, Guid y) => new SqlGuid(x).CompareTo(new SqlGuid(y));
        }

        private static readonly IComparer<Guid> _sqlGuidComparer = new SqlGuidComparer();

        public static bool IsGreaterThan(this Guid left, Guid right) => left.SqlCompareTo(right) > 0;
        public static bool IsGreaterThanOrEqual(this Guid left, Guid right) => left.SqlCompareTo(right) >= 0;
        public static bool IsLessThan(this Guid left, Guid right) => left.SqlCompareTo(right) < 0;
        public static bool IsLessThanOrEqual(this Guid left, Guid right) => left.SqlCompareTo(right) <= 0;

        public static int SqlCompareTo(this Guid x, Guid y)
        {
            return _sqlGuidComparer.Compare(x, y);
        }

        public static IOrderedEnumerable<TSource> OrderByGuid<TSource>(this IEnumerable<TSource> source, Func<TSource, Guid> keySelector)
        {
            CheckIsNotNull(nameof(source), source);
            return source.OrderBy(keySelector, _sqlGuidComparer);
        }

        public static IOrderedEnumerable<TSource> OrderByGuidDescending<TSource>(this IEnumerable<TSource> source, Func<TSource, Guid> keySelector)
        {
            return source.OrderByDescending(keySelector, _sqlGuidComparer);
        }
    }
}
