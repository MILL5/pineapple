using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;
using static Pineapple.Common.Cleanup;

namespace Pineapple.Threading
{
    internal class Operations : IDisposable, IAsyncDisposable
    {
        private long _totalCount = 0;
        private long _completedCount;
        private bool _disposed;
        private TimeSpan _totalTime = TimeSpan.Zero;

        private readonly List<Operation> _operations = new();
        private readonly object _operationsLock = new();
        private readonly Scavenger _scavenger;

        public Operations()
        {
            _scavenger = new Scavenger(ScavengeAndContinue, 60000);
        }

        private bool ScavengeAndContinue()
        {
            lock (OperationLock)
            {
                Scavenge();

            }

            return true; // Keep running until disposal; Add also restarts idle cleanup.
        }

        private void Scavenge()
        {
            var expiration = DateTime.UtcNow.AddMinutes(-1.0);
            var expiredOperations = _operations.Where(x => x.Status == OperationStatus.Complete &&
                                                           x.Start < expiration).ToList();

            foreach (var operation in expiredOperations)
            {
                _operations.Remove(operation);
            }
        }

        public void EnsureNotDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

        public object OperationLock => _operationsLock;

        public long TotalCount { get { lock (OperationLock) return _totalCount; } }
        public TimeSpan TotalTime { get { lock (OperationLock) return _totalTime; } }
        public TimeSpan AverageTime
        {
            get { lock (OperationLock) return _completedCount == 0 ? TimeSpan.Zero :
                TimeSpan.FromTicks(_totalTime.Ticks / _completedCount); }
        }

        public List<Operation> GetCurrent(DateTime expiration)
        {
            List<Operation> result;

            lock (OperationLock)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                result = _operations.Where(x => x.Start >= expiration)
                                  .ToList();
            }

            return result;
        }

        public void Add(Operation operation)
        {
            CheckIsNotNull(nameof(operation), operation);

            lock (OperationLock)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                _operations.Add(operation);
                _scavenger.Start();
                _totalCount++;
            }
        }

        public void Remove(Operation operation)
        {
            CheckIsNotNull(nameof(operation), operation);

            lock (OperationLock)
            {
                if (operation.Complete())
                {
                    _totalTime += operation.ElapsedTime;
                    _completedCount++;
                }

                Scavenge();
            }
        }

        public void Dispose()
        {
            lock (OperationLock)
            {
                if (_disposed) return;
                _disposed = true;
                _scavenger.Dispose();
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
