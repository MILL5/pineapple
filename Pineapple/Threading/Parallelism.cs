using System;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading;

public class Parallelism : IParallelism
{
    private readonly int? _requestedDegreeOfParallelism;

    public Parallelism(int? requestedDegreeOfParallelism = null, decimal? factor = null)
    {
        if (requestedDegreeOfParallelism.HasValue)
        {
            CheckIsNotLessThanOrEqualTo(nameof(requestedDegreeOfParallelism), requestedDegreeOfParallelism.Value, 0);
        }

        if (factor.HasValue)
        {
            CheckIsNotLessThanOrEqualTo(nameof(factor), factor.Value, 0.0m);
        }
        else
        {
            Factor = 2.0m / 3.0m;
        }


        _requestedDegreeOfParallelism = requestedDegreeOfParallelism;
    }

    public virtual decimal Factor { get; }
    protected virtual int CalculatedDegreeOfParallelism()
    {
        int maxCpusToUse = Convert.ToInt32(Math.Floor(Environment.ProcessorCount * Factor));
        int minParallelism = maxCpusToUse > 1 ? 2 : 1;

        // Ensure a minimum level of parallelism (i.e. maximum)
        int calculatedMaximum = Math.Max(maxCpusToUse, minParallelism);

        // Ensure we respect the wishes of the caller
        int calculatedDegreeOfParallelism = _requestedDegreeOfParallelism.HasValue ?
            Math.Min(calculatedMaximum, _requestedDegreeOfParallelism.Value) :
            calculatedMaximum;

        return calculatedDegreeOfParallelism;
    }

    public ParallelOptions Options => new() { MaxDegreeOfParallelism = CalculatedDegreeOfParallelism() };

    public int MaxDegreeOfParallelism => CalculatedDegreeOfParallelism();
}