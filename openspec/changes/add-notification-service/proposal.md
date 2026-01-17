# Change: Add Notification Service Skeleton

## Why
We need to establish the foundational infrastructure for a centralized Notification Service. Before implementing specific delivery channels (like Telegram), we must build a robust, extensible skeleton that defines how other services interact with notifications and how providers are plugged in.

## What Changes
- Create the core project structure for `NotificationService` in `src/backend/notification-service/`.
- Define the `INotificationProvider` abstraction in the Domain layer.
- Implement the REST API (`POST /notify`) with request validation.
- Implement a `NullProvider` or `LoggingProvider` to verify the pipeline without external dependencies.
- Set up the dependency injection container to handle multiple providers.
- **Standards Promotion**: Codify zero-warning, test coverage, and DI patterns into `openspec/project.md`.

## Impact
- Affected specs: none (new service)
- Affected documentation: `openspec/project.md` (Update project conventions)
- Affected code: `src/backend/notification-service/` (new), `src/backend/shared/` (new)
