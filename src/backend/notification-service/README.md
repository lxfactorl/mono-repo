# Notification Service

This is a backend service for the Monorepo, built with .NET Core using the **Minimal API** pattern.

## Architecture & Conventions

### 1. Solution Management
This service folder is a self-contained unit:
- **`NotificationService.slnx`**: The primary entry point for working specifically on this service. It contains both the service and its test projects.
- **Project Structure**:
  - `NotificationService/`: The main executable project (Api, Domain, Infra).
  - `NotificationService.Tests/`: The test suite (Unit and E2E tiers).

### 2. CI/CD Integration
- **Workflow**: Monitored by `.github/workflows/notification-service-ci.yml`.
- **Quality Gates**:
  - **Zero Warnings**: Build fails on any compiler warning.
  - **80% Coverage**: Merges are blocked if line coverage drops below this threshold.
  - **Formatting**: `dotnet format` is enforced.
  - **Security**: Vulnerable package scanning is performed on every PR.

### 3. Key Design Patterns
- **Minimal APIs**: No Controllers; endpoints are registered via extension methods.
- **Internal by Default**: Logic is kept `internal` and only exposed to tests via `InternalsVisibleTo`.
- **Validated Options**: Configuration uses the "Validated Singleton Options" pattern to ensure settings are correct at startup.

## Development

To work on this service in isolation:
```bash
# Using the dedicated service solution
dotnet build NotificationService.slnx
dotnet test NotificationService.slnx
```

To run the specific CI validation locally:
```bash
# From the monorepo root
dotnet test src/backend/notification-service --collect:"XPlat Code Coverage"
```
