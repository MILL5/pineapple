using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Threading;
using Shouldly;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pineapple.Shared.Tests
{
    [TestClass]
    public class RateLimitingTests
    {
        [TestMethod]
        public async Task RateLimitAsync()
        {
            const int MAX_CALLS_PER_MINUTE = 120;

            var sw = new Stopwatch();

            int x = 0;

            sw.Start();

            for (int i = 0; i < MAX_CALLS_PER_MINUTE; i++)
            {
                using (new RateLimiterScope(MAX_CALLS_PER_MINUTE))
                {
                    x++;

                    if (i % 100 == 0)
                    {
                        await Task.Delay(1);
                    }
                }
            }

            sw.Stop();

            var elapsed = sw.ElapsedMilliseconds;

            var callsPerMinute = x / (elapsed / 60000.00);

            callsPerMinute.ShouldBeLessThan(MAX_CALLS_PER_MINUTE);

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task ResourceGoverner1200Async()
        {
            const int MAX_CALLS_PER_MINUTE = 1200;

            await ResourceGovernerAsync(MAX_CALLS_PER_MINUTE);
        }

        [TestMethod]
        public async Task ResourceGoverner120Async()
        {
            const int MAX_CALLS_PER_MINUTE = 120;

            await ResourceGovernerAsync(MAX_CALLS_PER_MINUTE);
        }

        [TestMethod]
        public async Task ResourceGoverner12Async()
        {
            const int MAX_CALLS_PER_MINUTE = 12;

            await ResourceGovernerAsync(MAX_CALLS_PER_MINUTE);
        }

        [TestMethod]
        public async Task ResourceGoverner6Async()
        {
            const int MAX_CALLS_PER_MINUTE = 6;

            await ResourceGovernerAsync(MAX_CALLS_PER_MINUTE);
        }

        private async Task ResourceGovernerAsync(int maxCallsPerMinute)
        {
            var resourceGoverner = new ResourceGoverner(maxCallsPerMinute);
            var callCount = 0;

            var sw = new Stopwatch();
            sw.Start();

            int i;

            for (i = 0; i < maxCallsPerMinute; i++)
            {
                using (resourceGoverner.GetOperationScope())
                {
                    callCount++;
                }
            }

            sw.Stop();

            var callsPerMinute = callCount / (sw.ElapsedMilliseconds / 60000.00);
            callsPerMinute.ShouldBeLessThanOrEqualTo(maxCallsPerMinute);

            var efficiency = maxCallsPerMinute * 0.95;
            callsPerMinute.ShouldBeGreaterThanOrEqualTo(efficiency);

            Debug.WriteLine($"Efficiency:{callsPerMinute / maxCallsPerMinute}");

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task ResourceGovernerTimeOutAdjustmentMaxCallsPlusOneAsync()
        {
            //
            // With low calls per minute this test should complete without timing out.
            //
            const int MAX_CALLS_PER_MINUTE = 2;

            var resourceGoverner = new ResourceGoverner(MAX_CALLS_PER_MINUTE);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < MAX_CALLS_PER_MINUTE; i++)
            {
                using (resourceGoverner.GetOperationScope())
                {
                }
            }

            sw.Stop();
            Debug.WriteLine($"{sw.Elapsed.TotalSeconds}");

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task ResourceGovernerTimeOutAdjustmentMaxCallsAsync()
        {
            // With low calls per minute this test should complete without timing out.
            const int MAX_CALLS_PER_MINUTE = 2;

            var resourceGoverner = new ResourceGoverner(MAX_CALLS_PER_MINUTE);

            for (int i = 0; i < MAX_CALLS_PER_MINUTE; i++)
            {
                using (resourceGoverner.GetOperationScope())
                {
                }
            }

            await Task.CompletedTask;
        }

#if (NET || NETCOREAPP)
        [TestMethod]
        public async Task AsyncRateLimitTestAsync()
        {
            const int MAX_CALLS_PER_MINUTE = 120;

            var sw = new Stopwatch();

            int x = 0;

            sw.Start();

            for (int i = 0; i < MAX_CALLS_PER_MINUTE; i++)
            {
                await using (new RateLimiterScope(MAX_CALLS_PER_MINUTE))
                {
                    x++;

                    if (i % 100 == 0)
                    {
                        await Task.Delay(1);
                    }
                }
            }

            sw.Stop();

            var elapsed = sw.ElapsedMilliseconds;

            var callsPerMinute = x / (elapsed / 60000.00);

            callsPerMinute.ShouldBeLessThan(MAX_CALLS_PER_MINUTE);

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task RateNoLimitAsync()
        {
            const int MAX_CALLS_PER_MINUTE = 120;

            var sw = new Stopwatch();

            int x = 0;

            sw.Start();

            for (int i = 0; i < MAX_CALLS_PER_MINUTE; i++)
            {
                using (new RateLimiterScope(MAX_CALLS_PER_MINUTE * 10))
                {
                    x++;

                    if (i % 100 == 0)
                    {
                        await Task.Delay(1);
                    }
                }
            }

            sw.Stop();

            var elapsed = sw.ElapsedMilliseconds;

            var callsPerMinute = x / (elapsed / 60000.00);

            callsPerMinute.ShouldBeGreaterThan(MAX_CALLS_PER_MINUTE);

            await Task.CompletedTask;
        }
#endif
    }
}
