# Image URLs use cloud storage, originals for download, resized copies on screen

## Summary of the work completed

Hosted development API image URLs now use the cloud images account. Public image URLs include the `originals` folder. The website and the app swap that folder for `480x360` or `100x100` when drawing a picture. Download (website) and expand/zoom (app) keep the original. New uploads and resized copies are stored with an image content type.

Technical layout: [Images](../../technical/subsystems/images.md). Hosted runtime versus App Configuration label: [Configuration](../../technical/infra/configuration.md#hosted-api-runtime-and-app-configuration-label).

- [Pull request 46](https://github.com/dertinfo/dertinfo-mono/pull/46) — hosted development `ASPNETCORE_ENVIRONMENT=Production`, App Configuration label stays `Development`.
- [Pull request 47](https://github.com/dertinfo/dertinfo-mono/pull/47) — `BlobPath` in image URLs; website `imageDimension` and app `sizedImage`; content type on API upload and function resize.

## Why the work was completed

Development gallery images were broken. The hosted API was running as `Development`, so it published `http://127.0.0.1:10000/...` instead of `stdevdertinfoimagesuks`. After that host was corrected, download links still omitted `originals`, so they asked for `{container}/{blobName}` while the file lives at `{container}/originals/{blobName}`. Page views must keep using the resized blobs; serving the original on every `<img>` would download the full file. New resized blobs were also saved as `application/octet-stream`, which the gallery shows as a broken image.

## Date the work was started

2026-09-26

## Date the work was completed

2026-09-26

## Issues that were encountered on the way

- `ASPNETCORE_ENVIRONMENT` was also the App Configuration label. Setting the development App Service to `Production` without a separate `AZURE_APP_CONFIG_LABEL=Development` would load the wrong keys. The label is now its own setting. API source must be deployed before API infrastructure when that split changes — [Configuration](../../technical/infra/configuration.md#hosted-api-runtime-and-app-configuration-label).
- The resize function writes `{container}/480x360/{blobName}`, not `{container}/originals/480x360/{blobName}`. Client pipes replace the `originals` folder instead of inserting another segment.
- `Path.GetExtension` returns `.png`, and the resize encoder compared `png` without the dot, so every file was encoded as JPEG. The comparison now includes the dot, and the blob content type matches that extension.
- The API, website, and app have to deploy together. Shipping the API URL change first makes current clients request a path the function does not create.

## References to any best practices that we found

- [Blob properties and content type](https://learn.microsoft.com/en-us/rest/api/storageservices/set-blob-properties) — browsers treat `application/octet-stream` as a failed image.
- [Azure Functions blob trigger](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger) — resize watches `originals` and writes the sized prefixes beside it.

## Any remaining issues that we may wish to address

- Blobs resized before pull request 47 keep their previous content type until they are uploaded again.
- A stale Auth0 session can still stick on Warmup. Tracked as scenario C4 in [website auth integration tests](../planned-fixes/web-auth-integration-tests.md). Not part of this image work.
