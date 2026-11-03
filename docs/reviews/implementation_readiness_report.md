# Implementation readiness review

- Stage: 9; independent fresh-context readiness, codebase-details, and style reviewers.
- Reviewed snapshot: `design_draft.md:223–247`, with lines 1–222 as context; original SHA-256 `41d239ae74a115a3f1b17ef903011b1b40b5b27138d89ae2e5fac71ecc94430b`.
- Verdict: fails as an executable plan. It is a useful architecture roadmap, but acknowledged unresolved choices still fail the zero-design-choice rule.
- Reviewers confirmed reading their personas and full target; readiness and codebase-details reviewers also read the implementation-readiness standard. All reviews were read-only, without builds, writes, history, or memory.

## Readiness reviewer

- **R1 — Blocker; lines 225,231,234; context 212–213:** Resolve SQL availability success/error/timeout behavior and limiter synchronization, token lifetime, configuration conflicts, cancellation, and disposal contracts before implementation. Source: `Pineapple.Data/SqlClient/Check.cs:17–39`, `SafeRateLimiterScope.cs:29–47,68–83`.
- **R2 — Major; line 229:** Specify exact package/version/framework changes, override strategy, expected resolved graph, and vulnerability/deprecation acceptance. “Select patched compatible versions” and “only if needed” leave dependency decisions open.
- **R3 — Major; line 230:** Enumerate hash vectors, signed expected results, byte-order scope, and process-probe invocation/output/assertions. Preserve and extend existing fixed-result tests at `Pineapple.Tests/HashCodeTests.cs:145–176`.
- **R4 — Major; line 232:** Specify completion accounting, empty averages, scavenger lifecycle, retention, refreshed admission windows, clock seams, and assertions. “Verify and fix” leaves behavior undecided. Source: `ResourceGoverner.cs:43–100`, `Operations.cs:19–46,68–89`.
- **R5 — Major; lines 233–234:** Specify SQL service provisioning, fixtures, connection configuration, failure policy, exact model changes, query results, and Linux/Windows integration execution. Current CI has no SQL service (`ci.yml:12–30`); mappings are in `InformationSchemaContext.cs:30–107`.
- **R6 — Major; lines 227–236:** Order work into stable, buildable phases with prerequisites, exact commands and assertions. Resolve data-test/model-repair dependencies and package/consumer acceptance. A final blanket verification paragraph is insufficient.

The readiness reviewer inspected SQL availability, async limiter, hash tests, schema context, CI, governor, and operations.

## Codebase-details reviewer

- **K1 — Major; line 232:** Define a single idempotent operation-completion path. `Operations.cs:79–89` calls `operation.Dispose()` before accumulating time; `Operation.cs:45–62` only marks completion. Wiring disposal directly back to `Remove` risks recursion and double counting. Define completed-versus-started counts, empty average (`BaseResourceGoverner.cs:23`), and scavenger start/restart/stop assertions.
- **K2 — Major; lines 233–236:** Specify SQL service version, connection configuration, readiness, fixtures, classification, and filtering across local, CI, and release. CI and release both run the entire solution without SQL. Include `release.yml` if its command must change.
- **K3 — Major; line 230:** Define the hash-probe build/output contract: build dependency, executable discovery/copying, matching target framework, invocation format, timeout/termination, and nonzero-exit assertions. Adding a project to the solution alone does not make it available from test output. Tests currently reference only core (`Pineapple.Tests.csproj:17–19`).
- **K4 — Major; line 234:** Specify `Table` registration, `INFORMATION_SCHEMA.TABLES` mapping, property mappings, and key/keyless choice. Include model-only assertions and a known integration fixture with table/column/key/constraint rows. Current context exposes `Table` but configures only the other entities (`InformationSchemaContext.cs:30,35–107`).

The codebase reviewer checked current referenced files, project/build/test configuration, workflows, hash implementation, limiter/governor lifecycle, SQL check, and EF mappings. No current-file references were missing; proposed new files were not treated as missing-current-file defects.

## Style reviewer

- **U1 — Nit; line 225:** Clarify that unrelated unresolved decisions do not block every work package: “Execute a work package only after resolving the behavioral decisions that affect it.”

The style reviewer read its persona and complete target; no additional source reads were needed for this wording finding.
