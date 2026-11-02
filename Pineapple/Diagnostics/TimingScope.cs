using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Diagnostics
{
    public partial class TimingScope : IDisposable
    {
        private readonly bool _isEnabled;
        private readonly string _operationName;
        private readonly ILogger _logger;
        private readonly IDisposable _loggingScope;
        private readonly long _startTimestamp;
        private readonly string _memberName;

        public TimingScope(string operationName, ILogger logger, [CallerMemberName] string memberName = "")
        {
            CheckIsNotNullOrWhitespace(nameof(operationName), operationName);
            CheckIsNotNull(nameof(logger), logger);
            CheckIsNotNullOrWhitespace(nameof(memberName), memberName);

            _isEnabled = logger.IsEnabled(LogLevel.Debug);

            if (!_isEnabled)
                return;

            _operationName = operationName;
            _logger = logger;
            _loggingScope = _logger.BeginScope($"Caller=[{memberName}]");

            _memberName = memberName;
            _startTimestamp = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            if (!_isEnabled)
                return;

            SafeMethod(() =>
            {
                var elapsed = Stopwatch.GetElapsedTime(_startTimestamp);
                _loggingScope?.Dispose();
                LogOperationTiming(_logger, _operationName, _memberName, elapsed.TotalMilliseconds);
            });
        }

        // Source-generated, structured log: OperationName, MemberName and ElapsedMilliseconds are queryable fields.
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Operation [{OperationName}] called from [{MemberName}] took {ElapsedMilliseconds:0.00}ms")]
        private static partial void LogOperationTiming(ILogger logger, string operationName, string memberName, double elapsedMilliseconds);
    }
}
