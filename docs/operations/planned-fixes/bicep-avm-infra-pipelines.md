# Planned: Bicep (AVM) and infrastructure pipelines

**Status:** Parked — blocked on subscription foundation ([agent-safe-subscription-foundation.md](agent-safe-subscription-foundation.md)). **Needs further refinement** before implementation. Execute separately from estate RG creation and from app source CD.

**Related:** [Azure estate (dev/prd)](azure-estate-dev-prd.md), [Hosting and cost decisions](hosting-cost-decisions.md), existing Functions reference [`apps/dert-functions/infra/bicep/main.bicep`](../../../apps/dert-functions/infra/bicep/main.bicep)

---

## Intent

1. Author **Bicep** for the new-stack workloads using **Azure Verified Modules (AVM)** and a repeatable folder/module pattern.
2. Checked-in **`main.shared.bicepparam`** (`using none`) plus **`main.dev.bicepparam` / `main.prod.bicepparam`** (`extends` shared) hold **non-secret** values so contributors and pipelines avoid duplicating allow-lists and defaults; pipelines still override **secrets and identifiable placeholders**.
3. Add **infrastructure CD pipelines** that deploy those templates into the matching resource groups (RG scope — see estate / subscription foundation planned fixes).

Conventions for Bicep (naming inside resources, parameter style, secrets handling, pipeline triggers) will be **refined in this workstream** and then expanded into durable technical docs.

**Functions:** leave existing Bicep alone; align naming/`dev`/`prd` in a later pass.

---

## Workloads and paths

| Part | Path | Resources |
|------|------|-----------|
| config | [`infra/bicep/config/`](../../../infra/bicep/config/) | Key Vault, App Configuration |
| monitoring | [`infra/bicep/monitoring/`](../../../infra/bicep/monitoring/) | Log Analytics (1 GB/day cap), Application Insights |
| storage | [`infra/bicep/storage/`](../../../infra/bicep/storage/) | Images Storage Account, Azure SQL (gated) |
| api | [`infra/bicep/api/`](../../../infra/bicep/api/) | App Service Plan + Web App (Windows / `win-x86` CD) |
| web | (later) | Static Web App |
| app | (later) | Static Web App |

Target RGs: `rg-<env>-dertinfo-<part>-uks` — see [azure-estate-dev-prd.md](azure-estate-dev-prd.md).

Suggested deploy order: **config + monitoring → storage → api → web / app**. Storage and API keep `prerequisitesExist` false until you flip it in the param file after those parts exist.

---

## Bicep structure (baseline)

Per workload:

```
infra/bicep/
  main.bicep
  main.shared.bicepparam        # using none — shared non-secret defaults
  main.dev.bicepparam      # using main.bicep + extends main.shared — env overrides
  main.prod.bicepparam      # using main.bicep + extends main.shared — env overrides
  <module-name>/
    main.bicep             # local module only when it adds real composition
```

Use **extendable Bicep parameter files** (Bicep CLI **0.44.1+**), not JSON and not duplicated full copies of every value in each env file.

1. **`main.shared.bicepparam`** — `using none`. Shared non-secret defaults (location, product slug, allow-lists, workload parts, and similar).
2. **`main.dev.bicepparam` / `main.prod.bicepparam`** — `using 'main.bicep'` and `extends './main.shared.bicepparam'`. Set only what differs for that environment (e.g. `environmentTag`, env-specific SKUs). Redeclare a param to override; use `base` + spread when merging objects/arrays.
3. **Pipeline / CLI** — same deploy process for every environment: pick the leaf param file, then override **secrets and identifiable placeholders** from GitHub Environment secrets/vars (or local secure input).

Do not commit secrets, subscription IDs, principal IDs, or other identifiable estate detail. Declare those as empty placeholders with `// SECRET` or `// PLACEHOLDER` in `main.shared.bicepparam` and/or the leaf (so the contract is visible).

Example (development):

```bash
az deployment group create \
  --resource-group rg-dev-dertinfo-api-uks \
  --template-file main.bicep \
  --parameters main.dev.bicepparam \
  --parameters sqlAdminPassword=$SQL_ADMIN_PASSWORD
```

Example (production):

