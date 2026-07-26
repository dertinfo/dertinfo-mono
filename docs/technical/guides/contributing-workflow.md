---
name: Contributing workflow
type: guide
status: active
updated: 2026-07-26
---

# Contributing workflow (branches and PRs)

How contributors branch, raise PRs, and what happens after merge in the **monorepo**.

Adapted from the legacy wiki: [Branching approach for DertInfo](https://github.com/dertinfo/dertinfo/wiki/Branching-approach-for-DertInfo). Prefer this page and [`.cursor/rules/gitflow.mdc`](../../../.cursor/rules/gitflow.mdc) over the wiki where they differ.

## Branching

- **main** is protected; change it only via pull request.
- Prefer gitflow-style names: `feature/…`, `hotfix/…`, `release/…` (see gitflow rule). Issue numbers in the name help (`feature/123-short-description`).
- Older wiki convention used dated prefixes (`feature/20240912-34-…`, `docs/…`, `upgrade/…`). That remains readable if you encounter old branches; new work should follow the monorepo gitflow rule.

### Typical flow

1. Update your base branch (`main`, or `develop` when in use): `git pull`.
2. Create a focused feature branch.
3. Before opening a PR: merge or rebase the latest base into your branch so the PR is conflict-free.
4. Open a PR; obtain at least one approving review. CI (build/test) and secret scanning must pass.

## After merge (current monorepo)

Delivery is **GitHub Actions**, not Azure DevOps. See [CI/CD](../infra/cicd.md).

| Concern | Today |
|---------|--------|
| CI | Path-filtered `*-ci.yml` on PRs / branches |
| CD to test | `*-cd.yml` on `main` → Azure `test` environment + Docker Hub test tags |
| Production | Deferred / gated — see [planned-fixes/cicd-future-phase](../../operations/planned-fixes/cicd-future-phase.md) |

Hosted apps use Azure Static Web Apps (SPAs) and App Service / Function App (.NET), not containers in staging/production. Docker images support local/dev consumption.

### Historical (wiki / pre-monorepo)

Separate repos used Azure DevOps YAML under `/pipelines` for Docker Hub publish, staging release (with a manual prod gate), and Bicep infra. Those pipelines may still exist under `apps/*/pipelines/` as reference; do not treat them as the primary monorepo path.

## Related

- [CI/CD](../infra/cicd.md)
- [CONTRIBUTING.md](../../../CONTRIBUTING.md)
- [Local development](local-development.md)
