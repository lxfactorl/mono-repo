# Proposal: Fix Scalar Mixed Content Error

## Linked Issue
Relates to #96

## Summary
The Scalar UI on production fails to execute requests because the OpenAPI spec uses `http` schemes (due to the app running behind Railway's load balancer without forwarded headers), causing the browser to block requests from the `https` Scalar page as "Mixed Content".

## Problem Statement
The application generates OpenAPI specifications based on the incoming request scheme. Since Railway terminates SSL and forwards traffic as HTTP, the app detects `http`. When this spec is loaded in a browser via `https`, the browser blocks API calls to the `http` endpoints.

## Proposed Solution
Enable and configure `ForwardedHeadersMiddleware` to trust headers from the proxy (Railway).
- Update `ServiceBootstrap` to register `ForwardedHeadersOptions`.
- Add `app.UseForwardedHeaders()` to the pipeline before OpenAPI generation.

## Implementation Details
1.  **Modify `ServiceBootstrap.cs`**:
    - Add `services.Configure<ForwardedHeadersOptions>(options => ...)`
    - Set `ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto`.
    - Clear `KnownNetworks` and `KnownProxies` (since Railway IPs are dynamic/internal).
    - Insert `app.UseForwardedHeaders()` at the start of `ConfigurePipeline`.

## Risks and Mitigation
- **Security**: Trusting forwarded headers can be risky if the app is exposed directly. However, on Railway, the app is behind a private network/load balancer, so it is generally safe to trust the upstream proxy if configured correctly. We will limit scope if possible, but usually clearing known proxies is required for PAAS like Railway/Heroku.
- **Regression**: Incorrect middleware order could break other middleware relying on scheme/IP. We will place it early.

## Verification Plan
- Deploy to Railway.
- Access Scalar UI via HTTPS.
- Verify `server` URL in Scalar allows execution (should be `https`).
- Execute `health` check from Scalar UI.
