using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Threading
{
    public class NullResourceGoverner : BaseResourceGoverner
    {
        public override IRateLimiterScope GetOperationScope()
        {
            Operation nextOperation = null;

            lock (_operations.OperationLock)
            {
                nextOperation = new Operation(_operations);
            }

            return nextOperation;
        }
    }
}
