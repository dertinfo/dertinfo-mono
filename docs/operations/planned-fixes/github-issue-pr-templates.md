# Future task: GitHub issue and PR templates

**Status:** Future task — templates consolidated to repo root; content refresh deferred.

**Location:** `.github/ISSUE_TEMPLATE/`, `.github/PULL_REQUEST_TEMPLATE.md`

---

## Background

Per-app `.github/` folders were removed when the four repositories were merged into this monorepo. Issue and pull request templates were copied unchanged to the root `.github/` folder (same content as the former standalone repos).

---

## Follow-up

Review and tailor templates for monorepo workflow (see also root `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`, `LICENCE.md` — consolidated from per-app copies):

| Template | Ideas |
|----------|--------|
| **Bug report** | Optional **Affected area** field (`apps/dert-api`, `apps/dert-web`, `apps/dert-app`, `apps/dert-functions`, infra, shared contracts). Local repro: Docker Compose vs `ng serve` / `dotnet run`. |
| **Feature request** | Same affected-area / scope prompt; link to full-stack flow (API → contracts → frontend) when relevant. |
| **Pull request** | Replace embedded-style **Test configuration** (firmware / hardware / toolchain) with DertInfo-relevant checks: affected apps, `docker compose`, unit/e2e commands, config notes (`api.env`, Auth0). Optional **Apps touched** checklist. |

No urgency — current templates work; this is polish for contributors opening issues and PRs against the unified repo.
