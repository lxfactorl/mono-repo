# Issue Tracking and Backlog Management

This project uses **GitHub Issues** as the authoritative source for the backlog and high-level project management. This system works in tandem with **OpenSpec** for detailed requirements and implementation tracking.

## Lifecycle States

### 1. Backlog (`backlog` label)
Ideas, bugs, and technical debt that haven't been started yet. Captured via the GitHub UI or the `/openspec-issue-track` command.

### 2. Proposal (`openspec:proposal` label)
A specific change is being drafted. A spec branch `spec/<change-id>` exists locally and in GitHub.

### 3. Active (`openspec:active`, `status:in-progress` labels)
Implementation is underway. The proposal has been approved, and the Antigravity agent is working through `tasks.md`.

### 4. Review (`status:in-review` label)
The implementation is complete, and a Pull Request is open targeting `master`.

### 5. Archived (`openspec:archived` label, `CLOSED` state)
The PR has been merged, deployment verified, and the OpenSpec change archived.

## Tagging & Traceability

- **Issue ↔ Spec**: All OpenSpec `proposal.md` files MUST link to the corresponding issue using `Relates to #N`.
- **Commit/PR ↔ Issue**: Commits and PRs should use the `Closes #N` or `Relates to #N` syntax to link back to the issue.
- **Service Tags**: Use `service:<name>` labels (e.g., `service:notification`) to categorize issues by the backend service they affect.

## Slash Commands (for Antigravity)

- `/openspec-issue-track`: Quick capture of discovered items during implementation.
- `/openspec-spec-create`: Start a new proposal while ensuring it's linked to an issue.
- `/openspec-spec-from-issue`: Convert a backlog item into an active spec proposal.
- `/openspec-spec-cancel`: Cleanup and revert issues if work is abandoned.

## Automated Synchronization
When progress is pushed to a spec branch, the Antigravity agent will prompt to update the GitHub Issue. This syncs the checkboxes in the GitHub Issue body with the current state of `tasks.md`.
