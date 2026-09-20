/*
Storage-owned data-plane roles on the images account.
The images account is the security boundary: this module assigns Blob Data Contributor
to named managed identities (Function App now; API site MI later).
*/

targetScope = 'resourceGroup'

@description('Images storage account name.')
param imagesStorageAccountName string

@description('Entra principal ids (site managed identities) that may read and write blobs on the images account.')
param principalIds array

var storageBlobDataContributorRoleId = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'

resource imagesStorage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: imagesStorageAccountName
}

resource blobDataContributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = [
  for principalId in principalIds: if (!empty(principalId)) {
    name: guid(imagesStorage.id, principalId, storageBlobDataContributorRoleId)
    scope: imagesStorage
    properties: {
      principalId: principalId
      principalType: 'ServicePrincipal'
      roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataContributorRoleId)
      description: 'Images account owner granted Storage Blob Data Contributor on ${imagesStorageAccountName}'
    }
  }
]
