# Config Key Vault secret names, App Config references, and API infra RBAC

## Summary of the work completed

Config Bicep now deploys labelled App Configuration Key Vault references for four hosted API secrets (`auth0-managementclientsecret`, `az-storage-accountkey`, `mailgun-apikey`, `sendgrid-apikey`). Values are not in Bicep; [`New-DertInfoConfigKeyVaultSecrets.ps1`](../../../infra/scripts/New-DertInfoConfigKeyVaultSecrets.ps1) prompts an administrator to set them once. API infra assigns App Configuration Data Reader and Key Vault Secrets User through a local module nested on the config RG (`site-mi-config-roles`). Subscription Bicep grants the API workload SP a custom role with only nested `deployments/read|write` (not Contributor) so that nested deployment is allowed. Conditioned UAA still limits which role assignments the pipeline may create.

## Why the work was completed

The hosted API reads secrets only through App Configuration Key Vault references. Putting secret values in Bicep would overwrite them on every config CD. AVM role-assignment modules scoped to the config RG create nested deployments; the API workload SP has Reader and conditioned UAA there, not Contributor, so API infra CD failed with `deployments/write` denied.

## Date the work was started

2026-08-31

## Date the work was completed

2026-08-31

## Issues that were encountered on the way

- Nested AVM `scope: resourceGroup(config…)` always writes a deployment in the target RG. Bicep also forbids native role assignments on resources in another RG from the API template (BCP139). The working pattern is one local module on the config RG plus a custom role that only allows `Microsoft.Resources/deployments/*` there — not Contributor.
- App Configuration ARM key names encode `:` as `%3A` and append `$<label>` so keys match `ASPNETCORE_ENVIRONMENT`.

## References to any best practices that we found

- [Use Key Vault references in App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-key-vault-references-dotnet-core)
- [Bicep modules and deployment scopes](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/modules)

## Any remaining issues that we may wish to address

- Operator must run `New-DertInfoConfigKeyVaultSecrets.ps1` after the vault exists (and config CD for the references).
- Re-run **subscription** infra CD so the API SP receives the nested-deploy custom role, then re-run **API** infra CD.
- Leftover nested deployments named `avm-rbac-appcs-reader` / `avm-rbac-kv-secrets-user` on the config RG can be ignored or deleted; they are replaced by `site-mi-config-roles`.
- Hosted App Configuration still needs non-secret keys (Auth0 domain, SQL server/database, and so on) set separately.
