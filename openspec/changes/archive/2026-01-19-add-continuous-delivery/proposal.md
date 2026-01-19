# Change: Add Continuous Deployment to Railway

## Why
Currently, the project has CI (Continuous Integration) pipelines that validate code quality on pull requests, but deployment to Railway requires manual intervention. Adding CD (Continuous Deployment) will automate the deployment process after successful merges to `master`, reducing manual effort and ensuring consistent deployments. The solution must be designed to scale easily across the entire monorepo, allowing new services to be added to CD with minimal configuration effort.

## What Changes

### Service Preparation (Prerequisites)
- **Health endpoint**: Add `/health` endpoint to each service for Railway health checks
- **Version property**: Ensure each service's `.csproj` has `<Version>1.0.0</Version>` property
- **PORT binding**: Verify services bind to `$PORT` environment variable and `0.0.0.0`

### CI/CD Workflow
- Extend the reusable CI template workflow (`template-dotnet-ci.yml`) to include a deployment job (using `workflow_call` trigger)
- Add Railway CLI integration to GitHub Actions workflows using Docker image (`ghcr.io/railwayapp/cli:latest`)
- Configure per-service caller workflows that deploy only when their respective service code changes
- **Strict isolation**: Each caller workflow uses GitHub Actions path filtering (`on.push.paths`, `on.pull_request.paths`) to ensure only the affected service deploys when its code changes in a PR
- **GitHub Actions best practices**: Path filtering in caller workflows (reusable workflows don't support path filters), minimal permissions (`contents: write`), secrets via `${{ secrets.RAILWAY_TOKEN }}`
- **Concurrency control**: Use GitHub Actions concurrency groups to prevent race conditions when multiple PRs merge simultaneously
- **Commit loop prevention**: Use `[skip ci]` marker and `github.actor` check to prevent infinite workflow loops

### Secrets Management
- **Two-tier approach**: Application secrets (Telegram tokens, API keys) in Railway Variables, CI/CD secrets (Railway token) in GitHub repository secrets

### Railway Configuration
- Add Railway project configuration files (`railway.json` with JSON schema) for each service, following Railway best practices
- **Single project, multiple services**: One Railway project containing all backend services
- Ensure deployments only occur after successful CI validation passes
- **Robust Verification**: Implement wait-and-verify logic to poll deployment status after trigger, ensuring only successful deployments are tagged

### Versioning & Release
- **Automatic versioning**: Use `Versionize` (.NET tool) to increment version numbers based on semantic commit types
- **Changelog generation**: Automatically generate `CHANGELOG.md` entries using `Versionize`
- **Git tagging**: Use `Versionize` to create and push git tags (e.g., `notification-service/v1.3.0`)
- **First release handling**: Gracefully handle initial deployment when no previous tags exist

### Scalability & Onboarding
- **Design for scalability**: The CD solution SHALL be easily extensible to new services with minimal configuration (copy-paste caller workflow file pattern, add Railway config)
- **Documentation**: Step-by-step guide for adding new services to CD pipeline

## Impact

### Affected Specs
- `ci-pipeline` (add CD requirements, health check, concurrency, rollback)

### Affected Code
- `.github/workflows/template-dotnet-ci.yml` (install and run `Versionize`)
- `.github/workflows/*-service-ci.yml` (add path filters, permissions, secrets passing)
- `railway.json` files per service (with JSON schema)
- Service `.csproj` files (add/update `<Version>` property)
- Service `Bootstrap/ServiceBootstrap.cs` files (add health endpoint)
- `CHANGELOG.md` files per service (auto-generated)

### Railway-Specific
- Services must bind to `$PORT` environment variable and `0.0.0.0`
- Use Railpack builder with auto-detection
- Application secrets stored in Railway Variables per service
- Single Railway project with multiple services (one per backend microservice)
- Health check endpoint required for deployment verification
- **Internal services**: notification-service is internal only (no public domain), accessible via `<service>.railway.internal`
- **Observability**: Utilize Railway's built-in logs, metrics, and Log Explorer with structured JSON logging
- **Railway CLI**: Use for local development (`railway run`) and production debugging (`railway ssh`, `railway logs`)

### GitHub Actions-Specific
- Path filtering in caller workflows (reusable workflows don't support path filters)
- CI/CD secrets management via repository secrets
- Workflow permissions (`contents: write`)
- `GITHUB_TOKEN` for git operations (commits, tags)
- Concurrency groups for race condition prevention
- `[skip ci]` for commit loop prevention

### Secrets Separation
- Application runtime secrets (Telegram tokens, database credentials) → Railway Variables
- CI/CD secrets (Railway deployment token, service ID) → GitHub secrets

### Documentation Deliverables
- `docs/ci-cd/README.md` — CI/CD overview document (workflow, versioning, secrets)
- `docs/ci-cd/adding-service-to-cd.md` — Step-by-step onboarding guide for new services
