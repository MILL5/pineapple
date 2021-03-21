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
            bool enable;

            lock (OperationLock)
            {
                Scavenge();

                enable = _operations.Count > 0;
            }

            return enable;
        }

        private void Scavenge()
        {
            var expiration = DateTime.Now.AddMinutes(-1.0);
            var expiredOperations = _operations.Where(x => x.Status == OperationStatus.Complete &&
                                                           x.Start < expiration).ToList();
            
            foreach (var operation in expiredOperations)
            {
                _operations.Remove(operation);
            }
        }

        public object OperationLock => _operationsLock;

        public long TotalCount => _totalCount;
        public TimeSpan TotalTime => _totalTime;

        public List<Operation> GetCurrent(DateTime expiration)
        {
            List<Operation> result;

            lock (OperationLock)
            {
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
                _operations.Add(operation);
                _totalCount++;
            }
        }

        public void Remove(Operation operation)
        {
            CheckIsNotNull(nameof(operation), operation);

            lock (OperationLock)
            {
                operation.Dispose();
                
                _totalTime += operation.ElapsedTime;

                Scavenge();
            }
        }

        public void Dispose()
        {
            SafeMethod(_scavenger.Stop);
        }

        public async ValueTask DisposeAsync()
        {
            SafeMethod(_scavenger.Stop);
            await Task.CompletedTask;
        }
    }
}