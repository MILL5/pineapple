using System;
using System.Buffers.Text;
using static Pineapple.Common.Preconditions;
// ReSharper disable UnusedMember.Global

namespace Pineapple.Common
{
    public struct ShortGuid
    {
        private static readonly GuidValue EmptyValue = new(Guid.Empty);

        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        private GuidValue _guidValue;

        // default(ShortGuid) has no GuidValue; treat it as Empty instead of throwing NullReferenceException.
        private readonly GuidValue Current => _guidValue ?? EmptyValue;

        private class GuidValue
        {
            private readonly string _shortGuid;
            private readonly Guid _guid;

            public GuidValue(string value)
            {
                bool isLongGuid = Guid.TryParse(value, out _);

                if (isLongGuid)
                {
                    _guid = new Guid(value);
                    _shortGuid = Encode(value);
                }
                else
                {
                    _guid = DecodeShortGuidString(value);
                    _shortGuid = value;
                }
            }

            public GuidValue(Guid value)
            {
                _guid = value;
                _shortGuid = Encode(value);
            }

            public Guid Guid => _guid;

            public string Value => _shortGuid;
        }

        public ShortGuid(string value)
        {
            CheckIsNotNullOrWhitespace(nameof(value), value);
            CheckIsNotCondition(nameof(value), value.Length < 22, () => $"{value} is not a shortguid.");

            _guidValue = new GuidValue(value);
        }

        public ShortGuid(Guid guid)
        {
            _guidValue = new GuidValue(guid);
        }

        public Guid Guid
        {
            get => Current.Guid;
            set => _guidValue = new GuidValue(value);
        }

        public string Value
        {
            get => Current.Value;
            set => _guidValue = new GuidValue(value);
        }

        public override string ToString()
        {
            return Current.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ShortGuid guidAsShortGuid)
            {
                if (GetHashCode() != guidAsShortGuid.GetHashCode())
                    return false;

                return Current.Guid.Equals(guidAsShortGuid.Guid);
            }

            if (obj is Guid guid)
                return Guid.Equals(guid);

            if (obj is string guidAsString)
            {
                if (Guid.TryParse(guidAsString, out var g))
                {
                    return Guid.Equals(g);
                }

                try
                {
                    g = Decode(guidAsString);

                    return Guid.Equals(g);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Guid.GetHashCode();
        }

        public static ShortGuid NewGuid()
        {
            return new ShortGuid(Guid.NewGuid());
        }

        public static string Encode(string value)
        {
            var guid = new Guid(value);
            return Encode(guid);
        }

        public static bool TryParse(string input, out Guid guid)
        {
            guid = Guid.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            const int size = 22;

            if (Guid.TryParse(input, out guid))
            {
                return true;
            }

            if (input.Length == size)
            {
                return TryDecodeShortGuidString(input, out guid);
            }

            return false;
        }

        public static string Encode(Guid guid)
        {
            Span<byte> bytes = stackalloc byte[16];
            guid.TryWriteBytes(bytes);

            return Base64Url.EncodeToString(bytes);
        }

        private static Guid DecodeShortGuidString(string value)
        {
            if (value.Length != 22)
                throw new ArgumentOutOfRangeException("Value is not the length of a ShortGuid.");

            if (!TryDecodeShortGuidString(value, out var guid))
                throw new FormatException($"'{value}' is not a valid ShortGuid.");

            return guid;
        }

        private static bool TryDecodeShortGuidString(string value, out Guid guid)
        {
            guid = Guid.Empty;

            if (value.Length != 22)
                return false;

            // Accept the standard Base64 '+' and '/' as well, as 1.x did.
            Span<char> chars = stackalloc char[22];

            for (var i = 0; i < chars.Length; i++)
            {
                var c = value[i];
                chars[i] = c switch
                {
                    '+' => '-',
                    '/' => '_',
                    _ => c
                };

                if (!(char.IsAsciiLetterOrDigit(chars[i]) || chars[i] is '-' or '_'))
                    return false;
            }

            // 22 characters carry 132 bits; the last 4 are padding. Clear them like Convert.FromBase64String did,
            // because Base64Url rejects non-zero padding bits (e.g. "WorkNotificationOpened").
            const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
            chars[21] = Alphabet[Alphabet.IndexOf(chars[21]) & 0b110000];

            Span<byte> bytes = stackalloc byte[16];

            if (!Base64Url.TryDecodeFromChars(chars, bytes, out var written) || written != 16)
                return false;

            guid = new Guid(bytes);
            return true;
        }

        public static Guid Decode(string value)
        {
            CheckIsNotNullOrWhitespace(nameof(value), value);

            if (Guid.TryParse(value, out var g))
            {
                return g;
            }

            return DecodeShortGuidString(value);
        }

        #region ShortGuid Operators
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            return x.Guid == y.Guid;
        }

        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid.Value;
        }

        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid.Guid;
        }
        #endregion

        #region Guid Operators
        //public static bool operator ==(ShortGuid x, Guid y)
        //{
        //    return x.Guid == y;
        //}

        //public static bool operator !=(ShortGuid x, Guid y)
        //{
        //    return !(x == y);
        //}

        //public static bool operator ==(Guid x, ShortGuid y)
        //{
        //    return x == y.Guid;
        //}

        //public static bool operator !=(Guid x, ShortGuid y)
        //{
        //    return !(x == y);
        //}

        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }
        #endregion

        #region String Operators
        //public static bool operator ==(ShortGuid x, string y)
        //{
        //    bool result = false;

        //    try
        //    {
        //        var g = Decode(y);

        //        return x == g;
        //    }
        //    catch
        //    {
        //    }

        //    return result;
        //}

        //public static bool operator !=(ShortGuid x, string y)
        //{
        //    return !(x == y);
        //}

        //public static bool operator ==(string x, ShortGuid y)
        //{
        //    bool result = false;

        //    try
        //    {
        //        var g = Decode(x);

        //        result = (g == y);
        //    }
        //    catch
        //    {
        //    }

        //    return result;
        //}

        //public static bool operator !=(string x, ShortGuid y)
        //{
        //    return !(x == y);
        //}

        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }
        #endregion
    }
}