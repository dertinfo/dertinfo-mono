# Planned: Rebuild Auth0 tenants to match environments

**Status:** Not started — document today’s mapping now; rebuild later. Do not rename tenants in the SWA Bicep / first development SWA work.

**Related:** [Authentication](../../technical/subsystems/authentication.md), [Configuration](../../technical/infra/configuration.md), [CI/CD](../../technical/infra/cicd.md).

---

## Why

Today’s Auth0 tenant names do not match how we talk about environments:

| Runtime | GitHub / Azure | Auth0 tenant today | Intended name |
|---------|----------------|--------------------|---------------|
| Local native / Docker | none | `dertinfodev.eu.auth0.com` | **`dertinfolocal`** |
| New-stack Azure development | Environment `development` | `dertinfotest.eu.auth0.com` | **`dertinfodev`** |
| Live / old production | separate estate | `dertinfo.eu.auth0.com` | leave as-is until a dedicated PRD cutover |

That mismatch makes it easy to point a hosted SPA at the local tenant (or the reverse). GitHub Environment names are not Auth0 tenant names.

## Scope (when scheduled)

1. Create new tenants `dertinfolocal` and `dertinfodev` (EU region, same Actions / APIs / apps pattern as today).
2. Recreate website and PWA applications, API audience, Management M2M, and Post Login Actions.
3. Register callbacks and origins:
   - Local: `http://localhost:44200`, `http://localhost:44300` (and PWA `/auth/callback`).
   - Azure development: `https://dev.dertinfo.co.uk`, `https://app-dev.dertinfo.co.uk` (PWA `/auth/callback`).
4. Rotate client ids and secrets into `infra/secrets/api.env` (local) and Key Vault / catalog domains (hosted).
5. Update the tenant tables on [authentication.md](../../technical/subsystems/authentication.md) and [configuration.md](../../technical/infra/configuration.md).
6. Retire or lock down `dertinfodev` (old local) and `dertinfotest` after cutover.

## Out of scope

- Renaming or migrating the live production tenant (`dertinfo.eu.auth0.com`).
- Changing SWA Bicep or Src CD (they already take callback URLs from GitHub variables).
