using System;
using System.Runtime.CompilerServices;
using System.Text;
using K4os.Hash.xxHash;

namespace Pineapple.Common
{
    internal static class HashCodeHelper
    {
        public static void Add(this XXH32 hash, object item)
        {
            if (item == null)
            {
                hash.Add(0);
                return;
            }

            if (item is string)
            {
                var value = Convert.ToString(item);
                hash.Add(value);
            }
            else if (item is int)
            {
                var value = Convert.ToInt32(item);
                hash.Add(value);
            }
            else if (item is long)
            {
                var value = Convert.ToInt64(item);
                hash.Add(value);
            }
            else if (item is uint)
            {
                var value = Convert.ToUInt32(item);
                hash.Add(value);
            }
            else if (item is ulong)
            {
                var value = Convert.ToUInt64(item);
                hash.Add(value);
            }
            else if (item is ushort)
            {
                var value = Convert.ToUInt16(item);
                hash.Add(value);
            }
            else if (item is short)
            {
                var value = Convert.ToInt16(item);
                hash.Add(value);
            }
            else if (item is byte)
            {
                var value = Convert.ToByte(item);
                hash.Add(value);
            }
            else if (item is sbyte)
            {
                var value = Convert.ToSByte(item);
                hash.Add(value);
            }
            else if (item is float)
            {
                var value = Convert.ToSingle(item);
                hash.Add(value);
            }
            else if (item is double)
            {
                var value = Convert.ToDouble(item);
                hash.Add(value);
            }
            else if (item is decimal)
            {
                var value = Convert.ToDecimal(item);
                hash.Add(value);
            }
            else if (item is DateTime)
            {
                var value = Convert.ToDateTime(item);
                hash.Add(value);
            }
            else if (item is char)
            {
                var value = Convert.ToChar(item);
                hash.Add(value);
            }
            else
            {
                throw new NotSupportedException($"{item.GetType().Name} not supported.");
            }

        }

        public static void Add(this XXH32 hash, DateTime item)
        {
            hash.Add(item.Ticks);
        }

        public static void Add(this XXH32 hash, string item)
        {
            var bytes = Encoding.UTF8.GetBytes(item);
            hash.Update(bytes);
        }
        
        public static void Add(this XXH32 hash, int item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static void Add(this XXH32 hash, uint item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }
        
        public static void Add(this XXH32 hash, long item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static int ToHashCode(this ulong item)
        {
            var bytes = BitConverter.GetBytes(item);
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, ulong item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static int ToHashCode(this ulong? item)
        {
            return item.HasValue ? ToHashCode(item.Value) : 0;
        }

        public static void Add(this XXH32 hash, ulong? item)
        {
            if (item.HasValue)
            {
                var bytes = BitConverter.GetBytes(item.Value);
                hash.Update(bytes);
            }
        }

        public static int ToHashCode(this short item)
        {
            var bytes = BitConverter.GetBytes(item);
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, short item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static int ToHashCode(this short? item)
        {
            return item.HasValue ? ToHashCode(item.Value) : 0;
        }

        public static void Add(this XXH32 hash, short? item)
        {
            if (item.HasValue)
            {
                var bytes = BitConverter.GetBytes(item.Value);
                hash.Update(bytes);
            }
        }

        public static int ToHashCode(this ushort item)
        {
            var bytes = BitConverter.GetBytes(item);
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, ushort item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static int ToHashCode(this ushort? item)
        {
            return item.HasValue ? ToHashCode(item.Value) : 0;
        }

        public static void Add(this XXH32 hash, ushort? item)
        {
            if (item.HasValue)
            {
                var bytes = BitConverter.GetBytes(item.Value);
                hash.Update(bytes);
            }
        }

        public static int ToHashCode(this byte item)
        {
            var bytes = new byte[] { item };
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, byte item)
        {
            var bytes = new byte[] { item };
            hash.Update(bytes);
        }

        public static int ToHashCode(this byte? item)
        {
            return item.HasValue ? ToHashCode(item.Value) : 0;
        }

        public static void Add(this XXH32 hash, byte? item)
        {
            if (item.HasValue)
            {
                var bytes = new byte[] { item.Value };
                hash.Update(bytes);
            }
        }

        public static int ToHashCode(this sbyte item)
        {
            var bytes = new byte[] { (byte)item };
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, sbyte item)
        {
            var bytes = new byte[] { (byte)item };
            hash.Update(bytes);
        }

        public static int ToHashCode(this float item)
        {
            var bytes = BitConverter.GetBytes(item);
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, float item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static int ToHashCode(this double item)
        {
            var bytes = BitConverter.GetBytes(item);
            return ToHashCode(bytes);
        }

        public static void Add(this XXH32 hash, double item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static void Add(this XXH32 hash, decimal item)
        {
            var bits = decimal.GetBits(item);

            foreach (var b in bits)
            {
                byte[] bytes = BitConverter.GetBytes(b);
                hash.Update(bytes);
            }
        }

        public static void Add(this XXH32 hash, char item)
        {
            var bytes = BitConverter.GetBytes(item);
            hash.Update(bytes);
        }

        public static int ToHashCode(this byte[] bytes)
        {
            if (bytes == null)
                return 0;

            var hash = new XXH32();
            hash.Update(bytes);
            return hash.DigestAsInt();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DigestAsInt(this XXH32 hash)
        {
            return (int)hash.Digest();
        }
    }
}
