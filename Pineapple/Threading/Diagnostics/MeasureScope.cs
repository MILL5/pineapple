using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace Pineapple.Threading
{
    public class MeasureScope : IDisposable, IAsyncDisposable
    {
        private readonly Stopwatch _sw;
        private readonly string _token;
        private long _numOfCalls;

        public MeasureScope([CallerMemberName] string token = null)
        {
            CheckIsNotNullOrWhitespace(nameof(token), token);

            _token = token ?? nameof(MeasureScope);
            _sw = new Stopwatch();
            _sw.Start();
        }

        public void Count(Action countThis)
        {
            try
            {
                countThis();
                Interlocked.Increment(ref _numOfCalls);
            }
            catch
            {
                Debug.WriteLine($@"[{_token}] Error detected.");
                throw;
            }
        }

        public void Dispose()
        {
            _sw.Stop();

            var c = Interlocked.Read(ref _numOfCalls);
            var cpm = c * 1.0 / _sw.ElapsedMilliseconds;

            Debug.WriteLine($@"[{_token}] {c} calls, CPM={cpm}");
        }

        public async ValueTask DisposeAsync()
        {
            _sw.Stop();
            await Task.CompletedTask;
        }
    }
}
