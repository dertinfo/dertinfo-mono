# Pin SPA Docker images to Node 16 and drop dummy file: dependency

## Summary of the work completed

Web and PWA Dockerfiles now use `node:16-bookworm` instead of `node:lts`. The dummy `dertinfo-monorepo` `file:../../../..` dependency was removed from both client `package.json` / lockfiles so `npm install --force` no longer needs the repo root inside the image. Dev Docker Hub CD (`latest-dev`) can complete `npm install` in the builder stage.

## Why the work was completed

After [2026-09-07-004](./2026-09-07-004-swa-cd-caller-actions-read.md), Web Src CD Docker failed: `node:lts` is Node 24 / npm 11, which errors `EMISSINGTARGET` when the lockfile `file:` target (`node_modules/dertinfo-monorepo` → `..`) is not in the `apps/dert-web` build context. The link existed only so nested `npm install` could see the root package; it is not required at runtime. Pinning Node 16 also matches SPA CI and Angular 13/14.

## Date the work was started

2026-09-07

## Date the work was completed

2026-09-07

## Issues that were encountered on the way

- `--force` no longer skips a missing `file:` workspace on npm 11.
- Docker context is `apps/dert-web` / `apps/dert-app`, so `file:../../../..` cannot resolve even if we kept the dependency.

## References to any best practices that we found

- [npm `file:` dependencies](https://docs.npmjs.com/cli/v8/configuring-npm/package-json#local-paths)
- Angular 14 officially supports Node 16 (not Node 24)

## Any remaining issues that we may wish to address

- Global `npm install -g @angular/cli@14` in the PWA Dockerfile is still Angular 14 while the app is Angular 13; leftover, not this failure.
- Docker Hub images remain local-dev only; hosted traffic is SWA.
