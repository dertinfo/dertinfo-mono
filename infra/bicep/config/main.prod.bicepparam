/*
Config — production leaf.
az deployment group create --resource-group rg-prd-dertinfo-config-uks --template-file main.bicep --parameters main.prod.bicepparam
Requires Bicep CLI 0.44.1+.
environmentTag stays 'prd'.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param keyVaultName = 'kv-prd-dertinfo-uks'
param appConfigurationName = 'appcs-prd-dertinfo-config-uks'
