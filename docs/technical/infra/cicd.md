# CI/CD

How continuous integration and deployment are organised in the DertInfo monorepo.

Completed CD migration work is recorded in the [change log](changelogs/README.md) (see [2026-07-03 GitHub Actions CD pipelines](changelogs/2026-07-03-001-github-actions-cd-pipelines.md)).

## Overview

| Concern | Workflows | Trigger |
|---------|-----------|---------|
| **CI** (build, test, lint) | `*-src-ci.yml` | Pull requests **into `main`** (path-filtered). GitHub Flow — no `develop` branch. |
| **Src CD** (app deploy + Docker Hub) | `*-src-cd.yml` | Push to **`main`** after merge (path-filtered) |
| **Infra CD** (Bicep) | `*-infra-cd.yml` | Push to **`main`** after merge (path-filtered) |

Src CD workflows replace the per-app Azure DevOps pipelines under `apps/*/pipelines/`. Each `*-src-cd.yml` merges the former **source deploy** and **Docker Hub** pipelines into one workflow.

Legacy ADO definitions remain in the repo as reference until GitHub Actions is validated in production use.

## Environments

**New stack** (this monorepo) uses GitHub Environments **`development`** and **`production`**, matching Azure tags `dev` / `prd`. Live traffic today is still deployed from **separate repositories + Azure DevOps**; these Environments are the GitHub Actions + Bicep path.

| GitHub Environment | Azure tag | Typical RG prefix |
|--------------------|-----------|-------------------|
| `development` | `dev` | `rg-dev-dertinfo-*-uks` |
| `production` | `prd` | `rg-prd-dertinfo-*-uks` |

Each Environment has its **own** Entra app (OIDC) and subscription id. Both require approval from `dertinfo` or `davidsmonkeys` before deploy jobs run.

OIDC and naming for the older ADO-mapped `test` / `prod` Environments are historical; new workflows do not use them. See [GitHub Actions OIDC](../guides/github-azure-federated-credentials.md).

### ADO → GitHub mapping

