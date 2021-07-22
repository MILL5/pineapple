using System.Threading.Tasks;

namespace Pineapple.Threading
{
    public interface IParallelism
    {
        int MaxDegreeOfParallelism { get; }
        ParallelOptions Options { get; }

        int GetMaxDegreeOfParallelism(int? maxDegreeOfParallelism);
        ParallelOptions GetOptions(int? maxDegreeOfParallelism);
    }
}