# Pineapple Goldfish review and improvements

- Date: 2026-11-02.
- Reviewed target: the original 247-line `design_draft.md`.
- Reviewed SHA-256: `41d239ae74a115a3f1b17ef903011b1b40b5b27138d89ae2e5fac71ecc94430b`.
- Repository HEAD: `435ef7122bb93d4ddbe4a63fe708789faae1148b`; draft and review reports are uncommitted files.
- Method: all five EGM stages completed in sequence, using nine fresh-context reviewer invocations. Critic and readiness panels used three independent reviewers each. No reviewer received the parent conversation or consulted memory; reviewers confirmed persona, target, and applicable standard reads.
- Boundary: source inspection and document review, not executed runtime, database, CI, or release acceptance.

## Verdict

The architecture overview explains the package boundaries and compatibility requirements. The implementation section is a roadmap, not an executable specification: material behavior and test-infrastructure decisions remain open. Do not begin a behavior-changing work package until its own decisions and acceptance tests are specified. Unrelated open decisions need not block independent work.

Retain K4os xxHash and the persisted hash representation. No OWIN integration is needed. `Microsoft.Extensions.Http` is used by public client-builder extensions and must not be removed as unused.

## Requirement added after the reviewed snapshot

The owner clarified that compatibility must cover **.NET 8, 9, 10, and 11**, including STS releases and successive current .NET versions. The original draft described only the current .NET 8/10 targets. The draft now includes the four-framework matrix, intended target declarations, runtime installation requirements, consumer validation, and the necessary project/CI/release/documentation scope.

This addition was made after the nine reviews. Their findings refer to the original line numbers and do not establish acceptance of the revised framework design. The installed local toolchain has only SDK 10.0.403 and .NET runtime 10.0.14; execution on 8, 9, and 11 remains unverified.

