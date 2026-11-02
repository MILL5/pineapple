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
- Human approval count is zero. Copilot cloud review runs once when a PR into
  `develop` is ready; subsequent pushes do not request another review. Batch all
  code, documentation, configuration, and local verification before the initial
  PR push. Handle review findings locally, then make one consolidated correction
  push if needed. Require CI on the final head; do not wait for an additional
  review unless the owner explicitly requests it. A missing, skipped, or
  quota-refused initial review is not acceptance.
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
Before updating a named organization rule, the script verifies its exact repository
and ref selectors. A matching name on another repository or broader scope fails
before writes; names alone do not grant ownership.
The guard's `actions/github-script` dependency is also pinned to a full commit
SHA; a workflow pin alone would not freeze the behavior of a movable action tag.
Both long-lived branches must already exist; the updater fails before writes if
either is absent and never silently creates an unprotected replacement branch.

GitHub requires organization Administration permission (`admin:org` for this CLI's
OAuth token) for the trusted-workflow rule. Repository administration alone is
insufficient. The script checks that access before applying settings. To update the
guard, review and push the new policy commit, then run the script with its full SHA.

The workflow validates the source repository and branch; `Source to main` and
`Source to develop` are separate contexts so success for one target is not reused
for the other after PR retargeting. GitHub's branch-name protection alone does not
filter PR source branches. Copilot completion is checked by the shipping
workflow; its automatic-review ruleset schedules reviews, not a required-approval count.

Dependabot's weekly version-update group targets `develop` and generates
`feature/dependencies/*` heads, including its automatic uniqueness digest. This
uses the group-level `target-branch` and `pull-request-branch-name.template`
options documented in [GitHub's multi-ecosystem configuration](https://docs.github.com/en/code-security/concepts/supply-chain-security/multi-ecosystem-updates)
and [branch-name reference](https://docs.github.com/en/code-security/tutorials/secure-your-dependencies/customizing-dependabot-prs#full-example-with-multi-ecosystem-groups).
These options apply to new version-update PRs. Existing `dependabot/*` heads and
security-update proposals against the default branch do not get an exemption:
bring those commits through a `feature/*` PR into `develop`, preserving their history.

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
Required checks and the one applicable Copilot review must complete before
merge; protections are not relaxed for adoption. GitHub did not start the newly
installed required workflow for an already-open PR's ready transition alone;
the substantive adoption/acceptance documentation commit supplies the next
normal synchronization event received fresh checks and automatic review during
initial adoption. The owner subsequently limited this workflow to one review
per PR and consolidated pushes; automatic review-on-push is now disabled.
