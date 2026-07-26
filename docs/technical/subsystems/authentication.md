---
name: Authentication
type: subsystem
status: active
updated: 2026-07-26
---

# Subsystem: Authentication (Auth0)

How DertInfo authenticates users and authorises API access using Auth0 identity and access tokens.

Platform **roles** (who can do what) are listed under [capabilities/roles](../../capabilities/roles/) without Auth0 detail. This page is the technical mapping.

Adapted from the legacy wiki: [Identity and Access Tokens with Auth0](https://github.com/dertinfo/dertinfo/wiki/Authentication-%E2%80%90-Identity-and-Access-Tokens-in-DertInfo-with-Auth0).

## Overview

1. The user signs in via Auth0 (hosted login / Lock).
2. Auth0 issues an **ID token** (who the user is for the client) and an **access token** (what the API may allow).
3. Clients send the access token to the API; the API validates the JWT (issuer / audience) and applies claim-based policies.
4. DertInfo does **not** store user passwords — Auth0 holds credentials.

![Auth0 login](https://github.com/user-attachments/assets/5a606dd8-dd1b-4d22-a934-beff391d93d6)

Website (`dert-web`) uses the Auth0 Angular SDK with refresh-token renewal; the PWA may still use an older client pattern. Token storage details can differ by client — treat the claim model below as canonical.

## ID token vs access token

| Token | Purpose | Trust for data access |
|-------|---------|------------------------|
| **ID token** | Profile and UI flags for the client (email, name, picture, GDPR / admin *display* flags) | **Not** used by the API to authorise data |
| **Access token** | Scoped permissions for the API (`groupadmin`, `eventadmin`, etc.) | Authoritative for API authorisation |

Inspect tokens at [jwt.io](https://jwt.io). Prefer minimal, user-specific claims in the browser to limit blast radius if a token is exposed.

![Tokens in browser](https://github.com/user-attachments/assets/3895baef-6196-4ea6-b561-09b1d132cd9f)

### Example ID token payload (illustrative)

```json
{
  "https://dertinfo.co.uk/gdprconsentgained": true,
  "https://dertinfo.co.uk/superadmin": false,
  "https://dertinfo.co.uk/dodadmin": true,
  "nickname": "example",
  "name": "example@example.com",
  "picture": "https://s.gravatar.com/avatar/…",
  "email": "example@example.com",
  "email_verified": true,
  "iss": "https://dertinfodev.eu.auth0.com/",
  "aud": "<spa-client-id>",
  "sub": "auth0|…"
}
```

Client-side flags such as `superadmin` / `dodadmin` on the ID token only affect **UI**; the API must enforce the same concepts from the **access token**.

### Example access token payload (illustrative)

```json
{
  "https://dertinfo.co.uk/superadmin": false,
  "https://dertinfo.co.uk/dodadmin": false,
  "https://dertinfo.co.uk/groupadmin": ["10"],
  "https://dertinfo.co.uk/eventadmin": ["2"],
  "https://dertinfo.co.uk/venueadmin": [],
  "https://dertinfo.co.uk/groupmember": [],
  "iss": "https://dertinfo.eu.auth0.com/",
  "sub": "auth0|…",
  "aud": ["api.dertinfo.co.uk", "https://dertinfo.eu.auth0.com/userinfo"],
  "scope": "openid profile email offline_access"
}
```

Array claim values are **string entity IDs** (group, event, or venue). Creating a group adds that group’s ID to the user’s `groupadmin` list in Auth0; the client must obtain a **new access token** before the API reflects the change.

![App metadata in Auth0](https://github.com/user-attachments/assets/8d773aeb-9989-4690-88c3-b6f4dfb24d0c)

## Auth0 `app_metadata` (source of permissions)

Permissions are stored on the Auth0 user as **app metadata** (not editable by the end user). Fill or edit under Auth0 Dashboard → Users → user → **app_metadata**.

### Field reference

| `app_metadata` field | JWT custom claim (namespace `https://dertinfo.co.uk`) | Platform role |
|----------------------|--------------------------------------------------------|---------------|
| `isSuperAdmin` | `/superadmin` | [Super administrator](../../capabilities/roles/super-admin.md) |
| `isDodAdmin` | `/dodadmin` | [Dert of Derts administrator](../../capabilities/roles/dod-admin.md) |
| `groupadmin` | `/groupadmin` | [Group administrator](../../capabilities/roles/group-admin.md) (array of group IDs) |
| `eventadmin` | `/eventadmin` | [Event administrator](../../capabilities/roles/event-admin.md) (array of event IDs) |
| `venueadmin` | `/venueadmin` | [Venue administrator](../../capabilities/roles/venue-admin.md) (array of venue IDs) |
| `groupmember` | `/groupmember` | [Group member](../../capabilities/roles/group-member.md) (array of group IDs) |
| `gdprConsentGained` | `/gdprconsentgained` (ID token) | Consent flag |
| `gdprConsentGainedDate` | (stored in metadata; not always claimed) | Consent timestamp |

Arrays must be JSON string arrays of IDs, e.g. `"46"`, not bare numbers.

### Example `app_metadata` template

Trimmed example for documentation (use real IDs for the target environment’s database):

```json
{
  "gdprConsentGained": true,
  "gdprConsentGainedDate": "2021-03-01T17:54:23.4920576+00:00",
  "isSuperAdmin": false,
  "isDodAdmin": true,
  "eventadmin": ["30", "29", "1"],
  "groupadmin": ["46", "141", "1"],
  "groupmember": [],
  "venueadmin": []
}
```

After editing metadata manually, the user needs a **fresh login or token refresh** so Auth0 Actions re-emit claims into new tokens.

Automated updates (e.g. group create) go through the API’s Auth0 Management client — see [Updating Auth0 from the API](#updating-auth0-from-the-api).

## Auth0 Actions (Post Login)

Legacy **Rules** are EOL; tenants use **Actions** on the Post Login flow to copy `app_metadata` into custom claims on the ID and access tokens.

Illustrative Action (from the wiki; keep the live tenant Action as the source of truth if it has diverged):

```javascript
exports.onExecutePostLogin = async (event, api) => {
  var customNamespace = 'https://dertinfo.co.uk';

  if (api.idToken && event.user.app_metadata) {
    api.idToken.setCustomClaim(`${customNamespace}/gdprconsentgained`, event.user.app_metadata.gdprConsentGained);
    api.idToken.setCustomClaim(`${customNamespace}/superadmin`, event.user.app_metadata.isSuperAdmin);
    api.idToken.setCustomClaim(`${customNamespace}/dodadmin`, event.user.app_metadata.isDodAdmin);
    // Profile fields may also be set from user_metadata / app_metadata depending on tenant setup
  }

  if (api.accessToken && event.user.app_metadata) {
    api.accessToken.setCustomClaim(`${customNamespace}/superadmin`, event.user.app_metadata.isSuperAdmin);
    api.accessToken.setCustomClaim(`${customNamespace}/dodadmin`, event.user.app_metadata.isDodAdmin);
    api.accessToken.setCustomClaim(`${customNamespace}/groupadmin`, event.user.app_metadata.groupadmin);
    api.accessToken.setCustomClaim(`${customNamespace}/eventadmin`, event.user.app_metadata.eventadmin);
    api.accessToken.setCustomClaim(`${customNamespace}/venueadmin`, event.user.app_metadata.venueadmin);
    api.accessToken.setCustomClaim(`${customNamespace}/groupmember`, event.user.app_metadata.groupmember);
  }
};
```

Maintain this Action **per Auth0 tenant** (dev / staging / production).

## Updating Auth0 from the API

When a user creates a group (or similar), the API updates that user’s `app_metadata` via the Auth0 Management API using the **Management Client** credentials (`Auth0:ManagementClientId` / `ManagementClientSecret`).

That secret must only exist on a **confidential** client (the API). Never expose it to SPAs. Local values: [`infra/secrets/api.env`](../../../infra/secrets/); hosted: Key Vault / App Configuration — see [Secrets and rotation](../infra/secrets-and-rotation.md).

Implementation: [`Auth0V2ManagementApiClient`](../../../apps/dert-api/src/dertinfo-services/ExternalProviders/Auth0V2ManagementApiClient.cs).

## Auth0 tenants

Use a **separate Auth0 tenant per environment** (development, staging/test, production) so entity IDs and admins cannot cross environments (e.g. `groupadmin` for group `4` in dev must not unlock group `4` in production).

Local monorepo work typically uses the shared **dev** tenant — see [Configuration](../infra/configuration.md).

## Related

- [Security (CORS)](security.md)
- [Capability roles](../../capabilities/roles/)
- [Configuration](../infra/configuration.md)
- API policies: `apps/dert-api/src/dertinfo-api/Start/Authorisations.cs`
