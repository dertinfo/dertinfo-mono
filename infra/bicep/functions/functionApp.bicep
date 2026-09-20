/*
Linux Flex Consumption Function App, host storage, identity settings, host-storage site MI roles, alerts.
Images Blob Data Contributor and Event Grid live in storage Bicep (storage is that account's security boundary).
Only invoked when prerequisitesExist is true so existing cross-RG resources are never resolved early.
Raw Microsoft.Web@2024-04-01 for the Flex site (functionAppConfig). AVM web/site 0.24.0 does not express Flex.
*/

targetScope = 'resourceGroup'

@description('Azure region.')
param location string

@description('Environment tag (dev or prd).')
param environmentTag string

@description('Flex Consumption plan name.')
param appServicePlanName string

@description('Function App name.')
param functionAppName string

@description('Host storage account name.')
param hostStorageAccountName string

@description('New-stack images storage account name.')
param imagesStorageAccountName string

@description('Monitoring resource group name.')
param monitoringResourceGroupName string

@description('Application Insights component name.')
param applicationInsightsName string

@description('Notify action-group email receiver(s), comma-separated.')
param excessiveUseNotifyEmail string

@description('Warn metric-alert threshold (OnDemandFunctionExecutionCount, 5-minute window).')
param warningThreshold int

@description('Stop metric-alert threshold (OnDemandFunctionExecutionCount, 5-minute window).')
param stopThreshold int

@description('Subscription custom role id for Microsoft.Web/sites/stop/action on this RG.')
param functionAppStopRoleDefinitionId string

@description('Resource tags.')
param tags object

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

var deploymentContainerName = 'app-package-${functionAppName}'
var deploymentStorageValue = 'https://${hostStorageAccountName}.blob.${environment().suffixes.storage}/${deploymentContainerName}'

// #####################################################
// References
// #####################################################

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
  scope: resourceGroup(monitoringResourceGroupName)
}

// #####################################################
// Resources
// #####################################################

resource hostingPlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'FC1'
    tier: 'FlexConsumption'
  }
  kind: 'functionapp'
  properties: {
    reserved: true
  }
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  dependsOn: [
    hostStorage
  ]
  properties: {
    serverFarmId: hostingPlan.id
    httpsOnly: true
    functionAppConfig: {
      deployment: {
        storage: {
          type: 'blobContainer'
          value: deploymentStorageValue
          authentication: {
            type: 'SystemAssignedIdentity'
          }
        }
      }
      scaleAndConcurrency: {
        maximumInstanceCount: 40
        instanceMemoryMB: 2048
      }
      runtime: {
        name: 'dotnet-isolated'
        version: '8.0'
      }
    }
  }
}

resource functionAppSettings 'Microsoft.Web/sites/config@2024-04-01' = {
  name: 'appsettings'
  parent: functionApp
  properties: {
    AzureWebJobsStorage__accountName: hostStorageAccountName
    AzureWebJobsStorage__credential: 'managedidentity'
    StorageConnection__Images__accountName: imagesStorageAccountName
    StorageConnection__Images__credential: 'managedidentity'
    APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.properties.ConnectionString
    'AzureWebJobs.ResizeDefaultImagesPolling.Disabled': 'true'
    'AzureWebJobs.ResizeGroupImagesPolling.Disabled': 'true'
    'AzureWebJobs.ResizeEventImagesPolling.Disabled': 'true'
    'AzureWebJobs.ResizeSheetImagesPolling.Disabled': 'true'
  }
}

resource scmBasicAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-04-01' = {
  name: 'scm'
  parent: functionApp
  properties: {
    allow: false
  }
}

resource ftpBasicAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2024-04-01' = {
  name: 'ftp'
  parent: functionApp
  properties: {
    allow: false
  }
}

// #####################################################
// Modules
// #####################################################

module siteMiHostStorageRoles './siteMiHostStorageRoles.bicep' = {
  name: 'site-mi-host-storage-roles'
  params: {
    principalId: functionApp.identity.principalId
    hostStorageAccountName: hostStorageAccountName
  }
}

module alerts './alerts.bicep' = {
  name: 'functions-alerts'
  params: {
    location: location
    environmentTag: environmentTag
    functionAppName: functionApp.name
    excessiveUseNotifyEmail: excessiveUseNotifyEmail
    warningThreshold: warningThreshold
    stopThreshold: stopThreshold
    functionAppStopRoleDefinitionId: functionAppStopRoleDefinitionId
    tags: tags
  }
}

// #####################################################
// AVM Modules
// #####################################################

module hostStorage 'br/public:avm/res/storage/storage-account:0.33.0' = {
  name: 'avm-st-func-host'
  params: {
    name: hostStorageAccountName
    location: location
    skuName: 'Standard_LRS'
    kind: 'StorageV2'
    allowBlobPublicAccess: false
    publicNetworkAccess: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    blobServices: {
      containers: [
        {
          name: deploymentContainerName
          publicAccess: 'None'
        }
      ]
    }
    tags: tags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// Outputs
// #####################################################

output functionAppName string = functionApp.name
output functionAppResourceId string = functionApp.id
output appServicePlanName string = hostingPlan.name
output hostStorageAccountName string = hostStorage.outputs.name
output functionAppPrincipalId string = functionApp.identity.principalId
