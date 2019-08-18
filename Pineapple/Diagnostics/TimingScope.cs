using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Diagnostics
{
    public class TimingScope : IDisposable
    {
        private readonly bool _isEnabled;
        private readonly string _operationName;
        private readonly ILogger _logger;
        private readonly IDisposable _loggingScope;
        private readonly Stopwatch _sw;
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
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            if (!_isEnabled)
                return;

            SafeMethod(() =>
            {
                _sw.Stop();
                var ts = _sw.Elapsed;
                _loggingScope.Dispose();
                _logger.LogDebug($@"Operation [{_operationName}] called from [{_memberName}] took {ts.TotalMilliseconds:0.00}ms");
            }); 
        }
    }
}
