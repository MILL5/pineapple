using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
using System.Linq;
using System.Diagnostics;

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

        private enum InitState
        {
            Unknown = 0,
            Waiting = 1,
            Timeout = 2,
            Continue = 3
        }

        private class OperationScope : IRateLimiterScope
        {
            private readonly List<OperationScope> _operations;
            private readonly object _calllock;
            private readonly DateTime _start;
            private DateTime? _completed = null;

            private OperationStatus _status = OperationStatus.Running;
            
            /// <summary>
            /// DateTime is not precise and cannot be easily used for timeouts
            /// 
            /// GetNotWithoutMs strips the milliseconds from now. It it okay
            /// to use this version of now in comparisons such as Now > Expiration.
            /// </summary>
            /// <returns>Now without Milliseconds</returns>
            private DateTime GetNowWithoutMs()
            {
                var n = DateTime.Now;
                return n.AddMilliseconds(-n.Millisecond);
            }

            /// <summary>
            /// DateTime is not precise and cannot be easily used for timeouts
            /// 
            /// GetExpiration strips the milliseconds from now and adds 1 second.
            /// It it okay to use this version of now in comparisons such as
            /// Now > Expiration.
            /// </summary>
            /// <returns>Now without Milliseconds</returns>
            private DateTime GetExpiration(int timeoutInMs)
            {
                var e = DateTime.Now.AddMilliseconds(timeoutInMs);
                return e.AddSeconds(1).AddMilliseconds(-e.Millisecond);
            }

            public OperationScope(List<OperationScope> operations, object calllock, int maxCallsPerMinute, int timeoutInMs)
            {
                _operations = operations;
                _calllock = calllock;

                var lookbackExpiration = DateTime.Now.AddMinutes(-1);
                var timeoutExpiration = GetExpiration(timeoutInMs);

                var initState = InitState.Waiting;

                while (initState == InitState.Waiting)
                {
                    lock (_calllock)
                    {
                        var lookback = _operations.Where(x => x.Start > lookbackExpiration).ToList();
                        var count = lookback.Count();

                        if (count > 0)
                        {
                            var oldest = lookback.Min(x => x.Start);

                            var now = GetNowWithoutMs();
                            var diff = (now - timeoutExpiration).TotalMilliseconds;

                            if (diff > 0)
                            {
                                initState = InitState.Timeout;
                            }
                            else
                            {
                                var elapsedMins = Convert.ToDecimal((DateTime.Now - oldest).TotalMinutes);

                                if (elapsedMins > 0)
                                {
                                    var totalOpsPerMinute = count * 1.0m / elapsedMins;

                                    initState = (totalOpsPerMinute > maxCallsPerMinute) ?
                                        InitState.Waiting : InitState.Continue;

                                    if (initState == InitState.Continue)
                                        Debug.WriteLine("");
                                }
                                else
                                {
                                    initState = InitState.Waiting;
                                }
                            }
                        }
                        else
                        {
                            initState = InitState.Continue;
                        }
                    }

                    if (initState == InitState.Timeout)
                    {
                        throw new TimeoutException();
                    }
                    else if (initState == InitState.Waiting)
                    {
                        Thread.Sleep(1);
                    }
                    else if (initState == InitState.Continue)
                    {
                        _start = DateTime.Now;
                        operations.Add(this);
                    }
                }
            }

            public DateTime Start => _start;
            public DateTime? Completed => _completed;

            public void Dispose()
            {
                SafeMethod(() =>
                {
                    lock (_calllock)
                    {
                        _status = OperationStatus.Complete;
                        _completed = DateTime.Now;

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
                        _completed = DateTime.Now;
                        
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