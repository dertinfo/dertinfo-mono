# SWA custom-domain operation-status RBAC

## Summary of the work completed

Subscription Bicep now defines a custom role `dertinfo-swa-operation-status-read-<env>` with the published actions `Microsoft.Web/locations/operationResults/read` and `Microsoft.Web/locations/*/read`, assigned at subscription scope to the web and app GitHub workload identities. Docs: [CI/CD](../../technical/infra/cicd.md), [Bicep standards](../../technical/standards/bicep/README.md).

## Why the work was completed

Development web/app infra CD failed when `customDomainReady` was true. Custom-domain bind is an async Static Web App operation. ARM then polls status at subscription/location scope. WEB and APP only had Contributor on their own resource groups, which does not include that action.

## Date the work was started

2026-09-19

## Date the work was completed

2026-09-19

## Issues that were encountered on the way

- Error was `AuthorizationFailed` on `Microsoft.Web/locations/staticSitesOperationStatuses/read` under `/subscriptions/…/providers/Microsoft.Web/locations/westeurope/staticSitesOperationStatuses/…`, not a DNS or Bicep syntax failure.
- Putting that same string on a custom role fails with `InvalidActionOrNotAction`. The Microsoft.Web catalog has `locations/operationResults/read` and `locations/operations/read`, not `staticSitesOperationStatuses/read`.
- The existing API nested-deploy custom role is the same idea but RG-scoped (`Microsoft.Resources/deployments/operationStatuses/read` on config). This SWA poll cannot be assigned on the web/app RG.

## References to any best practices that we found

- [Azure RBAC custom roles](https://learn.microsoft.com/en-us/azure/role-based-access-control/custom-roles) — least-privilege actions at the scope where ARM evaluates them
- [Azure permissions for Web and Mobile](https://learn.microsoft.com/en-us/azure/role-based-access-control/permissions/web-and-mobile) — published `Microsoft.Web/locations/operationResults/read` (not `staticSitesOperationStatuses`)
- House pattern: API nested-deploy custom role in [`infra/bicep/subscription/main.bicep`](../../../infra/bicep/subscription/main.bicep) (not subscription Contributor)

## Any remaining issues that we may wish to address

- First hosted development pass (Src CD, login / refresh / logout on both custom hosts) is complete — [2026-09-19-002](./2026-09-19-002-dev-swa-first-pass.md).
- Trim CORS origins in the API so spaces after commas in App Configuration do not drop the PWA origin.
