using System.Threading.Tasks;

namespace Pineapple.Threading
{
    public class NullResourceGoverner : BaseResourceGoverner
    {
        public override async Task<IRateLimiterScope> GetOperationScopeAsync()
        {
            Operation nextOperation = null;

            lock (_operations.OperationLock)
            {
                nextOperation = new Operation(_operations);
            }

            _cpm.Add();

            await Task.CompletedTask;

            return nextOperation;
        }
    }
}
