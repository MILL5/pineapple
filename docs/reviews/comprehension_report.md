## What this proposes

The document records Pineapple’s current architecture and the compatibility boundaries for future modernization. It preserves the two-package split, public APIs, K4os hashing, and existing persisted hash representation while identifying defects and validation gaps. Its work packages organize later implementation; unresolved behavioral choices explicitly prevent treating it as an implementation-ready specification.

## How the existing system works, per this document

Pineapple is a consumer-hosted utility library targeting .NET 8 and .NET 10, with SQL Server and EF Core helpers isolated in `Pineapple.Data`. Core utilities provide identifiers, deterministic hashes, validation, cleanup, process-local pacing, diagnostics, and HTTP integration. Consumers own configuration, credentials, resource sharing, and deployment, while CI and release workflows build, test, and publish packages. The document distinguishes intended contracts from acknowledged existing defects and unvalidated behavior.

## Findings

### C1 — Nit: The semaphore scope’s architectural role is unclear

- **Affected scope:** `design_draft.md:98`.
- **Discrepancy:** Calling `SemaphoreRateLimiterScope` “another scope implementation” and telling callers to choose an implementation leaves its purpose unexplained. The adjacent paragraphs explain how the other scopes pace or admit operations, but this paragraph supplies only disposal-interface information.
- **Concrete improvement:** Add one sentence identifying its intended role and current usability status. The source constructs a zero-count semaphore, waits during acquisition, and contains no release path; describe that as an existing incomplete implementation requiring validation, rather than presenting it as an ordinary selectable limiter. This is a clarity gap in describing existing code, not a new defect introduced by the proposal.

No Blocker or Major comprehension findings. The remaining unresolved decisions are clearly labeled as follow-up work, which is appropriate for this architecture document.

## Evidence sources

Confirmed complete reads of:

- `.agents/agents/comprehension.md`.
- `design_draft.md`, all 247 lines, including FeatureRange 1–222 and DetailedRange 223–247.

Additional read-only evidence:

- `docs/guides/rate-limiting.md`.
- `Pineapple/Threading/ResourceGoverners/ResourceGoverner.cs`.
- `Pineapple/Threading/RateLimiting/SemaphoreRateLimiterScope.cs`.

No files modified, builds run, history consulted, or memory accessed.
