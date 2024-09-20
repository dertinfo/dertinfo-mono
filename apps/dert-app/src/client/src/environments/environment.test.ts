// Note that the settings here are used when the application deploys via
// The Azure pipelines to the test envionment. 
// This deployment does not use docker containers and teh deployment builds the application
// using the Angular CLI.

export const environment = {
  production: true,
  apiUrl: 'https://dertinfo-test-api-wa.azurewebsites.net/api',
  auth0CallbackUrl: "https://staging-app.dertinfo.com",
  allowedDomains: [
    "dertinfo-api-test.azurewebsites.net",
    "dertinfo-test-api-wa.azurewebsites.net",
  ]
};
