/*
Functions — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-functions-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param appServicePlanName = 'plan-dev-dertinfo-functions-uks'
param functionAppName = 'func-dev-dertinfo-functions-uks'
param hostStorageAccountName = 'stdevdertinfofuncuks'
param imagesStorageAccountName = 'stdevdertinfoimagesuks'
param monitoringResourceGroupName = 'rg-dev-dertinfo-monitoring-uks'
param applicationInsightsName = 'appi-dev-dertinfo-monitoring-uks'
param prerequisitesExist = true
