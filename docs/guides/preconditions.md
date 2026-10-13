---
title: Guard clauses in C# with Pineapple Preconditions
description: Validate .NET method arguments in one line with Pineapple Preconditions — null, whitespace, numeric ranges, dates, URIs, GUIDs, and custom conditions.
breadcrumb: Guard clauses
last_reviewed: 2026-10-13
faq:
  - q: What is a guard clause in C#?
    a: A guard clause is a check at the top of a method that throws immediately when an argument is invalid, so the rest of the method can assume valid input. Pineapple's Preconditions class provides guard clauses such as CheckIsNotNull, CheckIsNotNullOrWhitespace, and CheckIsNotLessThan.
  - q: Which exceptions do Pineapple preconditions throw?
    a: Null checks throw ArgumentNullException, range checks throw ArgumentOutOfRangeException, and format or condition checks throw ArgumentException. CheckIsNotOnSynchronizationContext throws Exception.
---

# Guard clauses with `Preconditions`

**`Pineapple.Common.Preconditions` is a static class of one-line guard clauses that validate method arguments and throw standard `ArgumentException`-family exceptions.** Every check takes the parameter name first (use `nameof`) so error messages identify the bad argument. The design follows Guava's `Preconditions` for Java.

```csharp
using static Pineapple.Common.Preconditions;

public Invoice(string customerId, decimal amount, DateTime issuedOn, string webhookUrl)
{
    CheckIsNotNullOrWhitespace(nameof(customerId), customerId);
    CheckIsNotLessThanOrEqualTo(nameof(amount), amount, 0m);
    CheckIsValidDateRange(nameof(issuedOn), issuedOn, plusOrMinusInHours: 48);
    CheckIsWellFormedUri(nameof(webhookUrl), webhookUrl);
}
```

## Relationship to .NET's `ThrowIf` helpers

Since 2.0, the null, whitespace, and numeric checks call the helpers built into .NET 8 — `ArgumentNullException.ThrowIfNull`, `ArgumentException.ThrowIfNullOrWhiteSpace`, and `ArgumentOutOfRangeException.ThrowIfLessThan`, `ThrowIfLessThanOrEqual`, `ThrowIfGreaterThan`, `ThrowIfEqual`, `ThrowIfNotEqual`, and `ThrowIfNegative` — so exceptions carry the correct `ParamName` and standard messages. The date, URI, GUID, type, condition, and synchronization-context checks have no built-in equivalent.

## Null and string checks

| Method | Fails when | Throws |
| --- | --- | --- |
| `CheckIsNotNull(name, value)` | `value` is `null` | `ArgumentNullException` |
| `CheckIsNotNull<TException>(name, value)` | `value` is `null` | `TException` |
| `CheckIsNotNullOrWhitespace(name, value)` | `null`, empty, or whitespace | `ArgumentNullException` / `ArgumentException` |
| `CheckIsNotWhitespace(name, value)` | empty or whitespace (`null` allowed) | `ArgumentNullException` |
| `CheckDoesNotContainSpaces(name, value)` | contains `' '` | `ArgumentNullException` |
| `CheckEndsWith(name, suffix, value)` | does not end with `suffix` | `ArgumentNullException` |
| `CheckIsWellFormedUri(name, value, kind)` | not a well-formed URI | `ArgumentException` |
| `CheckIsNotWellFormedUri(name, value, kind)` | is a well-formed URI | `ArgumentException` |
| `CheckIsNotEmptyGuid(name, value)` | `Guid.Empty` | `ArgumentNullException` |
| `CheckIsType<T>(name, value)` | `value` is not a `T`; returns the cast value | `ArgumentException` |

## Numeric checks

Overloads exist for `int`, `long`, `ushort`, `uint`, `ulong`, and (for the less-than family) `decimal`.

| Method | Fails when |
| --- | --- |
| `CheckIsNotLessThan(name, value, min)` | `value < min` |
| `CheckIsNotLessThanOrEqualTo(name, value, min)` | `value <= min` |
| `CheckIsNotGreaterThan(name, value, max)` | `value > max` |
| `CheckIsEqualTo(name, value, expected)` | `value != expected` (`int`) |
| `CheckIsNotEqualTo(name, value, other)` | `value == other` (`int`) |
| `CheckIsNotNegative(name, value)` | `value < 0` (`int`, `long`) |
| `CheckIsNegative(name, value)` | `value >= 0` (`int`, `long`) |

All numeric checks throw `ArgumentOutOfRangeException` and include the offending value in the message.

## Dates

```csharp
// Within ±24 hours of DateTime.UtcNow
CheckIsValidDateRange(nameof(timestamp), timestamp, plusOrMinusInHours: 24);

// Within an explicit window
CheckIsValidDateRange(nameof(start), start, begin: fiscalYearStart, end: fiscalYearEnd);
```

## Custom conditions

```csharp
CheckIsCondition(nameof(order), order.Lines.Count > 0, "must have at least one line.");
CheckIsNotCondition(nameof(order), () => order.IsLocked, () => $"{order.Id} is locked.");
CheckNotExpression(nameof(state), state == State.Unknown);   // ArgumentException "state is invalid."
```

Each `CheckIsCondition` / `CheckIsNotCondition` has four overloads: `bool` or `Func<bool>` for the condition, and `string` or `Func<string>` for the message. Use the `Func` forms when computing the condition or message is expensive.

## Threading

```csharp
// Throws if called on a UI or legacy ASP.NET synchronization context.
CheckIsNotOnSynchronizationContext();
```

{% include faq.html heading="Questions about guard clauses" %}
