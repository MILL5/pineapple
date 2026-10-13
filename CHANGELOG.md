# Changelog

All notable changes to the `Pineapple` and `Pineapple.Data` packages.

## 2.0.0 — 2026-10

### Breaking changes

- Targets **.NET 8** and **.NET 10** only. Dropped `netstandard2.0` (and with it .NET Framework, .NET Core 3.1, and .NET 5). Stay on 1.0.41 if you need those.
- `Pineapple.Data` targets .NET 8 with EF Core 8 and .NET 10 with EF Core 10.
- `BrotliCompressionHandler` now uses `System.IO.Compression.BrotliStream`; the `BrotliSharpLib` dependency is gone. Decompressed responses keep their original content headers (except `Content-Encoding` and `Content-Length`).

### Deprecated

APIs that .NET now provides are marked `[Obsolete]` with their own diagnostic IDs and will be removed in 3.0. See the [migration guide](https://mill5.github.io/pineapple/guides/migration/).

- `PINE001` `BrotliCompressionHandler`, `AddBrotliCompression` → `AutomaticDecompression`
- `PINE002` `ResourceGoverner`, `RateLimiterScope`, `SafeRateLimiterScope`, `SemaphoreRateLimiterScope` and related types → `System.Threading.RateLimiting`
- `PINE003` `AccurateTimeout`, `SimpleTimeout`, `ExponentialWait` → cancellation, `Task.WaitAsync`, Polly
- `PINE004` `MeasureScope`, `CallsPerMinute` → `System.Diagnostics.Metrics`
- `PINE005` `Scavenger` → `PeriodicTimer`
- `PINE006` `Health.Check`, `Data.SqlClient.Check` → `Microsoft.Extensions.Diagnostics.HealthChecks`
- `PINE007` `ImmutableExtensions` → `ImmutableList.Add`/`AddRange`
- `PINE008` `ServiceCollectionExtensions` → `TryAddSingleton`

### Changed

- `HashCode` uses `System.IO.Hashing.XxHash32` instead of `K4os.Hash.xxHash` and no longer boxes primitives. Hash values are unchanged (verified against recorded 1.x values).
- `Preconditions` null, whitespace, and range checks call the .NET 8 `ThrowIf*` helpers. Exception types are unchanged; `ParamName` is now always set and messages are the standard .NET ones.
- `ShortGuid` encodes and decodes with `Base64Url` (same output). `GuidExtensions` compares with `SqlGuid` (same order).
- `RowVersion` uses `BinaryPrimitives`; `TimingScope` writes a source-generated structured log event.
- `Health.Check.IsUrlAvailable` uses `HttpClient` instead of the obsolete `HttpWebRequest`.
- Dependencies upgraded: `Microsoft.Extensions.Http` 10.0, `Microsoft.Data.SqlClient` 7.1, EF Core 8.0 / 10.0. Removed `K4os.Hash.xxHash`, `Microsoft.Bcl.AsyncInterfaces`, `System.Collections.Immutable`, and `Microsoft.SourceLink.GitHub`. Added `System.IO.Hashing` (and `Microsoft.Bcl.Memory` on .NET 8).
- Packages include a README, XML documentation, SourceLink, deterministic builds, and symbol packages; license metadata is now the `Apache-2.0` SPDX expression.

### Fixed

- `ResourceGoverner` could exceed its calls-per-minute limit: it measured a one-minute window fixed before waiting, so stale calls stretched the measured span, and it woke up late from up-to-10-second backoffs. It now schedules calls one interval (60 s ÷ limit) apart, waits exactly until the next slot, and uses UTC so daylight-saving changes can't distort timing. `CallsPerMinute` records the same timestamps as the governor and no longer overstates the rate (n calls span n − 1 intervals).
- `default(ShortGuid)` no longer throws `NullReferenceException`; it equals `ShortGuid.Empty`.
- Converting a `byte[]` of the wrong length to `RowVersion` throws `ArgumentException` instead of `IndexOutOfRangeException`.
- `Preconditions`: `CheckIsNegative` and `CheckIsNotWellFormedUri` had wrong messages; `CheckIsType` and parameter-name validation swapped message and parameter name; condition checks now set `ParamName`.
- `ShortGuid.TryParse` always assigns its `out` parameter.
- `Pineapple.Data` no longer references both the `Pineapple` 1.0.40 NuGet package and the local project.

### Build

- Single `Pineapple.Tests` project (MSTest 4, Microsoft.Testing.Platform) replaces the shared-project test setup.
- `Pineapple.slnx`, central package management, `global.json`, GitHub Actions CI, a tag-triggered NuGet release using Trusted Publishing, and a documentation site at <https://mill5.github.io/pineapple/>.

## 1.0.41

- Rate limiting and resource governors. Last release targeting .NET Standard 2.0.
