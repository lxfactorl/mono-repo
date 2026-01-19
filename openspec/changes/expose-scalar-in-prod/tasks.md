# Tasks

## Analysis and Implementation

### Current State
**File:** `src/backend/notification-service/NotificationService/Bootstrap/ServiceBootstrap.cs`

**Lines 46-50:** Scalar is currently gated behind development environment check:
```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
```

### Required Changes

- [x] **Review current environment gates** - Scalar registration found at `ServiceBootstrap.cs:46-50`
- [x] **Remove environment gate** - Update `ServiceBootstrap.cs` to expose Scalar in all environments:
  - **Action:** Remove the `if (app.Environment.IsDevelopment())` condition wrapping lines 48-49
  - **Rationale:** Scalar only provides documentation UI; the OpenAPI schema itself is not sensitive
  - **Security Note:** No credentials or sensitive data are exposed through Scalar documentation
- [ ] **Verify Railway deployment** - After merge, confirm Scalar is accessible at `/scalar/v1` on production URL
- [ ] **Update launch settings** (optional) - `Properties/launchSettings.json` already points to `scalar/v1`
