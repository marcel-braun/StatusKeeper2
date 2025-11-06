#!/bin/bash

# Upload GitHub Release Script
# Prerequisites: GitHub CLI (gh) must be installed and authenticated
# Install: https://github.com/cli/cli#installation
# Authenticate: gh auth login

set -e

VERSION=${1:-"2.1.0"}
RELEASE_NAME="Status Keeper v$VERSION"
RELEASE_NOTES=${2:-"Release v$VERSION"}
DRAFT=${DRAFT:-false}
PRERELEASE=${PRERELEASE:-false}

echo "GitHub Release Upload Script"
echo "Version: $VERSION"
echo ""

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo "ERROR: GitHub CLI (gh) is not installed!"
    echo "Install it from: https://github.com/cli/cli#installation"
    echo "Then authenticate with: gh auth login"
    exit 1
fi

GH_VERSION=$(gh --version | head -n 1)
echo "GitHub CLI found: $GH_VERSION"

# Check if authenticated
if ! gh auth status &> /dev/null; then
    echo "ERROR: GitHub CLI is not authenticated!"
    echo "Please run: gh auth login"
    exit 1
fi

echo "GitHub CLI is authenticated"

# Check if releases folder exists
if [ ! -d "./releases" ]; then
    echo "ERROR: releases folder not found!"
    echo "Please run build-releases.sh first"
    exit 1
fi

# Get all zip files
ZIP_FILES=(./releases/*.zip)
if [ ! -e "${ZIP_FILES[0]}" ]; then
    echo "ERROR: No zip files found in releases folder!"
    echo "Please run build-releases.sh first"
    exit 1
fi

echo ""
echo "Found ${#ZIP_FILES[@]} release files:"
for zip in "${ZIP_FILES[@]}"; do
    SIZE=$(du -h "$zip" | cut -f1)
    echo "  - $(basename "$zip") ($SIZE)"
done

echo ""
echo "Creating GitHub Release..."

# Build gh release create command
TAG_NAME="v$VERSION"
GH_ARGS=(
    "release" "create" "$TAG_NAME"
    "--title" "$RELEASE_NAME"
    "--notes" "$RELEASE_NOTES"
)

if [ "$DRAFT" = "true" ]; then
    GH_ARGS+=("--draft")
    echo "Creating as DRAFT release"
fi

if [ "$PRERELEASE" = "true" ]; then
    GH_ARGS+=("--prerelease")
    echo "Creating as PRERELEASE"
fi

# Add all zip files
GH_ARGS+=("${ZIP_FILES[@]}")

# Create the release
echo ""
echo "Uploading to GitHub..."

if gh "${GH_ARGS[@]}"; then
    echo ""
    echo "SUCCESS! Release created successfully!"
    echo "View it at: https://github.com/marcel-braun/StatusKeeper2/releases/tag/$TAG_NAME"
else
    echo ""
    echo "ERROR: Failed to create release!"
    exit 1
fi

echo ""
echo "Done! 🎉"
