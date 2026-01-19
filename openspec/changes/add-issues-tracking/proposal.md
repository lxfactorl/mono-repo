# Change: Add Issues Tracking Workflow

## Why

This is a personal project managed on GitHub by a single developer. During feature implementation, I frequently discover additional work items that need to be captured for later — bugs to fix, improvements to make, or technical debt to address. Currently, I track these in scattered notes and TODO comments, but this approach lacks:

- **Visibility**: No central view of all pending work
- **Flexibility**: Hard to reprioritize or categorize items
- **Traceability**: No link between discovered issues and the work that uncovered them
- **Persistence**: Notes get lost or forgotten

GitHub Issues provides a native, integrated solution that fits the existing GitHub-based workflow. Additionally, this change extends the OpenSpec flow to integrate issue tracking into the spec-driven development process.

## What Changes

### GitHub Issues Configuration
- Define issue templates for common issue types (Bug Report, Feature Request, Technical Debt)
- Configure issue labels for categorization (priority, type, affected service)
- Set up issue forms with structured fields for consistent information capture

### Issue Workflow
- Define issue lifecycle states (Open, In Progress, In Review, Done)
- Establish linking conventions between issues, PRs, and commits
- Configure automatic issue closing via commit/PR references
- **Bidirectional traceability**:
  - From commit/PR → navigate to related issue
  - From issue → see all related PRs, commits, and code changes

### OpenSpec Integration
- **During implementation**: When working on a spec change, discovered work items are captured as GitHub Issues instead of TODO comments
- **Backlog management**: Issues serve as the backlog for future spec proposals
- **Linking**: Spec proposals MAY reference related issues in the `proposal.md`
- **Single active change rule**: Since only one spec change can be active at a time, new discoveries go to Issues backlog rather than creating parallel proposals

### Agent Workflow: `/openspec-issue-track`
- **Purpose**: Quick capture of discovered work items without breaking flow
- **Input**: Problem description from user
- **Agent behavior**: Antigravity analyzes the description to determine issue type then creates the GitHub Issue via `mcp_github-mcp-server_issue_write`.
  3. Apply relevant labels (type, priority if mentioned)
  4. Return issue URL to user
- **No OpenSpec docs created**: Issue tracking only — spec proposal created later when work begins
- **Implementation**: Antigravity workflow in `.agent/workflows/openspec-issue-track.md`

### Agent Workflow: `/openspec-spec-create`
- **Purpose**: Start work on a new spec change with mandatory issue linking
- **Input**: Description of the spec change
- **Agent behavior**:
  1. Verify no active spec is in progress (enforce single active change rule)
  2. Search GitHub Issues for relevant existing issues via MCP GitHub tool
  3. **If candidates found** → present list to user and ask:
     - "Link to existing issue #N?" or
     - "Create new issue?"
  4. If no candidates or user chooses "create new" → create new issue via MCP GitHub tool
  5. Create spec docs (`proposal.md`, `tasks.md`, `specs/`) with issue reference
  6. Update issue status to "In Progress" (apply label or move to project column)
  7. Return spec path and linked issue URL
- **Duplicate prevention**: Agent MUST check for existing issues before creating new ones
- **Issue search**: Agent extracts keywords from user description and searches open issues via MCP (best effort)
- **Strict requirement**: Spec docs MUST reference a GitHub Issue
- **Implementation**: Antigravity workflow in `.agent/workflows/openspec-spec-create.md`

### Agent Workflow: `/openspec-spec-from-issue`
- **Purpose**: Create spec proposal from an existing GitHub Issue
- **Input**: GitHub Issue URL (e.g., `https://github.com/owner/repo/issues/123` or `#123`)
- **Agent behavior**:
  1. Verify no active spec is in progress (enforce single active change rule)
  2. Fetch issue data from GitHub via MCP GitHub tool
  3. **If issue is closed** → ask user: "Issue #N is closed. Reopen and continue?"
  4. **If issue is already In Progress** → ask user: "Issue #N is already In Progress. Continue with this issue?"
  5. Create spec docs with pre-filled content:
     - `proposal.md` "Why" section ← issue body/description
     - `proposal.md` "What Changes" ← extracted from issue if available
     - Issue reference automatically included
  4. Derive change-id from issue title (kebab-case, verb-led)
  5. Update issue status to "In Progress"
  6. Return spec path and linked issue URL
- **Pre-filled mapping**:
  - Issue title → proposal title and change-id
  - Issue body → "Why" section
  - Issue labels → spec metadata
- **Non verb-led title handling**: If issue title doesn't start with verb, ask user for change-id
- **Implementation**: Antigravity workflow in `.agent/workflows/openspec-spec-from-issue.md`

### Complex Specs: Parent + Child Issues
- **Threshold**: If spec has 3+ phases in `tasks.md`, use hierarchical issues
- **Structure**:
  - Parent issue = main spec/feature (e.g., "Add Railway CD Pipeline")
  - Child issues = one per phase (e.g., "Phase 1: Service Preparation")
- **Linking**:
  - Child issues contain `Part of #N` referencing parent
  - Parent issue has task list with links: `- [ ] #11`, `- [ ] #12`
- **PR workflow**:
  - Each phase PR → `Closes #child-issue`
  - Final archive → closes parent issue
- **Simple specs (1-2 phases)**: Single issue with task list checkboxes

