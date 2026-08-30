# Planned: Azure estate layout (development and production)

**Status:** Parked — blocked on subscription foundation ([agent-safe-subscription-foundation.md](agent-safe-subscription-foundation.md)).

**Related:** [Bicep AVM + infra pipelines](bicep-avm-infra-pipelines.md), [Hosting and cost decisions](hosting-cost-decisions.md)

---

## Intent

Define the **new Azure stack** for cloud **development** (`dev`) and **production** (`prd`): resource group naming, deployment scope, and permission boundaries. Existing test/live resources remain until a deliberate cutover.

Durable technical docs (e.g. under `docs/technical/infra/`) should be written **when this work is completed**, not as a precondition.

---

## Environments

| Concept | Tag / code | Azure? |
|---------|------------|--------|
| Developer machine | **local** | No — native / Compose |
| Non-prod cloud (today “test” / “staging”) | **development** / **`dev`** | Yes — new stack |
| Production | **production** / **`prd`** | Yes — new stack |

GitHub Environment renames (`test` → `development`) and wider doc renames are **out of scope** for this planned fix; handle with the CI/CD future phase / separate work.

---

## Resource group naming

**Pattern:** `rg-<env>-dertinfo-<part>-<rgn>`

| Segment | Values | Meaning |
|---------|--------|---------|
| `env` | `dev` \| `prd` | development / production |
| `dertinfo` | fixed | product slug |
| `part` | `config` \| `storage` \| `web` \| `app` \| `api` \| `monitoring` | workload unit (Functions aligned later) |
| `rgn` | region **TLA** | Short code for Azure region |

### Region TLAs

| TLA | Azure `location` |
|-----|------------------|
| `uks` | `uksouth` |
| `eus` | `eastus` |
| `neu` | `northeurope` |

This stack targets **UK South** → suffix **`uks`**, location **`uksouth`**.

### Resource groups to create

| Part | development | production |
|------|-------------|------------|
| config | `rg-dev-dertinfo-config-uks` | `rg-prd-dertinfo-config-uks` |
| storage | `rg-dev-dertinfo-storage-uks` | `rg-prd-dertinfo-storage-uks` |
| web | `rg-dev-dertinfo-web-uks` | `rg-prd-dertinfo-web-uks` |
| app | `rg-dev-dertinfo-app-uks` | `rg-prd-dertinfo-app-uks` |
| api | `rg-dev-dertinfo-api-uks` | `rg-prd-dertinfo-api-uks` |
| monitoring | `rg-dev-dertinfo-monitoring-uks` | `rg-prd-dertinfo-monitoring-uks` |

**Intended contents per part** (provisioned later via Bicep — see related planned fix):

| Part | Contents |
|------|----------|
| `config` | Key Vault + App Configuration ([`infra/bicep/config/`](../../../infra/bicep/config/)) |
| `storage` | Images Storage Account + Azure SQL ([`infra/bicep/storage/`](../../../infra/bicep/storage/)) |
| `web` | Static Web App (Bicep later) |
| `app` | Static Web App (Bicep later) |
| `api` | App Service Plan + Web App ([`infra/bicep/api/`](../../../infra/bicep/api/)) |
| `monitoring` | Log Analytics + Application Insights ([`infra/bicep/monitoring/`](../../../infra/bicep/monitoring/)) |

Functions keep current Bicep/RG naming until a separate alignment pass.

---

## Deployment scope

- **Unit of deploy:** one **resource group**.
- Deployments use **resource group scope** (e.g. `az deployment group create --resource-group …`).
- One workload part → one RG → one deploy target.
- Do **not** use subscription-wide deployments for these app/config/storage units (unless a future shared bootstrap explicitly needs it).

---

## Permission boundaries (including agentic work)

Structuring by **one RG per part** is intentional:

1. **Least privilege for humans and CI** — Contributor (or tighter) on a single RG, not the whole subscription.
2. **Agentic work** — an agent identity can be scoped to **one resource group** so it may create/update/delete resources only inside that boundary while iterating on that workload’s infra.
3. **Blast radius** — a bad deploy or agent mistake is limited to that part’s RG (e.g. web cannot alter SQL in `storage`).

When executing this work, create matching identities (e.g. Entra app / managed identity / federated GitHub OIDC) and assign RBAC **at the RG**, not at subscription, for day-to-day infra changes.

---

## Checklist (when ready to execute)

- [ ] Create the twelve resource groups (six parts × `dev` / `prd`) in the appropriate subscriptions, location `uksouth`
- [ ] Tag RGs consistently (`environment`: `dev` \| `prd`, product, part)
- [ ] Define and assign **RG-scoped** identities for CI and for agentic infra work (per part or per env as decided during execution)
- [ ] Document the live estate (names, subscriptions, identities) under `docs/technical/infra/` when done
- [ ] Do **not** migrate or delete existing test/live RGs in this step unless explicitly planned
- [ ] Hand off to [Bicep AVM + infra pipelines](bicep-avm-infra-pipelines.md) for resource provisioning inside these RGs

---

## Out of scope for this planned fix

- Authoring or deploying Bicep
- Infra or app CD pipeline wiring
- Functions RG rename
- Writing the durable hosting/cost architecture page (see [hosting-cost-decisions.md](hosting-cost-decisions.md))
