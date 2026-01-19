---
description: Quick capture of discovered work items as GitHub Issues.
---
# Workflow: /openspec-issue-track

**Purpose**: Fast entry for work items found during implementation to prevent "TODO leakage" in source code.

**Steps**
1. **Input**: Problem description from the user.
2. **Analysis**: Antigravity analyzes the description to determine the appropriate issue type and priority.
   - Keywords like "fail", "error", "broken" -> `type:bug`
   - Keywords like "add", "increase", "new" -> `type:feature`
   - Keywords like "refactor", "cleanup", "debt" -> `type:tech-debt`
3. **Issue Creation**:
   - Call `mcp_github-mcp-server_issue_write` with:
     - `method`: `create`
     - `owner`: `lxfactorl`
     - `repo`: `mono-repo`
     - `title`: A concise title derived from the description.
     - `body`: The detailed description provided by the user.
     - `labels`: The determined `type:*` label and `backlog`.
4. **Completion**: Return the created Issue URL to the user.

**Reference**
- Use `mcp_github-mcp-server_issue_write` exclusively for creation.
- Default to `backlog` label for all captures.
