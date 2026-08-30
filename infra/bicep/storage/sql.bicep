/*
SQL server + Basic database. Only invoked when prerequisitesExist is true
so Entra admin placeholders are never resolved as empty SIDs.
Entra-only: no SQL administrator login or password.
*/

targetScope = 'resourceGroup'

@description('Azure region.')
param location string

@description('SQL logical server name.')
param sqlServerName string

@description('SQL database name.')
param sqlDatabaseName string

@description('Display name of the Entra group that is the SQL server admin.')
param sqlEntraAdminGroupName string

@description('Object id of the Entra SQL admins group. PLACEHOLDER — supply via CLI / pipeline.')
param sqlEntraAdminGroupObjectId string

@description('Entra tenant id for the SQL Entra admin. PLACEHOLDER — supply via CLI / pipeline.')
param entraTenantId string

@description('Point-in-time backup retention in days. Azure SQL cannot disable backups; 1 is the minimum.')
param sqlBackupShortTermRetentionDays int

@description('Resource tags.')
param tags object

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

// #####################################################
// AVM Modules
// #####################################################

module sqlServer 'br/public:avm/res/sql/server:0.22.0' = {
  name: 'avm-sql'
  params: {
    name: sqlServerName
    location: location
    administrators: {
      azureADOnlyAuthentication: true
      login: sqlEntraAdminGroupName
      principalType: 'Group'
      sid: sqlEntraAdminGroupObjectId
      tenantId: entraTenantId
    }
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
