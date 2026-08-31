# App Configuration catalog JSON, export, and import

## Summary of the work completed

Operator scripts for App Configuration and Key Vault secrets now read a committed catalog JSON per GitHub Environment (`infra/configuration/app-config.development.json` / `app-config.production.json`): store name, label, vault name, secret names, Key Vault reference keys, and non-secret `keyValues`. Secret **values** come from gitignored `kv-secrets.<environment>.json` (copy from `*.example`). Config Bicep creates the store and vault only; `Import-DertInfoAppConfiguration.ps1` writes keys and Key Vault references. `Export-DertInfoAppConfiguration.ps1` wraps `az appconfig kv export --skip-keyvault`.

## Why the work was completed

Migrating keys from an existing App Configuration store needed a repeatable export/import without copying secret values into Git. Hardcoded names in PowerShell made a second Environment require script edits; the catalog JSON is the Environment-specific input.

## Date the work was started

2026-08-31

## Date the work was completed

2026-08-31

## Issues that were encountered on the way

- `az appconfig kv export --resolve-keyvault` would write secrets to disk. Export always uses `--skip-keyvault`.
- Importing Key Vault references from an old store would keep old vault URIs. Import applies references from the catalog after the dump.
- `az appconfig kv set` / `export` default to access keys. With Entra-only (or without `listKeys`), that fails with "Cannot find a read write access key" even when the operator has App Configuration Data Owner. Import and export use `--auth-mode login`.
- PowerShell 5.1 strips quotes when passing JSON to `az.exe`, so Key Vault references arrived as `{uri:https://...}` instead of `{"uri":"..."}`. Import escapes JSON for the native command line.

## References to any best practices that we found

- [Import and export App Configuration data](https://learn.microsoft.com/en-us/azure/azure-app-configuration/howto-import-export-data)
- [Use Key Vault references in App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-key-vault-references-dotnet-core)

## Any remaining issues that we may wish to address

- Development `keyValues` include new-stack storage endpoints, SQL names, and API host. Key Vault references now cover Auth0 client IDs and App Insights telemetry IDs as well as storage/mail keys. Run `New-DertInfoConfigKeyVaultSecrets.ps1` then `Import-DertInfoAppConfiguration.ps1 -Force`.
- Config Bicep creates the store and vault only; keep Key Vault references in the catalog JSON, not in Bicep.
