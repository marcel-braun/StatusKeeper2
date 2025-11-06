#!/bin/bash

# Build script for creating releases for all platforms

VERSION=${1:-"1.0.0"}
CONFIGURATION="Release"

echo "Building Status Keeper v$VERSION for all platforms..."

# Project path
PROJECT_PATH="./StatusKeeperTerminalApp/StatusKeeperTerminalApp.csproj"

# Clean previous builds
echo ""
echo "Cleaning previous builds..."
rm -rf ./releases
mkdir -p ./releases

# Build configurations (runtime identifiers for .NET)
declare -a runtimes=(
    "win-x64:Windows x64"
    "win-x86:Windows x86"
    "win-arm64:Windows ARM64"
    # "linux-x64:Linux x64"
    # "linux-arm64:Linux ARM64"
    "osx-x64:macOS x64"
    "osx-arm64:macOS ARM64"
)

# Build for each platform
for runtime in "${runtimes[@]}"; do
    IFS=':' read -r rid name <<< "$runtime"
    output_path="./releases/$rid"
    
    echo ""
    echo "Building $name ($rid)..."
    
    # Publish the application
    dotnet publish "$PROJECT_PATH" -c "$CONFIGURATION" -r "$rid" --self-contained true -p:PublishSingleFile=true -p:Version="$VERSION" -o "$output_path"
    
    if [ $? -eq 0 ]; then
        echo "Successfully built $name"
        
        # Create zip archive
        zip_name="StatusKeeper-$rid-v$VERSION.zip"
        (cd "$output_path" && zip -q -r "../../releases/$zip_name" *)
        echo "Created archive $zip_name"
        
        # Remove the uncompressed directory to save space
        rm -rf "$output_path"
    else
        echo "Failed to build $name"
    fi
done

echo ""
echo "=== Build Summary ==="
ls -lh ./releases/*.zip 2>/dev/null | awk '{print $9, "-", $5}' | sed 's|./releases/||'

echo ""
echo "All releases built successfully!"
echo "Releases are available in the releases folder."
