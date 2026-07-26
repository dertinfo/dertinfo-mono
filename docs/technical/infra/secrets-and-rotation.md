---
name: Secrets and rotation
type: infra
status: active
updated: 2026-07-26
---

# Secrets inventory and rotation

Credentials used by DertInfo and how to rotate them in hosted environments. Local development uses [`infra/secrets/api.env`](../../../infra/secrets/) (or Visual Studio user secrets) — see [Configuration](configuration.md).

Adapted from the legacy wiki: [Credentials & Secrets](https://github.com/dertinfo/dertinfo/wiki/Credentials-&-Secrets).

## Management plane

Operational / management access to third-party dashboards is typically via the organisation’s admin mailbox with MFA. Prefer **managed identity** and Entra-based access between Azure resources where possible instead of long-lived shared secrets.

## Application secrets (API)

| Secret | Purpose |
|--------|---------|
| Images storage account access key | API writes original images to blob storage |
| SQL username / password | Database access |
| SQL server name / database name | Connection targeting (not always “secret”, but environment-specific) |
| SendGrid API key | Transactional email |
| Auth0 Management Client secret | API updates Auth0 user `app_metadata` |

Never commit these values. Hosted environments: Key Vault (often referenced from App Configuration). Local: `infra/secrets/api.env`.

## Rotating secrets (staging & production)

Restart the API after Key Vault / App Configuration updates unless a configuration refresh mechanism is enabled (it is not, by default).

### Database credentials

1. Connect as admin (`sa` / Azure SQL admin).
2. Create or alter the login/user:

```sql
-- Against master (admin)
CREATE LOGIN your_login_name WITH PASSWORD = 'your_password';

-- Against the application database (admin)
CREATE USER your_user_name FOR LOGIN your_login_name;
ALTER ROLE db_datareader ADD MEMBER your_user_name;
ALTER ROLE db_datawriter ADD MEMBER your_user_name;

-- Or change password
ALTER LOGIN your_login_name WITH PASSWORD = 'new_password';
```

3. Update Key Vault values.
4. Restart the API.
5. If credentials were compromised, delete or disable the old login after cutover.

### Images storage account

1. In Azure Portal, open the images storage account.
2. Rotate the primary (or secondary) access key.
3. Update the corresponding Key Vault secret.
4. Restart the API.

### Auth0 Management Client secret

1. Open the Auth0 tenant for the environment (dev / test / live).
2. Applications → **DertInfo – \<env\> – API Client** (Management / M2M style client used by the API).
3. Scroll to the danger zone → **Rotate Secret**.
4. Copy the new secret into Key Vault.
5. Restart the API.

![Auth0 applications list](https://github.com/user-attachments/assets/848852be-3858-4415-b1a3-c20c337b8dac)

![Rotate secret](https://github.com/user-attachments/assets/02f65d30-a881-4491-81d8-fd25d3e6ff72)

### SendGrid

Wiki note: account recovery / rotation may need provider support if the account was frozen (e.g. after secret scanning). Treat as follow-up if email is required in an environment.

## Related

- [Configuration](configuration.md)
- [Authentication](../subsystems/authentication.md)
- [`infra/secrets/README.md`](../../../infra/secrets/README.md)
