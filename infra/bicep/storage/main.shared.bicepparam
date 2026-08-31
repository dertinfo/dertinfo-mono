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
// If set to true indicates that the resources and actions required for this deployment have been completed.
// This includes:
// - Entra groups created by New-DertInfoSqlEntraGroups.ps1
// - Environment variables set in the github environment
//    - AZURE_ENTRA_SQL_ADMIN_GROUP_NAME=dertinfo-sql-admins-production
//    - AZURE_ENTRA_SQL_ADMIN_GROUP_OBJECTID=<object-id>
//    - AZURE_ENTRA_SQL_DBACCESS_GROUP_NAME=dertinfo-sql-db-access-production
//    - AZURE_ENTRA_SQL_DBACCESS_GROUP_OBJECTID=<object-id>
param prerequisitesExist = false
// PLACEHOLDER — Entra SQL admins group display name (dertinfo-sql-admins-development or -production)
param sqlEntraAdminGroupName = ''
// PLACEHOLDER — Entra SQL admins group object id; supply via CLI / pipeline
param sqlEntraAdminGroupObjectId = ''
// PLACEHOLDER — Entra tenant id; supply via CLI / pipeline
param entraTenantId = ''
