# Template Script Testing Guide

This document provides guidance for testing the template setup script.

## Manual Testing Instructions

Since the setup script performs destructive operations (renames folders and files), it should be tested carefully.

### Recommended Testing Approach

1. **Create a test copy of the repository:**
   ```bash
   # Create a copy of the entire repository
   cp -r GCSS_Survy GCSS_Survy_Backup
   # Or on Windows
   xcopy GCSS_Survy GCSS_Survy_Backup /E /I
   ```

2. **Run the script with a test project name:**
   ```powershell
   cd GCSS_Survy
   .\setup-project.ps1 -ProjectName "TestProject"
   ```

3. **Verify the changes:**
   - Check that the folder `GCSS_Survy/` was renamed to `TestProject/`
   - Check that `GCSS_Survy.sln` was renamed to `TestProject.sln`
   - Check that `TestProject/TestProject.csproj` exists (renamed from GCSS_Survy.csproj)
   - Verify namespaces in C# files are updated
   - Verify configuration values in appsettings.json are updated
   - Verify API routes in Constants.cs are updated

4. **Test the build:**
   ```bash
   cd TestProject
   dotnet restore
   dotnet build
   ```

5. **Check for any missed replacements:**
   ```bash
   # Search for any remaining references to GCSS_Survy
   grep -r "GCSS_Survy" TestProject/
   # Search for any remaining references to GCSS_Survy (in code/config)
   grep -r "GCSS_Survy" TestProject/ --exclude-dir=MyBuildingBlock
   ```

### What to Verify

✅ **Folder and File Renames:**
- Solution file renamed
- Project folder renamed
- Project file renamed

✅ **Code Changes:**
- All namespaces updated
- All using statements updated
- All class names in namespaces updated

✅ **Configuration Changes:**
- ApplicationName updated
- Database names updated
- JWT Issuer/Audience updated
- API base URL updated
- Swagger title updated
- Module name updated

✅ **Documentation:**
- README.md updated (if script updates it)
- Other markdown files updated

✅ **Docker:**
- Dockerfile references updated

✅ **Build Verification:**
- Project restores successfully
- Project builds without errors
- No missing references

## Automated Testing (Future Enhancement)

For automated testing in CI/CD, consider:

1. Creating a test script that:
   - Runs the setup script in a temporary directory
   - Validates file renames
   - Validates content replacements
   - Attempts to build the project
   - Cleans up afterwards

2. Using a containerized environment to isolate tests

## Known Limitations

- The script doesn't handle binary files (shouldn't be needed)
- The script doesn't update Git history (by design - creates new repo)
- The script doesn't update MyBuildingBlock submodule (by design - it's a separate repo)

## Reporting Issues

If you find issues with the setup script:

1. Document the steps to reproduce
2. Note the expected vs actual behavior
3. Check the script output for error messages
4. Report with the project name used for testing

