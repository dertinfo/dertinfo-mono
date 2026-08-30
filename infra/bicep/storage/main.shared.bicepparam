/*
Shared storage parameters (not bound to a template).
Leaf files use: extends './main.shared.bicepparam'
Requires Bicep CLI 0.44.1+.
Do not put secrets or identifiable estate ids here.
*/

using none

param location = 'uksouth'
param productSlug = 'dertinfo'
param enableTelemetry = false
// Default false so a local deploy without overrides does not resolve Key Vault.
param prerequisitesExist = false
// PLACEHOLDER — supply via pipeline when prerequisitesExist is true (value from Key Vault sql-dertinfo-storage-administrator-login)
param sqlAdministratorLogin = ''
