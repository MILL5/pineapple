---
title: Timeouts, exponential backoff, and parallelism in .NET
description: Enforce deadlines with Pineapple's AccurateTimeout and SimpleTimeout, retry with ExponentialWait, and size Parallel.ForEach loops with CPU-aware Parallelism.
breadcrumb: Timeouts & backoff
last_reviewed: 2026-10-13
faq:
  - q: How do I implement exponential backoff in C#?
    a: "Use Pineapple's ExponentialWait: `var wait = new ExponentialWait(initialMs: 100, growth: 2, maxDelay: 5000);` and call `await wait.WaitAsync()` between attempts. The nth wait is initialMs × (n + 1)^growth, capped at maxDelay."
  - q: How many threads should Parallel.ForEach use?
    a: A common choice is a fraction of the logical processors. Pineapple's Parallelism defaults to two-thirds of Environment.ProcessorCount (rounded down, minimum 1) and returns ready-made ParallelOptions.
---

# Timeouts, backoff, and parallelism

## Timeouts

> **Deprecated in 2.0 (PINE003).** Use `CancellationTokenSource(TimeSpan)`, `Task.WaitAsync`, or a Polly retry. `Parallelism` is not deprecated. See the [migration guide]({{ '/guides/migration/#PINE003' | relative_url }}).
{: .callout}

**`AccurateTimeout` and `SimpleTimeout` (in `Pineapple.Threading`) track a deadline and throw `TimeoutException` from `EnsureNotTimedOut()` once it passes.** Both implement `ITimeout`.

```csharp
await using var timeout = new AccurateTimeout(timeoutInMs: 30_000);

while (!await IsReadyAsync())
{
    timeout.EnsureNotTimedOut();                    // throws TimeoutException after 30 s
    await Task.Delay(500);
}
Console.WriteLine($"Ready after {timeout.ElapsedInMilliseconds} ms");
```

| Type | Clock | Notes |
| --- | --- | --- |
| `AccurateTimeout` | `Stopwatch` (monotonic, high resolution) | Preferred |
| `SimpleTimeout` | `DateTime.Now`, whole seconds | Lower overhead, coarse |

`ITimeout` exposes `HasExpired`, `ElapsedInMilliseconds`, `RemainingInMilliseconds`, and `EnsureNotTimedOut()`.

## Exponential backoff

**`ExponentialWait` delays for `initialMs × (attempt + 1)^growth` milliseconds, optionally capped at `maxDelay`.**

```csharp
var wait = new ExponentialWait(initialMs: 100, growth: 2, maxDelay: 5_000);
// waits: 400, 900, 1600, 2500, 3600, 4900, 5000, 5000 ... ms

for (var attempt = 0; attempt < 8; attempt++)
{
    if (await TryConnectAsync()) break;
    await wait.WaitAsync();
}
```

## Parallelism

**`Parallelism` computes a CPU-aware `MaxDegreeOfParallelism`** — by default two-thirds of `Environment.ProcessorCount` (rounded down, minimum 1), and never more than the value you request.

```csharp
IParallelism parallelism = new Parallelism(requestedDegreeOfParallelism: 8);
Parallel.ForEach(files, parallelism.Options, Process);

var heavy = new Parallelism(factor: 0.5m);   // use half the CPUs
```

{% include faq.html heading="Questions about timeouts and backoff" %}
