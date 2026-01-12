# Template Setup Guide

This repository is a **Private GitHub Template Repository** designed to be used as a starting point for new .NET 8.0 Web API projects.

**Access:** This template is private. You must be added as a collaborator to use it. Contact the repository owner for access.

## Quick Start

### 1. Create Repository from Template

**Prerequisites:**
- You must be added as a collaborator to this template repository
- You need Read access (minimum) to use the template

**Steps:**
1. Click the **"Use this template"** button on GitHub
2. Give your new repository a name (e.g., `MyNewProject`)
3. Choose Public or Private visibility
4. Click **"Create repository from template"**
5. **That's it!** The setup happens automatically via GitHub Actions

**Note:** If you don't see the "Use this template" button, you don't have access. Contact the repository owner to be added as a collaborator.

### 2. Automatic Setup

When you create a repository from this template, a GitHub Actions workflow automatically:

- ✅ Renames the project folder to match your repository name
- ✅ Renames solution and project files
- ✅ Updates all namespaces in C# files
- ✅ Updates configuration values (appsettings.json, Program.cs, Constants.cs)
- ✅ Updates API routes and JWT settings
- ✅ Updates database names in connection strings
- ✅ Copies pre-built MyBuildingBlock NuGet package to `bin/release/`
- ✅ Commits all changes automatically

**No manual setup required!** The workflow uses your repository name as the project name.

**Note:** MyBuildingBlock is included as a pre-built NuGet package in the template, so no additional authentication or setup is needed.

### 3. Clone Your New Repository

After the GitHub Action completes (usually within a minute):

```bash
# Clone your new repository (no submodules needed!)
git clone https://github.com/YOUR_USERNAME/YOUR_PROJECT_NAME.git
cd YOUR_PROJECT_NAME
```

**Note:** MyBuildingBlock is included as a NuGet package in the `bin/release/` folder. You don't need to clone submodules or set up external package feeds.

### Manual Setup (Alternative)

If you prefer to set up manually or if the automatic setup doesn't run, you can use the setup script:

**On Windows (PowerShell):**

```powershell
.\setup-project.ps1 -ProjectName "YourProjectName"
```

**On Linux/Mac (Bash with PowerShell):**

```bash
pwsh setup-project.ps1 -ProjectName "YourProjectName"
```

**Important:** 
- The project name should match your repository name for consistency
- Project name should only contain letters, numbers, underscores, and hyphens

### 4. What Gets Renamed Automatically

The GitHub Actions workflow (or manual script) automatically updates:

- ✅ **Namespaces**: `StarterKit_Test` → `YourProjectName`
- ✅ **Project folder**: `StarterKit_Test/` → `YourProjectName/`
- ✅ **Solution file**: `StarterKit_Test.sln` → `YourProjectName.sln`
- ✅ **Project file**: `StarterKit_Test.csproj` → `YourProjectName.csproj`
- ✅ **API routes**: `api/StarterKit` → `api/YourProjectName`
- ✅ **Application name**: `StarterKit-app` → `YourProjectName-app`
- ✅ **JWT settings**: `StarterKit.API` → `YourProjectName.API`
- ✅ **Database names**: `StarterKit` → `YourProjectName` (in connection strings)
- ✅ **Configuration values**: All references in `appsettings.json`, `Program.cs`, `Constants.cs`
- ✅ **Dockerfile**: Folder and assembly references

### 5. Post-Setup Steps

After the automatic setup completes (or after running the setup script manually):

1. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

2. **Verify the project builds:**
   ```bash
   dotnet build
   ```

3. **Update configuration files:**
   - Review and update `appsettings.json` with your specific settings
   - Update database connection strings if needed
   - Update JWT secret keys and settings

4. **Update documentation:**
   - Review and update `README.md` with your project-specific information
   - Remove or update `DEBUG_LOGIN.md` if not needed
   - Update `GITHUB_SETUP.md` with your repository information

5. **Initialize Git (if needed):**
   ```bash
   git add .
   git commit -m "Initial setup from template"
   git push
   ```

## Project Structure

After setup, your project will have this structure:

```
YourProjectName/
├── YourProjectName/
│   ├── bin/
│   │   └── release/
│   │       └── MyBuildingBlock.1.0.0.nupkg  # Local NuGet package
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── DataSeeder.cs
│   ├── Services/
│   │   ├── Security/
│   │   │   └── SecureAuthController.cs
│   │   └── UserInfo/
│   │       └── Features/
│   │           └── UserController.cs
│   ├── Migrations/
│   ├── Program.cs
│   ├── Constants.cs
│   ├── appsettings.json
│   ├── NuGet.config          # Configured for local package source
│   └── YourProjectName.csproj
├── setup-project.ps1         # Setup script
├── README.md
└── YourProjectName.sln
```

## Configuration

### Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnectionString": "Server=.;Database=YourProjectName;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### JWT Settings

Update JWT settings in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Issuer": "YourProjectName.API",
    "Audience": "YourProjectName.Client",
    "SecretKey": "YOUR_SECRET_KEY_HERE"
  }
}
```

### API Routes

The base API route is configured in `Constants.cs`:

```csharp
public const string BASE_URL = "api/YourProjectName";
```

This means your endpoints will be:
- Login: `POST /api/YourProjectName/SecureAuth/v1/login`
- Users: `GET /api/YourProjectName/User/v1`

## Troubleshooting

### Setup Script Errors

**Error: "Target project folder already exists"**
- The folder with your project name already exists
- Choose a different project name or remove the existing folder

**Error: "Template project folder not found"**
- Make sure you're running the script from the repository root
- The script expects to find the `StarterKit_Test` folder (before renaming)

**Error: "Project name can only contain..."**
- Use only letters, numbers, underscores, and hyphens
- Avoid special characters and spaces

### Build Errors After Setup

**Namespace errors:**
- Run the setup script again if some files weren't updated
- Check that all `.cs` files have the correct namespace

**Missing references:**
- Run `dotnet restore` to restore NuGet packages
- The MyBuildingBlock package is included in `bin/release/` folder and will be restored automatically
- If the package is missing, check that the GitHub Actions workflow completed successfully

**Database connection errors:**
- Verify SQL Server is running
- Update the connection string in `appsettings.json`
- Ensure the database exists or run migrations: `dotnet ef database update`

## Next Steps

1. Set up your development environment
2. Configure your database and connection strings
3. Update application settings and secrets
4. Review and customize the authentication/authorization setup
5. Add your project-specific features
6. Set up CI/CD pipelines
7. Configure deployment settings

## Need Help?

- Check the main [README.md](README.md) for project documentation
- Review [DEBUG_LOGIN.md](StarterKit_Test/DEBUG_LOGIN.md) for authentication troubleshooting
- MyBuildingBlock is included as a NuGet package - no separate setup needed

## Contributing Back

If you've made improvements to the template or found issues, consider contributing back to the template repository to help other team members!

