# Planned: Rename Bicep production leaf params to `prd`

**Status:** Not started — do **not** mix into a feature PR. Leaf filename `main.prod.bicepparam` is legacy; `environmentTag` inside those files is already `prd`.

**Related:** [env-acronyms](../../../.cursor/rules/env-acronyms.mdc), [Bicep standards](../../technical/standards/bicep/README.md), [CI/CD](../../technical/infra/cicd.md).

---

## Intent

Rename every `infra/bicep/**/main.prod.bicepparam` to `main.prd.bicepparam` so the filename matches the three-letter production acronym used in tags, resource names, and docs.

## Scope

- All workloads under `infra/bicep/` including subscription, config, monitoring, storage, api, web, app, **and functions**
- Keep `main.dev.bicepparam` and `main.shared.bicepparam`
- Update `parameters_path` in every `*-infra-cd.yml` that points at `main.prod.bicepparam`
- Update docs, [`.cursor/rules/env-acronyms.mdc`](../../../.cursor/rules/env-acronyms.mdc), and [`.cursor/rules/bicep.mdc`](../../../.cursor/rules/bicep.mdc)

## Why not with Functions Flex CD

Workflow paths and existing caller workflows already use `main.prod.bicepparam`. Renaming in the same change as a new workload would desynchronise CD. `environmentTag` is already `prd` (never `prod` or `stg`).

## Out of scope

- Renaming GitHub Environment `production`
- Renaming Azure resource names that already use `prd`
