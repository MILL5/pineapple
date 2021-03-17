using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
using System.Diagnostics;
using System.Numerics;

namespace Pineapple.Threading
{
    public class ResourceGovernerScope : IRateLimiterScope
    {
        private readonly int _maxCallsPerMinute;
        private readonly List<OperationScope> _callsInFlight = new List<OperationScope>();
        private readonly object _calllock = new List<OperationScope>();

        private long _count;

        private class OperationScope : IRateLimiterScope
        {
            private readonly List<OperationScope> _operations;
            private readonly object _calllock;
            private readonly DateTime _start;

            public OperationScope(List<OperationScope> operations, object calllock, int maxCallsPerMinute)
            {
                _operations = operations;
                _calllock = calllock;

                bool wait = true;

                const int timeoutInMs = 10000;
                DateTime timeoutStart = DateTime.Now;

                while (wait)
                {
                    wait = false;

                    lock (_calllock)
                    {
                        _start = DateTime.Now;
                        var count = operations.Count;

                        if (operations.Count > 0)
                        {
                            var oldest = operations[0].Start;
                            var minutes = (_start - oldest).TotalMilliseconds / 60000.0;
                            var currentCallsPerMinute = (count + 1) / minutes;
                            wait = currentCallsPerMinute > maxCallsPerMinute;
                        }
                    }

                    if (wait)
                    {
                        var elapsed = (DateTime.Now - timeoutStart).TotalMilliseconds;

                        if (elapsed > timeoutInMs)
                            throw new TimeoutException();

                        Thread.Sleep(1);
                    }
                    else
                    {
                        operations.Add(this);
                    }
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

        public ResourceGovernerScope(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;
        }

        public void Dispose()
        {
        }

        public IRateLimiterScope GetOperationScope()
        {
            var o = new OperationScope(_callsInFlight, _calllock, _maxCallsPerMinute);
            Interlocked.Increment(ref _count);
            return o;
        }

        public long TotalNumberOfCalls { get { return Interlocked.Read(ref _count); } }

        public async ValueTask DisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
