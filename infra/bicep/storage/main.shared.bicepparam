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
// Default false. Set true only after Entra SQL admin group ids are set (see comment on this param in main.bicep).
param prerequisitesExist = false
// PLACEHOLDER — Entra SQL admins group display name (dertinfo-sql-admins-development or -production)
param sqlEntraAdminGroupName = ''
// PLACEHOLDER — Entra SQL admins group object id; supply via CLI / pipeline
param sqlEntraAdminGroupObjectId = ''
// PLACEHOLDER — Entra tenant id; supply via CLI / pipeline
param entraTenantId = ''
