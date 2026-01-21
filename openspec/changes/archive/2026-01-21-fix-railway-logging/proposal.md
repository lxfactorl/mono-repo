# Change: Fix Railway Logging Observability

## Why
Currently, application logs are not appearing or are unreadable in the Railway dashboard. 
1. Missing logs: Due to missing `Serilog.Expressions` and `Serilog.Sinks.Console` in the `Using` configuration.
2. Unreadable logs: The `ExpressionTemplate` syntax was being logged literally or mangled (e.g., ` @A, @l: @l, @21: @21, ..@p`).

## What Changes
- Updated `appsettings.json` to include mandatory Serilog assemblies in the `Using` property.
- Fixed the `ExpressionTemplate` syntax to use a robust format: `"{@t:yyyy-MM-dd HH:mm:ss.fff zzz} [{@l:u3}] {@m}\n{@p}\n{@x}"`.

## Impact
- **Affected specs**: `backend-notification-service`
- **Affected code**: `src/backend/notification-service/NotificationService/appsettings.json`

## Linked Issue
Relates to #88, #90
