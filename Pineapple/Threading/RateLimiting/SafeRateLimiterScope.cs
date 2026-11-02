using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class SafeRateLimiterScope : IRateLimiterScope
    {
        private sealed class State
        {
            public int CallsPerMinute { get; init; }
            public int References { get; set; }
            public SemaphoreSlim Gate { get; } = new(1, 1);
        }

        private static readonly Dictionary<string, State> States = new();
        private static readonly object RegistryLock = new();
        private readonly string _token;
        private readonly State _state;
        private readonly Stopwatch _watch;
        private int _disposed;

        public SafeRateLimiterScope(int callsPerMinute, [CallerMemberName] string token = null)
        {
            CheckIsNotNullOrWhitespace(nameof(token), token);
            CheckIsNotLessThan(nameof(callsPerMinute), callsPerMinute, 1);
            _token = token;
            lock (RegistryLock)
            {
                if (!States.TryGetValue(token, out _state))
                {
                    _state = new State { CallsPerMinute = callsPerMinute };
                    States.Add(token, _state);
                }
                else if (_state.CallsPerMinute != callsPerMinute)
                {
                    throw new ArgumentException("Active scopes sharing a token must use the same rate.", nameof(callsPerMinute));
                }
                _state.References++;
            }
            _state.Gate.Wait();
            _watch = Stopwatch.StartNew();
        }

        private int RemainingMilliseconds => Math.Max(0,
            (int)Math.Ceiling(60000.0 / _state.CallsPerMinute - _watch.Elapsed.TotalMilliseconds));

        private void Release()
        {
            _state.Gate.Release();
            lock (RegistryLock)
            {
                if (--_state.References == 0)
                {
                    States.Remove(_token);
                    _state.Gate.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
            try { Thread.Sleep(RemainingMilliseconds); }
            finally { Release(); }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
            try { await Task.Delay(RemainingMilliseconds).ConfigureAwait(false); }
            finally { Release(); }
        }
    }
}
