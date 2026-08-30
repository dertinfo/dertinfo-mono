---
name: Agent-safe Azure subscription foundation
type: planned-fix
status: in-progress
updated: 2026-08-15
---

# Planned: Agent-safe Azure subscription foundation

**Status:** In progress — **development** subscription foundation deployed via Actions. Production subscription exists; first prod CD pending SP RBAC confirmation. Prerequisite before parked estate / workload Bicep work.

**Related:** [Azure estate (dev/prd)](azure-estate-dev-prd.md) (parked), [Bicep AVM + infra pipelines](bicep-avm-infra-pipelines.md) (parked), [Hosting and cost decisions](hosting-cost-decisions.md) (parked), [GitHub workflows security review](../security/github-workflows-security-review.md)

---

## Outcomes

- Empty Azure subscriptions are created by the **tenant administrator** (portal / billing) — Development and Production.
- Subscription foundation Bicep is deployed via **GitHub Actions** at **subscription scope**, using a **privileged service principal per Environment** (OIDC) that may create RGs, policy, and role assignments, and that registers the resource providers workload CD needs (RG-scoped identities cannot).
- **Isolation:** development and production use **separate** Entra apps/SPs; each has RBAC only on its own subscription (see [why](#identity-isolation)).
- Subscription policy **denies** SKUs outside the DertInfo allow-list (SQL **Basic** and App Service F1/D1/B1).
- Development resource groups exist with naming `rg-dev-dertinfo-<part>-uks`.
- Workload infra uses a **separate, more restricted** identity at **resource group** scope (`az deployment group create`).
- Agents do not hold subscription-scope deploy rights; they change infra only via PRs that run these pipelines.
- Environment required reviewers (`dertinfo` / `davidsmonkeys`) must approve before deploy jobs obtain Azure tokens.

## Steps (admin + repo)

1. Tenant admin creates empty Azure subscription(s) (portal / billing).
2. Create **two** Entra apps / service principals for **subscription infra CD** (development and production) via [`New-DertInfoSubscriptionOidcIdentities.ps1`](../../../infra/scripts/New-DertInfoSubscriptionOidcIdentities.ps1); each gets Contributor + User Access Administrator on **its** subscription only.
3. Apply federated credentials (script uses [`infra/configuration/`](../../../infra/configuration/)); set GitHub Environment variables (see below).
4. Configure required reviewers on both Environments (see below).
5. Create **per-workload** Entra apps via [`New-DertInfoWorkloadOidcIdentities.ps1`](../../../infra/scripts/New-DertInfoWorkloadOidcIdentities.ps1) (one Environment at a time). Paste `AZURE_ENTRA_OIDC_CLIENTID_WORKLOAD_*` and `AZURE_ENTRA_OIDC_PRINCIPALID_WORKLOAD_*` onto that GitHub Environment. Do **not** use the subscription SP for workload deploys.
6. Run [subscription-infra-cd.yml](../../../.github/workflows/subscription-infra-cd.yml) (push to `main` under `infra/bicep/subscription/**` or `workflow_dispatch` with target `full`). That pipeline **registers `resourceProvidersToRegister` on the subscription**, then deploys `main.dev.bicepparam` and, upon review approval, `main.prod.bicepparam` (or target `dev-only` to deploy only to development). See [CI/CD](../../technical/infra/cicd.md).
7. Verify: policy deny (disallowed SKU); RG workflow can deploy allowed resources with the restricted identity.
8. Unpark estate / workload Bicep planned fixes only after verification.

---

## Identity isolation

A **shared** app registration for both Environments would typically need RBAC on **both** subscriptions. On a **public** monorepo, that increases blast radius if GitHub write access or an approval account is abused.

**Required pattern:** one app + SP per Environment:

| GitHub Environment | Entra app (example display name) | Azure RBAC scope |
|--------------------|----------------------------------|------------------|
| `development` | `dertinfo-github-subscription-development` | DertInfo Development subscription only |
| `production` | `dertinfo-github-subscription-production` | DertInfo Production subscription only |

Create with the operator script; remove a retired client id with [`Remove-DertInfoSubscriptionOidcIdentity.ps1`](../../../infra/scripts/Remove-DertInfoSubscriptionOidcIdentity.ps1). Do **not** attach both federated JSON files to one privileged SP for this foundation.

Threat model detail: [github-workflows-security-review.md](../security/github-workflows-security-review.md).

---

## Deploy channels

| Channel | Who | Scope | How |
|---------|-----|-------|-----|
| Create empty subscription | Tenant administrator | Billing / portal | Manual — not automated in this repo |
| Subscription foundation | Privileged **per-Environment** subscription SP (OIDC) | That Environment’s subscription | GitHub Actions → register `resourceProvidersToRegister` then `az deployment sub create` |
| Workload infra / Src CD | Per-workload SP (OIDC) | That part’s resource group (Contributor). Storage also gets **Reader** + **Key Vault Secrets User** on the config RG. API also gets **Reader** on config and monitoring, plus **conditioned** UAA on the config RG only (may assign App Configuration Data Reader and Key Vault Secrets User only) | GitHub Actions → `az deployment group create` or zip deploy; identity is `AZURE_ENTRA_OIDC_CLIENTID_WORKLOAD_<PART>` |

Agents author Bicep via PRs; they do not authenticate as the subscription SP locally for day-to-day work.

---

## GitHub Actions — subscription deploy

| Workflow | Role |
|----------|------|
| [`.github/workflows/subscription-infra-cd.yml`](../../../.github/workflows/subscription-infra-cd.yml) | Caller: `push` to `main` (auto dev $\rightarrow$ gated prod) and `workflow_dispatch` (target `full` or `dev-only`) |
| [`.github/workflows/reusable-infra-deploy-bicep-subscription.yml`](../../../.github/workflows/reusable-infra-deploy-bicep-subscription.yml) | Reusable: OIDC login + `az deployment sub create` |

### Environment variables (per GitHub Environment)

Each Environment must use **its own** client id (and matching subscription id):

| Variable | Purpose |
|----------|---------|
| `AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION` | App (client) id of **that Environment’s** subscription-scope deploy SP |
| `AZURE_ENTRA_OIDC_TENANTID` | Entra tenant id |
| `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID` | Target Azure subscription id for that Environment |
| `AZURE_ENTRA_OIDC_CLIENTID_WORKLOAD_<PART>` | App (client) id of that workload SP (infra and Src CD) |
| `AZURE_ENTRA_OIDC_PRINCIPALID_WORKLOAD_<PART>` | Object id of that workload SP (subscription Bicep RG RBAC) |

Federate using [`infra/configuration/github-azure-dev-credential.json`](../../../infra/configuration/github-azure-dev-credential.json) and [`github-azure-prd-credential.json`](../../../infra/configuration/github-azure-prd-credential.json) — see [GitHub Actions OIDC to Azure](../../technical/guides/github-azure-federated-credentials.md). Preferred create path: [`infra/scripts/`](../../../infra/scripts/). App CD still uses Environments `test` / `prod` ([cicd.md](../../technical/infra/cicd.md)).

### Environment protection rules

Both GitHub Environments **`development`** and **`production`** require a reviewer before the subscription deploy job proceeds. Either of these GitHub accounts may approve:

- `dertinfo`
- `davidsmonkeys`

Configure under **Settings → Environments → [name] → Deployment protection rules → Required reviewers**.

RG-scoped reusable workflow is [reusable-infra-deploy-bicep-resourcegroup.yml](../../../.github/workflows/reusable-infra-deploy-bicep-resourcegroup.yml). Callers pass `azure_oidc_workload` (`CONFIG`, `API`, …); the reusable reads `vars.AZURE_ENTRA_OIDC_CLIENTID_WORKLOAD_<PART>` after the GitHub Environment is applied.

---

## Bicep params (standard)

- **`main.shared.bicepparam`** (`using none`) — shared non-secret defaults (location, allow-lists, workload parts, empty placeholders).
- **`main.dev.bicepparam` / `main.prod.bicepparam`** — `using 'main.bicep'` + `extends './main.shared.bicepparam'`; only env-specific overrides (e.g. `environmentTag`).
- Requires **Bicep CLI 0.44.1+** ([extendable parameter files](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-extend)).
- Secrets and identifiable values stay **empty placeholders**, overridden from GitHub Environment vars/secrets. Never commit real secrets.

---

## Policy allow-lists (initial)

**Types:** Static Web Apps, App Service plans/sites, SQL server/database, Storage accounts, Key Vault (including secrets), App Configuration, Application Insights, Log Analytics (plus children required to deploy).

**SKUs:** SWA Free; App Service F1 / D1 / B1; SQL **Basic only**; Key Vault Standard; Storage Standard_LRS; App Configuration Free.

---

## Repo paths

- Subscription Bicep: [`infra/bicep/subscription/`](../../../infra/bicep/subscription/)
- Subscription CD: [subscription-infra-cd.yml](../../../.github/workflows/subscription-infra-cd.yml) + [reusable-infra-deploy-bicep-subscription.yml](../../../.github/workflows/reusable-infra-deploy-bicep-subscription.yml)
- Operator scripts: [`infra/scripts/`](../../../infra/scripts/)
- RG CD (workload): [reusable-infra-deploy-bicep-resourcegroup.yml](../../../.github/workflows/reusable-infra-deploy-bicep-resourcegroup.yml)

---

## Checklist

- [x] Admin creates empty subscription(s) (Development; Production)
- [x] Isolated subscription-scope SPs created, RBAC per subscription, OIDC federated to GitHub Environments
- [x] Environment vars set (`AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION`, `AZURE_ENTRA_OIDC_TENANTID`, subscription id) per Environment
- [x] Required reviewers on `development` and `production` (`dertinfo`, `davidsmonkeys`)
- [x] Subscription CD workflow run succeeds for **development**
- [ ] Production SP RBAC confirmed; Subscription CD succeeds for **production**
- [ ] Workload identities created; `AZURE_ENTRA_OIDC_CLIENTID_WORKLOAD_*` / `PRINCIPALID_WORKLOAD_*` pasted; subscription CD grants RG RBAC
- [ ] Policy deny verified (e.g. attempt disallowed SKU)
- [x] Workload infra CD wired to per-part client ids (not the subscription SP)
- [ ] Unpark related planned fixes when ready for remaining estate work
