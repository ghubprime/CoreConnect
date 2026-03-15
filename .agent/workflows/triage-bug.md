---
description: triage and fix bugs based on issue reports
---
# Triage Bug Workflow

Step 1: Ingest the bug report and immediately check server logs (remind yourself they are in /app/AppData/logs or standard console output).

Step 2: If it's a proxy/networking issue, instruct yourself to temporarily enable UseHttpLogging in appsettings.json to debug headers.

Step 3: Implement the C# fix, verify via tests, and open a PR.
