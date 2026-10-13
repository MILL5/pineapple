using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class NullResourceGoverner : BaseResourceGoverner
    {
        public override async Task<IRateLimiterScope> GetOperationScopeAsync()
        {
            Operation nextOperation = null;

            lock (_operations.OperationLock)
            {
                nextOperation = new Operation(_operations);
            }

            _cpm.Add();

            await Task.CompletedTask;

            return nextOperation;
        }
    }
}
