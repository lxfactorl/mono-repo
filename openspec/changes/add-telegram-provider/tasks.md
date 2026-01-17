# Tasks

## Phase 1: Core Logic TDD (Isolated) - PR #1
*Note: Must be merged before starting Phase 2*
- [ ] Define internal `ITelegramMessenger` interface (Port) for sending messages
- [ ] Create `TelegramProvider` class depending on `ITelegramMessenger` and `TelegramSettings`
- [ ] Create `TelegramProviderTests`
- [ ] **RED**: Write failing test: Provider throws if Settings are invalid/missing
- [ ] **RED**: Write failing test: Provider formats message and calls `ITelegramMessenger.SendAsync`
- [ ] **RED**: Write failing test: Provider logs and bubbles up errors when Messenger fails
- [ ] **GREEN**: Implement `TelegramProvider` logic to pass tests (using mocks for Messenger)
- [ ] **REFACTOR**: Polish provider logic

## Phase 2: Integration & Adapter (The "Real World") - PR #2
*Note: Must be merged before starting Phase 3*
- [ ] Install `Telegram.Bot` NuGet package
- [ ] **Config TDD**: Write tests verifying `TelegramSettings` binds correctly from in-memory config
- [ ] Create `TelegramSettings` configuration class (`BotToken`, `ChatId`) and register in DI
- [ ] **Adapter TDD**: Write tests for `TelegramBotAdapter` mocking `ITelegramBotClient`
  - Verify `SendMessageAsync` is called with correct parameters
  - Verify `ApiRequestException` fro NuGet is caught and rethrown as domain exception
- [ ] Implement `TelegramBotAdapter` : `ITelegramMessenger` using real `TelegramBotClient`
- [ ] Register `TelegramBotAdapter` as `ITelegramMessenger` in DI

## Phase 3: Configuration & Credentials - PR #3
- [ ] **Manual**: Get Bot Token from [@BotFather](https://t.me/BotFather) and Chat ID (see reference below)
- [ ] Add `appsettings.Development.json` to `.gitignore`
- [ ] Configure `TelegramSettings` with real credentials in `appsettings.Development.json`
- [ ] Register `TelegramSettings` in DI

## Phase 4: Verification
- [ ] Local end-to-end test: send notification via API, verify Telegram message received
- [ ] **Post-Change**: Update monorepo TDD guidelines in `openspec/AGENTS.md`

### Reference: Bot Setup Steps
1. Chat with [@BotFather](https://t.me/BotFather) -> `/mybots` or `/newbot` -> Get Token
2. Message your bot -> Go to `https://api.telegram.org/bot<TOKEN>/getUpdates` -> Get Chat ID
