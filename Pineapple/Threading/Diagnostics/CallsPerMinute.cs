using System;
using System.Collections.Generic;

namespace Pineapple.Threading
{
    [Obsolete("Use System.Diagnostics.Metrics (Counter<T>, Histogram<T>) and dotnet-counters or OpenTelemetry.", DiagnosticId = "PINE004", UrlFormat = "https://mill5.github.io/pineapple/guides/migration/#{0}")]
    public class CallsPerMinute : ICallsPerMinute
    {
        private readonly List<DateTime> _calllog = new();
        private readonly object _statsLock = new();
        private double _cpm;

        public CallsPerMinute()
        {
        }

        public void Add()
        {
            Add(DateTime.UtcNow);
        }

        // Governors pass the operation's own start time so the statistic and the admission check
        // measure exactly the same events.
        internal void Add(DateTime atUtc)
        {
            lock (_statsLock)
            {
                _calllog.Add(atUtc);

                var windowStart = atUtc.AddMinutes(-1);
                _calllog.RemoveAll(x => x < windowStart);

                var count = _calllog.Count;

                if (count > 1)
                {
                    // n calls span n - 1 intervals; dividing n by the span overstates the rate.
                    var elapsed = _calllog[^1] - _calllog[0];
                    _cpm = (count - 1) / elapsed.TotalMinutes;
                }
                else
                {
                    _cpm = 1;
                }
            }
        }

        public double Value => _cpm;
    }
}
