/*
Web — production leaf.
az deployment group create --resource-group rg-prd-dertinfo-web-uks --template-file main.bicep --parameters main.prod.bicepparam
Requires Bicep CLI 0.44.1+.
environmentTag stays 'prd'.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'prd'
param staticWebAppName = 'swa-prd-dertinfo-web-uks'
param prerequisitesExist = false
param customDomainReady = false
param customDomainName = ''
