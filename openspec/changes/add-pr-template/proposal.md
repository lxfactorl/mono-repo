# Proposal: Add GitHub Pull Request Template

## Linked Issue
Relates to #99

## Summary
Add a standard `.github/pull_request_template.md` to enforce consistency, linking of OpenSpec changes/Issues, and verification checklists for all PRs.

## Motivation
Currently, PRs lack a unified structure, leading to inconsistent descriptions and missed verification steps (e.g., local build checks, linting). A template will automate the reminder to provide this context.

## Proposed Changes
Create `.github/pull_request_template.md` with sections for:
- Summary & OpenSpec ID
- Linked Issue
- Type of Change (Bug, Feature, Chore)
- Verification Checklist

## Reference
Similar to standard GitHub community health files.
