# Issues Tracking Specification

## ADDED Requirements

### Requirement: Issue Templates
The repository SHALL provide structured issue templates for consistent issue creation.

#### Scenario: Bug report creation
- **WHEN** a contributor creates a new issue
- **THEN** the system presents a bug report template with fields for:
  - Summary (required)
  - Steps to reproduce (required)
  - Expected behavior (required)
  - Actual behavior (required)
  - Environment details (optional)
  - Affected service (required dropdown)

#### Scenario: Feature request creation
- **WHEN** a contributor creates a new issue for a feature
- **THEN** the system presents a feature request template with fields for:
  - Problem statement (required)
  - Proposed solution (required)
  - Alternatives considered (optional)
  - Affected service (required dropdown)

#### Scenario: Technical debt creation
- **WHEN** a contributor creates an issue for technical debt
- **THEN** the system presents a technical debt template with fields for:
  - Description of debt (required)
  - Impact if not addressed (required)
  - Proposed remediation (optional)
  - Affected service (required dropdown)

### Requirement: Issue Labels
The repository SHALL maintain a standardized set of labels for issue categorization.

#### Scenario: Priority labeling
- **WHEN** an issue is triaged
- **THEN** it SHALL be assigned one priority label from: `priority:critical`, `priority:high`, `priority:medium`, `priority:low`

#### Scenario: Type labeling
- **WHEN** an issue is created
- **THEN** it SHALL be assigned one type label from: `type:bug`, `type:feature`, `type:tech-debt`, `type:documentation`

#### Scenario: Service labeling
- **WHEN** an issue affects a specific service
- **THEN** it SHALL be assigned a service label matching the affected service (e.g., `service:notification`)

### Requirement: Issue-PR Linking
Pull requests and commits SHALL reference related issues using standard GitHub linking conventions, enabling bidirectional traceability.

#### Scenario: Closing issue via PR
- **WHEN** a PR is merged that resolves an issue
- **THEN** the PR description SHALL contain `Closes #N` where N is the issue number
- **AND** the issue SHALL be automatically closed upon PR merge

#### Scenario: Related issue reference
- **WHEN** a PR relates to but does not fully resolve an issue
- **THEN** the PR description SHALL contain `Relates to #N` where N is the issue number

#### Scenario: Commit references issue
- **WHEN** a commit is made for work related to an issue
- **THEN** the commit message SHOULD contain `#N` where N is the issue number
- **AND** the commit SHALL appear in the issue's timeline

#### Scenario: Bidirectional navigation
- **GIVEN** an issue is linked to PRs and commits
- **WHEN** viewing the issue on GitHub
- **THEN** all related PRs and commits SHALL be visible in the issue timeline
- **WHEN** viewing a PR or commit on GitHub
- **THEN** the linked issue SHALL be visible and navigable

### Requirement: Issue Lifecycle
Issues SHALL follow a defined lifecycle with clear state transitions.

#### Scenario: Issue state progression
- **WHEN** an issue is created
- **THEN** it starts in `Open` state
- **WHEN** work begins on an issue
- **THEN** it transitions to `In Progress` (via linked PR or manual label)
- **WHEN** a PR is opened for the issue
- **THEN** it transitions to `In Review`
- **WHEN** the PR is merged
- **THEN** it transitions to `Done` (closed)

### Requirement: OpenSpec Integration
Issues SHALL integrate with the OpenSpec workflow for spec-driven development.

#### Scenario: Discovered work during implementation
- **WHEN** working on an active spec change
- **AND** additional work is discovered (bug, improvement, tech debt)
- **THEN** the work item SHALL be captured as a GitHub Issue
- **AND** the issue MAY be linked to the current spec change

#### Scenario: Issue as input for spec proposal
- **WHEN** planning a new spec change
- **THEN** relevant open issues SHOULD be reviewed
- **AND** the proposal MAY reference issues it addresses

#### Scenario: Single active change constraint
- **GIVEN** only one spec change can be active at a time
- **WHEN** new work is discovered during implementation
- **THEN** the work SHALL be captured as an Issue for future consideration
- **AND** a new spec proposal SHALL NOT be created until the current change is archived

### Requirement: Agent Track Workflow
The agent SHALL provide a `/openspec-issue-track` workflow for quick issue capture.

#### Scenario: Track workflow invocation
- **WHEN** user invokes `/openspec-issue-track` with a problem description
- **THEN** the agent SHALL analyze the description
- **AND** determine issue type (bug, feature, tech debt)
- **AND** create a GitHub Issue via MCP GitHub tool
- **AND** return the issue URL to the user