As of 2026-11-02, [.NET 11 is in release candidate](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/overview); [GA is scheduled for November 10](https://github.com/dotnet/core/blob/main/release-notes/11.0/README.md). [.NET 8/9 Microsoft servicing ends November 10](https://devblogs.microsoft.com/dotnet/dotnet-8-9-end-of-support/). The requested Pineapple compatibility coverage remains required; runtime servicing status must be stated separately.

## Prioritized improvements

### A0 — Required scope correction: validate every requested .NET version

The design now records .NET 8/9/10/11 coverage. Implementation must extend the project targets, provider-version mapping, SDK configuration, matching runtime installation, Linux/Windows CI, release validation, and consumer package tests. Include hash-result equivalence across the four runtimes. Test execution must not silently roll forward to another major. Revalidate .NET 11 with the stable SDK/runtime at GA.

**Applied:** design requirement and work-package scope only. **Not applied:** project, dependency, SDK, or workflow changes. This is new owner direction rather than a retroactive Goldfish finding.

### A1 — Blocker: specify behavior before concurrency and SQL fixes

Source problems are acknowledged, but the plan does not determine the replacement behavior. Before implementing the affected package:

- SQL availability: decide whether the method means TCP reachability or authenticated SQL readiness; define false/exception outcomes, retry policy, and timeout bounds. Existing unconditional true is a defect, not a contract to retain.
- Safe limiter: define synchronization, shared token ownership/lifetime, conflicting configurations, cancellation, and repeated or asynchronous disposal. A thread-owned `Monitor` cannot safely span an arbitrary await.
- Governor: define one idempotent completion path, started-versus-completed count semantics, empty-average result, cleanup lifecycle, retention, and refreshed admission windows. Calling `Operations.Remove` from disposal without restructuring can recurse because `Remove` already calls disposal.

**Acceptance:** each behavior has an explicit expected result and bounded tests covering contention, failures, and disposal. Preserve public names. Do not infer a new algorithm from the word “modernize.”

### A2 — Major: define SQL model repairs and test execution across every job

The context exposes `Table` but configures only the other three metadata entities. Specify `INFORMATION_SCHEMA.TABLES` registration, properties, and a deliberate key/keyless mapping. Define a fixture with known tables, columns, keys, and constraints and assertions for every exposed query.

Specify SQL Server service version, readiness, connection configuration, fixture setup/cleanup, test classification, and exact commands. Cover local runs, both CI operating systems, release jobs, and the expanded .NET matrix. Existing CI and release both run solution tests without SQL; adding tests to the solution alone leaves failures or silent exclusions to chance. Reuse current workflows and include `release.yml` when changing test filtering/provisioning.

**Acceptance:** model tests run without a server; integration runs visibly execute against a provisioned server. Jobs that exclude integration tests identify the required external evidence before publishing. Missing infrastructure is never represented as a passing integration test.

### A3 — Major: make persisted-hash testing executable and reuse existing vectors

The current test suite already contains fixed signed results: `HashCodeTests.cs:152` expects `-1492178786`, line 164 expects `-1487638711`, and line 176 expects `-1523258412`. Extend these rather than treating fixed-vector coverage as absent.

Enumerate additional supported-type, null, empty, Unicode, multi-block, combined, incremental, repeated-digest, and continued-update cases. State the supported byte-order environments. Preserve K4os and current conversions; do not silently canonicalize bytes or introduce random seeds.

For the cross-process probe, specify build dependency, matching target framework, output discovery, arguments, output format, exit handling, timeout/termination, and exact expected values. Merely adding the probe to the solution does not make its executable available to tests.

Update both `README.md` and `docs/guides/hashcode.md`: the packaged README's universal “every machine” claim is broader than the architecture's machine-byte-order limitation.

**Acceptance:** separately started processes on each requested runtime produce the established expected results; no implementation hash-format change occurs; documentation agrees on the guarantee's scope.

### A4 — Major: record reproducible dependency evidence and exact proposed changes

Replace session-relative assertions with dated commands, restored package versions per framework, and durable results. Before implementation, choose exact compatible package versions and any explicit override strategy, then specify the expected graph and vulnerability/deprecation result. Repeat checks after restore. Preserve the hash and HTTP dependencies justified above.

Earlier session audit artifacts remain available in `/tmp/pineapple-audit-*.json`; these are not durable repository evidence. The parent inspected their JSON during consolidation but did not rerun the audit. The vulnerable snapshot recorded:

| Project | Framework | Package | Resolved version | Reported advisories |
| --- | --- | --- | --- | --- |
| Pineapple.Data | net8.0 | System.Security.Cryptography.Pkcs | 8.0.1 | GHSA-846f-f764-76mx (High), GHSA-2fmv-rv2c-59x7 (Moderate) |
| Pineapple.Data | net10.0 | System.Security.Cryptography.Pkcs | 9.0.18 | Same two advisories |

Reproduce with `dotnet list Pineapple.slnx package --vulnerable --include-transitive --format json`; also run the corresponding `--deprecated` audit. Capture the complete graph with `dotnet list Pineapple.slnx package --include-transitive --format json` to establish OWIN absence and package boundaries. These commands describe evidence collection, not a completed clean audit. Extend them to net9.0/net11.0 after target expansion.

### A5 — Major: document the unusable semaphore acquisition path

`SemaphoreRateLimiterScope` creates a private semaphore with zero permits, synchronously waits on acquisition, and has no permit-release path. The architecture should not present it as an ordinary selectable limiter.

**Improvement:** add an explicit current-use warning and a follow-up decision about intended initialization, replenishment, ownership, and bounded acquisition/disposal tests. A warning is immediately actionable; implementing a replenishment framework is a separate behavioral decision.

### A6 — Major: define the safe-use boundary for Brotli decoding

The handler buffers decoded content without a decoded-byte bound. A configured destination, cancellation token, or compressed transfer-size limit does not prevent excessive decoded allocation.

**Improvement:** document that current use requires trusted, bounded payloads or enforced decoded-size/concurrency limits. Require bounded decoding before claiming support for unbounded or untrusted payloads. Do not introduce streaming infrastructure solely to complete this architecture document.

### A7 — Major: turn selected work packages into stable implementation phases

Order independent work with explicit prerequisites. Each phase must leave the solution buildable and define test commands and expected assertions across its applicable runtimes. Establish the data-test/mapping sequence so an intermediate phase cannot accidentally add failing integration tests to every job. Define packed target/dependency groups and temporary-consumer expectations.

Use “Execute a work package only after resolving the behavioral decisions that affect it.” This avoids blocking documentation or independent security updates on unrelated API decisions.

## Stage evidence and traceability

| Stage | Complete | Report | Findings |
| --- | --- | --- | --- |
| 5 Initial Zoom Out | Yes | [zoom_out_report.md](zoom_out_report.md) | Z1, Z2, Z3 |
| 6 Comprehension | Yes | [comprehension_report.md](comprehension_report.md) | C1; both full paraphrases retained |
| 7 Critic panel | Yes | [critic_report.md](critic_report.md) | D1, D2, T1; structural review clean |
| 8 Intermediate Zoom Out | Yes | [intermediate_zoom_out_report.md](intermediate_zoom_out_report.md) | I1–I5, all dispositions/duplicates |
| 9 Readiness panel | Yes | [implementation_readiness_report.md](implementation_readiness_report.md) | R1–R6, K1–K4, U1 |

| Consolidated action | Raw findings | Priority rationale |
| --- | --- | --- |
| A0 | Post-review owner requirement | Required scope expansion, not reviewed in the original snapshot. |
| A1 | R1, R4, K1 | R1 is a blocker to implementing the affected behavior; accounting findings identify additional required choices. |
| A2 | Z2, I3, R5, K2, K4 | Model correctness and SQL test/release execution are unspecified. |
| A3 | Z3, I5, R3, K3 | Fixed-vector/probe acceptance is incomplete; README parity is a related Nit. |
| A4 | T1, I4, R2 | Audit claims and dependency changes lack durable, executable evidence. |
| A5 | Z1, C1, D1, I1 | Repeated finding consolidated once; comprehension rated it Nit, architectural reviewers Major. |
| A6 | D2, I2 | Memory risk has no operational mitigation boundary. |
| A7 | R6, U1 | Stable phases and precise package-level prerequisites are missing; wording correction is Nit. |

At review completion, no production files or dependencies had changed, and the findings remained open. Only the framework requirement had been added at that point. The following implementation appendix records subsequent changes and actual test evidence.

## Implementation follow-through — 2026-11-02

The implementation addresses A0–A7 locally. Production changes preserve K4os and its existing hash bytes/results; there is no OWIN dependency.

| Area | Implemented contract |
| --- | --- |
| Frameworks and dependencies | Both libraries, both test projects, and the hash probe target net8.0/net9.0/net10.0/net11.0. SDK 11 RC3 is configured. Stable EF providers are 8.0.33, 9.0.22, and 10.0.14, with EF10 retained for net11 until EF11 GA. HTTP/logging and Pkcs use 10.0.14; MSTest uses 4.5.2. Vulnerability warnings fail restore. |
| SQL | TCP reachability returns false after failed attempts; tcp prefix and explicit ports work. Three attempts, five seconds each. No authenticated-readiness claim. All four schema entities map to INFORMATION_SCHEMA views with composite keys. |
| Token scopes | Semaphore synchronization supports disposal across awaits, rejects conflicting active rates, retires unused token state, and releases once. Existing synchronous acquisition remains; no FIFO/cancellation contract was added. |
| Concurrency scopes | Permits start at configured capacity. Operation disposal releases once. Owner shutdown rejects new callers, unblocks waiters, and lets admitted scopes drain safely. |
| Governors | Monotonic admission deadlines replace backoff under-utilization. First admission is immediate; starts are spaced without accumulated idle-time burst allowance. Completion/accounting is idempotent; averages use completed durations and return zero when empty. Cleanup timers are started and disposed. |
| Hash compatibility | Existing fixed vectors extended with supported-type and multi-block values; a framework-matched executable is launched twice and compared with fixed signed results. K4os source and input encodings were not changed. |
| ShortGuid | Final-symbol unused Base64 bits are normalized to preserve previously accepted aliases under .NET11 stricter decoding. Fixed GUID values captured on .NET8 pass on all four runtimes; canonical encoding is unchanged. |
| HTTP decoding | Default decoded-size cap is 64 MiB, configurable through a positive byte-limit constructor. Oversized content throws HttpRequestException; cancellation and content-read failures dispose response/buffer resources. |
| CI/release | Four-runtime Linux/Windows build definitions, required SQL integration in a Linux job and release, consumer-package validation, and package audits. SQL project invocation uses an absolute path to avoid the pinned SDK11 RC relative-project-path bug. |

### Executed verification

- Release solution build: **passed, zero errors**. Eight existing assertion-order warnings remain in ShortGuid tests.
- Initial full solution run: 612 cases, 598 passed, 10 failed, 4 expected Visual Studio skips. Those failures exposed governor under-utilization and .NET11 legacy ShortGuid decoding and were fixed.
- Governor re-test: every governor rate/timeout case passed on all four runtimes. That intermediate run still contained two ShortGuid failures from the pre-fix compiled snapshot; the final ShortGuid rebuild and tests below supersede those failures. Filtered-out data modules were not counted as SQL acceptance.
- Final rebuilt core regression selection: **144 passed, zero failed, four expected Visual Studio skips**, covering ShortGuid and fixed aliases, health, cross-process hashes, limiter lifecycles, and Brotli bounds on every target.
- Final required SQL suites: **12 passed, zero failed or skipped**, against an actual disposable SQL Server2022 container, across all four runtimes.
- Both NuGet packages pack successfully with four framework assets/dependency groups. Temporary consumers use a private cache and source mapping that forces local Pineapple artifacts; restore/build/run passed on runtimes 8.0.33, 9.0.22, 10.0.14, and 11.0.0-rc.3.26522.148. Consumer checks include fixed hashes, legacy ShortGuid identity, rowversion conversion, and EF metadata model initialization.
- Final direct/transitive dependency audits: **zero known vulnerabilities and zero deprecated packages**. K4os 1.0.8 is retained by requirement. No OWIN package found.
- Fresh source reviews found shutdown, cleanup, and package-provenance issues; fixes were re-reviewed with no remaining material findings. Actionlint and shell syntax validation passed. Markdown body validation passed; Jekyll front matter was excluded from the body-only lint to avoid the existing title/H1 rule conflict. Whitespace checks allow the repository's existing CRLF convention.

At the initial implementation handoff, the complete slow suite had not yet been repeated after the ShortGuid-only production fix; the continuation below closes that validation gap. Hosted CI, publication, and stable .NET11 acceptance remain separate gates.

### Reproduction and local evidence

The isolated toolchain is `/tmp/pineapple-dotnet`, containing SDK 11.0.100-rc.3.26522.148 and all four matching runtimes. Set `DOTNET_ROOT=/tmp/pineapple-dotnet` and place that directory first on PATH to reproduce. The system-wide SDK was not replaced.

Key logs are `/tmp/pineapple-tests.log` (initial full snapshot), `/tmp/pineapple-tests-targeted-final.log` (governor re-test), `/tmp/pineapple-tests-fast-final.log`, `/tmp/pineapple-tests-sql-final.log`, `/tmp/pineapple-consumer-smoke.log`, and `/tmp/pineapple-post-vulnerable.json` / `/tmp/pineapple-post-deprecated.json`. Packed artifacts are in `/tmp/pineapple-packages-final`. These are local evidence paths, not committed deliverables or hosted CI results. The owned SQL container and temporary credential state were removed after verification.

## Continuation: final-source full acceptance

The final uninterrupted solution run passed on 2026-11-02: **644 tests, 640 passed, zero failed, four expected Visual Studio skips**, in 6m56s. Each of net8.0/net9.0/net10.0/net11.0 passed157core tests plus one expected skip and3data tests. All **12 required SQL integration/model/connectivity tests passed**, with no SQL skips.

The full run initially exposed cross-framework SQL catalog deadlocks. The fixture now uses a bounded session-owned SQL application lock around create/query/drop, with an unpooled owner connection so session disposal releases the lock even on failure. This changes test isolation, not production query behavior.

Comparison with existing modernization PR203 identified a compatibility regression in the initial implementation: delaying the governor's first permit. **The first permit is restored to immediate admission.** Subsequent starts remain monotonic and spaced without accumulated idle credit. Timed throughput correctly measures N−1 intervals between N starts, retaining its real elapsed-time upper-bound and efficiency checks. Deterministic tests cover ceiling intervals, immediate first admission, concurrent spacing, ordered statistics, and equal timestamps; diagnostic UTC estimates are not used as admission guarantees.

The package manifest verifier from PR203 was retained and extended to all four targets. It checks both package IDs/shared versions, exact library assets and dependency groups, and matching Pineapple dependency versions. Consumer validation invokes it. Negative checks confirmed rejection of missing net11 assets and mismatched core dependency versions.

- Release build: zero errors; eight existing assertion-order warnings.
- Final full command: `dotnet test --solution Pineapple.slnx -c Release --no-build`, with required SQL configuration.
- Source fingerprint before/after: `540276d015da953089eff98465990785a6a79a70330ec5c9ab145cb3dbefa2a5`; no source/build-file changes during the run.
- Refreshed pack and isolated consumers: pass on all four matching runtimes, including persisted hashes and legacy ShortGuid values.
- Refreshed direct/transitive audits: zero vulnerable or deprecated packages. K4os source remains unchanged.
- Actionlint, shell syntax, manifest validation, and whitespace checks passed. The owned SQL container and temporary credential state were removed.
- Evidence: `/tmp/pineapple-full-final-build.log`, `/tmp/pineapple-full-final-tests.log`, and `/tmp/pineapple-full-final-source-before.sha256` / `-after.sha256`.

### Local integration boundary

This work is isolated on `feature/net8-11-modernization`, based on `origin/main`, preserving the unrelated author-card commit on `docs/author-card`. No remote PR or branch was changed.

Existing [PR203](https://github.com/MILL5/pineapple/pull/203), head `d8cb13c3865edaabfaebeb86bb644cc9744d81e4`, requires deliberate reconciliation before updating it. Its replacement of K4os conflicts with the owner's explicit requirement; its unrelated value-type fixes, tests, and API deprecation policy must not be silently overwritten or imported. Its [failed Sonar job](https://github.com/MILL5/pineapple/actions/runs/40685560733/job/123593305350) fails during scanner initialization with an authentication error, before analysis, and its required review is still outstanding. No review was manually requested, no gate was bypassed, and no package was published.

## Shipping reconciliation

The user requested consolidation/check-in with no outstanding PRs, then explicitly retired both SonarQube and SonarCloud. PR203 now preserves all five open heads: the .NET8–11 implementation, author card from211, and dependency updates208–210. K4os and its byte encoding remain intact; the earlier default-ShortGuid, RowVersion validation, SQL GUID ordering, golden-vector tests, migration documentation, CodeQL, and weekly dependency policy are retained.

The Sonar workflow and unused repository SONAR_TOKEN secret were removed. No Sonar application installation or Sonar webhook was found for MILL5/pineapple; the unrelated ZenHub webhook was preserved. Historical failure evidence above is retained as history and is not an active Sonar prerequisite.

CodeQL now builds all four target frameworks. SQL tests retain bounded session-lock isolation. Package verification checks all four library/dependency groups and synchronized versions. The reconciled solution builds with zero warnings/errors; fast regression tests pass756cases across all four targets with four expected skips. Full local and hosted CI validation follows on the consolidated head.

## Consolidated final validation

After reconciling all five PR heads, the full solution passed **808tests:804passed, zero failed, four expected Visual Studio skips**, in6m57.73s. Every target passed198core cases plus one expected skip and3data cases; required SQL acceptance passed12/12. Build:zero warnings/errors. Pack, four-runtime isolated consumers, direct/transitive audits, and actionlint also passed.

The tested source/build-file fingerprint remained `e052c6d22a416260677cecaf4983ecc705aa5c40337f029325d7286297c6d94b` throughout. Subsequent documentation-only commits did not change tested C# or build files. Evidence: `/tmp/pineapple-slam-full-tests.log`, `/tmp/pineapple-slam-full-build.log`, and companion source manifests. Temporary SQL resources and credentials were removed.

Strict production Jekyll validation passed:274 local links/assets are valid and all34 JSON-LD blocks parse. Migration links and four-framework metadata were corrected. The final share-preview caption was updated without version-specific claims; image dimensions in site metadata match the PNG.

### Share-preview asset

- Saved file: `docs/assets/og-image.png` (1731×909).
- Mode: built-in imagegen edit of the existing project image; original generated output remains outside the repository.
- Final prompt: “Edit the provided existing Pineapple Open Graph image. Make exactly one text correction and preserve all other design, branding, colors, layout, logo, border, and text. Keep the existing 1200 by 630 landscape canvas. Replace the two-line subtitle 'Guava-inspired core utilities for .NET 8' / 'and .NET 10' with these exact two lines: 'Guava-inspired core utilities' / 'for modern .NET'. Keep its existing typography, pale gray color, left alignment, font size, and position. Do not change the Pineapple title, pineapple logo, the five API pill labels, or the bottom MILL5 / Richard Crane / Microsoft MVP attribution. No new elements or extra text. This is a precise text-localization edit of an existing approved graphic, not a redesign.”
- The generated image preserved the intended proportions but returned1731×909 rather than1200×630; site metadata uses the actual output size.

PRs208–211 were closed after their exact heads were preserved in the pushed ancestry of PR203. SonarCloud/SonarQube workflow and token removal are complete; CodeQL and CI remain active. GitHub approval/check state is rechecked against the final head before any merge; no manual review request or protection bypass is used.
