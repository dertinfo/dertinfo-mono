# DertInfo Monorepo

Consolidated repository for the DertInfo platform.

## Apps

| App | Path | Stack |
|-----|------|-------|
| API | `apps/dert-api/` | .NET 8, EF Core, MSSQL |
| Functions | `apps/dert-functions/` | C# Azure Functions (image resize) |
| Website | `apps/dert-web/` | Angular |
| App (PWA) | `apps/dert-app/` | Ionic + Angular (PWA) |

## Packages

| Package | Path | Purpose |
|---------|------|---------|
| Shared contracts | `packages/shared-contracts/` | OpenAPI spec and generated API clients |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server (local or remote) for API development

## Common commands

```bash
# .NET (from repo root, after projects are imported)
dotnet build dertinfo.sln
dotnet test dertinfo.sln

# Angular website
npm run web:build

# Ionic PWA app
npm run app:build
```

## Importing existing repositories

Each legacy repo will be merged with full git history into its `apps/` folder using `git filter-repo`. See the conversation notes or ask in Cursor for the exact commands when you are ready to import a repo.

## CI/CD

GitHub Actions workflows in `.github/workflows/` build only the apps affected by each change (path filters).
