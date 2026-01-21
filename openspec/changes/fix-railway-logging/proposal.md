# Change: Fix Railway Logging Observability (Phase 3: True Structured Logging)

## Why
Currently, application logs are visible in Railway (Phase 1/2), but they are logged as plain text strings. This prevents Railway from indexing the "Attributes" (structured data like `recipient`, `subject`, etc.), making advanced filtering and observability impossible. We need to output logs in a JSON format that Railway's log collector can natively parse.

## What Changes
- Updated `appsettings.json` to include mandatory Serilog assemblies in the `Using` property.
- Fixed the `ExpressionTemplate` syntax to output a clean JSON object for every log entry.
- Ensured structured data is preserved so it appears in Railway's "Attributes" tab.

## Impact
- **Affected specs**: `backend-notification-service`
- **Affected code**: `src/backend/notification-service/NotificationService/appsettings.json`

## Linked Issue
Relates to #88, #90
