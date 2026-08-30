/*
Monitoring workload: Log Analytics (1 GB/day cap) + workspace-based Application Insights.
Deploy: az deployment group create --resource-group rg-<env>-dertinfo-monitoring-uks --template-file main.bicep --parameters main.<env>.bicepparam
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

@description('Log Analytics workspace name.')
param logAnalyticsWorkspaceName string

@description('Application Insights component name.')
param applicationInsightsName string

@description('Daily ingest cap in GB. Lowest requested floor is 1.')
param dailyQuotaGb string = '1'

@description('Log Analytics retention in days.')
param dataRetention int = 30

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
  part: 'monitoring'
  iac: 'infra-bicep-monitoring'
}

// #####################################################
// References
// #####################################################

// #####################################################
// Resources
// #####################################################

// #####################################################
// Modules
// #####################################################

// #####################################################
// AVM Modules
// #####################################################

module logAnalytics 'br/public:avm/res/operational-insights/workspace:0.16.1' = {
  name: 'avm-log-${environmentTag}'
  params: {
    name: logAnalyticsWorkspaceName
    location: location
    skuName: 'PerGB2018'
    dataRetention: dataRetention
    dailyQuotaGb: dailyQuotaGb
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

module applicationInsights 'br/public:avm/res/insights/component:0.8.0' = {
  name: 'avm-appi-${environmentTag}'
  params: {
    name: applicationInsightsName
    location: location
    workspaceResourceId: logAnalytics.outputs.resourceId
    applicationType: 'web'
    kind: 'web'
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// Outputs
// #####################################################

output logAnalyticsWorkspaceName string = logAnalytics.outputs.name
output logAnalyticsWorkspaceId string = logAnalytics.outputs.resourceId
output applicationInsightsName string = applicationInsights.outputs.name
output applicationInsightsResourceId string = applicationInsights.outputs.resourceId
output applicationInsightsConnectionString string = applicationInsights.outputs.connectionString
output environmentTag string = environmentTag
output regionTla string = regionTla
