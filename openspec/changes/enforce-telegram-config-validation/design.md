# Design: Enforce Telegram Configuration Validation

## Overview
We need to transition from a "permissive" configuration model (where missing config simply disables the feature) to a "strict" model (where missing config prevents startup).

## Components

### 1. TelegramSettings
-   Namespace: `NotificationService.Infrastructure.Telegram`
-   Changes: Add `System.ComponentModel.DataAnnotations` attributes.
    -   `BotToken`: `[Required]`
    -   `DefaultChatId`: `[Required]`

### 2. Dependency Injection
-   Location: `NotificationService.Web/Program.cs` or `ServiceCollectionExtensions.cs`
-   Current approach: Might be using `Configure<TelegramSettings>(...)`.
-   New approach:
    ```csharp
    services.AddOptions<TelegramSettings>()
        .BindConfiguration("Telegram")
        .ValidateDataAnnotations()
        .ValidateOnStart();
    ```

### 3. Error Handling
-   The default behavior of `ValidateOnStart` raises an `OptionsValidationException` before the host starts accepting requests. This is the desired behavior.
