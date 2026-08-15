---
name: Agent-safe Azure subscription foundation
type: planned-fix
status: in-progress
updated: 2026-08-15
---

# Planned: Agent-safe Azure subscription foundation

**Status:** In progress — prerequisite before parked estate / workload Bicep work.

**Related:** [Azure estate (dev/prd)](azure-estate-dev-prd.md) (parked), [Bicep AVM + infra pipelines](bicep-avm-infra-pipelines.md) (parked), [Hosting and cost decisions](hosting-cost-decisions.md) (parked)

---

## Outcomes

- Empty Azure subscription is created by the **tenant administrator** (portal / billing).
- Subscription foundation Bicep is deployed via **GitHub Actions** at **subscription scope**, using a **privileged service principal** (OIDC) that may create RGs, policy, and role assignments.
- Subscription policy **denies** resource types and SKUs outside the DertInfo allow-list (SQL **Basic** only for now).
- Development resource groups exist with naming `rg-dev-dertinfo-<part>-uks`.
- Workload infra uses a **separate, more restricted** identity at **resource group** scope (`az deployment group create`).
- Agents do not hold subscription-scope deploy rights; they change infra only via PRs that run these pipelines.

## Steps (admin + repo)

1. Tenant admin creates an empty Azure subscription (portal / billing).
2. Create an Entra app / service principal for **subscription infra CD**; grant it rights on that subscription sufficient to deploy RGs, policy definitions/assignments, and role assignments (typically Contributor plus ability to assign roles, e.g. User Access Administrator, or a custom role).
3. Federate that SP to GitHub Actions OIDC on Environments **`development`** and **`production`**; set environment variables (see below).
4. Configure required reviewers on both Environments (see below).
5. Optionally create a second SP for **RG-scoped** workload deploys; set `AZURE_ENTRA_OIDC_PRINCIPALID_RG` so subscription Bicep can grant it Contributor on each RG.
6. Run [subscription-infra-cd.yml](../../../.github/workflows/subscription-infra-cd.yml) (`workflow_dispatch` or push to `main` under `infra/bicep/subscription/**`) to deploy `main.dev.bicepparam` / `main.prod.bicepparam`.
7. Verify: policy deny (disallowed type/SKU); RG workflow can deploy allowed resources with the restricted identity.
8. Unpark estate / workload Bicep planned fixes only after verification.

---

## Deploy channels

| Channel | Who | Scope | How |
|---------|-----|-------|-----|
| Create empty subscription | Tenant administrator | Billing / portal | Manual — not automated in this repo |
| Subscription foundation | Privileged **subscription** SP (OIDC) | Subscription | GitHub Actions → `az deployment sub create` |
| Workload infra | Restricted **RG** SP (OIDC) | Resource group | GitHub Actions → `az deployment group create` with leaf params + secret/placeholder overrides |

Agents author Bicep via PRs; they do not authenticate as the subscription SP locally for day-to-day work.

---

## GitHub Actions — subscription deploy

| Workflow | Role |
|----------|------|
| [`.github/workflows/subscription-infra-cd.yml`](../../../.github/workflows/subscription-infra-cd.yml) | Caller: `workflow_dispatch` (dev/prod) and push to `main` for subscription path changes (deploys **dev**) |
| [`.github/workflows/reusable-deploy-bicep-subscription.yml`](../../../.github/workflows/reusable-deploy-bicep-subscription.yml) | Reusable: OIDC login + `az deployment sub create` |

### Environment variables (per GitHub Environment)

| Variable | Purpose |
|----------|---------|
| `AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION` | App (client) id of the **subscription-scope** deploy SP |
| `AZURE_ENTRA_OIDC_TENANTID_SUBSCRIPTION` | Entra tenant id |
| `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID` | Target Azure subscription id |
| `AZURE_ENTRA_OIDC_PRINCIPALID_RG` | Optional — object id of the RG deploy SP (passed as `pipelinePrincipalId`) |

Federate the subscription SP using [`infra/configuration/github-azure-dev-credential.json`](../../../infra/configuration/github-azure-dev-credential.json) and [`github-azure-prd-credential.json`](../../../infra/configuration/github-azure-prd-credential.json) — see [GitHub Actions OIDC to Azure](../../technical/guides/github-azure-federated-credentials.md). App CD still uses Environments `test` / `prod` ([cicd.md](../../technical/infra/cicd.md)).

### Environment protection rules

Both GitHub Environments **`development`** and **`production`** require a reviewer before the subscription deploy job proceeds. Either of these GitHub accounts may approve:

- `dertinfo`
- `davidsmonkeys`

Configure under **Settings → Environments → [name] → Deployment protection rules → Required reviewers**.

RG-scoped reusable workflow remains [reusable-deploy-bicep-resourcegroup.yml](../../../.github/workflows/reusable-deploy-bicep-resourcegroup.yml) with its own identity vars (`AZURE_ENTRA_OIDC_CLIENTID`, etc.).

---

## Bicep params (standard)

- **`main.shared.bicepparam`** (`using none`) — shared non-secret defaults (location, allow-lists, workload parts, empty placeholders).
- **`main.dev.bicepparam` / `main.prod.bicepparam`** — `using 'main.bicep'` + `extends './main.shared.bicepparam'`; only env-specific overrides (e.g. `environmentTag`).
- Requires **Bicep CLI 0.44.1+** ([extendable parameter files](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-extend)).
- Secrets and identifiable values stay **empty placeholders**, overridden from GitHub Environment vars/secrets. Never commit real secrets.

---

## Policy allow-lists (initial)

**Types:** Static Web Apps, App Service plans/sites, SQL server/database, Storage accounts, Key Vault, App Configuration (plus minimal children required to deploy).

**SKUs:** SWA Free; App Service F1 / D1 / B1; SQL **Basic only**; Key Vault Standard; Storage Standard_LRS.

---

## Repo paths

- Subscription Bicep: [`infra/bicep/subscription/`](../../../infra/bicep/subscription/)
- Subscription CD: [subscription-infra-cd.yml](../../../.github/workflows/subscription-infra-cd.yml) + [reusable-deploy-bicep-subscription.yml](../../../.github/workflows/reusable-deploy-bicep-subscription.yml)
- RG CD (workload): [reusable-deploy-bicep-resourcegroup.yml](../../../.github/workflows/reusable-deploy-bicep-resourcegroup.yml)

---

## Checklist

- [x] Admin creates empty subscription
- [x] Subscription-scope SP created, RBAC on subscription, OIDC federated to GitHub Environments
- [x] Environment vars set (`AZURE_ENTRA_OIDC_*_SUBSCRIPTION`, subscription id)
- [x] Required reviewers on `development` and `production` (`dertinfo`, `davidsmonkeys`)
- [ ] Subscription CD workflow run succeeds for development
- [ ] Optional: RG SP principal id set; role assignments created by Bicep
- [ ] Policy deny verified (e.g. attempt disallowed SKU)
- [ ] RG deploy workflow wired with restricted identity
- [ ] Unpark related planned fixes when ready for workload Bicep
