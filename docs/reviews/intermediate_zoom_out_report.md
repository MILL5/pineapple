# Intermediate Zoom Out review

- Stage: 8; fresh-context `egm-zoom-out`, Mode REVIEW.
- Target: complete unchanged `design_draft.md`, 247 lines.
- Result: no new Blocker or Major architectural findings. Prior findings remain open because no mitigations have been applied.
- Reviewer confirmed complete reads of its persona, zoom-out standard, target, and all three preceding stage reports.

## Disposition of proposed improvements

- **I1 — Major; duplicate Z1/C1/D1; target lines 98,210–221:** Accept the semaphore usability warning and explicit deferral. Source has zero initial permits, blocking acquisition, and no release (`SemaphoreRateLimiterScope.cs:18,21–28,36`). Other local limiters have different pacing contracts and do not supply a reusable permit mechanism. Do not introduce an unapproved replenishment framework.
- **I2 — Major; duplicate D2; target lines 108,114,206,219:** Accept a documented decoded-size/payload boundary while implementation remains deferred. Source buffers without a limit (`BrotliCompressionHandler.cs:23–29`); no existing decoded-byte limiter was found. The networking guide describes platform decompression, but switching handlers alone does not create a memory bound. Bespoke streaming infrastructure is unnecessary for this document task.
- **I3 — Major; duplicate Z2; target lines 143,233–236:** Define the SQL test execution matrix before implementation. CI and release run solution tests without a SQL service (`ci.yml:28`, `release.yml:36–37`). Specify exclusions, provisioning, and release evidence while reusing existing workflows.
- **I4 — Major; duplicate T1; target lines 135–137:** Accept dated audit commands, target-specific restored versions, and a durable evidence reference. No audit service or framework is needed. This review did not repeat the package audit.
- **I5 — Nit; duplicate Z3; target lines 145,230:** Include the packaged README in hash documentation parity. `README.md:80` retains the broader machine-independent claim; `Directory.Build.props:18,28` packages it.

## Evidence

Read-only source searches covered core/data/tests, workflows, and guides. Inspected semaphore, safe, and basic limiters; Brotli handler; networking guide; CI/release; README; test project; shared build properties. No writes, builds, history, or memory access.
