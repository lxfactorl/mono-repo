#!/usr/bin/env pwsh
# Local CI Quality Gate - mirrors GitHub CI exactly
# Usage: ./ci.ps1 [service-path] [coverage-threshold]
# Example: ./ci.ps1 src/backend/notification-service 80

param(
    [Parameter(Mandatory=$true)]
    [string]$ServicePath,
    
    [Parameter(Mandatory=$false)]
    [int]$CoverageThreshold = 80
)

$ErrorActionPreference = "Stop"
$Host.UI.RawUI.WindowTitle = "Local CI - $ServicePath"

function Write-Step {
    param([string]$Step, [string]$Status = "RUNNING")
    $color = switch ($Status) {
        "RUNNING" { "Cyan" }
        "PASS"    { "Green" }
        "FAIL"    { "Red" }
        default   { "White" }
    }
    Write-Host "[$Status] $Step" -ForegroundColor $color
}

function Invoke-Step {
    param([string]$Name, [scriptblock]$Command)
    Write-Step $Name "RUNNING"
    try {
        & $Command
        if ($LASTEXITCODE -ne 0) { throw "Exit code: $LASTEXITCODE" }
        Write-Step $Name "PASS"
    }
    catch {
        Write-Step $Name "FAIL"
        Write-Host "Error: $_" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "  LOCAL CI QUALITY GATE" -ForegroundColor Yellow
Write-Host "  Service: $ServicePath" -ForegroundColor Yellow
Write-Host "  Coverage Threshold: $CoverageThreshold%" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

# 1. Restore
Invoke-Step "Restore" {
    dotnet restore $ServicePath
}

# 2. Format Check
Invoke-Step "Format Check" {
    dotnet format $ServicePath --verify-no-changes
}

# 3. Security Scan
Invoke-Step "Security Scan" {
    dotnet list $ServicePath package --vulnerable --include-transitive
}

# 4. Build (with warnings as errors)
Invoke-Step "Build (zero warnings)" {
    dotnet build $ServicePath --no-restore /p:TreatWarningsAsErrors=true
}

# 5. Test with Coverage
Invoke-Step "Test with Coverage" {
    $coverageDir = "$ServicePath/coverage"
    if (Test-Path $coverageDir) { Remove-Item $coverageDir -Recurse -Force }
    dotnet test $ServicePath --no-build --collect:"XPlat Code Coverage" --results-directory $coverageDir
}

# 6. Verify Coverage
Invoke-Step "Verify Coverage ($CoverageThreshold%)" {
    # Find the coverage file
    $coverageFile = Get-ChildItem -Path "$ServicePath/coverage" -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1
    if (-not $coverageFile) { throw "Coverage file not found" }
    
    # Parse coverage from XML
    [xml]$coverage = Get-Content $coverageFile.FullName
    $lineRate = [math]::Round([double]$coverage.coverage.'line-rate' * 100, 2)
    
    Write-Host "  Line Coverage: $lineRate%" -ForegroundColor White
    
    if ($lineRate -lt $CoverageThreshold) {
        throw "Coverage $lineRate% is below threshold $CoverageThreshold%"
    }
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  ALL QUALITY GATES PASSED!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green
