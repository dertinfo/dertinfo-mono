# Configuration

How settings are organised across local development, Docker, and Azure-hosted environments.

## Principles

1. **Checked-in configuration should make local development work** — defaults in `appsettings.json` use Azurite endpoints and fixed local callback ports. Tenant- and machine-specific values (SQL, Auth0) live in `infra/secrets/api.env`.

2. **Secrets live outside source control** — under [`infra/secrets/`](../infra/secrets/) (gitignored real files beside `*.example` templates). Placeholders in `appsettings.json` use `[located in user secrets | keyvault]`.

3. **Checked-in values must hold for all consumers of the repo** — shared Auth0 domain/audience (and similar) only if they are correct for every developer. Put secrets **and** per-developer / per-tenant values (SQL connection, SPA Auth0 client IDs) in `infra/secrets/api.env` so empty placeholders make the required replacements obvious.

4. **Azure-hosted environments use Azure App Configuration** — when `AZURE_APP_CONFIG` is set, the API overlays labelled settings (and Key Vault references) onto `appsettings.json`.

5. **Local runs without Azure App Configuration** — if `AZURE_APP_CONFIG` is unset, secrets from [`infra/secrets/api.env`](../infra/secrets/api.env) are set as **process environment variables** before the API starts (`npm run start` or Compose `env_file`). .NET’s default configuration precedence overlays them on `appsettings.json` (same hierarchical keys as App Configuration; mechanism differs).

6. **Local durable data** lives under [`infra/data/`](../infra/data/) (e.g. Azurite). Native and Docker Compose both use this path; richer sharing across run types is future work.

7. **Dependencies are per-project** — do not auto-install on start. If `node_modules` (or tooling) is missing, install that project explicitly (`npm run web:install` / `app:install`). Consolidation across the monorepo is future work.

## Local native development (primary)

### Base settings for local native development

#### Host tooling (Node, nvm, npm)

| Item | Guidance |
|------|----------|
| Engine floor | Root `package.json` / doctor: **Node ≥ 16.10** (Angular 14) |
| Recommended | **Node 20 or 24 LTS** (verified: 24.18.0 with web Angular 14 + app Angular 13) |
| nvm-windows | `nvm install <ver>` then **elevated** `nvm use <ver>` if you see `exit status 5: Access is denied` (symlink under `C:\Program Files\nodejs`) |
| After `nvm use` | Reinstall globals on that Node: `npm install -g azurite@latest @azure/static-web-apps-cli` |
| Per-app deps | `npm run web:install` / `npm run app:install` (or `npm install` in each `apps/*/src/client`) after Node switches |
| Angular CLI | Prefer **project-local** `npx ng` (web CLI 14, app CLI 13) — avoid one global CLI for both |

```powershell
node --version
npm --version
where.exe node
nvm list   # if using nvm-windows
```

#### Azurite

| Item | Guidance |
|------|----------|
| Minimum version | **≥ 3.34.0** (doctor enforces; **3.36.0** verified with Functions Core Tools 4.12) |
| Install | `npm install -g azurite@latest` (not winget — that is for Functions Core Tools) |
| Check running | `npm run status`, or ports `10000/10001/10002` listening; `Server: Azurite-Blob/…` on `:10000` |
| Data directory | `infra/data/azurite` (gitignored), used as `--location` by `npm run start` |
| Well-known account key | Default in `api.env.example` for `StorageAccount__Images__Key` (Azurite emulator key) |
| After upgrade | Stop old listeners on `:10000–10002` before start — otherwise an old process may be reused |
| Disk GC crash | Rename/remove the bad store and use a fresh empty `infra/data/azurite`, **or** allow start’s fallback: `--inMemoryPersistence` **without** `--location` |

Manual disk start (same shape as orchestrator):

```powershell
azurite --blobHost 127.0.0.1 --queueHost 127.0.0.1 --tableHost 127.0.0.1 `
  --blobPort 10000 --queuePort 10001 --tablePort 10002 `
  --location C:\Projects\Cursor\DertInfo\infra\data\azurite
```

