using System;
using System.Threading.Tasks;

namespace Pineapple.Threading
{
    public interface IResourceGoverner : IDisposable, IAsyncDisposable
    {
        TimeSpan AverageTime { get; }
        long TotalCount { get; }
        TimeSpan TotalTime { get; }
        double CallsPerMinute { get; }

        Task<IRateLimiterScope> GetOperationScopeAsync();
    }
}