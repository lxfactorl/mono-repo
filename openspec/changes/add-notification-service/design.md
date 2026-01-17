# Design: Notification Service Abstraction

## Context
A centralized service to handle outbound notifications. The goal is to build the "brain" and "skeleton" of the service, implementing the core API and the abstraction for delivery channels (providers), without yet binding to concrete third-party APIs.

## Goals / Non-Goals
- **Goals**:
  - Establish a **Shared Infrastructure** project (`src/backend/shared/`) to house monorepo-wide patterns.
  - Define the `INotificationProvider` interface.
  - Implement a REST API using ASP.NET Core Minimal API.
  - Establish a Clean Architecture solution structure.
  - Implement a `LoggingProvider` (Internal) to demonstrate successful routing.
  - Configure the DI container to support dynamic provider registration.
- **Non-Goals**:
  - Implementing concrete Telegram, Email, or SMS providers.
  - Detailed channel-specific configuration (like Bot Tokens).

## Decisions
- **API**: ASP.NET Core Minimal API.
- **Documentation**: Native **OpenAPI 3.0** via `Microsoft.AspNetCore.OpenApi`.
  - **Metadata**: Full enrichment of request/response models using XML comments and OpenAPI attributes.
  - **Status Codes**: Explicit documentation of all expected status codes (202, 400, 500).
- **UI**: **Scalar** for interactive API documentation, configured with detailed service descriptions and contact info.
- **Pattern**: Strategy Pattern for providers. The core `NotificationDispatcher` will iterate through registered providers.
- **Logging**: Serilog with configuration read from `IConfiguration`.
- **Configuration**: "Validated Singleton Options" pattern with Data Annotations.
- **Code Quality**:
  - **Zero Warning Policy**: `TreatWarningsAsErrors` enabled for all projects in the monorepo.
  - **Analyzers**: Use monorepo-level `Directory.Build.props` to enforce Roslyn analyzers across all present and future services.
  - **Formatting**: Root-level `.editorconfig` for a unified coding style across the entire monorepo.
  - **Test Coverage**: Minimum **80% code coverage** threshold enforced globally for all backend services.
  - **Master Gate**: CI pipeline will block merges/pushes to `master` if coverage drops below the 80% threshold.
- **Testing Strategy**:
  - **Tier 1: Unit Tests**: Focus on isolating domain logic and application use cases using **NSubstitute** for mocking dependencies.
  - **Tier 2: E2E / Integration Tests**: Treat the entire service as a black box. Use **WebApplicationFactory** to host the API in-memory and perform real HTTP calls against it.
  - **Mocking**: **NSubstitute**.
  - **Assertions**: **FluentAssertions**.
  - **White-Box Testing**: `InternalsVisibleTo` for unit testing internal modules.
- **Internal Provider**: Include a `LoggingProvider` in the Infrastructure layer for initial testing.
- **Code Organization**:
  - **Shared Infrastructure**: Recurrent patterns (DI extensions like `AddValidatedOptions`, Serilog setup, common middleware) MUST be centralized in `src/backend/shared/` (e.g., `Monorepo.Shared.Infrastructure`).
  - **One Type Per File**: Every class, interface, or record MUST reside in its own dedicated file.
  - **Bootstrap Folder**: Service-specific DI registrations and host configurations MUST be organized within a `Bootstrap` folder.
  - **Clean Program.cs**: The entry point MUST delegate all setup to chained calls, many of which will come from the Shared Infrastructure.
  - **Modular DI**: Each domain or architectural layer manages its own extension classes.
- **Project Structure**:
  - **Standard**: Every service MUST consist of exactly two projects:
    1. `<ServiceName>`: The executable service project containing all layers as namespaces.
    2. `<ServiceName>.Tests`: The test project.
  - **Internal Organization**: Layers (Domain, Application, Infrastructure) are managed via namespaces and directory structures within the single service project.
  - `src/backend/notification-service/`:
    - `NotificationService/`: Single project containing all logic.
    - `NotificationService.Tests/`: XUnit project for all testing tiers.

## Risks / Trade-offs
- **Risk**: Abstraction might be too generic for channel-specific metadata.
  - **Mitigation**: Use a flexible `Properties` dictionary in the domain message model if needed.

## Migration Plan
- N/A (New service)

## Open Questions
- None.
