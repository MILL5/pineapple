---
title: Pineapple for .NET — frequently asked questions
description: Answers about Pineapple, the Guava-inspired C# utility library for .NET 8 and .NET 10 — install, supported frameworks, license, and comparisons.
faq:
  - q: What is Pineapple?
    a: Pineapple is a free, open-source C# library from MILL5 that bundles core utilities for .NET 8 and .NET 10 — guard clauses (preconditions), ShortGuid, a deterministic xxHash HashCode, a SQL Server RowVersion type, rate limiting, resource governors, timeouts, exponential backoff, timing scopes, and exception-safe cleanup.
  - q: Is Pineapple a port of Google Guava to .NET?
    a: No. Pineapple is inspired by Guava's philosophy — small, well-named utilities that remove boilerplate — and borrows ideas such as Preconditions, but it is an independent .NET library with its own API.
  - q: How do I install Pineapple?
    a: "Run `dotnet add package Pineapple`, or add `<PackageReference Include=\"Pineapple\" Version=\"2.*\" />` to your project file. SQL Server helpers are in the separate `Pineapple.Data` package."
  - q: Which .NET versions are supported?
    a: Pineapple 2.x supports .NET 8 and .NET 10, the current long-term-support releases. Pineapple 1.0.41 is the last version for .NET Standard 2.0, .NET Framework, and .NET Core 3.1.
  - q: What license is Pineapple released under?
    a: The Apache License 2.0. You can use it in commercial and closed-source software; keep the license notice.
  - q: Does Pineapple have dependencies?
    a: The core package depends on K4os.Hash.xxHash, Microsoft.Extensions.Http, and Microsoft.Extensions.Logging.Abstractions. Pineapple.Data adds Microsoft.Data.SqlClient and Microsoft.EntityFrameworkCore.SqlServer.
  - q: Should I use Pineapple Preconditions or ArgumentNullException.ThrowIfNull?
    a: .NET 8+ includes ArgumentNullException.ThrowIfNull, ArgumentException.ThrowIfNullOrWhiteSpace, and ArgumentOutOfRangeException.ThrowIfLessThan and friends. Use them for simple cases. Pineapple's Preconditions add checks .NET does not have — date windows, URI format, empty GUID, type checks, custom conditions with lazy messages, and synchronization-context assertions.
  - q: Why use Pineapple's HashCode instead of System.HashCode?
    a: System.HashCode is randomized per process, so its values change between runs. Pineapple's HashCode uses xxHash32 and is deterministic, which makes it safe for persisted hashes, cache keys, and partitioning.
  - q: Is Pineapple thread-safe?
    a: Stateless helpers (Preconditions, ShortGuid encoding, GuidExtensions, Cleanup) are thread-safe. ResourceGoverner, CallsPerMinute, and SafeRateLimiterScope are designed for concurrent use. HashCode accumulator instances and scope objects are meant for a single caller.
  - q: Who built Pineapple?
    a: Pineapple was built by Richard Crane, a Microsoft Most Valuable Professional (MVP) and founder of MILL5, a Boston-based Microsoft Gold Partner specializing in cloud, AI, IoT, data analytics and modern application development.
  - q: What other open-source projects does MILL5 publish?
    a: MILL5's open-source family includes FastSearch (fast substring search for .NET), M5.BloomFilter, the yocto IoC container, Polygon.Net, and the macOS utilities Cut & Move and MacEdgeLight.
  - q: Where do I report bugs or request features?
    a: Open an issue at https://github.com/MILL5/pineapple/issues or email rich@mill5.com. Pull requests are welcome.
---

# Frequently asked questions

{% include faq.html heading="Pineapple FAQ" %}
