/*
Site MI data-plane roles on the Functions host storage account
(Storage Blob Data Owner, Queue Data Contributor, Table Data Contributor).
*/

targetScope = 'resourceGroup'

@description('Function App system-assigned managed identity principal id.')
param principalId string

@description('Host storage account name.')
param hostStorageAccountName string

var storageBlobDataOwnerRoleId = 'b7e6ba4c-320b-476a-8d46-e3a97e8c5b32'
var storageQueueDataContributorRoleId = '974c5e8b-45b9-4653-ba55-5f573ca84842'
var storageTableDataContributorRoleId = '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'

resource hostStorage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: hostStorageAccountName
}

resource blobDataOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(hostStorage.id, principalId, storageBlobDataOwnerRoleId)
  scope: hostStorage
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataOwnerRoleId)
    description: 'Function App host storage — Storage Blob Data Owner'
  }
}

resource queueDataContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(hostStorage.id, principalId, storageQueueDataContributorRoleId)
  scope: hostStorage
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageQueueDataContributorRoleId)
    description: 'Function App host storage — Storage Queue Data Contributor'
  }
}

resource tableDataContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(hostStorage.id, principalId, storageTableDataContributorRoleId)
  scope: hostStorage
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageTableDataContributorRoleId)
    description: 'Function App host storage — Storage Table Data Contributor'
  }
}
