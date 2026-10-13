---
title: ShortGuid for .NET — 22-character URL-safe GUIDs
description: Encode a .NET Guid as a 22-character URL-safe string with Pineapple's ShortGuid, convert back and forth implicitly, and sort GUIDs the same way SQL Server does.
breadcrumb: ShortGuid
last_reviewed: 2026-10-13
faq:
  - q: What is a ShortGuid?
    a: A ShortGuid is a 128-bit Guid encoded as 22 URL-safe Base64 characters. Pineapple replaces '+' with '-' and '/' with '_' and drops the '==' padding, so the value is safe in URLs, file names, and route segments.
  - q: How do I convert a Guid to a short string in C#?
    a: "Use Pineapple's ShortGuid: `ShortGuid s = guid; string text = s;` or call `ShortGuid.Encode(guid)`. Convert back with `ShortGuid.Decode(text)` or `new ShortGuid(text).Guid`."
  - q: Why do GUIDs sort differently in SQL Server than in .NET?
    a: SQL Server's uniqueidentifier compares the last six bytes first, then the remaining groups in a specific order. Pineapple's GuidExtensions.SqlCompareTo and OrderByGuid reproduce SQL Server's order in .NET.
---

# `ShortGuid` and SQL Server GUID ordering

**`Pineapple.Common.ShortGuid` is a struct that represents a `Guid` as a 22-character, URL-safe string** — 14 characters shorter than the standard 36-character format — and converts implicitly to and from both `Guid` and `string`.

```csharp
using Pineapple.Common;

ShortGuid id = ShortGuid.NewGuid();
string text = id;                     // "xnS5yWvbWE2Ky1Ue7PZkZw"
Guid guid = id;                       // implicit Guid conversion

ShortGuid parsed = text;              // implicit from short string
ShortGuid parsed2 = guid.ToString();  // ...or from a standard Guid string

bool same = parsed == id;             // true
```

## Encoding rules

1. Take `Guid.ToByteArray()` (16 bytes).
2. Base64-encode it (24 characters).
3. Replace `/` with `_` and `+` with `-`.
4. Drop the trailing `==`.

## API

| Member | Purpose |
| --- | --- |
| `ShortGuid.NewGuid()` | New random ShortGuid |
| `ShortGuid.Empty` | Wraps `Guid.Empty` (`"AAAAAAAAAAAAAAAAAAAAAA"`) |
| `new ShortGuid(Guid)` / `new ShortGuid(string)` | Construct from a Guid, a 22-char short string, or a standard Guid string |
| `.Guid`, `.Value` | The underlying `Guid` and the 22-char string |
| `ShortGuid.Encode(Guid)` / `Encode(string)` | Guid → short string |
| `ShortGuid.Decode(string)` | Short or standard string → `Guid` |
| `ShortGuid.TryParse(string, out Guid)` | Non-throwing parse |
| `Equals(object)` | Equal to another `ShortGuid`, a `Guid`, or either string form |

## Sorting GUIDs like SQL Server

`Pineapple.Common.GuidExtensions` compares GUIDs using SQL Server's `uniqueidentifier` byte order, so in-memory sorting matches `ORDER BY` in the database.

```csharp
var ordered = rows.OrderByGuid(r => r.Id).ToList();
bool later = a.IsGreaterThan(b);      // also IsLessThan, IsGreaterThanOrEqual, IsLessThanOrEqual
int cmp = a.SqlCompareTo(b);
```

{% include faq.html heading="Questions about ShortGuid" %}
