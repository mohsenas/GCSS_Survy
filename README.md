# StarterKit Test

> **📋 This is a Private GitHub Template Repository**  
> Use this template to create new .NET 8.0 Web API projects. Setup happens **automatically** via GitHub Actions - just create a repository from this template and everything is renamed for you!  
> **[👉 See Template Setup Guide](TEMPLATE_SETUP.md) for detailed instructions.**  
> **🔒 Access:** This template is private. Contact the repository owner to be added as a collaborator.

A .NET 8.0 Web API project built with ASP.NET Core, Entity Framework Core, and the MyBuildingBlock framework.

## Features

- RESTful API with authentication and authorization
- Entity Framework Core with SQL Server
- JWT token-based authentication
- Rate limiting
- Request/response logging with Serilog
- Swagger/OpenAPI documentation

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code (optional)

## Getting Started

### Using This Template

**Prerequisites:** You must be added as a collaborator to this private template repository.

If you're creating a **new project from this template**:

1. Ensure you have access to this template repository (contact owner if needed)
2. Click **"Use this template"** on GitHub to create a new repository
3. Give your repository a name (this becomes your project name)
4. Wait for the GitHub Actions workflow to complete (automatic setup)
5. Clone your new repository: `git clone <your-new-repo-url>`
6. Follow the instructions in [TEMPLATE_SETUP.md](TEMPLATE_SETUP.md)

**Important:** 
- No submodules needed! MyBuildingBlock is included as a pre-built NuGet package
- The setup is fully automated - no manual script execution needed!
- Source code is not accessible - only the package is provided

### Setting Up the Template Repository Itself

If you're working on the template repository itself:

### 1. Clone the Repository

```bash
git clone --recurse-submodules https://github.com/mohsenas/StarterKit_Test.git
cd StarterKit_Test
```

### 2. Configure Database

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnectionString": "Server=.;Database=StarterKit;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Run Database Migrations

```bash
cd StarterKit_Test
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## API Endpoints

### Authentication

- `POST /api/StarterKit/SecureAuth/v1/login` - User login
- `POST /api/StarterKit/SecureAuth/v1/refresh` - Refresh access token
- `POST /api/StarterKit/SecureAuth/v1/logout` - User logout

### Health Check

- `GET /health/db` - Database health check
- `GET /v1` - Service status

## Default User

The application seeds a default admin user:
- Username: `admin`
- Password: (Check DataSeeder.cs for the hashed password)

## Project Structure

```
StarterKit_Test/
├── Data/
│   ├── ApplicationDbContext.cs    # Database context
│   └── DataSeeder.cs              # Database seeding
├── Services/
│   ├── Security/
│   │   └── SecureAuthController.cs
│   └── UserInfo/
│       └── Features/
│           └── UserController.cs
├── Migrations/                    # EF Core migrations
├── Program.cs                     # Application entry point
└── appsettings.json              # Configuration
```

## Configuration

Key configuration sections in `appsettings.json`:

- **JwtSettings**: JWT token configuration
- **SecuritySettings**: Security and password requirements
- **RateLimiting**: Rate limiting rules
- **Serilog**: Logging configuration

## Dependencies

- **MyBuildingBlock**: Custom framework library (NuGet package for template users, submodule for template developers)
- **Entity Framework Core**: ORM for database operations
- **Serilog**: Structured logging
- **Swashbuckle**: Swagger/OpenAPI documentation

### MyBuildingBlock Package/Submodule

**For Template Users (New Repositories):**
- MyBuildingBlock is provided as a pre-built NuGet package
- Package location: `bin/release/MyBuildingBlock.1.0.0.nupkg`
- No submodule setup needed - just run `dotnet restore`
- Source code is not accessible

**For Template Repository Developers:**
- MyBuildingBlock exists as a Git submodule for development
- When cloning the template repository itself:
  ```bash
  git clone --recurse-submodules https://github.com/mohsenas/StarterKit_Test.git
  git submodule update --init --recursive
  ```
- Submodule is excluded from template exports (via `.gitattributes`)
- Template users never see the submodule

## Template Usage

This repository is designed as a template with **automatic setup**. When team members create a new project:

1. They use the **"Use this template"** button on GitHub
2. Give the repository a name (used as the project name)
3. GitHub Actions automatically renames everything
4. All namespaces, configuration values, and file names are updated automatically

See [TEMPLATE_SETUP.md](TEMPLATE_SETUP.md) for complete setup instructions.

## Development

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Creating Migrations

```bash
dotnet ef migrations add MigrationName
```

## Troubleshooting

### Login Endpoint Hanging

If the login endpoint is not responding:

1. Check database connectivity: `GET /health/db`
2. Verify Swagger UI for correct request format
3. Check console logs for errors
4. Ensure SQL Server is running and accessible
5. Verify rate limiting hasn't been exceeded

See `DEBUG_LOGIN.md` for detailed debugging steps.

## License

[Your License Here]

## Contributing

[Your Contributing Guidelines Here]

