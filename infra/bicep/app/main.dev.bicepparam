/*
App (PWA) — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-app-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param staticWebAppName = 'swa-dev-dertinfo-app-uks'
param prerequisitesExist = true
// Flip true after CNAME/TXT for app-dev.dertinfo.co.uk exist, then re-run app infra CD.
param customDomainReady = true
param customDomainName = 'app-dev.dertinfo.co.uk'