#### Scenario: Issue type detection
- **WHEN** description mentions "bug", "broken", "error", "not working", "fails"
- **THEN** issue type SHALL be `type:bug`
- **WHEN** description mentions "add", "new", "feature", "want", "need"
- **THEN** issue type SHALL be `type:feature`
- **WHEN** description mentions "refactor", "cleanup", "tech debt", "improve"
- **THEN** issue type SHALL be `type:tech-debt`

#### Scenario: No OpenSpec docs on track
- **WHEN** `/openspec-issue-track` workflow is used
- **THEN** only a GitHub Issue SHALL be created
- **AND** no OpenSpec proposal, tasks, or spec files SHALL be created
- **AND** spec docs are created later when work on the issue begins

### Requirement: Agent Spec Create Workflow
The agent SHALL provide a `/openspec-spec-create` workflow to start spec work with mandatory issue linking.

#### Scenario: Spec create workflow invocation
- **WHEN** user invokes `/openspec-spec-create` with a description
- **THEN** the agent SHALL verify no active spec is in progress
- **AND** search GitHub Issues for a relevant existing issue
- **AND** if no issue found, create a new issue via MCP GitHub tool
- **AND** create spec docs (`proposal.md`, `tasks.md`, `specs/`) with issue reference
- **AND** update issue status to "In Progress"
- **AND** return spec path and linked issue URL

#### Scenario: Active spec blocking
- **GIVEN** a spec change already exists in `openspec/changes/` (not archived)
- **WHEN** user invokes `/openspec-spec-create`
- **THEN** the agent SHALL reject the request
- **AND** inform user of the active spec that must be completed first

#### Scenario: Issue search before creation
- **WHEN** `/openspec-spec-create` is invoked
- **THEN** the agent SHALL search open GitHub Issues for matching keywords via MCP GitHub tool

#### Scenario: Candidate issues found
- **GIVEN** the agent searched for existing issues
- **WHEN** one or more candidate issues are found
- **THEN** the agent SHALL present the candidates to the user
- **AND** ask: "Link to existing issue #N or create new?"
- **AND** wait for user response before proceeding

#### Scenario: User chooses existing issue
- **GIVEN** candidate issues were presented
- **WHEN** user chooses to link to an existing issue
- **THEN** the agent SHALL use that issue for the spec
- **AND** NOT create a duplicate issue

#### Scenario: User chooses new issue
- **GIVEN** candidate issues were presented
- **WHEN** user chooses to create a new issue
- **THEN** the agent SHALL create a new issue via MCP GitHub tool

#### Scenario: No candidates found
- **GIVEN** the agent searched for existing issues
- **WHEN** no matching issues are found
- **THEN** the agent SHALL create a new issue via MCP GitHub tool
- **AND** NOT prompt the user

### Requirement: Agent Spec From Issue Workflow
The agent SHALL provide a `/openspec-spec-from-issue` workflow to create spec from existing issue.

#### Scenario: Create spec from issue URL
- **WHEN** user invokes `/openspec-spec-from-issue <issue-url>`
- **THEN** the agent SHALL verify no active spec is in progress
- **AND** fetch issue data from GitHub via MCP GitHub tool
- **AND** create spec docs with pre-filled content from issue
- **AND** update issue status to "In Progress"
- **AND** return spec path and linked issue URL

#### Scenario: Closed issue handling
- **GIVEN** the referenced issue is closed
- **WHEN** user invokes `/openspec-spec-from-issue`
- **THEN** the agent SHALL ask: "Issue #N is closed. Reopen and continue?"
- **AND** if user confirms → reopen issue and proceed
- **AND** if user declines → abort workflow

#### Scenario: Already In Progress issue
- **GIVEN** the referenced issue is already marked In Progress
- **WHEN** user invokes `/openspec-spec-from-issue`
- **THEN** the agent SHALL ask: "Issue #N is already In Progress. Continue with this issue?"
- **AND** if user confirms → proceed without changing status
- **AND** if user declines → abort workflow

#### Scenario: Non verb-led issue title
- **GIVEN** the issue title does not start with a verb
- **WHEN** spec docs are being created
- **THEN** the agent SHALL ask user to provide a verb-led change-id
- **AND** use the provided change-id for spec folder name

#### Scenario: Pre-fill spec docs from issue
- **WHEN** spec docs are created from an existing issue
- **THEN** issue title SHALL be used to derive change-id (kebab-case, verb-led)
- **AND** issue body SHALL populate the "Why" section in `proposal.md`
- **AND** issue labels SHALL be reflected in spec metadata
- **AND** issue reference SHALL be included in `proposal.md`

