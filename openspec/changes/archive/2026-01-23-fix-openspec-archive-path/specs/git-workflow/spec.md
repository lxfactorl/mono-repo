## ADDED Requirements
### Requirement: Standardized Archive Storage
The system SHALL store archived changes in a centralized directory following the pattern `openspec/changes/archive/YYYY-MM-DD-[change-id]`.

#### Scenario: Successful archival move
- **WHEN** a change with ID `<id>` is archived on date `YYYY-MM-DD`
- **THEN** the workflow SHALL move the change directory from `openspec/changes/<id>/` to `openspec/changes/archive/YYYY-MM-DD-<id>/`
- **AND** the workflow SHALL NOT create top-level `openspec/archived/` directories
