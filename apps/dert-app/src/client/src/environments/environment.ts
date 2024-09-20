// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  apiUrl: 'http://localhost:44100/api',
  auth0CallbackUrl: "http://localhost:44300",
  allowedDomains: [
    "localhost:60280",
    "localhost:44100",
    "dertinfo-api-test.azurewebsites.net",
    "dertinfo-test-api-wa.azurewebsites.net",
    "dertinfo-api-live.azurewebsites.net",
    "dertinfo-live-api-wa.azurewebsites.net"
  ]
};

