# Production environment setup guide

## Summary of the work completed

[Set up the production environment](../../technical/guides/production-environment-setup.md) is a milestone list an operator follows after development is already up. Each milestone has the actions, then one check. The next milestone starts only after that check passes. Live DNS stays unchanged until the last milestone. [`Copy-DertInfoSqlToProduction.ps1`](../../../infra/scripts/Database/Copy-DertInfoSqlToProduction.ps1) and [`Copy-DertInfoImagesToProduction.ps1`](../../../infra/scripts/Storage/Copy-DertInfoImagesToProduction.ps1) are the SQL and image steps, so the guide runs those scripts. Indexes and the production note in [CI/CD](../../technical/infra/cicd.md) point at the guide.

## Why the work was completed

Production deploy was a short Functions list inside the CI/CD page, and the scripts had just moved into category folders. One milestone guide is what an operator follows to bring the website, API, and PWA up on Azure hostnames before changing CNAME records. The production SQL copy and the image copy are scripts, so the guide does not repeat ARM REST or AzCopy commands. Script paths match `infra/scripts` after [2026-09-26-004](./2026-09-26-004-script-folder-categories.md).

## Date the work was started

2026-09-26

## Date the work was completed

2026-09-26

## Issues that were encountered on the way

- [`Copy-DertInfoSqlToDevelopment.ps1`](../../../infra/scripts/Database/Copy-DertInfoSqlToDevelopment.ps1) always copies onto the development server. The production script uses the same ARM `createMode: Copy`, sidecar, and rename against `sql-prd-dertinfo-storage-uks` and does not touch development.
- The old image accounts are not named in the repo, and Eventbrite storage may be a separate account. The storage script takes those names as parameters and copies `groupimages`, `eventimages`, `sheetimages`, and `defaultimages` onto `stprddertinfoimagesuks` while the Function App is stopped. It does not enable Event Grid.
- [`app-config.production.json`](../../../infra/configuration/app-config.production.json) has no `keyValues`. The guide tells the operator to add them, with callback and CORS set to the API hostname until the Static Web App hostnames exist.
- `New-DertInfoConfigKeyVaultSecrets.ps1` rejects an empty `az-storage-functions-accountkey`, and the Flex host storage account has shared keys disabled. The guide uses a non-empty placeholder.

## References to any best practices that we found

- [GitHub Flow](https://docs.github.com/en/get-started/using-github/github-flow) — param flips merge to `main` before `workflow_dispatch`, because the workflow reads that ref.
- [CI/CD](../../technical/infra/cicd.md) — target `full` runs development, then gated production.

## Any remaining issues that we may wish to address

- The guide has not been executed against the live estate.
- Source storage account names, including a separate Eventbrite account if one exists, are still parameters the operator supplies. The repo does not list them.
