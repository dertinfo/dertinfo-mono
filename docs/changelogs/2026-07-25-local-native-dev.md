# Local-native development orchestration

## Summary of the work completed

Delivered a host-first local development path for the monorepo so the full estate (Azurite, API, website, PWA, image-resize Functions) can run without Docker Compose as the primary workflow.

**Outcomes:**

- Root npm scripts: `doctor`, `start`, `stop`, `status` backed by [`infra/dev/`](../../infra/dev/)
- Secrets template and docs: [`infra/secrets/`](../../infra/secrets/), [`docs/configuration.md`](../configuration.md)
- Fixed local ports: API `:44100`, web ng `:4200` → SWA `:44200`, app ng `:4201` → SWA `:44300`, Functions `:44400`, Azurite `:10000–10002`
- Documented tooling floors (Node ≥ 16.10 / verified on 24 LTS, Azurite ≥ 3.34, Functions Core Tools v4, SWA CLI)
- Verified full `npm run start` on Windows with SQL Express and Auth0 dev tenant

Operational how-to: [README local native quick start](../../README.md#local-native-quick-start), [`infra/dev/README.md`](../../infra/dev/README.md), [`docs/configuration.md`](../configuration.md#base-settings-for-local-native-development).

### Pull requests

| PR | Title |
|----|--------|
| [#11](https://github.com/dertinfo/dertinfo-mono/pull/11) | Add local-native development orchestration (doctor/start/stop) |

Related: main received EF8 string nullability via [#10](https://github.com/dertinfo/dertinfo-mono/pull/10); this branch was updated from that merge before the local-native commit.

## Date the work was started

2026-07-25

## Date the work was completed

2026-07-25

## Issues that were encountered on the way

1. **Functions Core Tools vs Azurite API version**  
   Core Tools 4.12+ uses Storage API `2024-11-04`. Azurite **3.31.0** rejects it (`InvalidHeaderValue` / Blob Secret Repository errors).  
   **Fix:** Require Azurite **≥ 3.34.0** (`npm install -g azurite@latest`); `npm run doctor` enforces the floor. After upgrading, kill any old process still on `:10000–10002`.

2. **Stale Azurite process after CLI upgrade**  
   `npm run start` reuses listeners on `:10000` if present, so an upgraded CLI could still talk to an old emulator.  
   **Fix:** Probe API `2024-11-04` before reuse; fail with a clear stop/restart message if rejected.

3. **Corrupt / incompatible on-disk Azurite workspace**  
   `--location infra/data/azurite` crashed during Blob GC (`Cannot close server in status Starting`). A **fresh** location (`azurite-v2`) worked.  
   **Fix:** Reset the data folder (rename old → `.bak`, use clean `infra/data/azurite`). Document that GC crashes usually mean reset the store, not that Azurite is unusable.

4. **Illegal Azurite flag combination**  
   Fallback used `--location` together with `--inMemoryPersistence`. Azurite 3.36 exits: option not supported when `--location` is set.  
   **Fix:** In-memory fallback omits `--location`.

5. **`func start` finds two `.csproj` files**  
   After build, `obj/.../WorkerExtensions.csproj` sits beside the real project; Core Tools errors with “Expected 1 … found 2”.  
   **Fix:** `dotnet build` then `func start --script-root bin/Debug/net8.0` (cwd remains the functions project so `local.settings.json` is found). That generated project is normal for isolated worker — do not treat it as a second app.

6. **Web SWA started before Angular was ready**  
   Concurrent full-estate start left web SWA dead while `ng serve` was still compiling (default SWA devserver timeout).  
   **Fix:** Wait for ng ports `:4200` / `:4201` before starting SWA; raise `--devserver-timeout`.

7. **nvm-windows `nvm use` → Access denied (exit 5)**  
   Switching Node versions needs elevation to update `C:\Program Files\nodejs`.  
   **Mitigation (docs):** Run elevated `nvm use`, or fix the junction; reinstall global CLIs (`azurite`, `swa`) after switching Node.

8. **Missing global SWA CLI after Node switch**  
   Doctor failed until `npm install -g @azure/static-web-apps-cli`.  
   **Fix:** Document SWA as a required global; reinstall globals after `nvm use`.

## References to any best practices that we found

- Prefer **host-native** local run with checked-in local defaults and gitignored `infra/secrets/api.env` ([configuration principles](../configuration.md))
- Keep **secrets and per-developer values** out of `appsettings.json`; inject via process env (`Auth0__X` → `Auth0:X`)
- Pin **tooling floors** in doctor rather than failing obscurely at runtime (Azurite / Core Tools)
- Use **fixed ports** so Auth0 callbacks and SWA URLs stay stable across developers
- Isolated Functions: treat **WorkerExtensions** as a build artifact; start via **script-root** ([Azure Functions isolated worker](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide))
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) / [Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local) install docs
- [nvm-windows](https://github.com/coreybutler/nvm-windows) — admin required for `nvm use` symlink updates

## Any remaining issues that we may wish to address

- Website silent-auth / group-create UX on localhost — still deferred; PWA does not show the same login problem ([`local-silent-auth-localhost.md`](../planned-fixes/local-silent-auth-localhost.md))
- Website warmup race — separate planned fix ([`web-warmup-race-condition.md`](../planned-fixes/web-warmup-race-condition.md))
- Managed process logs under `infra/dev/logs/` are often empty on Windows (stdio capture) — improve logging for diagnosis
- Optional: clear guidance / automation if `infra/data/azurite` GC-crashes again without manual rename
- Changelog/PR polish and any remaining review comments on [#11](https://github.com/dertinfo/dertinfo-mono/pull/11)
