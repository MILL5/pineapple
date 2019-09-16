using System;
using System.Diagnostics;

namespace Pineapple.Data
{
    [DebuggerDisplay("{ToString(),nq}")]
    public struct RowVersion : IComparable, IEquatable<RowVersion>, IComparable<RowVersion>
    {
        private readonly ulong _value;

        public static readonly RowVersion Zero = default;
        public static readonly RowVersion MinValue = default;
        public static readonly RowVersion MaxValue = (RowVersion)(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 });

        private RowVersion(ulong value)
        {
            _value = value;
        }

        public static implicit operator RowVersion(ulong value)
        {
            return new RowVersion(value);
        }

        public static implicit operator RowVersion(long value)
        {
            return new RowVersion(unchecked((ulong)value));
        }

        private static RowVersion ByteArrayToRowVersion(byte[] value)
        {
            if (value.Length == 0) return RowVersion.MinValue;

            if (value.Length == 1 && value[0] == 0) return RowVersion.MinValue;

            return new RowVersion(((ulong)value[0] << 56) | ((ulong)value[1] << 48) | ((ulong)value[2] << 40) | ((ulong)value[3] << 32) | ((ulong)value[4] << 24) | ((ulong)value[5] << 16) | ((ulong)value[6] << 8) | value[7]);
        }

        public static explicit operator RowVersion(byte[] value)
        {
            return ByteArrayToRowVersion(value);
        }
        
        public static explicit operator RowVersion?(byte[] value)
        {
            if (value == null) return null;

            return ByteArrayToRowVersion(value);
        }

        public static implicit operator byte[](RowVersion RowVersion)
        {
            var value = RowVersion._value;

            var r = new byte[8];
            r[0] = (byte)(value >> 56);
            r[1] = (byte)(value >> 48);
            r[2] = (byte)(value >> 40);
            r[3] = (byte)(value >> 32);
            r[4] = (byte)(value >> 24);
            r[5] = (byte)(value >> 16);
            r[6] = (byte)(value >> 8);
            r[7] = (byte)value;
            return r;
        }

        public override bool Equals(object obj)
        {
            return obj is RowVersion && Equals((RowVersion)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public bool Equals(RowVersion other)
        {
            return other._value == _value;
        }

        int IComparable.CompareTo(object obj)
        {
            return obj == null ? 1 : CompareTo((RowVersion)obj);
        }

        public int CompareTo(RowVersion other)
        {
            return _value == other._value ? 0 : _value < other._value ? -1 : 1;
        }

        public static bool operator ==(RowVersion x, RowVersion y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(RowVersion x, RowVersion y)
        {
            return !x.Equals(y);
        }
        
        public static bool operator >(RowVersion x, RowVersion y)
        {
            return x.CompareTo(y) > 0;
        }
        
        public static bool operator >=(RowVersion x, RowVersion y)
        {
            return x.CompareTo(y) >= 0;
        }
        
        public static bool operator <(RowVersion x, RowVersion y)
        {
            return x.CompareTo(y) < 0;
        }
        
        public static bool operator <=(RowVersion x, RowVersion y)
        {
            return x.CompareTo(y) <= 0;
        }

        public override string ToString()
        {
            return _value.ToString("x16");
        }

        public static RowVersion Max(RowVersion x, RowVersion y)
        {
            return x._value < y._value ? y : x;
        }
    }
}
