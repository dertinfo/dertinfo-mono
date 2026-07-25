# Local-native runtime

Scripts for running the estate on the developer machine (SQL Server + Azurite + Auth0 tenant from secrets).

| File | Role |
|------|------|
| `paths.mjs` | Shared ports, paths, env parsing, `loadRuntime()` |
| `runtime.example.json` | Template for which services `npm run start` launches |
| `runtime.json` | Active toggles (**gitignored**) — copy from the example |
| `doctor.mjs` | Fail-fast preflight (`npm run doctor`) — includes `swa` for web/app |
| `start.mjs` | Start selected services (`npm run start`) |
| `stop.mjs` | Stop managed processes (`npm run stop`) |
| `status.mjs` | Port probes (`npm run status`) |

## Which services start

```bash
cp infra/dev/runtime.example.json infra/dev/runtime.json
```

Booleans in `runtime.json` (if the file is missing, all services default to `true`):

| Key | Example default | What starts |
|-----|-----------------|-------------|
| `api` | `true` | `dotnet watch` on :44100 |
| `web` | `true` | `ng serve` on **:4200** → SWA on **:44200** |
| `app` | `true` | `ng serve` on **:4201** → SWA on **:44300** (fixed ports so web and PWA do not clash) |
| `imageResize` | `true` | `func start` on :44400 |
| `azurite` | `true` | Azurite on :10000–10002 |

Browse the website at `http://localhost:44200` and the PWA at `http://localhost:44300` (not the raw `ng serve` ports).

To run the API in Visual Studio instead, set `"api": false` in your local (gitignored) `runtime.json`.

```bash
cp infra/secrets/api.env.example infra/secrets/api.env
# fill secrets
cp apps/dert-functions/src/dertinfo-image-resize/local.settings.json.example \
   apps/dert-functions/src/dertinfo-image-resize/local.settings.json

npm run doctor
npm run start
npm run status
npm run stop
```

Does **not** auto-install npm packages or run `dotnet restore` on the happy path. See [`docs/configuration.md`](../../docs/configuration.md).

## Tooling versions (Functions + Azurite)

| Tool | Check | Recommended |
|------|-------|-------------|
| Azure Functions Core Tools | `func --version` | **v4** (e.g. 4.12.x) — `winget upgrade Microsoft.Azure.FunctionsCoreTools` |
| Azurite | `azurite --version` | **≥ 3.34.0** — `npm install -g azurite@latest` |

Core Tools 4.12+ uses Storage API **2024-11-04**. Azurite **3.31.0** returns `InvalidHeaderValue` / “API version … not supported” and the Functions host aborts. Upgrade Azurite, then **restart** it (do not leave an old process on `:10000`).

If the on-disk store under `infra/data/azurite` fails to start (GC crash), `npm run start` falls back to `--inMemoryPersistence` **without** `--location` (Azurite 3.36 rejects combining those flags).

`npm run start` waits for each `ng serve` port before launching the matching SWA (web `:4200` → `:44200`, app `:4201` → `:44300`).
