# DertInfo E2E scripts (Playwright)

Ad-hoc browser scripts for reproducing and validating user flows against the **Docker dev stack** (`docker compose up`).

| Service | URL |
|---------|-----|
| Web | http://localhost:44200 |
| API | http://localhost:44100/api |

## Setup

```bash
cd tests/e2e
npm install
```

Chromium is installed automatically via `postinstall`.

## Suites by capability

| Capability | Folder | npm script |
|------------|--------|------------|
| **Login** (post-auth navigation, warmup) | [`web/login/`](web/login/) | `npm run test:login:warmup` |

Legacy Protractor e2e for Angular apps remains under `apps/dert-web/src/client/e2e/` and `apps/dert-app/src/client/e2e/`.
