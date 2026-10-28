---
title: Query SQL Server INFORMATION_SCHEMA with EF Core — Pineapple.Data
description: Query SQL Server INFORMATION_SCHEMA tables, columns, keys, and constraints with LINQ using Pineapple.Data's Entity Framework Core model.
breadcrumb: Pineapple.Data
last_reviewed: 2026-10-28
faq:
  - q: How do I list all tables and columns in SQL Server with Entity Framework Core?
    a: "Install Pineapple.Data and query `new InformationSchemaContext(connectionString).Columns`, which maps INFORMATION_SCHEMA.COLUMNS to a LINQ-queryable, no-tracking EF Core set."
---

# Pineapple.Data for SQL Server

```bash
dotnet add package Pineapple.Data --version 2.0.0
```

**`Pineapple.Data` adds SQL Server helpers on top of Pineapple.** It targets .NET 8 (EF Core 8) and .NET 10 (EF Core 10) and uses `Microsoft.Data.SqlClient` 7.

Both packages use version **2.0.0** and are released together. Installing `Pineapple.Data` also brings in `Pineapple`; if your project references both directly, use matching versions. The previous `Pineapple.Data` 1.0.1 targets .NET Core 3.1; only the core `Pineapple` 1.0.41 package targets .NET Standard 2.0.

## `InformationSchemaContext`

**An EF Core `DbContext` mapped to SQL Server's `INFORMATION_SCHEMA` views**, so you can explore a database's schema with LINQ. All queries are no-tracking.

```csharp
using Pineapple.Data.InformationSchema;

await using var schema = new InformationSchemaContext(connectionString);

var primaryKeys =
    from c in schema.TableConstraints
    where c.ConstraintType == "PRIMARY KEY"
    join k in schema.KeyColumns on c.ConstraintName equals k.ConstraintName
    orderby k.TableSchema, k.TableName, k.Position
    select new { k.TableSchema, k.TableName, k.ColumnName };

foreach (var pk in await primaryKeys.ToListAsync())
    Console.WriteLine($"{pk.TableSchema}.{pk.TableName}.{pk.ColumnName}");
```

| Set | View | Columns |
| --- | --- | --- |
| `Tables` | `INFORMATION_SCHEMA.TABLES` | `TableCatalog`, `TableSchema`, `TableName` |
| `Columns` | `INFORMATION_SCHEMA.COLUMNS` | … `ColumnName`, `DataType` |
| `KeyColumns` | `INFORMATION_SCHEMA.KEY_COLUMN_USAGE` | … `ColumnName`, `Position`, `ConstraintName` |
| `TableConstraints` | `INFORMATION_SCHEMA.TABLE_CONSTRAINTS` | … `ConstraintType`, `ConstraintName` |

## `SqlClient.Check.IsSqlServerAvailable`

> **Deprecated in 2.0 (PINE006).** Use `EnableRetryOnFailure` to wake serverless databases and `AspNetCore.HealthChecks.SqlServer` for health checks. See the [migration guide]({{ '/guides/migration/#PINE006' | relative_url }}).
{: .callout}

Parses the server and port (default 1433) from a connection string and probes the TCP endpoint up to three times — handy for waking a paused Azure SQL serverless database before the first query.

```csharp
using Pineapple.Data.SqlClient;

Check.IsSqlServerAvailable(connectionString);
```

> In 2.0.0 this method returns `true` after probing even if every attempt fails, so treat it as a wake-up call rather than a reliable check. Use `Pineapple.Health.Check.IsAlive(server, port)` when you need the result.
{: .callout}

{% include faq.html heading="Questions about Pineapple.Data" %}
