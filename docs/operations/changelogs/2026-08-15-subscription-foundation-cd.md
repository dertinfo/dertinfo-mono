# Subscription foundation Bicep and CD workflows (untested on merge)

## Summary of the work completed

Landed the **agent-safe Azure subscription foundation** into the monorepo so subscription-scope infra can be deployed via GitHub Actions (OIDC), with docs and parked follow-up planned-fixes.

**What shipped (code + docs):**

- Subscription Bicep under [`infra/bicep/subscription/`](../../../infra/bicep/subscription/) (`targetScope = 'subscription'`: RGs, allow-list policy, optional RG role assignments)
- Workflows: [`subscription-infra-cd.yml`](../../../.github/workflows/subscription-infra-cd.yml), [`reusable-deploy-bicep-subscription.yml`](../../../.github/workflows/reusable-deploy-bicep-subscription.yml), plus RG-scoped reusable stub
- Entra federated-credential JSON and guide: [`infra/configuration/`](../../../infra/configuration/), [GitHub Actions OIDC to Azure](../../technical/guides/github-azure-federated-credentials.md)
- Planned fix hub: [agent-safe-subscription-foundation.md](../planned-fixes/agent-safe-subscription-foundation.md); related estate / Bicep / cost notes **parked**
- CI/CD inventory updates in [cicd.md](../../technical/infra/cicd.md); GitHub Environments `development` / `production` with required reviewers `dertinfo` or `davidsmonkeys`

**Explicit acceptance on merge:** The subscription CD workflows and the subscription Bicep deploy have **not** been executed successfully end-to-end in GitHub Actions before merge to `main`. That is accepted **only because this stack is not yet used for production continuous delivery**. A **close follow-up** will run the workflows from `main`, fix any failures, and only then treat the foundation as verified.

This approach would be **risky if full CD already depended on these pipelines** — new workflow files must exist on the default branch before GitHub will list or dispatch them, so untested pipeline YAML inevitably lands on `main` first. Prefer validating templates locally (or via a break-glass CLI deploy) where possible, then iterate quickly with a dedicated follow-up PR after the first Actions runs.

### Pull requests

| PR | Title |
|----|--------|
| [#16](https://github.com/dertinfo/dertinfo-mono/pull/16) | Docs restructure, subscription foundation Bicep, and CD workflows |

## Date the work was started

2026-08-15

## Date the work was completed

2026-08-15

## Issues that were encountered on the way

1. **Cannot run a new `workflow_dispatch` workflow from a feature branch alone**  
   GitHub only surfaces (and API-dispatches) workflows that exist on the **default branch** (`main`). While `subscription-infra-cd.yml` lived only on the feature branch, it did not appear under Actions and there was no UI to “switch branch” for an unknown workflow.  
   **Implication:** Pipeline YAML must reach `main` before it can be tested in Actions — awkward for continuous delivery of infra pipelines. Mitigations considered: merge workflow files only then `Run workflow` with **Use workflow from** = feature branch; or validate Bicep with `az deployment sub create` locally before relying on Actions.

2. **Git Bash (MINGW64) rewrites `/subscriptions/...` scopes**  
   `az role assignment create --scope /subscriptions/<id>` failed with `MissingSubscription` even when `az account show` was correct. MSYS path conversion can rewrite leading `/` when invoking Windows `az`.  
   **Workaround:** PowerShell/CMD, or `MSYS_NO_PATHCONV=1` (documented in the OIDC guide).

3. **OIDC subject must match GitHub Environment, not branch**  
   Federated credentials use `repo:dertinfo/dertinfo-mono:environment:development` / `production` (checked-in JSON). Branch refs alone are insufficient for these jobs. Same class of issue as earlier app CD (`test` environment subject). See [github-azure-federated-credentials.md](../../technical/guides/github-azure-federated-credentials.md) and historical notes in [2026-07-03-github-actions-cd-pipelines.md](./2026-07-03-github-actions-cd-pipelines.md).

4. **Separate Environment naming from app CD**  
   App CD uses `test` / `prod`; subscription foundation uses `development` / `production` to align with `dev` / `prd` tags. Easy to misconfigure vars or federated subjects if mixed with `*_STG` / `*_PRD` app CD names.

## References to any best practices that we found

- **Privileged vs restricted deploy identities:** Subscription foundation SP (Contributor + role-assignment rights) vs later RG-scoped SP — [agent-safe-subscription-foundation.md](../planned-fixes/agent-safe-subscription-foundation.md)
- **OIDC without client secrets:** Checked-in federated credential JSON (no secrets); Environment **variables** for client/tenant/subscription ids — [github-azure-federated-credentials.md](../../technical/guides/github-azure-federated-credentials.md), [cicd.md](../../technical/infra/cicd.md)
- **Public repo params:** Empty placeholders for principal/subscription ids; override from Environment / CLI — planned-fix Bicep conventions
- **Required reviewers on new-stack Environments:** Either `dertinfo` or `davidsmonkeys` for both `development` and `production`
- **Bicep `extends` params:** `main.shared.bicepparam` + leaf `main.dev` / `main.prod` (Bicep 0.44.1+)

## Any remaining issues that we may wish to address

- **Untested Actions path (immediate follow-up):** Run **Subscription infra CD** from `main` (target `dev`), approve Environment, fix OIDC/RBAC/Bicep failures in a follow-up PR. Do not treat foundation as verified until that succeeds.
- **Policy and RG verification:** Confirm deny of disallowed types/SKUs; wire optional `AZURE_ENTRA_OIDC_PRINCIPALID_RG` and RG deploy identity.
- **Workflow-testability for future CD:** Revisit how we introduce new pipelines without merging untested YAML to `main` (e.g. documented “workflows-first” micro-PR + run with feature-branch ref; or local `az` gate before merge). This matters once production continuous delivery depends on these workflows.
- **Unpark** estate / workload Bicep planned-fixes only after subscription deploy is verified — [azure-estate-dev-prd.md](../planned-fixes/azure-estate-dev-prd.md), [bicep-avm-infra-pipelines.md](../planned-fixes/bicep-avm-infra-pipelines.md).
