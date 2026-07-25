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

| Tool | Version / notes | Check / install |
|------|-----------------|-----------------|
| [.NET 8 SDK](https://dotnet.microsoft.com/download) | Required for API + Functions | `dotnet --version` |
| [Node.js](https://nodejs.org/) | **≥ 16.10** (Angular 14 floor); **20 or 24 LTS** recommended | `node --version` |
| [nvm-windows](https://github.com/coreybutler/nvm-windows) (optional) | Switch Node versions; `nvm use` often needs an **elevated** terminal (exit 5 = Access denied) | `nvm list` / `nvm use 24.18.0` |
| npm | Comes with Node; **reinstall globals after `nvm use`** | `npm --version` |
| [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) / Express | Connection in `infra/secrets/api.env` | — |
| [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) | **≥ 3.34.0** (3.36+ verified with Functions 4.12) | `npm install -g azurite@latest` → `azurite --version` |
| [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local) | **v4** (e.g. 4.12.x) | `winget upgrade Microsoft.Azure.FunctionsCoreTools` → `func --version` |
| [Azure Static Web Apps CLI](https://azure.github.io/static-web-apps-cli/) | Required for web/app under `npm run start` | `npm install -g @azure/static-web-apps-cli` → `swa --version` |
| `sqlcmd` | For `npm run doctor` SQL checks | on `PATH` |

**nvm / npm:** Prefer one active Node via nvm. After `nvm use <version>`, globals are per-version — run `npm install -g azurite@latest @azure/static-web-apps-cli` again. Confirm `where.exe node` and `where.exe azurite` point at the expected Node.

**Azurite + Functions:** Core Tools 4.12+ uses Storage API `2024-11-04`. Azurite **3.31.x** fails Functions host startup. After upgrading Azurite, **stop any old process** on `:10000–10002`. Data lives under `infra/data/azurite` (gitignored). If disk start GC-crashes, reset that folder (rename to `azurite.bak` and recreate empty) or let `npm run start` fall back to `--inMemoryPersistence` **without** `--location` (Azurite 3.36 forbids combining them). Details: [`docs/configuration.md`](docs/configuration.md#base-settings-for-local-native-development), [`infra/dev/README.md`](infra/dev/README.md).

## Local native quick start

Primary path: everything on the host; Auth0 is whatever cloud tenant you configure in `infra/secrets/api.env`. See [Configuration](docs/configuration.md).

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
# Edit infra/secrets/api.env — SQL, Auth0 domain/audience/client IDs, management secret
# StorageAccount__Images__Key default is the well-known Azurite key

cp infra/dev/runtime.example.json infra/dev/runtime.json   # optional; all-true if missing

npm run web:install          # when node_modules missing or after Node version change
npm run app:install
cp apps/dert-functions/src/dertinfo-image-resize/local.settings.json.example \
   apps/dert-functions/src/dertinfo-image-resize/local.settings.json

npm run doctor
npm run start
npm run status
```

`npm run start` follows [`infra/dev/runtime.json`](infra/dev/runtime.json) (gitignored). By default all services start; set `"api": false` to F5 the API in Visual Studio while npm runs the rest.

| URL | Service |
|-----|---------|
| http://localhost:44100 | API |
| http://localhost:44200 | Website (SWA → ng `:4200`) |
| http://localhost:44300 | PWA app (SWA → ng `:4201`) |
| http://localhost:44400 | Image resize |
| http://127.0.0.1:10000 | Azurite blob |

The API applies EF migrations on startup. Secrets from `infra/secrets/api.env` are injected into the process environment by `npm run start` (or Compose `env_file`); .NET applies them over `appsettings.json` via normal configuration precedence.

**Visual Studio (API F5 only):** VS does not load `api.env`. Set the same values via **Manage User Secrets** on the API project (`:` keys). See [`apps/dert-api/README.md`](apps/dert-api/README.md#2-visual-studio-f5--debug-the-api-project).

Known issues (local website): warmup screen can stick after login — see `docs/planned-fixes/web-warmup-race-condition.md`. Group create may appear to fail on `http://localhost:44200` (silent token renewal blocked by Auth0 on localhost; groups are created — re-login refreshes claims) — see `docs/planned-fixes/local-silent-auth-localhost.md`. The PWA on `:44300` does not show the same basic login problem.

## Docker (optional)

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
cp infra/docker/web.env.example infra/docker/web.env
cp infra/docker/app.env.example infra/docker/app.env
docker compose up --build
```

Compose uses the same `infra/secrets/api.env` and Azurite data under `infra/data/azurite`.

## Common commands

```bash
npm run doctor
npm run start
npm run status
npm run stop

dotnet build dertinfo.sln
dotnet test dertinfo.sln

npm run web:build
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
