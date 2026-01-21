# Tasks: Fix Railway Logging

## 1. Implementation
- [x] 1.1 Update `appsettings.json` with Serilog `Using` configuration.
- [x] 1.2 Update `appsettings.json` to output structured JSON using `ExpressionTemplate`.
- [x] 1.3 Verify locally that logs are valid JSON objects and contain all custom properties.

## 2. Validation
- [ ] 2.1 Deploy to Railway.
- [ ] 2.2 Confirm that logs appear in the Railway dashboard with indexed "Attributes" (verifying structured logging).
