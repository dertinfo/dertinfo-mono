/*
Storage — production leaf.
az deployment group create --resource-group rg-prd-dertinfo-storage-uks --template-file main.bicep --parameters main.prod.bicepparam
Requires Bicep CLI 0.44.1+.
environmentTag stays 'prd'.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param imagesStorageAccountName = 'stprddertinfoimagesuks'
param sqlServerName = 'sql-prd-dertinfo-storage-uks'
param sqlDatabaseName = 'sqldb-prd-dertinfo-storage-uks'
param keyVaultName = 'kv-prd-dertinfo-uks'
param sqlBackupShortTermRetentionDays = 7
