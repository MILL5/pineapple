using System;
using System.Threading;

namespace Pineapple.Threading
{
    public class SynchronizationContextScope : IDisposable
    {
        private readonly SynchronizationContext _saved;

        public SynchronizationContextScope(SynchronizationContext context)
        {
            _saved = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context);
        }

        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(_saved);
        }
    }
}
