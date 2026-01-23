## Context
The repository contains a legacy directory `openspec/archived/` which was used for a single archival early in the project. Since then, the standard has shifted to `openspec/changes/archive/` with date prefixes.

## Goals
- Restore repository structure consistency.
- Standardize the legacy archive entry to match current patterns.

## Decisions
- Rename `add-ci-quality-gates` to include the original archive date: `2026-01-17-add-ci-quality-gates`.
- Move the directory to `openspec/changes/archive/`.
- Delete the `openspec/archived/` folder.

## Risks / Trade-offs
- Risk: Moving files might confuse history.
- Mitigation: Git handles renames/moves well; history for the individual files will be preserved.
