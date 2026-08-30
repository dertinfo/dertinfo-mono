/*
Storage workload: images Storage Account (always) + SQL Basic (when prerequisitesExist).
Deploy: az deployment group create --resource-group rg-<env>-dertinfo-storage-uks --template-file main.bicep --parameters main.<env>.bicepparam
Requires Bicep CLI 0.44.1+.
*/

targetScope = 'resourceGroup'

// #####################################################
// Parameters
// #####################################################

@description('Azure region.')
param location string = 'uksouth'

@description('Environment tag (dev or prd).')
@allowed([
  'dev'
  'prd'
])
param environmentTag string = 'dev'

@description('Product slug used in resource names.')
param productSlug string = 'dertinfo'

@description('Public images Storage Account name (lowercase alphanumeric, 3-24). Purpose in the name so other accounts can be added later.')
param imagesStorageAccountName string

@description('SQL logical server name.')
param sqlServerName string

@description('SQL database name.')
param sqlDatabaseName string

@description('Display name of the Entra SQL admins group (dertinfo-sql-admins-development or -production).')
param sqlEntraAdminGroupName string = ''

@description('Object id of the Entra SQL admins group. PLACEHOLDER — supply via CLI / pipeline.')
param sqlEntraAdminGroupObjectId string = ''

@description('Entra tenant id. PLACEHOLDER — supply via CLI / pipeline.')
param entraTenantId string = ''

@description('Point-in-time backup retention in days. Azure SQL minimum is 1 (use for development). Production uses 7 with Local redundancy.')
param sqlBackupShortTermRetentionDays int

// Flip in main.shared.bicepparam (or a leaf) after these exist; workflows do not detect them:
// - Entra groups from New-DertInfoSqlEntraGroups.ps1
// - sqlEntraAdminGroupName, sqlEntraAdminGroupObjectId, and entraTenantId set (pipeline / CLI)
@description('When true, deploy Entra-only SQL. When false, deploy the storage account only.')
param prerequisitesExist bool = false

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

// #####################################################
// Variables
// #####################################################

var regionTlaByLocation = {
  uksouth: 'uks'
  eastus: 'eus'
  northeurope: 'neu'
}

var regionTla = regionTlaByLocation[location]

var resourceTags = {
  environment: environmentTag
  product: productSlug
  part: 'storage'
  iac: 'infra-bicep-storage'
}

var imageContainers = [
  'groupimages'
  'eventimages'
  'sheetimages'
  'defaultimages'
]

// #####################################################
// References
// #####################################################

// #####################################################
// Resources
// #####################################################

// #####################################################
// Modules
// #####################################################

module sql './sql.bicep' = if (prerequisitesExist) {
  name: 'sql-entra'
  params: {
    location: location
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    sqlEntraAdminGroupName: sqlEntraAdminGroupName
    sqlEntraAdminGroupObjectId: sqlEntraAdminGroupObjectId
    entraTenantId: entraTenantId
    sqlBackupShortTermRetentionDays: sqlBackupShortTermRetentionDays
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// AVM Modules
// #####################################################

module imagesStorage 'br/public:avm/res/storage/storage-account:0.33.0' = {
  name: 'avm-st-images'
  params: {
    name: imagesStorageAccountName
    location: location
    skuName: 'Standard_LRS'
    kind: 'StorageV2'
    allowBlobPublicAccess: true
    publicNetworkAccess: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    blobServices: {
      containers: [for name in imageContainers: {
        name: name
        publicAccess: 'Blob'
      }]
    }
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// Outputs
// #####################################################

output imagesStorageAccountName string = imagesStorage.outputs.name
output imagesStorageAccountResourceId string = imagesStorage.outputs.resourceId
output sqlServerName string = prerequisitesExist ? sql.outputs.sqlServerName : ''
output sqlServerFqdn string = prerequisitesExist ? sql.outputs.sqlServerFqdn : ''
output sqlDatabaseName string = prerequisitesExist ? sql.outputs.sqlDatabaseName : ''
output environmentTag string = environmentTag
output regionTla string = regionTla
