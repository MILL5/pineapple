using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Threading;

namespace Pineapple.Tests;

[TestClass]
public class LimiterLifecycleTests
{
    [TestMethod]
    public async Task SafeScopeCanBeDisposedAcrossThreadsAndReused()
    {
        var token = Guid.NewGuid().ToString();
        var scope = new SafeRateLimiterScope(6000, token);
        await Task.Run(async () => await scope.DisposeAsync()).WaitAsync(TimeSpan.FromSeconds(5));
        scope.Dispose();
        await using var next = new SafeRateLimiterScope(6000, token);
    }

    [TestMethod]
    public void ConflictingActiveTokenRatesFailAndRetiredTokensCanChangeRate()
    {
        var token = Guid.NewGuid().ToString();
        using (var first = new SafeRateLimiterScope(60000, token))
            Assert.ThrowsExactly<ArgumentException>(() => new SafeRateLimiterScope(1, token));
        using var second = new SafeRateLimiterScope(30000, token);
    }

    [TestMethod]
    public async Task SemaphoreAdmitsUpToCapacityAndDisposalReleasesOnce()
    {
        using var limiter = new SemaphoreRateLimiterScope(1);
        var first = limiter.GetOperationScope();
        using var started = new ManualResetEventSlim();
        var waiter = Task.Run(() => { started.Set(); return limiter.GetOperationScope(); });
        Assert.IsTrue(started.Wait(TimeSpan.FromSeconds(2)));
        Assert.IsFalse(waiter.IsCompleted);
        first.Dispose();
        await first.DisposeAsync();
        using var second = await waiter.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task SemaphoreShutdownUnblocksWaitersAndAllowsActiveScopesToDrain()
    {
        var limiter = new SemaphoreRateLimiterScope(1);
        var first = limiter.GetOperationScope();
        using var started = new ManualResetEventSlim();
        var waiter = Task.Run(() => { started.Set(); return limiter.GetOperationScope(); });
        Assert.IsTrue(started.Wait(TimeSpan.FromSeconds(2)));
        limiter.Dispose();
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(async () =>
            await waiter.WaitAsync(TimeSpan.FromSeconds(5)));
        first.Dispose();
        await first.DisposeAsync();
        Assert.ThrowsExactly<ObjectDisposedException>(() => limiter.GetOperationScope());
    }

    [TestMethod]
    public async Task GovernorCompletionIsIdempotentAndAveragesCompletedOperations()
    {
        using var governor = new ResourceGoverner(60000);
        Assert.AreEqual(TimeSpan.Zero, governor.AverageTime);
        var operation = await governor.GetOperationScopeAsync();
        await Task.Delay(10);
        operation.Dispose();
        var elapsed = governor.TotalTime;
        await operation.DisposeAsync();
        Assert.AreEqual(1L, governor.TotalCount);
        Assert.IsGreaterThan(TimeSpan.Zero, elapsed);
        Assert.AreEqual(elapsed, governor.TotalTime);
        Assert.AreEqual(elapsed, governor.AverageTime);
    }
}
