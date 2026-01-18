# backend-notification-service Specification Delta

## ADDED Requirements

### Requirement: Telegram Notification Provider
The system SHALL include a `TelegramProvider` that sends notifications via the Telegram Bot API.

#### Scenario: Successful message delivery
- **GIVEN** a valid Telegram Bot Token and Chat ID are configured
- **WHEN** a notification request is dispatched
- **THEN** the system SHALL send a message to the configured Telegram chat
- **AND** the message SHALL be formatted using MarkdownV2

#### Scenario: Telegram API failure
- **GIVEN** the Telegram API returns an error (e.g., invalid token, network failure)
- **WHEN** a notification request is dispatched
- **THEN** the system SHALL log the error details
- **AND** the API SHALL return `500 Internal Server Error`

### Requirement: Telegram Configuration
The system SHALL read Telegram settings from configuration with environment variable override support.

#### Scenario: Production configuration via environment variables
- **GIVEN** `Telegram__BotToken` and `Telegram__ChatId` environment variables are set
- **WHEN** the application starts
- **THEN** the system SHALL use these values to configure the provider

#### Scenario: Local development configuration
- **GIVEN** an `appsettings.Development.json` file with Telegram settings
- **WHEN** the application starts in Development environment
- **THEN** the system SHALL read the settings from the JSON file
- **AND** the file SHALL be excluded from version control

#### Scenario: Missing configuration fail-fast
- **GIVEN** the `BotToken` is missing or empty
- **WHEN** the application starts
- **THEN** the system SHALL throw a configuration validation error
- **AND** the application SHALL fail to start

### Requirement: Development Secrets Exclusion
The monorepo SHALL exclude development configuration files containing secrets from version control.

#### Scenario: Gitignore pattern for dev config
- **GIVEN** the `.gitignore` file at repository root
- **WHEN** a developer creates `appsettings.Development.json` in any service
- **THEN** the file SHALL NOT be tracked by Git
