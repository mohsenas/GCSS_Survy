# MyBuildingBlock Package Build Guide

This guide explains how to create and update the MyBuildingBlock NuGet package that's included in the template repository.

## Overview

The MyBuildingBlock package is pre-built and stored in `GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg`. This package is committed to the template repository so that new repositories created from the template don't need to build it themselves.

## Prerequisites

- .NET 8.0 SDK installed
- Access to the MyBuildingBlock project (submodule or source code)
- Git access to commit the package to the template repository

## Steps to Create/Update the Package

### 1. Navigate to MyBuildingBlock Project

```bash
cd MyBuildingBlock/MyBuildingBlock
```

### 2. Build the Project (Release Configuration)

```bash
dotnet build -c Release
```

This compiles the project in Release mode. You'll see warnings, but the build should succeed.

### 3. Create the NuGet Package

```bash
dotnet pack -c Release -o ../../GCSS_Survy/bin/release -p:Version=1.0.0 --no-build
```

**Parameters explained:**
- `-c Release`: Use Release configuration
- `-o ../../GCSS_Survy/bin/release`: Output directory (relative to MyBuildingBlock/MyBuildingBlock)
- `-p:Version=1.0.0`: Set package version to 1.0.0
- `--no-build`: Skip build (since we already built in step 2)

### 4. Verify the Package

```bash
# From repository root
ls -lh GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg
```

You should see the package file (approximately 238 KB).

### 5. Add Package to Git

The `.gitignore` file has exceptions to track this specific package:

```bash
git add -f GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg
```

The `-f` flag forces git to add the file even though `bin/` and `*.nupkg` are normally ignored.

### 6. Commit and Push

```bash
git commit -m "Update MyBuildingBlock package to version 1.0.0"
git push
```

## Complete Command Sequence

From the repository root, you can run all steps at once:

```bash
# Build and package
cd MyBuildingBlock/MyBuildingBlock
dotnet build -c Release
dotnet pack -c Release -o ../../GCSS_Survy/bin/release -p:Version=1.0.0 --no-build
cd ../../

# Verify and add to git
git add -f GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg
git status  # Verify it's staged
```

## Updating the Package Version

If you need to change the version (e.g., to 1.0.1):

1. Update the version in the pack command:
   ```bash
   dotnet pack -c Release -o ../../GCSS_Survy/bin/release -p:Version=1.0.1 --no-build
   ```

2. Update the template project file:
   - Edit `templates/GCSS_Survy.template.csproj`
   - Change `<PackageReference Include="MyBuildingBlock" Version="1.0.0" />` to `Version="1.0.1"`

3. Update the workflow if needed (currently hardcoded to check for 1.0.0)

4. Remove the old package and add the new one:
   ```bash
   git rm GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg
   git add -f GCSS_Survy/bin/release/MyBuildingBlock.1.0.1.nupkg
   ```

## Troubleshooting

### Package Not Found Error

If the workflow fails with "Pre-built MyBuildingBlock package not found":
- Verify the package exists: `ls GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg`
- Check the file is committed: `git ls-files GCSS_Survy/bin/release/`
- Ensure `.gitignore` has the exception rules

### Package Not Tracked by Git

If `git add` doesn't work:
- Use `git add -f` to force add the file
- Verify `.gitignore` has the exception: `!**/bin/release/MyBuildingBlock.*.nupkg`
- Check git status: `git status GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg`

### Build Errors

If the build fails:
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Check for missing dependencies
- Review build output for specific errors

## Automation (Optional)

You could create a PowerShell script to automate this:

```powershell
# build-package.ps1
Set-Location MyBuildingBlock/MyBuildingBlock
dotnet build -c Release
dotnet pack -c Release -o ../../GCSS_Survy/bin/release -p:Version=1.0.0 --no-build
Set-Location ../../
git add -f GCSS_Survy/bin/release/MyBuildingBlock.1.0.0.nupkg
Write-Host "Package built and staged. Run 'git commit' to commit it."
```

## Notes

- The package is approximately 238 KB
- The version is currently hardcoded to 1.0.0 in multiple places
- The package must be committed to the template repository for the workflow to work
- When MyBuildingBlock is updated, rebuild and update the package in the template

