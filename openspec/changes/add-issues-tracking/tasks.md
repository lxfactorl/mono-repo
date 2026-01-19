# Tasks: Add Issues Tracking

## 1. GitHub Repository Setup (Infrastructure)

- [x] 1.1 Create standardized GitHub Labels:
  - `openspec:proposal`, `openspec:active`, `openspec:archived`
  - `type:bug`, `type:feature`, `type:tech-debt`, `type:documentation`
  - `priority:critical`, `priority:high`, `priority:medium`, `priority:low`
  - `status:in-progress`, `status:in-review`, `backlog`
- [x] 1.2 Create Issue Templates in `.github/ISSUE_TEMPLATE/`:
  - `bug_report.yml`, `feature_request.yml`, `technical_debt.yml`, `config.yml` (dropdown for services)
- [x] 1.3 **Verify**: Run `mcp_github-mcp-server_get_label` for one new label to ensure creation.
- [x] 1.4 **Verify**: Manually check templates render in GitHub UI.

## 2. Base Rules and Shared Logic

- [x] 2.1 Create `.agent/rules/openspec-issues.md`:
  - Define the "Spec requires Issue" mandate.
  - Define the post-push sync prompt logic.
  - Specify usage of `mcp_github-mcp-server_issue_read` and `mcp_github-mcp-server_issue_write` for syncing.
- [x] 2.2 **Verify**: Agent acknowledges the new rule in a fresh conversation.

## 3. Workflow: `/openspec-issue-track`

- [x] 3.1 Create `.agent/workflows/openspec-issue-track.md`:
  - Input: Problem description.
  - Action: Determine type -> Call `mcp_github-mcp-server_issue_write(method: 'create', ...)`.
  - Output: Issue link.
- [x] 3.2 **Verify**: Run command with "Found a memory leak in notification service", check if `type:bug` is applied.

## 4. Workflow: `/openspec-spec-create`

- [x] 4.1 Create `.agent/workflows/openspec-spec-create.md`:
  - Step: Active spec check (local `ls openspec/changes/`).
  - Step: GitHub Search via `mcp_github-mcp-server_search_issues`.
  - Step: Scaffolding with `Linked Issue: #N`.
- [x] 4.2 **Verify**: Attempt to start a spec when one is already active (should block).
- [x] 4.3 **Verify**: Search logic finds an existing "Railway" issue and prompts to link.

## 5. Workflow: `/openspec-spec-from-issue`

- [x] 5.1 Create `.agent/workflows/openspec-spec-from-issue.md`:
  - Input: Issue URL/#ID.
  - Action: `mcp_github-mcp-server_issue_read` -> Scaffold doc fields from response.
  - Action: `mcp_github-mcp-server_issue_write` to set `status:in-progress`.
- [x] 5.2 **Verify**: Run on a test issue, check if kebab-cased change-id is correctly derived.

## 6. Workflow: `/openspec-spec-cancel`

- [x] 6.1 Create `.agent/workflows/openspec-spec-cancel.md`:
  - Prompt: Revert vs Close.
  - Action: `mcp_github-mcp-server_issue_write(state: 'open/closed', labels: [...])`.
  - Action: Repository cleanup (`rm -rf`).
- [x] 6.2 **Verify**: Cancel a spec and confirm the issue labels revert to `backlog`.

## 7. Integrated Archiving (`/openspec-archive`)

- [x] 7.1 Modify `.agent/workflows/openspec-archive.md`:
  - Add pre-archive check for linked issues via `mcp_github-mcp-server_issue_read`.
  - Add batch closing via `mcp_github-mcp-server_issue_write`.
- [x] 7.2 **Verify**: Archive a change and confirm the linked issue is closed automatically.

## 8. Documentation and Guidelines

- [x] 8.1 Update `openspec/AGENTS.md` with:
  - New workflows documentation.
  - Strict mapping rules.
  - "Discovery -> Issue" convention.
- [x] 8.2 Create `docs/issues/README.md` for human reference (lifecycle, labels).
- [x] 8.3 **Verify**: All links between `docs/` and `openspec/` are valid.

---

## Definition of Done
- All 5 new workflows functional and verified.
- Existing archive workflow updated with issue closing logic.
- Post-push sync prompts user correctly.
- Issue templates and labels active in repository.
- No spec document created without a corresponding Issue ID.