```bash
az deployment group create \
  --resource-group rg-prd-dertinfo-api-uks \
  --template-file main.bicep \
  --parameters main.prod.bicepparam \
  --parameters sqlAdminPassword=$SQL_ADMIN_PASSWORD
```

**Completeness:** every parameter declared in `main.bicep` that **does not have a default** must appear in the resolved param set (shared and/or leaf). Prefer putting shared values in `main.shared.bicepparam` once.

**Secrets and identifiable values:** still **declare** them (usually in `main.shared.bicepparam`) with an **empty value** and a comment. Pipelines inject real values the same way for **dev** and **prd**.

Example shape:

```bicep
// main.shared.bicepparam
using none

param location = 'uksouth'
param productSlug = 'dertinfo'
// PLACEHOLDER — supply via pipeline / CLI override
param pipelinePrincipalId = ''
// SECRET — supply via pipeline / CLI override
param sqlAdminPassword = ''
```

```bicep
// main.dev.bicepparam
using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param appServiceSku = 'F1'
```

```bicep
// main.prod.bicepparam
using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param appServiceSku = 'D1'
```

Docs: [Extendable parameter files](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-extend).

### Parent `main.bicep` sections

Use this section order:

1. Parameters  
2. Variables  
3. References  
4. Resources  
5. Modules  
6. **AVM Modules**  
7. Outputs  

**Modules vs AVM Modules**

- **AVM Modules** — call Azure Verified Modules (`br/public:avm/...`) **directly** from `main.bicep`. Do **not** add a local subfolder whose only job is to wrap a single AVM call.
- **Modules** — local `.bicep` modules only when they add real composition (multiple resources, shared logic, or non-AVM resources that are reused). Pure pass-through wrappers are not wanted.

Prefer AVM wherever a published module exists; use raw `Microsoft.*` in **Resources** only when AVM cannot express a required setting (e.g. custom `policyDefinitions`). Pin AVM **versions** explicitly.

Same `main.bicep` for **dev** and **prd**; shared non-secrets live in `main.shared.bicepparam`; env differences in leaf files via `extends`. Pipelines override secrets/placeholders only.

### Development-first defaults (local → Azure)

Checked-in Bicep + `main.dev.bicepparam` (extending `main.shared.bicepparam`) should let a contributor deploy from their machine to Azure and get a working **cloud development** estate without a private param pack.

- Put **safe, non-secret** shared configuration in `main.shared.bicepparam`; put env-specific non-secrets in the leaf (`environmentTag`, env SKUs, and similar).
- A local deploy using `main.dev.bicepparam` (plus any required secure CLI overrides) should be enough to **create the development workloads** in the cloud.
- **Production** uses `main.prod.bicepparam` (same `extends` chain). The infra pipeline supplies secrets and identifiable placeholders from GitHub Secrets / Environment values.
- This does **not** override the public-repo rule below: development convenience must not mean embedding secrets or overexposing deployment-scope / estate details that create a security risk.

### AVM usage (indicative)

Call these (and similar) from the **AVM Modules** section of `main.bicep`, not via empty local wrappers:

| Concern | AVM (indicative) |
|---------|------------------|
| Resource group | `br/public:avm/res/resources/resource-group` |
| Policy assignment | `br/public:avm/res/authorization/policy-assignment/sub-scope` (or rg/mg scope) |
| Role assignment | `br/public:avm/res/authorization/role-assignment/rg-scope` |
| Key Vault | `br/public:avm/res/key-vault/vault` |
| App Configuration | `br/public:avm/res/app-configuration/configuration-store` |
| Storage account | `br/public:avm/res/storage/storage-account` |
| SQL server | `br/public:avm/res/sql/server` |
| Static Web App | `br/public:avm/res/web/static-site` |
| App Service plan | `br/public:avm/res/web/serverfarm` |
| Web app | `br/public:avm/res/web/site` |

### Public repository — no secrets in checked-in params

This monorepo is **public**. Checked-in `main.bicep`, `main.shared.bicepparam`, and leaf `main.*.bicepparam` files must contain **no secrets** (passwords, connection strings, deploy tokens, client secrets, private keys, etc.) and **no identifiable estate detail** (exact subscription IDs, principal IDs, and similar).

