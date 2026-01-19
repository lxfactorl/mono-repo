# Proposal: Enforce Telegram Configuration Validation

## Linked Issue
Relates to #78

## Summary
Currently, the `TelegramProvider` may be skipped or not registered if the configuration is missing or invalid. This can lead to "silent failures" where the service runs without its core notification capability. This proposal aims to enforce configuration validation at startup, ensuring the service fails fast if the Telegram settings are invalid.

## Problem Description
If `TelegramSettings` are missing or invalid:
- The provider might be skipped during registration.
- The service starts up successfully but cannot send notifications.
- This creates a risk in production where misconfiguration goes unnoticed until an incident occurs.

## Proposed Solution
1.  **Enforce Validation**: Leverage the .NET Options Pattern with `ValidateOnStart()` and Data Annotations.
2.  **Fail Fast**: Ensure the application host prevents startup if validation fails.
3.  **Mandatory Registration**: Always register the `TelegramProvider` and let dependencies (Options) enforce validity.

## Rationale
-   **Reliability**: Guarantees that if the service is running, it is fully configured to send notifications.
-   **Safety**: preventing under-configured deployments.

## Technical Details
-   Update `TelegramSettings` class with `[Required]` attributes.
-   Refactor `ServiceCollectionExtensions` to use `AddOptions<TelegramSettings>().ValidateDataAnnotations().ValidateOnStart()`.
-   Ensure `TelegramProvider` does not catch validation exceptions during construction/usage in a way that hides the error.
