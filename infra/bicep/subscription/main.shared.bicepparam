/*
Shared subscription parameters (not bound to a template).
Leaf files (main.dev.bicepparam / main.prod.bicepparam) use: extends './main.shared.bicepparam'
Requires Bicep CLI 0.44.1+.
Do not put secrets or identifiable estate ids here.
*/

using none

param location = 'uksouth'
param productSlug = 'dertinfo'
param workloadParts = [
  'config'
  'storage'
  'web'
  'app'
  'api'
  'monitoring'
]
param pipelineRoleDefinitionIdOrName = 'b24988ac-6180-42a0-ab88-20f7382dd24c' // 'Contributor'
param allowedResourceTypes = [
  'Microsoft.Web/staticSites'
  'Microsoft.Web/serverfarms'
  'Microsoft.Web/sites'
  'Microsoft.Web/sites/config'
  'Microsoft.Web/sites/basicPublishingCredentialsPolicies'
  'Microsoft.Web/sites/extensions'
  'Microsoft.Sql/servers'
  'Microsoft.Sql/servers/databases'
  'Microsoft.Sql/servers/firewallRules'
  'Microsoft.Sql/servers/auditingSettings'
  'Microsoft.Sql/servers/securityAlertPolicies'
  'Microsoft.Sql/servers/encryptionProtector'
  'Microsoft.Sql/servers/connectionPolicies'
  'Microsoft.Sql/servers/databases/transparentDataEncryption'
  'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies'
  'Microsoft.Storage/storageAccounts'
  'Microsoft.Storage/storageAccounts/blobServices'
  'Microsoft.Storage/storageAccounts/blobServices/containers'
  'Microsoft.Storage/storageAccounts/queueServices'
  'Microsoft.Storage/storageAccounts/tableServices'
  'Microsoft.Storage/storageAccounts/fileServices'
  'Microsoft.KeyVault/vaults'
  'Microsoft.KeyVault/vaults/secrets'
  'Microsoft.AppConfiguration/configurationStores'
  'Microsoft.AppConfiguration/configurationStores/keyValues'
  'Microsoft.Insights/components'
  'Microsoft.Insights/dataCollectionRules'
  'Microsoft.Insights/dataCollectionEndpoints'
  'Microsoft.OperationalInsights/workspaces'
  'Microsoft.OperationalInsights/workspaces/tables'
  'Microsoft.Resources/deployments'
  'Microsoft.Authorization/policyDefinitions'
  'Microsoft.Authorization/policyAssignments'
  'Microsoft.Authorization/roleAssignments'
]
param allowedAppServicePlanSkus = [
  'F1'
  'D1'
  'B1'
  'Free'
  'Shared'
  'Basic'
]
param allowedSqlDatabaseSkus = [
  'Basic'
]
param enableTelemetry = false
// PLACEHOLDER — Entra object ids of workload SPs; supply via CLI / pipeline override
param pipelinePrincipalIdConfig = ''
param pipelinePrincipalIdMonitoring = ''
param pipelinePrincipalIdStorage = ''
param pipelinePrincipalIdApi = ''
param pipelinePrincipalIdWeb = ''
param pipelinePrincipalIdApp = ''
param pipelinePrincipalIdFunctions = ''
