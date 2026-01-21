# Tasks: Fix Railway Logging

## 1. Implementation
- [x] 1.1 Update `appsettings.json` with Serilog `Using` configuration.
- [x] 1.2 Add `Serilog.Formatting.Compact` package.
- [x] 1.3 Update `appsettings.json` to use `RenderedCompactJsonFormatter`.
- [x] 1.4 Verify locally that logs are valid Compact JSON (CLEF).

## 2. Validation
- [ ] 2.1 Deploy to Railway.
- [ ] 2.2 Confirm that logs are parsed correctly and attributes are indexed.
