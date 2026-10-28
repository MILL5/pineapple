# Pineapple — Guava-inspired core utilities for .NET

<img src="https://raw.githubusercontent.com/MILL5/pineapple/main/pineapple.png" alt="Pineapple logo" width="96" align="right" />

[![NuGet](https://img.shields.io/nuget/v/Pineapple.svg?label=Pineapple)](https://www.nuget.org/packages/Pineapple)
[![NuGet](https://img.shields.io/nuget/v/Pineapple.Data.svg?label=Pineapple.Data)](https://www.nuget.org/packages/Pineapple.Data)
[![CI](https://github.com/MILL5/pineapple/actions/workflows/ci.yml/badge.svg)](https://github.com/MILL5/pineapple/actions/workflows/ci.yml)
[![License: Apache 2.0](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/MILL5/pineapple/blob/main/LICENSE)

**Pineapple is an open-source C# library of core utilities for .NET 8 and .NET 10, inspired by Google's Guava.** It gives you guard clauses (preconditions), URL-safe 22-character `ShortGuid`s, a deterministic xxHash `HashCode`, a SQL Server `RowVersion` type, structured timing scopes, CPU-aware parallelism, and exception-safe cleanup helpers — small, dependency-light building blocks you would otherwise rewrite in every project.

📖 **Documentation:** https://mill5.github.io/pineapple/

## Install

```bash
dotnet add package Pineapple --version 2.0.0
dotnet add package Pineapple.Data --version 2.0.0   # optional: SQL Server / EF Core helpers
```

`Pineapple` and `Pineapple.Data` share version **2.0.0** and release together. Keep direct references to both packages on the same version.

| Package | Targets | What's inside |
| --- | --- | --- |
| `Pineapple` | .NET 8, .NET 10 | Preconditions, ShortGuid, HashCode, RowVersion, TimingScope, Parallelism, Cleanup |
| `Pineapple.Data` | .NET 8 (EF Core 8), .NET 10 (EF Core 10) | EF Core model for `INFORMATION_SCHEMA`, SQL Server availability check |

## Quick start

```csharp
using Pineapple.Common;
using Pineapple.Diagnostics;
using static Pineapple.Common.Preconditions;
using HashCode = Pineapple.Common.HashCode;

public sealed class OrderService(IOrderRepository repository, ILogger<OrderService> logger)
{
    public async Task<Order> GetAsync(string orderId, int quantity)
    {
        // Guard clauses throw ArgumentException-family exceptions with useful messages.
        CheckIsNotNullOrWhitespace(nameof(orderId), orderId);
        CheckIsNotLessThan(nameof(quantity), quantity, 1);

        // A ShortGuid is a Guid encoded as 22 URL-safe characters.
        ShortGuid id = orderId;

        // Deterministic hash: the same cache key on every machine and after every restart.
        var cacheKey = HashCode.Combine("order", id.Guid.ToString(), quantity);

        // Logs "Operation [LoadOrder] called from [GetAsync] took 12.34ms" at Debug level.
        using (new TimingScope("LoadOrder", logger))
        {
            return await repository.GetAsync(id.Guid, cacheKey);
        }
    }
}
```

## Features

| Area | Types | Use it to |
| --- | --- | --- |
| **Guard clauses** | `Preconditions` | Validate arguments: null/whitespace, ranges, dates, URIs, GUIDs, custom conditions |
| **Identifiers** | `ShortGuid`, `GuidExtensions` | Shorten GUIDs for URLs; sort GUIDs the way SQL Server does |
| **Hashing** | `HashCode` | Combine values into a hash that is **stable across processes** (xxHash32), unlike `System.HashCode` |
| **Concurrency tokens** | `RowVersion` | Compare, order, and convert SQL Server `rowversion`/`timestamp` values |
| **Parallelism** | `Parallelism` | Size `Parallel.ForEach` loops to the machine's CPUs |
| **Diagnostics** | `TimingScope`, `ConsoleColorScope` | Log how long an operation took with structured `ILogger` events |
| **Async plumbing** | `SynchronizationContextScope`, `SuppressSynchronizationContextScope` | Avoid sync-over-async deadlocks |
| **Cleanup** | `Cleanup.SafeMethod` | Run cleanup code that must never throw |
| **Schema** | `InformationSchemaContext` | Query SQL Server `INFORMATION_SCHEMA` with LINQ |

See the [documentation site](https://mill5.github.io/pineapple/) for a guide to each area.

### Deprecated in 2.0

.NET now ships built-in replacements for these, so they are marked `[Obsolete]` (diagnostics `PINE001`–`PINE008`) and will be removed in 3.0. The [migration guide](https://mill5.github.io/pineapple/guides/migration/) shows each replacement.

| Deprecated | Use instead |
| --- | --- |
| `BrotliCompressionHandler` | `AutomaticDecompression = DecompressionMethods.All` |
| `ResourceGoverner`, `RateLimiterScope`, `SafeRateLimiterScope`, `SemaphoreRateLimiterScope` | `System.Threading.RateLimiting` |
| `AccurateTimeout`, `SimpleTimeout`, `ExponentialWait` | `CancellationTokenSource`, `Task.WaitAsync`, Polly retry |
| `MeasureScope`, `CallsPerMinute` | `System.Diagnostics.Metrics` |
| `Scavenger` | `PeriodicTimer` |
| `Health.Check`, `Data.SqlClient.Check` | `Microsoft.Extensions.Diagnostics.HealthChecks` |
| `ImmutableExtensions` | `ImmutableList.Add` / `AddRange` |
| `ServiceCollectionExtensions` | `TryAddSingleton` |

## FAQ

**What is Pineapple?** An Apache-2.0-licensed set of C# utility classes for .NET from [MILL5](https://mill5.com), modeled on Google's Guava library for Java, with 110K+ NuGet downloads.

**Which .NET versions are supported?** .NET 8 (LTS) and .NET 10 (LTS). Version 1.x targeted .NET Standard 2.0; use `Pineapple` 1.0.41 if you still need .NET Framework.

**How is `Pineapple.Common.HashCode` different from `System.HashCode`?** `System.HashCode` is randomized per process. Pineapple's `HashCode` uses xxHash32, so the same inputs give the same hash on every machine and every run — useful for caching keys, sharding, and persisted hashes.

**What is a ShortGuid?** A 128-bit `Guid` encoded as 22 URL-safe Base64 characters (`-` and `_` instead of `+` and `/`, no padding) — 14 characters shorter than the standard 36-character form, with implicit conversions to and from `Guid` and `string`.

More questions: [FAQ](https://mill5.github.io/pineapple/faq/).

## Building from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet build Pineapple.slnx
dotnet test --solution Pineapple.slnx
dotnet pack Pineapple.slnx -c Release -o artifacts
```

## Part of the MILL5 open-source family

[FastSearch](https://mill5.github.io/FastSearch/) · [M5.BloomFilter](https://github.com/MILL5/M5.BloomFilter) · [yocto](https://github.com/MILL5/yocto) · [Polygon.Net](https://github.com/MILL5/Polygon.Net) · [Cut & Move](https://github.com/ChiefInnovator/cutandmove) · [MacEdgeLight](https://github.com/ChiefInnovator/macedgelight)

## Contributing

Issues and pull requests are welcome. See [CHANGELOG.md](https://github.com/MILL5/pineapple/blob/main/CHANGELOG.md) for release history.

## License

This project is licensed under the [Apache License 2.0](https://github.com/MILL5/pineapple/blob/main/LICENSE). © MILL5, LLC.

## Credits

Created by **[Richard Crane](https://mvp.microsoft.com/en-US/MVP/profile/10ce0bc0-7536-43f6-b28c-e9601a4a0d0d)** — Microsoft MVP and founder of **[MILL5](https://mill5.com)**.

- **MILL5:** [mill5.com](https://mill5.com)
- **Microsoft MVP profile:** [mvp.microsoft.com/…/Richard-Crane](https://mvp.microsoft.com/en-US/MVP/profile/10ce0bc0-7536-43f6-b28c-e9601a4a0d0d)
- **Podcast — _Inventing Fire with AI_:** [inventingfirewith.ai](https://inventingfirewith.ai)
- **Support:** [rich@mill5.com](mailto:rich@mill5.com)

---

Powered by [MILL5](https://www.mill5.com)
Explore [Richard Crane on Microsoft MVP](https://mvp.microsoft.com/en-US/MVP/profile/10ce0bc0-7536-43f6-b28c-e9601a4a0d0d)
Listen to the [Inventing Fire with AI](https://inventingfirewith.ai) podcast for insights on technology and innovation.
For support, contact: [rich@mill5.com](mailto:rich@mill5.com)
