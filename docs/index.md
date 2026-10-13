---
description: Pineapple is a free, open-source C# library for .NET 8 and .NET 10 with guard clauses, ShortGuid, deterministic xxHash HashCode, SQL Server RowVersion, rate limiting, timeouts, and backoff.
wide: true
faq:
  - q: What is Pineapple for .NET?
    a: Pineapple is a free, open-source (Apache 2.0) C# library from MILL5 that provides core utilities for .NET 8 and .NET 10 — guard clauses, ShortGuid, a deterministic HashCode, a SQL Server RowVersion type, rate limiting, resource governors, timeouts, exponential backoff, and timing scopes. It is inspired by Google's Guava library for Java.
  - q: How do I install Pineapple?
    a: "Run `dotnet add package Pineapple`. For SQL Server and Entity Framework Core helpers, also run `dotnet add package Pineapple.Data`."
  - q: Which .NET versions does Pineapple support?
    a: Pineapple 2.x targets .NET 8 (LTS) and .NET 10 (LTS). Pineapple 1.0.41 is the last release that targets .NET Standard 2.0 and .NET Framework.
  - q: Is Pineapple free for commercial use?
    a: Yes. Pineapple is licensed under the Apache License 2.0, which permits commercial use, modification, and redistribution.
  - q: Who built Pineapple?
    a: Pineapple was built by Richard Crane, a Microsoft Most Valuable Professional (MVP) and founder of MILL5, a Boston-based Microsoft Gold Partner specializing in cloud, AI, IoT, data analytics and modern application development.
---

<section class="hero">
  <div>
    <h1>Pineapple: Guava-inspired core utilities for .NET</h1>
    <p class="lead"><strong>Pineapple is a free, open-source C# library of small, well-tested building blocks for .NET 8 and .NET 10</strong> — guard clauses, URL-safe ShortGuids, a deterministic hash code, SQL Server row versions, rate limiting, timeouts, and backoff. Stop rewriting the same helpers in every project.</p>
    <p class="byline">Built by <a href="{{ site.author.url }}">Richard Crane</a>, Microsoft MVP, at <a href="{{ site.org.url }}">MILL5</a> · 110K+ NuGet downloads</p>
    <div class="actions">
      <a class="button" href="{{ '/getting-started/' | relative_url }}">Get started</a>
      <a class="button secondary" href="{{ site.github_repo }}">View on GitHub</a>
      <a class="button secondary" href="{{ site.nuget_url }}">NuGet</a>
    </div>
  </div>
  <img src="{{ '/assets/pineapple.png' | relative_url }}" alt="Pineapple logo" width="112" height="112">
</section>

<div class="install" markdown="1">

```bash
dotnet add package Pineapple
```

</div>

## What's in the box

<div class="cards">
  <div class="card"><a href="{{ '/guides/preconditions/' | relative_url }}"><h3>Guard clauses</h3></a><p>One-line argument checks for null, whitespace, ranges, dates, URIs, and GUIDs.</p></div>
  <div class="card"><a href="{{ '/guides/shortguid/' | relative_url }}"><h3>ShortGuid</h3></a><p>A Guid as 22 URL-safe characters, with implicit conversions to Guid and string.</p></div>
  <div class="card"><a href="{{ '/guides/hashcode/' | relative_url }}"><h3>Deterministic HashCode</h3></a><p>xxHash32 hashes that are identical across processes, machines, and restarts.</p></div>
  <div class="card"><a href="{{ '/guides/rowversion/' | relative_url }}"><h3>RowVersion</h3></a><p>Compare and convert SQL Server <code>rowversion</code> values as a 64-bit struct.</p></div>
  <div class="card"><a href="{{ '/guides/rate-limiting/' | relative_url }}"><h3>Rate limiting</h3></a><p>Cap calls per minute to an API or database with scopes and resource governors.</p></div>
  <div class="card"><a href="{{ '/guides/timeouts-and-backoff/' | relative_url }}"><h3>Timeouts &amp; backoff</h3></a><p>Deadlines, exponential waits, and CPU-aware degrees of parallelism.</p></div>
  <div class="card"><a href="{{ '/guides/diagnostics/' | relative_url }}"><h3>Diagnostics</h3></a><p>Time operations with <code>ILogger</code> using a single <code>using</code> block.</p></div>
  <div class="card"><a href="{{ '/guides/cleanup-and-async/' | relative_url }}"><h3>Cleanup &amp; async</h3></a><p>Exception-safe cleanup and synchronization-context scopes.</p></div>
  <div class="card"><a href="{{ '/guides/health-and-networking/' | relative_url }}"><h3>Health &amp; networking</h3></a><p>Probe ports, URLs, and processes; decompress Brotli responses.</p></div>
  <div class="card"><a href="{{ '/guides/data/' | relative_url }}"><h3>Pineapple.Data</h3></a><p>Query SQL Server <code>INFORMATION_SCHEMA</code> with EF Core and check availability.</p></div>
</div>

## A taste

```csharp
using Pineapple.Common;
using static Pineapple.Common.Preconditions;

public Order Create(string customerId, int quantity, DateTime requestedAt)
{
    CheckIsNotNullOrWhitespace(nameof(customerId), customerId);
    CheckIsNotLessThan(nameof(quantity), quantity, 1);
    CheckIsValidDateRange(nameof(requestedAt), requestedAt, plusOrMinusInHours: 24);

    ShortGuid id = ShortGuid.NewGuid();      // e.g. "xnS5yWvbWE2Ky1Ue7PZkZw"
    return new Order(id.Guid, customerId, quantity);
}
```

## Why Pineapple

- **Small and focused.** Each type solves one recurring problem and has no framework opinions.
- **Modern .NET.** Targets .NET 8 and .NET 10 LTS, with SourceLink and symbol packages for step-through debugging.
- **Tested.** Maintained by MILL5 since 2019, with unit tests run on .NET 8 and .NET 10 in CI.
- **Free.** Apache 2.0 licensed.

<section class="family" aria-labelledby="family-heading">
  <h2 id="family-heading">Part of the MILL5 open-source family</h2>
  <p>Pineapple is one of many open-source .NET libraries and tools built by MILL5, a Boston-based Microsoft Gold Partner.</p>
  <div class="family-grid">
    <div class="family-item"><h3><a href="https://mill5.github.io/FastSearch/">FastSearch</a></h3><p>Fast, low-memory substring search for .NET 10. NGramSearch indexes 1M items in about a second with 47 MiB.</p></div>
    <div class="family-item"><h3><a href="https://github.com/MILL5/M5.BloomFilter">M5.BloomFilter</a></h3><p>Probabilistic Bloom filters for membership testing at scale, with JSON and Protobuf serialization.</p></div>
    <div class="family-item"><h3><a href="https://github.com/MILL5/yocto">yocto</a></h3><p>An extremely small, low-on-ceremony, high-on-value Inversion of Control container for .NET.</p></div>
    <div class="family-item"><h3><a href="https://github.com/MILL5/Polygon.Net">Polygon.Net</a></h3><p>A C# wrapper for the Polygon.io financial data API: stocks, options, forex, and crypto.</p></div>
    <div class="family-item"><h3><a href="https://github.com/ChiefInnovator/cutandmove">Cut &amp; Move</a></h3><p>A free macOS menu bar utility that adds real Cmd+X cut-and-move to Finder.</p></div>
    <div class="family-item"><h3><a href="https://github.com/ChiefInnovator/macedgelight">MacEdgeLight</a></h3><p>An ambient screen-edge light for macOS that replaces a ring light on video calls.</p></div>
  </div>
</section>

{% include faq.html %}
