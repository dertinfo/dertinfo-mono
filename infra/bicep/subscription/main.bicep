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
Optional overrides (workload SP object ids; empty skips that part):
  --parameters pipelinePrincipalIdApi=<entra-object-id>
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
  'monitoring'
]

@description('Entra object id of the config workload SP. Leave empty to skip that RG role assignment.')
param pipelinePrincipalIdConfig string = ''

@description('Entra object id of the monitoring workload SP.')
param pipelinePrincipalIdMonitoring string = ''

@description('Entra object id of the storage workload SP.')
param pipelinePrincipalIdStorage string = ''

@description('Entra object id of the api workload SP.')
param pipelinePrincipalIdApi string = ''

@description('Entra object id of the web workload SP.')
param pipelinePrincipalIdWeb string = ''

@description('Entra object id of the app workload SP.')
param pipelinePrincipalIdApp string = ''

@description('Entra object id of the functions workload SP. No functions RG yet; reserved for later.')
param pipelinePrincipalIdFunctions string = ''

@description('Role definition id or name for the pipeline identity on each RG (Contributor by default).')
param pipelineRoleDefinitionIdOrName string = 'Contributor'

@description('Resource types allowed by Azure Policy (Deny). Values come from main.shared.bicepparam.')
param allowedResourceTypes array

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

// Lookups of part RGs created in the resourceGroups loop. Not names to deploy.
var configResourceGroupLookup = 'rg-${environmentTag}-${productSlug}-config-${regionTla}'
var monitoringResourceGroupLookup = 'rg-${environmentTag}-${productSlug}-monitoring-${regionTla}'

var workloadDeployPrincipals = [
  {
    part: 'config'
    principalId: pipelinePrincipalIdConfig
  }
  {
    part: 'monitoring'
    principalId: pipelinePrincipalIdMonitoring
  }
  {
    part: 'storage'
    principalId: pipelinePrincipalIdStorage
  }
  {
    part: 'api'
    principalId: pipelinePrincipalIdApi
  }
  {
    part: 'web'
    principalId: pipelinePrincipalIdWeb
  }
  {
    part: 'app'
    principalId: pipelinePrincipalIdApp
  }
]

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
  for item in workloadDeployPrincipals: if (!empty(item.principalId)) {
    name: 'avm-rbac-${item.part}'
    scope: resourceGroup('rg-${environmentTag}-${productSlug}-${item.part}-${regionTla}')
    dependsOn: [
      resourceGroups
    ]
    params: {
      principalId: item.principalId
      roleDefinitionIdOrName: pipelineRoleDefinitionIdOrName
      principalType: 'ServicePrincipal'
      description: 'GitHub Actions ${item.part} workload identity (RG Contributor)'
      enableTelemetry: enableTelemetry
    }
  }
]

// Storage infra CD reads SQL admin secrets from Key Vault in the config RG (lookup only).
module pipelineStorageKvSecretsUser 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = if (!empty(pipelinePrincipalIdStorage)) {
  name: 'avm-rbac-storage-kv-secrets-user'
  scope: resourceGroup(configResourceGroupLookup)
  dependsOn: [
    resourceGroups
    pipelineRgRoleAssignments
  ]
  params: {
    principalId: pipelinePrincipalIdStorage
    roleDefinitionIdOrName: 'Key Vault Secrets User'
    principalType: 'ServicePrincipal'
    description: 'Storage workload identity — read SQL admin secrets from config Key Vault'
    enableTelemetry: enableTelemetry
  }
}

module pipelineApiConfigUserAccessAdmin 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = if (!empty(pipelinePrincipalIdApi)) {
  name: 'avm-rbac-api-config-uaa'
  scope: resourceGroup(configResourceGroupLookup)
  dependsOn: [
    resourceGroups
    pipelineRgRoleAssignments
  ]
  params: {
    principalId: pipelinePrincipalIdApi
    roleDefinitionIdOrName: 'User Access Administrator'
    principalType: 'ServicePrincipal'
    description: 'API workload identity — grant site MI App Config / Key Vault data-plane roles'
    enableTelemetry: enableTelemetry
  }
}

module pipelineApiConfigReader 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = if (!empty(pipelinePrincipalIdApi)) {
  name: 'avm-rbac-api-config-reader'
  scope: resourceGroup(configResourceGroupLookup)
  dependsOn: [
    resourceGroups
    pipelineRgRoleAssignments
  ]
  params: {
    principalId: pipelinePrincipalIdApi
    roleDefinitionIdOrName: 'Reader'
    principalType: 'ServicePrincipal'
    description: 'API workload identity — resolve existing Key Vault and App Configuration'
    enableTelemetry: enableTelemetry
  }
}

module pipelineApiMonitoringReader 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = if (!empty(pipelinePrincipalIdApi)) {
  name: 'avm-rbac-api-monitoring-reader'
  scope: resourceGroup(monitoringResourceGroupLookup)
  dependsOn: [
    resourceGroups
    pipelineRgRoleAssignments
  ]
  params: {
    principalId: pipelinePrincipalIdApi
    roleDefinitionIdOrName: 'Reader'
    principalType: 'ServicePrincipal'
    description: 'API workload identity — resolve existing Application Insights'
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// Outputs
// #####################################################

output resourceGroupNames array = [for (name, i) in resourceGroupNames: resourceGroups[i].outputs.name]
output environmentTag string = environmentTag
output location string = location
output allowedResourceTypesAssignmentId string = policyModule.outputs.allowedResourceTypesAssignmentId
output appServiceSkuAssignmentId string = policyModule.outputs.appServiceSkuAssignmentId
output sqlSkuAssignmentId string = policyModule.outputs.sqlSkuAssignmentId
