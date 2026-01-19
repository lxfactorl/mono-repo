# issue-tracking Specification

## ADDED Requirements

### Requirement: GitHub Issue Backlog
The project SHALL maintain its primary backlog as GitHub Issues to ensure visibility and multi-agent collaboration support.

#### Scenario: Backlog Visibility
- **GIVEN** a stakeholder without local access to the repository
- **WHEN** they view the GitHub Issues page
- **THEN** they SHALL be able to see all pending, active, and completed work items
- **AND** they SHALL be able to identify the current priority of each item via labels

### Requirement: OpenSpec-Issue Mapping
Every OpenSpec `Change` SHALL be linked to a unique GitHub Issue to synchronize technical documentation with project management.

#### Scenario: Linking a Change to an Issue
- **GIVEN** an active OpenSpec change `add-issues-tracking`
- **WHEN** the proposal is created
- **THEN** it SHALL include the GitHub Issue number in the `proposal.md`
- **AND** the issue SHALL be updated with a link to the corresponding branch or PR

### Requirement: Label-Based Lifecycle Tracking
GitHub Issues SHALL use a standardized set of labels to reflect the OpenSpec lifecycle state.

#### Scenario: Advancing Lifecycle State
- **GIVEN** a GitHub issue in the `backlog` state
- **WHEN** a proposal is started for it
- **THEN** the `backlog` label SHALL be removed
- **AND** the `openspec:proposal` label SHALL be added