### Spec-to-Issue Sync
- **Trigger**: After `git push` (agent detects pushed changes to spec docs)
- **Agent behavior**:
  1. Detect pushed changes in `proposal.md`, `tasks.md`, or `specs/`
  2. **Ask user explicitly**: "Reflect spec changes to GitHub Issue #N?"
  3. If confirmed → update issue body/task list via MCP GitHub tool
  4. If declined → no sync, continue silently
- **Sync mapping**:
  - `proposal.md` "Why" section → issue description
  - `tasks.md` phases → issue task list checkboxes
  - Phase completion → checkbox checked

### Phase Transition (Complex Specs)
- **Trigger**: User explicitly tells agent phase is complete (no auto-detection)
- **Agent behavior**:
  1. User states phase is done (e.g., "Phase 1 complete, moving to Phase 2")
  2. **Ask user explicitly**: "Close phase issue #N and move to next phase?"
  3. If confirmed → close child issue via MCP GitHub tool
  4. If declined → keep open, continue to next phase
- **Rationale**: Gives user full control; no implicit triggers

### Agent Workflow: `/openspec-spec-cancel`
- **Purpose**: Abandon current spec without completing it
- **Agent behavior**:
  1. Verify active spec exists
  2. **Ask user**: "Cancel spec and revert issue to Open status?" or "Close issue as won't fix?"
  3. If revert → remove "In Progress" label, issue stays open for future
  4. If close → close issue with "won't fix" comment
  5. For complex specs: ask about child issues (close all or keep open)
  6. Delete or archive spec docs based on user preference
  7. Return confirmation
- **Implementation**: Antigravity workflow in `.agent/workflows/openspec-spec-cancel.md`

### Archive Validation
- **Trigger**: When `/openspec-archive` workflow is invoked
- **Agent behavior**:
  1. Identify all linked issues (parent + children for complex specs)
  2. Check status of all related issues via MCP GitHub tool
  3. **If any issues still open**:
     - Warn user: "The following issues are still open: #N, #M"
     - Ask: "Close all and proceed with archive?" or "Cancel archive?"
  4. If user confirms → close all open issues, then archive
  5. If user cancels → abort archive, keep issues open
- **Validation checks**:
  - All child issues closed (for complex specs)
  - Parent issue ready to close
  - No orphan issues left behind

### Issue Reference Format in proposal.md
- **Location**: Dedicated `## Linked Issue` section after "Why"
- **Format**:
  ```markdown
  ## Linked Issue
  
  Relates to #123
  ```
- **For complex specs**: Include parent and child references

### Multi-Service Issues
- **Approach**: Create main issue for primary service, with sub-issues for other affected services
- **Labeling**: Main issue gets primary `service:X` label; sub-issues get their respective service labels
- **Linking**: Sub-issues contain `Part of #main` in body

### MCP GitHub Tool Error Handling
- **On failure** (auth error, rate limit, network): Agent shows ERROR and stops
- **Message**: "GitHub operation failed: [error details]. Please retry or check MCP connection."
- **No fallback**: Agent does not proceed without successful GitHub operation

### Orphan Child Issues
- **Detection**: If parent issue not found but children reference it
- **Agent behavior**: Ask user "Found orphan child issues #X, #Y. Delete, close, or keep?"

### Issue Status Management
- **Approach**: Agent suggests using labels (e.g., `status:in-progress`, `status:in-review`)
- **Alternative**: If user prefers GitHub Projects, agent adapts
- **Default**: Labels (simpler for single developer)

### Strict Limitation: Specs Require Issues
- **Rule**: No spec work SHALL begin without an associated GitHub Issue
- **Enforcement**: Agent workflows validate issue linkage before creating spec docs
- **Rationale**: Ensures all work is tracked and visible in the backlog

### Documentation
- `docs/issues/README.md` — Issue workflow overview and OpenSpec integration
- Update `openspec/AGENTS.md` — Add guidance for capturing issues during implementation

## Impact

### Affected Specs
- `issues-tracking` (new capability)

### Affected Files
- `.github/ISSUE_TEMPLATE/bug_report.yml` (new)
- `.github/ISSUE_TEMPLATE/feature_request.yml` (new)
- `.github/ISSUE_TEMPLATE/technical_debt.yml` (new)
- `.github/ISSUE_TEMPLATE/config.yml` (new)
- `.agent/workflows/openspec-issue-track.md` (new) — `/openspec-issue-track` workflow
- `.agent/workflows/openspec-spec-create.md` (new) — `/openspec-spec-create` workflow
- `.agent/workflows/openspec-spec-from-issue.md` (new) — `/openspec-spec-from-issue` workflow
- `.agent/workflows/openspec-spec-cancel.md` (new) — `/openspec-spec-cancel` workflow
- `.agent/workflows/openspec-archive.md` (new) — `/openspec-archive` workflow (extends existing)
- `.agent/rules/openspec-issues.md` (new) — Always-on rule for issue tracking guidance

### Affected OpenSpec Docs
- `openspec/AGENTS.md` — Add section on issue tracking during implementation

### Process Changes
- Discovered work items during implementation → Create GitHub Issue (not TODO comment)
- Starting spec work → Must have linked GitHub Issue (created or existing)
- Spec docs MUST include issue reference in `proposal.md`
- Issue status transitions to "In Progress" when spec work begins
- PRs and commits MUST reference related issues using `Closes #N` or `Relates to #N`
- **Full traceability**: Issue ↔ Spec ↔ PR ↔ Commits all linked
