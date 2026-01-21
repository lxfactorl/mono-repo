# Design: Railway Logging Configuration Fix

## Context
Railway captures logs from the standard output (stdout) and standard error (stderr) of the running container. For .NET applications using Serilog, this requires the `Serilog.Sinks.Console` to be correctly configured and the `Serilog.Expressions` formatter to be properly initialized.

## Goals
- Ensure all application logs (Information level and above) are visible in the Railway dashboard.
- Maintain structured log format using `ExpressionTemplate`.

## Decisions
### 1. Mandatory 'Using' Block
Serilog's configuration-based initialization requires the assembly names to be explicitly listed in the `Using` block if they are not automatically discovered.
**Decision**: Add `Serilog.Sinks.Console` and `Serilog.Expressions` to the `Using` array in `appsettings.json`.

### 2. Correct ExpressionTemplate Syntax
The previous template syntax contained nested braces that could cause parsing issues or incorrect output.
**Decision**: Simplify the template to `{@t: @t, @l: @l, @m: @m, ..@p}\n` which is the standard format for `Serilog.Expressions` to output a flat JSON-like structure that Railway can parse.

## Risks / Trade-offs
- **Risk**: Overly verbose logging could increase Railway usage costs (if logging is billed by volume).
- **Mitigation**: Keep the default level as `Information` and only log critical path events.
