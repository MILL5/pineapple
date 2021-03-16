using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class SafeRateLimiterScope : IRateLimiterScope
    {
        private class State
        {
            public string Token { get; set; }
            public int CallsPerMinute { get; set; }
            public int Count { get; set; }
            public object CallLock { get; } = new object();

            public int msPerCall
            {
                get
                {
                    return Convert.ToInt32(Math.Ceiling(60000.0 / CallsPerMinute));
                }
            }
        }

        private static readonly ConcurrentDictionary<string, State> _states = new();
        private readonly Stopwatch _sw;
        private readonly State _state;
        private bool _disposed;

        public SafeRateLimiterScope(int callsPerMinute, [CallerMemberName] string token = null)
        {
            CheckIsNotNullOrWhitespace(nameof(token), token);
            CheckIsNotLessThan(nameof(callsPerMinute), callsPerMinute, 1);

            _state = _states.GetOrAdd(token, (k) =>
            {
                return new State { Token = token, CallsPerMinute = callsPerMinute };
            });

            _sw = new Stopwatch();
            _sw.Start();

            Monitor.Enter(_state.CallLock);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _sw.Stop();
            var diff = Convert.ToInt32(_state.msPerCall - _sw.ElapsedMilliseconds);

            if (diff > 0)
            {
                Task.Delay(diff).Wait();
            }

            Monitor.Exit(_state.CallLock);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            _sw.Stop();
            var diff = Convert.ToInt32(_state.msPerCall - _sw.ElapsedMilliseconds);

            if (diff > 0)
            {
                await Task.Delay(diff);
            }

            Monitor.Exit(_state.CallLock);
        }
    }
}
