# Change log

Chronological record of completed workstreams in this monorepo. Each entry links to a detail page describing what was done, issues encountered, practices adopted, and any follow-up work.

For operational how-to (CI/CD setup, configuration, planned fixes), see the wider docs under [`docs/`](../).

## Index

| Completed | Title | Detail |
|-----------|--------|--------|
| 2026-07-03 | EF Core nullable string properties on database entities | [2026-07-03-ef-nullable-string-properties.md](./2026-07-03-ef-nullable-string-properties.md) |
| 2026-07-03 | GitHub Actions CD pipelines for `test` | [2026-07-03-github-actions-cd-pipelines.md](./2026-07-03-github-actions-cd-pipelines.md) |

## How entries are added

When a chat or workstream is **seen through to completion**, add:

1. A new detail page: `docs/changelogs/YYYY-MM-DD-short-slug.md` (use the completion date).
2. A row at the **top** of the index table above (newest first).

Use the [detail page template](#detail-page-template) below. Prefer linking to existing docs (`docs/cicd.md`, `docs/configuration.md`, planned-fixes, etc.) rather than duplicating them.

### Detail page template

```markdown
# Title

## Summary of the work completed

Brief description of the outcome and what changed.

## Date the work was started

YYYY-MM-DD

## Date the work was completed

YYYY-MM-DD

## Issues that were encountered on the way

- Problem and how it was resolved (link PRs/runs where useful)

## References to any best practices that we found

- Practice, with link to docs or external references

## Any remaining issues that we may wish to address

- Follow-ups (link to `docs/planned-fixes/` when applicable)
```
