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

Prerequisites on the machine:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 16.10+](https://nodejs.org/) (Angular 14 compatibility floor)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (or Express) — connection details in `infra/secrets/api.env` (API applies EF migrations on startup)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) CLI on `PATH` — **≥ 3.34.0** (`npm install -g azurite@latest`; `azurite --version`)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local) **v4** (`func`) for image-resize — e.g. 4.12.x (`winget upgrade Microsoft.Azure.FunctionsCoreTools`; `func --version`)
- [Azure Static Web Apps CLI](https://azure.github.io/static-web-apps-cli/) (`swa`) for web and app
- `sqlcmd` on `PATH` (for `npm run doctor` database checks)

`npm run doctor` fails if Azurite is older than **3.34.0** (Functions Core Tools 4.12+ requires Storage API `2024-11-04`). After upgrading Azurite, stop any process still bound to `:10000–10002` before `npm run start`, or the orchestrator may reuse the old emulator.

```bash
# Secrets
cp infra/secrets/api.env.example infra/secrets/api.env
# Edit infra/secrets/api.env — SQL password, Azurite key, Auth0 management secret

# Per-project deps (only when missing / out of date — not on every start)
npm run web:install
npm run app:install
cp apps/dert-functions/src/dertinfo-image-resize/local.settings.json.example \
   apps/dert-functions/src/dertinfo-image-resize/local.settings.json

npm run doctor    # fail-fast: tooling, secrets, SQL schema, Azurite, Auth0 OIDC
npm run start     # services selected in infra/dev/runtime.json (api defaults off)
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

Orchestration scripts: [`infra/dev/`](../infra/dev/).

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
