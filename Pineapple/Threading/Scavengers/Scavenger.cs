using System;
using System.Timers;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class Scavenger
    {
        private readonly Timer _timer;
        private readonly Func<bool> _scavengeAndContinue;

        public Scavenger(Func<bool> scavengeAndContinue, int everyMs)
        {
            CheckIsNotNull(nameof(scavengeAndContinue), scavengeAndContinue);
            CheckIsNotLessThan(nameof(everyMs), everyMs, 1);

            _scavengeAndContinue = scavengeAndContinue;
            _timer = new Timer(everyMs);
            _timer.Elapsed += Elapsed;
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;

            bool enabled = true;

            try
            {
                enabled = _scavengeAndContinue();
            }
            catch
            {
            }

            _timer.Enabled = enabled;
        }

        public void Start()
        {
            _timer.Enabled = true;
        }

        public void Stop()
        {
            _timer.Enabled = false;
        }
    }
}
