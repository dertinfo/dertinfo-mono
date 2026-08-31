/*
Site MI data-plane roles on the config RG (App Configuration Data Reader + Key Vault Secrets User).
Invoked from appService.bicep as a module scoped to the config RG (nested deployment).
Native resources here so we do not use extra AVM nested deployments.
*/

targetScope = 'resourceGroup'

@description('App Service system-assigned managed identity principal id.')
param principalId string

var appConfigurationDataReaderRoleId = '516239f1-63e1-4d78-a4de-a74fb236a071'
var keyVaultSecretsUserRoleId = '4633458b-17de-408a-b874-0445c86b69e6'

resource appConfigDataReader 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, principalId, appConfigurationDataReaderRoleId)
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', appConfigurationDataReaderRoleId)
    description: 'API web app reads Azure App Configuration'
  }
}

resource keyVaultSecretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, principalId, keyVaultSecretsUserRoleId)
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    description: 'API web app resolves App Configuration Key Vault references'
  }
}
