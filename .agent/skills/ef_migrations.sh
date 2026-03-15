#!/bin/bash
if [ -z "$1" ]; then
    echo "Usage: $0 <MigrationName>"
    exit 1
fi

MIGRATION_NAME=$1

echo "Adding EF Core migration: $MIGRATION_NAME..."
dotnet ef migrations add "$MIGRATION_NAME"
