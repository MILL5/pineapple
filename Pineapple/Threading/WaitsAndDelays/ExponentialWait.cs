using System;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class ExponentialWait
    {
        private readonly int _initialMs;
        private readonly int _growth;
        private readonly int? _maxDelay;
        private int _count = 0;

        public ExponentialWait(int initialMs, int growth = 1, int? maxDelay = null)
        {
            CheckIsNotLessThan(nameof(initialMs), initialMs, 1);
            CheckIsNotLessThan(nameof(growth), growth, 1);

            if (maxDelay.HasValue)
                CheckIsNotLessThan(nameof(maxDelay), maxDelay.Value, 1);

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

        public async Task WaitAsync()
        {
            await Task.Delay(Value);
        }
    }
}
