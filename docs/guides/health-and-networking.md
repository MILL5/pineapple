---
title: Health checks and Brotli decompression for .NET HttpClient
description: Check if a TCP port, URL, or process is available with Pineapple's Health.Check, and decompress Brotli responses in HttpClient pipelines.
breadcrumb: Health & networking
last_reviewed: 2026-10-13
faq:
  - q: How do I check if a TCP port is open in C#?
    a: "Call `Pineapple.Health.Check.IsAlive(host, port)`. It attempts a TCP connection and returns true if it succeeds."
  - q: How do I enable Brotli decompression for HttpClient?
    a: "The simplest option is `new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All }`. If you build HttpClient pipelines with IHttpClientFactory and DelegatingHandlers, add Pineapple's handler with `services.AddHttpClient(\"api\").AddBrotliCompression(services)`."
---

# Health checks and networking

> Compatibility APIs with platform replacements remain available through 2.x and report migration diagnostics. See the [migration guide](migration.md).

## `Health.Check`

> **Deprecated in 2.0 (PINE006).** Use `Microsoft.Extensions.Diagnostics.HealthChecks` with the `AspNetCore.HealthChecks.*` packages. See the [migration guide]({{ '/guides/migration/#PINE006' | relative_url }}).
{: .callout}

**`Pineapple.Health.Check` provides quick yes/no probes for dependencies** — useful in startup checks, CLI tools, and test fixtures.

```csharp
using Pineapple.Health;

bool dbPortOpen = Check.IsAlive("sql.contoso.internal", 1433);       // TCP connect
bool siteUp     = Check.IsUrlAvailable("https://contoso.com/health", timeoutInMs: 5_000); // HTTP 200
bool running    = Check.IsRunning("dotnet");                          // process name contains "dotnet"
bool apiUp      = new Uri("https://api.contoso.com/ping").IsUrlAvailable(2_000);
```

All probes return `false` instead of throwing when the target is unreachable. For ASP.NET Core readiness and liveness endpoints, use these inside a [`IHealthCheck`](https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks).

## `BrotliCompressionHandler`

> **Deprecated in 2.0 (PINE001).** Set `AutomaticDecompression = DecompressionMethods.All` on the primary handler instead. See the [migration guide]({{ '/guides/migration/#PINE001' | relative_url }}).
{: .callout}

**A `DelegatingHandler` that decompresses `Content-Encoding: br` responses** with `System.IO.Compression.BrotliStream`, for `IHttpClientFactory` pipelines.

```csharp
using Pineapple.Networking;

builder.Services
    .AddHttpClient("catalog", c => c.BaseAddress = new Uri("https://catalog.contoso.com"))
    .AddBrotliCompression(builder.Services);
```

Remember to send `Accept-Encoding: br` on requests. If you control the primary handler, `HttpClientHandler.AutomaticDecompression = DecompressionMethods.Brotli` achieves the same thing without a delegating handler.

{% include faq.html heading="Questions about health checks and networking" %}

## Bounds and ownership

TCP probes have a five-second connection timeout and return false on connection failure. URL checks succeed only for HTTP 200.

Brotli responses are buffered with a default decoded-size limit of 64 MiB. Larger responses throw `HttpRequestException`; cancellation and content-read failures dispose the received response. To choose another positive byte limit, construct `new BrotliCompressionHandler(maxDecodedBytes)` when registering the handler. Copied content headers exclude the old encoding and length. Callers own request concurrency, so the per-response bound is not a process-wide memory budget.
