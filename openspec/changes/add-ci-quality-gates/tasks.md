- [x] 1.1 Create `.github/workflows/templates` directory for shared logic
- [x] 1.2 Define `template-dotnet-ci.yml` (Reusable Workflow) encapsulating build/test/quality logic
- [x] 1.3 Create `notification-service-ci.yml` (Caller Workflow) for the Notification Service

## 2. Quality Gates Implementation
- [x] 2.1 Implement **Build & Warning Gate**: Run `dotnet build` with `/p:TreatWarningsAsErrors=true`
- [x] 2.2 Implement **Formatting Gate**: Run `dotnet format --verify-no-changes`
- [x] 2.3 Implement **Security Gate**: Run `dotnet list package --vulnerable --include-transitive`
- [x] 2.4 Implement **Test Execution**: Run `dotnet test` with `XPlat Code Coverage` collector
- [x] 2.5 Implement **Coverage Gate**: Verify coverage >= 80% and fail if threshold not met

## 3. QA & Security Enforcement
- [ ] 3.1 Update `openspec/project.md` to document the CI pipeline and Repository Ruleset enforcement
- [ ] 3.2 **QA Suite**: Verify each gate successfully blocks PR merges:
    - [ ] 3.2.1 Verify **Build failure** blocks PR (e.g., syntax error)
    - [ ] 3.2.2 Verify **Warning failure** blocks PR (TreatWarningsAsErrors)
    - [ ] 3.2.3 Verify **Formatting failure** blocks PR
    - [ ] 3.2.4 Verify **Test failure** blocks PR
    - [ ] 3.2.5 Verify **Coverage < 80%** blocks PR
- [ ] 3.3 Verify **Successful CI run** enables the Merge button
