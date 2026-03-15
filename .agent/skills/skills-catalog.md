# CoreConnect Agent Skills Catalog

This catalog outlines the automated scripts available to the Lead Architect for testing, migrating, and verifying the application.

## 1. Build and Test (`build_and_test.sh`)
- **Usage:** `./.agent/skills/build_and_test.sh`
- **Purpose:** Restores, builds, and runs all unit tests for the .NET solution. Outputs verbose logs to help identify and self-correct any compiler errors locally.

## 2. EF Migrations (`ef_migrations.sh`)
- **Usage:** `./.agent/skills/ef_migrations.sh "MigrationName"`
- **Purpose:** Safely generates an Entity Framework Core migration with the specified name. Use this after altering any EF Core schemas to ensure consistency across SQLite, SQL Server, and PostgreSQL.

## 3. Docker Test Env (`docker_test_env.sh`)
- **Usage:** `./.agent/skills/docker_test_env.sh`
- **Purpose:** Spins up the application using the repository's `docker-compose/docker-compose.yml`, pings `localhost:5001` to verify the container is healthy, and cleanly tears it down.

## 4. Test API Alerts (`test_api_alerts.ps1`)
- **Usage:** `./.agent/skills/test_api_alerts.ps1`
- **Purpose:** Executes a mock POST request to the `/api/Alerts/Create/` endpoint using PowerShell. Validates that the Alerts API correctly parses incoming authenticated payloads without having to manually use Postman.
