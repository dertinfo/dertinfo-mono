# Planned: Continue `npm run start` when a service is already up

**Status:** Not started — after the current SPA runtime-config work is merged.

**Related:** [`infra/dev/start.mjs`](../../../infra/dev/start.mjs), [`infra/dev/health.mjs`](../../../infra/dev/health.mjs), [local hybrid runtime](../../../infra/dev/README.md).

---

## Intent

Re-running `npm run start` after flipping a service from `off` to `native` (or adding a service that was not in the first start) should **leave already-running processes alone**, print that they are already running (or that their port is in use), and **start only the ones that are still down**.

Typical flow: first start is API + website; later enable the PWA in `runtime.json` and run start again without stopping the estate.

## Why

Healthy skip already exists (`skip-healthy` when `isServiceHealthy` is true and `rebuild` is false). A later service still never starts if **planning throws**:

```javascript
if (!healthy && !cfg.rebuild && (await portConflict(name))) {
  throw new Error(
    `${name}: port in use but service not healthy. Set rebuild: true or free the port.`,
  );
}
```

That abort happens in `planActions` before any `applyAction`. One occupied port (stale process, SWA not yet reporting healthy, probe false negative) stops the whole run, including services that were `off` last time and need to come up.

`npm run stop` only kills PIDs in `.pids.json`, so a second start is the intended way to add services — it must not fail-fast on siblings that are already bound.

## Scope (when scheduled)

1. Treat **port in use** like **already running**: log a clear skip (`Skipping web (port 44200 in use)` / `already healthy`) and continue the plan. Do not throw from `planActions` for that case.
2. Keep `rebuild: true` as the path that stops/restarts that one service.
3. If the occupant is not our stack (unknown process on the port), still skip and warn rather than aborting the rest of the estate; optionally mention `rebuild: true` or free the port.
4. Ensure a second start with a newly enabled service (e.g. `app` was `off`, now `native`) starts that service after skipping api/web/azurite/imageResize.
5. Update [infra/dev/README.md](../../../infra/dev/README.md) so the workflow matches: first start → enable another service in `runtime.json` → start again → skips + new starts.

## Out of scope

- Changing `npm run stop` to leave some processes running.
- Docker compose recreate behaviour beyond what `rebuild` already does.
- Making health probes stricter/looser except as needed to distinguish skip vs start.

## Current behaviour (do not regress)

- `mode: off` → `skip-off`.
- Healthy + `rebuild: false` → `skip-healthy` and continue (this already works when probes succeed).
- `rebuild: true` → stop/rebuild/start that service.
