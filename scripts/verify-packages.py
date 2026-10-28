"""Check the two release archives and their shared package dependency."""

from pathlib import Path
import sys
import xml.etree.ElementTree as ET
from zipfile import ZipFile


def verify(artifacts: Path) -> None:
    root = Path(__file__).resolve().parents[1]
    version = ET.parse(root / "Directory.Build.props").findtext(".//Version")
    if not version:
        raise ValueError("Directory.Build.props must define the shared version")

    for package in ("Pineapple", "Pineapple.Data"):
        archive = artifacts / f"{package}.{version}.nupkg"
        with ZipFile(archive) as contents:
            spec = ET.fromstring(contents.read(f"{package}.nuspec"))
        metadata = spec.find("{*}metadata")
        if metadata is None:
            raise ValueError(f"{archive}: missing package metadata")
        if metadata.findtext("{*}id") != package or metadata.findtext("{*}version") != version:
            raise ValueError(f"{archive}: package ID/version differs from the shared release")
        groups = metadata.findall("{*}dependencies/{*}group")
        frameworks = {group.attrib.get("targetFramework") for group in groups}
        if frameworks != {"net8.0", "net10.0"}:
            raise ValueError(f"{archive}: unexpected dependency frameworks {frameworks}")
        if package == "Pineapple.Data":
            for group in groups:
                references = [dep for dep in group if dep.attrib.get("id") == "Pineapple"]
                if len(references) != 1 or references[0].attrib.get("version") != version:
                    raise ValueError(f"{archive}: Pineapple dependency must match {version}")
        print(f"Verified {package} {version} for .NET 8 and .NET 10")


if __name__ == "__main__":
    verify(Path(sys.argv[1] if len(sys.argv) > 1 else "artifacts"))
