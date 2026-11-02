using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class ResourceGoverner : BaseResourceGoverner
    {
        protected const int DEFAULT_TIMEOUT_SECS = 10;
        protected readonly int _maxCallsPerMinute;
        protected readonly int _timeOutMs;
        private readonly long _intervalTicks;
        private long _nextStart;

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);
            _maxCallsPerMinute = maxCallsPerMinute;
            _timeOutMs = Math.Max(DEFAULT_TIMEOUT_SECS,
                (int)Math.Ceiling(60.0 / maxCallsPerMinute) * 2) * 1000;
            _intervalTicks = GetInterval(maxCallsPerMinute, Stopwatch.Frequency);
        }

        internal static long GetInterval(int maxCallsPerMinute, long frequency) =>
            (frequency * 60 - 1) / maxCallsPerMinute + 1;

        public override async Task<IRateLimiterScope> GetOperationScopeAsync()
        {
            using var timeout = new AccurateTimeout(_timeOutMs);
            while (true)
            {
                int delayMilliseconds;
                lock (_operations.OperationLock)
                {
                    _operations.EnsureNotDisposed();
                    var now = Stopwatch.GetTimestamp();
                    if (_nextStart == 0) _nextStart = now;
                    if (now >= _nextStart)
                    {
                        var operation = new Operation(_operations);
                        _nextStart = Stopwatch.GetTimestamp() + _intervalTicks;
                        _cpm.Add(operation.Start);
                        return operation;
                    }
                    delayMilliseconds = Math.Max(1, (int)Math.Ceiling(
                        (_nextStart - now) * 1000.0 / Stopwatch.Frequency));
                }
                timeout.EnsureNotTimedOut();
                await Task.Delay((int)Math.Min(delayMilliseconds,
                    Math.Max(1, timeout.RemainingInMilliseconds))).ConfigureAwait(false);
            }
        }
    }
}
