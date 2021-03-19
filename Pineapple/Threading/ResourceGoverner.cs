using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
using System.Linq;

namespace Pineapple.Threading
{
    public class ResourceGoverner : IResourceGoverner
    {
        private const int DEFAULT_TIMEOUT_SECS = 10;

        private readonly int _maxCallsPerMinute;
        private readonly int _timeOutMs;
        private readonly List<OperationScope> _callsInFlight = new List<OperationScope>();
        private readonly object _calllock = new List<OperationScope>();

        private long _count;

        private enum OperationStatus
        {
            Unknown = 0,
            Running = 1,
            Complete = 2
        }

        private class OperationScope : IRateLimiterScope
        {
            private readonly List<OperationScope> _operations;
            private readonly object _calllock;
            private readonly DateTime _start;
            private OperationStatus _status = OperationStatus.Running;

            public OperationScope(List<OperationScope> operations, object calllock, int maxCallsPerMinute, int timeoutInMs)
            {
                _operations = operations;
                _calllock = calllock;

                bool wait = true;

                DateTime timeoutStart = DateTime.Now;

                while (wait)
                {
                    wait = false;

                    lock (_calllock)
                    {
                        _start = DateTime.Now;
                        var expiration = DateTime.Now.AddMinutes(-1.0);
                        var count = _operations.Where(x => x.Start > expiration).Count();

                        if (count > 0)
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

                        if (elapsed > (timeoutInMs + 100))
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
                        _status = OperationStatus.Complete;
                        var expiration = DateTime.Now.AddMinutes(-1.0);
                        var expiredOperations = _operations.Where(x => x._status == OperationStatus.Complete &&
                                                                        x.Start <= expiration).ToList();
                        foreach (var operation in expiredOperations)
                        {
                            _operations.Remove(operation);
                        }
                    }
                });
            }

            public async ValueTask DisposeAsync()
            {
                SafeMethod(() =>
                {
                    lock (_calllock)
                    {
                        _status = OperationStatus.Complete;
                        var expiration = DateTime.Now.AddMinutes(-1.0);
                        var expiredOperations = _operations.Where(x => x._status == OperationStatus.Complete &&
                                                                        x.Start <= expiration).ToList();
                        foreach (var operation in expiredOperations)
                        {
                            _operations.Remove(operation);
                        }
                    }
                });

                await Task.CompletedTask;
            }
        }

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;

            int adjustedTimeout = Convert.ToInt32(Math.Ceiling(60.0 / maxCallsPerMinute));
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