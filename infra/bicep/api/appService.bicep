/*
Windows App Service plan + site, wired to App Configuration and Application Insights.
Only invoked when prerequisitesExist is true so existing cross-RG resources are never resolved early.
*/

targetScope = 'resourceGroup'

@description('Azure region.')
param location string

@description('Environment tag (dev or prd).')
param environmentTag string

@description('App Service plan name.')
param appServicePlanName string

@description('Web app name.')
param webAppName string

@description('App Service plan SKU (F1 or D1).')
param appServiceSku string

@description('ASPNETCORE_ENVIRONMENT / App Configuration label.')
param aspNetCoreEnvironment string

@description('Lookup of the existing config part RG (Key Vault / App Configuration). Not a name to deploy.')
param configResourceGroupLookup string

@description('Key Vault name in the config resource group.')
param keyVaultName string

@description('App Configuration store name in the config resource group.')
param appConfigurationName string

@description('Monitoring resource group name.')
param monitoringResourceGroupName string

@description('Application Insights component name.')
param applicationInsightsName string

@description('Resource tags.')
param tags object

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

// #####################################################
// References
// #####################################################

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-03-01' existing = {
  name: appConfigurationName
  scope: resourceGroup(configResourceGroupLookup)
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
  scope: resourceGroup(monitoringResourceGroupName)
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
  scope: resourceGroup(configResourceGroupLookup)
}

// #####################################################
// AVM Modules
// #####################################################

module appServicePlan 'br/public:avm/res/web/serverfarm:0.7.0' = {
  name: 'avm-plan'
  params: {
    name: appServicePlanName
    location: location
    skuName: appServiceSku
    skuCapacity: 1
    kind: 'app'
    reserved: false
    zoneRedundant: false
    tags: tags
    enableTelemetry: enableTelemetry
  }
}

module webApp 'br/public:avm/res/web/site:0.24.0' = {
  name: 'avm-app'
  params: {
    name: webAppName
    location: location
    kind: 'app'
    serverFarmResourceId: appServicePlan.outputs.resourceId
    managedIdentities: {
      systemAssigned: true
    }
    httpsOnly: true
    siteConfig: {
      alwaysOn: false
      use32BitWorkerProcess: true
      netFrameworkVersion: 'v8.0'
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'AZURE_APP_CONFIG'
          value: appConfiguration.properties.endpoint
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: aspNetCoreEnvironment
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
      ]
    }
    basicPublishingCredentialsPolicies: [
      {
        name: 'ftp'
        allow: false
      }
      {
        name: 'scm'
        allow: false
      }
    ]
    tags: tags
    enableTelemetry: enableTelemetry
  }
}

module appConfigDataReader 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = {
  name: 'avm-rbac-appcs-reader'
  scope: resourceGroup(configResourceGroupLookup)
  params: {
    principalId: webApp.outputs.systemAssignedMIPrincipalId!
    roleDefinitionIdOrName: '516239f1-63e1-4d78-a4de-a74fb236a071' // 'App Configuration Data Reader'
    principalType: 'ServicePrincipal'
    description: 'API web app reads Azure App Configuration'
    enableTelemetry: enableTelemetry
  }
}

module keyVaultSecretsUser 'br/public:avm/res/authorization/role-assignment/rg-scope:0.1.1' = {
  name: 'avm-rbac-kv-secrets-user'
  scope: resourceGroup(configResourceGroupLookup)
  params: {
    principalId: webApp.outputs.systemAssignedMIPrincipalId!
    roleDefinitionIdOrName: '4633458b-17de-408a-b874-0445c86b69e6' // 'Key Vault Secrets User'
    principalType: 'ServicePrincipal'
    description: 'API web app resolves App Configuration Key Vault references'
    enableTelemetry: enableTelemetry
  }
}

output appServicePlanName string = appServicePlan.outputs.name
output webAppName string = webApp.outputs.name
output webAppResourceId string = webApp.outputs.resourceId
output webAppPrincipalId string = webApp.outputs.systemAssignedMIPrincipalId!
output keyVaultResourceId string = keyVault.id
output environmentTag string = environmentTag
