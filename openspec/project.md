# Project Context

## Purpose
Monorepo serving as the root for multiple backend services and client-side applications. Designed for deployment to Railway with GitHub as the source repository (private). Each service maintains its own OpenSpec documentation while the root-level OpenSpec handles cross-service changes.

## Tech Stack

### Backend Services
- .NET Core (latest LTS)
- C# with nullable reference types enabled
- .NET Generic Host pattern (DI, configuration, logging)
- Serilog for structured logging

### Client-Side
- Location-aware client applications (stack TBD per project)

### Infrastructure
- **Hosting**: Railway (Docker-based deployment)
- **Source Control**: GitHub (public repository)
- **Monorepo Tooling**: Shared build configuration at root level
- **Deployment Strategy**: **Isolated Docker Builds** using official .NET 10 images (Debian Slim runtime). Shared monorepo context (`Directory.Build.props`, `global.json`) is injected into the service directory prior to deployment.
- **Zero-Config Skeleton**: Standard services leverage a shared deployment skeleton (`.github/templates/`) to automatically provision `Dockerfile` and `railway.json` if missing.


## Project Conventions

### Code Style
- **Zero Warning Policy**: `TreatWarningsAsErrors` enabled globally. Build MUST fail on any compiler warning.
- **Analyzers**: Roslyn analyzers enforced as errors across all projects.
- **Formatting**: EditorConfig for consistent formatting at the monorepo root.
- **Naming**: PascalCase for public members, `_camelCase` for private fields.
- **Documentation**: All public APIs MUST have XML documentation.
- **Access Modifiers**: Default to `internal`. Only expose types as `public` if intended for external consumption. Use `InternalsVisibleTo` for testing.

### Architecture Patterns
- **Monorepo Structure**:
  ```
  /
  ├── src/
  │   ├── backend/                    # .NET Core services
  │   │   └── <service-name>/
  │   │       ├── <Service>/          # Executable service (Api, App, Domain, Infra)
  │   │       └── <Service>.Tests/    # XUnit project for all test tiers
  │   │
  │   └── client/                     # Client applications
  │       └── <client-name>/
  ```
- **Solution Management**:
  - **Service Solutions**: Each service folder MUST contain a dedicated solution file (e.g., `<Service>.slnx`) including the service project and its test project.
  - **Monorepo Solution**: The root-level solution (`Monorepo.sln`) MUST include all projects across the monorepo.
- **Service Architecture**:
  - **Framework**: ASP.NET Core **Minimal APIs** (no Controllers).
  - **API Documentation**: **Scalar** (via `Scalar.AspNetCore`) for OpenAPI UI.
  - **Single Service Project**: Layers managed via namespaces and internal directory hierarchy.
  - **Modular DI**: Dependencies registered via extension methods in a `Bootstrap/` folder.
  - **Configuration**: Use "Validated Singleton Options" pattern (Data Annotations + `ValidateOnStart`).
  - **Clean Program.cs**: Entry point delegates setup to chained extension methods.
- **Shared Configuration**: Root-level `.editorconfig`, `Directory.Build.props`, and analyzers apply to all projects.
- Domain-Driven Design principles where applicable

### Testing Strategy
- **Framework**: xUnit with **NSubstitute** (mocking) and **FluentAssertions**.
- **Coverage**: Minimum **80% line coverage** threshold enforced by CI gate.
- **Tiers**:
  - **Unit Tests**: Business logic isolation using `InternalsVisibleTo`.
  - **E2E Tests**: Black-box validation using `WebApplicationFactory`.

### Git Workflow
- **No Auto-Merge**: AI Agents must NEVER merge Pull Requests without explicit user confirmation.
- Main branch protected, requires PR reviews
- Feature branches: `feature/<description>`
- Conventional commits: `feat:`, `fix:`, `chore:`, `docs:`
- Each service deployable independently via Railway
- **Local CI (Mandatory)**:
  - **Script**: `./ci.ps1 -ServicePath "src/backend/<service-name>" -CoverageThreshold 80`
  - **Rule**: MUST run local CI before creating or updating a PR.
  - **Purpose**: Provides instant feedback loop identical to GitHub CI.
  - **Gates**: Format check, security scan, build (zero warnings), tests, coverage threshold.
- **CI/CD Pipeline**:
  - **Pattern**: **Per-Service CI Workflow** (e.g., `<Service> CI`). triggers on all PRs to enforce gates, but uses path-based filtering for post-merge pushes.
  - **Enforcement**: **GitHub Repository Rulesets** applied to the `master` branch.
    - **Rule**: Require status checks to pass (e.g., `<Service> CI / validate`).
    - **Rule**: Require a pull request before merging (Restricts direct pushes).
    - **Rule**: Include administrators (Enforces rules even for repository owners).
  - **Gates**: Zero Warnings, Formatting (`dotnet format`), Security Audit, 80% Coverage threshold.
  - **Efficiency**: Path-based filtering ensures only affected services run CI on PRs.

## Domain Context
This is a multi-service platform. Individual service domains will be documented in their respective `openspec/` directories. Root-level OpenSpec handles:
- Cross-service integrations
- Shared infrastructure decisions
- Monorepo-wide conventions

## Important Constraints
- All backend services MUST use the shared code style and linter configuration
- Each service MUST have its own OpenSpec documentation
- Services MUST be independently deployable to Railway
- **Security**: Public repository - NEVER check in secrets, credentials, or PII. Use environment variables or a secret manager.

## External Dependencies
- **Railway**: Platform-as-a-Service for deployment
- **GitHub**: Source control and CI/CD triggers
- Additional per-service dependencies documented in respective service specs
