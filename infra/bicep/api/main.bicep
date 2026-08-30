/*
API workload: Windows App Service plan + site when prerequisitesExist.
Deploy: az deployment group create --resource-group rg-<env>-dertinfo-api-uks --template-file main.bicep --parameters main.<env>.bicepparam
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

@description('App Service plan name.')
param appServicePlanName string

@description('Web app name.')
param webAppName string

@description('App Service plan SKU (F1 dev, D1 prd).')
param appServiceSku string = 'F1'

@description('ASPNETCORE_ENVIRONMENT / App Configuration label.')
param aspNetCoreEnvironment string = 'Development'

@description('Key Vault name in the config resource group (short: kv-<env>-dertinfo-uks).')
param keyVaultName string

@description('App Configuration store name.')
param appConfigurationName string

@description('Monitoring resource group name.')
param monitoringResourceGroupName string

@description('Application Insights component name.')
param applicationInsightsName string

// Flip in main.shared.bicepparam (or a leaf) after these exist; workflows do not detect them:
// - Key Vault kv-<env>-dertinfo-uks in rg-<env>-dertinfo-config-uks
// - App Configuration appcs-<env>-dertinfo-config-uks in the config RG
// - Application Insights appi-<env>-dertinfo-monitoring-uks in rg-<env>-dertinfo-monitoring-uks
@description('When true, deploy the plan and site and wire App Config / Insights. When false, succeed with no API resources.')
param prerequisitesExist bool = false

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

// Lookup of the existing config part RG (Key Vault / App Configuration). Not a name to deploy.
var configResourceGroupLookup = 'rg-${environmentTag}-${productSlug}-config-${regionTla}'

var resourceTags = {
  environment: environmentTag
  product: productSlug
  part: 'api'
  iac: 'infra-bicep-api'
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

module appService './appService.bicep' = if (prerequisitesExist) {
  name: 'api-appservice'
  params: {
    location: location
    environmentTag: environmentTag
    appServicePlanName: appServicePlanName
    webAppName: webAppName
    appServiceSku: appServiceSku
    aspNetCoreEnvironment: aspNetCoreEnvironment
    configResourceGroupLookup: configResourceGroupLookup
    keyVaultName: keyVaultName
    appConfigurationName: appConfigurationName
    monitoringResourceGroupName: monitoringResourceGroupName
    applicationInsightsName: applicationInsightsName
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// AVM Modules
// #####################################################

// #####################################################
// Outputs
// #####################################################

output webAppName string = prerequisitesExist ? appService.outputs.webAppName : ''
output webAppResourceId string = prerequisitesExist ? appService.outputs.webAppResourceId : ''
output appServicePlanName string = prerequisitesExist ? appService.outputs.appServicePlanName : ''
output environmentTag string = environmentTag
output regionTla string = regionTla
