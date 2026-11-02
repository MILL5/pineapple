---
title: Query SQL Server INFORMATION_SCHEMA with EF Core — Pineapple.Data
description: Query SQL Server INFORMATION_SCHEMA tables, columns, keys, and constraints with LINQ using Pineapple.Data's Entity Framework Core model.
breadcrumb: Pineapple.Data
last_reviewed: 2026-10-13
faq:
  - q: How do I list all tables and columns in SQL Server with Entity Framework Core?
    a: "Install Pineapple.Data and query `new InformationSchemaContext(connectionString).Columns`, which maps INFORMATION_SCHEMA.COLUMNS to a LINQ-queryable, no-tracking EF Core set."
---

# Pineapple.Data for SQL Server

```bash
dotnet add package Pineapple.Data
```

**`Pineapple.Data` adds SQL Server helpers on top of Pineapple.** It targets .NET 8 (EF Core 8), .NET 9 (EF Core 9), and .NET 10/11 (stable EF Core 10) and uses `Microsoft.Data.SqlClient` 7.

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

Parses the server and port (default 1433) from a connection string and probes the TCP endpoint up to three times — handy for waking a paused Azure SQL serverless database before the first query.

```csharp
using Pineapple.Data.SqlClient;

Check.IsSqlServerAvailable(connectionString);
```

> In 2.0.0 this method returns `true` after probing even if every attempt fails, so treat it as a wake-up call rather than a reliable check. Use `Pineapple.Health.Check.IsAlive(server, port)` when you need the result.
{: .callout}

{% include faq.html heading="Questions about Pineapple.Data" %}

## Availability and integration testing

`IsSqlServerAvailable` checks TCP reachability, not SQL authentication or query readiness. It understands the `tcp:` prefix and an explicit `host,port`, defaults to port 1433, and returns false after three failed attempts. Each TCP attempt has a five-second timeout. Named-instance discovery is not performed.

Metadata entities map to the four `INFORMATION_SCHEMA` views using their composite identifying columns and no-tracking queries. Database permissions still determine what metadata callers can read.

Model and loopback connectivity tests run without SQL Server. To run the SQL integration fixture, set `PINEAPPLE_SQL_CONNECTION` to a disposable SQL Server database with create/drop table permissions, then run `dotnet test --project "$(pwd)/Pineapple.Data.Tests/Pineapple.Data.Tests.csproj" -c Release`. The fixture creates and drops a uniquely named table. Framework processes share a bounded session-owned SQL application lock around catalog DDL and reads; the unpooled owner connection releases that lock when it closes, including failure paths. Without that variable the integration test is explicitly inconclusive. Set `PINEAPPLE_REQUIRE_SQL_TESTS=1` to fail instead of skipping when configuration is absent. Linux SQL CI and the release job require this fixture on every target; ordinary Linux/Windows build jobs run the model tests and visibly skip database integration.
