using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
    public abstract class BaseResourceGoverner : IResourceGoverner
    {
        internal readonly Operations _operations = new();
        internal readonly CallsPerMinute _cpm = new CallsPerMinute();

        public void Dispose()
        {
            _operations.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _operations.DisposeAsync();
        }

        public abstract IRateLimiterScope GetOperationScope();

        public TimeSpan AverageTime => TimeSpan.FromMilliseconds(TotalTime.TotalMilliseconds / TotalCount);
        public TimeSpan TotalTime => _operations.TotalTime;
        public long TotalCount => _operations.TotalCount;
        public double CallsPerMinute => _cpm.Value;
    }
}