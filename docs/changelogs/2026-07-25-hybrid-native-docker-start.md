# Hybrid native/Docker local startup

## Summary of the work completed

Replaced boolean `infra/dev/runtime.json` toggles with per-service `{ mode, rebuild, hotReload }` (`native` | `docker` | `off`). `npm run start` now skips healthy services when `rebuild` is false, supports mixed native/docker on shared host ports, and prints elapsed time when the estate is ready. Doctor and start guards are mode-aware. Documented and ran cold/warm start benchmarks B1–B4.

## Date the work was started

2026-07-25

## Date the work was completed

2026-07-25

## Issues that were encountered on the way

1. **Docker Desktop not running** — B2 failed until Docker was started; doctor/start now require compose when any service is `docker`.
2. **Docker API + native SQL Express** — `.\SQLEXPRESS` / named instances are not reachable from Linux containers. Bridge now sets `SqlConnection__ServerName=host.docker.internal,1433` (TCP must be enabled on Express) and passes credentials via compose substitutions.
3. **Compose `env_file` vs bridge** — `api.env` still supplies secrets; compose `environment` substitutions override `ServerName` / blob endpoints for hybrid modes.
4. **First docker API image build** — B2 (~19 min) was dominated by pulling base images and a cold `dertinfo-api` build; later runs with cache were much faster (B3 ~4 min).
5. **`status.mjs` import** — `portOpen` lives in `paths.mjs`, not `health.mjs` (fixed).

## References to any best practices that we found

- Fail fast on invalid runtime schema (no boolean compatibility shim).
- `--no-deps` on per-service `docker compose up` so mixed modes do not pull unwanted compose dependencies.
- Azurite Storage API `2024-11-04` probe before reuse; Functions `--script-root` after build (WorkerExtensions).
- Orchestration notes: [`infra/dev/README.md`](../../infra/dev/README.md), configuration: [`docs/configuration.md`](../configuration.md).

## Any remaining issues that we may wish to address

- Docker image-resize “healthy” is currently TCP-only; a stronger HTTP probe would be better.
- Named-instance SQL Express without a fixed TCP port still needs Configuration Manager setup for hybrid docker API.
- Optional: exclude doctor Auth0/SQL latency from the printed start timer, or add a `--bench` flag that skips non-essential doctor checks.
- Commit/PR for this workstream on `feature/local-native-dev` (or a follow-up branch) when ready.
- First-clone onboarding docs were tightened in a follow-up (root README checklist, secrets guidance, CONTRIBUTING local modes).
