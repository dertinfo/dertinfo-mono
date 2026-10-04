# DertInfo E2E scripts (Playwright)

Playwright scripts for the website.

How to run the smoke suite, reset the database, and record video: [Website smoke tests](../../docs/technical/guides/website-smoke-tests.md).

How a scenario is sized, how it calls steps, how those browsers become clips, and how session state is kept: [Playwright standards](../../docs/technical/standards/playwright/README.md).

## Setup

```bash
cd tests/e2e
npm install
```

Chromium is installed automatically via `postinstall`.

The smoke suite expects the local profile already running. SQL is Docker. The API, image resize, Azurite, website, and PWA are native. Bring that profile up with `npm run doctor`, `npm run start`, and `npm run status` from the repo root.

## Suites

| Suite | Folder | npm script |
|-------|--------|------------|
| Website smoke | [`web/smoke/scenarios/`](web/smoke/scenarios/) | `npm run test:web:smoke` |
| Login warmup investigations | [`web/login/`](web/login/) | `npm run test:login:warmup` |

Persona passwords live in gitignored `.env`. Auth0 `app_metadata` for those users is in [`auth0-personas.json`](auth0-personas.json).

Legacy Protractor e2e for Angular apps remains under `apps/dert-web/src/client/e2e/` and `apps/dert-app/src/client/e2e/`.
