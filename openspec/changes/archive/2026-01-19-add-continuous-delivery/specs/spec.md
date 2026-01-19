# Capability: CI/CD Pipeline

## ADDED Requirements

### Requirement: Automated Deployment to Railway
The system SHALL automatically deploy services to Railway after successful CI validation and merge to the `master` branch.

#### Scenario: Successful merge triggers deployment
- **WHEN** a pull request is merged to `master`
- **AND** all CI validation checks pass
- **THEN** the deployment workflow SHALL automatically trigger
- **AND** the service SHALL be deployed to Railway

#### Scenario: Deployment only for changed services
- **WHEN** code changes affect only one service (e.g., `notification-service`)
- **THEN** only that service SHALL be deployed
- **AND** other services SHALL remain unchanged

#### Scenario: Failed CI prevents deployment
- **WHEN** CI validation fails
- **THEN** deployment SHALL NOT occur
- **AND** the merge SHALL be blocked

### Requirement: Railway CLI Integration
The deployment workflow SHALL use Railway CLI to deploy services.

#### Scenario: Railway authentication
- **WHEN** the deployment job runs
- **THEN** it SHALL authenticate using Railway project token (`RAILWAY_TOKEN`) stored as GitHub repository secret
- **AND** the secret SHALL be referenced using `${{ secrets.RAILWAY_TOKEN }}` syntax
- **AND** the token SHALL never be hardcoded in workflow files
- **AND** GitHub SHALL automatically mask the token value in workflow logs
- **AND** it SHALL use `railway up --service=<service_id>` command to deploy
- **AND** it SHALL deploy to the correct Railway project

#### Scenario: Secrets management for CI/CD
- **WHEN** Railway token is configured for deployment
- **THEN** it SHALL be stored in GitHub repository settings (Settings → Secrets and variables → Actions)
- **AND** it SHALL be passed to reusable workflow via `workflow_call.secrets` mechanism
- **AND** secrets SHALL be accessible only to workflows running on the same repository (not from forks)
- **AND** CI/CD secrets SHALL be separate from application runtime secrets

### Requirement: Application Secrets Management
Application runtime secrets (e.g., Telegram bot tokens, database credentials) SHALL be managed separately from CI/CD secrets.

#### Scenario: Application secrets in Railway
- **WHEN** a service requires runtime secrets (e.g., `Telegram__BotToken`, `Telegram__ChatId`)
- **THEN** these secrets SHALL be stored in Railway Variables (Railway dashboard → Service → Variables)
- **AND** secrets SHALL be accessible to the application at runtime via environment variables
- **AND** secrets SHALL use double underscore format for nested configuration (`Telegram__BotToken` → `Telegram:BotToken` in .NET)
- **AND** secrets SHALL be service-scoped (each service has its own Railway Variables)

#### Scenario: Local development secrets
- **WHEN** developing locally
- **THEN** secrets SHALL be stored in `appsettings.Development.json` (excluded from git)
- **AND** the file SHALL be listed in `.gitignore` to prevent accidental commits
- **AND** developers SHALL use dummy or test values, never production secrets

#### Scenario: Secrets separation
- **WHEN** managing secrets for a service
- **THEN** application secrets (Telegram tokens, API keys) SHALL be stored in Railway Variables
- **AND** CI/CD secrets (Railway token for deployment) SHALL be stored in GitHub repository secrets
- **AND** these two types SHALL never be mixed or stored in the same location

#### Scenario: Application secrets format
- **WHEN** setting application secrets in Railway Variables
- **THEN** secrets SHALL use double underscore format for nested configuration (`Telegram__BotToken`)
- **AND** .NET configuration binding SHALL map them to nested settings (`Telegram:BotToken`)
- **AND** secrets SHALL be available at runtime via `IConfiguration` or validated options pattern

#### Scenario: Sealed variables for sensitive secrets
- **WHEN** storing ultra-sensitive secrets (e.g., API keys, tokens)
- **THEN** Railway Variables SHALL be marked as "sealed" when available
- **AND** sealed variables SHALL prevent accidental exposure in logs or PR environments
- **AND** sealed variables SHALL still be accessible to the application at runtime

#### Scenario: Service-specific Railway projects
- **WHEN** deploying a service
- **THEN** the workflow SHALL deploy to the Railway project configured for that service
- **AND** each service SHALL have its own Railway project configuration
- **AND** the `--service` flag SHALL be used to target the specific service

