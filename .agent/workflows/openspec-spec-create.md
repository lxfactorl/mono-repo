---
description: Scaffold a new OpenSpec change with mandatory GitHub Issue linking.
---
# Workflow: /openspec-spec-create

**Purpose**: Starts a new spec change while ensuring it is tracked in the GitHub backlog.

**Steps**
1. **Zero Point Gate**: 
   - Verify no active change exists in `openspec/changes/` (non-archived).
   - If one exists, STOP and notify the user.
2. **Backlog Search**:
   - Extract keywords from the user's request.
   - Call `mcp_github-mcp-server_search_issues` with query: `is:issue is:open [keywords] repo:lxfactorl/mono-repo`.
   - Present findings to the user.
3. **Issue Selection**:
   - Ask the user: "Link to existing issue #N?" or "Create new issue?"
4. **Issue Setup**:
   - **If existing**: Fetch details via `mcp_github-mcp-server_issue_read`.
   - **If new**: Call `mcp_github-mcp-server_issue_write` (method: `create`) with the user's description.
   - Add labels `openspec:proposal` and `status:in-progress`.
5. **Scaffolding**:
   - Create local files: `proposal.md`, `tasks.md`, and `design.md` (if required).
   - Ensure `proposal.md` includes:
     ```markdown
     ## Linked Issue
     Relates to #N
     ```
6. **Completion**: Return the local path and the Issue URL.

**Reference**
- Enforce the "Single Active Change" rule strictly.
- Every spec created MUST have a valid Issue ID.
