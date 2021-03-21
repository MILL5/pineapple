﻿using System;

namespace Pineapple.Threading
{
    public interface IResourceGoverner : IDisposable, IAsyncDisposable
    {
        TimeSpan AverageTime { get; }
        long TotalCount { get; }
        TimeSpan TotalTime { get; }

        IRateLimiterScope GetOperationScope();
    }
}