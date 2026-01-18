# Tasks

## Phase 1: Core Logic TDD (Isolated) - PR #1
*Note: Must be merged before starting Phase 2*
- [x] Define internal `ITelegramMessenger` interface (Port) for sending messages
- [x] Create `TelegramProvider` class depending on `ITelegramMessenger` and `TelegramSettings`
- [x] Create `TelegramProviderTests`
- [x] **RED**: Write failing test: Provider throws if Settings are invalid/missing
- [x] **RED**: Write failing test: Provider formats message and calls `ITelegramMessenger.SendAsync`
- [x] **RED**: Write failing test: Provider logs and bubbles up errors when Messenger fails
- [x] **GREEN**: Implement `TelegramProvider` logic to pass tests (using mocks for Messenger)
- [x] **REFACTOR**: Polish provider logic

## Phase 2: Integration & Adapter (The "Real World") - PR #2
*Note: Must be merged before starting Phase 3*
- [x] Install `Telegram.Bot` NuGet package
- [x] **Config TDD**: Write tests verifying `TelegramSettings` binds correctly from in-memory config
- [x] Create `TelegramSettings` configuration class (`BotToken`, `ChatId`) and register in DI
- [x] **Adapter TDD**: Write tests for `TelegramBotAdapter` mocking `ITelegramBotClient`
- [x]   Verify `SendMessageAsync` is called with correct parameters
- [x]   Verify `ApiRequestException` from NuGet is caught and rethrown as domain exception
- [x] Implement `TelegramBotAdapter` : `ITelegramMessenger` using real `TelegramBotClient`
- [x] Register `TelegramBotAdapter` as `ITelegramMessenger` in DI

## Phase 3: Configuration & Credentials - PR #3
- [x] **Manual**: Get Bot Token from [@BotFather](https://t.me/BotFather) and Chat ID (see reference below)
- [x] Add `appsettings.Development.json` to `.gitignore`
- [x] Configure `TelegramSettings` with real credentials in `appsettings.Development.json`
- [x] Register `TelegramSettings` in DI

## Phase 4: Verification
- [x] Local end-to-end test: send notification via API, verify Telegram message received
- [x] **Post-Change**: Update monorepo TDD guidelines in `openspec/AGENTS.md`

### Reference: Bot Setup Steps
1. Chat with [@BotFather](https://t.me/BotFather) -> `/mybots` or `/newbot` -> Get Token
2. Message your bot -> Go to `https://api.telegram.org/bot<TOKEN>/getUpdates` -> Get Chat ID
