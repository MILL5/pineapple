using System;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    /// <summary>Limits concurrent operation scopes; this is not a calls-per-minute limiter.</summary>
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class SemaphoreRateLimiterScope : IRateLimiterScope
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationTokenSource _shutdown = new();
        private readonly object _lifetimeLock = new();
        private int _references;
        private bool _disposed;

        private sealed class OperationScope : IRateLimiterScope
        {
            private SemaphoreRateLimiterScope _owner;
            public OperationScope(SemaphoreRateLimiterScope owner) => _owner = owner;
            public void Dispose() => Interlocked.Exchange(ref _owner, null)?.Release();
            public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
        }

        public SemaphoreRateLimiterScope(int maxCalls)
        {
            CheckIsNotLessThan(nameof(maxCalls), maxCalls, 1);
            _semaphore = new SemaphoreSlim(maxCalls, maxCalls);
        }

        public IRateLimiterScope GetOperationScope()
        {
            lock (_lifetimeLock)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                _references++;
            }
            try { _semaphore.Wait(_shutdown.Token); }
            catch (OperationCanceledException)
            {
                ReleaseReference();
                throw new ObjectDisposedException(nameof(SemaphoreRateLimiterScope));
            }
            catch { ReleaseReference(); throw; }
            lock (_lifetimeLock)
            {
                if (!_disposed) return new OperationScope(this);
            }
            Release();
            throw new ObjectDisposedException(nameof(SemaphoreRateLimiterScope));
        }

        private void Release()
        {
            _semaphore.Release();
            ReleaseReference();
        }

        private void ReleaseReference()
        {
            lock (_lifetimeLock)
            {
                _references--;
                DisposeIfUnused();
            }
        }

        private void DisposeIfUnused()
        {
            if (_disposed && _references == 0)
            {
                _semaphore.Dispose();
                _shutdown.Dispose();
            }
        }

        public void Dispose()
        {
            lock (_lifetimeLock)
            {
                if (_disposed) return;
                _disposed = true;
                _shutdown.Cancel();
                DisposeIfUnused();
            }
        }
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }
}
