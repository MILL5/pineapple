---
title: SQL Server rowversion in C# with Pineapple RowVersion
description: Pineapple's RowVersion struct wraps SQL Server rowversion/timestamp values as a comparable 64-bit number with conversions to and from byte[], long, and ulong.
breadcrumb: RowVersion
last_reviewed: 2026-10-13
faq:
  - q: What is a rowversion in SQL Server?
    a: rowversion (formerly timestamp) is an 8-byte, database-wide, auto-incrementing binary value that SQL Server updates on every row change. It is used for optimistic concurrency and change tracking.
  - q: How do I compare two rowversion values in C#?
    a: "Convert the byte[] to Pineapple's RowVersion: `var v = (RowVersion)bytes;` Then use <, >, ==, CompareTo, or RowVersion.Max."
---

# `RowVersion` for SQL Server

**`Pineapple.Data.RowVersion` turns SQL Server's 8-byte `rowversion` into a comparable, sortable struct.** EF Core and ADO.NET surface `rowversion` columns as `byte[]`, which can't be compared with `<` or `>`; `RowVersion` can.

```csharp
using Pineapple.Data;

RowVersion current = (RowVersion)entity.Version;   // byte[] from EF Core
RowVersion watermark = lastSyncedVersion;          // implicit from long/ulong

if (current > watermark)
{
    await SyncAsync(entity);
    watermark = RowVersion.Max(watermark, current);
}

byte[] bytes = watermark;                          // implicit back to byte[] (big-endian)
Console.WriteLine(watermark);                      // "00000000000007d1"
```

## API

| Member | Purpose |
| --- | --- |
| `RowVersion.Zero`, `MinValue`, `MaxValue` | Constants |
| `(RowVersion)byte[]`, `(RowVersion?)byte[]` | Explicit from 8 big-endian bytes (`null` → `null` for the nullable form) |
| implicit from `long` / `ulong` | Construct from a number |
| implicit to `byte[]` | 8 big-endian bytes |
| `==`, `!=`, `<`, `<=`, `>`, `>=`, `CompareTo` | Comparison |
| `RowVersion.Max(a, b)` | Larger of two values |
| `ToString()` | 16-digit lowercase hex |

Converting a `byte[]` that isn't 8 bytes long (other than an empty array or `{ 0 }`) throws `ArgumentException`.
