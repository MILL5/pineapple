---
title: Migrating from deprecated Pineapple APIs to built-in .NET
description: Replace Pineapple's deprecated rate limiters, timeouts, Brotli handler, health checks, and counters with System.Threading.RateLimiting, Polly, HealthChecks, and Metrics.
breadcrumb: Migration
last_reviewed: 2026-10-13
faq:
  - q: Why are parts of Pineapple deprecated in 2.0?
    a: .NET 6 through .NET 10 added built-in, better-supported versions of several Pineapple features — rate limiters, resilience pipelines, health checks, metrics, PeriodicTimer, and automatic Brotli decompression. Pineapple 2.0 marks its versions [Obsolete] with diagnostic IDs PINE001–PINE008 and will remove them in 3.0.
  - q: How do I suppress a Pineapple deprecation warning?
    a: "Each area has its own diagnostic ID. Add it to `<NoWarn>` in your project file (for example `<NoWarn>$(NoWarn);PINE002</NoWarn>`) or use `#pragma warning disable PINE002` around the call."
  - q: What replaces Pineapple's ResourceGoverner?
    a: System.Threading.RateLimiting's SlidingWindowRateLimiter, built into .NET 7 and later. Set PermitLimit to the calls per minute, Window to one minute, and a QueueLimit so callers wait instead of being rejected.
---

# Migrating from deprecated APIs

**Pineapple 2.0 marks the features that .NET now provides itself as `[Obsolete]`.** They still work in 2.x and will be removed in 3.0. Each area has its own diagnostic ID, so the compiler warning links straight to the matching section below and you can suppress one area at a time (`<NoWarn>$(NoWarn);PINE002</NoWarn>`).

