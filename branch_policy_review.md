# Branch policy Goldfish readiness review

- Date: 2026-11-02.
- Method: fresh-context implementation-readiness reviewer, no parent conversation.
- Read: `.agents/agents/implementation_readiness.md`,
  `.agents/resources/document_standards/implementation-readiness-standard.md`,
  `.agents/resources/execution_instructions.md`, branch architecture/design,
  GitHub configuration script, branch-policy workflow/tests, CI/CodeQL, AGENTS.md.
- Scope: the branch workflow addition; the earlier five-stage architecture review
  remains in `consolidated_review_report.md` and its linked reports.

## Findings and dispositions

### Blocker: a PR could replace its own source guard

The first implementation ran only from the PR-supplied workflow. An invalid-source
PR could keep its required check name and replace the check with a successful no-op.
Binding a check to GitHub Actions app15368 identifies its publisher, not trusted code.

**Implemented:** the configuration script defines an organization-owned required
workflow pinned to a reviewed, pushed SHA and scoped only to Pineapple's repository
ID and `main`/`develop`. A missing organization permission fails before settings writes.
**Installed:** after the owner completed GitHub authorization, active organization
ruleset26373889 pinned the workflow at `cd3d22be2e4885a1a84af3207fa04fa0bf1ffd5f`,
with no bypass actors, scoped only to repository196690353 and `main`/`develop`.
Both branches expose that inherited workflow rule through GitHub's effective-rules
API. Hosted acceptance below remains separate from successful configuration.

GitHub documents required workflows at organization/enterprise level, despite the
repository REST schema listing the rule type. Repository-level creation was attempted
and rejected with HTTP422; organization ruleset access initially returned HTTP404
and the CLI identified the missing `admin:org` scope. The owner authorized that scope;
the organization installation then succeeded. No administrator bypass was used.

### Major: merge method policy was not enforced

The original settings allowed squash/rebase, despite the documented merge-commit-only
policy. **Fixed:** repository settings now enable merge commits and disable squash and
rebase merges. The reproducible configuration script includes those settings.

### Major: adoption and PR retargeting needed explicit proof

A `pull_request_target` substitution alone would not bootstrap: the trusted workflow
is not yet on the default branch. Required-workflow event filters also cannot be
assumed to rerun after an `edited`/retarget event.

**Implemented:** pin the pushed feature-branch workflow before first adoption;
require distinct `Source to main` and `Source to develop` contexts. BRANCHING.md
documents the initial sequence and the recurring main-sync-through-feature route
needed after promotion merge commits. Hosted acceptance is recorded below.

## Validation boundary

A follow-up review found that the trusted-rule write originally came after the
branch protection writes. **Fixed:** install/update the trusted rule first; missing
read permission or rejected rule writes cannot replace protections with only a
PR-supplied check. Three mocked-API tests cover denied read, denied write, scoped
SHA pinning, and operation order. Hosted installation subsequently succeeded.

The actual inline script passes twelve local cases: valid feature/develop promotion,
invalid sources/targets, empty feature suffix, Dependabot head, and fork rejection.
Actionlint passes. These establish script logic and workflow syntax, not trusted
hosted enforcement. CodeQL stays separate from the required source workflow.
The existing weekly Dependabot group was also aligned with this branch contract:
group-level target `develop`, branch template `feature/dependencies/{name}`.
Security/default-branch and pre-existing proposals must still follow the feature
handoff; the source guard has no bot exemption.

## Hosted acceptance

- Effective branch rules for both long-lived branches include the active,
  organization-owned required-workflow rule26373889, pinned to `cd3d22b`.
- Rejection and tamper resistance: [PR213](https://github.com/MILL5/pineapple/pull/213)
  proposed `policy-probe/invalid-source-forged-guard` into `main`. Its head
  `f32e1a01fc5ff6971baf5db564ac57ce5592c11f` replaced the guard with a successful
  no-op. GitHub ran the pinned real guard instead, rejected the invalid source,
  and blocked merging: [trusted failed check](https://github.com/MILL5/pineapple/actions/runs/41463598244/job/125338573356).
- Positive feature path and same-head retargeting: draft
  [PR214](https://github.com/MILL5/pineapple/pull/214), `feature/policy-retarget-probe`
  into `develop`, received a fresh pinned
  [Source to develop success](https://github.com/MILL5/pineapple/actions/runs/41463944662/job/125339785256).
  Retargeting the unchanged `cd3d22b` head into `main` and marking that PR ready
  left it blocked: main's required `Source to main` context was absent. The
  earlier successful develop check did not satisfy main's protection.
- Both validation PRs were closed without merging; no reviews were manually
  requested. No Copilot review was scheduled by the develop rule while the
  retarget probe was a draft. Its ready transition happened only against main.
- These hosted tests exercise the trusted execution boundary, valid feature flow,
  invalid main source, and same-SHA target change. Other source and fork cases
  are covered by the twelve local tests against that exact guard logic; no
  hosted fork PR is claimed. The real develop-to-main promotion remains a
  separate shipping acceptance step.

Sources: [GitHub required workflows](https://docs.github.com/en/enterprise-cloud@latest/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets#require-workflows-to-pass-before-merging),
[organization rules API](https://docs.github.com/en/rest/orgs/rules),
[pull_request_target security](https://docs.github.com/en/actions/reference/security/securely-using-pull_request_target).
