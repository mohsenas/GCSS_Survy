# Employee Service - Contributor Guide

This guide will walk you through creating a complete service module following the standard pattern used in this project. Use this as a template for creating new services.

## Table of Contents

1. [Service Structure](#service-structure)
2. [Step 1: Create the Model](#step-1-create-the-model)
3. [Step 2: Create EFConfig](#step-2-create-efconfig)
4. [Step 3: Create DTOs](#step-3-create-dtos)
5. [Step 4: Create Service](#step-4-create-service)
6. [Step 5: Create Controller](#step-5-create-controller)
7. [Adding Custom Actions](#adding-custom-actions)
8. [Adding Custom Service Methods](#adding-custom-service-methods)
9. [Permission Attributes Guide](#permission-attributes-guide)

---

## Service Structure

Your service should follow this folder structure:

```
Emp_SimpleInfo/
├── Controllers/
│   └── EmployeeController.cs
├── Dtos/
│   ├── AddEmpDto.cs
│   ├── UpdateEmpDto.cs
│   └── EmpResult.cs
├── EFConfig/
│   └── EmpEfConfig.cs
├── Models/
│   └── Emp_Simple.cs
└── Services/
    └── Emp_SimpleService.cs
```

---

## Step 1: Create the Model

The model represents your database entity. It must inherit from `ParentEntity<TId>` where `TId` is your ID type (typically `int`, `long`, or `Guid`).

### Required Attributes

#### `[DocCode]` Attribute
- **Purpose**: Identifies the entity for document code generation
- **Usage**: Place on the class
- **Example**: `[DocCode(nameof(Emp_Simples))]`

#### `[Lookup]` Attribute
- **Purpose**: Specifies which properties should appear in lookup dropdowns
- **Usage**: Place on the class, specify property names as parameters
- **Example**: `[Lookup(nameof(EmpName), nameof(Job))]`
- **Note**: The `Id` property is automatically included in lookups

#### `[Search]` Attribute
- **Purpose**: Marks properties as searchable with priority order
- **Usage**: Place on individual properties with a priority number (1 = highest priority)
- **Example**: `[Search(1)]` on primary search field, `[Search(2)]` on secondary field

#### `IHasCustomFields` Interface
- **Purpose**: Enables dynamic custom fields storage
- **Usage**: Implement the interface and add `Dictionary<string, string> CustomFields` property

### Complete Model Example

```csharp
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Ef;
using MyBuildingBlock.EfConfig;
using System.ComponentModel.DataAnnotations;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Models
{
    [DocCode(nameof(Emp_Simples))]
    [Lookup(nameof(EmpName), nameof(Job))]

    public record Emp_Simples : ParentEntity<int>, IHasCustomFields
    {
        [Search(1)]
        [MaxLength(100)]
        public string EmpName { get; set; }

        [Search(2)]
        [MaxLength(100)]
        public string Job { get; set; }

        public DateTime JobDate { get; set; }
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        public bool IsEnabled { get; set; } = true;

        public Dictionary<string, string> CustomFields { get; set; }
    }
}
```

### Key Points:

- Use `record` type for immutable value semantics
- Inherit from `ParentEntity<TId>` where TId matches your ID type
- Use `[Search(priority)]` for searchable fields (lower number = higher priority)
- Use `[MaxLength]` for string validation
- Use nullable types (`string?`) for optional fields
- Implement `IHasCustomFields` if you need dynamic fields

---

## Step 2: Create EFConfig

EFConfig handles Entity Framework configuration for your model. It inherits from `CustomBaseEfConfig<TEntity>`.

### Complete EFConfig Example

```csharp
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBuildingBlock.Ef.EfConfig;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;

namespace GCSS_Survy.Services.Emp_SimpleInfo.EFConfig
{
    public sealed class EmpEfConfig : CustomBaseEfConfig<Emp_Simples>
    {
        public override void Configure(EntityTypeBuilder<Emp_Simples> builder)
        {
            base.Configure(builder);
            
            // Add custom configurations here if needed
            // Example: builder.Property(e => e.EmpName).IsRequired();
            // Example: builder.HasIndex(e => e.Email).IsUnique();
        }
    }
}
```

### Key Points:

- Inherit from `CustomBaseEfConfig<TEntity>` where TEntity is your model
- Always call `base.Configure(builder)` first
- Add custom configurations after the base call
- The base configuration automatically handles:
  - Primary key setup
  - Audit fields (UserId, BranchId, SessionId, AddTime, etc.)
  - CustomFields conversion (if implementing `IHasCustomFields`)

### Folder and Namespace:

- **Folder**: `EFConfig` (capital EF, capital Config)
- **Namespace**: `YourNamespace.Services.YourService.EFConfig`

---

## Step 3: Create DTOs

DTOs (Data Transfer Objects) are used for API communication. You need three DTOs:

### 3.1 AddDto (For Creating New Records)

```csharp
using MyBuildingBlock.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Dtos
{
    public class AddEmpDto : BaseAddRequestDto<int>
    {
        public string EmpName { get; set; }

        public string Job { get; set; }

        public DateTime JobDate { get; set; }
        
        public string? Email { get; set; }

        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
```

**Key Points:**
- Inherit from `BaseAddRequestDto<TId>` where TId matches your model ID type
- Include all properties that should be set during creation
- Use nullable types for optional fields
- Do NOT include `Id` - it's handled by the base class

### 3.2 UpdateDto (For Updating Existing Records)

```csharp
using MyBuildingBlock.Abstracts;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Dtos
{
    public class UpdateEmpDto : BaseUpdateRequestDto<int>
    {
        public string? Job { get; set; }

        public DateTime? JobDate { get; set; }
        
        public string? Email { get; set; }
        
        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
```

**Key Points:**
- Inherit from `BaseUpdateRequestDto<TId>`
- Include `Id` property from base class (automatically included)
- Make all properties nullable - only provided fields will be updated
- Omit properties that shouldn't be updated (e.g., `EmpName` in this example)

### 3.3 ResponseDto (For Returning Data)

```csharp
using MyBuildingBlock.Abstracts;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Dtos
{
    public sealed record EmpResult : BaseResult<int>
    {
        public string EmpName { get; set; }

        public string Job { get; set; }

        public DateTime JobDate { get; set; }
        
        public string? Email { get; set; }
        
        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
```

**Key Points:**
- Use `record` type for immutability
- Inherit from `BaseResult<TId>`
- Include `Id` property from base class (automatically included)
- Include all properties that should be returned to the client
- Use the same property types as your model

---

## Step 4: Create Service

The service handles business logic and data operations. It inherits from `RESTfulServiceBase`.

### Complete Service Example

```csharp
using MapsterMapper;
using MyBuildingBlock.Configurations;
using MyBuildingBlock.Data;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Localization;
using MyBuildingBlock.Lookup;
using GCSS_Survy.Data;
using GCSS_Survy.Services.Emp_SimpleInfo.Dtos;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Services
{
    public class Emp_SimpleService : RESTfulServiceBase<Emp_Simples, AddEmpDto, UpdateEmpDto, EmpResult, int>
    {
        public Emp_SimpleService(
            ApplicationDbContext context,
            IMapper mapper,
            ILookupMapper lookupMapper,
            ILogger<Emp_SimpleService> logger,
            ILocalizationService localizationService)
            : base(context, mapper, lookupMapper, logger, localizationService)
        {
        }
    }
}
```

### Generic Parameters Explained:

`RESTfulServiceBase<TEntity, TAddDto, TUpdateDto, TResponseDto, TId>`

- **TEntity**: Your model type (`Emp_Simples`)
- **TAddDto**: Your Add DTO (`AddEmpDto`)
- **TUpdateDto**: Your Update DTO (`UpdateEmpDto`)
- **TResponseDto**: Your Response DTO (`EmpResult`)
- **TId**: Your ID type (`int`)

### Constructor Dependencies:

- `ApplicationDbContext`: Your database context (or custom DbContext)
- `IMapper`: Mapster mapper for object mapping
- `ILookupMapper`: Mapper for lookup operations
- `ILogger<YourService>`: Logger instance
- `ILocalizationService`: Service for localization

### Built-in Methods Available:

The base class provides these standard CRUD operations:

- `AddAsync(AddDto request, UserContext userContext)` - Create new record
- `UpdateAsync(UpdateDto request, UserContext userContext)` - Update existing record
- `DeleteAsync(TId id, UserContext userContext)` - Delete record
- `GetByIdAsync(TId id, UserContext userContext)` - Get single record
- `GetAllAsync(UserContext userContext, ...)` - Get all records
- `GetPageAsync(PagingRequest request, UserContext userContext, ...)` - Get paginated records

---

## Step 5: Create Controller

The controller handles HTTP requests and routes them to the service.

### Complete Controller Example

```csharp
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBuildingBlock.Abstracts;
using MyBuildingBlock.Attributes;
using MyBuildingBlock.Exceptions;
using MyBuildingBlock.Infrastructure;
using MyBuildingBlock.Lookup;
using MyBuildingBlock.PermissionManagement.Attributes;
using MyBuildingBlock.PermissionManagement.Dtos;
using GCSS_Survy.Data;
using GCSS_Survy.Services.Emp_SimpleInfo.Dtos;
using GCSS_Survy.Services.Emp_SimpleInfo.Models;
using GCSS_Survy.Services.Emp_SimpleInfo.Services;

namespace GCSS_Survy.Services.Emp_SimpleInfo.Controllers
{
    [Route($"{BaseConstants.BASE_URL}/[Controller]/v1")]
    [AdminBranchOnly]
    [BasePermission("الموظفين", PermissionConstants.TREE_NODE_ICON, PermissionConstants.ROOT_CODE, "Employee")]
    public class EmployeeController : ControllerBase<
        Emp_SimpleService,
        AddEmpDto,
        UpdateEmpDto,
        EmpResult,
        int,
        ApplicationDbContext>
    {
        public EmployeeController(
            ApplicationDbContext context,
            ILogger<EmployeeController> logger,
            IMapper mapper,
            IServiceProvider service,
            ILookupMapper lookupMapper)
            : base(context, logger, mapper, service, lookupMapper)
        {
        }
    }
}
```

### Controller Base Generic Parameters:

`ControllerBase<TService, TAddDto, TUpdateDto, TResponseDto, TId, TDbContext>`

- **TService**: Your service class (`Emp_SimpleService`)
- **TAddDto**: Your Add DTO (`AddEmpDto`)
- **TUpdateDto**: Your Update DTO (`UpdateEmpDto`)
- **TResponseDto**: Your Response DTO (`EmpResult`)
- **TId**: Your ID type (`int`)
- **TDbContext**: Your DbContext type (`ApplicationDbContext`)

### Controller Attributes:

#### `[Route]`
- **Usage**: `[Route($"{BaseConstants.BASE_URL}/[Controller]/v1")]`
- **Purpose**: Defines the API route
- **Note**: `[Controller]` is automatically replaced with the controller name (without "Controller")

#### `[AdminBranchOnly]`
- **Purpose**: Restricts access to admin branch users only (BranchId = 1)
- **Usage**: Place on the controller class

#### `[BasePermission]`
- **Purpose**: Defines the permission path for role-based access control
- **Usage**: `[BasePermission("Label", Icon, RootCode, "PermissionCode")]`
- **Parameters**:
  - `"Label"`: Display name (e.g., "الموظفين" for Arabic)
  - `Icon`: Icon constant from `PermissionConstants` (e.g., `TREE_NODE_ICON`)
  - `RootCode`: Root permission code (e.g., `ROOT_CODE`)
  - `"PermissionCode"`: Unique permission identifier (e.g., "Employee")

### Built-in Endpoints:

The base controller automatically provides these endpoints:

- `GET /api/Employee/{id}` - Get by ID
- `POST /api/Employee/page` - Get paginated list
- `POST /api/Employee/all` - Get all records
- `POST /api/Employee` - Create new record
- `PUT /api/Employee` - Update existing record
- `DELETE /api/Employee/{id}` - Delete record
- `POST /api/Employee/Lookup` - Get lookup data
- `POST /api/Employee/Lookup/All` - Get all lookup data
- `POST /api/Employee/Lookup/Page` - Get paginated lookup data

---

## Adding Custom Actions

You can add custom action methods to your controller for specific business logic.

### Example: Activate/Deactivate Employee

```csharp
[HttpPut("ActivateEmp/{id}&{isActive}")]
[ActionPermission("تفعيل/إلغاء تفعيل موظف", PermissionConstants.BLOCK_ICON)]
public async Task<IActionResult> ActivateEmp(int id, bool isActive)
{
    var emp = await _context.Set<Emp_Simples>()
        .Where(f => f.ID == id)
        .FirstOrDefaultAsync();
        
    if (emp is null)
        throw new CustomNotFoundException("Employee not found");

    emp.IsEnabled = isActive;
    await _context.SaveChangesAsync();
    var result = _mapper.Map<EmpResult>(emp);
    return Ok(result);
}
```

### Key Points:

- Use standard HTTP verbs: `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- Define route explicitly: `[HttpPut("ActivateEmp/{id}&{isActive}")]`
- Add `[ActionPermission]` attribute for permission control (see [Permission Attributes](#permission-attributes-guide))
- Access `_context` for database operations
- Access `_mapper` for object mapping
- Throw `CustomNotFoundException` for not found errors
- Return `IActionResult` (use `Ok()`, `NotFound()`, etc.)

### Available Controller Properties:

- `_context`: Your DbContext instance
- `_mapper`: Mapster mapper instance
- `_logger`: Logger instance
- `_sp`: Service provider for dependency resolution
- `GetCustomService()`: Gets your service instance
- `GetUserContext()`: Gets current user context from JWT token

---

## Adding Custom Service Methods

You can override base service methods or add new methods for custom business logic.

### Example: Override AddAsync

```csharp
public override async Task<ReturnResult<EmpResult>> AddAsync(
    AddEmpDto request,
    UserContext userContext)
{
    // Custom validation
    if (string.IsNullOrWhiteSpace(request.EmpName))
        throw new CustomValidationException("Employee name is required");

    // Check if email already exists
    var existingEmp = await _context.Set<Emp_Simples>()
        .Where(e => e.Email == request.Email)
        .FirstOrDefaultAsync();
        
    if (existingEmp != null)
        throw new CustomValidationException("Employee with this email already exists");

    // Call base implementation
    return await base.AddAsync(request, userContext);
}
```

### Example: Add Custom Method

```csharp
public async Task<ReturnResult<List<EmpResult>>> GetActiveEmployeesAsync(
    UserContext userContext)
{
    var employees = await _context.Set<Emp_Simples>()
        .Where(e => e.IsEnabled == true)
        .ToListAsync();

    var result = new ReturnResult<List<EmpResult>>
    {
        Code = 200,
        Data = employees.Select(e => _mapper.Map<EmpResult>(e)).ToList()
    };

    return result;
}
```

### Key Points:

- Override base methods to add custom logic before/after base implementation
- Add new methods for specific business requirements
- Always return `ReturnResult<T>` for consistency
- Use `_context` for database access
- Use `_mapper` for object mapping
- Validate inputs and throw `CustomValidationException` for invalid data
- Use `CustomNotFoundException` when records don't exist

### Calling Custom Service Methods from Controller:

```csharp
[HttpGet("Active")]
[ActionPermission("Get Active Employees", PermissionConstants.LIST_ICON)]
public async Task<IActionResult> GetActiveEmployees()
{
    var service = GetCustomService();
    var userContext = GetUserContext();
    var result = await service.GetActiveEmployeesAsync(userContext);
    return ResultJson(result, null, null, result.Code);
}
```

---

## Permission Attributes Guide

### `[AdminOnly]` Attribute

Restricts access to system administrators only (users with RuleID = 1).

**Usage:**
```csharp
[AdminOnly]  // All CRUD operations require admin
[AdminOnly(enumActionType.All)]  // All actions require admin
[AdminOnly(enumActionType.CRUD)]  // Only CRUD operations require admin (default)
```

**Placement:** Controller or Action level

**When to use:**
- System-level operations (e.g., user management, system settings)
- Critical operations that only admins should perform

### `[AdminBranchOnly]` Attribute

Restricts access to admin branch users only (users with BranchId = 1).

**Usage:**
```csharp
[AdminBranchOnly]  // All CRUD operations require admin branch
[AdminBranchOnly(enumActionType.All)]  // All actions require admin branch
[AdminBranchOnly(enumActionType.CRUD)]  // Only CRUD operations require admin branch (default)
```

**Placement:** Controller or Action level

**When to use:**
- Operations that should only be performed from the main/admin branch
- Centralized management operations

### `[BasePermission]` Attribute

Defines the permission path for role-based access control. All actions under this controller inherit this permission path.

**Usage:**
```csharp
[BasePermission("الموظفين", PermissionConstants.TREE_NODE_ICON, PermissionConstants.ROOT_CODE, "Employee")]
```

**Parameters:**
1. **Label** (string): Display name for the permission (supports localization)
2. **Icon** (string): Icon constant from `PermissionConstants` (e.g., `TREE_NODE_ICON`, `BLOCK_ICON`, `LIST_ICON`)
3. **RootCode** (string): Root permission code (typically `PermissionConstants.ROOT_CODE`)
4. **PermissionCode** (string): Unique identifier for this permission (e.g., "Employee")

**Placement:** Controller level only

**Permission Path Format:**
The permission path will be: `{RootCode}.{PermissionCode}.{ActionName}`

For example, with `ROOT_CODE = "ROOT"` and `PermissionCode = "Employee"`:
- `ROOT.Employee.Add`
- `ROOT.Employee.Update`
- `ROOT.Employee.Delete`
- `ROOT.Employee.ActivateEmp` (for custom actions)

### `[ActionPermission]` Attribute

Defines permission for a specific action method. This extends the base permission path.

**Usage:**
```csharp
[ActionPermission("تفعيل/إلغاء تفعيل موظف", PermissionConstants.BLOCK_ICON)]
[ActionPermission("Label", "icon")]  // With action name auto-derived from method name
[ActionPermission("CustomAction", "Label", "icon")]  // With custom action name
[ActionPermission(ignore: true)]  // Ignore permission check for this action
```

**Parameters:**
1. **Action** (string, optional): Custom action name (if not provided, derived from method name)
2. **Label** (string): Display name for the action
3. **Icon** (string): Icon constant from `PermissionConstants`

**Placement:** Action method level

**When to use:**
- For custom action methods that need specific permissions
- To override default permission behavior for standard CRUD operations
- To ignore permission checks for public/internal endpoints

### `PermissionConstants` Icons:

Common icon constants available:
- `TREE_NODE_ICON`: For tree/list views
- `BLOCK_ICON`: For enable/disable actions
- `LIST_ICON`: For listing actions
- `ADD_ICON`: For create actions
- `EDIT_ICON`: For update actions
- `DELETE_ICON`: For delete actions

### Permission Checking Flow:

1. **Admin Check**: If user has RoleId = AdminRoleId, all permissions are granted
2. **AdminBranch Check**: `[AdminBranchOnly]` checks if BranchId = 1
3. **AdminOnly Check**: `[AdminOnly]` checks if RuleID = 1
4. **Permission Check**: `[ActionPermission]` checks if user's role has the permission path

### Examples:

**Controller with Admin Branch Only:**
```csharp
[Route($"{BaseConstants.BASE_URL}/[Controller]/v1")]
[AdminBranchOnly]  // Only admin branch can access
[BasePermission("الموظفين", PermissionConstants.TREE_NODE_ICON, PermissionConstants.ROOT_CODE, "Employee")]
public class EmployeeController : ControllerBase<...>
{
    // All actions require admin branch access
}
```

**Public Action (No Permission Check):**
```csharp
[HttpGet("PublicData")]
[ActionPermission(ignore: true)]  // Skip permission check
public IActionResult GetPublicData()
{
    // This endpoint is accessible without permission check
}
```

**Custom Action with Specific Permission:**
```csharp
[HttpPut("ActivateEmp/{id}&{isActive}")]
[ActionPermission("تفعيل/إلغاء تفعيل موظف", PermissionConstants.BLOCK_ICON)]
public async Task<IActionResult> ActivateEmp(int id, bool isActive)
{
    // Permission path: ROOT.Employee.ActivateEmp
    // User's role must have this permission (unless admin)
}
```

---

## Best Practices

### 1. Model Design
- Always use `record` for models (immutable value semantics)
- Use meaningful property names
- Add appropriate validation attributes (`[MaxLength]`, `[Required]`, etc.)
- Use nullable types for optional fields
- Implement `IHasCustomFields` when dynamic fields are needed

### 2. DTO Design
- Keep DTOs separate from models
- Use sealed records for response DTOs
- Make UpdateDto properties nullable
- Don't expose internal properties in DTOs

### 3. Service Design
- Keep services focused on business logic
- Use the base class methods when possible
- Override methods only when custom logic is needed
- Always return `ReturnResult<T>` for consistency

### 4. Controller Design
- Keep controllers thin - delegate to services
- Use proper HTTP verbs
- Return appropriate status codes
- Handle exceptions properly (use custom exception types)

### 5. Permissions
- Always add appropriate permission attributes
- Use `[AdminBranchOnly]` for admin-only operations
- Use `[ActionPermission]` for custom actions
- Document permission requirements clearly

### 6. Error Handling
- Use `CustomNotFoundException` for missing records
- Use `CustomValidationException` for validation errors
- Use `CustomUnAuthorizedException` for authorization errors
- Provide clear, user-friendly error messages

---

## Complete Example Flow

### 1. Request Flow
```
Client → Controller → Service → Database
Client ← Controller ← Service ← Database
```

### 2. Create Employee Example

**Request:**
```http
POST /api/Employee/v1
Content-Type: application/json

{
  "empName": "John Doe",
  "job": "Developer",
  "jobDate": "2024-01-15T00:00:00",
  "email": "john.doe@example.com"
}
```

**Controller receives** → Maps to `AddEmpDto` → Calls `service.AddAsync()` → Service validates and saves → Returns `EmpResult`

**Response:**
```json
{
  "code": 200,
  "status": "Success",
  "data": {
    "id": 1,
    "empName": "John Doe",
    "job": "Developer",
    "jobDate": "2024-01-15T00:00:00",
    "email": "john.doe@example.com",
    "isEnabled": true
  }
}
```

---

## Troubleshooting

### Issue: Model not appearing in database
**Solution:** Ensure your `ApplicationDbContext` includes the entity in `OnModelCreating`, or use the automatic discovery mechanism that finds all `ParentEntity<T>` subclasses.

### Issue: Permission denied errors
**Solution:** 
- Check if user has the required role
- Verify `[BasePermission]` and `[ActionPermission]` attributes are set correctly
- Ensure permission path matches what's stored in database
- Check if `[AdminBranchOnly]` or `[AdminOnly]` is preventing access

### Issue: Mapping errors
**Solution:**
- Ensure property names match between Model and DTOs
- Check that property types are compatible
- Verify Mapster configuration is correct

### Issue: Custom fields not saving
**Solution:**
- Ensure model implements `IHasCustomFields`
- Verify `CustomFields` property is included in DTOs
- Check EFConfig includes CustomFields configuration (handled by base class)

---

## Additional Resources

- Review `WeatherForecastInfo` service for another example
- Check `TicketInfo` service in MyBuildingBlock for advanced patterns
- Refer to MyBuildingBlock documentation for base class details

---

**Happy Coding! 🚀**

