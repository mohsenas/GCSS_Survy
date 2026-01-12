# Template Repository Access Control Guide

This guide explains how to secure the template repository so that contributors can only use the template to create new repositories without accessing the MyBuildingBlock source code.

## Overview

The template repository should be:
- **Private** - Only authorized users can access it
- **Template-enabled** - Can be used to create new repositories
- **Source-protected** - MyBuildingBlock source code is not accessible to template users

## Step 1: Make Repository Private

### On GitHub:

1. Go to your repository on GitHub
2. Click **Settings** (in the repository navigation bar)
3. Scroll down to the **Danger Zone** section
4. Click **Change visibility**
5. Select **Make private**
6. Type the repository name to confirm
7. Click **I understand, change repository visibility**

**Important:** 
- Only repository owners/admins can change visibility
- This action is permanent (you can change it back, but it requires admin access)
- Private repositories require a paid GitHub plan (unless using GitHub Free for personal accounts)

## Step 2: Enable Template Repository Feature

Even when private, you can enable the template feature:

1. In repository **Settings** → **General**
2. Scroll to **Template repository** section
3. Check the box **"Template repository"**
4. Click **Update**

**Note:** Private template repositories can still be used by authorized users.

## Step 3: Add Collaborators (Control Access)

### Add Individual Users:

1. Go to **Settings** → **Collaborators and teams**
2. Click **Add people** or **Invite a collaborator**
3. Enter the GitHub username or email
4. Select permission level:
   - **Read** - Can use template, view code (recommended for template users)
   - **Triage** - Can manage issues and pull requests
   - **Write** - Can push code (for maintainers)
   - **Maintain** - Can manage repository settings (for admins)
   - **Admin** - Full access
5. Click **Add [username] to this repository**

### Add Teams (Organization):

1. Go to **Settings** → **Collaborators and teams**
2. Click **Add teams**
3. Select the team(s) that should have access
4. Set permission level
5. Click **Add teams to this repository**

## Step 4: Verify Submodule Exclusion

The template is configured to exclude the MyBuildingBlock submodule:

### .gitattributes Configuration

The `.gitattributes` file contains:
```
MyBuildingBlock/ export-ignore
.gitmodules export-ignore
```

This ensures:
- When someone uses "Use this template", the submodule is NOT included
- Only the pre-built package (`bin/release/MyBuildingBlock.1.0.0.nupkg`) is included
- Source code remains completely hidden

### Verification

To verify the submodule is excluded:

1. Create a test repository from the template (as a test user)
2. Check that `MyBuildingBlock/` directory does NOT exist
3. Verify `bin/release/MyBuildingBlock.1.0.0.nupkg` IS present
4. Confirm `.gitmodules` file is NOT present

## Step 5: Workflow Security

The GitHub Actions workflow is configured to:
- ✅ NOT checkout submodules
- ✅ Only use the pre-built package
- ✅ Remove any submodule references from new repos
- ✅ Work without access to MyBuildingBlock source

## Access Control Levels

### Repository Owner/Admin
- Full access to everything
- Can see MyBuildingBlock submodule (for development)
- Can modify template and workflow
- Can manage collaborators

### Template User (Read Access)
- Can use "Use this template" button
- Gets new repository with package only
- Cannot see MyBuildingBlock source code
- Cannot access submodule
- Cannot modify template repository

### Maintainer (Write Access)
- Same as Read, plus:
- Can push changes to template
- Can modify workflow
- Still cannot access submodule (unless explicitly granted)

## How Template Users Access the Template

### For Authorized Users:

1. User must be added as a collaborator (Read access minimum)
2. User goes to template repository
3. Clicks **"Use this template"** button
4. Creates new repository
5. Gets only the template files + package (no source code)

### What They Get:

✅ Template project structure
✅ Pre-built MyBuildingBlock package (`bin/release/MyBuildingBlock.1.0.0.nupkg`)
✅ NuGet.config configured for local package
✅ All project files and configuration
✅ GitHub Actions workflow (already configured)

❌ MyBuildingBlock source code
❌ `.gitmodules` file
❌ Submodule references
❌ Access to private MyBuildingBlock repository

## Security Checklist

- [ ] Repository is set to Private
- [ ] Template repository feature is enabled
- [ ] Only authorized users are added as collaborators
- [ ] `.gitattributes` excludes submodule from exports
- [ ] Pre-built package is committed to repository
- [ ] Workflow doesn't require submodule access
- [ ] Documentation updated to reflect private access

## Troubleshooting

### User Cannot See "Use this template" Button

**Cause:** User doesn't have access to private repository

**Solution:**
1. Add user as collaborator with Read access
2. User must accept invitation
3. Refresh repository page

### Submodule Appears in New Repository

**Cause:** `.gitattributes` not working or not committed

**Solution:**
1. Verify `.gitattributes` is committed to repository
2. Check that `export-ignore` rules are correct
3. Test template export manually: `git archive --format=zip HEAD -o test.zip`

### Package Not Found in New Repository

**Cause:** Package file not committed or `.gitignore` blocking it

**Solution:**
1. Verify package is committed: `git ls-files bin/release/MyBuildingBlock.1.0.0.nupkg`
2. Check `.gitignore` has exception for package file
3. Ensure package is in correct location

## Best Practices

1. **Regular Audits:** Periodically review collaborator list
2. **Minimal Access:** Grant Read access for template users (not Write)
3. **Documentation:** Keep access control process documented
4. **Package Updates:** When updating MyBuildingBlock, rebuild and commit new package
5. **Testing:** Test template export with a test user account

## Alternative: Organization-Level Template

If using GitHub Organizations:

1. Create template repository in organization
2. Set repository visibility to Private
3. Use organization teams for access control
4. Template is available to all team members automatically

## Summary

By following this guide:
- ✅ Template repository is private and secure
- ✅ Only authorized users can access it
- ✅ Source code is completely hidden from template users
- ✅ Users get working template with package only
- ✅ No way to access MyBuildingBlock source code

The template is now secure and ready for controlled distribution!