#### Functions Core Tools + SWA

| Tool | Install | Notes |
|------|---------|--------|
| `func` v4 | `winget upgrade Microsoft.Azure.FunctionsCoreTools` | 4.12+ needs modern Azurite (API `2024-11-04`) |
| `swa` | `npm install -g @azure/static-web-apps-cli` | Required for website/PWA under `npm run start` |

Isolated worker build generates `obj/.../WorkerExtensions.csproj` (normal). Orchestrator runs `func start --script-root bin/Debug/net8.0` so Core Tools does not see two projects.

#### Required `infra/secrets/api.env` keys

Copy from [`api.env.example`](../infra/secrets/api.env.example). Uncommented `KEY=VALUE` lines only. .NET maps `Auth0__Domain` → `Auth0:Domain`.

| Key | Purpose |
|-----|---------|
| `SqlConnection__ServerName` | e.g. `.\SQLEXPRESS` |
| `SqlConnection__ServerAdminName` / `ServerAdminPassword` / `DatabaseName` | SQL login + DB |
| `Auth0__Domain` / `Audience` / `ManagementClientId` / `ManagementClientSecret` | Dev tenant + M2M |
| `WebClient__Auth0__ClientId` / `PwaClient__Auth0__ClientId` | SPA client IDs |
| `StorageAccount__Images__Key` | Azurite well-known key (example has default) |

Callbacks stay in `appsettings.json` at `http://localhost:44200` / `44300` — register them in Auth0.

#### Ports

| Port | Role |
|------|------|
| 44100 | API |
| 4200 → 44200 | Website `ng serve` → SWA |
| 4201 → 44300 | PWA `ng serve` → SWA |
| 44400 | Functions |
| 10000–10002 | Azurite blob / queue / table |

Orchestration waits for each `ng serve` port before starting SWA.

