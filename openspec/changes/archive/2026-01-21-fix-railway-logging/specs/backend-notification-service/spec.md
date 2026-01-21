## MODIFIED Requirements

### Requirement: Structured Logging Configuration
The system MUST configure Serilog to Read from `IConfiguration` and ensure all required sinks and formatters are explicitly registered to support observability in production environments (e.g., Railway).

#### Scenario: Log level control via settings
- **GIVEN** a valid `appsettings.json` with Serilog configuration
- **WHEN** the application starts
- **THEN** the logger SHALL respect the configured minimum levels and sinks from the settings file

#### Scenario: Production observability via console sink
- **GIVEN** the application is deployed to an environment that captures stdout (e.g., Railway)
- **WHEN** the `Serilog:Using` array includes `Serilog.Sinks.Console` and `Serilog.Expressions`
- **AND** a valid `ExpressionTemplate` is configured for the Console sink
- **THEN** application logs SHALL be correctly emitted to the console and visible in the platform's log dashboard
