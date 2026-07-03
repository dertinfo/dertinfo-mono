# Investigation: Docker silent token renewal after group create

**Status:** Reverted — auth flow restored to pre-change baseline. Root cause confirmed: see [`local-silent-auth-localhost.md`](local-silent-auth-localhost.md) (Auth0 `consent_required` on `http://localhost`; not Docker-specific).

**Symptom:** After creating a group in the Docker website (`http://localhost:44200`), the create-group modal stays open, the dashboard group list does not refresh, and `GET /api/group` does not run after a successful `POST /api/group/minimal` (201).

**Related:** [`local-silent-auth-localhost.md`](local-silent-auth-localhost.md) (definitive localhost diagnosis), [`web-warmup-race-condition.md`](web-warmup-race-condition.md) (separate warmup/dashboard 401 issue).

---

## Expected flow

1. User submits group-create modal → `POST /api/group/minimal` (201).
2. API updates Auth0 app metadata (`groupadmin` claims) via Management API.
3. `AuthService.renewToken()` runs silent Auth0 renewal (`renewAuth` + `usePostMessage`).
4. Hidden iframe loads silent callback → `parseHash` → `postMessage` with new tokens.
5. `dashboard.conductor` refetches `GET /api/group`, updates dashboard and nav, modal closes.

**Why renewal matters:** `GET /api/group` filters by JWT `https://dertinfo.co.uk/groupadmin` claims. Without a fresh access token, the new group is invisible even though it was created.

---

## Original configuration (restored)

| Piece | Value |
|-------|--------|
| Login callback | `{auth0CallbackUrl}/callback` |
| Silent renewal `redirectUri` | `{auth0CallbackUrl}/silent` |
| Silent callback route | Angular `AuthSilentComponent` at `/silent` → `parseSilentResponse()` |
| `auth0Silent.redirectUri` | `{auth0CallbackUrl}` (base origin only; used by `parseSilentResponse`) |
| `renewAuth` | `this.auth0.renewAuth({ audience, redirectUri: .../silent, usePostMessage: true, postMessageOrigin })` |
| Dashboard after create | `renewToken()` → `navigationService.addGroupForUser(groupDto)` → navigate to `group-configure` |

Local dev (fast feedback loop): `ng serve` on port 4200 + `swa start http://localhost:4200 --port 44200`.

Docker: `swa start ./dist` serves pre-built static assets (cold iframe load).

---

## Root cause confirmed (Docker)

**Group create succeeds; silent renewal fails.**

Runtime evidence from API logs and browser:

- `POST /api/group/minimal` → **201** every time.
- No `GET /api/group` after create when renewal fails.
- After failed renewal, console: `ERROR: Adding Group Failed` / `Silent token renewal returned no access token` or `Consent required`.

### Issue A: Full Angular SPA boots in silent iframe (Docker)

`renewAuth` opens a hidden iframe to `{origin}/silent`. In Docker, SWA serves `index.html` for that route → entire Angular app boots inside the iframe.

Proof: API logs showed `GET /api/clientconfiguration/web` immediately after each create — the SPA `APP_INITIALIZER` / `ConfigurationService` cold-starting in the iframe before `parseHash` could run.

Locally with `ng serve` + SWA proxy, the dev server is warm and the iframe loads faster, so the original `/silent` route could appear to work.

### Issue B: Azurite blob endpoint (fixed separately, kept)

`docker-compose.yml` had `StorageAccount__Images__BlobEndpoint=http://azstorage/devstoreaccount1` (port 80). Azurite listens on **10000**. This blocked group create entirely until corrected to `:10000`. Not an auth issue; fix retained in compose.

---

## Attempted fix: static silent callback (reverted)

Plan: replace Angular `/silent` with a lightweight static HTML page using `auth0-js` `parseHash` + `parent.postMessage`.

### What was tried

| Attempt | Change | Result |
|---------|--------|--------|
| 1 | `silent-callback.html` at `/silent-callback.html`, CDN `auth0-js` 9.28 | `auth0 is not defined` — **Auth0 CDN returns HTTP 403** from this environment |
| 2 | Bundle `auth0.min.js` from `node_modules` into `/assets/` | CDN error resolved; callback page loads |
| 3 | `redirectUri` → `/silent-callback.html`; update Auth0 Allowed Callback URLs | postMessage received but **`consent_required`** |
| 4 | Remove `offline_access` from silent `scope`; explicit token guards | Still `consent_required` on localhost |
| 5 | Switch to `checkSession` (`web_message`) | Auth0 iframe posts `{ error: consent_required }`; auth0-js warns consent cannot be skipped on localhost |
| 6 | Static page at `/silent` (existing Auth0 callback URL) + SWA route rewrite | Static page serves correctly; still **`consent_required`** during silent renewal |
| 7 | `auth0Silent.renewAuth()` instead of `this.auth0.renewAuth()` | No improvement |
| 8 | Dashboard conductor: renew → refetch groups → close modal (no navigate to group-configure) | Correct UX intent, but blocked while renewal fails |
| 9 | Fresh login + accept API consent screen | Dashboard showed all previously created groups (18) after login; **silent renewal after create still failed** with `consent_required` |

