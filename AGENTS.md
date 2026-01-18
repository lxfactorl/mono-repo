<!-- OPENSPEC:START -->
# OpenSpec Instructions

These instructions are for AI assistants working in this project.

Always open `@/openspec/AGENTS.md` when the request:
- Mentions planning or proposals (words like proposal, spec, change, plan)
- Introduces new capabilities, breaking changes, architecture shifts, or big performance/security work
- Sounds ambiguous and you need the authoritative spec before coding

Use `@/openspec/AGENTS.md` to learn:
- How to create and apply change proposals
- Spec format and conventions
- Project structure and guidelines

## Git Workflow

> **CRITICAL**: Never push directly to `master`. All changes must go through a spec branch (`spec/<change-id>`) and pull request.

## Local CI (Mandatory)

> **RULE**: Before creating or updating a PR, you MUST run the local CI script to get fast feedback. This mirrors GitHub CI exactly.

```powershell
./ci.ps1 -ServicePath "src/backend/<service-name>" -CoverageThreshold 80
```

The local CI validates:
- Format check (`dotnet format --verify-no-changes`)
- Security scan (vulnerable packages)
- Build with zero warnings
- Tests with coverage threshold

**Do NOT push until local CI passes.** This ensures short feedback loops and avoids waiting for GitHub CI.

Keep this managed block so 'openspec update' can refresh the instructions.

<!-- OPENSPEC:END -->