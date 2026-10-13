# Changelog

All notable changes to the `Pineapple` and `Pineapple.Data` packages.

## 2.0.0 — 2026-10

### Breaking changes

- Targets **.NET 8** and **.NET 10** only. Dropped `netstandard2.0` (and with it .NET Framework, .NET Core 3.1, and .NET 5). Stay on 1.0.41 if you need those.
- `Pineapple.Data` targets .NET 8 with EF Core 8 and .NET 10 with EF Core 10.
- `BrotliCompressionHandler` now uses `System.IO.Compression.BrotliStream`; the `BrotliSharpLib` dependency is gone. Decompressed responses keep their original content headers (except `Content-Encoding` and `Content-Length`).

### Changed

- `Health.Check.IsUrlAvailable` uses `HttpClient` instead of the obsolete `HttpWebRequest`.
- Dependencies upgraded: `K4os.Hash.xxHash` 1.0.8, `Microsoft.Extensions.Http` 10.0, `Microsoft.Data.SqlClient` 7.1, EF Core 8.0 / 10.0. Removed `Microsoft.Bcl.AsyncInterfaces`, `System.Collections.Immutable`, and `Microsoft.SourceLink.GitHub` (now built into the SDK).
- Packages include a README, XML documentation, SourceLink, deterministic builds, and symbol packages; license metadata is now the `Apache-2.0` SPDX expression.

### Fixed

- `ShortGuid.TryParse` always assigns its `out` parameter.
- `Pineapple.Data` no longer references both the `Pineapple` 1.0.40 NuGet package and the local project.

### Build

- Single `Pineapple.Tests` project (MSTest 4, Microsoft.Testing.Platform) replaces the shared-project test setup.
- `Pineapple.slnx`, central package management, `global.json`, GitHub Actions CI, a tag-triggered NuGet release using Trusted Publishing, and a documentation site at <https://mill5.github.io/pineapple/>.

## 1.0.41

- Rate limiting and resource governors. Last release targeting .NET Standard 2.0.
