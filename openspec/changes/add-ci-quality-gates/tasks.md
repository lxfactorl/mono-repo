- [x] 1.1 Create `.github/workflows/templates` directory for shared logic
- [x] 1.2 Define `template-dotnet-ci.yml` (Reusable Workflow) encapsulating build/test/quality logic
- [x] 1.3 Create `backend-notification-service.yml` implementing the trigger for the existing service

## 2. Quality Gates Implementation
- [x] 2.1 Implement **Build & Warning Gate**: Run `dotnet build` with `/p:TreatWarningsAsErrors=true`
- [x] 2.2 Implement **Formatting Gate**: Run `dotnet format --verify-no-changes`
- [x] 2.3 Implement **Security Gate**: Run `dotnet list package --vulnerable --include-transitive`
- [x] 2.4 Implement **Test Execution**: Run `dotnet test` with `XPlat Code Coverage` collector
- [x] 2.5 Implement **Coverage Gate**: Add a step to verify coverage >= 80% (using `ReportGenerator` or comparable tool to parse results)

## 3. Documentation & Enforcement
- [x] 3.1 Update `openspec/project.md` to document the CI pipeline and required status checks
- [x] 3.2 Verify the pipeline runs on a dummy PR (or locally via act/manual trigger)
- [x] 3.3 Implement **Monorepo Gate** workflow (`ci-gate.yml`) to orchestrate service checks and provide a single status check for branch protection.
