# Pineapple product

<!-- impeccable:product-schema 1 -->

## Platform

web

The interface covered here is the Jekyll documentation site under this directory (`docs/` from the repository root). The product itself is a pair of C#/.NET libraries, not a hosted application.

## Users

.NET developers evaluating, installing, upgrading, and using Pineapple. Repository contributors maintain the packages and the guides. This audience is inferred from the existing package installation instructions and API guides; no user research or customer segmentation was supplied.

## Product Purpose

Pineapple packages reusable guard clauses, compact GUIDs, reproducible hashes, timing and cleanup helpers. Pineapple.Data adds SQL Server row versions, availability checks, and EF Core metadata access. Documentation succeeds when a developer can choose a package, install it, find an API example, and understand compatibility and migration limits.

## Operating Context

Consumers install NuGet packages into their own .NET applications. Core consumers do not need SQL dependencies. The repository targets .NET 8, 9, 10, and 11 RC; .NET 11 is a preview target until stable acceptance is established. The site is static Jekyll with Liquid templates, Rouge code highlighting, CSS, and GitHub Pages configuration. Installation and package publication are separate events; a successful source build is not proof of a published release.

## Capabilities and Constraints

- Keep the existing public API and persisted hash values stable unless the owner explicitly approves a breaking change.
- Preserve K4os xxHash and the existing supported input encodings; process-randomized hashes are unsuitable for this product's persisted-hash contract.
- Keep the two package boundaries and all four target frameworks visible in the documentation.
- Existing compatibility APIs remain through 2.x and link to their built-in .NET replacements in the migration guide.
- Do not infer distributed rate-limiting guarantees, cryptographic strength, or collision-free hashes from these utilities.
- Preserve the existing documentation site's identity; this task is a review and refinement, not a rebrand.

## Brand Commitments

Pineapple is published by MILL5. The site credits Richard Crane and uses the existing pineapple mark and Inventing Fire with AI author mark. Preserve the repository's Apache 2.0 licensing and attribution. Product copy is direct and technical; do not invent customers, benchmarks, endorsements, or adoption claims.

## Evidence on Hand

[Architecture](architecture.md) records the full repository design. [Goldfish review](reviews/consolidated_review_report.md) traces feedback to implementation and verification. Package projects, tests, `getting-started.md`, `guides/`, `_config.yml`, and the shared layout provide the product facts above. Existing PNG brand assets live in `assets/`. Existing third-party attribution and adoption copy are incumbent content, not newly verified market evidence.

## Product Principles

- Make the correct package and API easy to find.
- Preserve compatibility contracts and state their limits.
- Put working installation and code examples before promotional material.
- Keep documentation readable with keyboard navigation and at narrow viewport sizes.

## Open Decisions

No new market positioning, customer cohort, or measurable conversion target was supplied. These remain undecided. No formal product-specific accessibility certification was requested; review findings use applicable web accessibility practices without claiming certification.
