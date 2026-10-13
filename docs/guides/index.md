---
title: Pineapple guides
description: Guides to every part of the Pineapple .NET library — guard clauses, ShortGuid, HashCode, RowVersion, rate limiting, timeouts, diagnostics, and SQL Server.
---

# Guides

Each guide explains what a Pineapple type is for, shows working C# code, and lists the API surface.

| Guide | Namespace | Key types |
| --- | --- | --- |
| [Guard clauses (Preconditions)]({{ '/guides/preconditions/' | relative_url }}) | `Pineapple.Common` | `Preconditions` |
| [ShortGuid and SQL GUID ordering]({{ '/guides/shortguid/' | relative_url }}) | `Pineapple.Common` | `ShortGuid`, `GuidExtensions` |
| [Deterministic HashCode]({{ '/guides/hashcode/' | relative_url }}) | `Pineapple.Common` | `HashCode` |
| [SQL Server RowVersion]({{ '/guides/rowversion/' | relative_url }}) | `Pineapple.Data` | `RowVersion` |
| [Rate limiting and resource governors]({{ '/guides/rate-limiting/' | relative_url }}) | `Pineapple.Threading` | `RateLimiterScope`, `SafeRateLimiterScope`, `ResourceGoverner` |
| [Timeouts, backoff, and parallelism]({{ '/guides/timeouts-and-backoff/' | relative_url }}) | `Pineapple.Threading` | `AccurateTimeout`, `SimpleTimeout`, `ExponentialWait`, `Parallelism` |
| [Diagnostics and timing]({{ '/guides/diagnostics/' | relative_url }}) | `Pineapple.Diagnostics`, `Pineapple.Threading` | `TimingScope`, `MeasureScope`, `ConsoleColorScope`, `CallsPerMinute` |
| [Cleanup and async helpers]({{ '/guides/cleanup-and-async/' | relative_url }}) | `Pineapple.Common`, `Pineapple.Threading`, `Pineapple.Extensions` | `Cleanup`, `SynchronizationContextScope`, `ImmutableExtensions` |
| [Health checks and networking]({{ '/guides/health-and-networking/' | relative_url }}) | `Pineapple.Health`, `Pineapple.Networking` | `Check`, `BrotliCompressionHandler` |
| [Pineapple.Data for SQL Server]({{ '/guides/data/' | relative_url }}) | `Pineapple.Data.*` | `InformationSchemaContext`, `SqlClient.Check` |
