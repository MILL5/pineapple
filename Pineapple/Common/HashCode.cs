using System;
using System.Buffers;
using System.Buffers.Binary;
using System.ComponentModel;
using System.IO.Hashing;
using System.Text;

#nullable enable

namespace Pineapple.Common
{
    /// <summary>
    /// Combines values into a 32-bit xxHash that is identical across processes, machines, and versions
    /// (unlike <see cref="System.HashCode"/>, which is randomized per process).
    /// </summary>
    public class HashCode
    {
        private readonly XxHash32 _hash = new();

        public HashCode()
        {
        }

        public static int ToHashCode<T>(T v1)
        {
            var hash = new XxHash32();

            Append(hash, v1);

            return Digest(hash);
        }

        public static int Combine<T1, T2>(T1 v1, T2 v2)
        {
            var hash = new XxHash32();

            Append(hash, v1);
            Append(hash, v2);

            return Digest(hash);
        }

        public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
        {
            var hash = new XxHash32();

            Append(hash, v1);
            Append(hash, v2);
            Append(hash, v3);

            return Digest(hash);
        }

        public static int Combine<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
        {
            var hash = new XxHash32();

            Append(hash, v1);
            Append(hash, v2);
            Append(hash, v3);
            Append(hash, v4);

            return Digest(hash);
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            var hash = new XxHash32();

            Append(hash, v1);
            Append(hash, v2);
            Append(hash, v3);
            Append(hash, v4);
            Append(hash, v5);

            return Digest(hash);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
        {
            var hash = new XxHash32();

            Append(hash, v1);
            Append(hash, v2);
            Append(hash, v3);
            Append(hash, v4);
            Append(hash, v5);
            Append(hash, v6);

            return Digest(hash);
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
        {
            var hash = new XxHash32();

            Append(hash, v1);
            Append(hash, v2);
            Append(hash, v3);
            Append(hash, v4);
            Append(hash, v5);
            Append(hash, v6);
            Append(hash, v7);

            return Digest(hash);
        }

        public void Add<T>(T value)
        {
            Append(_hash, value);
        }

        public int ToHashCode()
        {
            return Digest(_hash);
        }

        private static int Digest(XxHash32 hash) => unchecked((int)hash.GetCurrentHashAsUInt32());

        // Byte encodings are fixed (little-endian, UTF-8, DateTime ticks, null as int 0) so hashes never change.
        // typeof(T) checks are folded away by the JIT, so primitive values are hashed without boxing.
        private static void Append<T>(XxHash32 hash, T value)
        {
            if (value is null) { AppendInt32(hash, 0); return; }

            if (typeof(T) == typeof(int)) { AppendInt32(hash, (int)(object)value); return; }
            if (typeof(T) == typeof(long)) { AppendInt64(hash, (long)(object)value); return; }
            if (typeof(T) == typeof(string)) { AppendString(hash, (string)(object)value); return; }
            if (typeof(T) == typeof(double)) { AppendInt64(hash, BitConverter.DoubleToInt64Bits((double)(object)value)); return; }
            if (typeof(T) == typeof(bool)) { AppendByte(hash, (bool)(object)value ? (byte)1 : (byte)0); return; }
            if (typeof(T) == typeof(DateTime)) { AppendInt64(hash, ((DateTime)(object)value).Ticks); return; }

            AppendObject(hash, value);
        }

        private static void AppendObject(XxHash32 hash, object item)
        {
            switch (item)
            {
                case string v: AppendString(hash, v); break;
                case int v: AppendInt32(hash, v); break;
                case long v: AppendInt64(hash, v); break;
                case uint v: AppendInt32(hash, unchecked((int)v)); break;
                case ulong v: AppendInt64(hash, unchecked((long)v)); break;
                case ushort v: AppendInt16(hash, unchecked((short)v)); break;
                case short v: AppendInt16(hash, v); break;
                case byte v: AppendByte(hash, v); break;
                case sbyte v: AppendByte(hash, unchecked((byte)v)); break;
                case float v: AppendInt32(hash, BitConverter.SingleToInt32Bits(v)); break;
                case double v: AppendInt64(hash, BitConverter.DoubleToInt64Bits(v)); break;
                case decimal v: AppendDecimal(hash, v); break;
                case DateTime v: AppendInt64(hash, v.Ticks); break;
                case char v: AppendInt16(hash, unchecked((short)v)); break;
                case bool v: AppendByte(hash, v ? (byte)1 : (byte)0); break;
                default: throw new NotSupportedException($"{item.GetType().Name} not supported.");
            }
        }

        private static void AppendByte(XxHash32 hash, byte value)
        {
            Span<byte> buffer = stackalloc byte[1];
            buffer[0] = value;
            hash.Append(buffer);
        }

        private static void AppendInt16(XxHash32 hash, short value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(short)];
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
            hash.Append(buffer);
        }

        private static void AppendInt32(XxHash32 hash, int value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            hash.Append(buffer);
        }

        private static void AppendInt64(XxHash32 hash, long value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
            hash.Append(buffer);
        }

        private static void AppendDecimal(XxHash32 hash, decimal value)
        {
            Span<int> bits = stackalloc int[4];
            decimal.GetBits(value, bits);

            foreach (var part in bits)
                AppendInt32(hash, part);
        }

        private static void AppendString(XxHash32 hash, string value)
        {
            const int StackLimit = 512;

            var maxBytes = Encoding.UTF8.GetMaxByteCount(value.Length);
            byte[]? rented = null;
            Span<byte> buffer = maxBytes <= StackLimit
                ? stackalloc byte[StackLimit]
                : (rented = ArrayPool<byte>.Shared.Rent(maxBytes));

            try
            {
                var written = Encoding.UTF8.GetBytes(value, buffer);
                hash.Append(buffer[..written]);
            }
            finally
            {
                if (rented is not null)
                    ArrayPool<byte>.Shared.Return(rented);
            }
        }

#pragma warning disable 0809
        // Disallowing GetHashCode and Equals by design

        [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => throw new NotSupportedException("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.");

        [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => throw new NotSupportedException("HashCode is a mutable struct and should not be compared with other HashCodes.");
#pragma warning restore 0809
    }
}
