using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    internal enum OperationStatus { Unknown = 0, Running = 1, Complete = 2 }

    internal class Operation : IRateLimiterScope
    {
        private readonly Operations _operations;
        private readonly Stopwatch _watch = Stopwatch.StartNew();
        private TimeSpan _elapsed;
        public Operation(Operations operations)
        {
            CheckIsNotNull(nameof(operations), operations);
            _operations = operations;
            _operations.Add(this);
        }
        public OperationStatus Status { get; private set; } = OperationStatus.Running;
        public DateTime Start { get; } = DateTime.UtcNow;
        public DateTime? Completed { get; private set; }
        public TimeSpan ElapsedTime => Completed.HasValue ? _elapsed : throw new InvalidOperationException();

        // Called only while the owning collection holds its lock.
        internal bool Complete()
        {
            if (Status == OperationStatus.Complete) return false;
            _watch.Stop();
            _elapsed = _watch.Elapsed;
            Completed = DateTime.UtcNow;
            Status = OperationStatus.Complete;
            return true;
        }
        public void Dispose() => _operations.Remove(this);
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }
}
