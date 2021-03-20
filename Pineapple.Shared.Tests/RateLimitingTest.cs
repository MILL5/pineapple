using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Pineapple.Threading;

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
        public async Task ResourceGovernerAsync()
        {
            const int MAX_CALLS_PER_MINUTE = 120;

            var resourceGoverner = new ResourceGoverner(MAX_CALLS_PER_MINUTE);

            var callCount = 0;

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < MAX_CALLS_PER_MINUTE; i++)
            {
                using (resourceGoverner.GetOperationScope())
                {
                    callCount++;
                }
            }

            sw.Stop();

            Assert.AreEqual(MAX_CALLS_PER_MINUTE, resourceGoverner.TotalNumberOfCalls);

            var callsPerMinute = callCount / (sw.ElapsedMilliseconds / 60000.00m);
            callsPerMinute.ShouldBeLessThanOrEqualTo(MAX_CALLS_PER_MINUTE);

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task ResourceGovernerTimeOutAdjustmentMaxCallsPlusOneAsync()
        {
            // With low calls per minute this test should complete without timing out.
            const int MAX_CALLS_PER_MINUTE = 2;

            var resourceGoverner = new ResourceGoverner(MAX_CALLS_PER_MINUTE);

            for (int i = 0; i < MAX_CALLS_PER_MINUTE + 1; i++)
            {
                using (resourceGoverner.GetOperationScope())
                {
                }
            }

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
