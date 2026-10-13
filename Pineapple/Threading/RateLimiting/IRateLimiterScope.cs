using System;
using System.Collections.Generic;
using System.Text;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public interface IRateLimiterScope :  IDisposable, IAsyncDisposable
    {
    }
}
