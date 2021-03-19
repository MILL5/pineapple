using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Threading
{
    public class ResourceGoverner : IResourceGoverner
    {
        private const int DEFAULT_TIMEOUT_SECS = 10;

        private readonly int _maxCallsPerMinute;
        private readonly double _timeOutMs;
        private readonly List<OperationScope> _callsInFlight = new List<OperationScope>();
        private readonly object _calllock = new List<OperationScope>();

        private long _count;

        private class OperationScope : IRateLimiterScope
        {
            private readonly List<OperationScope> _operations;
            private readonly object _calllock;
            private readonly DateTime _start;

            public OperationScope(List<OperationScope> operations, object calllock, int maxCallsPerMinute, double timeoutInMs)
            {
                _operations = operations;
                _calllock = calllock;

                bool wait = true;

                DateTime timeoutStart = DateTime.Now;
                DateTime oldest;
                double minutes;
                double currentCallsPerMinute;
                int count;

                while (wait)
                {
                    wait = false;

                    lock (_calllock)
                    {
                        _start = DateTime.Now;
                        count = operations.Count;

                        if (count > 0)
                        {
                            oldest = operations[0].Start;
                            minutes = (_start - oldest).TotalMilliseconds / 60000.0;
                            currentCallsPerMinute = count / minutes;
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

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;

            double adjustedTimeout = 60 / maxCallsPerMinute;
            if (adjustedTimeout > DEFAULT_TIMEOUT_SECS)
            {
                _timeOutMs = adjustedTimeout * 1000;
            }
            else
            {
                _timeOutMs = DEFAULT_TIMEOUT_SECS * 1000;
            }
        }

        public IRateLimiterScope GetOperationScope()
        {
            var o = new OperationScope(_callsInFlight, _calllock, _maxCallsPerMinute, _timeOutMs);
            Interlocked.Increment(ref _count);
            return o;
        }

        public long TotalNumberOfCalls { get { return Interlocked.Read(ref _count); } }
    }
}