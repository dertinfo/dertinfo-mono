# Planned: Remove temporary `workflow_dispatch` from src CI/CD

**Status:** Not started — after hosted Azure settings (App Service names, OIDC, App Configuration) are validated by a few manual deploys.

**Related:** [CI/CD](../../technical/infra/cicd.md), [GitHub Flow](../../../.cursor/rules/github-flow.mdc).

---

## Intent

Src pipelines should run only when application code (or the workflow files themselves) changes: **CI** on PRs into `main`, **CD** on push to `main` (path-filtered). Manual **Run workflow** is a temporary way to deploy while testing new-stack Azure settings without dummy commits.

## Why

`workflow_dispatch` on src CD lets us zip-deploy the API (and the other apps) to the new App Services after infra exists, even when `apps/dert-api/**` has not changed. Once those settings are confirmed, the extra trigger is unused surface and easy to fire by accident.

## Scope (when scheduled)

1. Remove `on.workflow_dispatch` from:
   - [`api-src-ci.yml`](../../../.github/workflows/api-src-ci.yml), [`web-src-ci.yml`](../../../.github/workflows/web-src-ci.yml), [`app-src-ci.yml`](../../../.github/workflows/app-src-ci.yml), [`functions-src-ci.yml`](../../../.github/workflows/functions-src-ci.yml)
   - [`api-src-cd.yml`](../../../.github/workflows/api-src-cd.yml), [`web-src-cd.yml`](../../../.github/workflows/web-src-cd.yml), [`app-src-cd.yml`](../../../.github/workflows/app-src-cd.yml), [`functions-src-cd.yml`](../../../.github/workflows/functions-src-cd.yml)
2. Remove the `if:` conditions that special-case `workflow_dispatch` / `inputs.target` on src CD jobs (`docker-dev`, `deploy-dev`, `docker-prod`, `deploy-prod`).
3. Drop the temporary-dispatch notes from [CI/CD](../../technical/infra/cicd.md) and [`.cursor/rules/github-flow.mdc`](../../../.cursor/rules/github-flow.mdc).
4. Mark this planned-fix done (or delete the page and index row).

## Out of scope

- **Infra CD** (`*-infra-cd.yml`, `subscription-infra-cd.yml`) already has `workflow_dispatch` (`full` / `dev-only`) for Bicep re-runs without template diffs. Keep that; it is not part of this removal.
- Reusable `workflow_call` templates (`reusable-src-*.yml`, `reusable-infra-*.yml`) — they are not triggered directly.

## How to run while this is still in place

The **Run workflow** button appears only after the workflow file exists on the **default branch**. Merge the PR that added dispatch, then Actions → the src CD workflow → **Run workflow**, default target **`dev-only`**. Use **`full`** only when production should run too (still gated by the `production` Environment).