- Prefer `@secure()` on secret parameters in `main.bicep`. In `main.shared.bicepparam` (and/or leaves), still list every required secret/identifiable param with an **empty value** and a `// SECRET` or `// PLACEHOLDER` comment — never a real password, token, or id.
- SQL admin passwords and similar are supplied at deploy via **pipeline overrides** from GitHub Secrets (same mechanism for dev and prd) or local secure CLI input.
- Where a value is only needed at deploy and would enlarge attack surface if published, leave it empty and inject via pipeline override — refine the exact list during implementation so **development remains locally deployable** without leaking sensitive estate detail.

Non-secret SKUs, public naming patterns, and tags are fine in `main.shared.bicepparam` and env leaves.

---

## Cost-aligned default SKUs (parameters)

Align with [hosting-cost-decisions.md](hosting-cost-decisions.md):

| Resource | `dev` | `prd` (off-season) |
|----------|-------|---------------------|
| Static Web App | Free | Free |
| App Service plan | F1 | D1 |
| Azure SQL database | Basic | Basic |
| Key Vault | Standard | Standard |
| Functions (existing) | Y1 | Y1 |

Put shared SKU allow-lists in `main.shared.bicepparam`; put env-specific App Service SKUs (F1 vs D1) in the leaf files. Competition-weekend **B1** (API) and optional SQL **S0** remain **operational** SKU changes via pipeline/CLI override (or temporary policy expansion), not a third committed param file.

---

## Infrastructure pipelines (to design in this workstream)

Goals (detail TBD while refining):

- **Same process for `dev` and `prd`:** deploy `main.bicep` + the matching leaf `main.<env>.bicepparam` (which `extends` `main.shared.bicepparam`), then override **secrets and identifiable placeholders** from GitHub Environment secrets / vars
- Do **not** use the pipeline to restate every shared value already in `main.shared.bicepparam`
- Pass withheld deploy-time values only via GitHub Environment secrets / OIDC — not from the public repo
- Use OIDC / least-privilege identities scoped to the target RG where possible ([estate planned fix](azure-estate-dev-prd.md); [subscription foundation](agent-safe-subscription-foundation.md))
- Separate **infra CD** from existing **app source CD** (zip/SWA deploy) unless a deliberate merge is chosen later
- What-if / validation before apply where practical
- Do not wire production cutover until new stack is validated

---

## Refinement backlog (before or during implementation)

- [x] `main.shared.bicepparam` + `extends` in `main.dev.bicepparam` / `main.prod.bicepparam` (Bicep 0.44.1+); secrets/identifiable values as empty placeholders overridden by pipelines
- [ ] Confirm which values live in `main.shared.bicepparam` vs each leaf
- [ ] Enforce: every non-defaulted `main.bicep` param appears in the resolved shared/leaf set; secrets/identifiable values use empty value + `// SECRET` / `// PLACEHOLDER` comment
- [ ] List which non-secret-but-sensitive values stay empty and how **local** and pipeline overrides supply them
- [ ] Agree tags, output contracts, and `@secure()` parameter patterns
- [ ] Confirm SQL server vs database module split and admin secret flow (pipeline override / Key Vault only)
- [ ] Confirm whether API Bicep sets `AZURE_APP_CONFIG` / references config+storage by name or leaves wiring to App Config alone
- [ ] Choose infra pipeline layout (reusable workflow vs per-workload; triggers on `infra/bicep/**`)
- [ ] Pin AVM versions and document how to bump them
- [ ] Decide image blob container creation in Bicep vs first-run app behaviour
- [ ] Document the “local → Azure development” deploy steps in durable technical docs when implementing

---

## Checklist (when ready to execute)

- [ ] Estate RGs exist (or create them as part of first infra deploy runbook)
- [ ] Implement Bicep per workload with AVM wrappers
- [ ] `bicep build` / what-if against `dev`
- [ ] Add infra CD pipelines for `dev`, then `prd`
- [ ] Expand durable docs (Bicep conventions, infra CD) under `docs/technical/` when done
- [ ] Leave Functions Bicep alignment as a follow-up

---

## Out of scope for this planned fix

- Creating empty RGs only (see [azure-estate-dev-prd.md](azure-estate-dev-prd.md))
- Publishing the hosting/cost architecture page
- Migrating traffic from current test/live hosts
- Changing application code
