#!/usr/bin/env python3
"""Apply Pineapple's feature -> develop -> main policy using authenticated gh."""

import argparse
import json
import subprocess

REPOSITORY = "MILL5/pineapple"
CHECKS = [
    "build (ubuntu-latest)",
    "build (windows-latest)",
    "sql-integration",
    "analyze",
]


def api(path, method="GET", payload=None, *, organization=False):
    prefix = "orgs/MILL5" if organization else f"repos/{REPOSITORY}"
    endpoint = prefix + (f"/{path}" if path else "")
    command = ["gh", "api", endpoint, "--method", method]
    if payload is not None:
        command += ["--input", "-"]
    result = subprocess.run(
        command, input=json.dumps(payload) if payload is not None else None,
        text=True, capture_output=True,
    )
    if result.returncode:
        raise RuntimeError(f"GitHub {method} {path}: {result.stderr.strip()} {result.stdout.strip()}")
    return json.loads(result.stdout) if result.stdout.strip() else None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--policy-sha", required=True,
                        help="Reviewed, pushed commit containing the trusted branch-policy workflow")
    args = parser.parse_args()
    if len(args.policy_sha) != 40 or any(c not in "0123456789abcdef" for c in args.policy_sha):
        parser.error("--policy-sha must be a full lowercase commit SHA")
    # Resolve the policy before making any settings changes. The ruleset pins
    # this reviewed version, so a PR cannot rewrite its own source guard.
    api(f"contents/.github/workflows/branch-policy.yml?ref={args.policy_sha}")
    repository_id = api("")["id"]
    # Required workflows are an organization rule, restricted here to this
    # one repository. Fail before settings writes when admin:org is missing.
    trusted_name = "Pineapple trusted branch source policy"
    existing_trusted = [rule for rule in api("rulesets?per_page=100", organization=True)
                        if rule["name"] == trusted_name]
    if len(existing_trusted) > 1:
        raise RuntimeError("Duplicate trusted source rulesets; inspect before changing them")
    branches = {branch["name"] for branch in api("branches?per_page=100")}
    if "develop" not in branches:
        sha = api("git/ref/heads/main")["object"]["sha"]
        api("git/refs", "POST", {"ref": "refs/heads/develop", "sha": sha})

    trusted = {
        "name": trusted_name,
        "target": "branch", "enforcement": "active", "bypass_actors": [],
        "conditions": {
            "repository_id": {"repository_ids": [repository_id]},
            "ref_name": {"include": ["refs/heads/main", "refs/heads/develop"], "exclude": []},
        },
        "rules": [{"type": "workflows", "parameters": {
            "do_not_enforce_on_create": False,
            "workflows": [{"path": ".github/workflows/branch-policy.yml",
                           "repository_id": repository_id, "sha": args.policy_sha}],
        }}],
    }
    if existing_trusted:
        api(f"rulesets/{existing_trusted[0]['id']}", "PUT", trusted, organization=True)
    else:
        api("rulesets", "POST", trusted, organization=True)
    print(f"Trusted source workflow pinned to {args.policy_sha}")

    protection = {
        "required_status_checks": {
            "strict": True,
            "checks": [{"context": name, "app_id": 15368} for name in CHECKS],
        },
        "enforce_admins": True,
        "required_pull_request_reviews": {
            "dismiss_stale_reviews": True,
            "require_code_owner_reviews": False,
            "require_last_push_approval": False,
            "required_approving_review_count": 0,
        },
        "restrictions": None,
        "required_linear_history": False,
        "allow_force_pushes": False,
        "allow_deletions": False,
        "required_conversation_resolution": True,
    }
    api("", "PATCH", {"allow_merge_commit": True, "allow_squash_merge": False,
                       "allow_rebase_merge": False})
    for branch in ("main", "develop"):
        protection["required_status_checks"]["checks"] = [
            {"context": f"Source to {branch}", "app_id": 15368},
            *[{"context": name, "app_id": 15368} for name in CHECKS],
        ]
        api(f"branches/{branch}/protection", "PUT", protection)
        print(f"Protected {branch}: PR, branch flow, CI, resolved conversations; zero human approvals")

    # Only this repository-owned rule is managed here. Never rewrite an
    # unrelated repository or inherited organization ruleset.
    name = "Copilot review on develop"
    ruleset = {
        "name": name,
        "target": "branch",
        "enforcement": "active",
        "bypass_actors": [],
        "conditions": {"ref_name": {"include": ["refs/heads/develop"], "exclude": []}},
        "rules": [{"type": "copilot_code_review", "parameters": {
            "review_draft_pull_requests": False,
            "review_on_push": True,
        }}],
    }
    existing = [rule for rule in api("rulesets?per_page=100")
                if rule["name"] == name and rule["source_type"] == "Repository"]
    if len(existing) > 1:
        raise RuntimeError("Duplicate managed Copilot rulesets; inspect before changing them")
    if existing:
        api(f"rulesets/{existing[0]['id']}", "PUT", ruleset)
    else:
        api("rulesets", "POST", ruleset)
    print("Automatic Copilot review: PRs into develop, including real pushes")




if __name__ == "__main__":
    main()
