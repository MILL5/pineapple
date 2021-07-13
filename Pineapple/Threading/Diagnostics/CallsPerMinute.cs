using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Pineapple.Threading
{
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
            lock (_statsLock)
            {
                _calllog.Add(DateTime.Now);

                var nonExpired = _calllog.Where(x => x >= DateTime.Now.AddMinutes(-1));
                var count = nonExpired.Count();

                if (count > 1)
                {
                    var elapsed = DateTime.Now - nonExpired.Min();
                    _cpm = count / elapsed.TotalMinutes;
                }
                else
                {
                    _cpm = 1;
                }

                var removeThese = _calllog.Where(x => x < DateTime.Now.AddMinutes(-1)).ToList();

                foreach (var o in removeThese)
                {
                    _calllog.Remove(o);
                }
            }
        }

        public double Value => _cpm;
    }
}
