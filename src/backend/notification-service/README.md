# Notification Service

This is a backend service for the Monorepo, built with .NET Core using the **Minimal API** pattern.

## Architecture & Conventions

### 1. Solution Management
This service folder is a self-contained unit:
- **`NotificationService.slnx`**: The primary entry point for working specifically on this service. It contains both the service and its test projects.
- **Project Structure**:
  - `NotificationService/`: The main executable project (Api, Domain, Infra).
  - `NotificationService.Tests/`: The test suite (Unit and E2E tiers).

### 2. CI/CD Integration
- **Workflow**: Monitored by `.github/workflows/notification-service-ci.yml`.
- **Quality Gates**:
  - **Zero Warnings**: Build fails on any compiler warning.
  - **80% Coverage**: Merges are blocked if line coverage drops below this threshold.
  - **Formatting**: `dotnet format` is enforced.
  - **Security**: Vulnerable package scanning is performed on every PR.

### 3. Key Design Patterns
- **Minimal APIs**: No Controllers; endpoints are registered via extension methods.
- **Internal by Default**: Logic is kept `internal` and only exposed to tests via `InternalsVisibleTo`.
- **Validated Options**: Configuration uses the "Validated Singleton Options" pattern to ensure settings are correct at startup.

### 4. Features
- **Notification Providers**: Pluggable delivery channels.
  - **Logging**: (Default) Writes notifications to console logs for development.
  - **Telegram**: Sends messages via Telegram Bot API (requires `Telegram:BotToken` and `Telegram:ChatId`).

## Message Formatting (Telegram)

The service supports **Telegram MarkdownV2** for rich notifications. The following styles are available to API callers:

| Style | Syntax | Result |
| :--- | :--- | :--- |
| **Bold** | `*text*` | **Bold text** |
| _Italic_ | `_text_` | _Italic text_ |
| Spoiler | `||text||` | Hidden text (tap to reveal) |
| `Code` | `` `text` `` | `fixed-width text` |
| [Link](https://google.com) | `[text](url)` | Clickable link |

**Note**: To ensure reliability, the service automatically escapes special characters (like `.`, `!`, `-`) that are not part of formatting tags. This means you can send regular text with punctuation without crashing the Telegram API.

### API Examples (curl)

**Basic Notification**
```bash
curl -X POST http://localhost:5272/notify \
     -H "Content-Type: application/json" \
     -d '{"Recipient": "any", "Message": "Hello from Mono-Repo!"}'
```

**Rich Formatting**
```bash
curl -X POST http://localhost:5272/notify \
     -H "Content-Type: application/json" \
     -d '{"Recipient": "any", "Message": "*Urgent Update*\n_The system is_ ||REBOOTING|| NOW!"}'
```

**Documentation Link**
```bash
curl -X POST http://localhost:5272/notify \
     -H "Content-Type: application/json" \
     -d '{"Recipient": "any", "Message": "View the [Project Specs](https://github.com/lxfactorl/mono-repo)"}'
```

## Development

To work on this service in isolation:
```bash
# Using the dedicated service solution
dotnet build NotificationService.slnx
dotnet test NotificationService.slnx
```

To run the specific CI validation locally:
```bash
# From the monorepo root
dotnet test src/backend/notification-service --collect:"XPlat Code Coverage"
```
