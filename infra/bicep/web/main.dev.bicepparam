/*
Web — development leaf.
az deployment group create --resource-group rg-dev-dertinfo-web-uks --template-file main.bicep --parameters main.dev.bicepparam
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
param staticWebAppName = 'swa-dev-dertinfo-web-uks'
param customDomainName = 'dev.dertinfo.co.uk'
param prerequisitesExist = true
// Flip true after CNAME/TXT for dev.dertinfo.co.uk exist, then re-run web infra CD.
param customDomainReady = false
