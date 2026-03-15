---
name: generate_project_ideas
description: The user asks for ideas, improvements, or what to build next.
---
# Ideation Skill

**Skill Name:** generate_project_ideas
**Trigger:** The user asks for ideas, improvements, or what to build next.
**Role:** Senior .NET Architect and Product Manager for a Remote Monitoring and Management (RMM) platform.

## Execution Steps

### 1. Context Refresh
Quickly scan the current state of the `.csproj` files, Blazor UI, SignalR hubs, and EF Core models to understand the current technical baseline and .NET version. Review recent commits or open GitHub issues if accessible.

### 2. Brainstorming Categories
You must generate exactly one high-value, actionable idea for each of the following categories:

*   **Performance & Modernization:** (e.g., upgrading to the latest .NET features, optimizing SignalR serialization/byte-stream, EF Core query optimization).
*   **Security:** (e.g., tightening CORS, adding WebAuthn/Passkeys, role-based access control enhancements, robust rate-limiting).
*   **DevOps / DX:** (e.g., improving Docker footprint, GitHub Actions CI/CD enhancements, adding automated tests for the Alerts API).
*   **End-User Feature (RMM):** (e.g., a new background script type, advanced telemetry dashboard, Wake-on-LAN UI, automated remediation alerts).

### 3. Output Format
Present the ideas to the user in a clean Markdown table with the following columns:

| Idea / Feature Name | Category | Value Proposition (Why it matters to the end user or admin) | Estimated Complexity (Low / Medium / High) | Technical Approach (1-2 sentences on exactly how you would code it in C#/Blazor) |
| :--- | :--- | :--- | :--- | :--- |
| ... | ... | ... | ... | ... |

### 4. Call to Action
Conclude by asking the user: "Which of these ideas would you like me to implement? Reply with your choice, and I will begin the implement-feature workflow."
