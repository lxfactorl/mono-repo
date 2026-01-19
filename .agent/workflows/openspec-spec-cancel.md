---
description: Abandon current spec without completing it and revert issue.
---
# Workflow: /openspec-spec-cancel

**Purpose**: Cleanup abandoned work and ensure the backlog stays accurate.

**Steps**
1. **Confirmation**:
   - Ask the user: "Cancel spec and revert issue #N to Open status?" or "Close issue as won't fix?"
2. **Issue Cleanup**:
   - Based on response, call `mcp_github-mcp-server_issue_write`:
     - **Revert**: Remove `openspec:*` and `status:*` labels, add `backlog`.
     - **Close**: Set `state: closed`, add a "won't fix" comment using `mcp_github-mcp-server_add_issue_comment`.
3. **Local Cleanup**:
   - Delete the `openspec/changes/<id>/` folder.
4. **Completion**: Confirm deletion and issue status change.

**Reference**
- Always ask for confirmation before deleting local files.
