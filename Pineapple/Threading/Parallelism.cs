using System;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class Parallelism : IParallelism
    {
        public ParallelOptions Options
        {
            get
            {
                int maxDegreeOfParallelism = MaxDegreeOfParallelism;

                return new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism };
            }
        }

        public int MaxDegreeOfParallelism
        {
            get
            {
                int maxCpusToUse = Convert.ToInt32(Math.Floor(Environment.ProcessorCount * 2.0 / 3.0));
                int minParallelism = maxCpusToUse > 1 ? 2 : 1;

                return Math.Max(maxCpusToUse, minParallelism);
            }
        }

        public int GetMaxDegreeOfParallelism(int? maxDegreeOfParallelism)
        {
            int value;

            if (maxDegreeOfParallelism.HasValue)
            {
                CheckIsNotLessThanOrEqualTo(nameof(maxDegreeOfParallelism), maxDegreeOfParallelism.Value, 0);

                value = Math.Min(MaxDegreeOfParallelism, maxDegreeOfParallelism.Value);
            }
            else
            {
                value = MaxDegreeOfParallelism;
            }

            return value;
        }

        public ParallelOptions GetOptions(int? maxDegreeOfParallelism)
        {
            int degreeOfParallelism = GetMaxDegreeOfParallelism(maxDegreeOfParallelism);

            return new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };
        }
    }
}
