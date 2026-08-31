/*
Config workload: Key Vault + App Configuration (Free).
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

@description('App Configuration label matching ASPNETCORE_ENVIRONMENT.')
@allowed([
  'Development'
  'Production'
])
param appConfigurationLabel string

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

var keyVaultReferenceContentType = 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'

// Portal / New-DertInfoConfigKeyVaultSecrets.ps1 must create these secret names. Values are not in Bicep.
var hostedApiKeyVaultRefs = [
  {
    appConfigKey: 'Auth0:ManagementClientSecret'
    secretName: 'auth0-managementclientsecret'
  }
  {
    appConfigKey: 'StorageAccount:Images:Key'
    secretName: 'az-storage-accountkey'
  }
  {
    appConfigKey: 'Mailgun:ApiKey'
    secretName: 'mailgun-apikey'
  }
  {
    appConfigKey: 'SendGrid:ApiKey'
    secretName: 'sendgrid-apikey'
  }
]

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

resource appConfigurationStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName
}

resource appConfigKeyVaultRefs 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = [
  for item in hostedApiKeyVaultRefs: {
    parent: appConfigurationStore
    name: '${replace(item.appConfigKey, ':', '%3A')}$${appConfigurationLabel}'
    dependsOn: [
      appConfiguration
      keyVault
    ]
    properties: {
      contentType: keyVaultReferenceContentType
      value: '{"uri":"https://${keyVaultName}.vault.azure.net/secrets/${item.secretName}"}'
    }
  }
]

// #####################################################
// Outputs
// #####################################################

output keyVaultName string = keyVault.outputs.name
output keyVaultResourceId string = keyVault.outputs.resourceId
output appConfigurationName string = appConfiguration.outputs.name
output appConfigurationEndpoint string = appConfiguration.outputs.endpoint
output environmentTag string = environmentTag
output regionTla string = regionTla
