// Note that the settings here are used when the application deploys via
// The Azure pipelines to the live envionment. 
// This deployment does not use docker containers and teh deployment builds the application
// using the Angular CLI.

export const environment = {
  production: true,
  apiUrl: 'https://dertinfo-live-api-wa.azurewebsites.net/api',
  auth0CallbackUrl: "https://app.dertinfo.com",
  allowedDomains: [
    "dertinfo-live-api-wa.azurewebsites.net"
  ]
};