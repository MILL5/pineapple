using System;
using System.Globalization;
using System.Linq;
using System.Text;

using static Pineapple.Common.Preconditions;

namespace Pineapple.Data
{
    public struct RowVersion : IComparable
    {
        private readonly byte[] _rowVersion;
        private ulong? _rowVersionAsUlong;

        public static readonly RowVersion MinValue = new RowVersion(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        public static readonly RowVersion MaxValue = new RowVersion(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 });

        public RowVersion(byte[] rowVersion)
        {
            CheckIsNotNull(nameof(rowVersion), rowVersion);
            CheckIsEqualTo(nameof(rowVersion.Length), rowVersion.Length, 8);

            _rowVersion = rowVersion;
            _rowVersionAsUlong = null;
        }

        public byte[] ToByteArray()
        {
            return _rowVersion;
        }

        public ulong ToUInt64()
        {
            if (_rowVersionAsUlong == null)
            {
                _rowVersionAsUlong = BitConverter.ToUInt64(_rowVersion, 0);
            }

            return _rowVersionAsUlong.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is RowVersion other ? other._rowVersion.SequenceEqual(_rowVersion) : false;
        }

        public override int GetHashCode()
        {
            return _rowVersion.GetHashCode();
        }

        public override string ToString()
        {
            var value = _rowVersion;

            var stringBuilder = new StringBuilder(18);
            stringBuilder.Append("0x");

            foreach (var @byte in value)
            {
                stringBuilder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }

        public int CompareTo(object obj)
        {
            if ((obj == null) || !(obj is RowVersion other))
                return -1;

            if (this < other)
                return -1;

            if (this > other)
                return 1;

            return 0;
        }

        public static bool operator ==(RowVersion x, RowVersion y)
        {
            return x._rowVersion.SequenceEqual(y._rowVersion);
        }

        public static bool operator !=(RowVersion x, RowVersion y)
        {
            return !x._rowVersion.SequenceEqual(y._rowVersion);
        }

        public static bool operator >=(RowVersion x, RowVersion y)
        {
            return x.ToUInt64() >= y.ToUInt64();
        }

        public static bool operator >(RowVersion x, RowVersion y)
        {
            return x.ToUInt64() > y.ToUInt64();
        }

        public static bool operator <=(RowVersion x, RowVersion y)
        {
            return x.ToUInt64() <= y.ToUInt64();
        }

        public static bool operator <(RowVersion x, RowVersion y)
        {
            return x.ToUInt64() < y.ToUInt64();
        }
    }
}
