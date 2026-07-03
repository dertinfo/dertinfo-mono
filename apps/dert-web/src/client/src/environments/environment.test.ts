// Note that the settings here are used when the application deploys via
// The Azure pipelines to the test envionment. 
// This deployment does not use docker containers and teh deployment builds the application
// using the Angular CLI.

export const environment = {
  production: true,
  // Direct App Service (not APIM). Reintroduce APIM when that layer is ready.
  apiUrl: 'https://dertinfo-test-api-wa.azurewebsites.net/api',
  auth0CallbackUrl: "https://staging.dertinfo.co.uk",
  allowedDomains: [
    "dertinfo-test-api-wa.azurewebsites.net",
  ]
};
