# CI/CD future phase

Deferred work after GitHub Actions CD is stable on the **`test`** environment.

## 0. Upgrade dert-app to Angular 14

Align the Ionic PWA (`apps/dert-app/`) with the website (`apps/dert-web/`) on **Angular 14** so both frontends share a compatible major version. Today the app remains on Angular 13 with an isolated `npm install` under `apps/dert-app/src/client` (no npm workspaces) to avoid dependency clashes in CI.

Scope when undertaken:

- Bump `@angular/*`, `@angular-devkit/*`, `@angular-eslint/*`, and `@ionic/angular` to versions compatible with Angular 14
- Update `angular.json`, TypeScript, and any breaking API migrations
- Re-evaluate whether a shared root `package.json` workspace is desirable after versions align

## 1. Rename test → development

Align GitHub, frontend, and Azure naming when the test environment is retired in favour of a single non-prod **development** environment:

- GitHub Environments: `test` → `development`
- Secret names: `TEST_*` → `DEV_*` (or equivalent)
- Angular: `ado-build-ui-test` / `test` build configuration → `development`
- `environment.test.ts` → `environment.development.ts` (or equivalent)
- Azure resource tags and URLs where applicable

Keep **`prod`** for production.

## 2. Infrastructure as code

Migrate deployment of Azure resources from ADO to GitHub Actions.

### Functions (existing Bicep)

- Source: [`apps/dert-functions/pipelines/azure-pipelines-infra.yml`](../../apps/dert-functions/pipelines/azure-pipelines-infra.yml)
- Bicep: [`apps/dert-functions/infra/bicep/`](../../apps/dert-functions/infra/bicep/)
- Capture test/prod parameters as GitHub environment variables from:
  - `DertInfoImageResizeV4_Infrastucture_Staging_VariablesGroup` (test)
  - `DertInfoImageResizeV4_Infrastucture_Production_VariablesGroup` (prod)
- Add a reusable `deploy-bicep.yml` workflow; wire `functions-infra-cd.yml` on `main` when `infra/bicep/**` changes

### API, Web, App

No IaC exists in the repo today. Future work:

- Define Bicep (or Terraform) modules per app
- Provision App Service, Static Web Apps (or replacements), monitoring, and networking
- Align parameter naming with `test` / `prod` (and later `development` / `prod`)

## 3. Web and app delivery review

Evaluate alternatives to Azure Static Web Apps as part of the development environment redesign:

- Container-based hosting (Docker images already published to Docker Hub)
- Azure App Service
- Storage + CDN
- Hybrid (e.g. static assets on CDN, API on App Service)

The current SWA + Oryx build path remains until this review completes.

## 4. Production CD on GitHub Actions

- Wire `prod` environment jobs in each `*-cd.yml` (or separate promotion workflow)
- Add GitHub Environment protection rules (required reviewers, wait timers)
- Map `PROD_*` secrets and prod subscription OIDC variables

## 5. ADO decommission

After one full release cycle on GitHub Actions:

- Disable ADO pipeline triggers
- Retire ADO variable groups and service connections
- Archive or remove `apps/*/pipelines/` ADO YAML (optional; can keep as history)

## Related docs

- [cicd.md](../cicd.md) — current GitHub Actions setup and secrets checklist
