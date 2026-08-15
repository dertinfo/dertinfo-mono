---
name: GitHub Azure federated credentials
type: guide
status: active
updated: 2026-08-15
---

# GitHub Actions OIDC to Azure (federated credentials)

How DertInfo lets **GitHub Actions** sign in to Azure **without a client secret**, using Entra ID federated identity credentials. The checked-in JSON in [`infra/configuration/`](../../../infra/configuration/) is the trust policy applied to the Entra app; it is not a password.

App CD still uses GitHub Environments **`test`** / **`prod`**. This guide and these JSON files are for the **new-stack** Environments **`development`** and **`production`** (subscription foundation and later workload infra). See [CI/CD](../infra/cicd.md) for the older app CD OIDC variables.

## What the files are

| File | Entra credential name | GitHub Environment it trusts |
|------|----------------------|------------------------------|
| [`github-azure-dev-credential.json`](../../../infra/configuration/github-azure-dev-credential.json) | `github-development` | `development` |
| [`github-azure-prd-credential.json`](../../../infra/configuration/github-azure-prd-credential.json) | `github-production` | `production` |

Each file is the body for `az ad app federated-credential create --parameters`. Fields:

| Field | Meaning |
|-------|---------|
| `issuer` | GitHub Actions OIDC issuer (`https://token.actions.githubusercontent.com`) |
| `subject` | Exact token subject GitHub will present. Must match `repo:dertinfo/dertinfo-mono:environment:<environment-name>` |
| `audiences` | Always `api://AzureADTokenExchange` for Azure login |
| `name` / `description` | Labels on the Entra federated credential |

The **subject** must match the job’s GitHub Environment **exactly**. `development` is not `dev`, `test`, or `prd`. A mismatch is a silent AADSTS70021 / federated credential error at `azure/login`.

These files contain **no secrets**. Do not put client secrets, certificates, tenant ids, or subscription ids in them.

## How GitHub uses them

1. A workflow job sets `environment: development` (or `production`) and `permissions: id-token: write`.
2. `azure/login` requests an OIDC token from GitHub. The token’s subject is `repo:dertinfo/dertinfo-mono:environment:development` (or `…:environment:production`).
3. Entra accepts that token **only if** a federated credential on the target app matches issuer + subject + audience.
4. Entra issues an Azure AD access token for that app’s service principal. GitHub never stores an Azure client secret.

That is what allows Actions to **deploy** (subscription Bicep, later RG Bicep, and similar) into Azure.

## Create the Entra apps and apply the JSON

**Preferred:** run the operator script (creates **separate** apps for development and production, applies the JSON above, and grants Contributor + User Access Administrator on each subscription only):

```powershell
cd infra\scripts
.\New-DertInfoSubscriptionOidcIdentities.ps1 `
  -DevSubscriptionId '<development-subscription-guid>' `
  -PrdSubscriptionId '<production-subscription-guid>'
```

See [`infra/scripts/README.md`](../../../infra/scripts/README.md). Run as tenant / subscription administrator from **PowerShell**. The script prints the GitHub Environment variable values to set next.

Do this as tenant administrator. **Required shape for subscription foundation:** **one app registration + service principal per Environment**, each with RBAC only on that Environment’s subscription. Do **not** use a single privileged SP for both development and production — a shared identity would hold rights on both subscriptions and enlarge blast radius on a public monorepo. Rationale: [GitHub workflows security review](../../operations/security/github-workflows-security-review.md).

Manual equivalent (if not using the script):

```bash
# Development app
APP_ID=$(az ad app create --display-name "dertinfo-github-subscription-development" --query appId -o tsv)
az ad sp create --id "$APP_ID"

az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters "@infra/configuration/github-azure-dev-credential.json"
```

```bash
# Production app
APP_ID=$(az ad app create --display-name "dertinfo-github-subscription-production" --query appId -o tsv)
az ad sp create --id "$APP_ID"

az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters "@infra/configuration/github-azure-prd-credential.json"
```

Then assign Contributor and User Access Administrator on the matching subscription to each service principal (see the script for the exact `az role assignment create` shape).

Do **not** attach both federated JSON files to one subscription-foundation SP. (A narrower, separate RG-deploy app may reuse the **same Environment’s** JSON only.)

Grant Azure RBAC on the **service principal** (not only the app registration): subscription-scope rights for [subscription infra CD](../../operations/planned-fixes/agent-safe-subscription-foundation.md); later, RG-scope Contributor for workload Bicep. Apply the **matching** environment JSON to every Entra app that Actions should impersonate for that Environment (for example a second, narrower RG-deploy app).

Confirm:

```bash
az ad app federated-credential list --id "$APP_ID" -o table
```

## GitHub Environment variables

Create GitHub Environments named **`development`** and **`production`**. On each, set variables (not secrets) for the app that Environment should login as:

| Variable | Purpose |
|----------|---------|
| `AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION` | App (client) id of the subscription-scope deploy identity |
| `AZURE_ENTRA_OIDC_TENANTID_SUBSCRIPTION` | Entra tenant id |
| `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID` | Target Azure subscription id |

Optional: `AZURE_ENTRA_OIDC_PRINCIPALID_RG` — object id of a separate RG-scoped identity for Bicep to grant Contributor.

Do not commit those ids in the JSON files or in `main.*.bicepparam`.

### Required reviewers

Both **`development`** and **`production`** use deployment protection rules with required reviewers. Either GitHub account may approve:

- `dertinfo`
- `davidsmonkeys`

Settings → Environments → [name] → Deployment protection rules → Required reviewers.

## Troubleshooting

### Git Bash (MINGW64) and subscription-scoped Azure CLI

On Git Bash, arguments that start with `/` (for example `--scope /subscriptions/<id>`) can be rewritten as Windows paths when launching `az`, which yields ARM errors such as `MissingSubscription`. Prefer PowerShell/CMD for those commands, or disable conversion:

```bash
MSYS_NO_PATHCONV=1 az role assignment create \
  --assignee "$SP_APP_ID" \
  --role "Contributor" \
  --scope "/subscriptions/$SUB_ID"
```

`echo /subscriptions/...` is not a reliable test — path conversion often applies only when invoking Windows executables.

## Related

- Operator scripts: [`New-DertInfoSubscriptionOidcIdentities.ps1`](../../../infra/scripts/New-DertInfoSubscriptionOidcIdentities.ps1) (create) and [`Remove-DertInfoSubscriptionOidcIdentity.ps1`](../../../infra/scripts/Remove-DertInfoSubscriptionOidcIdentity.ps1) (tear down by client id) — [`infra/scripts/README.md`](../../../infra/scripts/README.md)
- Security review: [GitHub workflows — subscription OIDC](../../operations/security/github-workflows-security-review.md)
- Folder README: [`infra/configuration/README.md`](../../../infra/configuration/README.md)
- [CI/CD](../infra/cicd.md) — workflow inventory and existing `test` / `prod` app CD OIDC
- [Agent-safe subscription foundation](../../operations/planned-fixes/agent-safe-subscription-foundation.md)
- Microsoft: [Connect GitHub and Azure with OpenID Connect](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect)
