﻿using System;

namespace Pineapple.Threading
{
    public class SuppressSynchronizationContextScope : IDisposable
    {
        private readonly SynchronizationContextScope _contextScope;

        public SuppressSynchronizationContextScope()
        {
            _contextScope = new SynchronizationContextScope(null);
        }

        public void Dispose()
        {
            _contextScope.Dispose();
        }
    }
}
