# Pineapple — Guava-inspired core utilities for .NET

<img src="https://raw.githubusercontent.com/MILL5/pineapple/main/pineapple.png" alt="Pineapple logo" width="96" align="right" />

[![NuGet](https://img.shields.io/nuget/v/Pineapple.svg?label=Pineapple)](https://www.nuget.org/packages/Pineapple)
[![NuGet](https://img.shields.io/nuget/v/Pineapple.Data.svg?label=Pineapple.Data)](https://www.nuget.org/packages/Pineapple.Data)
[![CI](https://github.com/MILL5/pineapple/actions/workflows/ci.yml/badge.svg)](https://github.com/MILL5/pineapple/actions/workflows/ci.yml)
[![License: Apache 2.0](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/MILL5/pineapple/blob/main/LICENSE)

**Pineapple is an open-source C# library of core utilities for .NET 8 and .NET 10, inspired by Google's Guava.** It gives you guard clauses (preconditions), URL-safe 22-character `ShortGuid`s, a deterministic xxHash `HashCode`, a SQL Server `RowVersion` type, calls-per-minute rate limiting and resource governors, timeouts, exponential backoff, timing scopes, and exception-safe cleanup helpers — small, dependency-light building blocks you would otherwise rewrite in every project.

📖 **Documentation:** https://mill5.github.io/pineapple/

## Install

```bash
dotnet add package Pineapple
dotnet add package Pineapple.Data   # optional: SQL Server / EF Core helpers
```

| Package | Targets | What's inside |
| --- | --- | --- |
| `Pineapple` | .NET 8, .NET 10 | Preconditions, ShortGuid, HashCode, RowVersion, Cleanup, threading, diagnostics, health checks, Brotli handler |
| `Pineapple.Data` | .NET 8 (EF Core 8), .NET 10 (EF Core 10) | EF Core model for `INFORMATION_SCHEMA`, SQL Server availability check |

## Quick start

```csharp
using Pineapple.Common;
using Pineapple.Threading;
using static Pineapple.Common.Preconditions;

public sealed class OrderService(IOrderRepository repository)
{
    // One governor per downstream dependency: at most 600 calls per minute.
    private static readonly ResourceGoverner Governor = new(maxCallsPerMinute: 600);

    public async Task<Order> GetAsync(string orderId, int quantity)
    {
        // Guard clauses throw ArgumentException-family exceptions with useful messages.
        CheckIsNotNullOrWhitespace(nameof(orderId), orderId);
        CheckIsNotLessThan(nameof(quantity), quantity, 1);

        // A ShortGuid is a Guid encoded as 22 URL-safe characters.
        ShortGuid id = orderId;

        await using (await Governor.GetOperationScopeAsync())
        {
            return await repository.GetAsync(id.Guid);
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
| **Rate limiting** | `RateLimiterScope`, `SafeRateLimiterScope`, `ResourceGoverner` | Cap calls per minute to an API or database |
| **Timing & retry** | `AccurateTimeout`, `SimpleTimeout`, `ExponentialWait`, `Parallelism` | Enforce deadlines, back off, size parallel loops |
| **Diagnostics** | `TimingScope`, `MeasureScope`, `ConsoleColorScope` | Log how long an operation took with `ILogger` |
| **Async plumbing** | `SynchronizationContextScope`, `SuppressSynchronizationContextScope` | Avoid sync-over-async deadlocks |
| **Cleanup** | `Cleanup.SafeMethod` | Run cleanup code that must never throw |
| **Health** | `Health.Check`, `Data.SqlClient.Check` | Probe TCP ports, URLs, processes, SQL Server |
| **Networking** | `BrotliCompressionHandler` | Decompress `br` responses in `HttpClient` pipelines |
| **Schema** | `InformationSchemaContext` | Query SQL Server `INFORMATION_SCHEMA` with LINQ |

See the [documentation site](https://mill5.github.io/pineapple/) for a guide to each area.

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
