# Design: Telegram Notification Provider

## Context
The Notification Service has a `LoggingProvider` for development verification but lacks a production-ready delivery channel. This design extends the service with a `TelegramProvider` that sends real messages via the Telegram Bot API, making it the first external integration in the notification system.

## System Context

```
┌─────────────┐      HTTP POST       ┌─────────────────────┐
│   Client    │ ─────────────────▶  │ Notification Service │
└─────────────┘   /notifications    └──────────┬──────────┘
                                               │
                   ┌───────────────────────────┼───────────────────────────┐
                   │ INotificationProvider     │                           │
                   ▼                           ▼                           ▼
           ┌──────────────┐          ┌─────────────────┐          ┌──────────────┐
           │LoggingProvider│         │TelegramProvider │          │ (Future SMS) │
           └──────────────┘          └────────┬────────┘          └──────────────┘
                                              │
                                              │ HTTPS
                                              ▼
                                    ┌─────────────────┐
                                    │ Telegram Bot API│
                                    │api.telegram.org │
                                    └─────────────────┘
```

## Data Flow

1. **Inbound Request**: Client POSTs to `/notifications` with message payload
2. **Dispatcher**: `NotificationDispatcher` iterates registered `INotificationProvider` instances
3. **TelegramProvider**:
   - Reads `BotToken` and `ChatId` from `TelegramSettings`
   - Formats message body with MarkdownV2 escaping
   - POSTs to `https://api.telegram.org/bot{token}/sendMessage`
4. **Response Handling**:
   - Success (HTTP 200): Log and continue
   - Failure: Log error, throw exception → triggers HTTP 500 to client

## Configuration Schema

```json
{
  "Telegram": {
    "BotToken": "123456789:ABCdefGHIjklMNOpqrSTUvwxYZ",
    "ChatId": "987654321"
  }
}
```

| Key | Type | Required | Source |
|-----|------|----------|--------|
| `BotToken` | string | ✅ Yes | Env var `Telegram__BotToken` or `appsettings.*.json` |
| `ChatId` | string | ✅ Yes | Env var `Telegram__ChatId` or `appsettings.*.json` |

**Validation**: Application fails fast on startup if `BotToken` is missing or empty.

## Secrets Management

| Environment | Strategy |
|-------------|----------|
| **Local Dev** | `appsettings.Development.json` (git-ignored) |
| **Production** | Environment variables injected by deployment platform |

Pattern added to `.gitignore`:
```
**/appsettings.Development.json
```

## Error Handling Strategy

| Failure Mode | Behavior |
|--------------|----------|
| Network timeout | Log error, throw → HTTP 500 |
| Invalid token (HTTP 401) | Log error, throw → HTTP 500 |
| Rate limited (HTTP 429) | Log warning, throw → HTTP 500 (no retry in v1) |
| Invalid chat ID | Log error, throw → HTTP 500 |

**Trade-off**: No retry logic in v1. Retry/queue mechanism deferred to future enhancement.

## Key Decisions

1. **HttpClient via DI**: Use `IHttpClientFactory` for proper connection pooling and lifecycle management.

2. **MarkdownV2 Formatting**: Telegram's MarkdownV2 requires escaping special characters (`_`, `*`, `[`, `]`, etc.). Implement a helper method to sanitize message content.

3. **Single Recipient**: Hardcoded `ChatId` for single-owner delivery. Multi-recipient support is out of scope.

4. **Conditional Registration**: Provider is registered only when `BotToken` is configured, allowing graceful fallback to `LoggingProvider` only in unconfigured environments.

## Trade-offs

| Decision | Trade-off |
|----------|-----------|
| No retry logic | Simpler implementation, but transient failures are not recovered |
| Single recipient | No fan-out complexity, but limits to personal notifications |
| Fail-fast config | Clear deployment errors, but no partial startup possible |

## Open Questions

- None. Design is scoped and validated against existing patterns.
