Status: Archived

# Change: Add CI Quality Gates

## Why
Currently, code quality standards (zero warnings, formatting, coverage) are defined in documentation but not enforced automatically. This allows non-compliant code to be merged into `master`, potentially accumulating technical debt and bugs. We need a robust "Quality Gate" to ensure the main branch always remains in a deployable, high-quality state.

## What Changes
- Create a **Reusable Workflow Template** (`.github/workflows/templates/dotnet-ci.yml`) that standardizes the "Quality Gate" logic:
    - Zero Warnings
    - Formatting Compliance
    - Security Scanning
    - Test Coverage (80%)
- Create a **Service-Specific Workflow** for the `NotificationService` that triggers only on relevant path changes.
- Implement **GitHub Repository Rulesets** for the `master` branch to enforce the **Required status checks** (Per-service CI) for all users, including administrators.
- Add Repository Rulesets guidance to `openspec/project.md`.

## Impact
- **Affected Specs**: `ci-pipeline` (New Capability)
- **Affected Code**: `.github/workflows/` (New directory), `Directory.Build.props` (Coverage configuration)
- **Workflow**: All future PRs will require these checks to pass before merging.
