---
title: Exception-safe cleanup and SynchronizationContext scopes in C#
description: Run cleanup code that must never throw with Cleanup.SafeMethod and avoid sync-over-async deadlocks with Pineapple's SynchronizationContextScope.
breadcrumb: Cleanup & async
last_reviewed: 2026-10-13
faq:
  - q: How do I avoid deadlocks when blocking on async code in .NET?
    a: "Prefer async all the way. When you must block (for example in a legacy UI or ASP.NET handler), clear the SynchronizationContext first so continuations don't try to resume on the blocked thread: `using (new SuppressSynchronizationContextScope()) { task.GetAwaiter().GetResult(); }`."
---

# Cleanup and async helpers

## `Cleanup.SafeMethod`

**`Pineapple.Common.Cleanup.SafeMethod` runs an action and swallows any exception**, returning a fluent object for follow-up actions. Use it in `finally` blocks and `Dispose` methods where a secondary failure must not mask the original one.

```csharp
using static Pineapple.Common.Cleanup;

SafeMethod(() => File.Delete(tempPath))
    .OnSuccess(() => _logger.LogDebug("Temp file removed"))
    .OnFailure(() => _logger.LogWarning("Could not remove {Path}", tempPath))
    .Continue(connection.Close);
```

| Method | Runs when |
| --- | --- |
| `OnSuccess(action)` | The previous action succeeded |
| `OnFailure(action)` | The previous action threw |
| `Continue(action)` | Always |

Every follow-up is itself run safely.

## `SynchronizationContextScope`

Temporarily replaces `SynchronizationContext.Current`, restoring the original on dispose. `SuppressSynchronizationContextScope` sets it to `null`.

```csharp
using Pineapple.Threading;

using (new SuppressSynchronizationContextScope())
{
    // Continuations run on the thread pool, not the captured UI/request context.
    var result = LoadAsync().GetAwaiter().GetResult();
}
```

Pair with `Preconditions.CheckIsNotOnSynchronizationContext()` to assert that code runs without a context.

## Immutable collection helpers

`Pineapple.Extensions.ImmutableExtensions` adds null-tolerant `Append` methods to `IImmutableList<T>` and `IImmutableDictionary<TKey, TValue>`:

```csharp
IImmutableList<Order> list = null;
list = list.Append(order);            // creates the list if null
list = list.Append(moreOrders);
```

## Dependency injection helpers

`Pineapple.Extensions.ServiceCollectionExtensions` adds `services.IsConfigured<T>()` (is a service of type `T` registered?) and `services.Configure<T>()` (register `T` as a singleton).

{% include faq.html heading="Questions about async helpers" %}
