/*
Event Grid system topic and BlobCreated webhook subscriptions on the images account.
Storage owns this topic: BlobTrigger (EventGrid source) is not an EventGridTrigger.
Event Grid must POST to /runtime/webhooks/blobs with the blobs_extension host key.
Code must already be One-Deployed so the webhook handshake succeeds.
Reset after AzCopy: delete the four subscriptions, then re-run storage infra CD.
*/

targetScope = 'resourceGroup'

@description('Environment tag (dev or prd).')
param environmentTag string

@description('Images storage account that produces BlobCreated events.')
param imagesStorageAccountName string

@description('Function App that receives Event Grid blob webhooks.')
param functionAppName string

@description('Resource group of the Function App (for host listKeys).')
param functionAppResourceGroupName string

@description('Azure region of the storage account / system topic.')
param location string = resourceGroup().location

var systemTopicName = '${imagesStorageAccountName}-system-topic'
var blobExtensionKey = listKeys('${functionApp.id}/host/default', '2022-09-01').systemKeys.blobs_extension
var webhookBase = 'https://${functionAppName}.azurewebsites.net/runtime/webhooks/blobs'

var groupWebhook = '${webhookBase}?functionName=Host.Functions.ResizeGroupImages&code=${blobExtensionKey}'
var eventWebhook = '${webhookBase}?functionName=Host.Functions.ResizeEventImages&code=${blobExtensionKey}'
var sheetWebhook = '${webhookBase}?functionName=Host.Functions.ResizeSheetImages&code=${blobExtensionKey}'
var defaultWebhook = '${webhookBase}?functionName=Host.Functions.ResizeDefaultImages&code=${blobExtensionKey}'

resource imagesStorageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: imagesStorageAccountName
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' existing = {
  name: functionAppName
  scope: resourceGroup(functionAppResourceGroupName)
}

resource systemTopic 'Microsoft.EventGrid/systemTopics@2023-12-15-preview' = {
  name: systemTopicName
  location: location
  tags: {
    environment: environmentTag
  }
  properties: {
    source: imagesStorageAccount.id
    topicType: 'Microsoft.Storage.StorageAccounts'
  }
}

resource groupOriginals 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2023-12-15-preview' = {
  name: 'evgs-group-originals'
  parent: systemTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: groupWebhook
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      subjectBeginsWith: '/blobServices/default/containers/groupimages/blobs/originals/'
    }
  }
}

resource eventOriginals 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2023-12-15-preview' = {
  name: 'evgs-event-originals'
  parent: systemTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: eventWebhook
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      subjectBeginsWith: '/blobServices/default/containers/eventimages/blobs/originals/'
    }
  }
}

resource sheetOriginals 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2023-12-15-preview' = {
  name: 'evgs-sheet-originals'
  parent: systemTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: sheetWebhook
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      subjectBeginsWith: '/blobServices/default/containers/sheetimages/blobs/originals/'
    }
  }
}

resource defaultOriginals 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2023-12-15-preview' = {
  name: 'evgs-default-originals'
  parent: systemTopic
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: defaultWebhook
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      subjectBeginsWith: '/blobServices/default/containers/defaultimages/blobs/originals/'
    }
  }
}