### Requirement: Railway Configuration File
Each service SHALL have a `railway.json` configuration file following Railway best practices.

#### Scenario: Railway configuration file structure
- **WHEN** a service is deployed
- **THEN** it SHALL have a `railway.json` file in the service directory
- **AND** the file SHALL include JSON schema reference: `"$schema": "https://railway.com/railway.schema.json"`
- **AND** the file SHALL specify `startCommand` that uses `$PORT` environment variable
- **AND** for .NET services, startCommand SHALL be: `dotnet <app>.dll --urls http://*:$PORT`

#### Scenario: Minimal Railway configuration
- **WHEN** a service uses Railway's Railpack builder
- **THEN** the `railway.json` SHALL contain minimal configuration (startCommand, optional healthcheckPath)
- **AND** build commands SHALL be auto-detected by Railpack
- **AND** no Dockerfile SHALL be required unless custom build steps are needed

### Requirement: PORT Environment Variable Usage
Services SHALL listen on Railway's `PORT` environment variable and bind to `0.0.0.0`.

#### Scenario: Application binds to PORT
- **WHEN** a service is deployed to Railway
- **THEN** the application SHALL read the `PORT` environment variable
- **AND** the application SHALL bind to `0.0.0.0` (not `localhost` or `127.0.0.1`)
- **AND** the startCommand in `railway.json` SHALL include `$PORT` variable substitution

### Requirement: Internal Service Networking
Internal services (e.g., notification-service) SHALL NOT be exposed to the public internet and SHALL only be accessible via Railway's private networking.

#### Scenario: Internal service has no public domain
- **WHEN** an internal service is deployed to Railway
- **THEN** no public domain SHALL be assigned to the service
- **AND** the service SHALL only be accessible via Railway's private networking
- **AND** the internal hostname SHALL follow pattern: `<service-name>.railway.internal`

#### Scenario: Internal service accessible from other services
- **WHEN** another service in the same Railway project needs to call notification-service
- **THEN** it SHALL use the internal URL: `http://notification-service.railway.internal`
- **AND** the call SHALL work without public internet exposure

#### Scenario: Deployment verification for internal service
- **WHEN** an internal service is deployed
- **THEN** verification SHALL be performed via Railway dashboard (deployment status, health check, logs)
- **AND** direct HTTP testing from public internet SHALL NOT be possible
- **AND** Railway health checks SHALL confirm service is healthy

### Requirement: Deployment Job Dependency
The deployment job SHALL depend on successful completion of all CI validation steps.

#### Scenario: Deployment waits for CI
- **WHEN** CI validation is running
- **THEN** the deployment job SHALL wait for completion
- **AND** deployment SHALL only proceed if validation passes

#### Scenario: CI failure blocks deployment
- **WHEN** any CI validation step fails
- **THEN** the deployment job SHALL be skipped
- **AND** no deployment SHALL occur

### Requirement: Per-Service Deployment Configuration
Each service SHALL have its own Railway configuration file (`railway.json`) defining deployment settings.

#### Scenario: Service Railway configuration
- **WHEN** a service is deployed
- **THEN** Railway SHALL use the `railway.json` file from that service's directory (`src/backend/<service-name>/railway.json`)
- **AND** the configuration SHALL specify `startCommand` with `$PORT` variable
- **AND** the configuration MAY specify `healthcheckPath` if health endpoint exists
- **AND** build commands SHALL be auto-detected by Railpack unless explicitly overridden

### Requirement: Path-Based Deployment Filtering
The deployment workflow SHALL use path-based filtering to deploy only services whose code has changed.

