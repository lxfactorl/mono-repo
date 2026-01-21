# Change: Fix Railway Logging Observability (Phase 4: Standard Compact JSON)

## Why
Previous attempts at structured logging using manual `ExpressionTemplate` resulted in "double-JSON" issues in Railway and questionable reliability. To verify true structured logging and ensure industry-standard interoperability, we are switching to the official `Serilog.Formatting.Compact` formatter.

## What Changes
- Added `Serilog.Formatting.Compact` package to `NotificationService`.
- Updated `appsettings.json` to use `Serilog.Formatting.Compact.RenderedCompactJsonFormatter`.
- This ensures logs are emitted as robust, standard CLEF (Compact Log Event Format) JSON, which is widely supported and reliable in containerized environments.

## Impact
- **Affected specs**: `backend-notification-service`
- **Affected code**: `src/backend/notification-service/NotificationService/appsettings.json`
- **Dependencies**: Added `Serilog.Formatting.Compact`

## Linked Issue
Relates to #88, #90
