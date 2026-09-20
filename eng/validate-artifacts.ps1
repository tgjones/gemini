<#
.SYNOPSIS
Validates the Gemini demo build outputs and NuGet package family.

.DESCRIPTION
This script is the package-contract gate used after the solution is built and
packed. It intentionally inspects package contents instead of accepting a
successful `dotnet pack` as proof that the release family is complete.

The expected frameworks, package IDs, assemblies, and dependencies live in
package-contract.json so reviewers can audit or update the contract without
reading the validation implementation. The script verifies:

- both demo shaders were compiled for every retained target framework;
- exactly eight primary and eight symbol packages were produced;
- every primary package contains one assembly for each retained framework;
- nuspec dependency groups and target-specific dependency versions match the
  declared contract;
- all internal Gemini dependencies use the same NBGV-calculated version; and
- every symbol package contains the matching portable PDBs.

Any framework, package, assembly, or dependency change must update the
declarative contract deliberately. This prevents partial per-framework packages
with identical IDs and versions from passing CI.

.PARAMETER PackagePath
Directory containing the `.nupkg` and `.snupkg` files. Defaults to
`build\packages` under the repository root.

.PARAMETER DemoOutputPath
Directory containing the Release output for Gemini.Demo. Defaults to
`src\Gemini.Demo\bin\Release` under the repository root.

.PARAMETER ContractPath
Path to the declarative package contract. Defaults to
`eng\package-contract.json`.

.EXAMPLE
./eng/validate-artifacts.ps1

Validates outputs produced in their standard repository locations.
#>
[CmdletBinding()]
param(
    [string] $PackagePath,
    [string] $DemoOutputPath,
    [string] $ContractPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($PackagePath)) {
    $PackagePath = Join-Path $repositoryRoot 'build\packages'
}

if ([string]::IsNullOrWhiteSpace($DemoOutputPath)) {
    $DemoOutputPath = Join-Path $repositoryRoot 'src\Gemini.Demo\bin\Release'
}

if ([string]::IsNullOrWhiteSpace($ContractPath)) {
    $ContractPath = Join-Path $PSScriptRoot 'package-contract.json'
}

$PackagePath = [System.IO.Path]::GetFullPath($PackagePath)
$DemoOutputPath = [System.IO.Path]::GetFullPath($DemoOutputPath)
$ContractPath = [System.IO.Path]::GetFullPath($ContractPath)

