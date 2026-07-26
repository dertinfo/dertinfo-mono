# Local secrets

Gitignored secret files live **beside** their checked-in `*.example` siblings.

| File | Used by | Purpose |
|------|---------|---------|
| `api.env.example` | (template) | Required keys for the API when running without Azure App Configuration |
| `api.env` | `npm run start` (process env); Compose `env_file` | Real values — copy from the example and fill in |

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
# Edit api.env — see below
```

## What to fill in (first clone)

| Keys | Source |
|------|--------|
| `SqlConnection__*` | **Your machine** — SQL Express/Server instance, SQL login, and database name. Create an **empty** database with that name before first API start. |
| `Auth0__*` and SPA `*ClientId` | **Shared team dev tenant** — get domain, audience, client IDs, and Management API secret from a teammate or your team vault. Do not invent a new tenant unless you also configure Auth0 callbacks for `http://localhost:44200` / `44300`. |
| `StorageAccount__Images__Key` | Keep the example **Azurite** well-known key for local. |

Never commit `api.env`.

## Behaviour vs Azure App Configuration

- **Hosted:** `AZURE_APP_CONFIG` is set → the API overlays App Configuration (+ Key Vault) on `appsettings.json`.
- **Local (`npm run start` / Compose):** `AZURE_APP_CONFIG` is unset → secrets from `api.env` are injected into the **process environment** before the API starts. .NET’s default configuration precedence applies `Auth0__X` → `Auth0:X` over `appsettings.json`.
- **Visual Studio F5:** VS does not load `api.env`. Set the same values via **Manage User Secrets** (`:` keys). See [`apps/dert-api/README.md`](../../apps/dert-api/README.md#2-visual-studio-f5--debug-the-api-project).

Do not put non-secrets here if they already belong in `appsettings.json`.

See also: [Configuration](../../docs/technical/infra/configuration.md), [First-time setup](../../README.md#first-time-setup-clone--running-locally).
