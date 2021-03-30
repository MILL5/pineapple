using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class ResourceGoverner : BaseResourceGoverner
    {
        private enum InitState
        {
            Unknown = 0,
            Waiting = 1,
            Continue = 2
        }

        protected const int DEFAULT_TIMEOUT_SECS = 10;

        protected readonly int _maxCallsPerMinute;
        protected readonly int _timeOutMs;

        public ResourceGoverner(int maxCallsPerMinute)
        {
            CheckIsNotLessThan(nameof(maxCallsPerMinute), maxCallsPerMinute, 1);

            _maxCallsPerMinute = maxCallsPerMinute;
            _timeOutMs = GetAdjustedTimeout(maxCallsPerMinute);
        }

        private static int GetAdjustedTimeout(int maxCallsPerMinute)
        {
            int adjustedTimeout = Convert.ToInt32(Math.Ceiling(60.0 / maxCallsPerMinute)) * 2;
            return Math.Max(adjustedTimeout, DEFAULT_TIMEOUT_SECS) * 1000;
        }

        public override async Task<IRateLimiterScope> GetOperationScopeAsync()
        {
            Operation nextOperation = null;

            var expiration = DateTime.Now.AddMinutes(-1);
            var initState = InitState.Waiting;

            var timeout = new AccurateTimeout(_timeOutMs);
            var delay = new ExponentialWait(1, 3, Math.Min(_timeOutMs, 10000));

            while (initState == InitState.Waiting)
            {
                lock (_operations.OperationLock)
                {
                    //
                    // We lookback at only those operations in the last minute.
                    // Our governing is done on a calls per minute basis,
                    // therefore we look back a minute
                    //
                    var lookback = _operations.GetCurrent(expiration);
                    var count = lookback.Count();

                    initState = InitState.Continue;

                    if (count > 0)
                    {
                        var oldest = lookback.Min(x => x.Start);

                        //
                        // We ount the next operation in the count when calculating
                        // the potentialOpsPerMinute. We never want the potential operations
                        // per minute to exceed the maximum operations per minute.
                        //

                        var difference = DateTime.Now - oldest;
                        var potentialOpsPerMinute = (count + 1) / (difference.TotalSeconds / 60);

                        if (potentialOpsPerMinute > _maxCallsPerMinute)
                            initState = InitState.Waiting;

                        Debug.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}]:State:{initState}:Ops Per Minute:{potentialOpsPerMinute}");
                    }

                    //
                    // We do this here (i.e. inside lock) since this operation
                    // is supposed to start and we want to make sure the next
                    // operation includes this operation in the count when it
                    // does the operations per minute calculation.
                    //
                    if (initState == InitState.Continue)
                    {
                        nextOperation = new Operation(_operations);
                    }
                }

                if (initState == InitState.Waiting)
                {
                    //
                    // We only check for timeout if we are waiting
                    //
                    timeout.EnsureNotTimedOut();
                    await delay.WaitAsync();
                }
            }

            _cpm.Add();

            return nextOperation;
        }
    }
}