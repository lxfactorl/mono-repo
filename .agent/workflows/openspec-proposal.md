---
description: Scaffold a new OpenSpec change and validate strictly.
---
<!-- OPENSPEC:START -->
**Guardrails**
- Favor straightforward, minimal implementations first and add complexity only when it is requested or clearly required.
- Keep changes tightly scoped to the requested outcome.
- Refer to `openspec/AGENTS.md` (located inside the `openspec/` directory—run `ls openspec` or `openspec update` if you don't see it) if you need additional OpenSpec conventions or clarifications.
- Identify any vague or ambiguous details and ask the necessary follow-up questions before editing files.
- Do not write any code during the proposal stage. Only create design documents (proposal.md, tasks.md, design.md, and spec deltas). Implementation happens in the apply stage after approval.

**Steps**
0. **Zero Point Gate**: Ensure the workspace is clean before starting.
   a. Check out `master` and pull latest changes: `git checkout master && git pull`.
   b. Ensure no local feature/spec branches exist. Run `git branch` to list local branches. If any branches other than `master` exist, ask the user to delete them or confirm they are safe to keep.
   c. Run `openspec list` and check for any active (non-archived) changes in `openspec/changes/`. If an active change exists, STOP and inform the user: "Cannot create new proposal — active change `<id>` must be archived first."
   Only proceed if these conditions are met.
1. Review `openspec/project.md`, run `openspec list` and `openspec list --specs`, and inspect related code or docs (e.g., via `rg`/`ls`) to ground the proposal in current behaviour; note any gaps that require clarification.
2. Choose a unique verb-led `change-id` and scaffold `proposal.md`, `tasks.md`, and `design.md` under `openspec/changes/<id>/`. The `design.md` is **required** when any of the following apply:
   - External service integrations (APIs, databases, message brokers)
   - New architectural patterns or abstractions
   - Cross-system or cross-project communication
   - Security, secrets, or authentication handling
   - Data migrations or schema changes
   If none apply, `design.md` may be omitted for simple, isolated changes.
3. Map the change into concrete capabilities or requirements, breaking multi-scope efforts into distinct spec deltas with clear relationships and sequencing.
4. In `design.md`, document: system context diagram (if multi-system), data flow, configuration schema, error handling strategy, and key trade-offs. Skip only for trivial changes that don't meet the criteria in step 2.
5. Draft spec deltas in `changes/<id>/specs/<capability>/spec.md` (one folder per capability) using `## ADDED|MODIFIED|REMOVED Requirements` with at least one `#### Scenario:` per requirement and cross-reference related capabilities when relevant.
6. Draft `tasks.md` as an ordered list of small, verifiable work items that deliver user-visible progress, include validation (tests, tooling), and highlight dependencies or parallelizable work.
7. Validate with `openspec validate <id> --strict --no-interactive` and resolve every issue before sharing the proposal.

**Reference**
- Use `openspec show <id> --json --deltas-only` or `openspec show <spec> --type spec` to inspect details when validation fails.
- Search existing requirements with `rg -n "Requirement:|Scenario:" openspec/specs` before writing new ones.
- Explore the codebase with `rg <keyword>`, `ls`, or direct file reads so proposals align with current implementation realities.
<!-- OPENSPEC:END -->
