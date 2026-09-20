/*
Functions workload: Linux Flex Consumption Function App when prerequisitesExist.
Deploy: az deployment group create --resource-group rg-<env>-dertinfo-functions-uks --template-file main.bicep --parameters main.<env>.bicepparam
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

@description('Flex Consumption plan name.')
param appServicePlanName string

@description('Function App name.')
param functionAppName string

@description('Host storage account name (AzureWebJobsStorage + One Deploy package). 3–24 lowercase alphanumeric.')
param hostStorageAccountName string

@description('New-stack images storage account name (blob trigger source).')
param imagesStorageAccountName string

@description('Monitoring resource group name.')
param monitoringResourceGroupName string

@description('Application Insights component name.')
param applicationInsightsName string

@description('Notify action-group email receiver(s). PLACEHOLDER — supply via GitHub Environment variable AZURE_MONITOR_FUNCTIONS_EXCESSIVEUSE_EMAIL. Comma-separated if several.')
param excessiveUseNotifyEmail string = ''

// Flip in main.shared.bicepparam (or a leaf) after these exist; workflows do not detect them:
// - Images SA st<env>dertinfoimagesuks in rg-<env>-dertinfo-storage-uks
// - Application Insights appi-<env>-dertinfo-monitoring-uks in rg-<env>-dertinfo-monitoring-uks
@description('When true, deploy the Flex plan, Function App, host storage, alerts, and host-storage site MI roles. When false, succeed with no Functions resources.')
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

var resourceTags = {
  environment: environmentTag
  product: productSlug
  part: 'functions'
  iac: 'infra-bicep-functions'
}

var warningThreshold = (environmentTag == 'prd') ? 100 : 30
var stopThreshold = (environmentTag == 'prd') ? 20000 : 100

var functionAppStopRoleDefinitionId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  guid(subscription().id, environmentTag, 'dertinfo-functionapp-stop')
)

// #####################################################
// References
// #####################################################

// #####################################################
// Resources
// #####################################################

// #####################################################
// Modules
// #####################################################

module functionApp './functionApp.bicep' = if (prerequisitesExist) {
  name: 'functions-flex'
  params: {
    location: location
    environmentTag: environmentTag
    appServicePlanName: appServicePlanName
    functionAppName: functionAppName
    hostStorageAccountName: hostStorageAccountName
    imagesStorageAccountName: imagesStorageAccountName
    monitoringResourceGroupName: monitoringResourceGroupName
    applicationInsightsName: applicationInsightsName
    excessiveUseNotifyEmail: excessiveUseNotifyEmail
    warningThreshold: warningThreshold
    stopThreshold: stopThreshold
    functionAppStopRoleDefinitionId: functionAppStopRoleDefinitionId
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

output functionAppName string = prerequisitesExist ? functionApp.outputs.functionAppName : ''
output functionAppResourceId string = prerequisitesExist ? functionApp.outputs.functionAppResourceId : ''
output appServicePlanName string = prerequisitesExist ? functionApp.outputs.appServicePlanName : ''
output hostStorageAccountName string = prerequisitesExist ? functionApp.outputs.hostStorageAccountName : ''
output environmentTag string = environmentTag
output regionTla string = regionTla
