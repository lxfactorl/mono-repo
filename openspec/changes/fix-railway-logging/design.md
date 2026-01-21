# Design: Railway Logging Configuration Fix

## Context
Railway captures logs from stdout. If the output is a JSON object, Railway automatically parses it and populates the log entry's "Attributes".

## Goals
- Ensure all application logs are visible in the Railway dashboard.
- Ensure logs are fully structured (JSON) so that custom properties (e.g. `recipient`) are indexed by Railway.

## Decisions
### 1. Mandatory 'Using' Block
Keep `Serilog.Sinks.Console` and `Serilog.Expressions` in the `Using` array to support the expression-based formatter.

### 2. Structured JSON Output
Instead of a human-readable string (which flattens data), we will use an `ExpressionTemplate` that emits a JSON object.
**Decision**: Use the following template: `{@t, @l, @m, @x, ..@p}\n`
- `@t`: ISO 8601 Timestamp
- `@l`: Log Level
- `@m`: Rendered Message
- `@x`: Exception details
- `..@p`: All other properties (flattened into the root object)

## Risks / Trade-offs
- JSON logs are slightly less readable when viewing "Raw Data" manually, but provide vastly superior filtering and dashboarding capabilities in Railway.
