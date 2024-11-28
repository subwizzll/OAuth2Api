# Requires -RunAsAdministrator

Write-Host "🚀 Setting up OAuth2 API development environment..." -ForegroundColor Cyan

# Check if .env.local exists
if (-not (Test-Path .env.local)) {
    Write-Host "❌ Error: .env.local file not found!" -ForegroundColor Red
    Write-Host "Please create a .env.local file with the required environment variables."
    exit 1
}

# Remove existing .env if it exists
if (Test-Path .env) {
    Remove-Item .env -Force
}

# Create symbolic link
try {
    New-Item -ItemType SymbolicLink -Path .env -Target .env.local
    Write-Host "✅ Created symbolic link from .env.local to .env" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error: Failed to create symbolic link. Make sure you're running as Administrator!" -ForegroundColor Red
    exit 1
}

# Check if dotnet is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Error: dotnet SDK is not installed!" -ForegroundColor Red
    Write-Host "Please install the .NET SDK from https://dotnet.microsoft.com/download"
    exit 1
}

Write-Host "✅ Setup completed successfully!" -ForegroundColor Green
Write-Host "You can now run the application." 