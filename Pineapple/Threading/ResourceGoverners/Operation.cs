using System;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;
using System.Threading;

namespace Pineapple.Threading
{
    internal enum OperationStatus
    {
        Unknown = 0,
        Running = 1,
        Complete = 2
    }

    internal class Operation : IRateLimiterScope
    {
        private readonly Operations _operations;
        private readonly DateTime _start = DateTime.Now;
        private DateTime? _completed = null;
        private OperationStatus _status = OperationStatus.Running;

        public Operation(Operations operations)
        {
            CheckIsNotNull(nameof(operations), operations);

            _operations = operations;
            _operations.Add(this);        }

        public OperationStatus Status => _status;
        public DateTime Start => _start;
        public DateTime? Completed => _completed;

        public TimeSpan ElapsedTime
        {
            get
            {
                if (!Completed.HasValue)
                    throw new InvalidOperationException();

                return Completed.Value - Start;
            }
        }

        private void DisposeInternal()
        {
            SafeMethod(() =>
            {
                _status = OperationStatus.Complete;
                _completed = DateTime.Now;
            });
        }

        public void Dispose()
        {
            DisposeInternal();
        }

        public async ValueTask DisposeAsync()
        {
            DisposeInternal();
            await Task.CompletedTask;
        }
    }

}
