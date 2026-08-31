/*
Config workload: Key Vault + App Configuration (Free).
Keys and Key Vault references are applied by operator scripts from
infra/configuration/app-config.<environment>.json — not this template.
Deploy: az deployment group create --resource-group rg-<env>-dertinfo-config-uks --template-file main.bicep --parameters main.<env>.bicepparam
Requires Bicep CLI 0.44.1+.
*/

targetScope = 'resourceGroup'

// #####################################################
// Parameters
// #####################################################

@description('Azure region.')
param location string = 'uksouth'

@description('Environment tag (dev or prd).')
@allowed([
  'dev'
  'prd'
])
param environmentTag string = 'dev'

@description('Product slug used in resource names.')
param productSlug string = 'dertinfo'

@description('Key Vault name (max 24 characters).')
param keyVaultName string

@description('App Configuration store name.')
param appConfigurationName string

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

var resourceTags = {
  environment: environmentTag
  product: productSlug
  part: 'config'
  iac: 'infra-bicep-config'
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

// #####################################################
// AVM Modules
// #####################################################

module keyVault 'br/public:avm/res/key-vault/vault:0.14.0' = {
  name: 'avm-kv-${environmentTag}'
  params: {
    name: keyVaultName
    location: location
    enableRbacAuthorization: true
    enableVaultForTemplateDeployment: true
    enablePurgeProtection: false
    softDeleteRetentionInDays: 7
    sku: 'standard'
    publicNetworkAccess: 'Enabled'
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

module appConfiguration 'br/public:avm/res/app-configuration/configuration-store:0.9.3' = {
  name: 'avm-appcs-${environmentTag}'
  params: {
    name: appConfigurationName
    location: location
    sku: 'Free'
    publicNetworkAccess: 'Enabled'
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// Outputs
// #####################################################

output keyVaultName string = keyVault.outputs.name
output keyVaultResourceId string = keyVault.outputs.resourceId
output appConfigurationName string = appConfiguration.outputs.name
output appConfigurationEndpoint string = appConfiguration.outputs.endpoint
output environmentTag string = environmentTag
output regionTla string = regionTla
