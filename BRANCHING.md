# Branch and review policy

Work moves through `feature/*` → `develop` → `main`, using merge commits.

- Start feature branches from `develop`; merge `origin/main` into the feature
  branch if a prior promotion left `develop` behind `main`, then open the PR
  against `develop`. This carries the promotion merge commit forward through
  the permitted source path; do not open a `main` → `develop` PR or push a
  synchronization commit directly to `develop`.
- Only same-repository `feature/*` branches may merge into `develop`.
- Only same-repository `develop` may merge into `main`.
- Both long-lived branches require a PR, an up-to-date branch, resolved conversations,
  the Branch policy check, Linux and Windows builds, SQL integration, and CodeQL analysis.
  These requirements include administrators. Force pushes and branch deletion are disabled.
- Human approval count is zero. Copilot cloud review runs automatically on PRs into
  `develop`, including subsequent real pushes. Address its current-head findings before
  merging; a missing, skipped, or quota-refused review is not acceptance.
- Do not request reviews manually or manufacture commits to trigger them.
- Promotion from `develop` to `main` does not request another Copilot review.
  CodeQL remains active for both branches; security scanning is separate from Copilot review.
- GitHub Pages deploys from `main`. NuGet publication still requires a version tag
  whose commit belongs to `main`; merging a PR does not publish packages.

`scripts/configure-github-branches.py` applies the repository-owned GitHub protections
and develop-only Copilot ruleset using an authenticated `gh` administrator session.
It preserves unrelated rulesets. `Branch policy` validates the source repository and
branch in `.github/workflows/branch-policy.yml`; GitHub's branch-name protection alone
does not filter PR source branches. Copilot completion is checked by the shipping
workflow; its automatic-review ruleset schedules reviews, not a required-approval count.

Dependabot opens update proposals against `develop`. Its generated `dependabot/*`
heads cannot pass the source-branch guard: bring the proposed commits into a
`feature/*` branch before merging, preserving their original commits.

## First adoption

`develop` was created from the existing `main`. The existing modernization head and
its consolidated dependency/author-card history were preserved on
`feature/complete-project-workflow`. Renaming the old remote head closed PR203;
its replacement targets `develop`. No commits were discarded or rebased.

The first PR carries the new branch-policy workflow into `develop`, then promotion
brings it to `main`. Required checks must run successfully before either merge;
the protections are not relaxed for adoption.
