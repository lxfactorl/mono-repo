# Add Telegram Provider

## Status
DRAFT

## Why
The Notification Service currently only has a `LoggingProvider` that writes to stdout. To deliver real notifications, we need a concrete provider that can reach the user. Telegram is a lightweight, free, and well-documented messaging platform with a simple Bot API—making it an ideal first production provider.

## What Changes
- Add a `TelegramProvider` implementing `INotificationProvider`
- Configure Telegram Bot Token via environment variable (production) or `appsettings.Development.json` (local dev)
- Hardcode the owner's Chat ID in configuration for single-recipient delivery
- Format messages using Telegram's MarkdownV2 for rich display
- Return HTTP 500 and log errors when Telegram API fails

## Scope
- **In Scope**:
  - `TelegramProvider` implementation
  - Configuration model (`TelegramSettings`)
  - Secrets management pattern for local development (`appsettings.Development.json` excluded from git)
  - Unit tests with mocked HTTP client
  - E2E test with real Telegram API (manual verification)

- **Out of Scope**:
  - Multi-recipient support (future enhancement)
  - Message queuing/retry logic
  - Other providers (Email, SMS)

## Dependencies
- Telegram Bot Token (user must create bot via @BotFather)
- Chat ID (user must obtain via Telegram API or bot)

## Risks
- **Telegram API rate limits**: Mitigated by single-user scope
- **Token exposure**: Mitigated by `.gitignore` pattern for dev config
