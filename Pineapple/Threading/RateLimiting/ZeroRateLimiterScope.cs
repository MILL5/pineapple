using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class ZeroRateLimiterScope : IRateLimiterScope
    {
        public ZeroRateLimiterScope()
        {
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