#### Scenario: Issue URL formats
- **WHEN** user provides issue reference
- **THEN** the agent SHALL accept full URL format: `https://github.com/owner/repo/issues/123`
- **AND** the agent SHALL accept short format: `#123`

### Requirement: Agent Spec Cancel Workflow
The agent SHALL provide a `/openspec-spec-cancel` workflow to abandon specs.

#### Scenario: Cancel workflow invocation
- **WHEN** user invokes `/openspec-spec-cancel`
- **THEN** the agent SHALL verify an active spec exists
- **AND** ask user: "Cancel spec and revert issue to Open?" or "Close issue as won't fix?"

#### Scenario: Revert issue to Open
- **GIVEN** user chose to revert
- **WHEN** cancel proceeds
- **THEN** the agent SHALL remove "In Progress" label
- **AND** issue SHALL remain open for future work

#### Scenario: Close as won't fix
- **GIVEN** user chose to close
- **WHEN** cancel proceeds
- **THEN** the agent SHALL close issue with "won't fix" comment

#### Scenario: Cancel complex spec
- **GIVEN** spec has parent + child issues
- **WHEN** user invokes `/openspec-spec-cancel`
- **THEN** the agent SHALL ask: "Close all child issues or keep open?"
- **AND** handle child issues based on user response

#### Scenario: Spec docs cleanup
- **WHEN** cancel completes
- **THEN** the agent SHALL ask: "Delete spec docs or keep for reference?"
- **AND** perform cleanup based on user response

### Requirement: Complex Specs with Hierarchical Issues
Specs with multiple phases SHALL use parent-child issue structure when appropriate.

#### Scenario: Simple spec (1-2 phases)
- **WHEN** a spec has 1-2 phases in `tasks.md`
- **THEN** a single issue with task list checkboxes SHALL be used
- **AND** no child issues SHALL be created

#### Scenario: Complex spec (3+ phases)
- **WHEN** a spec has 3 or more phases in `tasks.md`
- **THEN** the agent SHALL create a parent issue for the overall spec
- **AND** create child issues for each phase
- **AND** child issues SHALL contain `Part of #parent` in body
- **AND** parent issue SHALL have task list with `- [ ] #child` links

#### Scenario: Phase PR closes child issue
- **WHEN** a PR is merged for a specific phase
- **THEN** the PR SHALL reference `Closes #child-issue`
- **AND** the child issue SHALL be automatically closed
- **AND** the parent issue task list checkbox SHALL be checked

#### Scenario: Archive closes parent issue
- **WHEN** spec is archived (all phases complete)
- **THEN** the parent issue SHALL be closed

### Requirement: Multi-Service Issues
Issues affecting multiple services SHALL use main issue with sub-issues structure.

#### Scenario: Multi-service issue creation
- **WHEN** an issue affects multiple services
- **THEN** a main issue SHALL be created for the primary service
- **AND** sub-issues SHALL be created for other affected services
- **AND** sub-issues SHALL contain `Part of #main` in body
- **AND** main issue SHALL reference sub-issues in task list

#### Scenario: Multi-service labeling
- **WHEN** multi-service issues are created
- **THEN** main issue SHALL have primary `service:X` label
- **AND** sub-issues SHALL have their respective `service:Y` labels

### Requirement: Orphan Child Issue Handling
The agent SHALL detect and handle orphan child issues.

#### Scenario: Orphan detection
- **WHEN** child issues reference a parent that doesn't exist or is deleted
- **THEN** the agent SHALL detect the orphan condition
- **AND** ask user: "Found orphan child issues #X, #Y. Delete, close, or keep?"

#### Scenario: Orphan cleanup
- **GIVEN** user chose to delete or close orphans
- **WHEN** cleanup proceeds
- **THEN** the agent SHALL perform the chosen action via MCP GitHub tool

### Requirement: Issue Reference Format
Spec proposals SHALL include a dedicated section for issue references.

#### Scenario: Issue reference in proposal
- **WHEN** spec docs are created
- **THEN** `proposal.md` SHALL include a `## Linked Issue` section after "Why"
- **AND** the section SHALL contain `Relates to #N` or `Closes #N`

#### Scenario: Complex spec issue reference
- **WHEN** spec has parent + child issues
- **THEN** `## Linked Issue` section SHALL include parent and child references

### Requirement: MCP Error Handling
The agent SHALL handle MCP GitHub tool failures gracefully.

#### Scenario: MCP operation failure
- **WHEN** MCP GitHub tool call fails (auth error, rate limit, network)
- **THEN** the agent SHALL show ERROR with details
- **AND** stop the current operation
- **AND** NOT proceed with incomplete state

