# Planned: Retire the old live and test Azure estates

**Status:** Not started. New-stack production is live. Do this after a short soak, not in the same change as a production deploy.

**Related:** [Production deployment complete](../changelogs/2026-09-27-001-production-deployment-complete.md), [CI/CD](../../technical/infra/cicd.md), [Set up the production environment](../../technical/guides/production-environment-setup.md).

---

## Intent

Remove the Azure infrastructure that served DertInfo before the new-stack production cutover, then remove any subscription that only existed to hold that infrastructure.

Keep the new-stack production subscription `5997dcc1-f0ad-47ef-835b-b509ff2132d2` and its `rg-prd-dertinfo-*` groups. Keep the new-stack development groups `rg-dev-dertinfo-*`.

## Before deleting anything

1. Confirm `https://www.dertinfo.co.uk` and `https://app.dertinfo.co.uk` are served by `swa-prd-dertinfo-web-uks` and `swa-prd-dertinfo-app-uks`.
2. List every subscription this account can see, and every resource group in the ones that are not the new-stack development or production subscriptions.
3. Treat the names below as a starting list from this repo and from the first production copy. Confirm each one in Azure before deleting it. The list is not a full inventory.

## Names already known

Old live estate, the source of the production SQL and image copy:

- Resource group `dertinfo-live-rg`
- SQL server `dertinfo-live-sqlsvr` and database `dertinfo-live-sqldb`
- Images account `dertinfoliveimagessa` (confirm its resource group in Azure)

Old test estate, from the legacy Functions table in [CI/CD](../../technical/infra/cicd.md):

- Subscription `9ee4f83c-a9a6-41a0-822d-13e18dc6c648`
- `dertinfo-test-rg`
- `dertinfotestimagessa`
- `di-rg-imageresizev4-stg`
- `di-rg-monitoring-stg`
- `dertinfo-test-ais`

Legacy deploy paths that can recreate those apps:

- Azure DevOps pipelines under `apps/*/pipelines/`
- GitHub Environments `test` and `prod`

## Actions

1. Disable the legacy Azure DevOps pipeline triggers so a later run cannot recreate the old apps.
2. After the soak, delete the old live and test resource groups.
3. When a subscription has no remaining resources, remove that subscription.
4. Remove GitHub Environments `test` and `prod` only after nothing still deploys to them.

## Out of scope

- New-stack development and production resources
- The DNS records that already point at the new Static Web Apps
