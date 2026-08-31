# Planned: Storage Entra / managed identity and App Configuration cleanup

**Status:** Not started — after hosted SQL is Entra-only; storage still uses account **keys**.

**Related:** [Secrets and rotation](../../technical/infra/secrets-and-rotation.md), [Configuration](../../technical/infra/configuration.md), images account `stdevdertinfoimagesuks` / `stprddertinfoimagesuks` ([`infra/bicep/storage/`](../../../infra/bicep/storage/)). Catalog: [`infra/configuration/app-config.development.json`](../../../infra/configuration/app-config.development.json).

---

## Intent

Finish the hosted identity story the same way as Azure SQL: apps use Entra (managed identity), not shared secrets in connection strings. Then **clean App Configuration and Key Vault** so leftover keys, unused settings, and old-export cruft are gone.

SQL is already Entra-only (`Authentication=Active Directory Default` when `AZURE_APP_CONFIG` is set). Storage keys in Key Vault (`az-storage-images-accountkey`, `az-storage-functions-accountkey`) are an interim step.

## Why

Account keys are long-lived and leak in App Configuration dumps. Several catalog entries are unused or duplicate Azure defaults and should not stay “because the old store had them.”

## Scope (when scheduled)

### A. App Configuration cleanup (can run before full storage MI)

The API does **not** read `StorageAccount:Functions:Key`. Functions still get `AzureWebJobsStorage` / `StorageConnection:Images` from Function App settings (`listKeys()` in Functions Bicep), not this store.

Hosted images connection is built from `Name` + `Protocol` + `Key`. Blob/queue/table endpoints are only appended when non-empty; for public Azure they match the default `*.core.windows.net` URLs.

1. Remove from the development catalog `keyValues` (and live store): `StorageAccount:Images:BlobEndpoint`, `QueueEndpoint`, `TableEndpoint`. Keep `Name` and `Protocol`. Local Azurite still needs custom endpoints in `appsettings.json` / `api.env`.
2. Remove `StorageAccount:Functions:Key` / `az-storage-functions-accountkey` from the catalog, Key Vault, and App Configuration **unless** Functions are first wired to this store. Prefer identity-based Functions connections (section B) over teaching Functions to read that key.
3. Confirm hosted App Configuration has **no** `SqlConnection:ServerAdminName` / `ServerAdminPassword` (Entra-only). Those remain local-only in `infra/secrets/api.env`.
4. Confirm there is no leftover `StorageConnection:Images` / `StorageConnection:Functions` **connection string** in this App Configuration store (old test export). The API uses piece-wise `StorageAccount:Images:*` only.

### B. Storage access with managed identity

1. **Images account (API)** — grant the App Service MI a data-plane role (typically Storage Blob Data Contributor) on `st<env>dertinfoimagesuks`. Change `StorageAccountConnection` to use `DefaultAzureCredential` (or Azure.Storage.Blobs with a token credential) instead of `AccountKey`. Drop `StorageAccount:Images:Key` / `az-storage-images-accountkey`.
2. **Images account (Functions)** — blob trigger `StorageConnection:Images` is a connection string. Switch to identity-based (`StorageConnection:Images__accountName` + credential) per [Azure Functions identity-based connections](https://learn.microsoft.com/en-us/azure/azure-functions/functions-reference?tabs=blob#connecting-to-host-storage-with-an-identity).
3. **Function App host storage** — `AzureWebJobsStorage` is a second account. Move to identity-based host storage (`AzureWebJobsStorage__accountName`).
4. **Harden** — consider `allowSharedKeyAccess: false` on the accounts once no client uses keys (Azurite/local remains key-based).

## Out of scope

- Local native / Azurite (keep the well-known emulator key in `infra/secrets/api.env`).
- Aligning Functions onto new-stack RG naming (separate from this identity work).
