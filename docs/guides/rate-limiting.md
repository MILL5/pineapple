---
title: Rate limiting in C# — calls per minute with Pineapple
description: Throttle .NET code to a maximum calls per minute with Pineapple's async ResourceGoverner, RateLimiterScope, and SafeRateLimiterScope.
breadcrumb: Rate limiting
last_reviewed: 2026-10-28
faq:
  - q: How do I limit calls per minute in C#?
    a: "On .NET 7 and later, use System.Threading.RateLimiting: `new SlidingWindowRateLimiter(new() { PermitLimit = 600, Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6, QueueLimit = int.MaxValue })`, then `using var lease = await limiter.AcquireAsync();` before each call. Pineapple's ResourceGoverner does the same job but is deprecated in Pineapple 2.0."
  - q: What is the difference between Pineapple's ResourceGoverner and System.Threading.RateLimiting?
    a: The System.Threading.RateLimiting package (also included in ASP.NET Core) offers token-bucket, fixed-window, sliding-window, and concurrency limiters with lease semantics. Pineapple's ResourceGoverner is a simpler calls-per-minute governor with a timeout and call statistics. It is deprecated in Pineapple 2.0; use the Microsoft rate limiters for new code.
  - q: What happens if a ResourceGoverner cannot get a slot?
    a: It waits for the next slot and throws TimeoutException if admission keeps failing beyond its timeout. The timeout is max(10, 2 × ceiling(60 / maxCallsPerMinute)) seconds.
---

# Rate limiting and resource governors

> **Deprecated in 2.0 (PINE002).** Use the `System.Threading.RateLimiting` package (included in the ASP.NET Core shared framework). These types keep working in 2.x and are removed in 3.0. See the [migration guide]({{ '/guides/migration/#PINE002' | relative_url }}).
{: .callout}

**Pineapple offers three ways to cap how often code runs, all measured in calls per minute:** a per-call pacing scope, a cross-thread pacing scope keyed by caller, and an async governor that spaces out admissions. All live in `Pineapple.Threading`.

| Type | Model | Best for |
| --- | --- | --- |
| `ResourceGoverner` | Async; spaces admissions using a monotonic clock; waits for the next slot, with a timeout | Outbound API/database calls from concurrent async code |
| `RateLimiterScope` | Each scope lasts at least `60000 / callsPerMinute` ms | Simple sequential loops |
| `SafeRateLimiterScope` | Like `RateLimiterScope`, but serializes all callers sharing a token | Several threads calling the same method |
| `NullResourceGoverner`, `ZeroRateLimiterScope` | No limiting | Tests and feature flags |

## `ResourceGoverner`

```csharp
using Pineapple.Threading;

// Share one governor per downstream dependency (e.g. as a singleton).
var governor = new ResourceGoverner(maxCallsPerMinute: 300);

await Parallel.ForEachAsync(messages, async (message, ct) =>
{
    await using (await governor.GetOperationScopeAsync())
    {
        await client.PostAsync(message, ct);
    }
});

Console.WriteLine($"{governor.TotalCount} calls, ~{governor.CallsPerMinute:F0}/min");
```

The first call is admitted immediately. Later admissions are at least one interval apart, using the monotonic `Stopwatch` clock. The interval `60 s / maxCallsPerMinute` is rounded up to a whole clock tick, with a minimum of one tick, so rounding cannot shorten the required spacing. The next slot is scheduled from the actual admission time; idle time does not accumulate credit for a burst.

Callers wait asynchronously for the next slot and then recheck admission under a lock. There is no busy-spin wait; operating-system scheduling can make a wait longer than the requested interval. If admission keeps failing beyond the timeout, the caller throws `TimeoutException`. The timeout is `max(10, 2 × ceiling(60 / maxCallsPerMinute))` seconds.

Register it with dependency injection behind `IResourceGoverner` so tests can swap in `NullResourceGoverner`:

```csharp
services.AddSingleton<IResourceGoverner>(_ => new ResourceGoverner(maxCallsPerMinute: 300));
```

## `RateLimiterScope`

Pads each iteration so it takes at least `60000 / callsPerMinute` milliseconds.

```csharp
for (var i = 0; i < 100; i++)
{
    await using (new RateLimiterScope(callsPerMinute: 120))   // ≥ 500 ms per iteration
    {
        await SendAsync(i);
    }
}
```

## `SafeRateLimiterScope`

Serializes every caller that shares a token (by default the calling method's name) and paces them together.

```csharp
public void Poll()
{
    using (new SafeRateLimiterScope(callsPerMinute: 60))   // token = "Poll"
    {
        PollDevice();
    }
}
```

> `SafeRateLimiterScope` holds a `Monitor` lock for the duration of the scope. Use synchronous `using` and don't `await` inside it.
{: .callout}

{% include faq.html heading="Questions about rate limiting" %}
