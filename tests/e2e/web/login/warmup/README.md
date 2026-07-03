# Warmup — login E2E scripts

Scripts that reproduce the **“Warming up”** screen stuck on `/session/warmup` after login. Parent capability: [`../README.md`](../README.md).

**Prerequisite:** Docker stack running (`web` on :44200, `api` on :44100).

## Tests

| Script | npm script | Summary |
|--------|------------|---------|
| [`baseline-smoke.mjs`](baseline-smoke.mjs) | `test:login:warmup:baseline` | Full flow smoke test; logs API calls and console |
| [`refresh-trap.mjs`](refresh-trap.mjs) | `test:login:warmup:refresh-trap` | Direct visit to `/session/warmup` — no recovery |
| [`url-history.mjs`](url-history.mjs) | `test:login:warmup:url-history` | Address bar timeline during warmup |
| [`mid-warmup-race.mjs`](mid-warmup-race.mjs) | `test:login:warmup:mid-race` | **Key repro:** delayed `/api/status` + 401 → stuck |
| [`delay-success-control.mjs`](delay-success-control.mjs) | `test:login:warmup:delay-control` | Control case: same delay + mocked 200s → dashboard |
| [`resolver-hang-vs-auth.mjs`](resolver-hang-vs-auth.mjs) | `test:login:warmup:hang-vs-auth` | Auth failure vs hung resolver scenarios |

Run everything:

```bash
cd tests/e2e && npm run test:login:warmup
```

## Suggested order when investigating

1. `baseline-smoke` — confirm `/api/status` vs dashboard API status codes  
2. `mid-warmup-race` — confirm race + auth failure combination  
3. `delay-success-control` — confirm fix when APIs return 200  
4. `refresh-trap` — confirm dead-end on refresh  
5. `resolver-hang-vs-auth` — distinguish URL stuck vs UI stuck  
