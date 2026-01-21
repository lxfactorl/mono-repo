# Design: Railway Logging Configuration Fix

## Context
Railway captures logs from stdout. For .NET applications using Serilog, this requires the `Serilog.Sinks.Console` to be correctly configured and the `Serilog.Expressions` formatter to be properly initialized with a valid template.

## Goals
- Ensure all application logs are visible in the Railway dashboard.
- Ensure logs are readable and correctly formatted.

## Decisions
### 1. Mandatory 'Using' Block
Add `Serilog.Sinks.Console` and `Serilog.Expressions` to the `Using` array in `appsettings.json`.

### 2. Robust ExpressionTemplate Syntax
The previously tried template syntax produced mangled output.
**Decision**: Use the standard bracketed syntax for the `ExpressionTemplate`:
`"{@t:yyyy-MM-dd HH:mm:ss.fff zzz} [{@l:u3}] {@m}\n{@p}\n{@x}"`

## Risks / Trade-offs
- Detailed timestamps and levels increase log volume slightly but are essential for troubleshooting.
