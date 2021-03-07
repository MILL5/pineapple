using K4os.Hash.xxHash;
using System;
using System.ComponentModel;

#nullable enable

namespace Pineapple.Common
{
    public class HashCode
    {
        private readonly XXH32 _hash = new();

        public HashCode()
        {
        }

        public static int ToHashCode<T>(T v1)
        {
            var hash = new XXH32();

            hash.Add(v1);

            return hash.DigestAsInt();
        }

        public static int Combine<T1, T2>(T1 v1, T2 v2)
        {
            var hash = new XXH32();

            hash.Add(v1);
            hash.Add(v2);

            return hash.DigestAsInt();
        }

        public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
        {
            var hash = new XXH32();

            hash.Add(v1);
            hash.Add(v2);
            hash.Add(v3);

            return hash.DigestAsInt();
        }

        public static int Combine<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
        {
            var hash = new XXH32();

            hash.Add(v1);
            hash.Add(v2);
            hash.Add(v3);
            hash.Add(v4);

            return hash.DigestAsInt();
        }

        public static int Combine<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
        {
            var hash = new XXH32();

            hash.Add(v1);
            hash.Add(v2);
            hash.Add(v3);
            hash.Add(v4);
            hash.Add(v5);

            return hash.DigestAsInt();
        }

        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
        {
            var hash = new XXH32();

            hash.Add(v1);
            hash.Add(v2);
            hash.Add(v3);
            hash.Add(v4);
            hash.Add(v5);
            hash.Add(v6);

            return hash.DigestAsInt();
        }

        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
        {
            var hash = new XXH32();

            hash.Add(v1);
            hash.Add(v2);
            hash.Add(v3);
            hash.Add(v4);
            hash.Add(v5);
            hash.Add(v6);
            hash.Add(v7);

            return hash.DigestAsInt();
        }

        public void Add<T>(T value)
        {
            _hash.Add(value == null ? 0 : value);
        }

        public int ToHashCode()
        {
            return _hash.DigestAsInt();
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
