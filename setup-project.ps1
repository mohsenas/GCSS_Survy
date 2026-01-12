# Setup Script for Template Repository
# This script renames all project-specific references when creating a new project from the template

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectName
)

$ErrorActionPreference = "Stop"

# Validate project name
if ([string]::IsNullOrWhiteSpace($ProjectName)) {
    Write-Error "Project name cannot be empty"
    exit 1
}

# Validate project name contains only valid characters (alphanumeric, underscore, hyphen)
if ($ProjectName -notmatch '^[a-zA-Z0-9_-]+$') {
    Write-Error "Project name can only contain letters, numbers, underscores, and hyphens"
    exit 1
}

# Template values to replace
$TemplateProjectName = "StarterKit_Test"
$TemplateShortName = "StarterKit"

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "Project Setup Script" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "Template Project: $TemplateProjectName" -ForegroundColor Yellow
Write-Host "New Project Name: $ProjectName" -ForegroundColor Green
Write-Host ""

# Get the script directory (root of repository)
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFolder = Join-Path $ScriptDir $TemplateProjectName
$NewProjectFolder = Join-Path $ScriptDir $ProjectName

# Check if target folder already exists
if (Test-Path $NewProjectFolder) {
    Write-Error "Target project folder '$ProjectName' already exists. Please choose a different name or remove the existing folder."
    exit 1
}

if (-not (Test-Path $ProjectFolder)) {
    Write-Error "Template project folder '$TemplateProjectName' not found. Make sure you're running this script from the repository root."
    exit 1
}

Write-Host "Step 1: Renaming project folder..." -ForegroundColor Yellow
Rename-Item -Path $ProjectFolder -NewName $ProjectName
Write-Host "  ✓ Renamed folder: $TemplateProjectName → $ProjectName" -ForegroundColor Green

Write-Host "Step 2: Renaming solution file..." -ForegroundColor Yellow
$SolutionFile = Join-Path $ScriptDir "$TemplateProjectName.sln"
$NewSolutionFile = Join-Path $ScriptDir "$ProjectName.sln"
if (Test-Path $SolutionFile) {
    Rename-Item -Path $SolutionFile -NewName "$ProjectName.sln"
    Write-Host "  ✓ Renamed solution: $TemplateProjectName.sln → $ProjectName.sln" -ForegroundColor Green
}

Write-Host "Step 3: Renaming project file..." -ForegroundColor Yellow
$ProjectFile = Join-Path $NewProjectFolder "$TemplateProjectName.csproj"
$NewProjectFile = Join-Path $NewProjectFolder "$ProjectName.csproj"
if (Test-Path $ProjectFile) {
    Rename-Item -Path $ProjectFile -NewName "$ProjectName.csproj"
    Write-Host "  ✓ Renamed project file: $TemplateProjectName.csproj → $ProjectName.csproj" -ForegroundColor Green
}

Write-Host "Step 4: Updating files with project references..." -ForegroundColor Yellow

