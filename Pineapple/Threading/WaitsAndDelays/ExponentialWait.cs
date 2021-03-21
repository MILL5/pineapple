using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class ExponentialWait
    {
        private readonly int _initialMs;
        private int _count = 0;
        private int _growth;
        private int? _maxDelay;

        public ExponentialWait(int initialMs, int growth = 1, int? maxDelay = null)
        {
            CheckIsNotLessThan(nameof(initialMs), initialMs, 1);
            CheckIsNotLessThan(nameof(growth), growth, 1);

            _initialMs = initialMs;
            _growth = growth;
            _maxDelay = maxDelay;
        }

        private int Value
        {
            get
            {
                var current = Interlocked.Increment(ref _count);

                int calcDelay = Convert.ToInt32(_initialMs * Math.Pow(1 + current, _growth));

                return _maxDelay.HasValue ? Math.Min(calcDelay, _maxDelay.Value) : calcDelay;
            }
        }

        public void Wait()
        {
            Thread.Sleep(Value);
        }

        public async Task WaitAsync()
        {
            await Task.Delay(Value);
        }
    }
}
