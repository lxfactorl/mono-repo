## 1. Scaffolding
- [ ] 1.1 Create solution structure in `src/backend/notification-service/`
- [ ] 1.2 Initialize projects: `.Api`, `.Application`, `.Domain`, `.Infrastructure`, `.Tests`
- [ ] 1.3 Add core dependencies (Serilog, FluentValidation)
- [ ] 1.4 Create root-level `.editorconfig` for formatting
- [ ] 1.5 Create root-level `Directory.Build.props` to enforce `TreatWarningsAsErrors`, Analyzers, and `InternalsVisibleTo`
- [ ] 1.6 Configure global test coverage thresholds (80%) in root-level build/CI configuration
- [ ] 1.7 Add testing dependencies to `.Tests` project: `NSubstitute`, `FluentAssertions`, `Microsoft.AspNetCore.Mvc.Testing`
- [ ] 1.8 Establish `Bootstrap/` folder structure in `.Api` project for DI extensions
- [ ] 1.9 Initialize `Monorepo.Shared.Infrastructure` project in `src/backend/shared/`

## 2. Abstraction Layer
- [ ] 2.1 Define `INotificationProvider` interface in Domain
- [ ] 2.2 Create `NotificationRequest` model in Domain
- [ ] 2.3 Implement `LoggingProvider` in Infrastructure

## 3. Core Logic & API
- [ ] 3.1 Implement `NotificationDispatcher` in Application
- [ ] 3.2 Create `POST /notify` Minimal API endpoint
- [ ] 3.3 Set up DI to register multiple providers and the dispatcher
- [ ] 3.4 Configure Serilog to use `IConfiguration` in `Program.cs`
- [ ] 3.5 Implement `AddValidatedOptions` and common bootstrap logic in `Shared.Infrastructure`
- [ ] 3.6 Integrate `Microsoft.AspNetCore.OpenApi` and `Scalar.AspNetCore`
- [ ] 3.7 Enrich OpenAPI metadata: add descriptions, status codes (202, 400), and XML comments for In/Out models

## 4. Validation & Testing
- [ ] 4.1 Unit tests for `NotificationDispatcher` and `LoggingProvider`
- [ ] 4.2 Set up `WebApplicationFactory` for E2E testing
- [ ] 4.3 Implement E2E tests for `POST /notify` (Success and invalid request cases)
- [ ] 4.4 Verify that the service starts and accepts requests with the 80% coverage threshold

## 5. Documentation & Standards
- [ ] 5.1 Update `openspec/project.md` with new monorepo standards (Zero warnings, 80% coverage, Singleton Options, Modular DI, Scalar UI)
