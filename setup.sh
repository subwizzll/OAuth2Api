#!/bin/bash

echo "🚀 Setting up OAuth2 API development environment..."

# Check if .env.local exists
if [ ! -f .env.local ]; then
    echo "❌ Error: .env.local file not found!"
    echo "Please create a .env.local file with the required environment variables."
    exit 1
fi

# Remove existing .env if it exists
if [ -f .env ] || [ -L .env ]; then
    rm .env
fi

# Create symbolic link
ln -s .env.local .env
echo "✅ Created symbolic link from .env.local to .env"

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: dotnet SDK is not installed!"
    echo "Please install the .NET SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✅ Setup completed successfully!"
echo "You can now run the application." 