# API App Service quota block (UK South F1 / D1)

## Summary of the work completed

New-stack **API infra CD** is parked. Development App Service (`plan-dev-dertinfo-api-uks`, SKU **F1**, UK South) failed to deploy with a **quota** error. A support ticket was raised on the **DertInfo development** subscription for an F1 increase (requested as F1 under App Service / UK South; the resource is an App Service plan, not a Compute VM).

The **DertInfo production** subscription had workload resource providers registered locally with [`Register-DertInfoResourceProviders.ps1`](../../../infra/scripts/Register-DertInfoResourceProviders.ps1) so App Service quotas were visible. A second support ticket was raised there for **D1** in UK South (`appServiceSku` in [`infra/bicep/api/main.prod.bicepparam`](../../../infra/bicep/api/main.prod.bicepparam)).

No API App Service plan or site was created. Config, storage, and catalog/secrets work can continue independently; hosted API deploys cannot until the development F1 quota is granted.

## Why the work was completed

The portal **Compute** quota list was empty because this stack does not register `Microsoft.Compute`. App Service uses **`Microsoft.Web`**. Empty Compute quotas were a red herring. Development is blocked on Free **F1** capacity in UK South; production D1 was requested now so PRD is not blocked the same way later.

This entry exists so the next session can resume from a known stop: re-run API infra CD after the tickets complete, rather than rediscovering the quota path.

## Date the work was started

2026-08-31

## Date the work was completed

2026-08-31 (parked — quota tickets outstanding)

## Issues that were encountered on the way

- ARM reported quota; **Usage + quotas → Compute** showed nothing. F1/D1 are **App Service plan** SKUs (`Microsoft.Web/serverfarms`), not VM sizes. Check **App Service / Web** quotas for **UK South**.
- Workload identities cannot register providers. Production needed [`Register-DertInfoResourceProviders.ps1`](../../../infra/scripts/Register-DertInfoResourceProviders.ps1) (same list as subscription infra CD) before D1 quota was visible to request.
- Subscription CD also registers `Microsoft.Web` (and the rest of the workload list) before `az deployment sub create`. Local register was used for production so the D1 ticket did not wait on a CD run.

## References to any best practices that we found

- [Azure subscription and service limits (App Service)](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits#app-service-limits)
- [Register resource providers](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-providers-and-types)
- Providers list: [`Register-DertInfoResourceProviders.ps1`](../../../infra/scripts/Register-DertInfoResourceProviders.ps1) / [`reusable-infra-deploy-bicep-subscription.yml`](../../../.github/workflows/reusable-infra-deploy-bicep-subscription.yml)
- SKUs: [`infra/bicep/api/main.dev.bicepparam`](../../../infra/bicep/api/main.dev.bicepparam) `F1`; [`main.prod.bicepparam`](../../../infra/bicep/api/main.prod.bicepparam) `D1`

## Any remaining issues that we may wish to address

- **Resume here** when Azure approves the tickets:
  1. Confirm F1 quota ≥ 1 in **development** UK South (App Service, not Compute).
  2. Re-run **API infra CD** for GitHub Environment `development` (`prerequisitesExist` is already `true`).
  3. Confirm D1 quota in **production** UK South before production API infra CD.
  4. Then continue hosted API: App Configuration import / Key Vault secrets, site MI on `dertinfo-sql-db-access-development`, smoke the site.
- Do not change Bicep SKUs unless a ticket is refused and a different allowed SKU is chosen (subscription policy also constrains App Service SKUs).
- Production SQL `prerequisitesExist` and `New-DertInfoSqlDbAccessUser.ps1 -GitHubEnvironment production` remain separate follow-ups.
