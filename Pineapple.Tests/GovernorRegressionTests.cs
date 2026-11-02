using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pineapple.Threading;

namespace Pineapple.Tests;

[TestClass]
public class GovernorRegressionTests
{
    [TestMethod]
    [DataRow(7, 10000000L, 85714286L)]
    [DataRow(int.MaxValue, 10000000L, 1L)]
    [DataRow(2, 10000000L, 300000000L)]
    public void IntervalRoundsUpAtTheClockResolution(int rate, long frequency, long expected)
    {
        Assert.AreEqual(expected, ResourceGoverner.GetInterval(rate, frequency));
    }

    [TestMethod]
    public async Task FirstPermitIsImmediateAndConcurrentPermitsAreSpaced()
    {
        using var governor = new ResourceGoverner(600);
        var first = governor.GetOperationScopeAsync();
        Assert.IsTrue(first.IsCompletedSuccessfully);
        using var firstScope = await first;

        var waiting = Enumerable.Range(0, 5).Select(_ => governor.GetOperationScopeAsync()).ToArray();
        var scopes = await Task.WhenAll(waiting);
        try
        {
            var starts = scopes.Prepend(firstScope).Cast<Operation>().Select(x => x.Start).Order().ToArray();
            for (var i = 1; i < starts.Length; i++)
                Assert.IsGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(99), starts[i] - starts[i - 1]);
        }
        finally
        {
            foreach (var scope in scopes)
                scope.Dispose();
        }
    }

    [TestMethod]
    public void StatisticsAcceptOutOfOrderTimestamps()
    {
        var stats = new CallsPerMinute();
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        stats.Add(start.AddSeconds(2));
        stats.Add(start);
        stats.Add(start.AddSeconds(1));
        Assert.AreEqual(60d, stats.Value);

        // An older event cannot move the one-minute window backwards.
        stats.Add(start.AddMinutes(-2));
        Assert.AreEqual(60d, stats.Value);
    }

    [TestMethod]
    public void EqualTimestampsHaveNoMeasurableRate()
    {
        var stats = new CallsPerMinute();
        stats.Add(DateTime.MinValue);
        stats.Add(DateTime.MinValue);
        Assert.IsTrue(double.IsNaN(stats.Value));

        stats.Add(DateTime.MinValue.AddSeconds(1));
        Assert.AreEqual(120d, stats.Value);
    }
}
