---
description: Implement an approved OpenSpec change and keep tasks in sync.
---
<!-- OPENSPEC:START -->
**Guardrails**
- Favor straightforward, minimal implementations first and add complexity only when it is requested or clearly required.
- Keep changes tightly scoped to the requested outcome.
- Refer to `openspec/AGENTS.md` (located inside the `openspec/` directory—run `ls openspec` or `openspec update` if you don't see it) if you need additional OpenSpec conventions or clarifications.

**Steps**
Track these steps as TODOs and complete them one by one.
0. **Git Workflow Gate**: Check current branch with `git branch --show-current`. If on `master`, create and switch to a spec branch: `git checkout -b spec/<change-id>`. If already on a matching spec branch, proceed. If on a different branch, STOP and ask the user to confirm branch management.
1. Read `changes/<id>/proposal.md`, `design.md` (if present), and `tasks.md` to confirm scope and acceptance criteria.
2. Work through tasks sequentially, keeping edits minimal and focused on the requested change.
3. Confirm completion before updating statuses—make sure every item in `tasks.md` is finished.
4. Update the checklist after all work is done so each task is marked `- [x]` and reflects reality.
5. Push the spec branch and create a Pull Request to `master`. Use `/openspec-pr` to generate the PR description.
6. **Wait for PR merge** — Archiving is a separate step that happens AFTER the PR is merged and deployment is verified.

**Reference**
- Use `openspec show <id> --json --deltas-only` if you need additional context from the proposal while implementing.
- After PR is merged: run `/openspec-archive <id>` to complete the change lifecycle.
<!-- OPENSPEC:END -->
