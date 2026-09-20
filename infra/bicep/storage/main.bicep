/*
Storage workload: images Storage Account (always) + SQL Basic (when flagSqlServerIsReady).
Images data-plane roles and Event Grid live here (storage is the security boundary).
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
param flagSqlServerIsReady bool = false

// Flip in the leaf after the Function App resource exists (site MI is present even with no code).
// Pipeline injects imagesDataPlanePrincipalIds (do not commit principal ids).
@description('When true, assign Storage Blob Data Contributor on the images account to imagesDataPlanePrincipalIds.')
param flagImagesFunctionAppReady bool = false

@description('Site MI principal ids granted Blob Data Contributor on the images account. PLACEHOLDER — supply via CLI / pipeline.')
param imagesDataPlanePrincipalIds array = []

// Flip in the leaf after Functions Src CD (blobs_extension host key + webhook handshake).
// A handshake timeout must not fail SQL or the images account — keep this module separate.
@description('When true, create Event Grid system topic and BlobCreated subscriptions on the images account. First infra run must be false.')
param flagImagesEventGridReady bool = false

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

var functionsResourceGroupLookup = 'rg-${environmentTag}-${productSlug}-functions-${regionTla}'
var functionAppName = 'func-${environmentTag}-${productSlug}-functions-${regionTla}'

// Evaluated only when the flag is true so an empty first deploy stays valid.
var imagesDataPlanePrincipalIdsEffective = flagImagesFunctionAppReady
  ? (empty(imagesDataPlanePrincipalIds)
      ? fail('flagImagesFunctionAppReady is true but imagesDataPlanePrincipalIds is empty. Storage infra CD looks up the Function App site MI when AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME is set.')
      : imagesDataPlanePrincipalIds)
  : []

// #####################################################
// References
// #####################################################

// #####################################################
// Resources
// #####################################################

// #####################################################
// Modules
// #####################################################

module sql './sql.bicep' = if (flagSqlServerIsReady) {
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

module imagesDataPlaneRoles './imagesDataPlaneRoles.bicep' = if (flagImagesFunctionAppReady) {
  name: 'images-data-plane-roles'
  params: {
    imagesStorageAccountName: imagesStorageAccountName
    principalIds: imagesDataPlanePrincipalIdsEffective
  }
  dependsOn: [
    imagesStorage
  ]
}

module eventGrid './eventGrid.bicep' = if (flagImagesEventGridReady) {
  name: 'event-grid-images'
  params: {
    environmentTag: environmentTag
    imagesStorageAccountName: imagesStorageAccountName
    functionAppName: functionAppName
    functionAppResourceGroupName: functionsResourceGroupLookup
    location: location
  }
  dependsOn: [
    imagesStorage
  ]
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
    // AVM 0.33.0 defaults networkAcls.defaultAction to Deny even when publicNetworkAccess is Enabled.
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
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
output sqlServerName string = flagSqlServerIsReady ? sql.outputs.sqlServerName : ''
output sqlServerFqdn string = flagSqlServerIsReady ? sql.outputs.sqlServerFqdn : ''
output sqlDatabaseName string = flagSqlServerIsReady ? sql.outputs.sqlDatabaseName : ''
output environmentTag string = environmentTag
output regionTla string = regionTla
