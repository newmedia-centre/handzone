<#
  Install HANDZONe Grasshopper plugin (Rhino, net48)
  - Builds via dotnet (preferred), falls back to MSBuild if available
  - Detects actual output folder (bin/Debug or bin/Debug/net48)
  - Copies .gha and .dll dependencies to %APPDATA%\Grasshopper\Libraries\Handzone
#>

$ErrorActionPreference = 'Stop'

$ProjectRoot = Split-Path (Split-Path $PSScriptRoot) -Parent   # repo root
$ProjFile    = Join-Path $ProjectRoot "grasshopper\Handzone.csproj"
$BinRoot     = Join-Path $ProjectRoot "grasshopper"
$DestDir     = Join-Path $env:APPDATA "Grasshopper\Libraries\Handzone"

Write-Host "Project: $ProjFile" -ForegroundColor DarkGray

# Build (dotnet preferred)
function Invoke-Build {
    param($ProjOrSolution)
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        Write-Host "Restoring and building with dotnet..." -ForegroundColor Cyan
        try {
            dotnet restore $ProjOrSolution 2>&1 | Write-Verbose
            $b = dotnet build $ProjOrSolution -c Debug 2>&1
            if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }
            $b | Write-Verbose
            return $true
        }
        catch {
            Write-Host "dotnet build failed: $_" -ForegroundColor Red
            return $false
        }
    }

    if (Get-Command msbuild.exe -ErrorAction SilentlyContinue) {
        Write-Host "dotnet not found or failed; building with MSBuild..." -ForegroundColor Yellow
        try {
            msbuild.exe $ProjOrSolution /t:Build /p:Configuration=Debug /nologo 2>&1 | Write-Verbose
            if ($LASTEXITCODE -ne 0) { throw "msbuild failed" }
            return $true
        }
        catch {
            Write-Host "msbuild build failed: $_" -ForegroundColor Red
            return $false
        }
    }

    Write-Host "No build tool (dotnet or MSBuild) found. Proceeding with existing binaries..." -ForegroundColor Yellow
    return $false
}

$built = Invoke-Build -ProjOrSolution $ProjFile

# Locate output folder containing Handzone.gha - search both bin and obj trees
$searchRoots = @(
    "$BinRoot\bin",
    "$BinRoot\obj",
    "$BinRoot\bin\Debug",
    "$BinRoot\bin\Debug\net48",
    "$BinRoot\bin\Release\net48"
)

$ghaPath = $null
foreach ($root in $searchRoots) {
    if (-not (Test-Path $root)) { continue }
    $found = Get-ChildItem -Path $root -Recurse -Filter "Handzone.gha" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) { $ghaPath = $found.FullName; break }
}

if (-not $ghaPath) {
    # last-resort: search entire project folder
    $found = Get-ChildItem -Path (Join-Path $ProjectRoot "grasshopper") -Recurse -Filter "Handzone.gha" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($found) { $ghaPath = $found.FullName }
}

if (-not $ghaPath) {
    Write-Host "Could not locate Handzone.gha output after build." -ForegroundColor Red
    if (-not $built) {
        Write-Host "Attempted to build but no artifact was found. If you are targeting net48, ensure the .NET Framework 4.8 Developer Pack / targeting pack is installed or build the solution in Visual Studio." -ForegroundColor Yellow
    }
    throw "Could not locate build output (Handzone.gha). Ensure the project builds successfully and produces a .gha file."
}

$OutDir = Split-Path $ghaPath -Parent
Write-Host "Using output: $OutDir" -ForegroundColor DarkGray

# Prepare destination
New-Item -ItemType Directory -Force -Path $DestDir | Out-Null
Write-Host "Installing to $DestDir" -ForegroundColor Cyan

# Remove previous version (optional)
Get-ChildItem $DestDir -Include "Handzone.*" -File -Recurse -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue

# Copy plugin and dependencies (copy any .gha/.dll from output)
$toCopy = Get-ChildItem -Path (Join-Path $OutDir '*') -File -Include *.gha,*.dll -ErrorAction SilentlyContinue
if (-not $toCopy) {
    # Maybe dotnet produced a .dll but not a .gha; try to find Handzone.dll and synthesize a .gha
    $dll = Get-ChildItem $OutDir -File -Filter "Handzone.dll" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($dll) {
    Write-Host 'Found Handzone.dll but no .gha - creating Handzone.gha from the DLL' -ForegroundColor Yellow
        $ghaSynth = Join-Path $OutDir "Handzone.gha"
        Copy-Item -Path $dll.FullName -Destination $ghaSynth -Force
        # Recompute items to copy
    $toCopy = Get-ChildItem -Path (Join-Path $OutDir '*') -File -Include *.gha,*.dll -ErrorAction SilentlyContinue
    }
}
if (-not $toCopy) {
    throw "No plugin artifacts found in $OutDir."
}
$toCopy | Copy-Item -Destination $DestDir -Force

# Unblock files (avoid Windows 'downloaded' block)
Get-ChildItem -Path (Join-Path $DestDir '*') -File -Include *.gha,*.dll | ForEach-Object { Unblock-File $_.FullName -ErrorAction SilentlyContinue }