### Prerequisites (summary)

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 16.10+](https://nodejs.org/) (20/24 LTS recommended); optional [nvm-windows](https://github.com/coreybutler/nvm-windows)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or Express) — connection details in `infra/secrets/api.env`
- Azurite CLI **≥ 3.34.0**, Functions Core Tools **v4**, SWA CLI — see tables above
- `sqlcmd` on `PATH` (for `npm run doctor` database checks)

`npm run doctor` fails if Azurite is older than **3.34.0**. After upgrading Azurite, stop any process still bound to `:10000–10002` before `npm run start`.

```bash
# Secrets
cp infra/secrets/api.env.example infra/secrets/api.env
# Edit infra/secrets/api.env — SQL password, Azurite key, Auth0 management secret

# Per-project deps (only when missing / out of date — not on every start)
npm run web:install
npm run app:install
cp apps/dert-functions/src/dertinfo-image-resize/local.settings.json.example `
   apps/dert-functions/src/dertinfo-image-resize/local.settings.json

npm run doctor    # fail-fast: tooling, secrets, SQL schema, Azurite, Auth0 OIDC
npm run start     # services selected in infra/dev/runtime.json
npm run status
npm run stop
```

Copy [`infra/dev/runtime.example.json`](../infra/dev/runtime.example.json) → `infra/dev/runtime.json` to choose what starts (`api` / `web` / `app` / `imageResize` / `azurite` booleans). Defaults (and the example) start everything; set `"api": false` when debugging the API in Visual Studio.

| URL | Service |
|-----|---------|
| http://localhost:44100 | API |
| http://localhost:44200 | Website |
| http://localhost:44300 | PWA app |
| http://localhost:44400 | Image resize |
| http://127.0.0.1:10000 | Azurite blob |

Auth0 SPA callback URLs in `appsettings.json` use the fixed local ports (`http://localhost:44200`, `http://localhost:44300`). Register those callbacks in whichever Auth0 tenant you configure via `infra/secrets/api.env`.

Orchestration scripts: [`infra/dev/`](../infra/dev/). Change log for this workstream: [`docs/changelogs/2026-07-25-local-native-dev.md`](./changelogs/2026-07-25-local-native-dev.md).

## Configuration layers (API)

| Layer | When | Purpose |
|-------|------|---------|
| `appsettings.json` | Always (base) | Local-friendly non-secrets; placeholders for secrets |
| `infra/secrets/api.env` | Local via process env / Compose `env_file` | Secret values injected before start (same keys App Configuration would supply) |
| .NET user secrets | Visual Studio F5 / `dotnet user-secrets` | Same keys as `api.env` but with `:` nesting — set manually when debugging the API in VS (VS does not load `api.env`) |
| Process environment | Always | Standard .NET override of `appsettings.json` (`__` → `:`) |
| `AZURE_APP_CONFIG` + Key Vault | Test / production in Azure | Hosted overlay |

### Local secrets ≈ App Configuration

Deployed hosts set `AZURE_APP_CONFIG` and load labelled configuration (Key Vault for secret values). Locally, with that variable **unset**:

- **`npm run start` / Compose:** load `infra/secrets/api.env` into the process environment; `CreateDefaultBuilder` applies those variables over `appsettings.json`.
- **Visual Studio F5:** set the same settings via **user secrets** (Manage User Secrets). Map `Auth0__Domain` → `Auth0:Domain`, etc. Details: [`apps/dert-api/README.md`](../apps/dert-api/README.md#2-visual-studio-f5--debug-the-api-project).

No custom secrets parsing in `Program.cs`.

### Auth0 (local)

| Setting | Where | Secret? |
|---------|-------|---------|
| `Auth0:Domain` / `Audience` / `ManagementClientId` / `ManagementClientSecret` | `infra/secrets/api.env` | Tenant-specific (secret or not) |
| `WebClient:Auth0:ClientId` / `PwaClient:Auth0:ClientId` | `infra/secrets/api.env` | Tenant-specific (not shared in repo) |
| `WebClient:Auth0:CallbackUrl` / `PwaClient:Auth0:CallbackUrl` | `appsettings.json` | No (fixed local ports) |

### Frontends (web / PWA)

| Environment | API URL & Auth0 callback |
|-------------|--------------------------|
| Local `ng serve` / `npm run start` | `apps/*/src/client/src/assets/app.config.json` |
| Docker | `infra/docker/web.env` / `app.env` |
| Azure Static Web Apps | Build-time env or remote `/api/clientconfiguration/*` |

Client-facing Auth0 settings are also exposed via `GET /api/clientconfiguration/web` and `/app`.

## Data paths

| Path | Role |
|------|------|
| `infra/data/azurite/` | Azurite persistence (native `--location` and Compose bind mount) |
| SQL Server | Instance data directories (not under `infra/data/`) |

## Docker (optional / secondary)

Compose remains available for full-stack containers. Azurite data uses `./infra/data/azurite`. API `env_file` is `./infra/secrets/api.env` (same secrets file as native).

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
# fill secrets
docker compose up --build
```

## What not to do

- Do not hardcode machine-specific SQL connection details in `appsettings.json` — use `infra/secrets/api.env`.
- Do not put secrets under `infra/docker/` for local API runs — use `infra/secrets/`.
- Do not add shared non-secret Auth0 identifiers to `api.env` if they already exist in `appsettings.json`.
- Do not commit secrets; do not require Azure App Configuration for local runs.
- Do not auto-download npm/dotnet dependencies on every `start` — install per project when needed.
- Do not change production/test Azure settings in checked-in files; those are owned by App Configuration labels.

## Future work

- Consolidate dependency install across web/app (and tooling) in the monorepo.
- Stronger shared-storage semantics between native and Docker run types beyond the shared `infra/data/` path.

## Related docs

- [`infra/secrets/README.md`](../infra/secrets/README.md) — secrets layout
- [`infra/dev/README.md`](../infra/dev/README.md) — doctor / start / stop
- [`apps/dert-api/README.md`](../apps/dert-api/README.md) — API-specific notes
- [`README.md`](../README.md) — monorepo quick start
