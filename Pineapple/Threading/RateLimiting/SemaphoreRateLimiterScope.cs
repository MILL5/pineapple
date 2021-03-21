using System;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Threading
{
    public class SemaphoreRateLimiterScope : IRateLimiterScope
    {
        private readonly int _maxcalls;
        private readonly SemaphoreSlim _s;

        private class OperationScope : IRateLimiterScope
        {
            public OperationScope(SemaphoreSlim s)
            {
                s.Wait();
            }

            public void Dispose()
            {
            }

            public async ValueTask DisposeAsync()
            {
                await Task.CompletedTask;
            }
        }

        public SemaphoreRateLimiterScope(int maxCalls)
        {
            CheckIsNotLessThan(nameof(maxCalls), maxCalls, 1);

            _maxcalls = maxCalls;
            _s = new SemaphoreSlim(0, _maxcalls);
        }

        public IRateLimiterScope GetOperationScope()
        {
            return new OperationScope(_s);
        }

        public void Dispose()
        {
            SafeMethod(_s.Dispose);
        }

        public async ValueTask DisposeAsync()
        {
            SafeMethod(_s.Dispose);
            await Task.CompletedTask;
        }
    }
}