# Try to co-locate Robots plugin to guarantee load order (optional but fixes Yak load order issues)
if ($robotsPresent -and $robotsPath) {
    try {
        $robotsDir = Split-Path $robotsPath -Parent
        $robotsGha = Get-ChildItem -Path $robotsDir -Filter 'Robots.gha' -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($robotsGha) {
            Copy-Item -Path $robotsGha.FullName -Destination (Join-Path $DestDir 'Robots.gha') -Force
            Write-Host "Copied Robots.gha into Handzone folder to ensure Robots loads before Handzone." -ForegroundColor DarkGray
        }
        # Try copy Robots.dll and Robots.Grasshopper.dll from the same folder (Yak)
        foreach ($fname in @('Robots.dll','Robots.Grasshopper.dll')) {
            $f = Get-ChildItem -Path $robotsDir -Filter $fname -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($f) { Copy-Item -Path $f.FullName -Destination (Join-Path $DestDir $fname) -Force }
        }
    }
    catch {
        Write-Host "Could not copy Robots plugin files locally: $_" -ForegroundColor Yellow
    }
}

# Also try copy Robots assemblies from NuGet cache if present
try {
    $nugetRoot = Join-Path $env:USERPROFILE '.nuget\packages'
    $pkgRoot = Join-Path $nugetRoot 'robots.rhino'
    if (Test-Path $pkgRoot) {
        $versions = Get-ChildItem -Path $pkgRoot -Directory | Sort-Object Name -Descending
        foreach ($v in $versions) {
            $libNet48 = Join-Path $v.FullName 'lib\net48'
            if (Test-Path $libNet48) {
                foreach ($fname in @('Robots.dll','Robots.Grasshopper.dll')) {
                    $src = Join-Path $libNet48 $fname
                    if (Test-Path $src) {
                        Copy-Item -Path $src -Destination (Join-Path $DestDir $fname) -Force
                        Write-Host "Copied $fname from NuGet cache ($($v.Name))" -ForegroundColor DarkGray
                    }
                }
                break
            }
        }
    }
}
catch {
    Write-Host "NuGet Robots copy skipped: $_" -ForegroundColor Yellow
}

# Report results
Write-Host "Installed files:" -ForegroundColor DarkGray
Get-ChildItem -Path (Join-Path $DestDir '*') -File -Include *.gha,*.dll | ForEach-Object { Write-Host " - " $_.Name }

# Check Robots plugin dependency and version compatibility
$ghLibRoot = Split-Path $DestDir -Parent  # %APPDATA%\Grasshopper\Libraries
$robotsCandidates = @()
if (Test-Path $ghLibRoot) {
    $robotsCandidates += @( Get-ChildItem -Path $ghLibRoot -Recurse -Include 'Robots.Grasshopper.dll','Robots.gha' -ErrorAction SilentlyContinue )
}

# Search Rhino Yak package folders for Robots (Rhino 7/8, case variants)
$yakRoots = @()
foreach ($rv in @('7.0','8.0')) {
    foreach ($pkgName in @('Robots','robots')) {
        $yakRoots += (Join-Path $env:APPDATA ("McNeel\Rhinoceros\packages\$rv\$pkgName"))
    }
}
foreach ($root in $yakRoots) {
    if (Test-Path $root) {
    $robotsCandidates += @( Get-ChildItem -Path $root -Recurse -Include 'Robots.Grasshopper.dll','Robots.gha' -ErrorAction SilentlyContinue )
    }
}

$robotsPresent = $false
$robotsPath = $null
$robotsVersion = $null
foreach ($c in $robotsCandidates) {
    if ($c -and (Test-Path $c.FullName)) {
        $robotsPresent = $true
        $robotsPath = $c.FullName
        try {
            $fv = (Get-Item $c.FullName).VersionInfo.FileVersion
            $robotsVersion = $fv
        } catch {}
        break
    }
}

if (-not $robotsPresent) {
    Write-Host "WARNING: Robots plugin not found in your Grasshopper Libraries or Yak packages." -ForegroundColor Yellow
    Write-Host "Install the Robots plugin for Grasshopper (recommended version 1.9.0), then restart Rhino. Handzone depends on it." -ForegroundColor Yellow
} else {
    Write-Host "Detected Robots.Grasshopper at: $robotsPath" -ForegroundColor DarkGray
    if ($robotsVersion) { Write-Host "Robots detected version: $robotsVersion" -ForegroundColor DarkGray }
    # Compare expected version (based on build-time dependency) and warn if mismatch likely
    $expectedVersion = '1.9.0.0'
    if ($robotsVersion -and ($robotsVersion -ne $expectedVersion)) {
        Write-Host "NOTE: Handzone was built against Robots $expectedVersion but you have $robotsVersion. If you see 'Could not load Robots.Grasshopper 1.9.0.0', either install Robots $expectedVersion or rebuild Handzone targeting your installed Robots version." -ForegroundColor Yellow
    }
}

Write-Host 'HANDZONe Grasshopper plugin installed. Restart Rhino to load it.' -ForegroundColor Green