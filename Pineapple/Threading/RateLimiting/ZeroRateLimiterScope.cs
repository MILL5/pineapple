using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
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
