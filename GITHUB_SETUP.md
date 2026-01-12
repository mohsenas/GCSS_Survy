# GitHub Repository Setup Guide

Your local Git repository has been initialized and your initial commit is ready. Follow these steps to create a GitHub repository and push your code.

## Step 1: Create GitHub Repository

### Option A: Using GitHub Web Interface

1. Go to [GitHub](https://github.com) and sign in
2. Click the **+** icon in the top right corner
3. Select **New repository**
4. Fill in the details:
   - **Repository name**: `GCSS_Survy` (or your preferred name)
   - **Description**: "A .NET 8.0 Web API project with authentication"
   - **Visibility**: Choose Public or Private
   - **DO NOT** initialize with README, .gitignore, or license (we already have these)
5. Click **Create repository**

### Option B: Using GitHub CLI (if installed)

```bash
gh repo create GCSS_Survy --public --source=. --remote=origin --push
```

## Step 2: Add Remote and Push

After creating the repository on GitHub, you'll see instructions. Use these commands:

```bash
# Add the remote repository (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/GCSS_Survy.git

# Rename the default branch to main (if needed)
git branch -M main

# Push your code to GitHub
git push -u origin main
```

### If you're using SSH:

```bash
git remote add origin git@github.com:YOUR_USERNAME/GCSS_Survy.git
git branch -M main
git push -u origin main
```

## Step 3: Verify

1. Go to your GitHub repository page
2. Verify all files are present
3. Check that the README.md displays correctly

## Important Notes

### MyBuildingBlock Package

This project uses MyBuildingBlock as a NuGet package (not a submodule). The pre-built package is included in the repository.

**For template users (new repositories created from template):**
- MyBuildingBlock is provided as a pre-built NuGet package
- Package location: `bin/release/MyBuildingBlock.1.0.0.nupkg`
- No submodule setup needed
- Source code is not accessible

**For template repository maintainers (development):**
- MyBuildingBlock exists as a submodule for development purposes
- Submodule is excluded from template exports via `.gitattributes`
- When using "Use this template", submodule is NOT included

**For cloning this template repository (developers only):**
```bash
# Clone with submodules (only for template repository development)
git clone --recurse-submodules https://github.com/mohsenas/GCSS_Survy.git

# Or if already cloned, initialize submodules
git submodule update --init --recursive
```

**Note:** Template users (those creating new repos from template) do NOT need submodules. They get the package automatically.

### Sensitive Information

Before pushing, ensure you haven't committed:
- ✅ Passwords or API keys (already in .gitignore)
- ✅ Connection strings with credentials (use User Secrets for development)
- ✅ Personal information

The `.gitignore` file already excludes:
- `appsettings.Development.json`
- `*.secrets.json`
- `Logs/` directory

## Next Steps

After pushing to GitHub:

1. **Set up branch protection** (optional but recommended)
2. **Add a license file** (if needed)
3. **Set up GitHub Actions** for CI/CD (optional)
4. **Add collaborators** (if working in a team)
5. **Create issues** for known bugs or features

## Troubleshooting

### Authentication Issues

If you get authentication errors:

```bash
# For HTTPS, use a Personal Access Token
# Generate one at: https://github.com/settings/tokens

# For SSH, set up SSH keys
# Guide: https://docs.github.com/en/authentication/connecting-to-github-with-ssh
```

### Push Rejected

If push is rejected:
```bash
# Pull first (if repository was initialized with files)
git pull origin main --allow-unrelated-histories

# Then push
git push -u origin main
```

## Quick Reference Commands

```bash
# Check status
git status

# View commit history
git log --oneline

# Add changes
git add .

# Commit changes
git commit -m "Your commit message"

# Push to GitHub
git push

# Pull latest changes
git pull
```

## Mark Repository as Template

To enable the **"Use this template"** button on GitHub:

1. Go to your repository on GitHub
2. Click on **Settings** (in the repository navigation bar)
3. Scroll down to the **Template repository** section
4. Check the box that says **"Template repository"**
5. Click **Save**

Once enabled:
- A **"Use this template"** button will appear on your repository's main page
- Team members can click this button to create new repositories from your template
- The new repository will be a copy of your template, not a fork
- **Automatic setup:** A GitHub Actions workflow will automatically rename all project files and references when the repository is first created
- Team members just need to wait for the workflow to complete, then clone and start working
- See [TEMPLATE_SETUP.md](TEMPLATE_SETUP.md) for complete instructions

**Note:** 
- Only repository owners and administrators can enable template mode
- The automatic setup workflow uses the repository name as the project name
- Team members don't need to run any setup scripts manually

