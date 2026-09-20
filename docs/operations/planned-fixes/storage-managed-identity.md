# Planned: Storage Entra / managed identity and App Configuration cleanup

**Status:** In progress — hosted SQL is Entra-only; **new-stack Functions** use site managed identity on host storage and the images account. The API still uses images **account keys**.

**Related:** [Secrets and rotation](../../technical/infra/secrets-and-rotation.md), [Configuration](../../technical/infra/configuration.md), images account `stdevdertinfoimagesuks` / `stprddertinfoimagesuks` ([`infra/bicep/storage/`](../../../infra/bicep/storage/)), Functions [`infra/bicep/functions/`](../../../infra/bicep/functions/). Catalog: [`infra/configuration/app-config.development.json`](../../../infra/configuration/app-config.development.json).

---

## Intent

Finish the hosted identity story the same way as Azure SQL: apps use Entra (managed identity), not shared secrets in connection strings. Then **clean App Configuration and Key Vault** so leftover keys, unused settings, and old-export cruft are gone.

SQL is already Entra-only (`Authentication=Active Directory Default` when `AZURE_APP_CONFIG` is set). Storage keys in Key Vault (`az-storage-images-accountkey`, `az-storage-functions-accountkey`) are an interim step.

## Why

Account keys are long-lived and leak in App Configuration dumps. Several catalog entries are unused or duplicate Azure defaults and should not stay “because the old store had them.”

## Scope (when scheduled)

### A. App Configuration cleanup (can run before full storage MI)

The API does **not** read `StorageAccount:Functions:Key`. New-stack Functions use identity-based `AzureWebJobsStorage` / `StorageConnection:Images` app settings, not this store.

Hosted images connection is built from `Name` + `Protocol` + `Key`. Blob/queue/table endpoints are only appended when non-empty; for public Azure they match the default `*.core.windows.net` URLs.

1. Remove from the development catalog `keyValues` (and live store): `StorageAccount:Images:BlobEndpoint`, `QueueEndpoint`, `TableEndpoint`. Keep `Name` and `Protocol`. Local Azurite still needs custom endpoints in `appsettings.json` / `api.env`.
2. Remove `StorageAccount:Functions:Key` / `az-storage-functions-accountkey` from the catalog, Key Vault, and App Configuration **unless** Functions are first wired to this store. Prefer identity-based Functions connections (section B) over teaching Functions to read that key.
3. Confirm hosted App Configuration has **no** `SqlConnection:ServerAdminName` / `ServerAdminPassword` (Entra-only). Those remain local-only in `infra/secrets/api.env`.
4. Confirm there is no leftover `StorageConnection:Images` / `StorageConnection:Functions` **connection string** in this App Configuration store (old test export). The API uses piece-wise `StorageAccount:Images:*` only.

### B. Storage access with managed identity

1. **Images account (API)** — grant the App Service MI a data-plane role (typically Storage Blob Data Contributor) on `st<env>dertinfoimagesuks`. Change `StorageAccountConnection` to use `DefaultAzureCredential` (or Azure.Storage.Blobs with a token credential) instead of `AccountKey`. Drop `StorageAccount:Images:Key` / `az-storage-images-accountkey`.
2. **Images account (Functions)** — **done on the new stack, assigned by storage Bicep.** Identity-based `StorageConnection__Images__accountName` + `managedidentity` (Linux Flex; `__` maps to the `StorageConnection:Images` trigger connection). BlobWriter uses `DefaultAzureCredential` when the connection string is absent. Storage infra CD grants Blob Data Contributor when `flagImagesFunctionAppReady` is true (pipeline looks up the Function App principal id). The same principal-id list can later take the API site MI (B.1).
3. **Function App host storage** — **done on the new stack.** Identity-based `AzureWebJobsStorage__accountName`.
4. **Harden** — consider `allowSharedKeyAccess: false` on the accounts once no client uses keys (Azurite/local remains key-based). Do **not** disable shared-key on the images account until API identity (B.1) is done.

## C. Cleanup inventory (next step)

Do **not** execute this in the Functions Flex CD work. After DEV Functions are proven, remove leftovers from stores. Confirm against the live development catalog/vault before deleting.

| Item | Action |
|------|--------|
| Key Vault / App Configuration `az-storage-functions-accountkey` / `StorageAccount:Functions:Key` | API does not read it; new Functions use the site MI. Safe to remove from `kv-secrets.*.json.example`, catalogs, vault, and the live store once DEV Functions are proven. |
| `az-storage-images-accountkey` / `StorageAccount:Images:Key` | **Keep** — API still uses keys. |
| App Configuration `StorageAccount:Images:BlobEndpoint` / `QueueEndpoint` / `TableEndpoint` | Already listed in section A if still present. |
| GitHub leftover `AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME_STG` (and similar `test`/`prod` vars) | Drop after new-stack `AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME` is in use. |
| ADO Functions infra YAML | Trigger already retired. Delete the file after nobody relies on the ADO definition. |
| Old image-resize RG / Y1 app / Event Grid on the live images account | **After** production switchover only. |

## Out of scope

- Local native / Azurite (keep the well-known emulator key in `infra/secrets/api.env`).
