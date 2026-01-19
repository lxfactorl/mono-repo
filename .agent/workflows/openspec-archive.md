---
description: Archive a deployed OpenSpec change and update specs.
---
<!-- OPENSPEC:START -->
**Guardrails**
- Favor straightforward, minimal implementations first and add complexity only when it is requested or clearly required.
- Keep changes tightly scoped to the requested outcome.
- Refer to `openspec/AGENTS.md` (located inside the `openspec/` directory—run `ls openspec` or `openspec update` if you don't see it) if you need additional OpenSpec conventions or clarifications.
- **Archiving is a SEPARATE step** that happens AFTER the implementation PR is merged and deployment is verified. This confirms the change works before marking it complete.

**Steps**
1. Determine the change ID to archive:
   - If this prompt already includes a specific change ID (for example inside a `<ChangeId>` block populated by slash-command arguments), use that value after trimming whitespace.
   - If the conversation references a change loosely (for example by title or summary), run `openspec list` to surface likely IDs, share the relevant candidates, and confirm which one the user intends.
   - Otherwise, review the conversation, run `openspec list`, and ask the user which change to archive; wait for a confirmed change ID before proceeding.
   - If you still cannot identify a single change ID, stop and tell the user you cannot archive anything yet.
2. Validate the change ID by running `openspec list` (or `openspec show <id>`) and stop if the change is missing, already archived, or otherwise not ready to archive.
3. **Git Workflow Gate**: Verify the spec branch `spec/<id>` has been merged to `master` by running `git branch --merged master | grep spec/<id>`. If the branch is NOT merged, STOP and inform the user: "Cannot archive — branch `spec/<id>` must be merged to master first via pull request."
4. Create a new branch for the archive: `git checkout -b archive/<id>` from `master`.
5. Run `openspec archive <id> --yes` so the CLI moves the change and applies spec updates without prompts (use `--skip-specs` only for tooling-only work).
6. Review the command output to confirm the target specs were updated and the change landed in `changes/archive/`.
7. Validate with `openspec validate --strict --no-interactive` and inspect with `openspec show <id>` if anything looks off.
8. Commit the archive changes and create a PR to merge `archive/<id>` to `master`.

**Issue Integration**
- Before archiving, identify the linked issue in `proposal.md`.
- Call `mcp_github-mcp-server_issue_read` to verify status.
- If open, call `mcp_github-mcp-server_issue_write` with `state: closed` and label `openspec:archived`.
- If child issues exist (per `design.md`), close them as well.

**Reference**
- Use `openspec list` to confirm change IDs before archiving.
- Inspect refreshed specs with `openspec list --specs` and address any validation issues before handing off.
<!-- OPENSPEC:END -->
