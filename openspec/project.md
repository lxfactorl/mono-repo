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
- EditorConfig for consistent formatting across all services
- Roslyn analyzers enforced as errors in CI
- Naming: PascalCase for public members, camelCase for private fields with `_` prefix
- All public APIs must have XML documentation

### Architecture Patterns
- **Monorepo Structure**:
  ```
  /
  ├── src/
  │   ├── backend/                    # .NET Core services
  │   │   └── <service-name>/
  │   │       ├── <Service>.Api/
  │   │       ├── <Service>.Application/
  │   │       ├── <Service>.Domain/
  │   │       ├── <Service>.Infrastructure/
  │   │       └── <Service>.Tests/
  │   │
  │   └── client/                     # Client applications (TBD)
  │       └── <client-name>/
  │
  └── openspec/                       # Centralized documentation
      ├── project.md                  # This file - monorepo conventions
      ├── AGENTS.md                   # AI assistant instructions
      ├── specs/                      # All capability specs
      │   ├── backend-<service>/      # Per-service specs
      │   └── client-<client>/        # Per-client specs
      └── changes/                    # Proposed changes
  ```
- **Shared Configuration**: Root-level `.editorconfig`, `Directory.Build.props`, and analyzers apply to all services
- Domain-Driven Design principles where applicable

### Testing Strategy
- xUnit as test framework
- Minimum 80% code coverage threshold
- Test projects mirror source structure
- Integration tests for API endpoints
- Unit tests for domain logic

### Git Workflow
- Main branch protected, requires PR reviews
- Feature branches: `feature/<description>`
- Conventional commits: `feat:`, `fix:`, `chore:`, `docs:`
- Each service deployable independently via Railway

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