Naming follows `[SERVICEPROVIDER]_[SERVICETYPE]_[WORKLOADNAME]_[DESCRIPTION]_[TARGETENV]` — see [naming convention](#naming-convention) below. `STG` maps to GitHub environment **`test`**; `PRD` maps to **`prod`**.

| Layer | GitHub (`test`) | ADO |
|-------|-----------------|-----|
| API App Service | `AZURE_WEBAPP_API_RESOURCENAME_STG` (variable) | `CD_Pipeline.TestEnvApiWebAppName` |
| Functions App | `AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME_STG` (variable) | `DertInfoImageResizeV4_VariablesGroup.FunctionAppName_Stg` |
| Web SWA deploy token | `AZURE_STATICWEBAPP_WEB_DEPLOYTOKEN_STG` (secret) | `staging_deployment_token` |
| App SWA deploy token | `AZURE_STATICWEBAPP_APP_DEPLOYTOKEN_STG` (secret) | `staging_deployment_token` |
| Angular build | `npm run ado-build-ui-test` | unchanged |
| Hosted URLs | `staging.dertinfo.co.uk`, etc. | unchanged |

### `prod` placeholders (not wired yet)

| Kind | Name | ADO source |
|------|------|------------|
| Variable | `AZURE_WEBAPP_API_RESOURCENAME_PRD` | `LiveEnvApiWebAppName` |
| Variable | `AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME_PRD` | `FunctionAppName_Prod` |
| Secret | `AZURE_STATICWEBAPP_WEB_DEPLOYTOKEN_PRD` | `live_deployment_token` |
| Secret | `AZURE_STATICWEBAPP_APP_DEPLOYTOKEN_PRD` | `live_deployment_token` |

Reusable deploy workflows accept an `environment` input (`development` \| `production`).

Callers that use reusable workflows and need repository secrets must set `secrets: inherit` (or pass secrets explicitly). Secrets are not available inside `workflow_call` jobs by default.

## Workflows

### Reusable templates (`.github/workflows/reusable-src-*.yml` / `reusable-infra-*.yml`)

GitHub requires reusable workflows at the **top level** of `.github/workflows/` (not in a subfolder).

| Workflow | Purpose |
|----------|---------|
| `reusable-src-build-push-docker.yml` | Build and push to Docker Hub (`latest-dev` / `{run_id}-dev` for `development`; `latest` / `{run_id}` for `production`) |
| `reusable-src-deploy-dotnet-appservice.yml` | OIDC login + zip/folder deploy to App Service |
| `reusable-src-deploy-static-web-app.yml` | Deploy to Azure Static Web Apps |
| `reusable-infra-deploy-bicep-resourcegroup.yml` | OIDC + `az deployment group create` (workload infra) |
| `reusable-infra-deploy-bicep-subscription.yml` | OIDC + `az deployment sub create` (subscription foundation) |

### Infrastructure CD

| Workflow | Scope | Notes |
|----------|-------|-------|
| `subscription-infra-cd.yml` | Subscription | Privileged SP **per Environment**; RGs + policy — [agent-safe subscription foundation](../../operations/planned-fixes/agent-safe-subscription-foundation.md) |
| `config-infra-cd.yml` | `rg-<env>-dertinfo-config-uks` | Key Vault + App Configuration |
| `monitoring-infra-cd.yml` | `rg-<env>-dertinfo-monitoring-uks` | Log Analytics (1 GB/day) + Application Insights |
| `storage-infra-cd.yml` | `rg-<env>-dertinfo-storage-uks` | Images SA always; SQL when `prerequisitesExist` |
| `api-infra-cd.yml` | `rg-<env>-dertinfo-api-uks` | Windows App Service when `prerequisitesExist` |

Bicep house rules: [Bicep standards](../standards/bicep/). Operator scripts: [`infra/scripts/`](../../../infra/scripts/).

### Per-app src CD

| Workflow | Build | Docker image | Deploy target |
|----------|-------|--------------|---------------|
| `api-src-cd.yml` | .NET `win-x86` publish; unit tests **gate** deploy | `dertinfo/dertinfo-api` | New-stack API App Service (`development` then gated `production`) |
| `web-src-cd.yml` | SWA Oryx build | `dertinfo/dertinfo-web` | Static Web App (`development` / `production`) — needs SWA tokens |
| `app-src-cd.yml` | SWA Oryx build | `dertinfo/dertinfo-app` | Static Web App (`development` / `production`) — needs SWA tokens |
| `functions-src-cd.yml` | .NET publish | `dertinfo/dertinfo-imageresizev4` | Function App (`development` / `production`) — needs Function resource |

Docker images are for **local development** (root `docker-compose.yml`, Codespaces). Hosted Azure deployments use native App Service / SWA deploy, not containers.

## GitHub setup checklist

Complete these steps in the GitHub repository **before the first CD run**.

### 0. Protect `main` (GitHub Flow)

CI on a PR does not block merge by itself. On `main`, require a pull request and require the relevant `*-src-ci.yml` job names to pass (and be up to date) before merge.

Path-filtered CI jobs are **skipped** when the PR does not touch those paths. Prefer a ruleset that treats skipped checks as non-blocking, or do not mark a path-specific job as required unless every PR is expected to run it.

Branching rule: [`.cursor/rules/github-flow.mdc`](../../../.cursor/rules/github-flow.mdc). Guide: [Contributing workflow](../guides/contributing-workflow.md).

### 1. Create GitHub Environments

- **`development`** and **`production`** — required for new-stack infra and app CD
- Set Environment-scoped `AZURE_ENTRA_OIDC_CLIENTID`, `AZURE_ENTRA_OIDC_TENANTID`, `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID`
- Set `AZURE_WEBAPP_API_RESOURCENAME` = `app-<env>-dertinfo-api-uks` after API infra exists
- SWA tokens: `AZURE_STATICWEBAPP_WEB_DEPLOYTOKEN_DEV` / `_PRD` (and the `APP` equivalents) when those sites exist

### 2. Azure OIDC (recommended)

GitHub Actions signs in to Azure with **OIDC federated credentials** (no client secret). The token **subject must match the job’s GitHub Environment** (for example `repo:dertinfo/dertinfo-mono:environment:development`), not only a branch ref.

**New-stack Environments (`development` / `production`):** apply the checked-in JSON in [`infra/configuration/`](../../../infra/configuration/) via [`New-DertInfoSubscriptionOidcIdentities.ps1`](../../../infra/scripts/New-DertInfoSubscriptionOidcIdentities.ps1) (or manually) as described in [GitHub Actions OIDC to Azure](../guides/github-azure-federated-credentials.md). Use a **separate** Entra app registration per Environment; set that Environment’s `AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION` to the matching client id.

**Existing app CD (`test` / `prod`):** create an Entra app registration per environment with a federated credential (same issuer and audience; subject `…:environment:test` or `…:environment:prod`). Grant the app **Contributor** on the target resource group(s) or individual web apps.

On the GitHub **`test`** environment, set **variables**:

| Variable | Description |
|----------|-------------|
| `AZURE_ENTRA_OIDC_CLIENTID_STG` | App registration client ID |
| `AZURE_ENTRA_OIDC_TENANTID_STG` | Entra tenant ID |
| `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID_STG` | Test subscription ID (`9ee4f83c-a9a6-41a0-822d-13e18dc6c648` from ADO) |

Repeat for **`prod`** with `_PRD` names and the DertInfo subscription when production deploy is enabled.

### 3. Variables and secrets

Use **variables** for non-sensitive configuration (visible in workflow logs and the GitHub UI). Use **secrets** only for tokens, passwords, and deployment keys.

#### Naming convention

Use **UPPER_SNAKE_CASE** with this pattern for Azure (and similar cloud) values:

```
[SERVICEPROVIDER]_[SERVICETYPE]_[WORKLOADNAME]_[DESCRIPTION]_[TARGETENV]
```

- **Target env:** three-letter acronym only — `STG` for GitHub environment `test` (ADO staging); `DEV` for development; `PRD` for production. **Never `PROD`.** See [`.cursor/rules/env-acronyms.mdc`](../../../.cursor/rules/env-acronyms.mdc).
- **Third-party, non-env-specific** accounts (e.g. Docker Hub) may use `[PROVIDER]_[DESCRIPTION]` — `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN`.

Avoid prefixing names with `TEST_` or suffixing with `_TEST`; put the environment last as `STG` / `PRD` so the name reads as *what* + *where*.

**Repository variables** (shared across environments):

| Variable | Example | Notes |
|----------|---------|-------|
| `DOCKERHUB_USERNAME` | `dertinfo` | Public Docker Hub org/user — not a secret |

**Repository secrets:**

| Secret | Notes |
|--------|-------|
| `DOCKERHUB_TOKEN` | Docker Hub access token |

**`test` environment variables:**

| Variable | ADO source |
|----------|------------|
| `AZURE_ENTRA_OIDC_CLIENTID_STG` | Entra app registration client ID |
| `AZURE_ENTRA_OIDC_TENANTID_STG` | Entra tenant ID |
| `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID_STG` | Test subscription ID |
| `AZURE_WEBAPP_API_RESOURCENAME_STG` | `CD_Pipeline.TestEnvApiWebAppName` |
| `AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME_STG` | `FunctionAppName_Stg` |

**`test` environment secrets:**

| Secret | ADO source |
|--------|------------|
| `AZURE_STATICWEBAPP_WEB_DEPLOYTOKEN_STG` | Web SWA `staging_deployment_token` |
| `AZURE_STATICWEBAPP_APP_DEPLOYTOKEN_STG` | App SWA `staging_deployment_token` |

If migrating from an earlier setup, rename (do not duplicate) old names such as `TEST_API_WEBAPP_NAME`, `AZURE_CLIENT_ID`, or `AZURE_STATIC_WEB_APPS_API_TOKEN_TEST`.

### 4. Validate

After secrets are configured:

1. Push a change to `main` under one app path (or re-run the workflow from the Actions tab).
2. Confirm the workflow completes: artifact deploy to App Service / SWA, Docker Hub tags `latest-test` and `{run_id}-test` (test CD does not overwrite release tags).
3. Smoke-test hosted URLs (e.g. `staging.dertinfo.co.uk` for web).
4. Compare with the last successful ADO run for the same scope.
5. Leave ADO pipelines enabled until one full release cycle confirms GitHub Actions; then disable ADO triggers.

## Functions infrastructure reference (`test`)

From ADO variable group `DertInfoImageResizeV4_Infrastucture_Staging_VariablesGroup` (used by the Bicep infra pipeline — **not migrated to GitHub Actions in this phase**):

| Variable | Value |
|----------|-------|
| `resourceGroupName` | `di-rg-imageresizev4-stg` |
| `location` | `uksouth` |
| `ownerInitials` | `di` |
| `workloadName` | `imageresizev4` |
| `environmentTag` | `stg` |
| `imagesStorageAccountName` | `dertinfotestimagessa` |
| `imagesStorageAccountResourceGroupName` | `dertinfo-test-rg` |
| `applicationInsightsName` | `dertinfo-test-ais` |
| `applicationInsightsResourceGroupName` | `di-rg-monitoring-stg` |
| `excessiveUseActionGroupName` | `di-agrp-excessiveuse-stg` |
| `excessiveUseActionGroupResourceGroupName` | `di-rg-monitoring-stg` |

See [planned-fixes/cicd-future-phase.md](../../operations/planned-fixes/cicd-future-phase.md) for IaC migration.

## ADO pipeline inventory (legacy)

| App | Source deploy | Docker |
|-----|---------------|--------|
| API | `apps/dert-api/pipelines/azure-pipelines-api-cicd.yml` | `azure-pipelines-docker.yml` |
| Web | `apps/dert-web/pipelines/azure-pipelines-swa-cicd.yml` | `azure-pipelines-docker.yml` |
| App | `apps/dert-app/pipelines/ado-application-pipeline-cicd.yml` | `azure-pipelines-docker.yml` |
| Functions | `apps/dert-functions/pipelines/azure-pipelines-functions-cicd.yml` | `azure-pipelines-docker.yml` |
| Functions IaC | `apps/dert-functions/pipelines/azure-pipelines-infra.yml` | — |
