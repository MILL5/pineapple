using System;
using System.Diagnostics;
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

        // Scheduling uses the monotonic Stopwatch clock; wall-clock adjustments must not shift slots.
        private readonly long _interval;
        private long? _nextAllowed;

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;
            _timeOutMs = GetAdjustedTimeout(maxCallsPerMinute);
            _interval = GetInterval(maxCallsPerMinute, Stopwatch.Frequency);
        }

        // Round up so even limits above the clock's resolution retain a nonzero interval.
        internal static long GetInterval(int maxCallsPerMinute, long frequency) =>
            (frequency * 60 - 1) / maxCallsPerMinute + 1;

        // Timer overshoot may reduce throughput, but waiting never occupies a CPU core.
        private static Task WaitAsync(TimeSpan wait) =>
            Task.Delay(TimeSpan.FromMilliseconds(Math.Max(1, Math.Ceiling(wait.TotalMilliseconds))));

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
                    var now = Stopwatch.GetTimestamp();

                    //
                    // The first call is immediate; subsequent calls are at least one rounded-up
                    // interval apart. Idle time does not accumulate credit for a burst.
                    //
                    if (!_nextAllowed.HasValue || now >= _nextAllowed.Value)
                    {
                        initState = InitState.Continue;

                        //
                        // Schedule from the actual start, never from a missed slot: catching up would let a
                        // caller that runs slower than the limit build up credit and then burst past it.
                        //
                        _nextAllowed = now + _interval;

                        //
                        // Create the operation inside the lock so the next caller sees it.
                        //
                        nextOperation = new Operation(_operations, DateTime.UtcNow);
                        _cpm.Add(nextOperation.Start);
                    }
                    else
                    {
                        wait = Stopwatch.GetElapsedTime(now, _nextAllowed.Value);
                    }

                    Debug.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}]:State:{initState}:Wait:{wait.TotalMilliseconds:0}ms");
                }

                if (initState == InitState.Waiting)
                {
                    //
                    // We only check for timeout if we are waiting
                    //
                    timeout.EnsureNotTimedOut();

                    var remaining = TimeSpan.FromMilliseconds(Math.Max(1, timeout.RemainingInMilliseconds + 1));
                    await WaitAsync(wait > remaining ? remaining : wait).ConfigureAwait(false);
                }
            }

            return nextOperation;
        }
    }
}
