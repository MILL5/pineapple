---
title: Timing .NET code with ILogger — Pineapple TimingScope
description: Measure and log how long .NET code takes with Pineapple's TimingScope and ILogger; count calls with MeasureScope and CallsPerMinute.
breadcrumb: Diagnostics
last_reviewed: 2026-10-13
---

# Diagnostics and timing

## `TimingScope`

**`Pineapple.Diagnostics.TimingScope` logs the elapsed time of a `using` block through `Microsoft.Extensions.Logging.ILogger` at `Debug` level.** It records the calling member automatically, opens a logging scope `Caller=[member]`, and writes a source-generated structured event (EventId 1) with `OperationName`, `MemberName`, and `ElapsedMilliseconds` fields you can query in Application Insights, Seq, or any structured log store. When `Debug` is disabled, it does nothing — no stopwatch, no allocation of a log message.

```csharp
using Pineapple.Diagnostics;

public async Task<IReadOnlyList<Customer>> LoadAsync()
{
    using (new TimingScope("LoadCustomers", _logger))
    {
        return await _db.Customers.ToListAsync();
    }
}
// dbug: Operation [LoadCustomers] called from [LoadAsync] took 42.17ms
```

## `MeasureScope`

> **Deprecated in 2.0 (PINE004).** Use `System.Diagnostics.Metrics` counters and histograms; `MeasureScope` and `CallsPerMinute` are removed in 3.0. See the [migration guide]({{ '/guides/migration/#PINE004' | relative_url }}).
{: .callout}

Counts successful calls made through `Count(Action)` and writes the total to `Debug.WriteLine` on dispose.

```csharp
using (var scope = new MeasureScope())
{
    foreach (var item in items)
        scope.Count(() => Process(item));
}
```

## `CallsPerMinute`

A thread-safe rolling calls-per-minute counter over the last 60 seconds.

Samples are kept in timestamp order even when concurrent callers report them out of order. The rate uses the elapsed time between the oldest and newest samples, with `n - 1` intervals for `n` samples.

```csharp
var cpm = new CallsPerMinute();
cpm.Add();                       // call on every request
double rate = cpm.Value;
```

## `ConsoleColorScope`

Temporarily changes `Console.ForegroundColor` and restores it on dispose.

```csharp
using (new ConsoleColorScope(ConsoleColor.Red))
{
    Console.WriteLine("Build failed");
}
```
