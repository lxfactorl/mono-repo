# Change: Fix OpenSpec Archive Path Inconsistency

## Why
A single archived change (`add-ci-quality-gates`) was incorrectly placed in `openspec/archived/` instead of the standard `openspec/changes/archive/` directory. This creates inconsistency in the repository structure and potentially breaks tooling that expects a single location for completed changes.

## What Changes
- **MOVE** contents of `openspec/archived/add-ci-quality-gates` to `openspec/changes/archive/add-ci-quality-gates`.
- **REMOVE** the now-empty `openspec/archived/` directory.
- **RESTORE** consistency with the defined standard in `openspec/AGENTS.md`.

## Impact
- Affected specs: `git-workflow`
- Affected code: None (documentation/metadata only)

## Linked Issue
Relates to #104
