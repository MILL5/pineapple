"""Check the two release archives and their shared package dependency."""

from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET
from zipfile import ZipFile


MIGRATION_GUIDE_URL = "https://mill5.github.io/pineapple/guides/migration/"


def verify_readme(contents: ZipFile, metadata: ET.Element, archive: Path) -> None:
    readme_path = metadata.findtext("{*}readme")
    if readme_path != "README.md":
        raise ValueError(f"{archive}: package readme metadata must name README.md")
    if readme_path not in contents.namelist():
        raise ValueError(f"{archive}: packaged README.md is missing")
    readme = contents.read(readme_path).decode("utf-8-sig")
    links = re.findall(r"\[migration guide\]\(([^)]+)\)", readme, re.IGNORECASE)
    if not links or any(link != MIGRATION_GUIDE_URL for link in links):
        raise ValueError(f"{archive}: every migration-guide link must use {MIGRATION_GUIDE_URL}")


def verify(artifacts: Path) -> None:
    root = Path(__file__).resolve().parents[1]
    version = ET.parse(root / "Directory.Build.props").findtext(".//Version")
    if not version:
        raise ValueError("Directory.Build.props must define the shared version")

    for package in ("Pineapple", "Pineapple.Data"):
        archive = artifacts / f"{package}.{version}.nupkg"
        with ZipFile(archive) as contents:
            assets = {name.split("/")[1] for name in contents.namelist() if name.startswith("lib/") and name.endswith(".dll")}
            if assets != {"net8.0", "net9.0", "net10.0", "net11.0"}:
                raise ValueError(f"{archive}: unexpected library assets {assets}")
            spec = ET.fromstring(contents.read(f"{package}.nuspec"))
            metadata = spec.find("{*}metadata")
            if metadata is None:
                raise ValueError(f"{archive}: missing package metadata")
            verify_readme(contents, metadata, archive)
        if metadata.findtext("{*}id") != package or metadata.findtext("{*}version") != version:
            raise ValueError(f"{archive}: package ID/version differs from the shared release")
        groups = metadata.findall("{*}dependencies/{*}group")
        frameworks = {group.attrib.get("targetFramework") for group in groups}
        if frameworks != {"net8.0", "net9.0", "net10.0", "net11.0"}:
            raise ValueError(f"{archive}: unexpected dependency frameworks {frameworks}")
        if package == "Pineapple.Data":
            for group in groups:
                references = [dep for dep in group if dep.attrib.get("id") == "Pineapple"]
                if len(references) != 1 or references[0].attrib.get("version") != version:
                    raise ValueError(f"{archive}: Pineapple dependency must match {version}")
        print(f"Verified {package} {version} for .NET 8, 9, 10, and 11")


if __name__ == "__main__":
    verify(Path(sys.argv[1] if len(sys.argv) > 1 else "artifacts"))
