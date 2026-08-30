/*
API — production leaf.
az deployment group create --resource-group rg-prd-dertinfo-api-uks --template-file main.bicep --parameters main.prod.bicepparam
Requires Bicep CLI 0.44.1+.
environmentTag stays 'prd'.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param appServiceSku = 'D1'
param aspNetCoreEnvironment = 'Production'
param appServicePlanName = 'plan-prd-dertinfo-api-uks'
param webAppName = 'app-prd-dertinfo-api-uks'
param keyVaultName = 'kv-prd-dertinfo-uks'
param appConfigurationName = 'appcs-prd-dertinfo-config-uks'
param monitoringResourceGroupName = 'rg-prd-dertinfo-monitoring-uks'
param applicationInsightsName = 'appi-prd-dertinfo-monitoring-uks'
