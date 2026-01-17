# Tasks: Initialize GitHub Private Repository

## 1. Local Git Setup
- [x] 1.1 Configure local Git user name and email (lxfactorl / fiodorov@gmail.com)
- [x] 1.2 Initialize Git repository with `git init`
- [x] 1.3 Create `.gitignore` with appropriate patterns for .NET and monorepo

## 2. GitHub Repository Setup
- [x] 2.1 Create private repository `mono-repo` on GitHub via browser
- [x] 2.2 Add remote origin pointing to the new repository
- [x] 2.3 Create initial commit with current files
- [x] 2.4 Push to master branch

## 3. Configure Master Branch Protection (GitHub UI)
- [ ] 3.1 Navigate to repository Settings → Branches
- [ ] 3.2 Add branch protection rule for `master`
- [ ] 3.3 Enable "Require a pull request before merging"
- [ ] 3.4 Disable "Allow force pushes" and "Allow deletions"
- [ ] 3.5 Save branch protection rule

## 4. OpenSpec Workflow Extensions
- [x] 4.1 Update `openspec-proposal.md` to check for active unarchived specs before creating new
- [x] 4.2 Update `openspec-apply.md` to create spec branch `spec/<change-id>` before implementation
- [x] 4.3 Update `openspec-archive.md` to verify branch is merged and pushed before archiving
- [x] 4.4 Add new workflow `openspec-pr.md` to generate PR description from spec content

## 5. Verification
- [x] 5.1 Test `git status` works locally
- [x] 5.2 Test `git remote -v` shows correct origin
- [ ] 5.3 Verify master branch protection prevents direct push (requires Section 3)
- [x] 5.4 Verify workflow changes with `openspec validate`
