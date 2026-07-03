# Configuration

How settings are organised across local development, Docker, and Azure-hosted environments.

## Principles

1. **Checked-in configuration should make local development work** — defaults in `appsettings.json`, `docker-compose.yml`, and frontend `*.env.example` files use localhost URLs, Azurite, and other local substitutes where possible.

2. **Secrets and third-party SaaS credentials are the exception** — only values that cannot be faked locally (or that must point at a shared dev tenant) live outside source control.

3. **Non-secret identifiers stay in source control** — e.g. `Auth0:ManagementClientId`, `Auth0:Domain`, `Auth0:Audience`, and Auth0 application client IDs are not secrets and belong in `appsettings.json` or compose environment, not in `api.env`.

4. **Azure-hosted environments use Azure App Configuration** — when `AZURE_APP_CONFIG` is set, the API loads labelled settings (and Key Vault references) from Azure App Configuration instead of relying on local files.

5. **Local runs without Azure App Configuration** — if `AZURE_APP_CONFIG` is unset, `Program.cs` skips App Configuration entirely and the app uses `appsettings.json`, environment variables, user secrets, or Docker `env_file` overrides.

## Configuration layers

| Layer | When | Purpose |
|-------|------|---------|
| `appsettings.json` | Always (base) | Local-friendly defaults; non-secret SaaS identifiers; placeholders for secrets |
| User secrets | `dotnet run` / VS locally | Developer-specific secrets (see `apps/dert-api/README.md`) |
| `infra/docker/api.env` | Docker Compose API service | **Secrets only** — gitignored; overrides placeholders in the image |
| `infra/docker/web.env` / `app.env` | Docker Compose frontends | Public runtime URLs (`API_URL`, callback URLs) — not secrets |
| `docker-compose.yml` `environment:` | Docker Compose | Local infrastructure (SQL, Azurite, Auth0 client IDs, CORS-related IDs) |
| `AZURE_APP_CONFIG` + Key Vault | Test / production in Azure | Replaces local values with environment-specific settings |

### API: Azure App Configuration

```43:69:apps/dert-api/src/dertinfo-api/Program.cs
        private static void LoadConfiguration(IConfigurationBuilder config)
        {
            var appConfigurationUri = Environment.GetEnvironmentVariable("AZURE_APP_CONFIG");

            if (appConfigurationUri != null)
            {
                // ... DefaultAzureCredential + AddAzureAppConfiguration ...
            }
        }
```

- **Local / Docker:** `AZURE_APP_CONFIG` is not set → App Configuration is not used.
- **Azure:** connection string URI is set → settings for `ASPNETCORE_ENVIRONMENT` label override `appsettings.json`; Key Vault secrets are resolved at runtime.

Client-facing Auth0 and telemetry settings for the Angular apps are exposed via `GET /api/clientconfiguration/web` and `/app`, which read from the same `IConfiguration` stack (`ClientConfigurationController`).

### Auth0 (example)

| Setting | Where (local) | Secret? |
|---------|----------------|---------|
| `Auth0:Domain` | `appsettings.json` | No |
| `Auth0:Audience` | `appsettings.json` | No |
| `Auth0:ManagementClientId` | `appsettings.json` | No |
| `Auth0:ManagementClientSecret` | `api.env` or user secrets | **Yes** |
| `WebClient:Auth0:ClientId` | `docker-compose.yml` (Docker) or user secrets (VS) | No |
| `PwaClient:Auth0:ClientId` | `docker-compose.yml` (Docker) or user secrets (VS) | No |

Local Docker only needs **`Auth0__ManagementClientSecret`** in `infra/docker/api.env` (uncommented). The management client ID is already in `appsettings.json`.

Other SaaS examples (SendGrid, MailGun when enabled) follow the same pattern: checked-in config disables or stubs the integration locally; API keys go in `api.env` or user secrets.

### Frontends (web / PWA)

| Environment | API URL & Auth0 callback |
|-------------|--------------------------|
| Local `ng serve` | `apps/dert-web/src/client/src/assets/app.config.json` |
| Docker | `infra/docker/web.env` / `app.env` (substituted into `app.config.json` at container start) |
| Azure Static Web Apps | Build-time `environment.*.ts` or remote `/api/clientconfiguration/*` |

## Docker quick reference

```bash
cp infra/docker/api.env.example infra/docker/api.env
# Edit api.env: set Auth0__ManagementClientSecret only (uncommented)

docker compose up -d --force-recreate dertinfo-api   # after changing api.env
```

Files in `infra/docker/api.env`, `web.env`, and `app.env` are gitignored.

## What not to do

- Do not add non-secret values to `api.env` if they already exist in `appsettings.json`.
- Do not commit secrets to `appsettings.json` — use `[located in user secrets | keyvault]` placeholders.
- Do not require Azure App Configuration for local runs.
- Do not change production/test Azure settings in checked-in files; those are owned by App Configuration labels.

## Related docs

- `apps/dert-api/README.md` — user secrets template for Visual Studio / devcontainer
- `README.md` — Docker quick start
- `docs/planned-fixes/web-warmup-race-condition.md` — example of misconfigured auth blocking the dashboard
