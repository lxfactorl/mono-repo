# Tasks

## Phase 0: Telegram Bot Setup (Prerequisites)
- [ ] Open Telegram and start a chat with [@BotFather](https://t.me/BotFather)
- [ ] List existing bots: send `/mybots` to BotFather
- [ ] If you see your old bot → select it → click "API Token" to get the token
- [ ] If bot is missing or you want a new one → send `/newbot` and follow prompts
- [ ] Copy the Bot Token (format: `123456789:ABCdefGHIjklMNOpqrSTUvwxYZ`)
- [ ] Get your Chat ID:
  - Start a chat with your bot (search for it by username)
  - Send any message to the bot
  - Open `https://api.telegram.org/bot<YOUR_TOKEN>/getUpdates` in browser
  - Find `"chat":{"id":XXXXXXX}` in the response — that's your Chat ID
- [ ] Test the token works: send a test message via API:
  ```
  curl "https://api.telegram.org/bot<TOKEN>/sendMessage?chat_id=<CHAT_ID>&text=Hello"
  ```
- [ ] Store credentials securely (do NOT commit to git)

## Phase 1: Configuration & Secrets Infrastructure
- [ ] Add `appsettings.Development.json` to monorepo-wide `.gitignore` pattern
- [ ] Create `TelegramSettings` configuration class with `BotToken` and `ChatId` properties
- [ ] Register `TelegramSettings` with validation in DI (fail fast on missing token)
- [ ] Document secrets setup in README or dedicated SECRETS.md

## Phase 2: TDD - Provider Implementation
- [ ] Install `Telegram.Bot` NuGet package
- [ ] Create `TelegramProvider` class skeleton and `TelegramProviderTests` in test project
- [ ] **RED**: Write failing unit test verifying `SendMessageAsync` is called with correct ChatId and Markdown
- [ ] **RED**: Write failing unit test verifying `ApiRequestException` is caught and logged/rethrown appropriate to spec
- [ ] **GREEN**: Implement `TelegramProvider` logic to pass the tests
- [ ] **REFACTOR**: Review code for clarity and standard formatting helpers

## Phase 3: Wiring & Integration
- [ ] Register `ITelegramBotClient` in DI (using `HttpClient` factory)
- [ ] Register `TelegramProvider` in DI (conditionally enabled by config)
- [ ] Verify application startup with and without secrets

## Phase 4: Verification & Process
- [ ] Local end-to-end test: send notification via API, verify Telegram message received
- [ ] Document the Telegram Bot setup steps for future reference
- [ ] **Post-Change**: Update monorepo documentation (e.g., `openspec/AGENTS.md`) to adopt TDD as the standard workflow for logic-heavy features
