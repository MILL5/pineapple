# Critic review

- Stage: 7; three independent fresh-context Goldfish reviewers.
- Target: `design_draft.md:1–222`; lines 223–247 used as context.
- Result: 3 Major findings; structural review clean. D1 duplicates Z1 from Stage 5.
- All reviewers confirmed reading their personas and the target. Design/structural reviewers also read `feature-spec-standard.md`; style has no additional standard.

## Design reviewer

### D1 — Major — Missing semaphore acquisition limitation

Target: lines 98 and 210–221. Source: `Pineapple/Threading/RateLimiting/SemaphoreRateLimiterScope.cs:16–18,21–28,31–36`.

The semaphore starts with zero permits, acquisition waits synchronously, and no disposal path releases a permit. First acquisition cannot complete normally. Presenting the type as an ordinary selectable limiter omits an existing defect.

Improvement: describe the unsupported acquisition path, then record a decision about permit initialization, replenishment, and scope ownership. Require bounded acquisition and disposal tests. This does not require replacing the limiter framework.

### D2 — Major — Brotli resource risk has no usable mitigation

Target: lines 108, 114, 206, and 219. Source: `Pineapple/Networking/BrotliCompressionHandler.cs:23–29,34–45`.

The handler buffers decoded responses without a byte limit. A configured destination can still return an unexpectedly large or highly compressed response. Destination validation and cancellation do not enforce a memory bound.

Improvement: explicitly limit current safe use to trusted, bounded payloads or externally enforced size/concurrency bounds. Bounded decoding is required before supporting unbounded or untrusted payloads; an implementation may remain deferred. An on-wire compressed-byte limit alone is not a decoded-byte limit.

## Structural reviewer

No findings. Required content is present: problem, goals/non-goals, architecture, alternatives, and risks/limits. The supplied standard does not require exact heading wording. H1/H2/H3 nesting, tables, lists, Mermaid fences, and links are well formed. All local link targets exist. Reviewed the full target, README, documentation index organization, and referenced local paths.

## Style reviewer

### T1 — Major — Audit claims lack reproducible evidence

Target: lines 135–137.

The phrases “graphs inspected during this session” and “audit … reported” do not establish reproducible evidence for OWIN absence or vulnerable package versions.

Improvement: record dated audit commands, restored versions per target, and a durable evidence reference. Replace session-relative wording with the dated inspection scope. Suggested shape: “On 2026-11-02, command X reported package/version Y for target Z; see evidence E.”

No other material style findings. Inspected handler, rate limiter, SQL availability, and release workflow source.

## Evidence boundaries

The design reviewer additionally inspected all project files, central versions, SDK configuration, CI/release, hash implementation/conversions, HTTP integration, health/SQL helpers, schema context, limiters, and governor operation tracking. Existing acknowledged SQL, EF, governor, and Monitor concerns were not counted again. All reviews were read-only, without builds, history, or memory.
