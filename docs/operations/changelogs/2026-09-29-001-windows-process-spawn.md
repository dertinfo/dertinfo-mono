# Windows local process spawning without DEP0190

## Summary of the work completed

The local development orchestration now uses `cross-spawn` for synchronous and managed child processes. Windows no longer combines an argument array with `shell: true`, removing Node.js `DEP0190` warnings while preserving support for `.cmd` shims used by Angular CLI, Azurite, and Static Web Apps CLI. The change covers start, stop, Docker Compose, tool-version, and guard paths.

## Why the work was completed

Node.js reports `DEP0190` because arguments passed alongside `shell: true` are concatenated without escaping, which can permit shell injection and gives callers a misleading impression that each argument remains isolated. A cross-platform process launcher retains Windows command-shim compatibility without relying on that deprecated combination.

## Date the work was started

2026-09-29

## Date the work was completed

2026-09-29

## Issues that were encountered on the way

- The first trace showed that the warning came from the shared tool-version helper as well as the main launcher. The fix was applied across all local orchestration paths that used `shell: true` with arguments.
- Installing the root dependency initially caused unrelated lockfile churn because the current root manifest no longer declares the workspaces recorded in the existing lockfile. The lockfile was preserved and updated only with the root `cross-spawn` dependency.
- Full estate startup reached the API build without `DEP0190`, then stopped because the new computer has .NET runtimes but no .NET SDK. SQL Server and Azurite were healthy; this environment prerequisite is separate from the process-spawning fix.

## References to any best practices that we found

- [Node.js `DEP0190` documentation](https://nodejs.org/api/deprecations.html#dep0190-passing-args-to-nodechild_process-execfilespawn-with-shell-option) — avoid passing an argument array when a child process uses a shell.
- [`cross-spawn`](https://github.com/moxystudio/node-cross-spawn) — cross-platform spawning with Windows command-shim handling.
- [Local development orchestration](../../../infra/dev/README.md) — supported runtime modes, prerequisites, and startup workflow.

## Any remaining issues that we may wish to address

- Install a compatible .NET SDK before retrying the full local estate.
- Angular CLI 14 reports Node.js 24 as unsupported; use a supported Node.js version or address this as part of the planned frontend upgrade.
