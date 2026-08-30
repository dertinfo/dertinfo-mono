# Entra-only Azure SQL with admins and database access groups

## Summary of the work completed

Hosted Azure SQL no longer uses a SQL administrator login or password. Storage Bicep deploys an Entra-only server whose Entra admin is the `dertinfo-sql-admins-<environment>` group. Application access is the `dertinfo-sql-db-access-<environment>` group (`CREATE USER` plus reader/writer/ddladmin). The API App Service system-assigned identity is added to that access group and connects with `Authentication=Active Directory Default` when `AZURE_APP_CONFIG` is set. Two operator scripts: create the groups before SQL, then bind the access group to the database after. Storage no longer has Reader or Key Vault Secrets User on the config resource group.

## Why the work was completed

A shared SQL admin password in Key Vault would have been the app’s (and Bicep’s) credential, which is broader than the site managed identity and harder to revoke. Two Entra groups separate operators (server admin) from the application (database roles) without storing SQL passwords in Azure.

## Date the work was started

2026-08-30

## Date the work was completed

2026-08-30

## Issues that were encountered on the way

- ARM cannot `CREATE USER ... FROM EXTERNAL PROVIDER`. That step stays an operator script run as a member of the SQL admins group (`sqlcmd -G`).
- `getSecret()` cannot supply `administratorLogin` (non-secure string). Entra-only removes that path entirely.
- Incremental ARM does not delete the old storage-on-config Reader and Key Vault Secrets User assignments. Remove those in Azure after subscription infra CD.

## References to any best practices that we found

- [Microsoft Entra authentication for Azure SQL](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-overview)
- [Connect using Active Directory Default](https://learn.microsoft.com/en-us/sql/connect/ado-net/sql/azure-active-directory-authentication)
- AVM SQL server: `administrators` with `azureADOnlyAuthentication` and no SQL login — [avm/res/sql/server](https://github.com/Azure/bicep-registry-modules/tree/main/avm/res/sql/server)

## Any remaining issues that we may wish to address

- Operator must run `New-DertInfoSqlEntraGroups.ps1`, paste admin group vars, flip `prerequisitesExist`, deploy storage, then run `New-DertInfoSqlDbAccessUser.ps1`. Add operators and the App Service MI to the Entra groups later.
- Delete leftover storage SP Reader and Key Vault Secrets User on `rg-<env>-dertinfo-config-uks` in Azure.
- If `sql-dertinfo-storage-administrator-login` / `-password` were seeded, delete those two secrets only.
- Hosted App Configuration must have `SqlConnection:ServerName` and `SqlConnection:DatabaseName` (no password).
- Local SQL Express still uses `infra/secrets/api.env` user/password.
