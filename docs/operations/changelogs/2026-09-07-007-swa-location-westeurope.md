# Deploy Static Web Apps to West Europe

## Summary of the work completed

Website and PWA Bicep now set the Static Web App `location` to `westeurope` instead of `uksouth`. Resource groups, names, and infra CD RG location stay `uksouth` / `*-uks`. Docs: [CI/CD](../../technical/infra/cicd.md), [Bicep standards](../../technical/standards/bicep/README.md).

## Why the work was completed

`Microsoft.Web/staticSites` cannot be created in UK South. The first web/app infra CD after merge failed for that reason. West Europe is the nearest region ARM lists for this resource type.

## Date the work was started

2026-09-07

## Date the work was completed

2026-09-07

## Issues that were encountered on the way

- Azure Static Web Apps is a global front-end service; the ARM `location` is only the region for the site resource (and managed Functions / staging). That location set is small and does not include `uksouth`.
- The Azure products-by-region / pricing pickers can list UK South or mark the product as non-regional, which does not mean you can create `Microsoft.Web/staticSites` there.

## References to any best practices that we found

- [Azure Static Web Apps FAQ — region](https://learn.microsoft.com/en-us/azure/static-web-apps/faq#how-do-i-ensure-my-app-is-deployed-to-a-specific-azure-region) — static assets are global; you pick a region for the managed Functions / staging side
- [Relocate Azure Static Web Apps](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/relocation/relocation-static-web-apps) — confirm the service is available in the target region before deploy
- ARM provider `Microsoft.Web/staticSites` locations (this subscription): Central US, East US 2, West US 2, West Europe, East Asia

## Any remaining issues that we may wish to address

- Re-run Web and App infra CD `dev-only` after this change merges.
