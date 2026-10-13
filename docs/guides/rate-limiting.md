---
title: Rate limiting in C# — calls per minute with Pineapple
description: Throttle .NET code to a maximum calls per minute with Pineapple's async ResourceGoverner, RateLimiterScope, and SafeRateLimiterScope.
breadcrumb: Rate limiting
last_reviewed: 2026-10-13
faq:
  - q: How do I limit calls per minute in C#?
    a: "Create one Pineapple ResourceGoverner per dependency, for example `new ResourceGoverner(maxCallsPerMinute: 600)`, and wrap each call in `await using (await governor.GetOperationScopeAsync()) { ... }`. The governor waits until starting the call would keep the rate under the limit."
  - q: What is the difference between Pineapple's ResourceGoverner and System.Threading.RateLimiting?
    a: System.Threading.RateLimiting (built into .NET 7+) offers token-bucket, fixed-window, sliding-window, and concurrency limiters with lease semantics. Pineapple's ResourceGoverner is a simpler calls-per-minute governor with built-in exponential backoff, a timeout, and call statistics. Use the built-in limiters for ASP.NET Core request throttling; use ResourceGoverner for simple outbound throttling.
  - q: What happens if a ResourceGoverner cannot get a slot?
    a: It retries with exponential backoff and throws TimeoutException after the larger of 10 seconds or two call intervals (2 × 60 / maxCallsPerMinute seconds).
---

# Rate limiting and resource governors

**Pineapple offers three ways to cap how often code runs, all measured in calls per minute:** a per-call pacing scope, a cross-thread pacing scope keyed by caller, and an async sliding-window governor. All live in `Pineapple.Threading`.

| Type | Model | Best for |
| --- | --- | --- |
| `ResourceGoverner` | Async; sliding one-minute window; backoff + timeout | Outbound API/database calls from concurrent async code |
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

Before each call, the governor counts operations started in the last minute and computes the rate that starting one more would produce. If that would exceed `maxCallsPerMinute`, it waits with exponential backoff (capped at 10 s) and tries again. If no slot frees up within `max(10 s, 2 × 60 / maxCallsPerMinute s)`, it throws `TimeoutException`.

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
