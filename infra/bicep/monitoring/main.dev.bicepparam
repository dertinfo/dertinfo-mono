/*
Monitoring — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-monitoring-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param logAnalyticsWorkspaceName = 'log-dev-dertinfo-monitoring-uks'
param applicationInsightsName = 'appi-dev-dertinfo-monitoring-uks'
