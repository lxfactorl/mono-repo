# Proposal: add-issues-tracking

## Summary
The project currently manages its backlog and task tracking within the repository using OpenSpec `tasks.md` files and proposal documents. While this is excellent for local development and spec-driven work, it lacks centralized visibility and collective management features provided by GitHub Issues. This proposal aims to integrate GitHub Issues into the OpenSpec workflow to manage the project backlog effectively.

## Impact
- **Visibility**: Stakeholders can view the project status and backlog directly on GitHub without cloning the repository.
- **Traceability**: Changes and Pull Requests will be linked to specific GitHub Issues.
- **Collaboration**: Multiple agents and human reviewers can use labels and comments to discuss spec changes.

## Goals
- Establish a convention for mapping OpenSpec `Changes` to GitHub Issues.
- Configure GitHub repository labels and milestones for backlog management.
- Update `openspec/project.md` to document the integrated workflow.
