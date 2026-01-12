# How to Use a Local NuGet Package (.nupkg file)

This guide explains how to use a NuGet package from a local `.nupkg` file instead of downloading it from a package feed.

## Overview

When you have a `.nupkg` file (like `MyBuildingBlock.1.0.0.nupkg`), you can use it in your project by:
1. Placing it in a folder (e.g., `bin/release/` or `packages/`)
2. Configuring NuGet to look in that folder
3. Adding a PackageReference in your `.csproj` file
4. Restoring packages

## Step-by-Step Guide

### Step 1: Place the .nupkg File

Place your `.nupkg` file in a folder. Common locations:
- `bin/release/` (as used in this template)
- `packages/`
- `nuget-packages/`
- Any folder you prefer

**Example:**
```
YourProject/
├── bin/
│   └── release/
│       └── MyBuildingBlock.1.0.0.nupkg  ← Place it here
├── YourProject.csproj
└── NuGet.config
```

### Step 2: Configure NuGet.config

Create or update `NuGet.config` in your project root (or solution root) to add the local folder as a package source:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <!-- NuGet.org as fallback -->
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <!-- Local folder with your .nupkg file -->
    <add key="LocalPackages" value="./bin/release" />
  </packageSources>
</configuration>
```

**Important Notes:**
- `value="./bin/release"` is a **relative path** from where `NuGet.config` is located
- Use `./` for relative paths (relative to NuGet.config location)
- Use absolute paths like `C:\packages` if needed
- The folder must contain the `.nupkg` file directly (not in subfolders)

### Step 3: Add PackageReference to .csproj

In your `.csproj` file, add a `PackageReference`:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <!-- ... other content ... -->
  
  <ItemGroup>
    <PackageReference Include="MyBuildingBlock" Version="1.0.0" />
    <!-- Other package references -->
  </ItemGroup>
</Project>
```

**Key Points:**
- `Include="MyBuildingBlock"` - Must match the package ID (not the filename)
- `Version="1.0.0"` - Must match the version in the `.nupkg` filename
- The package ID and version are embedded in the `.nupkg` file metadata

### Step 4: Restore Packages

Run the restore command:

```bash
dotnet restore
```

This will:
1. Read `NuGet.config` to find package sources
2. Look in `./bin/release` for `MyBuildingBlock.1.0.0.nupkg`
3. Extract and install the package
4. Add it to your project references

### Step 5: Build and Use

```bash
dotnet build
```

Now you can use the package in your code:

```csharp
using MyBuildingBlock.Extensions;
using MyBuildingBlock.Data;

// Use MyBuildingBlock types and methods
```

## Current Template Setup

In this template, the setup is already configured:

### NuGet.config
```xml
<add key="LocalPackages" value="./bin/release" />
```

### Project File (Template)
```xml
<PackageReference Include="MyBuildingBlock" Version="1.0.0" />
```

### Package Location
```
StarterKit_Test/bin/release/MyBuildingBlock.1.0.0.nupkg
```

## Troubleshooting

### Package Not Found

**Error:** `NU1101: Unable to find package MyBuildingBlock`

**Solutions:**
1. Verify the file exists:
   ```bash
   ls bin/release/MyBuildingBlock.1.0.0.nupkg
   ```

2. Check NuGet.config path is correct:
   - Relative path: `./bin/release` (from NuGet.config location)
   - Absolute path: `C:\full\path\to\bin\release`

3. Verify package ID and version match:
   ```bash
   # Check package metadata (on Windows with 7-Zip or similar)
   # Or use NuGet Package Explorer
   ```

4. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   dotnet restore
   ```

### Wrong Version

**Error:** Package version mismatch

**Solution:**
- Update the `Version` in `PackageReference` to match the `.nupkg` filename
- Or rename the `.nupkg` file to match the version in `.csproj`

### Path Issues

**Error:** Package source path not found

**Solutions:**
- Use relative paths: `./bin/release` (recommended)
- Use absolute paths: `C:\packages` (if needed)
- Ensure path is relative to `NuGet.config` location, not project file

## Alternative: Direct File Reference

If you only have one package and don't want to configure NuGet.config, you can use a direct file reference:

```xml
<ItemGroup>
  <Reference Include="MyBuildingBlock">
    <HintPath>bin\release\MyBuildingBlock.dll</HintPath>
  </Reference>
</ItemGroup>
```

However, this requires extracting the DLL from the `.nupkg` manually. Using `PackageReference` with `NuGet.config` is the recommended approach.

## Multiple Package Locations

You can add multiple local package sources:

```xml
<packageSources>
  <clear />
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  <add key="LocalPackages" value="./bin/release" />
  <add key="TeamPackages" value="./packages" />
  <add key="CustomPackages" value="C:\MyPackages" />
</packageSources>
```

NuGet will search all sources in order until it finds the package.

## Verifying Package Installation

After restore, check:

1. **Project assets file:**
   ```bash
   cat obj/project.assets.json | grep MyBuildingBlock
   ```

2. **Package cache:**
   ```bash
   dotnet nuget locals global-packages --list
   ```

3. **Build output:**
   - Check `bin/Debug` or `bin/Release` for the DLL
   - Verify no build errors related to the package

## Example: Complete Setup

```bash
# 1. Create folder and place package
mkdir -p bin/release
cp MyBuildingBlock.1.0.0.nupkg bin/release/

# 2. Create/update NuGet.config
cat > NuGet.config << 'EOF'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="LocalPackages" value="./bin/release" />
  </packageSources>
</configuration>
EOF

# 3. Add to .csproj (if not already there)
# Edit YourProject.csproj and add:
# <PackageReference Include="MyBuildingBlock" Version="1.0.0" />

# 4. Restore and build
dotnet restore
dotnet build
```

## Summary

1. ✅ Place `.nupkg` file in a folder (e.g., `bin/release/`)
2. ✅ Configure `NuGet.config` with local package source
3. ✅ Add `PackageReference` in `.csproj` with correct ID and version
4. ✅ Run `dotnet restore`
5. ✅ Build and use the package

The template already has all of this configured! Just ensure the package file is in `bin/release/` and run `dotnet restore`.

