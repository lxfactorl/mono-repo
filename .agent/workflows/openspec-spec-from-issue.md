---
description: Create a spec proposal from an existing GitHub Issue.
---
# Workflow: /openspec-spec-from-issue

**Purpose**: Directly promotes a backlog item to an active proposal.

**Steps**
1. **Input**: GitHub Issue URL or #ID.
2. **Fetch Data**:
   - Call `mcp_github-mcp-server_issue_read` to fetch title, body, and labels.
3. **Status Check**:
   - If issue is closed, ask: "Issue #N is closed. Reopen and continue?"
   - If user says yes, use `mcp_github-mcp-server_issue_write` to set `state: open`.
4. **Local Setup**:
   - Derive `change-id` from title (kebab-case, verb-led). If title isn't verb-led, prompt for a name.
   - Scaffold `proposal.md`:
     - "Why" section -> Issue Body.
     - Include issue link.
   - Scaffold `tasks.md` and `design.md` (if needed).
5. **Issue Update**:
   - Update labels via `mcp_github-mcp-server_issue_write`: remove `backlog`, add `openspec:proposal`, `status:in-progress`.
6. **Completion**: Return the local path and instructions to start implementation.

**Reference**
- Use the issue body to pre-fill as much as possible to save time.
