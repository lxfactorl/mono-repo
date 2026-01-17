## ADDED Requirements

### Requirement: Automated Pull Request Validation
The system MUST automatically validate every Pull Request targeting the `master` branch using a Continuous Integration (CI) pipeline.

#### Scenario: Developer opens a PR
- **WHEN** a developer creates or updates a Pull Request
- **THEN** the CI pipeline is triggered automatically
- **AND** the PR status remains "Pending" until the pipeline completes

---

### Requirement: Strict Build Quality
The CI pipeline MUST enforce a strict "Zero Warning" policy during the build process.

#### Scenario: PR contains compiler warnings
- **WHEN** the code contains compiler or analyzer warnings
- **THEN** the build step fails
- **AND** the PR is blocked from merging

---

### Requirement: Code Style Enforcement
The CI pipeline MUST verify that all code adheres to the project's formatting rules defined in `.editorconfig`.

#### Scenario: PR contains unformatted code
- **WHEN** a file has indentation or naming violations
- **THEN** the formatting check fails
- **AND** the CI pipeline reports an error

---

### Requirement: Automated Testing and Coverage
The CI pipeline MUST execute all unit and E2E tests and enforce a minimum code coverage threshold.

#### Scenario: Tests fail
- **WHEN** one or more tests fail execution
- **THEN** the pipeline fails immediately

#### Scenario: Coverage below threshold
- **WHEN** the aggregate line coverage is below 80%
- **THEN** the pipeline fails
- **AND** a report is generated indicating the missing coverage

---

### Requirement: Dependency Security Scanning
The CI pipeline MUST scan all project dependencies for known security vulnerabilities.

#### Scenario: Vulnerable package detected
- **WHEN** a usage of a NuGet package with a known CVE is detected
- **THEN** the pipeline fails
- **AND** the vulnerability details are logged
