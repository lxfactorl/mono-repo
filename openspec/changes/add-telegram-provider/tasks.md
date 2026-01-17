# Tasks

## Phase 1: Configuration & Secrets Infrastructure
- [ ] Add `appsettings.Development.json` to monorepo-wide `.gitignore` pattern
- [ ] Create `TelegramSettings` configuration class with `BotToken` and `ChatId` properties
- [ ] Register `TelegramSettings` with validation in DI (fail fast on missing token)
- [ ] Document secrets setup in README or dedicated SECRETS.md

## Phase 2: Provider Implementation
- [ ] Create `TelegramProvider` implementing `INotificationProvider`
- [ ] Use `HttpClient` to call Telegram Bot API (`sendMessage` endpoint)
- [ ] Format message body using MarkdownV2 parse mode
- [ ] Handle Telegram API errors: log and throw to trigger HTTP 500

## Phase 3: DI Registration
- [ ] Register `TelegramProvider` in the provider collection
- [ ] Ensure provider is conditionally enabled (only when `BotToken` is configured)

## Phase 4: Testing
- [ ] Add unit tests for `TelegramProvider` with mocked `HttpClient`
- [ ] Add integration test scaffold (manual run with real token)
- [ ] Verify 80% coverage threshold is maintained

## Phase 5: Verification
- [ ] Local end-to-end test: send notification via API, verify Telegram message received
- [ ] Document the Telegram Bot setup steps for future reference
