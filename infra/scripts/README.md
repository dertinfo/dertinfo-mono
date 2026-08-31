# Infra scripts

Operator scripts for Azure / Entra setup that are not Bicep and not local secrets.

| Script | Purpose |
|--------|---------|
| [`New-DertInfoSubscriptionOidcIdentities.ps1`](New-DertInfoSubscriptionOidcIdentities.ps1) | Create **isolated** Entra apps + service principals for subscription-scope GitHub Actions OIDC (`development` and `production`), apply federated credentials from [`infra/configuration/`](../configuration/), and grant Contributor + User Access Administrator on each subscription |
| [`Remove-DertInfoSubscriptionOidcIdentity.ps1`](Remove-DertInfoSubscriptionOidcIdentity.ps1) | Tear down **one** subscription-scope identity by app (client) id |
| [`New-DertInfoWorkloadOidcIdentity.ps1`](New-DertInfoWorkloadOidcIdentity.ps1) | Create **one** workload identity (one Environment + one part). No subscription RBAC |
| [`New-DertInfoWorkloadOidcIdentities.ps1`](New-DertInfoWorkloadOidcIdentities.ps1) | Master: create all workload identities for **one** Environment / subscription; prints copy-paste GitHub variables |
| [`Remove-DertInfoWorkloadOidcIdentity.ps1`](Remove-DertInfoWorkloadOidcIdentity.ps1) | Tear down **one** workload identity by app (client) id |
| [`Register-DertInfoResourceProviders.ps1`](Register-DertInfoResourceProviders.ps1) | Register workload resource providers (local / break-glass; keep in sync with the subscription CD reusable workflow) |
| [`New-DertInfoSqlEntraGroups.ps1`](New-DertInfoSqlEntraGroups.ps1) | **Before SQL:** create or reuse the two Entra groups; prints GitHub variable names |
| [`New-DertInfoSqlDbAccessUser.ps1`](New-DertInfoSqlDbAccessUser.ps1) | **After SQL:** bind the database access group as a database user (needs sqlcmd; you must be a SQL Entra admin) |
| [`New-DertInfoConfigKeyVaultSecrets.ps1`](New-DertInfoConfigKeyVaultSecrets.ps1) | **After config KV exists:** prompt once per Environment for the four hosted API secrets (skip existing unless `-Force`) |

## Why two subscription apps (not one)

Subscription foundation must not use a **shared** service principal across development and production. A shared identity would hold privileged RBAC on both subscriptions and increase blast radius on a **public** monorepo. The create script always provisions **two** apps and scopes each to one subscription. Threat model: [GitHub workflows security review](../../docs/operations/security/github-workflows-security-review.md).

Workload identities are **one app per part per Environment** (`dertinfo-github-workload-<part>-development` / `-production`). They must not use the subscription SP (that identity has User Access Administrator on the subscription).

## Prerequisites

- Azure CLI (`az`) installed and logged in (`az login`)
- Rights to create app registrations (and, for the subscription script, assign roles on the target subscription)
- Run from **PowerShell** (avoids Git Bash `/subscriptions/...` path rewriting)

## Subscription OIDC identities

Full context: [GitHub Actions OIDC to Azure (federated credentials)](../../docs/technical/guides/github-azure-federated-credentials.md).

```powershell
cd C:\Projects\Cursor\DertInfo\infra\scripts

.\New-DertInfoSubscriptionOidcIdentities.ps1 `
  -DevSubscriptionId '<development-subscription-guid>' `
  -PrdSubscriptionId '<production-subscription-guid>'
```

The script prints the GitHub Environment **variables** to set on **`development`** and **`production`**. It does **not** remove older shared app registrations — use [`Remove-DertInfoSubscriptionOidcIdentity.ps1`](Remove-DertInfoSubscriptionOidcIdentity.ps1) after Environments point at the new client ids:

```powershell
.\Remove-DertInfoSubscriptionOidcIdentity.ps1 `
  -ClientId '<app-registration-client-id>'
```

The remove script prints display name, federated credential names, and each role/scope, then prompts for confirmation. Pass `-Force` to skip the prompt. Missing federated credentials, the service principal, or the app are skipped (safe to re-run).

## Workload OIDC identities

Run the **master** script once per Environment. It calls [`New-DertInfoWorkloadOidcIdentity.ps1`](New-DertInfoWorkloadOidcIdentity.ps1) for `config`, `monitoring`, `storage`, `api`, `web`, `app`, and `functions`.

```powershell
.\New-DertInfoWorkloadOidcIdentities.ps1 `
  -GitHubEnvironment development `
  -SubscriptionId '<development-subscription-guid>'

.\New-DertInfoWorkloadOidcIdentities.ps1 `
  -GitHubEnvironment production `
  -SubscriptionId '<production-subscription-guid>'
```

Copy the printed `AZURE_ENTRA_OIDC_*` names and values onto that GitHub Environment. Then re-run subscription infra CD so Bicep can grant each SP Contributor on its RG (plus the extra RG-scoped roles API needs).

Tear down one workload app:

```powershell
.\Remove-DertInfoWorkloadOidcIdentity.ps1 `
  -ClientId '<app-registration-client-id>'
```

## Entra-only Azure SQL groups

```powershell
.\New-DertInfoSqlEntraGroups.ps1 -GitHubEnvironment development
```

Run this **before** SQL exists. It only creates or reuses the two groups. Paste `AZURE_ENTRA_SQL_ADMIN_GROUP_NAME` and `AZURE_ENTRA_SQL_ADMIN_GROUP_OBJECTID` onto that GitHub Environment. Flip storage `prerequisitesExist`, re-run storage infra CD, then bind the access group to the database:

```powershell
.\New-DertInfoSqlDbAccessUser.ps1 -GitHubEnvironment development
```

Add operators and the App Service MI to the Entra groups later (portal or `az ad group member add`).

## Hosted API Key Vault secrets

After config infra CD has created the vault, an administrator sets the four secret **values** once:

```powershell
.\New-DertInfoConfigKeyVaultSecrets.ps1 -GitHubEnvironment development
```

The script prompts for each value (not echoed) and skips names that already exist unless `-Force`. Config Bicep deploys App Configuration Key Vault **references** to these names; it does not store the values.

Related artefacts:

- Federated credential JSON: [`infra/configuration/`](../configuration/)
- Subscription foundation: [agent-safe-subscription-foundation.md](../../docs/operations/planned-fixes/agent-safe-subscription-foundation.md)
- Security review: [github-workflows-security-review.md](../../docs/operations/security/github-workflows-security-review.md)
