# backend-notification-service Specification

## Purpose
TBD - created by archiving change add-notification-service. Update Purpose after archive.
## Requirements
### Requirement: Notification Dispatch API
The system SHALL expose a `POST /notify` endpoint to receive notification requests.

#### Scenario: Request validation
- **WHEN** an empty message is sent to the API
- **THEN** the system SHALL return a `400 Bad Request`
- **AND** the notification is not processed

### Requirement: Modern API Documentation
The system SHALL generate an OpenAPI 3.0 specification natively and expose a well-configured **Scalar** UI.

#### Scenario: Comprehensive API metadata
- **GIVEN** the Scalar documentation endpoint
- **WHEN** the documentation is viewed
- **THEN** it SHALL display detailed descriptions for the service and endpoints
- **AND** it SHALL clearly define the Request (In-model) and Response (Out-model) schemas
- **AND** it SHALL list all possible HTTP status codes (e.g., 202 Accepted, 400 Bad Request) with descriptions of when they occur

### Requirement: Provider Interface
The system SHALL define a common `INotificationProvider` interface for all delivery channels.

#### Scenario: Message routing to providers
- **GIVEN** at least one provider is registered (e.g., `LoggingProvider`)
- **WHEN** a valid notification request is received
- **THEN** the system SHALL call the `SendAsync` method on all registered providers

### Requirement: Internal Logging Provider
The system SHALL include a default `LoggingProvider` that logs notifications to the standard output.

#### Scenario: Verification of pipeline
- **WHEN** a notification is dispatched
- **THEN** the `LoggingProvider` SHALL write the message content to the system logs
- **AND** the API SHALL return `202 Accepted`

### Requirement: Structured Logging Configuration
The system MUST configure Serilog to Read from `IConfiguration`.

#### Scenario: Log level control via settings
- **GIVEN** a valid `appsettings.json` with Serilog configuration
- **WHEN** the application starts
- **THEN** the logger SHALL respect the configured minimum levels and sinks from the settings file

### Requirement: Startup Configuration Validation
The system MUST validate configuration objects using Data Annotations on application startup.

#### Scenario: Fail fast on invalid configuration
- **GIVEN** a configuration class with `[Required]` or `[Range]` attributes
- **WHEN** the application starts with invalid or missing settings
- **THEN** the system SHALL throw a `OptionsValidationException` during host build
- **AND** the application SHALL fail to start

### Requirement: Singleton Configuration Injection
Configuration objects MUST be available in the DI container as singletons of the POCO type itself, not wrapped in `IOptions<T>`.

#### Scenario: Direct injection
- **GIVEN** a service that needs `AppNotificationSettings`
- **WHEN** the service is resolved via DI
- **THEN** the container SHALL provide the unwrapped, validated instance of `AppNotificationSettings`

### Requirement: Zero Warning Policy
The build process MUST treat all compiler warnings as errors.

#### Scenario: Build failure on warnings
- **GIVEN** a syntax or code style violation that triggers a warning
- **WHEN** the project is compiled
- **THEN** the build SHALL fail with an error

### Requirement: Shared Analyzer Configuration
The solution MUST use a centralized `Directory.Build.props` at the monorepo root to enforce analysis rules.

#### Scenario: Global monorepo rule application
- **GIVEN** any service within the `src/` directory
- **WHEN** the project is built
- **THEN** it SHALL automatically inherit the `TreatWarningsAsErrors` and analyzer settings from the monorepo root

### Requirement: Global Test Coverage Threshold
All backend services MUST maintain a minimum of 80% line coverage for business logic and infrastructure.

#### Scenario: Coverage validation
- **GIVEN** a backend service in the monorepo
- **WHEN** tests are executed with coverage collection
- **THEN** the aggregated coverage SHALL be 80% or higher
- **AND** the build SHALL fail if the threshold is not met

### Requirement: Master Branch Coverage Gate
Merging into the `master` branch MUST be blocked if the 80% coverage threshold is violated.

#### Scenario: Block push on low coverage
- **GIVEN** a Pull Request with 75% test coverage
- **WHEN** trying to push or merge to `master`
- **THEN** the CI gate SHALL fail
- **AND** prevent the merge/push

### Requirement: White-Box Testing Support
The system MUST allow testing of internal members without making them public.

#### Scenario: Testing internal logic
- **GIVEN** a class with an `internal` method or property
- **WHEN** the test project attempts to access it
- **THEN** the member SHALL be accessible due to `InternalsVisibleTo` configuration
- **AND** the internal behavior can be verified in isolation

### Requirement: Black-Box E2E Validation
The system MUST be verifiable as a complete unit using in-memory hosting.

#### Scenario: Full request-to-log pipeline
- **GIVEN** a running instance of the service via `WebApplicationFactory`
- **WHEN** a `POST /notify` request is made with a valid message
- **THEN** the system SHALL return `202 Accepted`
- **AND** the mock of the `INotificationProvider` (or the actual `LoggingProvider`) SHALL receive the message

### Requirement: Modular Code Organization
The codebase MUST follow strict organization patterns to ensure maintainability.

#### Scenario: Single type per file enforcement
- **GIVEN** a source file in any project
- **WHEN** inspected for content
- **THEN** it SHALL contain exactly one class, interface, or record definition
- **AND** the file name SHALL match the type name

### Requirement: Chained Modular Dependency Injection
DI registrations MUST be modularized into domain-specific extension classes and chained in the entry point.

#### Scenario: Clean Program.cs entry point
- **GIVEN** the `Program.cs` file
- **WHEN** reviewing the service registration logic
- **THEN** it SHALL only contain chained calls to extension methods (e.g., `builder.Services.AddInfrastructure().AddApplication().AddDomain()`)
- **AND** all concrete registration logic SHALL reside in `Bootstrap/` directory extension classes

### Requirement: Centralized Shared Infrastructure
The monorepo MUST include a shared infrastructure project to promote code reuse and consistency across all backend services.

#### Scenario: Reusing common patterns
- **GIVEN** a recurrent pattern (e.g., validated options registration)
- **WHEN** implemented in the `src/backend/shared/` project
- **THEN** all backend services SHALL be able to reference and use the pattern without duplication

