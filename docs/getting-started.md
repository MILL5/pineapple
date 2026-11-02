---
title: Getting started with Pineapple for .NET
description: Install the Pineapple NuGet package, add guard clauses, ShortGuids, and rate limiting to a .NET 8, 9, 10, or 11 project in five minutes.
last_reviewed: 2026-10-13
---

# Getting started with Pineapple

> Compatibility APIs with platform replacements remain available through 2.x and report migration diagnostics. See the [migration guide](guides/migration.md).

**Pineapple installs from NuGet and needs no configuration.** Add the package, import a namespace, and call the helpers directly.

## 1. Install

```bash
dotnet add package Pineapple --version 2.0.0
```

Requirements: .NET 8, 9, 10, or 11. For SQL Server helpers, also add `Pineapple.Data` (see [Pineapple.Data]({{ '/guides/data/' | relative_url }})).

## 2. Validate arguments with guard clauses

Import `Preconditions` statically so checks read like assertions:

```csharp
using static Pineapple.Common.Preconditions;

public void Ship(string sku, int quantity, Uri callback)
{
    CheckIsNotNullOrWhitespace(nameof(sku), sku);       // ArgumentNullException / ArgumentException
    CheckIsNotLessThan(nameof(quantity), quantity, 1);  // ArgumentOutOfRangeException
    CheckIsNotNull(nameof(callback), callback);
}
```

## 3. Shorten identifiers with ShortGuid

```csharp
using Pineapple.Common;

ShortGuid id = Guid.NewGuid();
string slug = id;                 // 22 URL-safe characters
Guid back = new ShortGuid(slug);  // round-trips to the same Guid
```

## 4. Hash values the same way on every machine

```csharp
using HashCode = Pineapple.Common.HashCode;

int shard = (HashCode.Combine(tenantId, customerId) & 0x7fffffff) % 16;   // same shard after every restart
```

## 5. Time an operation

```csharp
using Pineapple.Diagnostics;

using (new TimingScope("LoadCustomers", logger))
{
    customers = await repository.LoadAsync();
}
// Debug log: Operation [LoadCustomers] called from [Handle] took 42.17ms
```

## Next steps

- Browse the [guides]({{ '/guides/' | relative_url }}) for every feature area.
- Read the [FAQ]({{ '/faq/' | relative_url }}).
- Upgrading from 1.x? See the [migration guide]({{ '/guides/migration/' | relative_url }}) and the [changelog]({{ '/changelog/' | relative_url }}).