| ID | Deprecated | Replacement |
| --- | --- | --- |
| [PINE001](#PINE001) | `BrotliCompressionHandler`, `AddBrotliCompression` | `AutomaticDecompression = DecompressionMethods.All` |
| [PINE002](#PINE002) | `ResourceGoverner`, `RateLimiterScope`, `SafeRateLimiterScope`, `SemaphoreRateLimiterScope` | `System.Threading.RateLimiting` |
| [PINE003](#PINE003) | `AccurateTimeout`, `SimpleTimeout`, `ExponentialWait` | `CancellationTokenSource`, `Task.WaitAsync`, Polly retry |
| [PINE004](#PINE004) | `MeasureScope`, `CallsPerMinute` | `System.Diagnostics.Metrics` |
| [PINE005](#PINE005) | `Scavenger` | `PeriodicTimer` |
| [PINE006](#PINE006) | `Health.Check`, `Data.SqlClient.Check` | `Microsoft.Extensions.Diagnostics.HealthChecks` |
| [PINE007](#PINE007) | `ImmutableExtensions.Append` | `ImmutableList.Add` / `AddRange` |
| [PINE008](#PINE008) | `ServiceCollectionExtensions` | `TryAddSingleton` |

## PINE001 — Brotli decompression {#PINE001}

`HttpClient` has decompressed Brotli natively since .NET Core 3.0.

```csharp
builder.Services
    .AddHttpClient("catalog", c => c.BaseAddress = new Uri("https://catalog.contoso.com"))
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        AutomaticDecompression = DecompressionMethods.All   // gzip, deflate, and br
    });
```

The handler also sends the matching `Accept-Encoding` header for you.

## PINE002 — Rate limiting {#PINE002}

`System.Threading.RateLimiting` (built into .NET 7+) replaces every Pineapple rate limiter.

**`ResourceGoverner` → `SlidingWindowRateLimiter`**

```csharp
using System.Threading.RateLimiting;

var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
{
    PermitLimit = 300,                       // calls per minute
    Window = TimeSpan.FromMinutes(1),
    SegmentsPerWindow = 6,
    QueueLimit = int.MaxValue,               // wait for a permit instead of failing
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
});

using RateLimitLease lease = await limiter.AcquireAsync(permitCount: 1, cancellationToken: cancellationToken);
if (lease.IsAcquired)
{
    await client.PostAsync(message, cancellationToken);
}
```

**`RateLimiterScope` / `SafeRateLimiterScope` (steady pacing) → `TokenBucketRateLimiter`**

```csharp
var pacer = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
{
    TokenLimit = 1,
    TokensPerPeriod = 1,
    ReplenishmentPeriod = TimeSpan.FromMilliseconds(60_000 / 120),   // 120 calls per minute
    QueueLimit = int.MaxValue
});
```

**`SemaphoreRateLimiterScope` → `ConcurrencyLimiter`**, and **`NullResourceGoverner` / `ZeroRateLimiterScope`** → a `ConcurrencyLimiter` with `PermitLimit = int.MaxValue` in tests.

For outbound HTTP, add the limiter to a resilience pipeline with [Microsoft.Extensions.Http.Resilience](https://learn.microsoft.com/dotnet/core/resilience/http-resilience): `.AddResilienceHandler("api", b => b.AddRateLimiter(limiter))`.

## PINE003 — Timeouts and backoff {#PINE003}

**`AccurateTimeout` / `SimpleTimeout` → cancellation**

```csharp
using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

while (!await IsReadyAsync(timeout.Token))
{
    await Task.Delay(500, timeout.Token);    // throws OperationCanceledException after 30 s
}

// Or bound a single task:
var result = await LoadAsync().WaitAsync(TimeSpan.FromSeconds(30));   // throws TimeoutException
```

Inject [`TimeProvider`](https://learn.microsoft.com/dotnet/standard/datetime/timeprovider-overview) (.NET 8+) when tests need to control time.

**`ExponentialWait` → Polly retry** (package `Polly.Core`)

```csharp
using Polly;
using Polly.Retry;

var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 8,
        BackoffType = DelayBackoffType.Exponential,
        Delay = TimeSpan.FromMilliseconds(100),
        MaxDelay = TimeSpan.FromSeconds(5),
        UseJitter = true
    })
    .AddTimeout(TimeSpan.FromSeconds(30))
    .Build();

await pipeline.ExecuteAsync(async ct => await ConnectAsync(ct), cancellationToken);
```

## PINE004 — Counting calls {#PINE004}

```csharp
using System.Diagnostics.Metrics;

var meter = new Meter("Contoso.Orders");
var calls = meter.CreateCounter<long>("orders.calls");
var duration = meter.CreateHistogram<double>("orders.duration", unit: "ms");

calls.Add(1);
duration.Record(elapsed.TotalMilliseconds);
```

Watch live rates with `dotnet-counters monitor --counters Contoso.Orders`, or export to OpenTelemetry.

## PINE005 — Background scavenging {#PINE005}

```csharp
using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

while (await timer.WaitForNextTickAsync(cancellationToken))
{
    Scavenge();
}
```

## PINE006 — Health checks {#PINE006}

```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)                                      // AspNetCore.HealthChecks.SqlServer
    .AddUrlGroup(new Uri("https://contoso.com/health"))                  // AspNetCore.HealthChecks.Uris
    .AddTcpHealthCheck(o => o.AddHost("sql.contoso.internal", 1433));   // AspNetCore.HealthChecks.Network

app.MapHealthChecks("/health");
```

To wake a paused Azure SQL serverless database, let EF Core retry the first connection: `options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure())`. For `Check.IsRunning`, use `Process.GetProcessesByName(name).Length > 0` (exact process name match).

## PINE007 — Immutable collections {#PINE007}

`IImmutableList<T>.Add` already returns a new list and shares structure with the old one, so it avoids the full copy `Append` made.

```csharp
list = (list ?? ImmutableList<Order>.Empty).Add(order);
list = list.AddRange(moreOrders);
dict = dict.Add(key, value);           // throws on a duplicate key, like Append did
ImmutableArray<int> all = [.. first, .. second];   // C# 12 collection expression
```

## PINE008 — Service registration {#PINE008}

```csharp
using Microsoft.Extensions.DependencyInjection.Extensions;

services.TryAddSingleton<MyService>();                                   // replaces Configure<T>
bool registered = services.Any(d => d.ServiceType == typeof(MyService)); // replaces IsConfigured<T>
```

`Configure<T>` also clashed with the options-pattern `services.Configure<TOptions>(...)`.

{% include faq.html heading="Questions about migrating" %}
