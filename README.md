# Mono-Repo

[![CI Status](https://github.com/lxfactorl/mono-repo/actions/workflows/notification-service-ci.yml/badge.svg)](https://github.com/lxfactorl/mono-repo/actions/workflows/notification-service-ci.yml)

A monorepo for backend services and client-side applications, designed for deployment to [Railway](https://railway.app).

## Repository Structure

```
/
├── src/
│   ├── backend/              # .NET Core services
│   │   └── <service-name>/
│   │       ├── <Service>.Api/
│   │       ├── <Service>.Application/
│   │       ├── <Service>.Domain/
│   │       ├── <Service>.Infrastructure/
│   │       └── <Service>.Tests/
│   │
│   └── client/               # Client applications
│       └── <client-name>/
│
└── openspec/                 # Centralized documentation
    ├── project.md            # Project conventions
    ├── specs/                # Capability specifications
    └── changes/              # Proposed changes
```

## Tech Stack

### Backend
- **.NET Core** (latest LTS)
- **C#** with nullable reference types
- .NET Generic Host pattern (DI, configuration, logging)
- Serilog for structured logging

### Client
- Location-aware applications (stack per project)

### Infrastructure
- **Railway** - container-based deployment
- **GitHub** - source control (private)

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (latest LTS)
- Git

## Getting Started

```bash
# Clone the repository
git clone https://github.com/lxfactorl/mono-repo.git
cd mono-repo

# Build all projects
dotnet build

# Run tests
dotnet test
```

## Development Workflow

### Branching
- `master` - protected, requires PR reviews
- `feature/<description>` - feature development
- `spec/<change-id>` - OpenSpec implementation

### Commits
Use [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` - new features
- `fix:` - bug fixes
- `chore:` - maintenance tasks
- `docs:` - documentation updates

### Code Quality
- EditorConfig enforces consistent formatting
- Roslyn analyzers run as errors in CI
- Minimum 80% test coverage

## Documentation

Project conventions and specifications are maintained in the [`openspec/`](./openspec/) directory using the OpenSpec workflow.
