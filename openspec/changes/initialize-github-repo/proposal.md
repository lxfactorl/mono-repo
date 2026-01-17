# Change: Initialize GitHub Private Repository

## Why
The monorepo needs version control with a private GitHub repository to enable collaboration, code backup, and a structured workflow where each OpenSpec change is implemented on a dedicated branch before merging to master.

## What Changes
- Initialize local Git repository
- Configure local Git user (if needed)
- Create private `mono-repo` repository on GitHub
- Connect local repo to remote origin
- Create and protect `master` branch (prevent direct pushes)
- Extend OpenSpec workflows to enforce:
  - Only one active spec in work at a time
  - New spec cannot be opened if previous spec is not archived and fully pushed
  - Each spec implementation on its own branch: `spec/<change-id>`
  - Pull request flow from spec branch to master
  - Auto-generate PR description from current spec (proposal.md, tasks.md)

## Impact
- Affected specs: `git-workflow` (NEW)
- Affected code: `.agent/workflows/` (updates to OpenSpec workflows)
- Affected files: `.gitignore`, GitHub branch protection settings
