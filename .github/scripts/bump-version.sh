#!/bin/bash
set -e

SERVICE_NAME=$1
PROJECT_PATH=$2

if [ -z "$SERVICE_NAME" ] || [ -z "$PROJECT_PATH" ]; then
    echo "Usage: $0 <service-name> <project-path>"
    exit 1
fi

# Find the csproj file specifically in the service project directory, not the tests
CSPROJ_FILE=$(find "$PROJECT_PATH" -maxdepth 2 -name "$SERVICE_NAME.csproj" | head -n 1)

if [ ! -f "$CSPROJ_FILE" ]; then
    # Fallback to any csproj if specific name not found
    CSPROJ_FILE=$(find "$PROJECT_PATH" -maxdepth 2 -name "*.csproj" | grep -v "Tests" | head -n 1)
fi

if [ ! -f "$CSPROJ_FILE" ]; then
    echo "Error: .csproj file not found in $PROJECT_PATH"
    exit 1
fi

echo "Targeting project file: $CSPROJ_FILE"

# Extract version using a more robust grep pattern for XML
CURRENT_VERSION=$(grep -oP '(?<=<Version>)[^<]+' "$CSPROJ_FILE" || echo "1.0.0")
echo "Current Version: $CURRENT_VERSION"

# Get last tag for this service
LAST_TAG=$(git tag -l "$SERVICE_NAME/v*" --sort=-v:refname | head -n 1)

if [ -z "$LAST_TAG" ]; then
    echo "No previous tag found for $SERVICE_NAME. Starting from initial release commits."
    COMMITS=$(git log --pretty=format:"%s")
else
    echo "Last Tag: $LAST_TAG"
    # Filter commits that affected the project path
    COMMITS=$(git log "$LAST_TAG..HEAD" --pretty=format:"%s" -- "$PROJECT_PATH")
fi

MAJOR=$(echo "$CURRENT_VERSION" | cut -d. -f1)
MINOR=$(echo "$CURRENT_VERSION" | cut -d. -f2)
PATCH=$(echo "$CURRENT_VERSION" | cut -d. -f3)

BUMP="none"

if [ -z "$COMMITS" ]; then
    echo "No new commits detected in $PROJECT_PATH"
else
    while IFS= read -r commit; do
        if echo "$commit" | grep -qi "BREAKING CHANGE:"; then
            BUMP="major"
            break
        elif echo "$commit" | grep -qi "feat:"; then
            if [ "$BUMP" != "major" ]; then BUMP="minor"; fi
        elif echo "$commit" | grep -qi "fix:"; then
            if [ "$BUMP" == "none" ]; then BUMP="patch"; fi
        fi
    done <<< "$COMMITS"
fi

if [ "$BUMP" == "none" ]; then
    echo "No version bump needed based on commits."
    echo "updated_version=$CURRENT_VERSION" >> $GITHUB_OUTPUT
    echo "needs_bump=false" >> $GITHUB_OUTPUT
    exit 0
fi

if [ "$BUMP" == "major" ]; then
    MAJOR=$((MAJOR + 1))
    MINOR=0
    PATCH=0
elif [ "$BUMP" == "minor" ]; then
    MINOR=$((MINOR + 1))
    PATCH=0
elif [ "$BUMP" == "patch" ]; then
    PATCH=$((PATCH + 1))
fi

NEW_VERSION="$MAJOR.$MINOR.$PATCH"
echo "New Version: $NEW_VERSION ($BUMP bump)"

# Update .csproj using sed
# Note: On macOS sed -i requires an empty extension, but on Linux it doesn't. 
# GitHub Actions runs on Linux, so we use the standard syntax.
sed -i "s|<Version>$CURRENT_VERSION</Version>|<Version>$NEW_VERSION</Version>|g" "$CSPROJ_FILE"

echo "updated_version=$NEW_VERSION" >> $GITHUB_OUTPUT
echo "needs_bump=true" >> $GITHUB_OUTPUT
