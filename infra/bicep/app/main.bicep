/*
App (PWA) workload: Azure Static Web App (Free) when prerequisitesExist.
Deploy: az deployment group create --resource-group rg-<env>-dertinfo-app-uks --template-file main.bicep --parameters main.<env>.bicepparam
Requires Bicep CLI 0.44.1+.
*/

targetScope = 'resourceGroup'

// #####################################################
// Parameters
// #####################################################

@description('Azure region for the Static Web App. Microsoft.Web/staticSites is not available in uksouth; westeurope is the nearest supported region. Resource groups stay uksouth.')
param location string = 'westeurope'

@description('Environment tag (dev or prd).')
@allowed([
  'dev'
  'prd'
])
param environmentTag string = 'dev'

@description('Product slug used in resource names.')
param productSlug string = 'dertinfo'

@description('Static Web App name.')
param staticWebAppName string = ''

@description('Custom hostname to bind after DNS exists (e.g. app-dev.dertinfo.co.uk). Empty until a name is chosen.')
param customDomainName string = ''

// Flip in a leaf after CNAME/TXT exist; first deploy must leave this false.
@description('When true, bind customDomainName on the Static Web App. Fails until DNS validation records exist.')
param customDomainReady bool = false

// Flip in main.shared.bicepparam (or a leaf) after these exist; workflows do not detect them:
// - API App Service app-<env>-dertinfo-api-uks (hosted API the PWA calls)
@description('When true, deploy the Static Web App. When false, succeed with no app resources.')
param prerequisitesExist bool = false

@description('Disable AVM telemetry.')
param enableTelemetry bool = false

// #####################################################
// Variables
// #####################################################

var regionTlaByLocation = {
  uksouth: 'uks'
  westeurope: 'weu'
  eastus: 'eus'
  northeurope: 'neu'
}

var regionTla = regionTlaByLocation[location]

var resourceTags = {
  environment: environmentTag
  product: productSlug
  part: 'app'
  iac: 'infra-bicep-app'
}

var customDomainsToBind = (customDomainReady && !empty(customDomainName)) ? [
  customDomainName
] : []

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

module staticSite 'br/public:avm/res/web/static-site:0.9.0' = if (prerequisitesExist) {
  name: 'avm-swa-app-${environmentTag}'
  params: {
    name: staticWebAppName
    location: location
    sku: 'Free'
    customDomains: customDomainsToBind
    tags: resourceTags
    enableTelemetry: enableTelemetry
  }
}

// #####################################################
// Outputs
// #####################################################

output staticWebAppName string = staticSite.?outputs.name ?? ''
output staticWebAppResourceId string = staticSite.?outputs.resourceId ?? ''
output defaultHostname string = staticSite.?outputs.defaultHostname ?? ''
output environmentTag string = environmentTag
output regionTla string = regionTla
