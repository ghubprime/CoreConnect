#!/bin/bash
echo "Restoring packages..."
dotnet restore

echo "Building project with verbose output..."
dotnet build -v normal

echo "Running tests..."
dotnet test --no-build --verbosity normal
