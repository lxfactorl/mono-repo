---
description: Generate a pull request description from the current OpenSpec change.
---
<!-- OPENSPEC:START -->
**Guardrails**
- Favor straightforward, minimal implementations first and add complexity only when it is requested or clearly required.
- Keep changes tightly scoped to the requested outcome.
- Refer to `openspec/AGENTS.md` (located inside the `openspec/` directory—run `ls openspec` or `openspec update` if you don't see it) if you need additional OpenSpec conventions or clarifications.

**Steps**
1. Determine the change ID for the PR:
   - If this prompt already includes a specific change ID, use that value.
   - Otherwise, check the current branch with `git branch --show-current` — if on `spec/<id>`, use that ID.
   - If unable to determine, run `openspec list` and ask the user which change to generate a PR for.
2. Read `openspec/changes/<id>/proposal.md` and extract:
   - Title from the `# Change:` header
   - "Why" section content
   - "What Changes" section content
3. Read `openspec/changes/<id>/tasks.md` and summarize task completion status.
4. Check for spec deltas in `openspec/changes/<id>/specs/` and list affected capabilities with their change types (ADDED/MODIFIED/REMOVED).
5. Generate the PR description in the following format:

```markdown
## Summary
[Title from proposal.md]

## Why
[Content from Why section]

## What Changes
[Content from What Changes section]

## Tasks
[Task completion summary, e.g., "5/5 tasks completed"]

## Affected Specifications
[List of capabilities and requirement changes]
```

6. Output the generated description for the user to copy into GitHub PR creation.

**Reference**
- Use `openspec show <id> --json --deltas-only` to inspect spec deltas if needed.
- The PR should be created from branch `spec/<id>` targeting `master`.
<!-- OPENSPEC:END -->
