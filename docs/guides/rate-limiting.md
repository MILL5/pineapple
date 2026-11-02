---
title: Rate limiting in C# — calls per minute with Pineapple
description: Throttle .NET code to a maximum calls per minute with Pineapple's async ResourceGoverner, RateLimiterScope, and SafeRateLimiterScope.
breadcrumb: Rate limiting
last_reviewed: 2026-10-28
faq:
  - q: How do I limit calls per minute in C#?
    a: "On .NET 7 and later, use System.Threading.RateLimiting: `new SlidingWindowRateLimiter(new() { PermitLimit = 600, Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6, QueueLimit = int.MaxValue })`, then `using var lease = await limiter.AcquireAsync();` before each call. Pineapple's ResourceGoverner does the same job but is deprecated in Pineapple 2.0."
  - q: What is the difference between Pineapple's ResourceGoverner and System.Threading.RateLimiting?
    a: System.Threading.RateLimiting (built into .NET 7+) offers token-bucket, fixed-window, sliding-window, and concurrency limiters with lease semantics. Pineapple's ResourceGoverner is a simpler calls-per-minute governor with evenly spaced admissions, a timeout, and call statistics. Use the built-in limiters for ASP.NET Core request throttling; use ResourceGoverner for simple outbound throttling.
  - q: What happens if a ResourceGoverner cannot get a slot?
    a: It waits for the next admission deadline and throws TimeoutException after the larger of 10 seconds or two call intervals (2 × 60 / maxCallsPerMinute seconds).
---

# Rate limiting and resource governors

> Compatibility APIs with platform replacements remain available through 2.x and report migration diagnostics. See the [migration guide](migration.md).

**Pineapple offers three ways to cap how often code runs, all measured in calls per minute:** a per-call pacing scope, a cross-thread pacing scope keyed by caller, and an async paced-admission governor. All live in `Pineapple.Threading`.

| Type | Model | Best for |
| --- | --- | --- |
| `ResourceGoverner` | Async; evenly spaced starts; bounded waiting | Outbound API/database calls from concurrent async code |
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

The governor spaces admitted starts at least `60000 / maxCallsPerMinute` milliseconds apart using a monotonic clock. The first acquisition is immediate, preserving existing behavior. Idle time does not accumulate burst allowance; acquisition is not FIFO, and admitted operations can run concurrently. If admission cannot complete within `max(10 s, 2 × 60 / maxCallsPerMinute s)`, it throws `TimeoutException`.

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

> `SafeRateLimiterScope` acquires synchronously and releases through a semaphore, so `await using` may dispose on a different thread. Do not nest scopes with the same token.
{: .callout}

{% include faq.html heading="Questions about rate limiting" %}

## Lifetime and concurrency contracts

`SafeRateLimiterScope` uses a process-local semaphore per token. The constructor acquires synchronously; asynchronous disposal can resume on another thread. Active scopes sharing a token must use the same rate or construction throws `ArgumentException`. Registry entries are retired after their last active or waiting scope leaves. Scope duration starts after acquisition; disposal pads that duration, releases once, and repeated disposal is harmless. The existing API does not accept cancellation and does not promise FIFO fairness.

`SemaphoreRateLimiterScope(maxCalls)` limits concurrent operations, not calls per minute. Acquire with `GetOperationScope()` and dispose each operation to release its permit. Disposing the owner rejects new acquisitions and unblocks waiting acquisitions with `ObjectDisposedException`; already admitted scopes may finish and release safely.

For governors, `TotalCount` counts starts, `TotalTime` sums completed operation durations, and `AverageTime` averages completed operations, returning zero before completion. Completion is idempotent. Completed operation records are retained for the one-minute diagnostic window and periodically scavenged until the governor is disposed. The window is used for diagnostics and retention; admission uses monotonic deadlines. Share a governor instance to share its limit; replicas do not coordinate limits.

`CallsPerMinute` estimates observed rate from `N−1` intervals between `N` starts in the latest recorded one-minute window. Equal timestamps yield `NaN`; out-of-order events are ordered before calculation. The cached estimate updates on new events and is diagnostic; monotonic permit scheduling enforces the limit independently.
