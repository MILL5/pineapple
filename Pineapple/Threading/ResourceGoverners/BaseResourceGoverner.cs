using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public abstract class BaseResourceGoverner : IResourceGoverner
    {
        internal readonly Operations _operations = new();
        internal readonly CallsPerMinute _cpm = new();

        public void Dispose()
        {
            _operations.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _operations.DisposeAsync();
        }

        public abstract Task<IRateLimiterScope> GetOperationScopeAsync();

        public TimeSpan AverageTime => TimeSpan.FromMilliseconds(TotalTime.TotalMilliseconds / TotalCount);
        public TimeSpan TotalTime => _operations.TotalTime;
        public long TotalCount => _operations.TotalCount;
        public double CallsPerMinute => _cpm.Value;
    }
}