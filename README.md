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
- [Node.js 16.10+](https://nodejs.org/) (Angular 14 floor; raise later as apps upgrade)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or Express) — set connection in `infra/secrets/api.env`
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) CLI — **≥ 3.34.0** (use `npm install -g azurite@latest`; check with `azurite --version`)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local) **v4** (`func`) — e.g. **4.12.x** via `winget upgrade Microsoft.Azure.FunctionsCoreTools` (check with `func --version`)
- [Azure Static Web Apps CLI](https://azure.github.io/static-web-apps-cli/) (`swa`)
- `sqlcmd` on `PATH` (for `npm run doctor`)

**Functions + Azurite:** Core Tools 4.12+ talks to Storage with API version `2024-11-04`. Azurite **3.31.x** rejects that and Functions host startup fails. After upgrading Azurite, **stop any old Azurite process** still listening on `:10000–10002` (`npm run stop` or kill those ports), then start again — otherwise `npm run start` may reuse the outdated process. If the on-disk store under `infra/data/azurite` fails to start (GC crash), `npm run start` retries with `--inMemoryPersistence`.

## Local native quick start

Primary path: everything on the host; Auth0 is whatever cloud tenant you configure in `infra/secrets/api.env`. See [Configuration](docs/configuration.md).

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
# Edit infra/secrets/api.env — SQL password, Azurite key, Auth0 management secret

npm run web:install          # only when node_modules missing
npm run app:install
cp apps/dert-functions/src/dertinfo-image-resize/local.settings.json.example \
   apps/dert-functions/src/dertinfo-image-resize/local.settings.json

npm run doctor
npm run start
```

`npm run start` follows [`infra/dev/runtime.json`](infra/dev/runtime.json) (copy from [`runtime.example.json`](infra/dev/runtime.example.json)). By default all services start; set `"api": false` in your local `runtime.json` to F5 the API in Visual Studio while npm runs the rest.
| URL | Service |
|-----|---------|
| http://localhost:44100 | API |
| http://localhost:44200 | Website |
| http://localhost:44300 | PWA app |
| http://localhost:44400 | Image resize |
| http://127.0.0.1:10000 | Azurite blob |

The API applies EF migrations on startup. Secrets from `infra/secrets/api.env` are injected into the process environment by `npm run start` (or Compose `env_file`); .NET applies them over `appsettings.json` via normal configuration precedence.

**Visual Studio (API F5 only):** VS does not load `api.env`. Set the same values via **Manage User Secrets** on the API project (`:` keys). See [`apps/dert-api/README.md`](apps/dert-api/README.md#2-visual-studio-f5--debug-the-api-project).

Known issues (local website): warmup screen can stick after login — see `docs/planned-fixes/web-warmup-race-condition.md`. Group create may appear to fail on `http://localhost:44200` (silent token renewal blocked by Auth0 on localhost; groups are created — re-login refreshes claims) — see `docs/planned-fixes/local-silent-auth-localhost.md`.

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
