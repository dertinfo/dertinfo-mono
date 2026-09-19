/*
App (PWA) — production leaf.
az deployment group create --resource-group rg-prd-dertinfo-app-uks --template-file main.bicep --parameters main.prod.bicepparam
Requires Bicep CLI 0.44.1+.
environmentTag stays 'prd'.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param staticWebAppName = 'swa-prd-dertinfo-app-uks'
param prerequisitesExist = false
param customDomainReady = false
param customDomainName = 'app.dertinfo.co.uk'
