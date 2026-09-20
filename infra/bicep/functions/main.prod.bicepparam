/*
Functions — production leaf.
az deployment group create --resource-group rg-prd-dertinfo-functions-uks --template-file main.bicep --parameters main.prod.bicepparam
Requires Bicep CLI 0.44.1+.
environmentTag stays 'prd'. Keep this leaf filename until the planned rename to main.prd.bicepparam.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param appServicePlanName = 'plan-prd-dertinfo-functions-uks'
param functionAppName = 'func-prd-dertinfo-functions-uks'
param hostStorageAccountName = 'stprddertinfofuncuks'
param imagesStorageAccountName = 'stprddertinfoimagesuks'
param monitoringResourceGroupName = 'rg-prd-dertinfo-monitoring-uks'
param applicationInsightsName = 'appi-prd-dertinfo-monitoring-uks'
