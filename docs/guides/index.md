---
title: Pineapple guides
description: Guides to every part of the Pineapple .NET library — guard clauses, ShortGuid, HashCode, RowVersion, timing, SQL Server, and migrating from deprecated APIs.
---

# Guides

Each guide explains what a Pineapple type is for, shows working C# code, and lists the API surface.

<ul class="guide-index">
  <li><h2><a href="{{ '/guides/preconditions/' | relative_url }}">Guard clauses (Preconditions)</a></h2><p>Namespace: <code>Pineapple.Common</code></p><p>Key types: <code>Preconditions</code></p></li>
  <li><h2><a href="{{ '/guides/shortguid/' | relative_url }}">ShortGuid and SQL GUID ordering</a></h2><p>Namespace: <code>Pineapple.Common</code></p><p>Key types: <code>ShortGuid</code>, <code>GuidExtensions</code></p></li>
  <li><h2><a href="{{ '/guides/hashcode/' | relative_url }}">Deterministic HashCode</a></h2><p>Namespace: <code>Pineapple.Common</code></p><p>Key types: <code>HashCode</code></p></li>
  <li><h2><a href="{{ '/guides/rowversion/' | relative_url }}">SQL Server RowVersion</a></h2><p>Namespace: <code>Pineapple.Data</code></p><p>Key types: <code>RowVersion</code></p></li>
  <li><h2><a href="{{ '/guides/rate-limiting/' | relative_url }}">Rate limiting and resource governors</a> (deprecated)</h2><p>Namespace: <code>Pineapple.Threading</code></p><p>Key types: <code>RateLimiterScope</code>, <code>SafeRateLimiterScope</code>, <code>ResourceGoverner</code></p></li>
  <li><h2><a href="{{ '/guides/timeouts-and-backoff/' | relative_url }}">Timeouts, backoff, and parallelism</a></h2><p>Namespace: <code>Pineapple.Threading</code></p><p>Key types: <code>AccurateTimeout</code>, <code>SimpleTimeout</code>, <code>ExponentialWait</code>, <code>Parallelism</code></p></li>
  <li><h2><a href="{{ '/guides/diagnostics/' | relative_url }}">Diagnostics and timing</a></h2><p>Namespace: <code>Pineapple.Diagnostics</code>, <code>Pineapple.Threading</code></p><p>Key types: <code>TimingScope</code>, <code>MeasureScope</code>, <code>ConsoleColorScope</code>, <code>CallsPerMinute</code></p></li>
  <li><h2><a href="{{ '/guides/cleanup-and-async/' | relative_url }}">Cleanup and async helpers</a></h2><p>Namespace: <code>Pineapple.Common</code>, <code>Pineapple.Threading</code>, <code>Pineapple.Extensions</code></p><p>Key types: <code>Cleanup</code>, <code>SynchronizationContextScope</code>, <code>ImmutableExtensions</code></p></li>
  <li><h2><a href="{{ '/guides/health-and-networking/' | relative_url }}">Health checks and networking</a> (deprecated)</h2><p>Namespace: <code>Pineapple.Health</code>, <code>Pineapple.Networking</code></p><p>Key types: <code>Check</code>, <code>BrotliCompressionHandler</code></p></li>
  <li><h2><a href="{{ '/guides/migration/' | relative_url }}">Migrating from deprecated APIs</a></h2><p>Namespace: —</p><p>Key types: <code>PINE001</code>–<code>PINE008</code> replacements</p></li>
  <li><h2><a href="{{ '/guides/data/' | relative_url }}">Pineapple.Data for SQL Server</a></h2><p>Namespace: <code>Pineapple.Data.*</code></p><p>Key types: <code>InformationSchemaContext</code>, <code>SqlClient.Check</code></p></li>
</ul>
