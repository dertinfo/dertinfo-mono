# DertInfo E2E scripts (Playwright)

Playwright scripts for the website. The smoke suite expects this local profile already running. SQL is Docker. The API, image resize, Azurite, website, and PWA are native.

`infra/dev/runtime.json` (gitignored):

| Service | Mode |
|---------|------|
| sql | docker |
| api, web, app, imageResize, azurite | native |

Tests run against an empty database. Recreate the Compose volume `sqlserver-data` before a clean run. API startup applies EF migrations on that empty server. `infra/secrets/api.env` must point `SqlConnection__ServerName` at `127.0.0.1,44000`, with the Compose `sa` login and database `DertInfoDb`.

| Service | URL |
|---------|-----|
| Web | http://localhost:44200 |
| PWA | http://localhost:44300 |
| API | http://localhost:44100/api |
| Image resize | http://localhost:44400 |
| Azurite | http://127.0.0.1:10000 |
| SQL | localhost,44000 |

Bring the profile up with `npm run doctor`, `npm run start`, and `npm run status` from the repo root.

## Setup

```bash
cd tests/e2e
npm install
```

Chromium is installed automatically via `postinstall`.

## Suites by capability

| Capability | Folder | npm script |
|------------|--------|------------|
| **Website smoke** (scenario contracts on the capability pages) | [`web/smoke/scenarios/`](web/smoke/scenarios/) | `npm run test:web:smoke` |
| **Login** (post-auth navigation, warmup) | [`web/login/`](web/login/) | `npm run test:login:warmup` |

Smoke order, personas, and helpers: [Playwright standards](../../docs/technical/standards/playwright/README.md).

Persona passwords live in gitignored `.env`. Auth0 `app_metadata` for those users is in [`auth0-personas.json`](auth0-personas.json). Emails are `event-admin-1@dertinfo.co.uk` and `group-admin-1@dertinfo.co.uk` on tenant `dertinfodev.eu.auth0.com`.

Legacy Protractor e2e for Angular apps remains under `apps/dert-web/src/client/e2e/` and `apps/dert-app/src/client/e2e/`.
