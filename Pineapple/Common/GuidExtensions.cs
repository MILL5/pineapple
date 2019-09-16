using System;
using System.Collections.Generic;
using System.Linq;

using static Pineapple.Common.Preconditions;

namespace Pineapple.Common
{
    public static class GuidExtensions
    {
        private class SqlGuidComparer : IComparer<Guid>
        {

            static SqlGuidComparer()
            {
            }

            private static int CompareTo(Guid x, Guid y)
            {
                byte byte1, byte2;

                byte[] xBytes = x.ToByteArray();
                byte[] yBytes = y.ToByteArray();

                byte1 = xBytes[10];
                byte2 = yBytes[10];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[11];
                byte2 = yBytes[11];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[12];
                byte2 = yBytes[12];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[13];
                byte2 = yBytes[13];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[14];
                byte2 = yBytes[14];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[15];
                byte2 = yBytes[15];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[8];
                byte2 = yBytes[8];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[9];
                byte2 = yBytes[9];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[6];
                byte2 = yBytes[6];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[7];
                byte2 = yBytes[7];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[4];
                byte2 = yBytes[4];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[5];
                byte2 = yBytes[5];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[0];
                byte2 = yBytes[0];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[1];
                byte2 = yBytes[1];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[2];
                byte2 = yBytes[2];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                byte1 = xBytes[3];
                byte2 = yBytes[3];

                if (byte1 != byte2)
                    return (byte1 < byte2) ? -1 : 1;

                return 0;
            }

            public int Compare(Guid x, Guid y)
            {
                return CompareTo(x, y);
            }
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
