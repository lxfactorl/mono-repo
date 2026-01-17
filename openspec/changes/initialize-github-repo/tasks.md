# Tasks: Initialize GitHub Private Repository

## 1. Local Git Setup
- [ ] 1.1 Configure local Git user name and email (if not already configured)
- [ ] 1.2 Initialize Git repository with `git init`
- [ ] 1.3 Create `.gitignore` with appropriate patterns for .NET and monorepo

## 2. GitHub Repository Setup
- [ ] 2.1 Create private repository `mono-repo` on GitHub via browser
- [ ] 2.2 Add remote origin pointing to the new repository
- [ ] 2.3 Create initial commit with current files
- [ ] 2.4 Push to master branch

## 3. Configure Master Branch Protection (GitHub UI)
- [ ] 3.1 Navigate to repository Settings → Branches
- [ ] 3.2 Add branch protection rule for `master`
- [ ] 3.3 Enable "Require a pull request before merging"
- [ ] 3.4 Disable "Allow force pushes" and "Allow deletions"
- [ ] 3.5 Save branch protection rule

## 4. OpenSpec Workflow Extensions
- [ ] 4.1 Update `openspec-proposal.md` to check for active unarchived specs before creating new
- [ ] 4.2 Update `openspec-apply.md` to create spec branch `spec/<change-id>` before implementation
- [ ] 4.3 Update `openspec-archive.md` to verify branch is merged and pushed before archiving
- [ ] 4.4 Add new workflow `openspec-pr.md` to generate PR description from spec content:
  - Collect title, "Why", "What Changes" from `proposal.md`
  - Include task completion status from `tasks.md`
  - List affected capabilities from spec deltas

## 5. Verification
- [ ] 5.1 Test `git status` works locally
- [ ] 5.2 Test `git remote -v` shows correct origin
- [ ] 5.3 Verify master branch protection prevents direct push
- [ ] 5.4 Verify workflow changes with `openspec validate`
