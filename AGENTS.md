# Pineapple

- C# utility libraries targeting .NET 8, 9, 10, and 11. Core utilities live in `Pineapple/`; SQL Server and EF Core helpers live in `Pineapple.Data/`; core tests live in `Pineapple.Tests/`; SQL/model tests live in `Pineapple.Data.Tests/`.
- Keep changes surgical and preserve public API compatibility unless a breaking change is explicitly requested.
- Use `Directory.Packages.props` for dependency versions and `Directory.Build.props` for shared build settings.
- Build with the .NET 11 RC SDK pinned in `global.json` and install matching .NET 8, 9, 10, and 11 runtimes. Keep K4os xxHash and its persisted outputs unchanged.
- Validate code changes with `dotnet build Pineapple.slnx` and relevant tests using `dotnet test --solution Pineapple.slnx`.
- Google Elephant-Goldfish Model skills are installed in `.agents/skills/egm-*`. Use them for substantial work requiring design decisions; simple, clear fixes can use the normal workflow.
- Resolve EGM reviewer prompts, standards, and integration docs from `.agents/agents/`, `.agents/resources/`, and `.agents/docs/`. Goldfish reviews require fresh subagent contexts.
- Keep shared agent guidance in `AGENTS.md` files.
- Branch flow is `feature/*` → `develop` → `main`, using merge commits. Both long-lived branches require the target-specific source and CI checks; do not push directly. The trusted guard is pinned by an organization ruleset scoped only to this repository (see BRANCHING.md). Promotion PRs to `main` must come from `develop`.
- Batch all scoped code, docs, configuration, and local validation before pushing to the PR. Run one automatic Copilot review on the ready PR into `develop`; consolidate any corrections into one push and do not request or wait for another review unless the owner explicitly asks. CI must pass on the final head.
- SonarQube and SonarCloud are retired. Keep CodeQL and CI validation; do not restore Sonar workflows or tokens.
