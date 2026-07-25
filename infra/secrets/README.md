# Local secrets

Gitignored secret files live **beside** their checked-in `*.example` siblings.

| File | Used by | Purpose |
|------|---------|---------|
| `api.env.example` | (template) | Required keys for the API when running without Azure App Configuration |
| `api.env` | `npm run start` (process env); Compose `env_file` | Real values — copy from the example and fill in (includes machine-specific SQL settings) |

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
# Edit api.env — set passwords and Auth0 management secret
```

## Behaviour vs Azure App Configuration

- **Hosted:** `AZURE_APP_CONFIG` is set → the API overlays App Configuration (+ Key Vault) on `appsettings.json`.
- **Local (`npm run start` / Compose):** `AZURE_APP_CONFIG` is unset → secrets from `api.env` are injected into the **process environment** before the API starts. .NET’s default configuration precedence applies `Auth0__X` → `Auth0:X` over `appsettings.json`.
- **Visual Studio F5:** VS does not load `api.env`. Set the same values via **Manage User Secrets** (`:` keys). See [`apps/dert-api/README.md`](../../apps/dert-api/README.md#2-visual-studio-f5--debug-the-api-project).

Do not commit `api.env`. Do not put non-secrets here if they already belong in `appsettings.json`.

See also: [Base settings for local native development](../../docs/configuration.md#base-settings-for-local-native-development).
