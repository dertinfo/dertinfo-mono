# Infra scripts

Operator scripts for Azure / Entra setup that are not Bicep and not local secrets.

| Script | Purpose |
|--------|---------|
| [`New-DertInfoSubscriptionOidcIdentities.ps1`](New-DertInfoSubscriptionOidcIdentities.ps1) | Create **isolated** Entra apps + service principals for subscription-scope GitHub Actions OIDC (`development` and `production`), apply federated credentials from [`infra/configuration/`](../configuration/), and grant Contributor + User Access Administrator on each subscription |
| [`Remove-DertInfoSubscriptionOidcIdentity.ps1`](Remove-DertInfoSubscriptionOidcIdentity.ps1) | Tear down **one** identity by app (client) id: federated credentials, role assignments, service principal, then the app registration |

## Why two apps (not one)

Subscription foundation must not use a **shared** service principal across development and production. A shared identity would hold privileged RBAC on both subscriptions and increase blast radius on a **public** monorepo. The create script always provisions **two** apps and scopes each to one subscription. Threat model: [GitHub workflows security review](../../docs/operations/security/github-workflows-security-review.md).

## Prerequisites

- Azure CLI (`az`) installed and logged in (`az login`)
- Rights to create app registrations and assign roles on both target subscriptions
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

Related artefacts:

- Federated credential JSON: [`infra/configuration/`](../configuration/)
- Subscription foundation: [agent-safe-subscription-foundation.md](../../docs/operations/planned-fixes/agent-safe-subscription-foundation.md)
- Security review: [github-workflows-security-review.md](../../docs/operations/security/github-workflows-security-review.md)