#### Scenario: MCP error message
- **WHEN** MCP failure occurs
- **THEN** agent SHALL display: "GitHub operation failed: [error details]. Please retry or check MCP connection."

### Requirement: Issue Status Management
The agent SHALL manage issue status via labels.

#### Scenario: Status label usage
- **WHEN** issue status changes
- **THEN** the agent SHALL suggest using labels (e.g., `status:in-progress`, `status:in-review`)
- **AND** adapt to GitHub Projects if user prefers

### Requirement: Spec-to-Issue Sync
Spec changes SHALL be optionally synced to the linked GitHub Issue with user confirmation.

#### Scenario: Spec update detected
- **WHEN** `proposal.md`, `tasks.md`, or `specs/` files are pushed to remote (`git push`)
- **THEN** the agent SHALL detect the pushed changes

#### Scenario: User confirmation for sync
- **WHEN** spec changes are detected
- **THEN** the agent SHALL ask: "Reflect spec changes to GitHub Issue #N?"
- **AND** wait for explicit user confirmation
- **AND** NOT sync automatically

#### Scenario: Confirmed sync
- **GIVEN** user confirmed sync
- **WHEN** sync is executed
- **THEN** `proposal.md` "Why" section SHALL update issue description
- **AND** `tasks.md` phases SHALL update issue task list
- **AND** completed phases SHALL check corresponding checkboxes

#### Scenario: Declined sync
- **GIVEN** user declined sync
- **WHEN** agent continues
- **THEN** no changes SHALL be made to GitHub Issue
- **AND** agent SHALL continue silently

### Requirement: Phase Transition
When moving between phases in a complex spec, the agent SHALL prompt for child issue closure.

#### Scenario: Phase completion declared
- **WHEN** user explicitly states phase is complete (e.g., "Phase 1 complete")
- **THEN** the agent SHALL identify the corresponding child issue
- **AND** proceed with phase transition flow

#### Scenario: Prompt to close phase issue
- **WHEN** user declares phase complete
- **THEN** the agent SHALL ask: "Close phase issue #N and move to next phase?"
- **AND** wait for explicit user confirmation
- **AND** NOT close automatically

#### Scenario: User confirms phase close
- **GIVEN** user confirmed phase close
- **WHEN** agent proceeds
- **THEN** the child issue SHALL be closed via MCP GitHub tool
- **AND** the parent issue task list checkbox SHALL be checked
- **AND** agent SHALL proceed to next phase

#### Scenario: User declines phase close
- **GIVEN** user declined phase close
- **WHEN** agent proceeds
- **THEN** the child issue SHALL remain open
- **AND** agent SHALL proceed to next phase

### Requirement: Archive Validation
Before archiving a spec, the agent SHALL validate all related issues are closed.

#### Scenario: Archive command invoked
- **WHEN** `/openspec archive` command is invoked
- **THEN** the agent SHALL identify all linked issues (parent + children)
- **AND** check status of each issue via MCP GitHub tool

#### Scenario: All issues closed
- **GIVEN** all related issues are closed
- **WHEN** archive validation runs
- **THEN** archive SHALL proceed without warning
- **AND** spec SHALL be moved to archived state

#### Scenario: Open issues found
- **GIVEN** one or more related issues are still open
- **WHEN** archive validation runs
- **THEN** the agent SHALL warn: "The following issues are still open: #N, #M"
- **AND** ask: "Close all and proceed with archive?" or "Cancel archive?"
- **AND** wait for user response

#### Scenario: User confirms close and archive
- **GIVEN** user confirmed "Close all and proceed"
- **WHEN** agent proceeds
- **THEN** all open issues SHALL be closed via MCP GitHub tool
- **AND** spec SHALL be archived

#### Scenario: User cancels archive
- **GIVEN** user cancelled archive
- **WHEN** agent proceeds
- **THEN** no issues SHALL be closed
- **AND** spec SHALL NOT be archived
- **AND** agent SHALL inform user archive was cancelled

### Requirement: Specs Require Issues
All spec changes SHALL have an associated GitHub Issue.

#### Scenario: Mandatory issue linkage
- **WHEN** spec docs are created
- **THEN** `proposal.md` SHALL contain a reference to the linked GitHub Issue
- **AND** the reference SHALL be in the format `Relates to #N` or `Closes #N`

#### Scenario: Issue status on spec start
- **WHEN** spec work begins (spec docs created)
- **THEN** the linked issue SHALL transition to "In Progress" status
- **AND** this transition SHALL be automatic via agent command

#### Scenario: Prevent unlinked specs
- **WHEN** attempting to create spec docs without an issue reference
- **THEN** the agent SHALL block the creation
- **AND** require an issue to be created or linked first
