---
name: Bicep standards
type: standards
status: active
updated: 2026-08-30
---

# Bicep standards

House conventions for Azure Bicep in this monorepo. Follow these wherever you add or change templates under [`infra/bicep/`](../../../../infra/bicep/).

**Primary reference:** subscription policy composition [`infra/bicep/subscription/policy/main.bicep`](../../../../infra/bicep/subscription/policy/main.bicep) and the workload folders beside it (`config`, `monitoring`, `storage`, `api`). Cursor rule: [`.cursor/rules/bicep.mdc`](../../../../.cursor/rules/bicep.mdc).

Requires **Bicep CLI 0.44.1+** (extendable parameter files).

## Workload layout

Each RG-scoped workload lives under `infra/bicep/<part>/`, not under `apps/*/infra/bicep` (Functions remains the exception until a later alignment).

```
infra/bicep/<part>/
  main.bicep
  main.shared.bicepparam    # using none
  main.dev.bicepparam       # using 'main.bicep' + extends shared
  main.prod.bicepparam      # environmentTag = prd
  <composition>/            # local module only when it adds real composition
    main.bicep              # or a named file such as sql.bicep
```

Same `main.bicep` for development and production. Leaf files override only what differs (`environmentTag`, SKUs, resource names).

## Section order in `main.bicep`

1. Parameters  
2. Variables  
3. References  
4. Resources  
5. Modules  
6. **AVM Modules**  
7. Outputs  

Use the comment banners already in the subscription and workload templates.

## Modules vs AVM Modules

- **AVM Modules** — call Azure Verified Modules (`br/public:avm/res/...`) **directly** from `main.bicep`. Pin the version (for example `:0.14.0`). Do not wrap a single AVM call in a local folder.
- **Modules** — local `.bicep` only when they add composition: multiple resources, shared logic, or **`existing` + `getSecret()`** that must not be evaluated unless a condition is true.

## Parameter files (`extends`)

```bicep
// main.shared.bicepparam
using none
param location = 'uksouth'
// PLACEHOLDER — supply via pipeline / CLI
param pipelinePrincipalId = ''
// SECRET — supply via pipeline / CLI
param sqlAdminPassword = ''
```

```bicep
// main.dev.bicepparam
using 'main.bicep'
extends './main.shared.bicepparam'
param environmentTag = 'dev'
```

- `environmentTag` is `dev` or `prd` (never `prod`). See [env-acronyms](../../../../.cursor/rules/env-acronyms.mdc).
- Never commit secrets, subscription IDs, or principal IDs. Declare empty `// SECRET` / `// PLACEHOLDER` values.
- Non-secret SKUs, public names, and tags belong in shared or leaf files.

## `prerequisitesExist` (do not fail by design)

Workloads that **read or assign into another part** take `param prerequisitesExist bool = false` (default false in `main.shared.bicepparam`).

| Workload | Always deploys | Gated when `prerequisitesExist` is false |
|----------|----------------|------------------------------------------|
| config, monitoring | All resources | n/a |
| storage | Images Storage Account | SQL server and database |
| api | Nothing (empty success) | Plan, site, MI role assignments, App Config / Insights settings |

Put `existing` Key Vault / App Configuration / Application Insights and `getSecret()` **inside a local module** that is itself `if (prerequisitesExist)`. An unconditional `existing` in `main.bicep` makes ARM resolve the resource even when unused, and the deploy fails.

Set `prerequisitesExist` in the Bicep param file (`main.shared.bicepparam` or a leaf). Workflows do not detect or override it. Comment next to the param in `main.bicep` lists what must exist before you flip it.

## Naming and length limits

Pattern for most resources: `<type>-<env>-dertinfo-<part>-<rgn>` with `env` = `dev` \| `prd` and `rgn` = `uks`.

| Constraint | Limit | House choice |
|------------|-------|----------------|
| Key Vault | 3–24 characters, hyphens allowed | `kv-<env>-dertinfo-uks` (19). `kv-<env>-dertinfo-config-uks` is 26 — too long. |
| Storage Account | 3–24, **no hyphens** | `stdevdertinfoimagesuks` / `stprddertinfoimagesuks` (purpose in the name; other accounts may be added later) |
| App Configuration | 5–50 | `appcs-<env>-dertinfo-config-uks` |
| Log Analytics | 4–63 | `log-<env>-dertinfo-monitoring-uks` |
| Application Insights | | `appi-<env>-dertinfo-monitoring-uks` |
| SQL server | 1–63, globally unique | `sql-<env>-dertinfo-storage-uks` |
| SQL database | | `sqldb-<env>-dertinfo-storage-uks` |
| App Service plan / site | | `plan-<env>-dertinfo-api-uks` / `app-<env>-dertinfo-api-uks` |

Do not use `prod` in names.

## Secrets

- Key Vault must set AVM `enableVaultForTemplateDeployment: true` so `getSecret()` works.
- SQL admin secrets are operator-seeded in Key Vault as `sql-dertinfo-storage-administrator-password` and `sql-dertinfo-storage-administrator-login` (storage SQL server, not a generic SQL login).
- Storage Bicep reads the password with `getSecret()`. Set `sqlAdministratorLogin` in the param file when you flip `prerequisitesExist` (same value as Key Vault `sql-dertinfo-storage-administrator-login`).
- The API uses `AZURE_APP_CONFIG` (store endpoint URI) and `DefaultAzureCredential` — not an App Configuration access key.

## Pinning AVM

Pin versions in `main.bicep` / local modules. Bump deliberately and rebuild with `az bicep build --file infra/bicep/<part>/main.bicep`. Do not commit compiled `main.json` (`infra/bicep/**/*.json` is gitignored).

Current pins (this workstream):

| Module | Version |
|--------|---------|
| `avm/res/key-vault/vault` | 0.14.0 |
| `avm/res/app-configuration/configuration-store` | 0.9.3 |
| `avm/res/operational-insights/workspace` | 0.16.1 |
| `avm/res/insights/component` | 0.8.0 |
| `avm/res/storage/storage-account` | 0.33.0 |
| `avm/res/sql/server` | 0.22.0 |
| `avm/res/web/serverfarm` | 0.7.0 |
| `avm/res/web/site` | 0.24.0 |
| `avm/res/authorization/role-assignment/rg-scope` | 0.1.1 |
| `avm/res/resources/resource-group` | 0.4.4 |

## Deploy locally (development)

Resource groups must already exist (subscription foundation). Example:

```bash
az deployment group create \
  --resource-group rg-dev-dertinfo-config-uks \
  --template-file infra/bicep/config/main.bicep \
  --parameters infra/bicep/config/main.dev.bicepparam
```

Storage/API: leave `prerequisitesExist` false until the prerequisites listed on that param in `main.bicep` exist, then set it true in the param file (and set `sqlAdministratorLogin` for storage).

Related: [CI/CD](../../infra/cicd.md), [configuration](../../infra/configuration.md), [Azure estate planned fix](../../../operations/planned-fixes/azure-estate-dev-prd.md).