#### Scenario: Path filtering in caller workflows
- **WHEN** a service-specific workflow (e.g., `notification-service-ci.yml`) is configured
- **THEN** path filtering SHALL be defined in the caller workflow using `on.push.paths` and `on.pull_request.paths`
- **AND** the reusable workflow (`template-dotnet-ci.yml`) SHALL NOT contain path filters (reusable workflows don't support path filters)
- **AND** path filtering SHALL trigger only when changes affect `src/backend/<service-name>/**`

#### Scenario: Single service PR triggers only that service deployment
- **WHEN** a pull request is merged to `master`
- **AND** the PR contains changes only to `src/backend/notification-service/**`
- **THEN** only the notification-service deployment workflow SHALL trigger
- **AND** only notification-service SHALL be versioned, changelogged, and deployed
- **AND** all other services SHALL remain unchanged and not deployed

#### Scenario: Path filtering for deployment
- **WHEN** changes are made only to `src/backend/notification-service/**`
- **THEN** only the notification-service deployment SHALL trigger
- **AND** other services SHALL not be deployed

#### Scenario: Multiple services in PR trigger multiple deployments
- **WHEN** a pull request contains changes to both `src/backend/service-a/**` and `src/backend/service-b/**`
- **THEN** both service-a and service-b deployment workflows SHALL trigger independently
- **AND** each service SHALL be versioned, changelogged, and deployed separately
- **AND** services not affected by the PR SHALL not be deployed

#### Scenario: Workflow file changes don't trigger deployment
- **WHEN** changes are made only to `.github/workflows/**` files
- **AND** no service code paths are changed
- **THEN** no service deployment SHALL trigger
- **AND** only CI validation SHALL run

### Requirement: Easy Service Onboarding
Adding a new service to the CD pipeline SHALL require minimal configuration changes and follow a consistent pattern.

#### Scenario: Adding a new service to CD
- **WHEN** a new service is added to `src/backend/<service-name>/`
- **THEN** adding CD support SHALL require:
  - Creating a single workflow file `.github/workflows/<service-name>-ci.yml` following the established pattern
  - Configuring path filters in caller workflow: `on.push.paths` and `on.pull_request.paths` with service-specific paths
  - Adding Railway configuration file (`railway.json`) in the service directory
  - Configuring Railway project token (`RAILWAY_TOKEN`) and service ID as GitHub repository secrets
- **AND** no changes to the reusable template workflow SHALL be required
- **AND** the new service SHALL automatically benefit from path-based filtering and CI validation
- **AND** path filtering SHALL be configured in the caller workflow, not in the reusable workflow

#### Scenario: Consistent deployment pattern across services
- **WHEN** multiple services exist in the monorepo
- **THEN** all services SHALL follow the same deployment workflow pattern
- **AND** each service SHALL have identical workflow structure (only service name and paths differ)
- **AND** the reusable template SHALL handle all deployment logic without service-specific modifications

#### Scenario: Template workflow is service-agnostic
- **WHEN** the reusable template workflow (`template-dotnet-ci.yml`) is used
- **THEN** it SHALL accept service name, project path, and Railway project ID as inputs
- **AND** it SHALL work for any service without modification
- **AND** adding a new service SHALL NOT require changes to the template workflow file

### Requirement: Automatic Semantic Versioning
The system SHALL automatically increment version numbers based on semantic commit message types before deployment.

#### Scenario: Feature commit increments minor version
- **WHEN** commits since last release contain `feat:` type commits
- **AND** no `BREAKING CHANGE:` footer is present
- **THEN** `Versionize` SHALL increment the version as a minor version bump (e.g., 1.2.3 → 1.3.0)
- **AND** the version SHALL be updated directly in the service's `.csproj` file

#### Scenario: Fix commit increments patch version
- **WHEN** commits since last release contain only `fix:` type commits
- **AND** no `feat:` or `BREAKING CHANGE:` commits are present
- **THEN** the version SHALL be incremented as a patch version bump (e.g., 1.2.3 → 1.2.4)

#### Scenario: Breaking change increments major version
- **WHEN** commits since last release contain `BREAKING CHANGE:` footer or `!` in commit type
- **THEN** the version SHALL be incremented as a major version bump (e.g., 1.2.3 → 2.0.0)

#### Scenario: Version update before deployment
- **WHEN** `Versionize` increments the version
- **THEN** it SHALL commit the changes (`.csproj` and `CHANGELOG.md`) and create a git tag
- **AND** the workflow SHALL push these changes back to the repository
- **AND** the deployment SHALL use the newly bumped version

### Requirement: Automatic Changelog Generation
The system SHALL automatically generate CHANGELOG.md entries from semantic commit messages since the last release.

#### Scenario: Changelog generation from commits
- **WHEN** deployment workflow runs
- **THEN** `Versionize` SHALL analyze commits since the last git tag for that specific service path
- **AND** it SHALL group commits by type (`feat:`, `fix:`, `chore:`, etc.)
- **AND** it SHALL generate `CHANGELOG.md` entries according to the Conventional Commits spec

#### Scenario: Changelog format
- **WHEN** `CHANGELOG.md` is generated by `Versionize`
- **THEN** it SHALL include:
  - Version number and release date
  - Sections for "Added" (feat), "Fixed" (fix), "Changed" (BREAKING)
  - Commit messages with short-hash links
- **AND** entries SHALL be prepended to the existing `CHANGELOG.md` (newest first)

#### Scenario: Changelog per service
- **WHEN** a service is deployed
- **THEN** CHANGELOG.md SHALL be generated in that service's directory
- **AND** only commits affecting that service's code paths SHALL be included
- **AND** each service SHALL maintain its own CHANGELOG.md file

#### Scenario: Changelog commit
- **WHEN** `Versionize` updates the files
- **THEN** it SHALL create a single commit with message format: `chore(<service-name>): release v<version> [skip ci]`
- **AND** it SHALL create a git tag following the format specified in Requirement: Git Tag Creation

### Requirement: Git Tag Creation
The system SHALL create git tags for each deployed version to enable release tracking and rollback capabilities.

#### Scenario: Tag creation after successful deployment
- **WHEN** a service is successfully deployed to Railway
- **THEN** a git tag SHALL be created with the format `<service-name>/v<version>` (e.g., `notification-service/v1.3.0`)
- **AND** the tag SHALL reference the commit that includes the version bump and changelog
- **AND** the tag SHALL be pushed to the repository using `GITHUB_TOKEN` authentication
- **AND** the workflow SHALL have `contents: write` permission to create tags

#### Scenario: Tag format per service
- **WHEN** multiple services exist in the monorepo
- **THEN** tags SHALL be prefixed with service name (e.g., `notification-service/v1.3.0`)
- **AND** tags SHALL be filterable per service: `git tag -l 'notification-service/*'`

#### Scenario: Tag creation only on successful deployment
- **WHEN** deployment fails
- **THEN** no git tag SHALL be created
- **AND** version bump and changelog commits SHALL still be created (for tracking purposes)

### Requirement: GitHub Actions Workflow Permissions
The deployment workflow SHALL use minimal required permissions for security.

#### Scenario: Workflow permissions configuration
- **WHEN** a workflow needs to commit version/changelog changes and create git tags
- **THEN** the workflow SHALL declare `permissions: contents: write`
- **AND** all other permissions SHALL default to `none` (principle of least privilege)
- **AND** `GITHUB_TOKEN` SHALL be used for git operations (no additional secrets needed)

### Requirement: Health Check Endpoint
Each service deployed to Railway SHALL expose a health check endpoint for deployment verification.

#### Scenario: Health endpoint returns healthy status
- **WHEN** Railway performs a health check on a deployed service
- **THEN** the service SHALL respond on `/health` endpoint with HTTP 200 OK
- **AND** the response SHALL indicate the service is healthy

#### Scenario: Health endpoint configured in Railway
- **WHEN** a service has a `railway.json` configuration
- **THEN** the `healthcheckPath` property SHALL be set to `/health`
- **AND** Railway SHALL wait for health check to pass before routing traffic

#### Scenario: Health check timeout
- **WHEN** a service fails to respond to health check within configured timeout
- **THEN** the deployment SHALL be marked as failed
- **AND** Railway SHALL NOT route traffic to the failed deployment

### Requirement: Deployment Concurrency Control
The deployment workflow SHALL prevent concurrent deployments of the same service to avoid race conditions.

#### Scenario: Concurrent deployment prevention
- **WHEN** two deployments for the same service are triggered simultaneously
- **THEN** the workflow SHALL use concurrency groups to serialize deployments
- **AND** the second deployment SHALL wait for the first to complete
- **AND** deployments SHALL NOT be cancelled (cancel-in-progress: false)

#### Scenario: Concurrency group naming
- **WHEN** configuring concurrency for a service deployment
- **THEN** the concurrency group SHALL be named `deploy-<service-name>`
- **AND** each service SHALL have its own independent concurrency group

### Requirement: Robust Deployment Verification
The deployment process SHALL explicitly wait for and verify successful deployment before considering the job complete using Railway's GraphQL API.

#### Scenario: Verify Deployment Status via Railway GraphQL API
- **WHEN** a deployment is triggered via `railway up --detach --json`
- **THEN** the workflow SHALL capture the deployment ID from the JSON output
- **AND** the workflow SHALL poll Railway's GraphQL API endpoint (`https://backboard.railway.com/graphql/v2`) for deployment status
- **AND** it SHALL query the deployment object using the captured deployment ID
- **AND** it SHALL wait until status is `SUCCESS`, `ACTIVE`, `FAILED`, `CRASHED`, or `REMOVED`
- **AND** if status is `SUCCESS` or `ACTIVE`, the CI job SHALL succeed (Railway only marks as SUCCESS after health checks pass)
- **AND** if status is `FAILED`, `CRASHED`, or `REMOVED`, the CI job SHALL fail
- **AND** the workflow SHALL fetch and display deployment logs on failure for debugging
- **AND** the git tag SHALL NOT be created for failed deployments

#### Scenario: Deployment Status Polling Configuration
- **WHEN** polling Railway's GraphQL API for deployment status
- **THEN** the workflow SHALL use the Railway project token for authentication via `Project-Access-Token: <token>` header
- **AND** it SHALL poll at 5-second intervals to balance responsiveness with API rate limits
- **AND** it SHALL timeout after 5 minutes (60 attempts) if deployment does not reach a terminal state
- **AND** on timeout, it SHALL fail the CI job and display recent deployment logs

#### Scenario: Fallback for Missing Deployment ID
- **WHEN** the deployment ID cannot be extracted from `railway up --json` output
- **THEN** the workflow SHALL log a warning about the missing deployment ID
- **AND** it SHALL fall back to querying the service's latest deployment via GraphQL API
- **AND** it SHALL continue with status verification using the latest deployment ID

### Requirement: Automated Commit Loop Prevention
The deployment workflow SHALL prevent infinite loops caused by automated version commits.

#### Scenario: Skip CI on automated commits
- **WHEN** the workflow creates a version bump commit
- **THEN** the commit message SHALL include `[skip ci]` marker
- **AND** no new workflow run SHALL be triggered by this commit

#### Scenario: Actor check for workflow trigger
- **WHEN** a workflow is triggered
- **AND** the triggering actor is `github-actions[bot]`
- **THEN** the deployment job SHALL be skipped
- **AND** only CI validation SHALL run if applicable

### Requirement: First Release Handling
The deployment workflow SHALL handle the first release of a service when no previous git tags exist.

#### Scenario: First release with no previous tags
- **WHEN** deployment runs for a service with no existing git tags
- **THEN** the version SHALL default to the version in `.csproj` (e.g., `1.0.0`)
- **AND** the changelog SHALL include all commits since repository creation or initial commit
- **AND** the deployment SHALL proceed normally

#### Scenario: First release changelog
- **WHEN** generating changelog for first release
- **THEN** the system SHALL handle missing previous tag gracefully
- **AND** the changelog SHALL indicate this is the initial release

### Requirement: No Version-Bumping Commits Handling
The deployment workflow SHALL handle cases where commits don't require version bumps.

#### Scenario: Only chore/docs commits since last release
- **WHEN** all commits since last release are `chore:`, `docs:`, or `refactor:` type
- **AND** no `feat:` or `fix:` commits are present
- **THEN** no version bump SHALL occur
- **AND** no new git tag SHALL be created
- **AND** deployment SHALL still proceed with the current version

#### Scenario: Empty commit history for service
- **WHEN** no commits affecting the service exist since last tag
- **THEN** deployment SHALL be skipped for that service
- **AND** a warning message SHALL be logged

### Requirement: Deployment Failure Handling
The deployment workflow SHALL handle deployment failures gracefully and provide clear feedback.

#### Scenario: Railway deployment fails
- **WHEN** the Railway CLI `up` command fails
- **THEN** the workflow job SHALL fail with non-zero exit code
- **AND** no git tag SHALL be created for this version
- **AND** error details SHALL be visible in workflow logs

#### Scenario: Version commit already pushed but deployment fails
- **WHEN** version bump is committed and pushed
- **AND** subsequent Railway deployment fails
- **THEN** the git tag SHALL NOT be created
- **AND** the version commit SHALL remain in history (fix in next release)

### Requirement: Rollback Capability
The system SHALL support rollback to previous deployed versions using git tags.

#### Scenario: List available versions for rollback
- **WHEN** an operator needs to rollback a service
- **THEN** they SHALL be able to list all deployed versions using: `git tag -l '<service-name>/v*'`
- **AND** tags SHALL be sorted by version number

#### Scenario: Identify previous deployment commit
- **WHEN** rollback is needed
- **THEN** the operator SHALL find the previous version tag
- **AND** the tag SHALL point to the exact commit that was deployed

### Requirement: Version Property Existence
Services using automatic versioning SHALL have a `<Version>` property in their `.csproj` file.

#### Scenario: Version property required
- **WHEN** a service is configured for automatic versioning
- **THEN** the service's `.csproj` file SHALL contain a `<Version>` element
- **AND** the initial version SHALL be set (e.g., `1.0.0`)

#### Scenario: Missing version property
- **THEN** `Versionize` SHALL fail with a clear error condition
- **AND** the CI workflow SHALL catch the failure or handle insignificant commits correctly

### Requirement: Structured Logging for Observability
Services deployed to Railway SHALL use structured JSON logging to enable log filtering and searching in Railway's Log Explorer.

#### Scenario: Logs emitted in JSON format
- **WHEN** a service writes logs to stdout
- **THEN** logs SHALL be formatted as single-line JSON
- **AND** each log entry SHALL contain at minimum: `level` and `message` fields
- **AND** logs SHALL use standard levels: `debug`, `info`, `warn`, `error`

#### Scenario: Custom attributes in logs
- **WHEN** a service logs an event with context (e.g., provider name, recipient)
- **THEN** custom attributes SHALL be included in the JSON log
- **AND** attributes SHALL be searchable in Railway Log Explorer using `@attribute:value` syntax

#### Scenario: Log filtering in Railway
- **WHEN** an operator searches logs in Railway's Observability tab
- **THEN** they SHALL be able to filter by log level: `@level:error`
- **AND** they SHALL be able to filter by custom attributes: `@provider:telegram`
- **AND** they SHALL be able to combine filters: `@level:error AND "notification"`

### Requirement: Metrics Visibility
Services deployed to Railway SHALL have automatic metrics visible in the Railway dashboard.

#### Scenario: Built-in metrics available
- **WHEN** a service is deployed to Railway
- **THEN** the following metrics SHALL be automatically available:
  - CPU usage
  - Memory usage
  - Disk usage
  - Network I/O
- **AND** metrics SHALL be visible in Railway dashboard → Service → Metrics tab
- **AND** up to 30 days of historical data SHALL be available

#### Scenario: Deployment markers on metrics
- **WHEN** viewing metrics graphs in Railway dashboard
- **THEN** deployment events SHALL be marked with dotted lines
- **AND** operators SHALL be able to correlate resource changes with deployments

### Requirement: Railway CLI for Local Development
Developers SHALL be able to use Railway CLI for local development with production environment variables.

#### Scenario: Run locally with production secrets
- **WHEN** a developer runs `railway run <cmd>` in the service directory
- **THEN** the command SHALL execute with all Railway environment variables injected
- **AND** secrets like `Telegram__BotToken` SHALL be available to the application

#### Scenario: View environment variables
- **WHEN** a developer runs `railway variables`
- **THEN** all service environment variables SHALL be displayed
- **AND** secret values SHALL be visible (for development purposes)

#### Scenario: Link project and service
- **WHEN** a developer runs `railway link` from repository root
- **THEN** they SHALL be prompted to select team, project, and environment
- **AND** subsequent commands SHALL operate on the linked project

### Requirement: Railway CLI for Production Debugging
Developers SHALL be able to use Railway CLI for debugging deployed services.

#### Scenario: View deployment logs via CLI
- **WHEN** a developer runs `railway logs`
- **THEN** the latest deployment logs SHALL stream to terminal
- **AND** logs SHALL update in real-time

#### Scenario: SSH into running container
- **WHEN** a developer runs `railway ssh`
- **THEN** a shell session SHALL open inside the running container
- **AND** the developer SHALL have access to the application filesystem

#### Scenario: Execute single command in container
- **WHEN** a developer runs `railway ssh -- <command>`
- **THEN** the command SHALL execute in the container
- **AND** output SHALL be returned to the terminal
- **AND** the session SHALL close automatically
