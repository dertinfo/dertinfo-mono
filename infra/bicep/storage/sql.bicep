/*
SQL server + Basic database. Only invoked when prerequisitesExist is true
so existing Key Vault / getSecret() are never evaluated without the vault.
*/

targetScope = 'resourceGroup'

@description('Azure region.')
param location string

@description('SQL logical server name.')
param sqlServerName string

@description('SQL database name.')
param sqlDatabaseName string

@description('SQL administrator login (from Key Vault; passed by the caller).')
param administratorLogin string

@description('Lookup of the existing config part RG that holds the Key Vault. Not a name to deploy.')
param configResourceGroupLookup string

@description('Key Vault name that holds sql-dertinfo-storage-administrator-password.')
param keyVaultName string

@description('Point-in-time backup retention in days. Azure SQL cannot disable backups; 1 is the minimum.')
param sqlBackupShortTermRetentionDays int

@description('Resource tags.')
param tags object

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

// #####################################################
// References
// #####################################################

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
  scope: resourceGroup(configResourceGroupLookup)
}

// #####################################################
// AVM Modules
// #####################################################

module sqlServer 'br/public:avm/res/sql/server:0.22.0' = {
  name: 'avm-sql'
  params: {
    name: sqlServerName
    location: location
    administratorLogin: administratorLogin
    administratorLoginPassword: keyVault.getSecret('sql-dertinfo-storage-administrator-password')
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    firewallRules: [
      {
        name: 'AllowAllWindowsAzureIps'
        startIpAddress: '0.0.0.0'
        endIpAddress: '0.0.0.0'
      }
    ]
    databases: [
      {
        name: sqlDatabaseName
        availabilityZone: -1
        zoneRedundant: false
        maxSizeBytes: 2147483648
        requestedBackupStorageRedundancy: 'Local'
        backupShortTermRetentionPolicy: {
          retentionDays: sqlBackupShortTermRetentionDays
        }
        sku: {
          name: 'Basic'
          tier: 'Basic'
          capacity: 5
        }
      }
    ]
    tags: tags
    enableTelemetry: enableTelemetry
  }
}

output sqlServerName string = sqlServer.outputs.name
output sqlServerFqdn string = sqlServer.outputs.fullyQualifiedDomainName
output sqlDatabaseName string = sqlDatabaseName
