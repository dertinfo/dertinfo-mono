/*
Excess-use notify + stop: action groups, metric alerts, Logic App (ARM stop).
New-stack resources only — do not existing-reference di-agrp-excessiveuse-*.
*/

targetScope = 'resourceGroup'

@description('Azure region (Logic App and ARM connection).')
param location string

@description('Environment tag (dev or prd).')
param environmentTag string

@description('Function App name to monitor and stop.')
param functionAppName string

@description('Notify action-group email receiver(s), comma-separated.')
param excessiveUseNotifyEmail string

@description('Warn metric-alert threshold.')
param warningThreshold int

@description('Stop metric-alert threshold.')
param stopThreshold int

@description('Subscription custom role id for Microsoft.Web/sites/stop/action.')
param functionAppStopRoleDefinitionId string

@description('Resource tags.')
param tags object

var notifyActionGroupName = 'agrp-${environmentTag}-dertinfo-functions-excessiveuse-uks'
var stopActionGroupName = 'agrp-${environmentTag}-dertinfo-functions-stop-uks'
var warnAlertName = 'alert-${environmentTag}-dertinfo-functions-excessiveuse-uks'
var stopAlertName = 'alert-${environmentTag}-dertinfo-functions-stop-uks'
var logicAppName = 'logic-${environmentTag}-dertinfo-functions-stop-uks'
var armConnectionName = '${logicAppName}-arm'
var azureManagedConnectorResourceId = '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Web/locations/${location}/managedApis/arm'
var functionAppPath = 'sites/${functionAppName}'

var notifyEmails = split(replace(excessiveUseNotifyEmail, ' ', ''), ',')
var notifyEmailReceivers = [for (email, i) in notifyEmails: {
  name: 'notify-${i}'
  emailAddress: email
  useCommonAlertSchema: true
}]

resource functionApp 'Microsoft.Web/sites@2024-04-01' existing = {
  name: functionAppName
}

resource notifyActionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: notifyActionGroupName
  location: 'global'
  tags: tags
  properties: {
    groupShortName: 'ExcessUse'
    enabled: true
    emailReceivers: notifyEmailReceivers
  }
}

resource armConnection 'Microsoft.Web/connections@2018-07-01-preview' = {
  name: armConnectionName
  location: location
  tags: tags
  properties: {
    displayName: 'arm-connection'
    authenticatedUser: {}
    parameterValueType: 'Alternative'
    api: {
      name: 'arm'
      displayName: 'Azure Resource Manager'
      category: 'Standard'
      type: 'Microsoft.Web/locations/managedApis'
      id: azureManagedConnectorResourceId
    }
  }
}

resource stopLogicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: logicAppName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    state: 'Enabled'
    definition: {
      '$schema': 'https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#'
      contentVersion: '1.0.0.0'
      parameters: {
        '$connections': {
          defaultValue: {}
          type: 'Object'
        }
      }
      triggers: {
        When_a_HTTP_request_is_received: {
          type: 'Request'
          kind: 'Http'
          inputs: {
            method: 'POST'
          }
        }
      }
      actions: {
        Invoke_resource_operation: {
          runAfter: {}
          type: 'ApiConnection'
          inputs: {
            host: {
              connection: {
                name: '@parameters(\'$connections\')[\'arm\'][\'connectionId\']'
              }
            }
            method: 'post'
            path: '/subscriptions/@{encodeURIComponent(\'${subscription().subscriptionId}\')}/resourcegroups/@{encodeURIComponent(\'${resourceGroup().name}\')}/providers/@{encodeURIComponent(\'Microsoft.Web\')}/@{encodeURIComponent(\'${functionAppPath}\')}/@{encodeURIComponent(\'stop\')}'
            queries: {
              'x-ms-api-version': '2021-03-01'
            }
          }
        }
      }
      outputs: {}
    }
    parameters: {
      '$connections': {
        value: {
          arm: {
            id: azureManagedConnectorResourceId
            connectionId: armConnection.id
            connectionName: armConnectionName
            connectionProperties: {
              authentication: {
                type: 'ManagedServiceIdentity'
              }
            }
          }
        }
      }
    }
  }
}

resource stopRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, stopLogicApp.id, functionAppStopRoleDefinitionId)
  properties: {
    roleDefinitionId: functionAppStopRoleDefinitionId
    principalId: stopLogicApp.identity.principalId
    principalType: 'ServicePrincipal'
    description: 'Logic App may stop the Function App on the cost-protection alert'
  }
}

resource stopActionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: stopActionGroupName
  location: 'global'
  tags: tags
  properties: {
    groupShortName: 'FnStop'
    enabled: true
    logicAppReceivers: [
      {
        name: 'StopWorkload'
        resourceId: stopLogicApp.id
        callbackUrl: listCallbackURL('${stopLogicApp.id}/triggers/When_a_HTTP_request_is_received', '2019-05-01').value
        useCommonAlertSchema: true
      }
    ]
  }
}

resource warnAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: warnAlertName
  location: 'global'
  tags: tags
  properties: {
    description: 'High OnDemandFunctionExecutionCount — notify (excess use).'
    severity: 2
    enabled: true
    scopes: [
      functionApp.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          name: 'Excessive on-demand executions'
          criterionType: 'StaticThresholdCriterion'
          metricName: 'OnDemandFunctionExecutionCount'
          metricNamespace: 'Microsoft.Web/sites'
          operator: 'GreaterThan'
          threshold: warningThreshold
          timeAggregation: 'Total'
          skipMetricValidation: false
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
    }
    actions: [
      {
        actionGroupId: notifyActionGroup.id
      }
    ]
    autoMitigate: true
    targetResourceType: 'Microsoft.Web/sites'
    targetResourceRegion: resourceGroup().location
  }
}

resource stopAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: stopAlertName
  location: 'global'
  tags: tags
  properties: {
    description: 'OnDemandFunctionExecutionCount reached the stop threshold — stop the Function App.'
    severity: 0
    enabled: true
    scopes: [
      functionApp.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          name: 'Stop threshold on-demand executions'
          criterionType: 'StaticThresholdCriterion'
          metricName: 'OnDemandFunctionExecutionCount'
          metricNamespace: 'Microsoft.Web/sites'
          operator: 'GreaterThan'
          threshold: stopThreshold
          timeAggregation: 'Total'
          skipMetricValidation: false
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
    }
    actions: [
      {
        actionGroupId: stopActionGroup.id
      }
    ]
    autoMitigate: true
    targetResourceType: 'Microsoft.Web/sites'
    targetResourceRegion: resourceGroup().location
  }
}
