# Tasks: Enforce Telegram Configuration Validation

- [x] **Scaffold**: Initialize change and proposal (Done) <!-- id: 0 -->
- [x] **Define Validation Rules**: Add `[Required]` attributes to properties in `TelegramSettings.cs` <!-- id: 1 -->
- [x] **Implement Validation**: Update DI registration in `Program.cs` or extensions to use `ValidateDataAnnotations()` and `ValidateOnStart()` <!-- id: 2 -->
- [x] **Verify Fail-Fast**: Verify that the application throws a `OptionsValidationException` on startup if config is missing <!-- id: 3 -->
- [x] **Clean Up Registration**: Remove any logic that conditionally registers the provider based on config presence (it should always be registered) <!-- id: 4 -->
- [x] **Test**: Add or update unit/integration tests to confirm validation behavior <!-- id: 5 -->
