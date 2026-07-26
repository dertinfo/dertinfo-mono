---
name: Security
type: subsystem
status: active
updated: 2026-07-26
---

# Subsystem: Security notes

Selected application security controls. Authentication and token claims: [Authentication](authentication.md).

Adapted from the legacy wiki: [Security](https://github.com/dertinfo/dertinfo/wiki/Security).

## CORS origins

The API restricts browser access to known SPA origins via the `AllowSpecificOrigins` policy.

Local defaults come from `appsettings.json`:

```json
"Cors": {
  "AllowedOrigins": "http://localhost:44200,http://localhost:44300"
}
```

Staging and production origin lists are supplied via **Azure App Configuration** (when `AZURE_APP_CONFIG` is set), overlaying these settings at runtime.

Do not open CORS to `*` for authenticated API routes.
