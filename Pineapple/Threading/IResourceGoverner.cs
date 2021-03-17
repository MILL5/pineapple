using System.Threading.Tasks;

namespace Pineapple.Threading
{
    public interface IResourceGoverner
    {
        long TotalNumberOfCalls { get; }

        IRateLimiterScope GetOperationScope();
    }
}