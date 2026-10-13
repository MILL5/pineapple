using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{

    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class ResourceGoverner : BaseResourceGoverner
    {
        private enum InitState
        {
            Unknown = 0,
            Waiting = 1,
            Continue = 2
        }

        protected const int DEFAULT_TIMEOUT_SECS = 10;

        protected readonly int _maxCallsPerMinute;
        protected readonly int _timeOutMs;

        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _interval;
        private DateTime? _nextAllowedUtc;
        private DateTime _lastStartUtc;

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;
            _timeOutMs = GetAdjustedTimeout(maxCallsPerMinute);
            _interval = TimeSpan.FromTicks(OneMinute.Ticks / maxCallsPerMinute);
        }

        private static int GetAdjustedTimeout(int maxCallsPerMinute)
        {
            int adjustedTimeout = Convert.ToInt32(Math.Ceiling(60.0 / maxCallsPerMinute)) * 2;
            return Math.Max(adjustedTimeout, DEFAULT_TIMEOUT_SECS) * 1000;
        }

        public override async Task<IRateLimiterScope> GetOperationScopeAsync()
        {
            Operation nextOperation = null;

            var initState = InitState.Waiting;

            var timeout = new AccurateTimeout(_timeOutMs);
            var wait = TimeSpan.Zero;

            while (initState == InitState.Waiting)
            {
                lock (_operations.OperationLock)
                {
                    var now = DateTime.UtcNow;

                    //
                    // Calls are scheduled one interval (60s / maxCallsPerMinute) apart, so no rolling
                    // minute ever holds more than maxCallsPerMinute calls. After a quiet minute the first
                    // call starts immediately and the next waits two intervals, so the first N calls
                    // take N intervals and throughput never exceeds the maximum.
                    //
                    var isIdle = !_nextAllowedUtc.HasValue || now - _lastStartUtc >= OneMinute;

                    if (isIdle || now >= _nextAllowedUtc.Value)
                    {
                        initState = InitState.Continue;

                        _nextAllowedUtc = now + (isIdle ? _interval + _interval : _interval);
                        _lastStartUtc = now;

                        //
                        // Create the operation inside the lock so the next caller sees it.
                        //
                        nextOperation = new Operation(_operations, now);
                    }
                    else
                    {
                        wait = _nextAllowedUtc.Value - now;
                    }

                    Debug.WriteLine($"[{now:hh:mm:ss.fff}]:State:{initState}:Wait:{wait.TotalMilliseconds:0}ms");
                }

                if (initState == InitState.Waiting)
                {
                    //
                    // We only check for timeout if we are waiting
                    //
                    timeout.EnsureNotTimedOut();

                    var remaining = TimeSpan.FromMilliseconds(Math.Max(1, timeout.RemainingInMilliseconds + 1));
                    var minimum = TimeSpan.FromMilliseconds(1);
                    await Task.Delay(wait < minimum ? minimum : wait > remaining ? remaining : wait);
                }
            }

            _cpm.Add(nextOperation.Start);

            return nextOperation;
        }
    }
}