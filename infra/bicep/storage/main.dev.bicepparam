/*
Storage — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-storage-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param imagesStorageAccountName = 'stdevdertinfoimagesuks'
param sqlServerName = 'sql-dev-dertinfo-storage-uks'
param sqlDatabaseName = 'sqldb-dev-dertinfo-storage-uks'
param sqlBackupShortTermRetentionDays = 1
param prerequisitesExist = true
