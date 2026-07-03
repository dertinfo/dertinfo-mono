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
- SQL Server (local or remote) for API development — or use Docker Compose below

## Docker quick start

Run the full stack (API, website, PWA, image-resize function, SQL Server, and Azurite) with Docker Desktop:

```bash
# From repo root — copy env templates (see docs/configuration.md)
cp infra/docker/api.env.example infra/docker/api.env
cp infra/docker/web.env.example infra/docker/web.env
cp infra/docker/app.env.example infra/docker/app.env
# Edit api.env: set Auth0__ManagementClientSecret (uncommented)

docker compose up --build
```

| URL | Service |
|-----|---------|
| http://localhost:44100 | API |
| http://localhost:44200 | Website |
| http://localhost:44300 | PWA app |
| http://localhost:44400 | Image resize function |
| localhost:44000 | SQL Server (optional external access) |

The API runs EF migrations automatically on startup. Auth0 login in Docker requires an uncommented `Auth0__ManagementClientSecret` in `infra/docker/api.env` (the management client ID is in `appsettings.json`). See [Configuration](docs/configuration.md). Recreate the API container after changing `api.env`:

```bash
docker compose up -d --force-recreate dertinfo-api
```

Known issues (local website): warmup screen can stick after login — see `docs/planned-fixes/web-warmup-race-condition.md`. Group create may appear to fail on `http://localhost:44200` (silent token renewal blocked by Auth0 on localhost; groups are created — re-login refreshes claims) — see `docs/planned-fixes/local-silent-auth-localhost.md`.

Per-app compose files under `apps/*/infra/docker/` remain available for isolated development.

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

| Repo | Status | Path |
|------|--------|------|
| [dertinfo-api](https://github.com/dertinfo/dertinfo-api) | Imported | `apps/dert-api/` |
| [dertinfo-image-resize-v4](https://github.com/dertinfo/dertinfo-image-resize-v4) | Imported | `apps/dert-functions/` |
| [dertinfo-web](https://github.com/dertinfo/dertinfo-web) | Imported | `apps/dert-web/` (Angular client: `src/client/`) |
| [dertinfo-app](https://github.com/dertinfo/dertinfo-app) | Imported | `apps/dert-app/` (Ionic client: `src/client/`) |

API solution file (legacy): `apps/dert-api/src/DertInfoApiSolution.sln`  
Root solution: `dertinfo.sln` (includes API projects)

## Contributing

Please refer to [CONTRIBUTING.md](CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md). The project is licensed under GNU GPLv3 — see [LICENCE.md](LICENCE.md).

## CI/CD

- **CI:** GitHub Actions workflows (`*-ci.yml`) build and test on `main`, `develop`, and pull requests (path-filtered per app).
- **CD:** GitHub Actions workflows (`*-cd.yml`) deploy to the **`test`** Azure environment and push Docker images to Docker Hub on pushes to `main` only.

See [docs/cicd.md](docs/cicd.md) for workflow inventory, secrets setup, and ADO mapping.
