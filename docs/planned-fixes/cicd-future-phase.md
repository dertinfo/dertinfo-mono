# CI/CD future phase

Deferred work after GitHub Actions CD is stable on the **`test`** environment.

## 0. Fix API tests for CI (priority)

### Unit tests

- **Passing:** `dertinfo-api.utests` (17), `dertinfo-services.utests` (19), `dertinfo-repository.utests` (2) — repository fixture updated to use a valid `Group` entity for EF Core in-memory nullability.

Unit tests run in a separate **`unit-test`** job in [`api-ci.yml`](../../.github/workflows/api-ci.yml) and [`api-cd.yml`](../../.github/workflows/api-cd.yml). CI passes build artefacts (`bin`/`obj`) from the **build** job for `--no-build` test runs. CD unit tests build test projects in parallel with the Windows publish job.

Fix the repository fixture, then gate deploy on `unit-test` if desired.

### Integration tests

- All integration tests fail without SQL Server (connection error on startup migration in `Startup.Configure`).
- The **`integration-test`** job exists in both workflows with `if: false` until a SQL Server service container (or equivalent) is added.

### When complete

- Enable `integration-test` jobs and add SQL Server to CI.
- Optionally require `unit-test` (and later `integration-test`) before `deploy-test` in `api-cd.yml`.

## 1. Upgrade dert-app to Angular 14

Align the Ionic PWA (`apps/dert-app/`) with the website (`apps/dert-web/`) on **Angular 14** so both frontends share a compatible major version. Today the app remains on Angular 13 with an isolated `npm install` under `apps/dert-app/src/client` (no npm workspaces) to avoid dependency clashes in CI.

Scope when undertaken:

- Bump `@angular/*`, `@angular-devkit/*`, `@angular-eslint/*`, and `@ionic/angular` to versions compatible with Angular 14
- Update `angular.json`, TypeScript, and any breaking API migrations
- Re-evaluate whether a shared root `package.json` workspace is desirable after versions align

## 2. Rename test → development

Align GitHub, frontend, and Azure naming when the test environment is retired in favour of a single non-prod **development** environment:

- GitHub Environments: `test` → `development`
- Variable/secret names: adopt `[PROVIDER]_[TYPE]_[WORKLOAD]_[DESCRIPTION]_[ENV]` with `DEV` target suffix (see [docs/cicd.md](../cicd.md#naming-convention))
- Angular: `ado-build-ui-test` / `test` build configuration → `development`
- `environment.test.ts` → `environment.development.ts` (or equivalent)
- Azure resource tags and URLs where applicable

Keep **`prod`** for production.

## 3. Infrastructure as code

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

## 4. Web and app delivery review

Evaluate alternatives to Azure Static Web Apps as part of the development environment redesign:

- Container-based hosting (Docker images already published to Docker Hub)
- Azure App Service
- Storage + CDN
- Hybrid (e.g. static assets on CDN, API on App Service)

The current SWA + Oryx build path remains until this review completes.

## 5. Production CD on GitHub Actions

- Wire `prod` environment jobs in each `*-cd.yml` (or separate promotion workflow)
- Add GitHub Environment protection rules (required reviewers, wait timers)
- Map `*_PRD` variables/secrets and prod subscription OIDC variables on GitHub environment `prod`

## 6. ADO decommission

After one full release cycle on GitHub Actions:

- Disable ADO pipeline triggers
- Retire ADO variable groups and service connections
- Archive or remove `apps/*/pipelines/` ADO YAML (optional; can keep as history)

## Related docs

- [cicd.md](../cicd.md) — current GitHub Actions setup and secrets checklist
