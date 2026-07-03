# Login — web E2E scripts

Playwright scripts for **authenticated access** on the website (`apps/dert-web`). These tests do not drive the real Auth0 login UI; they seed `localStorage` with a fake token so `AuthGuard` passes, then exercise post-login routes.

Use this folder to collate anything related to signing in, session state, and the first navigation after authentication.

## Sub-suites

| Sub-suite | Folder | What it covers |
|-----------|--------|----------------|
| **API warmup** | [`warmup/`](warmup/) | `/session/warmup`, `WarmupResolver`, and navigation to `/dashboard` after `GET /api/status` |

Future examples (not yet implemented):

- `auth0-callback/` — real Auth0 redirect flow (needs test tenant credentials)
- `token-refresh/` — silent renew via `/silent`

## Shared helpers

- [`_helpers/session.mjs`](_helpers/session.mjs) — `seedFakeSession()`, `WEB_BASE`, `API_BASE`

## Run all warmup tests

From `tests/e2e`:

```bash
npm run test:login:warmup
```

## Related docs

- [`docs/planned-fixes/web-warmup-race-condition.md`](../../../../docs/planned-fixes/web-warmup-race-condition.md)
- [`docs/configuration.md`](../../../../docs/configuration.md) — Auth0 / Docker `api.env`