# Function to replace text in files
function Replace-InFiles {
    param(
        [string]$Path,
        [string]$SearchText,
        [string]$ReplaceText,
        [string[]]$FileExtensions = @("*.cs", "*.json", "*.sln", "*.csproj", "*.md", "Dockerfile", "*.ps1", "*.sh")
    )
    
    $files = Get-ChildItem -Path $Path -Recurse -Include $FileExtensions | Where-Object { $_.FullName -notmatch '\\obj\\' -and $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\.git\\' }
    
    $count = 0
    foreach ($file in $files) {
        $content = Get-Content $file.FullName -Raw -Encoding UTF8
        if ($content -match [regex]::Escape($SearchText)) {
            $newContent = $content -replace [regex]::Escape($SearchText), $ReplaceText
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
            $count++
        }
    }
    return $count
}

# Replace namespace references (exact match)
$count = Replace-InFiles -Path $NewProjectFolder -SearchText $TemplateProjectName -ReplaceText $ProjectName
Write-Host "  ✓ Updated $count files with namespace references" -ForegroundColor Green

# Replace short name in solution file
if (Test-Path $NewSolutionFile) {
    $content = Get-Content $NewSolutionFile -Raw -Encoding UTF8
    $newContent = $content -replace [regex]::Escape($TemplateProjectName), $ProjectName
    Set-Content -Path $NewSolutionFile -Value $newContent -Encoding UTF8 -NoNewline
    Write-Host "  ✓ Updated solution file" -ForegroundColor Green
}

# Replace short name in configuration and code files (StarterKit → ProjectName)
$replacements = @(
    @{ Search = $TemplateShortName; Replace = $ProjectName; Context = "API routes and config" },
    @{ Search = "$TemplateShortName-app"; Replace = "$ProjectName-app"; Context = "Application name" },
    @{ Search = "$TemplateShortName.API"; Replace = "$ProjectName.API"; Context = "JWT Issuer" },
    @{ Search = "$TemplateShortName.Client"; Replace = "$ProjectName.Client"; Context = "JWT Audience" }
)

foreach ($replacement in $replacements) {
    # Only update files in the new project folder and root-level files (not submodules)
    $count = Replace-InFiles -Path $NewProjectFolder -SearchText $replacement.Search -ReplaceText $replacement.Replace -FileExtensions @("*.cs", "*.json", "*.md")
    # Also update root-level documentation files
    $rootFiles = Get-ChildItem -Path $ScriptDir -Include @("*.md", "*.ps1", "*.sh") -File | Where-Object { $_.FullName -notmatch '\\MyBuildingBlock\\' -and $_.FullName -notmatch '\\StarterKit_Test\\' }
    $rootCount = 0
    foreach ($file in $rootFiles) {
        $content = Get-Content $file.FullName -Raw -Encoding UTF8
        if ($content -match [regex]::Escape($replacement.Search)) {
            $newContent = $content -replace [regex]::Escape($replacement.Search), $replacement.Replace
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
            $rootCount++
        }
    }
    if (($count + $rootCount) -gt 0) {
        Write-Host "  ✓ Updated $($count + $rootCount) files: $($replacement.Context)" -ForegroundColor Green
    }
}

# Update Dockerfile specifically
$Dockerfile = Join-Path $NewProjectFolder "Dockerfile"
if (Test-Path $Dockerfile) {
    $content = Get-Content $Dockerfile -Raw -Encoding UTF8
    $newContent = $content -replace [regex]::Escape($TemplateProjectName), $ProjectName
    Set-Content -Path $Dockerfile -Value $newContent -Encoding UTF8 -NoNewline
    Write-Host "  ✓ Updated Dockerfile" -ForegroundColor Green
}

# Update database names in appsettings.json
$AppSettingsFile = Join-Path $NewProjectFolder "appsettings.json"
if (Test-Path $AppSettingsFile) {
    $content = Get-Content $AppSettingsFile -Raw -Encoding UTF8
    # Replace database names: Database=StarterKit → Database={ProjectName}
    # Escape the search string for regex
    $escapedTemplate = [regex]::Escape($TemplateShortName)
    $newContent = $content -replace "Database=$escapedTemplate;", "Database=$ProjectName;"
    $newContent = $newContent -replace "Database=$escapedTemplate`_Log", "Database=$ProjectName`_Log"
    # Handle database names with numbers (like StarterKit2) - use regex for this
    $newContent = $newContent -replace "Database=$escapedTemplate\d+", "Database=$ProjectName"
    # Final replacement for any remaining instances (without trailing characters)
    $newContent = $newContent -replace "Database=$escapedTemplate`"", "Database=$ProjectName`""
    Set-Content -Path $AppSettingsFile -Value $newContent -Encoding UTF8 -NoNewline
    Write-Host "  ✓ Updated database names in appsettings.json" -ForegroundColor Green
}

Write-Host ""
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review the changes in the '$ProjectName' folder" -ForegroundColor White
Write-Host "2. Update connection strings in appsettings.json if needed" -ForegroundColor White
Write-Host "3. Run 'dotnet restore' to restore NuGet packages" -ForegroundColor White
Write-Host "4. Run 'dotnet build' to verify the project builds successfully" -ForegroundColor White
Write-Host "5. Update README.md and other documentation files as needed" -ForegroundColor White
Write-Host ""

