"""Exercise the README check against package archives, not source files."""

import importlib.util
from io import BytesIO
from pathlib import Path
import unittest
import xml.etree.ElementTree as ET
from zipfile import ZipFile


spec = importlib.util.spec_from_file_location(
    "verify_packages", Path(__file__).with_name("verify-packages.py")
)
verifier = importlib.util.module_from_spec(spec)
spec.loader.exec_module(verifier)


class PackageReadmeTests(unittest.TestCase):
    def check(self, readme, metadata_path="README.md"):
        metadata = ET.Element("{urn:nuspec}metadata")
        if metadata_path is not None:
            ET.SubElement(metadata, "{urn:nuspec}readme").text = metadata_path
        buffer = BytesIO()
        with ZipFile(buffer, "w") as archive:
            if readme is not None:
                archive.writestr("README.md", readme)
        with ZipFile(buffer) as archive:
            verifier.verify_readme(archive, metadata, Path("test.nupkg"))

    def test_published_url(self):
        self.check(f"See the [migration guide]({verifier.MIGRATION_GUIDE_URL}).")

    def test_original_200_mixed_links_are_rejected(self):
        with self.assertRaisesRegex(ValueError, "every migration-guide link"):
            self.check(
                "See the [migration guide](docs/guides/migration.md).\n"
                f"See the [migration guide]({verifier.MIGRATION_GUIDE_URL})."
            )

    def test_wrong_url_is_rejected(self):
        with self.assertRaises(ValueError):
            self.check("See the [migration guide](https://example.com/migration/).")

    def test_missing_link_is_rejected(self):
        with self.assertRaises(ValueError):
            self.check("No migration link.")

    def test_missing_readme_is_rejected(self):
        with self.assertRaisesRegex(ValueError, "packaged README.md is missing"):
            self.check(None)

    def test_missing_or_wrong_metadata_is_rejected(self):
        for metadata_path in (None, "docs/README.md"):
            with self.subTest(metadata_path=metadata_path):
                with self.assertRaisesRegex(ValueError, "readme metadata"):
                    self.check("README", metadata_path)


if __name__ == "__main__":
    unittest.main()
