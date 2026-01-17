# Git Workflow Specification

## ADDED Requirements

### Requirement: Single Active Spec Enforcement
The system SHALL prevent opening a new OpenSpec proposal if there is an existing active (non-archived) spec that has not been fully merged and pushed to GitHub.

#### Scenario: Block new proposal when previous spec is active
- **WHEN** a user attempts to create a new OpenSpec proposal
- **AND** there exists an active change in `openspec/changes/` that is not archived
- **THEN** the workflow SHALL display an error message indicating the active spec must be archived first
- **AND** the workflow SHALL NOT create a new proposal

#### Scenario: Allow new proposal when no active specs exist
- **WHEN** a user attempts to create a new OpenSpec proposal
- **AND** no active (non-archived) changes exist in `openspec/changes/`
- **THEN** the workflow SHALL proceed with proposal creation

---

### Requirement: Spec Branch Workflow
The system SHALL require each OpenSpec implementation to be executed on a dedicated branch following the naming convention `spec/<change-id>`.

#### Scenario: Create spec branch when starting implementation
- **WHEN** a user starts implementing an approved OpenSpec proposal with change-id `<id>`
- **THEN** the workflow SHALL create a new branch named `spec/<id>` from `master`
- **AND** the workflow SHALL switch to that branch

#### Scenario: Block implementation on master branch
- **WHEN** a user attempts to implement an OpenSpec change
- **AND** the current branch is `master`
- **THEN** the workflow SHALL refuse to proceed
- **AND** the workflow SHALL instruct the user to create or switch to a spec branch

---

### Requirement: Protected Master Branch
The `master` branch SHALL be protected from direct pushes; all changes MUST go through a pull request.

#### Scenario: Direct push to master is rejected
- **WHEN** a user attempts to push directly to `master`
- **THEN** GitHub SHALL reject the push
- **AND** the user SHALL receive an error indicating branch protection rules

#### Scenario: Changes merged via pull request
- **WHEN** a spec branch is ready for merge
- **THEN** the user SHALL create a pull request from `spec/<id>` to `master`
- **AND** upon approval, the changes SHALL be merged to `master`

---

### Requirement: Archive Gate Check
The system SHALL verify that the spec branch has been merged and pushed to GitHub before allowing a change to be archived.

#### Scenario: Block archive when branch not merged
- **WHEN** a user attempts to archive a change with change-id `<id>`
- **AND** the branch `spec/<id>` has not been merged to `master`
- **THEN** the workflow SHALL display an error
- **AND** the workflow SHALL NOT archive the change

#### Scenario: Allow archive when branch is merged
- **WHEN** a user attempts to archive a change with change-id `<id>`
- **AND** the branch `spec/<id>` has been merged to `master`
- **THEN** the workflow SHALL proceed with archiving

---

### Requirement: Pull Request Auto-Description
The system SHALL automatically generate pull request descriptions from the current spec being implemented, providing a clear summary of the change.

#### Scenario: Generate PR description from proposal
- **WHEN** a user creates a pull request from branch `spec/<id>` to `master`
- **THEN** the workflow SHALL collect the following from `openspec/changes/<id>/`:
  - Title from `proposal.md` "Change:" header
  - "Why" and "What Changes" sections from `proposal.md`
  - Task completion status from `tasks.md`
- **AND** the workflow SHALL format this as the PR description

#### Scenario: Include spec delta summary
- **WHEN** generating PR description
- **AND** spec deltas exist in `changes/<id>/specs/`
- **THEN** the workflow SHALL include a list of affected capabilities and requirement changes (ADDED/MODIFIED/REMOVED)
