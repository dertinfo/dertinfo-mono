/*
Outline: Subscription foundation for the agent-safe / development Azure estate.
Empty subscription: tenant administrator (portal / billing) only.
Foundation deploy: GitHub Actions subscription-infra-cd.yml (privileged subscription SP via OIDC).
  Agents and day-to-day contributors do not hold subscription-scope deploy rights.
Author: DertInfo
Azure CLI (admin / break-glass):
  az account set --subscription <new-subscription-id>
  az deployment sub create --location uksouth --template-file main.bicep --parameters main.dev.bicepparam
  az deployment sub create --location uksouth --template-file main.bicep --parameters main.prod.bicepparam
Optional overrides (e.g. pipeline principal):
  --parameters pipelinePrincipalId=<entra-object-id>
Shared non-secret defaults: main.shared.bicepparam (extends). Leaf files: main.dev.bicepparam / main.prod.bicepparam.
Requires Bicep CLI 0.44.1+.
*/

targetScope = 'subscription'

// #####################################################
// Parameters
// #####################################################

@description('Azure region for resource groups and deployment metadata.')
param location string = 'uksouth'

@description('Environment tag for resources (set in main.dev.bicepparam / main.prod.bicepparam).')
@allowed([
  'dev'
  'prd'
])
param environmentTag string = 'dev'

@description('Product slug used in resource group names.')
param productSlug string = 'dertinfo'

@description('Workload parts that each get a resource group.')
param workloadParts array = [
  'config'
  'storage'
  'web'
  'app'
  'api'
]

@description('Entra object id of the GitHub Actions infra deploy identity. Leave empty to skip role assignments.')
param pipelinePrincipalId string = ''

@description('Role definition id or name for the pipeline identity on each RG (Contributor by default).')
param pipelineRoleDefinitionIdOrName string = 'Contributor'

@description('Resource types allowed by Azure Policy (Deny).')
param allowedResourceTypes array = [
  'Microsoft.Web/staticSites'
  'Microsoft.Web/serverfarms'
  'Microsoft.Web/sites'
  'Microsoft.Web/sites/config'
  'Microsoft.Web/sites/basicPublishingCredentialsPolicies'
  'Microsoft.Sql/servers'
  'Microsoft.Sql/servers/databases'
  'Microsoft.Sql/servers/firewallRules'
  'Microsoft.Storage/storageAccounts'
  'Microsoft.Storage/storageAccounts/blobServices'
  'Microsoft.Storage/storageAccounts/blobServices/containers'
  'Microsoft.KeyVault/vaults'
  'Microsoft.AppConfiguration/configurationStores'
  'Microsoft.Insights/components'
  'Microsoft.OperationalInsights/workspaces'
  'Microsoft.Resources/deployments'
  'Microsoft.Authorization/policyDefinitions'
  'Microsoft.Authorization/policyAssignments'
  'Microsoft.Authorization/roleAssignments'
]

@description('App Service plan SKUs allowed by policy (Deny).')
param allowedAppServicePlanSkus array = [
  'F1'
  'D1'
  'B1'
  'Free'
  'Shared'
  'Basic'
]

@description('SQL database SKU names allowed by policy (Deny). Basic only for now.')
param allowedSqlDatabaseSkus array = [
  'Basic'
]

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

// #####################################################
// Variables
// #####################################################

var regionTlaByLocation = {
  uksouth: 'uks'
  eastus: 'eus'
  northeurope: 'neu'
}

var regionTla = regionTlaByLocation[location]

var resourceGroupNames = [for part in workloadParts: 'rg-${environmentTag}-${productSlug}-${part}-${regionTla}']

var resourceGroupTags = {
  environment: environmentTag
  product: 'dertinfo'
  iac: 'infra-bicep-subscription'
}

// #####################################################
// References
// #####################################################

// #####################################################
// Resources
// #####################################################

// #####################################################
// Modules
// #####################################################

// Policy composition (definitions + assignments) — not a pure AVM wrapper.
module policyModule './policy/main.bicep' = {
  name: 'subscription-policy'
  params: {
    assignmentSuffix: environmentTag
    allowedResourceTypes: allowedResourceTypes
    allowedAppServicePlanSkus: allowedAppServicePlanSkus
    allowedSqlDatabaseSkus: allowedSqlDatabaseSkus
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// AVM Modules
// #####################################################

module resourceGroups 'br/public:avm/res/resources/resource-group:0.4.4' = [
  for name in resourceGroupNames: {
    name: 'avm-rg-${name}'
    params: {
      name: name
      location: location
      tags: resourceGroupTags
      enableTelemetry: enableTelemetry
    }
  }
]

module pipelineRgRoleAssignments 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = [
  for name in resourceGroupNames: if (!empty(pipelinePrincipalId)) {
    name: 'avm-rbac-${name}'
    scope: resourceGroup(name)
    dependsOn: [
      resourceGroups
    ]
    params: {
      principalId: pipelinePrincipalId
      roleDefinitionIdOrName: pipelineRoleDefinitionIdOrName
      principalType: 'ServicePrincipal'
      description: 'GitHub Actions infra deploy identity (RG scope)'
      enableTelemetry: enableTelemetry
    }
  }
]

// #####################################################
// Outputs
// #####################################################

output resourceGroupNames array = [for (name, i) in resourceGroupNames: resourceGroups[i].outputs.name]
output environmentTag string = environmentTag
output location string = location
output allowedResourceTypesAssignmentId string = policyModule.outputs.allowedResourceTypesAssignmentId
output appServiceSkuAssignmentId string = policyModule.outputs.appServiceSkuAssignmentId
output sqlSkuAssignmentId string = policyModule.outputs.sqlSkuAssignmentId
