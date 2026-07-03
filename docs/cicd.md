# CI/CD

How continuous integration and deployment are organised in the DertInfo monorepo.

## Overview

| Concern | Workflows | Trigger |
|---------|-----------|---------|
| **CI** (build, test, lint) | `*-ci.yml` | `main`, `develop`, pull requests (path-filtered) |
| **CD** (deploy + Docker Hub) | `*-cd.yml` | `main` only (path-filtered) |

CD workflows replace the per-app Azure DevOps pipelines under `apps/*/pipelines/`. Each `*-cd.yml` merges the former **source deploy** and **Docker Hub** pipelines into one workflow.

Legacy ADO definitions remain in the repo as reference until GitHub Actions is validated in production use.

## Environments

GitHub Environments use **`test`** and **`prod`** to align with Azure infrastructure naming.

| GitHub Environment | ADO stage | Azure subscription (from ADO) |
|--------------------|-----------|-------------------------------|
| `test` | `Test` | Visual Studio Professional Subscription |
| `prod` | `Live` | DertInfo Subscription *(not wired yet)* |

ADO variable suffixes `*_Stg` / `*_Prod` map to `test` / `prod` in GitHub. Azure resource names may still mix `test` and `stg` suffixes from historical deployments — store app/resource names as **environment variables**, not secrets.

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

Reusable deploy workflows accept an `environment` input (`test` \| `prod`) so production jobs can be added later without restructuring.

## Workflows

### Reusable templates (`.github/workflows/reusable/`)

| Workflow | Purpose |
|----------|---------|
| `build-push-docker.yml` | Build and push to Docker Hub (`latest` + run id) |
| `deploy-dotnet-appservice.yml` | OIDC login + zip/folder deploy to App Service |
| `deploy-static-web-app.yml` | Deploy to Azure Static Web Apps |

### Per-app CD

| Workflow | Build | Docker image | Deploy target |
|----------|-------|--------------|---------------|
| `api-cd.yml` | .NET `win-x86` publish + unit tests | `dertinfo/dertinfo-api` | API App Service (`test`) |
| `web-cd.yml` | SWA Oryx build | `dertinfo/dertinfo-web` | Static Web App (`test`) |
| `app-cd.yml` | SWA Oryx build | `dertinfo/dertinfo-app` | Static Web App (`test`) |
| `functions-cd.yml` | .NET publish | `dertinfo/dertinfo-imageresizev4` | Function App (`test`) |

Docker images are for **local development** (root `docker-compose.yml`, Codespaces). Hosted Azure deployments use native App Service / SWA deploy, not containers.

## GitHub setup checklist

Complete these steps in the GitHub repository **before the first CD run**.

### 1. Create GitHub Environments

- **`test`** — required now
- **`prod`** — optional; create when ready for production cutover

### 2. Azure OIDC (recommended)

For each environment, create an Entra ID app registration with a federated credential:

- **Issuer:** `https://token.actions.githubusercontent.com`
- **Subject:** `repo:<org>/<repo>:ref:refs/heads/main` (adjust org/repo)
- **Audience:** `api://AzureADTokenExchange`

Grant the app **Contributor** on the target resource group(s) or individual web apps.

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

- **Target env:** `STG` for GitHub environment `test` (ADO staging); `PRD` for GitHub environment `prod`.
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
2. Confirm the workflow completes: artifact deploy to App Service / SWA, Docker Hub tags `latest` and run id.
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

See [planned-fixes/cicd-future-phase.md](planned-fixes/cicd-future-phase.md) for IaC migration.

## ADO pipeline inventory (legacy)

| App | Source deploy | Docker |
|-----|---------------|--------|
| API | `apps/dert-api/pipelines/azure-pipelines-api-cicd.yml` | `azure-pipelines-docker.yml` |
| Web | `apps/dert-web/pipelines/azure-pipelines-swa-cicd.yml` | `azure-pipelines-docker.yml` |
| App | `apps/dert-app/pipelines/ado-application-pipeline-cicd.yml` | `azure-pipelines-docker.yml` |
| Functions | `apps/dert-functions/pipelines/azure-pipelines-functions-cicd.yml` | `azure-pipelines-docker.yml` |
| Functions IaC | `apps/dert-functions/pipelines/azure-pipelines-infra.yml` | — |
