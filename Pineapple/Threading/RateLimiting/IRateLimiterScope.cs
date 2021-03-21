using System;
using System.Collections.Generic;
using System.Text;

namespace Pineapple.Threading
{
    public interface IRateLimiterScope :  IDisposable, IAsyncDisposable
    {
    }
}
