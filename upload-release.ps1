# Upload GitHub Release Script
# Prerequisites: GitHub CLI (gh) must be installed and authenticated
# Install: winget install GitHub.cli
# Authenticate: gh auth login

param(
    [string]$Version = "2.1.0",
    [string]$ReleaseName = "Status Keeper v$Version",
    [string]$ReleaseNotes = "Release v$Version",
    [switch]$Draft = $false,
    [switch]$Prerelease = $false
)

$ErrorActionPreference = "Stop"

Write-Host "GitHub Release Upload Script" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor White
Write-Host ""

# Check if gh CLI is installed
try {
    $ghVersion = gh --version 2>&1
    Write-Host "GitHub CLI found: $($ghVersion[0])" -ForegroundColor Green
} catch {
    Write-Host "ERROR: GitHub CLI (gh) is not installed!" -ForegroundColor Red
    Write-Host "Install it with: winget install GitHub.cli" -ForegroundColor Yellow
    Write-Host "Then authenticate with: gh auth login" -ForegroundColor Yellow
    exit 1
}

# Check if authenticated
try {
    gh auth status 2>&1 | Out-Null
    Write-Host "GitHub CLI is authenticated" -ForegroundColor Green
} catch {
    Write-Host "ERROR: GitHub CLI is not authenticated!" -ForegroundColor Red
    Write-Host "Please run: gh auth login" -ForegroundColor Yellow
    exit 1
}

# Check if releases folder exists
if (-not (Test-Path ".\releases")) {
    Write-Host "ERROR: releases folder not found!" -ForegroundColor Red
    Write-Host "Please run build-releases.ps1 first" -ForegroundColor Yellow
    exit 1
}

# Get all zip files
$zipFiles = Get-ChildItem ".\releases\*.zip"
if ($zipFiles.Count -eq 0) {
    Write-Host "ERROR: No zip files found in releases folder!" -ForegroundColor Red
    Write-Host "Please run build-releases.ps1 first" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Found $($zipFiles.Count) release files:" -ForegroundColor Cyan
$zipFiles | ForEach-Object {
    $size = [math]::Round($_.Length / 1MB, 2)
    Write-Host "  - $($_.Name) ($size MB)" -ForegroundColor White
}

Write-Host ""
Write-Host "Creating GitHub Release..." -ForegroundColor Yellow

# Build gh release create command
$tagName = "v$Version"
$ghArgs = @(
    "release", "create", $tagName,
    "--title", $ReleaseName,
    "--notes", $ReleaseNotes
)

if ($Draft) {
    $ghArgs += "--draft"
    Write-Host "Creating as DRAFT release" -ForegroundColor Yellow
}

if ($Prerelease) {
    $ghArgs += "--prerelease"
    Write-Host "Creating as PRERELEASE" -ForegroundColor Yellow
}

# Add all zip files
foreach ($zipFile in $zipFiles) {
    $ghArgs += $zipFile.FullName
}

# Create the release
try {
    Write-Host ""
    Write-Host "Uploading to GitHub..." -ForegroundColor Cyan
    
    & gh @ghArgs
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "SUCCESS! Release created successfully!" -ForegroundColor Green
        Write-Host "View it at: https://github.com/marcel-braun/StatusKeeper2/releases/tag/$tagName" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "ERROR: Failed to create release!" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Done! 🎉" -ForegroundColor Green
