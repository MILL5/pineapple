"""Verify access failures cannot replace trusted policy with PR-supplied checks."""

import copy
import importlib.util
import pathlib
import unittest
from unittest.mock import patch

spec = importlib.util.spec_from_file_location(
    "branch_configuration", pathlib.Path(__file__).with_name("configure-github-branches.py"))
configuration = importlib.util.module_from_spec(spec)
spec.loader.exec_module(configuration)


class BranchConfigurationTests(unittest.TestCase):
    def run_configuration(self, *, fail_org_write=False, fail_org_read=False, missing_develop=False,
                          existing_selector=None, expected_failure=False):
        events = []
        failed = False

        def fake_api(path, method="GET", payload=None, *, organization=False):
            events.append((path, method, copy.deepcopy(payload), organization))
            if organization and (fail_org_read or (fail_org_write and method != "GET")):
                raise RuntimeError("organization access denied")
            if path == "" and method == "GET":
                return {"id": 196690353}
            if path.startswith("branches?"):
                return [{"name": "main"}] if missing_develop else [{"name": "main"}, {"name": "develop"}]
            if path == "rulesets/42" and method == "GET" and organization:
                return {"target": "branch", "conditions": copy.deepcopy(existing_selector)}
            if path.startswith("rulesets?") and organization and existing_selector is not None:
                return [{"id": 42, "name": "Pineapple trusted branch source policy"}]
            if path.startswith("rulesets?"):
                return []
            return {}

        with patch.object(configuration, "api", side_effect=fake_api), \
                patch("sys.argv", ["configure", "--policy-sha", "a" * 40]), \
                patch("builtins.print"):
            try:
                configuration.main()
            except RuntimeError:
                failed = True
                if not (fail_org_write or fail_org_read or missing_develop or expected_failure):
                    raise
        if expected_failure:
            self.assertTrue(failed, "Scope mismatch must fail explicitly")
        return events

    @staticmethod
    def trusted_selector():
        return {"repository_id": {"repository_ids": [196690353]},
                "ref_name": {"include": ["refs/heads/main", "refs/heads/develop"], "exclude": []}}

    def test_missing_org_access_does_not_change_any_settings(self):
        events = self.run_configuration(fail_org_read=True)
        self.assertTrue(all(method == "GET" for _, method, _, _ in events))

    def test_failed_trusted_policy_install_does_not_replace_branch_checks(self):
        events = self.run_configuration(fail_org_write=True)
        writes = [(path, org) for path, method, _, org in events if method != "GET"]
        self.assertEqual(writes, [("rulesets", True)])

    def test_missing_develop_does_not_create_an_unprotected_long_lived_branch(self):
        events = self.run_configuration(missing_develop=True)
        self.assertTrue(all(method == "GET" for _, method, _, _ in events))

    def test_named_rule_for_another_repository_is_not_modified(self):
        selector = self.trusted_selector()
        selector["repository_id"]["repository_ids"] = [999]
        events = self.run_configuration(existing_selector=selector, expected_failure=True)
        self.assertTrue(all(method == "GET" for _, method, _, _ in events))

    def test_named_rule_with_broader_refs_is_not_modified(self):
        selector = self.trusted_selector()
        selector["ref_name"]["include"] = ["~ALL"]
        events = self.run_configuration(existing_selector=selector, expected_failure=True)
        self.assertTrue(all(method == "GET" for _, method, _, _ in events))

    def test_matching_existing_rule_is_updated_in_place(self):
        events = self.run_configuration(existing_selector=self.trusted_selector())
        first_write = next(entry for entry in events if entry[1] != "GET")
        self.assertEqual((first_write[0], first_write[1], first_write[3]),
                         ("rulesets/42", "PUT", True))

    def test_trusted_policy_is_scoped_pinned_and_installed_before_protections(self):
        events = self.run_configuration()
        writes = [entry for entry in events if entry[1] != "GET"]
        path, method, rule, org = writes[0]
        self.assertEqual((path, method, org), ("rulesets", "POST", True))
        self.assertEqual(rule["conditions"]["repository_id"]["repository_ids"], [196690353])
        self.assertEqual(rule["conditions"]["ref_name"]["include"],
                         ["refs/heads/main", "refs/heads/develop"])
        self.assertEqual(rule["rules"][0]["parameters"]["workflows"][0]["sha"], "a" * 40)
        copilot = next(payload for path, _, payload, org in writes
                       if path == "rulesets" and not org)
        self.assertEqual(copilot["conditions"]["ref_name"]["include"], ["refs/heads/develop"])
        self.assertFalse(copilot["rules"][0]["parameters"]["review_on_push"])
        self.assertFalse(copilot["rules"][0]["parameters"]["review_draft_pull_requests"])
        for branch in ("main", "develop"):
            protection = next(payload for path, _, payload, _ in writes
                              if path == f"branches/{branch}/protection")
            self.assertEqual(protection["required_status_checks"]["checks"][0]["context"],
                             f"Source to {branch}")
            self.assertTrue(protection["enforce_admins"])
            self.assertEqual(protection["required_pull_request_reviews"]
                             ["required_approving_review_count"], 0)


if __name__ == "__main__":
    unittest.main()
