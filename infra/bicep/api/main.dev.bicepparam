/*
API — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-api-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param appServiceSku = 'F1'
param aspNetCoreEnvironment = 'Development'
param appServicePlanName = 'plan-dev-dertinfo-api-uks'
param webAppName = 'app-dev-dertinfo-api-uks'
param keyVaultName = 'kv-dev-dertinfo-uks'
param appConfigurationName = 'appcs-dev-dertinfo-config-uks'
param monitoringResourceGroupName = 'rg-dev-dertinfo-monitoring-uks'
param applicationInsightsName = 'appi-dev-dertinfo-monitoring-uks'
param prerequisitesExist = true
