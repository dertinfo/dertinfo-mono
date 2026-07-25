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

Full host prerequisites (Node/nvm/npm, Azurite, Core Tools, `api.env` keys): [`docs/configuration.md`](../../docs/configuration.md#base-settings-for-local-native-development). Workstream notes: [`docs/changelogs/2026-07-25-local-native-dev.md`](../../docs/changelogs/2026-07-25-local-native-dev.md).

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
| `imageResize` | `true` | `dotnet build` + `func start --script-root bin/Debug/net8.0` on :44400 |
| `azurite` | `true` | Azurite on :10000–10002 (`--location infra/data/azurite`, with in-memory fallback) |

Browse the website at `http://localhost:44200` and the PWA at `http://localhost:44300` (not the raw `ng serve` ports). `npm run start` **waits** for each ng port before launching SWA.

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

Does **not** auto-install npm packages or run `dotnet restore` on the happy path. After changing Node with nvm, reinstall project deps and globals (Azurite, SWA). See [`docs/configuration.md`](../../docs/configuration.md).

## Tooling versions

| Tool | Check | Recommended |
|------|-------|-------------|
| Node.js | `node --version` | ≥ 16.10; 20/24 LTS recommended |
| Azure Functions Core Tools | `func --version` | **v4** (e.g. 4.12.x) — `winget upgrade Microsoft.Azure.FunctionsCoreTools` |
| Azurite | `azurite --version` | **≥ 3.34.0** — `npm install -g azurite@latest` |
| SWA CLI | `swa --version` | `npm install -g @azure/static-web-apps-cli` |

Core Tools 4.12+ uses Storage API **2024-11-04**. Azurite **3.31.0** returns `InvalidHeaderValue` and the Functions host aborts. Upgrade Azurite, then **restart** it (do not leave an old process on `:10000`).

### Azurite start behaviour (`start.mjs`)

1. Reuse `:10000–10002` only if they accept API `2024-11-04`.
2. Else start disk: `--location <repo>/infra/data/azurite` (+ hosts/ports, `--silent`).
3. If disk fails (e.g. Blob GC crash): kill that process and start **`--inMemoryPersistence` without `--location`** (Azurite 3.36 rejects combining those flags).
4. If the data directory repeatedly GC-crashes, rename it (`azurite.bak`) and use a fresh empty `infra/data/azurite`.

### Functions start behaviour

Isolated worker builds emit `obj/.../WorkerExtensions.csproj` (expected). Plain `func start` in the project folder can error “found 2” projects. Orchestrator builds `DertInfoImageResizeV4.csproj` then runs `func start --port 44400 --script-root bin/Debug/net8.0`.

### Troubleshooting

| Symptom | Action |
|---------|--------|
| `nvm use` Access denied / exit 5 | Elevated terminal; fix `Program Files\nodejs` junction |
| Doctor: Azurite too old | `npm install -g azurite@latest`; stop old `:10000` process |
| Doctor: `swa` missing | `npm install -g @azure/static-web-apps-cli` |
| Functions API version / Secret Repository errors | Upgrade Azurite; confirm `Server: Azurite-Blob/3.36.x` |
| Disk Azurite GC crash | Reset `infra/data/azurite` or use in-memory fallback |
| Web SWA down, ng still compiling | Ensure start waits for `:4200` (current scripts do); raise timeout if needed |
