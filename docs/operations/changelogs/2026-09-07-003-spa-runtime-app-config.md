# SPA runtime app.config.json (build once, inject per environment)

## Summary of the work completed

Website and PWA clients always load API base, Auth0 callback, and JWT allowed domains from `assets/app.config.json`. `environment.ts` / `environment.prod.ts` only toggle Angular production mode. `environment.test.ts` is removed. GitHub CD builds each SPA once (`npm run build:hosted`) and writes `app.config.json` at deploy time for `development` and `production`. Docs: [Configuration](../../technical/infra/configuration.md), [CI/CD](../../technical/infra/cicd.md).

## Why the work was completed

Hosted API URLs were compiled in via Angular `fileReplacements`, so DEV and PRD needed different builds and CD could ship the wrong host if the `--configuration` script was wrong. Runtime JSON is the same path as local and Docker, and lets one minified artefact be promoted with only the JSON changed.

## Date the work was started

2026-09-07

## Date the work was completed

2026-09-07

## Issues that were encountered on the way

- Angular minification (`angular.json` `production` configuration) is separate from `environment.production`. Hosted CD still uses `--configuration production` so `enableProdMode()` runs; `ng serve` stays on the default unminified build.
- Azure Static Web Apps Oryx rebuilds from source unless `skip_app_build` is set. CD now uploads `dist` (including `staticwebapp.config.json`) and deploys that artefact.

## References to any best practices that we found

- [Angular application environments](https://angular.dev/tools/cli/environments) — file replacements for build mode, not for per-environment URLs
- [Azure Static Web Apps skip_app_build](https://learn.microsoft.com/en-us/azure/static-web-apps/build-configuration?tabs=github-actions#skip-building-the-front-end-app)

## Any remaining issues that we may wish to address

- Set `AZURE_STATICWEBAPP_WEB_CALLBACKURL` and `AZURE_STATICWEBAPP_APP_CALLBACKURL` on GitHub Environments `development` and `production` before the next SWA deploy (Auth0 Allowed Callback URLs must match).
- Confirm Auth0 tenants allow the new-stack callback hosts (SWA default hostname or custom domain).
- Legacy ADO SWA pipelines still compile from source and do not inject hosted JSON; GitHub Actions is the canonical path.
