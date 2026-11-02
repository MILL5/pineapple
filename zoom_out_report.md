# Initial Zoom Out review

- Stage: 5, fresh-context `egm-zoom-out`, Mode REVIEW.
- Target: `design_draft.md`, 247 lines; FeatureRange 1–222, DetailedRange 223–247.
- Target SHA-256: `41d239ae74a115a3f1b17ef903011b1b40b5b27138d89ae2e5fac71ecc94430b`.
- Result: 2 Major findings, 1 Nit; no Blockers. Read-only review, no execution proof.
- Reviewer confirmed reading its persona, the zoom-out standard, and the complete target. No memory or conversation history supplied.

## Z1 — Major — Semaphore admission limitation is missing

Affected target: lines 98 and 210–221.

`Pineapple/Threading/RateLimiting/SemaphoreRateLimiterScope.cs:16–18,31–41` constructs a private semaphore with initial count zero, synchronously waits during acquisition, and contains no release path. The document presents the type as another choice and qualifies scheduling and fairness without explaining this admission limitation.

Improvement: state that the current implementation has no evident permit-production path and requires validation before recommending it. Record a decision about intended semantics or explicitly defer this implementation. This is an existing source defect omitted from the inventory, not a newly introduced implementation defect.

## Z2 — Major — SQL tests lack a release execution contract

Affected target: lines 143 and 233–236.

The plan adds a data test project to the solution and provisions SQL in CI, but `.github/workflows/release.yml:36–37` also runs full-solution tests without SQL. The document leaves implementers to decide whether ordinary local runs and releases fail, exclude integration tests, or provision a database.

Improvement: define model-test and integration-test execution across local runs, Linux/Windows CI, and release. State where SQL is provisioned, where integration tests are explicitly excluded, and which evidence gates release. Include the release workflow in scope if the selected contract changes it.

## Z3 — Nit — Hash documentation parity omits the packaged README

Affected target: lines 145 and 230.

`README.md:80` promises identical hashes on every machine; `Directory.Build.props` packages that README. The guide has similarly broad claims. Numeric encoding in `Pineapple/Common/HashCodeHelper.cs:111–138` uses machine-dependent `BitConverter`. The architecture narrows the guarantee, but the work package updates only the guide.

Improvement: add `README.md` to the hash documentation work and require agreement between README, guide, and architecture. Do not change the hash implementation.

## Scope and evidence

The document establishes sufficient context and preserves the package boundary. No speculative infrastructure was identified. Searches found no existing cross-process hash probe to reuse. Already documented SQL-success, async-lock, governor-accounting, moving-window, and EF-mapping concerns were not counted again.

Inspected: `README.md`, `AGENTS.md`, `CHANGELOG.md`, `global.json`, shared package/build properties, all three project files, all three GitHub workflows, hash implementation and tests, safe/semaphore limiters, operation tracking and governor, SQL availability and schema context/models, HTTP integration and handler, health checks, hash/rate-limiting guides. Reviewed complete `HashCode.cs`, first 250 lines of its helper, first 240 lines of hash tests, and the complete relevant threading/data/networking files. No package-advisory refresh or runtime test was performed.
