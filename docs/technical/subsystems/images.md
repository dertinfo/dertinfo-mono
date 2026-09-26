---
name: Images
type: subsystem
status: active
updated: 2026-09-26
---

# Images

How uploaded pictures are stored, resized, and shown. Account names, public access, and who may write blobs are in [Security — storage accounts](security.md#storage-accounts). The hosted API host name versus the App Configuration label is in [Configuration](../infra/configuration.md#hosted-api-runtime-and-app-configuration-label).

## Layout

Each picture is one database row (`Container`, `BlobPath`, `BlobName`) and several blobs in the images storage account.

| Blob path | Role |
|-----------|------|
| `{container}/originals/{blobName}` | File the user uploaded. Download, and the app zoom view. |
| `{container}/480x360/{blobName}` | Gallery tiles, headers, and the app marking-sheet thumbnail. |
| `{container}/100x100/{blobName}` | Avatars and list thumbnails. |

Containers are `groupimages`, `eventimages`, `sheetimages`, and `defaultimages`. Older rows with an empty `BlobPath` still use `{container}/{blobName}` for the original. The resize function writes the sized copies beside `originals`, not inside it.

## Who does what

| App | Role |
|-----|------|
| [`apps/dert-api`](../../../apps/dert-api/) | Uploads the original, stores `BlobPath` (`originals`) and `BlobName`, and returns a public URL `{account}/{container}/{blobPath}/{blobName}`. Sets an image content type on that upload. |
| [`apps/dert-functions`](../../../apps/dert-functions/) | Watches `{container}/originals/{name}` and writes `100x100` and `480x360` with an image content type. |
| [`apps/dert-web`](../../../apps/dert-web/) | `imageDimension` swaps `originals` for `480x360` or `100x100` on `<img>` tags. The gallery Download link uses the API URL unchanged. |
| [`apps/dert-app`](../../../apps/dert-app/) | `sizedImage` does the same swap. Marking-sheet thumbnails use `480x360`. Expand / zoom uses `originals`. |

Local `ASPNETCORE_ENVIRONMENT=Development` publishes image URLs on Azurite (`http://127.0.0.1:10000/{name}`). Hosted development and production publish `https://{StorageAccount:Images:Name}.blob.core.windows.net`.

## Deploy together

Ship the API, website, and app in the same release. An API that adds `originals` to the URL, with a client that still inserts the size in front of the file name, requests `{container}/originals/480x360/{blobName}`. That blob is not created. The function app can follow: content type applies to uploads after it is deployed. Blobs already resized keep their previous content type until they are uploaded again.
