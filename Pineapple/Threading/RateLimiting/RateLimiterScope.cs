using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Threading.RateLimiting (SlidingWindowRateLimiter, TokenBucketRateLimiter, ConcurrencyLimiter) or Microsoft.Extensions.Http.Resilience.", DiagnosticId = "PINE002", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class RateLimiterScope : IRateLimiterScope
    {
        private readonly int _callsPerMinute;
        private readonly Stopwatch _sw;
        private readonly long _msPerCall;

        public RateLimiterScope(int callsPerMinute)
        {
            CheckIsNotLessThan(nameof(callsPerMinute), callsPerMinute, 1);

            _callsPerMinute = callsPerMinute;
            _msPerCall = Convert.ToInt32(Math.Ceiling(60000.0 / _callsPerMinute));
            _sw = new Stopwatch();
            _sw.Start();
        }

        public void Dispose()
        {
            _sw.Stop();
            var diff = Convert.ToInt32(_msPerCall - _sw.ElapsedMilliseconds);

            if (diff > 0)
            {
                Task.Delay(diff).Wait();
            }
        }

        public async ValueTask DisposeAsync()
        {
            _sw.Stop();
            var diff = Convert.ToInt32(_msPerCall - _sw.ElapsedMilliseconds);

            if (diff > 0)
            {
                await Task.Delay(diff);
            }
        }
    }
}
