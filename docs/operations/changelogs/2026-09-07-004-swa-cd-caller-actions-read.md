# SWA CD caller `actions: read` for nested artefact download

## Summary of the work completed

Web and App Src CD caller jobs (`deploy-dev` / `deploy-prod`) now set `permissions: contents: read` and `actions: read` so they can call [`reusable-src-deploy-static-web-app.yml`](../../../.github/workflows/reusable-src-deploy-static-web-app.yml). That reusable job downloads the prebuilt SPA artefact (`actions/download-artifact`). Same pattern as API Src CD passing `id-token: write` into the App Service reusable workflow.

## Why the work was completed

After [2026-09-07-003](./2026-09-07-003-spa-runtime-app-config.md) merged, GitHub rejected both CD workflows: the nested `deploy` job requested `actions: read` but the caller only allowed `actions: none` (org/repo default GITHUB_TOKEN). Reusable workflow permissions are the **intersection** of caller and callee.

## Date the work was started

2026-09-07

## Date the work was completed

2026-09-07

## Issues that were encountered on the way

- Setting `permissions` only on the reusable workflow is not enough. The **caller** job that uses `uses:` must grant the same scopes.

## References to any best practices that we found

- [Reusing workflows — access to nested job permissions](https://docs.github.com/en/actions/using-workflows/reusing-workflows#nested-reusable-workflow-limitations)
- [GITHUB_TOKEN permissions for reusable workflows](https://docs.github.com/en/actions/security-guides/automatic-token-authentication#permissions-for-the-github_token)

## Any remaining issues that we may wish to address

- Re-run Web and App Src CD (`dev-only`) after merge; Environment variables `AZURE_STATICWEBAPP_WEB_CALLBACKURL` / `AZURE_STATICWEBAPP_APP_CALLBACKURL` are still required for the JSON inject step.
