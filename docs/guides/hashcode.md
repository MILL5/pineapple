---
title: Deterministic hash codes in .NET with xxHash
description: Pineapple's HashCode combines values into a 32-bit xxHash that is stable across processes and machines — unlike System.HashCode, which is randomized per process.
breadcrumb: HashCode
last_reviewed: 2026-10-13
faq:
  - q: Why does System.HashCode return different values each run?
    a: .NET randomizes System.HashCode and string.GetHashCode per process to defend against hash-flooding attacks, so values are not stable across runs or machines. Do not persist them.
  - q: How do I get a stable hash code in C#?
    a: "Use a deterministic hash such as xxHash. Pineapple's `HashCode.Combine(a, b, c)` uses xxHash32 and returns the same int for the same inputs on every machine and run."
---

# Deterministic `HashCode`

**`Pineapple.Common.HashCode` combines values into a 32-bit [xxHash](https://xxhash.com/) that is identical across processes, machines, and restarts.** Use it when a hash must be stored or shared — cache keys, partition/shard keys, change detection — where `System.HashCode` (randomized per process) is unsuitable.

```csharp
using HashCode = Pineapple.Common.HashCode;

int key = HashCode.Combine(tenantId, "orders", orderDate);   // up to 7 values

var hc = new HashCode();
foreach (var line in order.Lines)
{
    hc.Add(line.Sku);
    hc.Add(line.Quantity);
}
int linesHash = hc.ToHashCode();
```

> Alias the type (`using HashCode = Pineapple.Common.HashCode;`) to avoid a clash with `System.HashCode`.
{: .callout}

## Supported value types

`string` (UTF-8), `bool`, `char`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `decimal`, `DateTime` (by ticks), their nullable forms, and `null`. Other types throw `NotSupportedException` — hash their stable fields instead.

## Notes

- Integer values are hashed in machine byte order (little-endian on every platform .NET 8+ supports).
- `HashCode` instances are accumulators: `Equals` and `GetHashCode` on the instance are deliberately unsupported.
