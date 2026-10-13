using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public interface IResourceGoverner : IDisposable, IAsyncDisposable
    {
        TimeSpan AverageTime { get; }
        long TotalCount { get; }
        TimeSpan TotalTime { get; }
        double CallsPerMinute { get; }

        Task<IRateLimiterScope> GetOperationScopeAsync();
    }
}