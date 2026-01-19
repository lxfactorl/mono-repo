# Rule: OpenSpec Issue Tracking Integration

This rule enforces the integration between OpenSpec development and GitHub Issue tracking. It ensures every spec change is traceable and visible in the project backlog.

## Mandatory Issue Linking
- **Antigravity MUST NOT** start a new OpenSpec proposal without an associated GitHub Issue.
- All new proposals created via `/openspec-spec-create` or `/openspec-spec-from-issue` MUST include a `Relates to #N` reference in the `proposal.md` under a `## Linked Issue` section.
- If a user asks to start a spec without a description that maps to an issue, Antigravity MUST prompt to create an issue first or search for an existing one.

## Post-Push Synchronization
- After every `git push` that includes changes to `openspec/changes/<id>/`, Antigravity MUST check if there are updates to `tasks.md` or `proposal.md`.
- Antigravity MUST prompt the user: "Reflect spec changes to GitHub Issue #N?"
- If confirmed, Antigravity SHALL:
  1. Read the current `tasks.md`.
  2. Update the GitHub Issue body with the current task list status (syncing `[ ]` and `[x]` states).
  3. Use `mcp_github-mcp-server_issue_write` with the `body` parameter containing the updated task list.
  4. If significant progress is made (e.g., a phase is completed), add an issue comment with a summary of changes.

## Discovery Capture
- If during implementation, a new bug, feature idea, or technical debt is identified, Antigravity MUST suggest capturing it via `/openspec-issue-track` instead of adding TODO comments to the code.

## MCP Usage Guidelines
- Always use `mcp_github-mcp-server_issue_read` to fetch the latest state before updating.
- Always use `mcp_github-mcp-server_issue_write` for status, label, and body updates.
- If a GitHub operation fails, report the error clearly and do not proceed with local changes that depend on that link.
