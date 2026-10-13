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

        // Scheduling uses the monotonic Stopwatch clock; wall-clock adjustments must not shift slots.
        private static readonly long OneMinute = Stopwatch.Frequency * 60;
        private readonly long _interval;
        private long? _nextAllowed;
        private long _lastStart;

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;
            _timeOutMs = GetAdjustedTimeout(maxCallsPerMinute);
            _interval = OneMinute / maxCallsPerMinute;
        }

        //
        // Timers can fire up to ~16ms late (Windows timer resolution), which matters when slots are
        // only tens of milliseconds apart. Sleep until just before the slot, then yield until it opens.
        //
        private static readonly TimeSpan TimerResolution = TimeSpan.FromMilliseconds(16);

        private static async Task WaitAsync(TimeSpan wait)
        {
            var target = Stopwatch.GetTimestamp() + (long)(wait.TotalSeconds * Stopwatch.Frequency);

            if (wait > TimerResolution)
                await Task.Delay(wait - TimerResolution).ConfigureAwait(false);

            while (Stopwatch.GetTimestamp() < target)
                Thread.Yield();
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
                    var now = Stopwatch.GetTimestamp();

                    //
                    // Calls are scheduled one interval (60s / maxCallsPerMinute) apart, so no rolling
                    // minute ever holds more than maxCallsPerMinute calls. After a quiet minute the first
                    // call starts immediately and the next waits two intervals, so the first N calls
                    // take N intervals and throughput never exceeds the maximum.
                    //
                    var isIdle = !_nextAllowed.HasValue || now - _lastStart >= OneMinute;

                    if (isIdle || now >= _nextAllowed.Value)
                    {
                        initState = InitState.Continue;

                        //
                        // Schedule from the actual start, never from a missed slot: catching up would let a
                        // caller that runs slower than the limit build up credit and then burst past it.
                        //
                        _nextAllowed = now + (isIdle ? _interval + _interval : _interval);
                        _lastStart = now;

                        //
                        // Create the operation inside the lock so the next caller sees it.
                        //
                        nextOperation = new Operation(_operations, DateTime.UtcNow);
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
                    await WaitAsync(wait > remaining ? remaining : wait);
                }
            }

            _cpm.Add(nextOperation.Start);

            return nextOperation;
        }
    }
}