# Assertion helpers include missing and unexpected values in failures so CI logs
# identify the contract difference without requiring artifact inspection.
function Assert-Condition {
    param(
        [Parameter(Mandatory)]
        [bool] $Condition,

        [Parameter(Mandatory)]
        [string] $Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

function Assert-SetEqual {
    param(
        [Parameter(Mandatory)]
        [string[]] $Expected,

        [Parameter(Mandatory)]
        [string[]] $Actual,

        [Parameter(Mandatory)]
        [string] $Description
    )

    $missing = @($Expected | Where-Object { $_ -notin $Actual })
    $unexpected = @($Actual | Where-Object { $_ -notin $Expected })

    Assert-Condition `
        -Condition ($missing.Count -eq 0 -and $unexpected.Count -eq 0) `
        -Message "$Description mismatch. Missing: [$($missing -join ', ')]. Unexpected: [$($unexpected -join ', ')]."
}

function Get-NuspecMetadata {
    param(
        [Parameter(Mandatory)]
        [System.IO.Compression.ZipArchive] $Archive,

        [Parameter(Mandatory)]
        [string] $PackageName
    )

    $nuspecEntries = @($Archive.Entries | Where-Object { $_.FullName -like '*.nuspec' })
    Assert-Condition `
        -Condition ($nuspecEntries.Count -eq 1) `
        -Message "Expected one nuspec in $PackageName, found $($nuspecEntries.Count)."

    $reader = [System.IO.StreamReader]::new($nuspecEntries[0].Open())
    try {
        [xml] $nuspec = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }

    return $nuspec.package.metadata
}

Assert-Condition -Condition (Test-Path -LiteralPath $PackagePath -PathType Container) `
    -Message "Package directory does not exist: $PackagePath"
Assert-Condition -Condition (Test-Path -LiteralPath $DemoOutputPath -PathType Container) `
    -Message "Demo output directory does not exist: $DemoOutputPath"
Assert-Condition -Condition (Test-Path -LiteralPath $ContractPath -PathType Leaf) `
    -Message "Package contract does not exist: $ContractPath"

$contract = Get-Content -LiteralPath $ContractPath -Raw | ConvertFrom-Json
$expectedFrameworks = @($contract.frameworks)
$expectedNuspecFrameworks = @($expectedFrameworks | ForEach-Object nuspec)
$expectedPackages = @{}
foreach ($packageDefinition in @($contract.packages)) {
    Assert-Condition -Condition (-not $expectedPackages.ContainsKey($packageDefinition.id)) `
        -Message "Duplicate package ID '$($packageDefinition.id)' in $ContractPath."

    # Overrides are complete external-dependency sets keyed by the emitted
    # nuspec TFM. Reject unknown keys so a typo cannot silently use defaults.
    $frameworkOverrides = $packageDefinition.PSObject.Properties['externalDependenciesByFramework']
    if ($null -ne $frameworkOverrides) {
        foreach ($frameworkOverride in $frameworkOverrides.Value.PSObject.Properties) {
            Assert-Condition -Condition ($frameworkOverride.Name -in $expectedNuspecFrameworks) `
                -Message "Package '$($packageDefinition.id)' has an external dependency override for unknown framework '$($frameworkOverride.Name)'."
        }
    }

    $expectedPackages[$packageDefinition.id] = $packageDefinition
}

# A solution build must compile the non-packable demo and its HLSL items. Checking
# those outputs closes the gap left by a package-only build.
foreach ($framework in $expectedFrameworks) {
    foreach ($shaderName in @($contract.shaders)) {
        $shaderPath = Join-Path $DemoOutputPath "$($framework.build)\Modules\FilterDesigner\ShaderEffects\$shaderName"
        Assert-Condition -Condition (Test-Path -LiteralPath $shaderPath -PathType Leaf) `
            -Message "Missing compiled HLSL shader: $shaderPath"
        Assert-Condition -Condition ((Get-Item -LiteralPath $shaderPath).Length -gt 0) `
            -Message "Compiled HLSL shader is empty: $shaderPath"
    }
}

Add-Type -AssemblyName System.IO.Compression

$packages = @(
    Get-ChildItem -LiteralPath $PackagePath -File |
        Where-Object { $_.Extension -eq '.nupkg' } |
        Sort-Object Name
)
$symbolPackages = @(
    Get-ChildItem -LiteralPath $PackagePath -File |
        Where-Object { $_.Extension -eq '.snupkg' } |
        Sort-Object Name
)

Assert-Condition -Condition ($packages.Count -eq $expectedPackages.Count) `
    -Message "Expected $($expectedPackages.Count) NuGet packages, found $($packages.Count)."
Assert-Condition -Condition ($symbolPackages.Count -eq $expectedPackages.Count) `
    -Message "Expected $($expectedPackages.Count) symbol packages, found $($symbolPackages.Count)."

$actualPackageIds = [System.Collections.Generic.List[string]]::new()
$actualVersions = [System.Collections.Generic.List[string]]::new()

# Validate each primary package and its matching symbol package as one coherent
# release family. NuGet paths use forward slashes on every host.
foreach ($package in $packages) {
    $archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)
    try {
        $metadata = Get-NuspecMetadata -Archive $archive -PackageName $package.Name
        $packageId = [string] $metadata.id
        $packageVersion = [string] $metadata.version

        Assert-Condition -Condition $expectedPackages.ContainsKey($packageId) `
            -Message "Unexpected package ID '$packageId' in $($package.Name)."
        Assert-Condition -Condition ($package.Name -ceq "$packageId.$packageVersion.nupkg") `
            -Message "Package filename '$($package.Name)' does not match nuspec identity '$packageId.$packageVersion'."

        $actualPackageIds.Add($packageId)
        $actualVersions.Add($packageVersion)

        $definition = $expectedPackages[$packageId]
        $entryNames = @($archive.Entries | ForEach-Object FullName)

        foreach ($framework in $expectedFrameworks) {
            $assemblyPath = "lib/$($framework.asset)/$($definition.assembly).dll"
            Assert-Condition -Condition ($assemblyPath -in $entryNames) `
                -Message "$packageId is missing $assemblyPath."
        }

        $groups = @($metadata.dependencies.group)
        $actualGroupFrameworks = @($groups | ForEach-Object { [string] $_.targetFramework })
        Assert-SetEqual `
            -Expected @($expectedFrameworks | ForEach-Object nuspec) `
            -Actual $actualGroupFrameworks `
            -Description "$packageId dependency groups"

        # Internal package relationships remain identical for every TFM and use
        # the coherent version read from the package currently being inspected.
        $expectedDependencies = @{}
        foreach ($internalDependency in @($definition.internalDependencies)) {
            $expectedDependencies[$internalDependency] = $packageVersion
        }

        foreach ($group in $groups) {
            # Start with a fresh set for each nuspec group. A framework override
            # replaces the full external set, so an empty object asserts none.
            $expectedGroupDependencies = $expectedDependencies.Clone()
            $expectedExternalDependencies = $definition.externalDependencies
            $frameworkOverrides = $definition.PSObject.Properties['externalDependenciesByFramework']
            if ($null -ne $frameworkOverrides) {
                $frameworkOverride = $frameworkOverrides.Value.PSObject.Properties[[string] $group.targetFramework]
                if ($null -ne $frameworkOverride) {
                    $expectedExternalDependencies = $frameworkOverride.Value
                }
            }

            foreach ($externalDependency in $expectedExternalDependencies.PSObject.Properties) {
                $expectedGroupDependencies[$externalDependency.Name] = [string] $externalDependency.Value
            }

            $actualDependencies = @{}
            foreach ($dependency in @($group.dependency)) {
                $dependencyId = [string] $dependency.id
                Assert-Condition -Condition (-not $actualDependencies.ContainsKey($dependencyId)) `
                    -Message "$packageId has duplicate dependency '$dependencyId' in $($group.targetFramework)."
                $actualDependencies[$dependencyId] = [string] $dependency.version
            }

            Assert-SetEqual `
                -Expected @($expectedGroupDependencies.Keys) `
                -Actual @($actualDependencies.Keys) `
                -Description "$packageId dependencies for $($group.targetFramework)"

            foreach ($dependency in $expectedGroupDependencies.GetEnumerator()) {
                Assert-Condition -Condition ($actualDependencies[$dependency.Key] -ceq $dependency.Value) `
                    -Message "$packageId dependency '$($dependency.Key)' in $($group.targetFramework) has version '$($actualDependencies[$dependency.Key])'; expected '$($dependency.Value)'."
            }
        }
    }
    finally {
        $archive.Dispose()
    }

    $symbolPackagePath = Join-Path $PackagePath "$packageId.$packageVersion.snupkg"
    Assert-Condition -Condition (Test-Path -LiteralPath $symbolPackagePath -PathType Leaf) `
        -Message "Missing symbol package for $packageId $packageVersion."

    $symbolArchive = [System.IO.Compression.ZipFile]::OpenRead($symbolPackagePath)
    try {
        $symbolEntries = @($symbolArchive.Entries | ForEach-Object FullName)
        foreach ($framework in $expectedFrameworks) {
            $pdbPath = "lib/$($framework.asset)/$($definition.assembly).pdb"
            Assert-Condition -Condition ($pdbPath -in $symbolEntries) `
                -Message "$packageId symbol package is missing $pdbPath."
        }
    }
    finally {
        $symbolArchive.Dispose()
    }
}

Assert-SetEqual `
    -Expected @($expectedPackages.Keys) `
    -Actual @($actualPackageIds) `
    -Description 'Package IDs'

$distinctVersions = @($actualVersions | Sort-Object -Unique)
Assert-Condition -Condition ($distinctVersions.Count -eq 1) `
    -Message "Expected one coherent package version, found: $($distinctVersions -join ', ')."

$shaderCount = $expectedFrameworks.Count * @($contract.shaders).Count
Write-Host "Validated $shaderCount compiled shaders and $($expectedPackages.Count) packages with symbols at version $($distinctVersions[0])."
