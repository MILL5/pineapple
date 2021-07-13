using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Threading
{
    public interface ITimeout : IDisposable, IAsyncDisposable
    {
        bool HasExpired { get; }
        void EnsureNotTimedOut();
        long ElapsedInMilliseconds { get; }
        long RemainingInMilliseconds { get; }
    }

    public class AccurateTimeout : ITimeout
    {
        private readonly Stopwatch _sw;
        private readonly int _timeoutInMs;

        public AccurateTimeout(int timeoutInMs)
        {
            CheckIsNotLessThan(nameof(timeoutInMs), timeoutInMs, 1);

            _timeoutInMs = timeoutInMs;

            _sw = new Stopwatch();
            _sw.Start();
        }

        ~AccurateTimeout()
        {
            DisposeInternal();
        }

        public long RemainingInMilliseconds
        {
            get
            {
                return _timeoutInMs - _sw.ElapsedMilliseconds;
            }
        }

        public long ElapsedInMilliseconds
        {
            get
            {
                return _sw.ElapsedMilliseconds;
            }
        }

        public bool HasExpired
        {
            get
            {
                bool expired = _sw.ElapsedMilliseconds > _timeoutInMs;

                if (expired)
                {
                    _sw.Stop();
                }

                return expired;
            }
        }

        public void EnsureNotTimedOut()
        {
            if (HasExpired)
                throw new TimeoutException($"Elapsed time exceeded {_timeoutInMs}ms");
        }

        protected virtual void DisposeInternal()
        {
            SafeMethod(() => _sw.Stop());
        }

        public void Dispose()
        {
            DisposeInternal();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }
    }

    public class SimpleTimeout : ITimeout
    {
        private readonly int _timeoutInMs;
        private readonly DateTime _now;
        private readonly DateTime _expiration;

        public SimpleTimeout(int timeoutInMs)
        {
            CheckIsNotLessThan(nameof(timeoutInMs), timeoutInMs, 1);

            _timeoutInMs = timeoutInMs;
            _now = DateTime.Now;
            _expiration = _now.AddMilliseconds(timeoutInMs);
        }

        private DateTime GetNow()
        {
            var n = DateTime.Now;
            return n.AddMilliseconds(-n.Millisecond);
        }

        public long RemainingInMilliseconds
        {
            get
            {
                var remaining = _timeoutInMs - ElapsedInMilliseconds;
                return remaining >= 0 ? remaining : 0;
            }
        }

        public long ElapsedInMilliseconds
        {
            get
            {
                return Convert.ToInt64(Math.Floor((GetNow() - _now).TotalMilliseconds));
            }
        }

        public bool HasExpired
        {
            get
            {
                return GetNow() > _expiration;
            }
        }

        public void EnsureNotTimedOut()
        {
            if (HasExpired)
                throw new TimeoutException($"Elapsed time exceeded {_timeoutInMs}ms");
        }

        public void Dispose()
        {
        }

        public async ValueTask DisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
