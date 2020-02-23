using System;
using static Pineapple.Common.Preconditions;
// ReSharper disable UnusedMember.Global

namespace Pineapple.Common
{
    public struct ShortGuid
    {
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        private GuidValue _guidValue;

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
            get => _guidValue.Guid;
            set => _guidValue = new GuidValue(value);
        }

        public string Value
        {
            get => _guidValue.Value;
            set => _guidValue = new GuidValue(value);
        }

        public override string ToString()
        {
            return _guidValue.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ShortGuid guidAsShortGuid)
            {
                if (GetHashCode() != guidAsShortGuid.GetHashCode())
                    return false;

                return _guidValue.Guid.Equals(guidAsShortGuid.Guid);
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
            if (input == null)
                return false;

            const int size = 22;

            if (Guid.TryParse(input, out guid))
            {
                return true;
            }

            if (input.Length == size)
            {
                try
                {
                    guid = Decode(input);
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());

            encoded = encoded
              .Replace("/", "_")
              .Replace("+", "-");

            return encoded.Substring(0, 22);
        }

        private static Guid DecodeShortGuidString(string value)
        {
            if (value.Length != 22)
                throw new ArgumentOutOfRangeException("Value is not the length of a ShortGuid.");

            value = value
              .Replace("_", "/")
              .Replace("-", "+");

            var decodeThis = $@"{value}==";
            byte[] buffer = Convert.FromBase64String(decodeThis);

            return new Guid(buffer);
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