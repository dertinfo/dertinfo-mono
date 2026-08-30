# Change log

Chronological record of completed workstreams in this monorepo. Each entry links to a detail page describing what was done, why, issues encountered, practices adopted, and any follow-up work.

For operational how-to (CI/CD setup, configuration, planned fixes), see the wider docs under [`docs/`](../../).

## Index

| Completed | Title | Detail |
|-----------|--------|--------|
| 2026-08-30 | Entra-only Azure SQL with admins and database access groups | [2026-08-30-003-entra-only-sql-groups.md](./2026-08-30-003-entra-only-sql-groups.md) |
| 2026-08-30 | Per-workload Entra OIDC identities and GitHub Environment variables | [2026-08-30-002-per-workload-oidc-identities.md](./2026-08-30-002-per-workload-oidc-identities.md) |
| 2026-08-30 | New-stack Bicep, GitHub Flow, and src/infra CD | [2026-08-30-001-new-stack-bicep-and-github-flow.md](./2026-08-30-001-new-stack-bicep-and-github-flow.md) |
| 2026-08-15 | Subscription OIDC isolation and successful development CD | [2026-08-15-002-subscription-oidc-isolation-dev-cd.md](./2026-08-15-002-subscription-oidc-isolation-dev-cd.md) |
| 2026-08-15 | Subscription foundation Bicep and CD workflows (untested on merge) | [2026-08-15-001-subscription-foundation-cd.md](./2026-08-15-001-subscription-foundation-cd.md) |
| 2026-07-26 | Website Auth0 refresh tokens and warmup race fix | [2026-07-26-001-web-auth-refresh-warmup.md](./2026-07-26-001-web-auth-refresh-warmup.md) |
| 2026-07-25 | Hybrid native/Docker local startup | [2026-07-25-002-hybrid-native-docker-start.md](./2026-07-25-002-hybrid-native-docker-start.md) |
| 2026-07-25 | Local-native development orchestration | [2026-07-25-001-local-native-dev.md](./2026-07-25-001-local-native-dev.md) |
| 2026-07-03 | GitHub Actions CD pipelines for `test` | [2026-07-03-001-github-actions-cd-pipelines.md](./2026-07-03-001-github-actions-cd-pipelines.md) |

## How entries are added

When a chat or workstream is **seen through to completion**, add:

1. A new detail page: `docs/operations/changelogs/YYYY-MM-DD-NNN-short-slug.md`.
   - `YYYY-MM-DD` is the completion date.
   - `NNN` is a three-digit sequence **for that date** (`001`, `002`, …). The first entry on a day is `001` — never omit the number. Take the next number after the highest existing file for that date so the folder sorts in completion order.
2. A row at the **top** of the index table above (newest first; on the same date, higher `NNN` above lower `NNN`).

Use the [detail page template](#detail-page-template) below. Prefer linking to existing docs (`docs/technical/infra/cicd.md`, `docs/technical/infra/configuration.md`, planned-fixes, etc.) rather than duplicating them.

### Detail page template

```markdown
# Title

## Summary of the work completed

Brief description of the outcome and what changed.

## Why the work was completed

Rationale: what we were trying to achieve and why these changes were the right ones (not a repeat of the summary).

## Date the work was started

YYYY-MM-DD

## Date the work was completed

YYYY-MM-DD

## Issues that were encountered on the way

- Problem and how it was resolved (link PRs/runs where useful)

## References to any best practices that we found

- Practice, with link to docs or external references

## Any remaining issues that we may wish to address

- Follow-ups (link to `docs/operations/planned-fixes/` when applicable)
```
