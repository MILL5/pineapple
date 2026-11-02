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
**Acceptance pending:** current CLI token lacks `admin:org`; the organization rule
and hosted tamper tests are not yet verified. Do not call source protection complete.

GitHub documents required workflows at organization/enterprise level, despite the
repository REST schema listing the rule type. Repository-level creation was attempted
and rejected with HTTP422; organization ruleset access returned HTTP404 and the CLI
identified the missing `admin:org` scope. No administrator bypass was used.

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
needed after promotion merge commits. **Acceptance pending:** hosted proof of
allowed/forbidden sources, same-SHA retargeting, and guard tampering.

## Validation boundary

A follow-up review found that the trusted-rule write originally came after the
branch protection writes. **Fixed:** install/update the trusted rule first; missing
read permission or rejected rule writes cannot replace protections with only a
PR-supplied check. Three mocked-API tests cover denied read, denied write, scoped
SHA pinning, and operation order. Hosted installation remains a separate gate.

The actual inline script passes twelve local cases: valid feature/develop promotion,
invalid sources/targets, empty feature suffix, Dependabot head, and fork rejection.
Actionlint passes. These establish script logic and workflow syntax, not trusted
hosted enforcement. CodeQL stays separate from the required source workflow.

Sources: [GitHub required workflows](https://docs.github.com/en/enterprise-cloud@latest/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets#require-workflows-to-pass-before-merging),
[organization rules API](https://docs.github.com/en/rest/orgs/rules),
[pull_request_target security](https://docs.github.com/en/actions/reference/security/securely-using-pull_request_target).
