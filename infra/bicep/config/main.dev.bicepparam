/*
Config — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-config-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param keyVaultName = 'kv-dev-dertinfo-uks'
param appConfigurationName = 'appcs-dev-dertinfo-config-uks'
