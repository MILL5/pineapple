using System;
using System.Runtime.CompilerServices;
using System.Threading;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Pineapple.Common
{
    public static class Preconditions
    {
        private static void ThrowException(Exception ex)
        {
            throw ex;
        }

        private static void CheckParamName(string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
            {
                var ex = paramName == null
                    ? new ArgumentNullException(nameof(paramName), $"{nameof(paramName)} cannot be null.")
                    : new ArgumentException($"{nameof(paramName)} cannot be empty or whitespace.", nameof(paramName));

                ThrowException(ex);
            }
        }

        public static void CheckNotExpression(string paramName, bool check)
        {
            if (check)
                throw new ArgumentException($"{paramName} is invalid.", paramName);
        }

        public static void CheckIsNotOnSynchronizationContext([CallerMemberName] string memberName = "")
        {
            var context = SynchronizationContext.Current;

            if (context != null)
                throw new Exception($@"{memberName} is running on a synchronization context.");
        }

        public static void CheckIsNotCondition(string paramName, Func<bool> condition, Func<string> message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (condition())
            {
                var messageToSend = message();

                CheckIsNotNullOrWhitespace(nameof(messageToSend), messageToSend);
                ThrowException(new ArgumentException($"{paramName} {messageToSend}", paramName));
            }
        }

        public static void CheckIsNotCondition(string paramName, bool condition, Func<string> message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (condition)
            {
                var messageToSend = message();

                CheckIsNotNullOrWhitespace(nameof(messageToSend), messageToSend);
                ThrowException(new ArgumentException($"{paramName} {messageToSend}", paramName));
            }
        }

        public static void CheckIsNotCondition(string paramName, Func<bool> condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNullOrWhitespace(nameof(message), message);

            if (condition())
            {
                ThrowException(new ArgumentException($"{paramName} {message}", paramName));
            }
        }

        public static void CheckIsNotCondition(string paramName, bool condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNullOrWhitespace(nameof(message), message);

            if (condition)
            {
                ThrowException(new ArgumentException($"{paramName} {message}", paramName));
            }
        }

        public static void CheckIsCondition(string paramName, Func<bool> condition, Func<string> message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (!condition())
            {
                var messageToSend = message();

                CheckIsNotNull(nameof(messageToSend), messageToSend);
                ThrowException(new ArgumentException($"{paramName} {messageToSend}", paramName));
            }
        }

        public static void CheckIsCondition(string paramName, bool condition, Func<string> message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (!condition)
            {
                var messageToSend = message();

                CheckIsNotNull(nameof(messageToSend), messageToSend);
                ThrowException(new ArgumentException($"{paramName} {messageToSend}", paramName));
            }
        }

        public static void CheckIsCondition(string paramName, Func<bool> condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (!condition())
            {
                CheckIsNotNull(nameof(message), message);
                ThrowException(new ArgumentException($"{paramName} {message}", paramName));
            }
        }

        public static void CheckIsCondition(string paramName, bool condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (!condition)
            {
                CheckIsNotNull(nameof(message), message);
                ThrowException(new ArgumentException($"{paramName} {message}", paramName));
            }
        }

        public static void CheckIsNotNull(string paramName, string value)
        {
            CheckParamName(paramName);

            ArgumentNullException.ThrowIfNull(value, paramName);
        }

        public static void CheckIsNotNull(string paramName, object value)
        {
            CheckParamName(paramName);

            ArgumentNullException.ThrowIfNull(value, paramName);
        }

        public static void CheckIsNotNull<T>(string paramName, object value) where T : Exception, new()
        {
            CheckParamName(paramName);

            if (value == null)
            {
                ThrowException(new T());
            }
        }

        public static T CheckIsType<T>(string paramName, object value) where T : class
        {
            CheckParamName(paramName);

            CheckIsNotNull(paramName, value);

            if (!(value is T))
            {
                ThrowException(new ArgumentException($"{paramName} is not of type [{typeof(T).Name}].", paramName));
            }

            return (T)value;
        }

        public static void CheckIsValidDateRange(string paramName, DateTime value, int plusOrMinusInHours)
        {
            CheckIsNotLessThanOrEqualTo(nameof(plusOrMinusInHours), plusOrMinusInHours, 0);

            var utcNow = DateTime.UtcNow;
            var begin = utcNow.AddHours(-plusOrMinusInHours);
            var end = utcNow.AddHours(plusOrMinusInHours);

            CheckIsValidDateRange(paramName, value, begin, end);
        }

        public static void CheckIsValidDateRange(string paramName, DateTime value, DateTime begin, DateTime end)
        {
            CheckParamName(paramName);

            if (value < begin)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} is outside the valid range [< begin]."));

            if (value > end)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} is outside the valid range [> end]."));
        }

        public static void CheckIsNotNullOrWhitespace(string paramName, string value)
        {
            CheckParamName(paramName);

            ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        }

        public static void CheckIsNotWhitespace(string paramName, string value)
        {
            CheckParamName(paramName);

            if (string.IsNullOrWhiteSpace(value) && value != null)
            {
                ThrowException(new ArgumentNullException(paramName, $"{paramName} cannot be empty or whitespace."));
            }
        }

        public static void CheckIsNotEmptyGuid(string paramName, Guid value)
        {
            CheckParamName(paramName);

            if (value.Equals(Guid.Empty))
                ThrowException(new ArgumentNullException(paramName, $"{paramName} cannot be empty."));
        }

        public static void CheckDoesNotContainSpaces(string paramName, string value)
        {
            CheckParamName(paramName);

            if (value.Contains(" "))
                ThrowException(new ArgumentNullException(paramName, $"{paramName} cannot contain spaces. Value: {value}"));
        }

        public static void CheckEndsWith(string paramName, string endswith, string value)
        {
            CheckParamName(paramName);
            CheckIsNotNullOrWhitespace(nameof(endswith), endswith);

            if (!value.EndsWith(endswith))
                ThrowException(new ArgumentNullException(paramName, $"{paramName} must end with '{endswith}'. Value: {value}"));
        }

        public static void CheckIsNotGreaterThan(string paramName, int value, int greaterThanThis)
        {
            CheckParamName(paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, greaterThanThis, paramName);
        }

        public static void CheckIsNotGreaterThan(string paramName, long value, long greaterThanThis)
        {
            CheckParamName(paramName);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, greaterThanThis, paramName);
        }

		public static void CheckIsNotGreaterThan(string paramName, ushort value, ushort greaterThanThis)
		{
			CheckParamName(paramName);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, greaterThanThis, paramName);
		}

		public static void CheckIsNotGreaterThan(string paramName, uint value, uint greaterThanThis)
		{
			CheckParamName(paramName);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, greaterThanThis, paramName);
		}

		public static void CheckIsNotGreaterThan(string paramName, ulong value, ulong greaterThanThis)
		{
			CheckParamName(paramName);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, greaterThanThis, paramName);
		}

        public static void CheckIsEqualTo(string paramName, int value, int equalToThis)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfNotEqual(value, equalToThis, paramName);
        }

        public static void CheckIsNotEqualTo(string paramName, int value, int equalToThis)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfEqual(value, equalToThis, paramName);
        }


        public static void CheckIsNotLessThan(string paramName, int value, int lessThanThis)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfLessThan(value, lessThanThis, paramName);
        }

        public static void CheckIsNotLessThan(string paramName, long value, long lessThanThis)
        {
            CheckParamName(paramName);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, lessThanThis, paramName);
        }

		public static void CheckIsNotLessThan(string paramName, ushort value, ushort lessThanThis)
		{
			CheckParamName(paramName);

			ArgumentOutOfRangeException.ThrowIfLessThan(value, lessThanThis, paramName);
		}

		public static void CheckIsNotLessThan(string paramName, uint value, uint lessThanThis)
		{
			CheckParamName(paramName);

			ArgumentOutOfRangeException.ThrowIfLessThan(value, lessThanThis, paramName);
		}

        public static void CheckIsNotLessThan(string paramName, decimal value, decimal lessThanThis)
        {
            CheckParamName(paramName);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, lessThanThis, paramName);
        }
		
        public static void CheckIsNotLessThan(string paramName, ulong value, ulong lessThanThis)
		{
			CheckParamName(paramName);
			ArgumentOutOfRangeException.ThrowIfLessThan(value, lessThanThis, paramName);
		}

        public static void CheckIsNotLessThanOrEqualTo(string paramName, decimal value, decimal lessThanOrEqualToThis)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, lessThanOrEqualToThis, paramName);
        }
        
        public static void CheckIsNotLessThanOrEqualTo(string paramName, int value, int lessThanOrEqualToThis)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, lessThanOrEqualToThis, paramName);
        }

        public static void CheckIsNotLessThanOrEqualTo(string paramName, long value, long lessThanOrEqualToThis)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, lessThanOrEqualToThis, paramName);
        }

		public static void CheckIsNotLessThanOrEqualTo(string paramName, ushort value, ushort lessThanOrEqualToThis)
		{
			CheckParamName(paramName);

			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, lessThanOrEqualToThis, paramName);
		}

		public static void CheckIsNotLessThanOrEqualTo(string paramName, uint value, uint lessThanOrEqualToThis)
		{
			CheckParamName(paramName);

			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, lessThanOrEqualToThis, paramName);
		}

		public static void CheckIsNotLessThanOrEqualTo(string paramName, ulong value, ulong lessThanOrEqualToThis)
		{
			CheckParamName(paramName);

			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, lessThanOrEqualToThis, paramName);
		}

		public static void CheckIsNotNegative(string paramName, int value)
        {
            CheckParamName(paramName);

            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
        }

        public static void CheckIsNegative(string paramName, int value)
        {
            CheckParamName(paramName);

            if (value >= 0)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} must be negative. Value: {value}"));
        }

        public static void CheckIsNotNegative(string paramName, long value)
		{
			CheckParamName(paramName);

			ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
		}

        public static void CheckIsNegative(string paramName, long value)
        {
            CheckParamName(paramName);

            if (value >= 0)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} must be negative. Value: {value}"));
        }

        public static void CheckIsWellFormedUri(string paramName, string value, UriKind uriKind = UriKind.Absolute)
        {
            CheckParamName(paramName);

            if (!Uri.IsWellFormedUriString(value, uriKind))
                ThrowException(new ArgumentException($"{paramName} must be well formed. Value: {value}", paramName));
        }

        public static void CheckIsNotWellFormedUri(string paramName, string value, UriKind uriKind = UriKind.Absolute)
        {
            CheckParamName(paramName);

            if (Uri.IsWellFormedUriString(value, uriKind))
                ThrowException(new ArgumentException($"{paramName} must not be a well formed URI. Value: {value}", paramName));
        }
    }
}
