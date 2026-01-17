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
- **Hosting**: Railway (container-based deployment)
- **Source Control**: GitHub (private repository)
- **Monorepo Tooling**: Shared build configuration at root level

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
- Main branch protected, requires PR reviews
- Feature branches: `feature/<description>`
- Conventional commits: `feat:`, `fix:`, `chore:`, `docs:`
- Each service deployable independently via Railway
- **CI/CD Pipeline**:
  - **Pattern**: Reusable Workflows (`.github/workflows/templates/dotnet-ci.yml`) called by service-specific workflows.
  - **Gates**: Zero Warnings, Formatting (`dotnet format`), Security Audit, 80% Coverage.
  - **Triggers**: Path-based filtering ensures efficient execution per service.

## Domain Context
This is a multi-service platform. Individual service domains will be documented in their respective `openspec/` directories. Root-level OpenSpec handles:
- Cross-service integrations
- Shared infrastructure decisions
- Monorepo-wide conventions

## Important Constraints
- All backend services MUST use the shared code style and linter configuration
- Each service MUST have its own OpenSpec documentation
- Services MUST be independently deployable to Railway
- Private repository - no public exposure of code or secrets

## External Dependencies
- **Railway**: Platform-as-a-Service for deployment
- **GitHub**: Source control and CI/CD triggers
- Additional per-service dependencies documented in respective service specs