### postMessage payloads observed

```json
{ "error": "silent_callback_failed", "errorDescription": "ReferenceError: auth0 is not defined" }
```

```json
{ "error": "consent_required", "errorDescription": "Consent required", "state": "..." }
```

With `checkSession`, from `https://dertinfodev.eu.auth0.com`:

```json
{ "type": "authorization_response", "response": { "error": "consent_required", ... } }
```

### Learnings

1. **Docker silent iframe loading the full SPA is a real problem** — static callback is the right direction for Docker, but insufficient alone.
2. **Auth0 CDN is not reliable** for `silent-callback.html` — bundle `auth0-js` from `node_modules` if revisiting static callback.
3. **`localhost` + silent renewal + API `audience`** — Auth0 returns `consent_required` with `prompt=none`. Documented behaviour: [skipping consent for first-party clients does not apply to localhost](https://auth0.com/docs/api-auth/user-consent#skipping-consent-for-first-party-clients). Interactive login can show the API consent screen; silent renewal may still fail afterward on localhost.
4. **Accepting API consent at login does not guarantee silent renewal works on localhost** — observed in E2E testing after explicit consent.
5. **Group create can succeed while UI appears broken** — groups accumulate in DB/Auth0 metadata; fresh login with consent can reveal them in the list (count jumped from 8 → 18).
6. **SWA routing matters** — `/silent` without a route rewrite redirected to `/silent/`; rewrite to `/silent/index.html` needed for hash preservation.
7. **`staticwebapp.config.json` in Docker** — `swa start ./dist` did not pick up config from `./app/` unless copied into `dist/` (attempted in Dockerfile; reverted with auth rollback).

---

## Auth0 configuration referenced

**Application:** DertInfo Web — `JREkJwdUbZmUs7d983uBXnrZu9UOWbd9`  
**API audience:** `api.dev.dertinfo.co.uk`

Allowed Callback URLs (existing baseline):

```
http://localhost:44200
http://localhost:44200/callback
http://localhost:44200/silent
```

Allowed Web Origins: `http://localhost:44200`

---

## Files changed during investigation (all auth-related changes reverted)

- `apps/dert-web/src/client/src/app/core/authentication/auth.service.ts`
- `apps/dert-web/src/client/src/app/modules/dashboard/services/dashboard.conductor.ts`
- `apps/dert-web/src/client/src/app/modules/dashboard/group-create/group-create.component.ts`
- `apps/dert-web/src/client/angular.json`
- `apps/dert-web/src/client/staticwebapp.config.json`
- `apps/dert-web/Dockerfile`
- `apps/dert-web/README.md`
- `README.md` (removed silent-callback Auth0 note)
- Added then removed: `src/silent/index.html`, `src/silent-callback.html`

**Not reverted (non-auth):** root `docker-compose.yml` Azurite `:10000` blob endpoint fix.

---

## Recommended next steps

1. **Benchmark original flow outside Docker**
   ```bash
   cd apps/dert-web/src/client
   npm i
   ng serve
   # separate terminal
   swa start http://localhost:4200 --port 44200
   ```
   Create a group; confirm whether `/silent` Angular route + `renewAuth` succeeds with browser DevTools (Network + postMessage).

2. **If local works but Docker fails** — problem is iframe cold-boot / static hosting, not Auth0 tenant config. Revisit static `/silent` **without** changing `redirectUri` away from `/silent`.

3. **If local also fails with `consent_required`** — Auth0 tenant/API consent settings need review before any callback-page change.

4. **If renewal cannot work on localhost** — consider testing on Codespaces/staging URL, or a deliberate re-login fallback after group create (poor UX; last resort).

---

## Success criteria (when revisiting)

1. Submit group create → modal closes (or original navigate to group-configure).
2. Network: `POST /api/group/minimal` (201) → silent iframe loads `/silent` → `GET /api/group` (200).
3. Dashboard group count increases; new group visible.
4. Console: no `Silent token renewal returned no access token` or `Consent required`.
