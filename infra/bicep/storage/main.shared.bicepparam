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
// Default false. Set true only after config KV + SQL admin secrets exist (see comment on this param in main.bicep).
param prerequisitesExist = false
// PLACEHOLDER — set when prerequisitesExist is true (same value as Key Vault sql-dertinfo-storage-administrator-login)
param sqlAdministratorLogin = ''
