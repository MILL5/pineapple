---
title: Deterministic hash codes in .NET with xxHash
description: Pineapple's HashCode combines values into a 32-bit xxHash that is stable across application instances using the same byte encoding — unlike System.HashCode, which is randomized per process.
breadcrumb: HashCode
last_reviewed: 2026-10-28
faq:
  - q: Why does System.HashCode return different values each run?
    a: .NET randomizes System.HashCode and string.GetHashCode per process to defend against hash-flooding attacks, so values are not stable across runs or machines. Do not persist them.
  - q: How do I get a stable hash code in C#?
    a: "Use a deterministic hash such as xxHash. Pineapple's `HashCode.Combine(a, b, c)` uses xxHash32 and returns the same int for the same inputs across application instances using the same byte encoding."
---

# Deterministic `HashCode`

**`Pineapple.Common.HashCode` combines values into a 32-bit [xxHash](https://xxhash.com/) that is repeatable across application instances and restarts using the same byte encoding.** Use it when a hash must be stored or shared — cache keys, partition/shard keys, change detection — where `System.HashCode` (randomized per process) is unsuitable.

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

- Integer values are hashed in machine byte order (the persisted test vectors cover little-endian environments).
- `HashCode` instances are accumulators: `Equals` and `GetHashCode` on the instance are deliberately unsupported.

## Compatibility validation

K4os xxHash32 and the existing input encoding are retained. Fixed expected values and two independently launched processes validate persisted hashes on .NET 8, 9, 10, and 11. Numeric encodings use machine byte order; changing that encoding would change persisted values. Hashes can collide, and concatenated fields do not contain type or length boundaries. Retain the original identity when collisions matter; this is not cryptographic integrity.
