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
                var index = _calllog.BinarySearch(atUtc);
                _calllog.Insert(index < 0 ? ~index : index, atUtc);

                var latest = _calllog[^1];
                var windowStart = latest - DateTime.MinValue < TimeSpan.FromMinutes(1)
                    ? DateTime.MinValue
                    : latest.AddMinutes(-1);
                _calllog.RemoveAll(x => x < windowStart);

                var count = _calllog.Count;

                if (count > 1)
                {
                    // n calls span n - 1 intervals; dividing n by the span overstates the rate.
                    var elapsed = _calllog[^1] - _calllog[0];
                    // Equal timestamps provide no measurable interval.
                    _cpm = elapsed > TimeSpan.Zero ? (count - 1) / elapsed.TotalMinutes : double.NaN;
                }
                else
                {
                    _cpm = 1;
                }
            }
        }

        public double Value
        {
            get
            {
                lock (_statsLock)
                    return _cpm;
            }
        }
    }
}
