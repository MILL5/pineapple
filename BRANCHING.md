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
  the target-specific source check, Linux and Windows builds, SQL integration, and CodeQL analysis.
  These requirements include administrators. Force pushes and branch deletion are disabled.
- Human approval count is zero. Copilot cloud review runs automatically on PRs into
  `develop`, including subsequent real pushes. Address its current-head findings before
  merging; a missing, skipped, or quota-refused review is not acceptance.
- Do not request reviews manually or manufacture commits to trigger them.
- Promotion from `develop` to `main` does not request another Copilot review.
  CodeQL remains active for both branches; security scanning is separate from Copilot review.
- GitHub Pages deploys from `main`. NuGet publication still requires a version tag
  whose commit belongs to `main`; merging a PR does not publish packages.

`python3 scripts/configure-github-branches.py --policy-sha <reviewed-pushed-commit>`
applies the GitHub protections and develop-only Copilot ruleset using an authenticated
`gh` administrator session. It also installs an organization-owned required-workflow
rule scoped only to Pineapple's repository ID and the `main`/`develop` refs. That rule
pins `.github/workflows/branch-policy.yml` to the reviewed commit, so a PR cannot
replace its own guard with a successful no-op. Unrelated rulesets are preserved.

GitHub requires organization Administration permission (`admin:org` for this CLI's
OAuth token) for the trusted-workflow rule. Repository administration alone is
insufficient. The script checks that access before applying settings. To update the
guard, review and push the new policy commit, then run the script with its full SHA.

The workflow validates the source repository and branch; `Source to main` and
`Source to develop` are separate contexts so success for one target is not reused
for the other after PR retargeting. GitHub's branch-name protection alone does not
filter PR source branches. Copilot completion is checked by the shipping
workflow; its automatic-review ruleset schedules reviews, not a required-approval count.

Dependabot opens update proposals against `develop`. Its generated `dependabot/*`
heads cannot pass the source-branch guard: bring the proposed commits into a
`feature/*` branch before merging, preserving their original commits.

## First adoption

`develop` was created from the existing `main`. The existing modernization head and
its consolidated dependency/author-card history were preserved on
`feature/complete-project-workflow`. Renaming the old remote head closed PR203;
its replacement targets `develop`. No commits were discarded or rebased.

The workflow was committed and pushed on the feature branch, and draft PR212
was created while organization authorization was pending. After authorization,
organization rule26373889 pinned reviewed commit `cd3d22b`, before any merge.
It runs without trusting the PR's proposed workflow or first merging into the
default branch. Hosted probes verified that the pinned guard defeats a forged
guard and rejects invalid sources; a successful develop check cannot satisfy main
after same-head retargeting. See [the Goldfish acceptance evidence](branch_policy_review.md).

The real PR carries this work into `develop`, then promotion brings it to `main`.
Required checks and applicable current-head Copilot review must complete before
merge; protections are not relaxed for adoption. GitHub did not start the newly
installed required workflow for an already-open PR's ready transition alone;
the substantive adoption/acceptance documentation commit supplies the next
normal synchronization event and receives fresh checks and automatic review.
