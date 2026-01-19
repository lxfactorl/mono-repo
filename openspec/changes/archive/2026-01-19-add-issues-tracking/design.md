# Design: GitHub Issues & OpenSpec Workflow Integration

## System Context
This design integrates the local-first, spec-driven development process of OpenSpec with the centralized project management capabilities of GitHub Issues. The integration is performed by the Antigravity agent using the `github-mcp-server`.

## State Machine: Spec & Issue Lifecycle

| OpenSpec State | GitHub Issue State | Primary Labels | Actions |
| :--- | :--- | :--- | :--- |
| **New Idea** | `OPEN` | `backlog` | Captured via `/openspec-issue-track` |
| **Proposed** | `OPEN` | `openspec:proposal` | Created/Linked via `/openspec-spec-create` |
| **Approved/In Progress** | `OPEN` | `openspec:active`, `status:in-progress` | `/openspec-apply` status |
| **In Review** | `OPEN` | `status:in-review` | PR Created |
| **Implemented & Archived** | `CLOSED` | `openspec:archived` | Merged and `/openspec-archive` run |
| **Cancelled** | `OPEN` or `CLOSED` | (Back to `backlog` or `won't fix`) | `/openspec-spec-cancel` |

## Workflow Implementation Details

### 1. `/openspec-issue-track` (Capture Discovery)
- **Goal**: Fast entry for work items found during implementation.
- **MCP Tool**: `mcp_github-mcp-server_issue_write` (Method: `create`).
- **Logic**: Extracts keywords to determine labels (`type:bug`, `type:feature`, `type:tech-debt`).
- **Output**: Returns the GitHub Issue URL.

### 2. `/openspec-spec-create` (Start Work)
- **Goal**: Ensures no work begins without an issue.
- **Logic**:
    - Scans `openspec/changes/` for active changes.
    - Uses `mcp_github-mcp-server_search_issues` with query `is:issue is:open [keywords]` to prevent duplicates.
    - If user picks one, `mcp_github-mcp-server_issue_read` fetches details to pre-populate `proposal.md`.
    - If creating new, `mcp_github-mcp-server_issue_write` with `openspec:proposal` label.
    - Scaffolds local files with `Linked Issue: #N` in `proposal.md`.

### 3. `/openspec-spec-from-issue` (Issue -> Spec)
- **Goal**: Directly convert a backlog item to an active proposal.
- **Logic**:
    - Fetches issue via `mcp_github-mcp-server_issue_read`.
    - Converts title to kebab-case `change-id`.
    - Maps Body to "Why" section.
    - Updates labels to `openspec:proposal`.

### 4. Spec-to-Issue Sync (The "Always On" Rule)
- **Trigger**: Detected file changes in `openspec/changes/[active-change]/` followed by `git push`.
- **Logic**: 
    - Agent reads `tasks.md` and `proposal.md`.
    - Prompts: "Reflect spec changes to GitHub Issue #N?"
    - If yes, uses `mcp_github-mcp-server_issue_write` to update the issue body with the current task list status (checkboxes).
    - Uses `mcp_github-mcp-server_add_issue_comment` for progress updates.

### 5. Hierarchical Tracking (Complex Specs)
- **Threshold**: `tasks.md` containing `Phase 3` or higher.
- **Structure**:
    - **Parent Issue**: The main feature.
    - **Child Issues**: Created via `mcp_github-mcp-server_issue_write` for each phase (`Phase 1: ...`, `Phase 2: ...`).
    - **Linking**: Child bodies include `Part of #ParentID`. Parent body updated with task list: `- [ ] #ChildID Phase N`.

## GitHub MCP Toolset Mapping

| Requirement | MCP Tool | Parameters |
| :--- | :--- | :--- |
| Create New Issue | `issue_write` | `method: create`, `title`, `body`, `labels[]` |
| Update Issue Status | `issue_write` | `method: update`, `issue_number`, `state`, `labels[]` |
| Link/Comment | `add_issue_comment` | `issue_number`, `body` |
| Search Backlog | `search_issues` | `query: "is:issue is:open [terms] repo:lxfactorl/mono-repo"` |
| Fetch Context | `issue_read` | `method: get`, `issue_number` |

## Error & Edge Case Handling
- **GitHub Unavailable**: Workflow stops and prompts user (no local-only bypass for these specific slash commands).
- **Issue Already Linked**: `/openspec-spec-create` stops if the picked issue is already marked `openspec:active` or `openspec:proposal` by another branch.
- **Rate Limits**: Agent reports rate limit errors and suggests waiting.
