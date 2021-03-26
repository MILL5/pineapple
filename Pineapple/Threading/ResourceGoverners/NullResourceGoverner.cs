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

            _cpm.Add();

            return nextOperation;
        }
    }
}
