using System;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
using System.Threading;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    internal enum OperationStatus
    {
        Unknown = 0,
        Running = 1,
        Complete = 2
    }

    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    internal class Operation : IRateLimiterScope
    {
        private readonly Operations _operations;
        private readonly DateTime _start;
        private DateTime? _completed = null;
        private OperationStatus _status = OperationStatus.Running;

        public Operation(Operations operations)
            : this(operations, DateTime.UtcNow)
        {
        }

        // The governor passes the same timestamp it used for its window check.
        public Operation(Operations operations, DateTime startUtc)
        {
            _start = startUtc;
            CheckIsNotNull(nameof(operations), operations);

            _operations = operations;
            _operations.Add(this);        }

        public OperationStatus Status => _status;
        public DateTime Start => _start;
        public DateTime? Completed => _completed;

        public TimeSpan ElapsedTime
        {
            get
            {
                if (!Completed.HasValue)
                    throw new InvalidOperationException();

                return Completed.Value - Start;
            }
        }

        private void DisposeInternal()
        {
            SafeMethod(() =>
            {
                _status = OperationStatus.Complete;
                _completed = DateTime.UtcNow;
            });
        }

        public void Dispose()
        {
            DisposeInternal();
        }

        public async ValueTask DisposeAsync()
        {
            DisposeInternal();
            await Task.CompletedTask;
        }
    }

}
