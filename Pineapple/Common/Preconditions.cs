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
                    : new ArgumentException(nameof(paramName), $"{nameof(paramName)} cannot be empty or whitespace.");

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
                ThrowException(new ArgumentException($"{paramName} {messageToSend}"));
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
                ThrowException(new ArgumentException($"{paramName} {messageToSend}"));
            }
        }

        public static void CheckIsNotCondition(string paramName, Func<bool> condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNullOrWhitespace(nameof(message), message);

            if (condition())
            {
                ThrowException(new ArgumentException($"{paramName} {message}"));
            }
        }

        public static void CheckIsNotCondition(string paramName, bool condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNullOrWhitespace(nameof(message), message);

            if (condition)
            {
                ThrowException(new ArgumentException($"{paramName} {message}"));
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
                ThrowException(new ArgumentException($"{paramName} {messageToSend}"));
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
                ThrowException(new ArgumentException($"{paramName} {messageToSend}"));
            }
        }

        public static void CheckIsCondition(string paramName, Func<bool> condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (!condition())
            {
                CheckIsNotNull(nameof(message), message);
                ThrowException(new ArgumentException($"{paramName} {message}"));
            }
        }

        public static void CheckIsCondition(string paramName, bool condition, string message)
        {
            CheckParamName(paramName);
            CheckIsNotNull(nameof(message), message);

            if (!condition)
            {
                CheckIsNotNull(nameof(message), message);
                ThrowException(new ArgumentException($"{paramName} {message}"));
            }
        }

        public static void CheckIsNotNull(string paramName, string value)
        {
            CheckParamName(paramName);

            if (value == null)
            {
                ThrowException(new ArgumentNullException($"{paramName} cannot be null."));
            }
        }

        public static void CheckIsNotNull(string paramName, object value)
        {
            CheckParamName(paramName);

            if (value == null)
            {
                ThrowException(new ArgumentNullException($"{paramName} cannot be null."));
            }
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
                ThrowException(new ArgumentException(paramName, $"{paramName} is not of type [{typeof(T).Name}]."));
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

            if (string.IsNullOrWhiteSpace(value))
            {
                ThrowException(value == null
                    ? new ArgumentNullException(paramName, $@"{paramName} cannot be null.")
                    : new ArgumentException(paramName, $"{paramName} cannot be empty or whitespace."));
            }
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
            if (value > greaterThanThis)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be greater than {greaterThanThis}. Value: {value}"));
        }

        public static void CheckIsNotGreaterThan(string paramName, long value, long greaterThanThis)
        {
            CheckParamName(paramName);
            if (value > greaterThanThis)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be greater than {greaterThanThis}. Value: {value}"));
        }

        public static void CheckIsNotLessThan(string paramName, int value, int lessThanThis)
        {
            CheckParamName(paramName);

            if (value < lessThanThis)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be less than {lessThanThis}. Value: {value}"));
        }

        public static void CheckIsNotLessThan(string paramName, long value, long lessThanThis)
        {
            CheckParamName(paramName);
            if (value < lessThanThis)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be less than {lessThanThis}. Value: {value}"));
        }

        public static void CheckIsNotLessThanOrEqualTo(string paramName, int value, int lessThanOrEqualToThis)
        {
            CheckParamName(paramName);

            if (value <= lessThanOrEqualToThis)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be less than or equal to {lessThanOrEqualToThis}. Value: {value}"));
        }

        public static void CheckIsNotLessThanOrEqualTo(string paramName, long value, long lessThanOrEqualToThis)
        {
            CheckParamName(paramName);

            if (value <= lessThanOrEqualToThis)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be less than or equal to {lessThanOrEqualToThis}. Value: {value}"));
        }

        public static void CheckIsNotNegative(string paramName, int value)
        {
            CheckParamName(paramName);

            if (value < 0)
                ThrowException(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be negative. Value: {value}"));
        }

        public static void CheckIsWellFormedUri(string paramName, string value, UriKind uriKind = UriKind.Absolute)
        {
            CheckParamName(paramName);

            if (!Uri.IsWellFormedUriString(value, uriKind))
                ThrowException(new ArgumentException($"{paramName} must be well formed. Value: {value}", paramName));
        }
    }
}
