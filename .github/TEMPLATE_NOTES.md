# Template Repository Notes

This directory contains GitHub-specific template configuration and notes.

## Template Repository Setup

This repository is configured as a **Private GitHub Template Repository**. This means:

1. Only authorized collaborators can use the template
2. Team members can click **"Use this template"** to create new projects
3. The new repository will be a complete copy, not a fork
4. Setup happens automatically via GitHub Actions workflow
5. MyBuildingBlock source code is NOT included (only pre-built package)

## Access Control

- **Private Repository:** Only authorized users can access
- **Template Feature:** Enabled for controlled distribution
- **Source Protection:** MyBuildingBlock submodule is excluded from template exports
- **Package Only:** New repositories get pre-built package, not source code

See [ACCESS_CONTROL_GUIDE.md](../ACCESS_CONTROL_GUIDE.md) for access control setup.

## How to Enable Template Mode

1. Repository Settings → General → Template repository → Enable
2. Repository Settings → General → Change visibility to Private (if not already)
3. Repository Settings → Collaborators and teams → Add authorized users

## Template Workflow

1. Authorized user clicks "Use this template" on GitHub
2. Creates new repository with their project name
3. GitHub Actions workflow automatically:
   - Renames all project files and folders
   - Updates namespaces and configuration
   - Copies pre-built MyBuildingBlock package
   - Removes submodule references
   - Commits all changes
4. User clones the new repository: `git clone <repo-url>`
5. User runs `dotnet restore` and `dotnet build`
6. Project is ready for development!

**Note:** No submodules needed! The package is included in `bin/release/`.

See [TEMPLATE_SETUP.md](../TEMPLATE_SETUP.md) for detailed instructions.

