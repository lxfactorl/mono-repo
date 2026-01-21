# Change: Fix Railway Logging Observability

## Why
Currently, application logs are not appearing in the Railway dashboard or CLI. This is due to an incomplete Serilog configuration in `appsettings.json` that lacks the necessary context for Serilog to correctly initialize the console sink and format logs for Railway's log collector. Specifically, the `Using` array is missing "Serilog.Expressions" and "Serilog.Sinks.Console", and the `ExpressionTemplate` syntax is incorrect.

## What Changes
- Updated `appsettings.json` to include mandatory Serilog assemblies in the `Using` property.
- Fixed the `ExpressionTemplate` syntax to ensure structured logs are correctly emitted to stdout.

## Impact
- **Affected specs**: `backend-notification-service`
- **Affected code**: `src/backend/notification-service/NotificationService/appsettings.json`

## Linked Issue
Relates to #88
