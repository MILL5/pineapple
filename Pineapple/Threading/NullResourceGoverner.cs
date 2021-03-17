using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Threading
{
    public class NullResourceGoverner : IResourceGoverner
    {
        private readonly List<OperationScope> _callsInFlight = new();
        private readonly object _calllock = new List<OperationScope>();

        private long _count;

        private class OperationScope : IRateLimiterScope
        {
            private readonly List<OperationScope> _operations;
            private readonly object _calllock;
            private readonly DateTime _start;

            public OperationScope(List<OperationScope> operations, object calllock)
            {
                _operations = operations;
                _calllock = calllock;
                _start = DateTime.Now;

                lock(_calllock)
                {
                    _operations.Add(this);
                }
            }

            public DateTime Start => _start;

            public void Dispose()
            {
                SafeMethod(() =>
                {
                    lock (_calllock)
                    {
                        _operations.Remove(this);
                    }
                });
            }

            public async ValueTask DisposeAsync()
            {
                SafeMethod(() =>
                {
                    lock (_calllock)
                    {
                        _operations.Remove(this);
                    }
                });

                await Task.CompletedTask;
            }
        }

        public long TotalNumberOfCalls => Interlocked.Read(ref _count);

        public IRateLimiterScope GetOperationScope()
        {
            var o = new OperationScope(_callsInFlight, _calllock);
            Interlocked.Increment(ref _count);
            return o;
        }
    }
}
