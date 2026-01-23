## 1. Preparation
- [x] 1.1 Create refactor branch `refactor/fix-openspec-archive-path`

## 2. Implementation
- [x] 2.1 Move `openspec/archived/add-ci-quality-gates` to `openspec/changes/archive/2026-01-17-add-ci-quality-gates`
- [x] 2.2 Remove empty `openspec/archived` directory

## 3. Verification
- [x] 3.1 Run `openspec validate --strict --no-interactive`
- [x] 3.2 Verify `openspec list` shows legacy data correctly (if applicable)
