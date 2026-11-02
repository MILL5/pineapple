#!/usr/bin/env bash
set -euo pipefail
package_dir=$(cd "${1:-artifacts}" && pwd)
probe_dir=$(mktemp -d)
trap 'rm -rf "$probe_dir"' EXIT
version=$(dotnet msbuild Pineapple/Pineapple.csproj -getProperty:Version)
python3 scripts/verify-packages.py "$package_dir"
cat > "$probe_dir/NuGet.Config" <<EOF
<configuration><packageSources><clear/><add key="local" value="$package_dir"/><add key="nuget" value="https://api.nuget.org/v3/index.json"/></packageSources><packageSourceMapping><packageSource key="local"><package pattern="Pineapple"/><package pattern="Pineapple.Data"/></packageSource><packageSource key="nuget"><package pattern="*"/></packageSource></packageSourceMapping></configuration>
EOF
# An isolated package cache prevents a previously cached release masking local artifacts.
export NUGET_PACKAGES="$probe_dir/packages"
for framework in net8.0 net9.0 net10.0 net11.0; do
  project="$probe_dir/$framework"
  mkdir -p "$project"
  cat > "$project/Consumer.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>$framework</TargetFramework><ImplicitUsings>enable</ImplicitUsings></PropertyGroup></Project>
EOF
  dotnet add "$project" package Pineapple.Data --version "$version" --no-restore
  cat > "$project/Program.cs" <<'EOF'
using Pineapple.Common;
using Pineapple.Data;
using Pineapple.Data.InformationSchema;
using Microsoft.EntityFrameworkCore;
if (Pineapple.Common.HashCode.ToHashCode("Hello") != -234433905) throw new Exception("Hash changed");
if (!ShortGuid.TryParse("WorkNotificationOpened", out var legacy) ||
    legacy != Guid.Parse("36e48a5a-628b-277e-1ab6-2a273a97a779")) throw new Exception("Legacy identifier changed");
var guid = Guid.NewGuid();
if (new ShortGuid(guid).Guid != guid) throw new Exception("Identifier changed");
RowVersion rowVersion = 42UL;
if ((byte[])rowVersion is not { Length: 8 }) throw new Exception("Rowversion changed");
using var metadata = new InformationSchemaContext("Server=localhost;Database=metadata;Integrated Security=true");
if (!metadata.Tables.ToQueryString().Contains("INFORMATION_SCHEMA")) throw new Exception("Metadata mapping missing");
Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
EOF
  dotnet restore "$project" --configfile "$probe_dir/NuGet.Config"
  DOTNET_ROLL_FORWARD=LatestPatch dotnet run --project "$project" --no-restore
 done
