using System;
using System.Buffers.Binary;
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
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length == 0) return RowVersion.MinValue;

            if (value.Length == 1 && value[0] == 0) return RowVersion.MinValue;

            if (value.Length != sizeof(ulong))
                throw new ArgumentException($"A rowversion must be {sizeof(ulong)} bytes. Length: {value.Length}", nameof(value));

            return new RowVersion(BinaryPrimitives.ReadUInt64BigEndian(value));
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
            var r = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(r, RowVersion._value);
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
