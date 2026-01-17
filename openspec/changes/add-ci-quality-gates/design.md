# Design: CI Quality Gates Strategy

## Context
We are implementing a CI pipeline for a .NET Monorepo containing multiple backend services and client applications.
The goal is to ensure code quality (zero warnings, formatting, coverage) while remaining efficient and scalable as the number of services grows.

## Constraints
- **Granularity**: We want to validate/deploy *Services*, not the whole repository indiscriminately.
- **Efficiency**: Avoid running tests for Service B if only Service A changed.
- **Maintainability**: Adding a new service should require minimal CI configuration effort.
- **CD Alignment**: The CI structure must pave the way for per-service Continuous Deployment.

## Decision: Reusable Workflows with Path Filtering

We will adopt a **Reusable Workflow** pattern (also known as "Called Workflows" in GitHub Actions).

### 1. The "Template" (Reusable Workflow)
We will create a centralized logic definition (e.g., `.github/workflows/template-dotnet-ci.yml`).
This template encapsulates the "Quality Standard":
- Setup .NET
- Restore dependencies
- Build (Strict mode)
- Format Check
- Security Scan
- Test (with coverage)

### 2. The "Trigger" (Caller Workflows)
Each service will have a tiny, specific workflow file (e.g., `.github/workflows/backend-notification-service.yml`).
This file defines:
- **Triggers**: Uses `paths` filters to run ONLY when that service's code (or shared libs) changes.
- **Context**: Passes the service's project path to the template.

### Example Structure
```yaml
# .github/workflows/backend-notification-service.yml
name: Notification Service CI
on:
  pull_request:
    paths:
      - 'src/backend/notification-service/**'
      - 'src/backend/shared/**' # Also trigger if shared code changes
jobs:
  validate:
    uses: ./.github/workflows/template-dotnet-ci.yml
    with:
      service_name: 'NotificationService'
      project_path: 'src/backend/notification-service'
```

## Alternatives Considered

### Option A: Single Monolithic Build
- **Pros**: Simplest to setup (one `.sln` build). Guaranteed consistency.
- **Cons**: Slow. feedback loop gets longer as repo grows. Testing unrelated components.
- **Verdict**: Rejected. Too inefficient for 10+ services.

### Option B: Smart Monorepo Tools (Nx, Bazel)
- **Pros**: Intelligent graph-based execution.
- **Cons**: High complexity/learning curve to introduce right now.
- **Verdict**: Rejected for now (YAGNI). Can migrate later if "dumb" path filtering becomes unmanageable.

## Status Checks Policy
GitHub requires specific status check names for Branch Protection.
With dynamic/multiple workflows, we don't have a single "CI Passed" check.
**Strategy**: We will rely on the specific `Notification Service CI / validate` check for that folder's protection rules, or use a "Path Filter" action within a single workflow if GitHub protection rules become cumbersome to manage per-service.

For this iteration, **one workflow file per service** calling a **shared template** is the best balance of simplicity and feature requirements.
