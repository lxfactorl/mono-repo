# Design: Railway Logging Configuration Fix (Phase 4)

## Context
Railway captures logs from stdout. To enable "True Structured Logging", the application must emit valid, standard JSON that the platform's log collector recognizes and parses.

## Goals
- Ensure all application logs are visible in the Railway dashboard.
- Ensure logs are fully structured (JSON) so that custom properties (e.g., `recipient`, `ProviderCount`) are indexed.
- Eliminate "double-encoded" JSON strings in the message field.

## Decisions
### 1. Use Standard Compact JSON Formatter
Switch from `ExpressionTemplate` to `Serilog.Formatting.Compact.RenderedCompactJsonFormatter`.
- **Why**: It is the industry standard for container logging (CLEF). It handles exceptions, rendering, and property serialization robustly without manual template errors.
- **Dependency**: Requires `Serilog.Formatting.Compact` package.

### 2. Mandatory 'Using' Block
Keep `Serilog.Sinks.Console` in `Using`. (`Serilog.Expressions` is no longer strictly needed for formatting but kept if other config relies on it).

## Risks / Trade-offs
- Log keys will be shortened (`@t` for timestamp, `@m` for message, etc.). This makes raw text slightly harder for humans to read but is optimized for machine parsing and storage. Railway's attribute viewer should map these or at least display them clearly.
