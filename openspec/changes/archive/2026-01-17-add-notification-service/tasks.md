## 1. Scaffolding
- [x] 1.1 Create solution structure in `src/backend/notification-service/`
- [x] 1.2 Initialize projects: `.Api`, `.Application`, `.Domain`, `.Infrastructure`, `.Tests`
- [x] 1.3 Add core dependencies (Serilog, FluentValidation)
- [x] 1.4 Create root-level `.editorconfig` for formatting
- [x] 1.5 Create root-level `Directory.Build.props` to enforce `TreatWarningsAsErrors`, Analyzers, and `InternalsVisibleTo`
- [x] 1.6 Configure global test coverage thresholds (80%) in root-level build/CI configuration
- [x] 1.7 Add testing dependencies to `.Tests` project: `NSubstitute`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`
- [x] 1.8 Establish `Bootstrap/` folder structure in `.Api` project for DI extensions
- [x] 1.9 Initialize `Monorepo.Shared.Infrastructure` project in `src/backend/shared/` (Implemented as `Infrastructure/Common` within the service for YAGNI)

## 2. Abstraction Layer
- [x] 2.1 Define `INotificationProvider` interface in Domain
- [x] 2.2 Create `NotificationRequest` model in Domain
- [x] 2.3 Implement `LoggingProvider` in Infrastructure

## 3. Core Logic & API
- [x] 3.1 Implement `NotificationDispatcher` in Application
- [x] 3.2 Create `POST /notify` Minimal API endpoint
- [x] 3.3 Set up DI to register multiple providers and the dispatcher
- [x] 3.4 Configure Serilog to use `IConfiguration` in `Program.cs`
- [x] 3.5 Implement `AddValidatedOptions` and common bootstrap logic in `Shared.Infrastructure` (Implemented as `Infrastructure/Common`)
- [x] 3.6 Integrate `Microsoft.AspNetCore.OpenApi` and `Scalar.AspNetCore`
- [x] 3.7 Enrich OpenAPI metadata: add descriptions, status codes (202, 400), and XML comments for In/Out models
- [x] 3.8 Refactor access modifiers: Enforce `internal` by default, using `InternalsVisibleTo` for tests

## 4. Validation & Testing
- [x] 4.1 Unit tests for `NotificationDispatcher` and `LoggingProvider`
- [x] 4.2 Set up `WebApplicationFactory` for E2E testing
- [x] 4.3 Implement E2E tests for `POST /notify` (Success and invalid request cases)
- [x] 4.4 Verify that the service starts and accepts requests with the 80% coverage threshold

## 5. Documentation & Standards
- [x] 5.1 Update `openspec/project.md` with new monorepo standards (Zero warnings, 80% coverage, Singleton Options, Modular DI, Scalar UI)
