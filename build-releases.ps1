# Build script for creating releases for all platforms

param(
[string]$Version = "2.1.0",
[string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "Building Status Keeper v$Version for all platforms..." -ForegroundColor Cyan

# Project path
$ProjectPath = ".\StatusKeeperTerminalApp\StatusKeeperTerminalApp.csproj"

# Clean previous builds
Write-Host ""
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path ".\releases") {
    Remove-Item -Path ".\releases" -Recurse -Force
}
New-Item -ItemType Directory -Path ".\releases" | Out-Null

# Build configurations (runtime identifiers for .NET)
$runtimes = @(
@{ RID = "win-x64"; Name = "Windows x64" },
@{ RID = "win-x86"; Name = "Windows x86" },
@{ RID = "win-arm64"; Name = "Windows ARM64" },
# @{ RID = "linux-x64"; Name = "Linux x64" },
# @{ RID = "linux-arm64"; Name = "Linux ARM64" },
@{ RID = "osx-x64"; Name = "macOS x64" },
@{ RID = "osx-arm64"; Name = "macOS ARM64" }
)

foreach ($runtime in $runtimes) {
    $rid = $runtime.RID
    $name = $runtime.Name
    $outputPath = ".\releases\$rid"
    
    Write-Host ""
    Write-Host "Building $name ($rid)..." -ForegroundColor Green
    
    # Publish the application
    dotnet publish $ProjectPath -c $Configuration -r $rid --self-contained true -p:PublishSingleFile=true -p:Version=$Version -o $outputPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully built $name" -ForegroundColor Green
        
        # Create zip archive
        $zipName = "StatusKeeper-$rid-v$Version.zip"
        Compress-Archive -Path "$outputPath\*" -DestinationPath ".\releases\$zipName" -Force
        Write-Host "Created archive $zipName" -ForegroundColor Green
    } else {
        Write-Host "Failed to build $name" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Build Summary ===" -ForegroundColor Cyan
Get-ChildItem ".\releases\*.zip" | ForEach-Object {
    $size = [math]::Round($_.Length / 1MB, 2)
    Write-Host "$($_.Name) - $size MB" -ForegroundColor White
}

Write-Host ""
Write-Host "All releases built successfully!" -ForegroundColor Green
Write-Host "Releases are available in the releases folder." -ForegroundColor Cyan
