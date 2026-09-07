// Default (ng serve / ng build). Hosted CD uses --configuration production,
// which file-replaces this with environment.prod.ts so enableProdMode() runs.
// API URL, Auth0 callback, and allowed domains come from assets/app.config.json.

export const environment = {
  production: false
};
