#!/usr/bin/env node
/**
 * Start the local estate per infra/dev/runtime.json (native | docker | off).
 * Phases: loadConfig → guards/doctor → planActions → applyActions.
 */
import { spawn, spawnSync } from 'node:child_process';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import {
  API_ENV_PATH,
  AZURITE_DATA_DIR,
  DEV_DIR,
  PATHS,
  PIDS_PATH,
  PORTS,
  REPO_ROOT,
  RUNTIME_EXAMPLE_PATH,
  RUNTIME_PATH,
  RUNTIME_SERVICE_KEYS,
  anyMode,
  ensureDir,
  envFileToProcessEnv,
  loadRuntime,
  parseSemver,
  portOpen,
  toolVersionOutput,
} from './paths.mjs';
import {
  isServiceHealthy,
  portConflict,
  probeAzuriteApiVersion,
} from './health.mjs';
import {
  assertDockerAvailable,
  composeBuild,
  composeServiceName,
  composeStop,
  composeUp,
  dockerBridgeEnv,
} from './docker.mjs';

/** Start order: deps first, then apps, then frontends. */
const START_ORDER = ['azurite', 'sql', 'api', 'imageResize', 'web', 'app'];

const NATIVE_PID_KEYS = {
  api: ['api'],
  web: ['webNg', 'web'],
  app: ['appNg', 'app'],
  imageResize: ['imageResize'],
  azurite: ['azurite'],
  sql: [],
};

function runDoctor() {
  console.log('Running doctor…');
  const r = spawnSync(process.execPath, [path.join(DEV_DIR, 'doctor.mjs')], {
    cwd: REPO_ROOT,
    stdio: 'inherit',
  });
  if (r.status !== 0) {
    process.exit(r.status ?? 1);
  }
}

function readPids() {
  if (!fs.existsSync(PIDS_PATH)) return { native: {}, docker: [] };
  try {
    const raw = JSON.parse(fs.readFileSync(PIDS_PATH, 'utf8'));
    if (raw.native || raw.docker) {
      return { native: raw.native || {}, docker: raw.docker || [] };
    }
    // Legacy flat pid map
    return { native: raw, docker: [] };
  } catch {
    return { native: {}, docker: [] };
  }
}

function writePids(state) {
  fs.writeFileSync(PIDS_PATH, JSON.stringify(state, null, 2) + '\n', 'utf8');
}

function stopPid(pid) {
  if (!pid) return;
  try {
    if (process.platform === 'win32') {
      spawnSync('taskkill', ['/PID', String(pid), '/T', '/F'], { stdio: 'ignore' });
    } else {
      process.kill(pid, 'SIGTERM');
    }
  } catch {
    // already gone
  }
}

function stopNativeService(name, state) {
  for (const key of NATIVE_PID_KEYS[name] || []) {
    if (state.native[key]) {
      console.log(`Stopping native ${key} (pid ${state.native[key]})`);
      stopPid(state.native[key]);
      delete state.native[key];
    }
  }
}

function stopDockerService(name, state) {
  const composeName = composeServiceName(name);
  if (state.docker.includes(composeName)) {
    composeStop(composeName);
    state.docker = state.docker.filter((s) => s !== composeName);
  } else {
    // Still try stop in case started outside pid tracking
    composeStop(composeName);
  }
}

function nativeEnv(extra = {}) {
  const nugetPackages = path.join(os.homedir(), '.nuget', 'packages');
  return {
    ...process.env,
    NUGET_PACKAGES: nugetPackages,
    ...extra,
  };
}

function spawnManaged(name, command, args, cwd, env = {}) {
  const logDir = path.join(DEV_DIR, 'logs');
  ensureDir(logDir);
  const logPath = path.join(logDir, `${name}.log`);
  const errPath = path.join(logDir, `${name}.err.log`);
  const outFd = fs.openSync(logPath, 'a');
  const errFd = fs.openSync(errPath, 'a');

  console.log(`[${name}] ${command} ${args.join(' ')}`);
  console.log(`[${name}] log → ${logPath}`);

  const child = spawn(command, args, {
    cwd,
    env: nativeEnv(env),
    detached: true,
    stdio: ['ignore', outFd, errFd],
    shell: process.platform === 'win32',
    windowsHide: true,
  });
  child.on('error', (e) => {
    try {
      fs.writeSync(errFd, `[spawn error] ${e.message}\n`);
    } catch {
      // ignore
    }
  });
  child.unref();
  return child.pid;
}

function localBin(projectDir, binName) {
  const win = process.platform === 'win32';
  const candidate = path.join(projectDir, 'node_modules', '.bin', win ? `${binName}.cmd` : binName);
  if (fs.existsSync(candidate)) return candidate;
  return null;
}

async function waitForPort(port, label, timeoutMs = 180_000) {
  const started = Date.now();
  console.log(`Waiting for ${label} on :${port} (up to ${Math.round(timeoutMs / 1000)}s)…`);
  while (Date.now() - started < timeoutMs) {
    if (await portOpen(port, '127.0.0.1', 500)) {
      console.log(`${label} ready on :${port} after ${Date.now() - started}ms`);
      return true;
    }
    await new Promise((r) => setTimeout(r, 1000));
  }
  return false;
}

async function waitUntilHealthy(name, cfg, timeoutMs = 300_000) {
  const started = Date.now();
  console.log(`Waiting for ${name} healthy (up to ${Math.round(timeoutMs / 1000)}s)…`);
  while (Date.now() - started < timeoutMs) {
    if (await isServiceHealthy(name, cfg)) {
      console.log(`${name} healthy after ${Date.now() - started}ms`);
      return true;
    }
    await new Promise((r) => setTimeout(r, 2000));
  }
  return false;
}

/** Fail-fast guards beyond doctor (schema already validated by loadRuntime). */
function runGuards(runtime) {
  console.log('\nGuards');
  if (anyMode(runtime, 'docker')) {
    assertDockerAvailable();
    console.log('  OK  docker compose available');
  }

  if (runtime.web.mode === 'native' || runtime.app.mode === 'native') {
    const r = spawnSync(process.platform === 'win32' ? 'where' : 'which', ['swa'], {
      encoding: 'utf8',
      shell: process.platform === 'win32',
    });
    if (r.status !== 0) {
      throw new Error('swa required for native web/app — npm install -g @azure/static-web-apps-cli');
    }
  }

  if (runtime.imageResize.mode === 'native') {
    const funcOut = toolVersionOutput('func');
    const funcVer = parseSemver(funcOut);
    if (!funcVer || funcVer.major < 4) {
      throw new Error('Azure Functions Core Tools v4+ required for native imageResize');
    }
  }

  if (runtime.azurite.mode === 'native') {
    const azOut = toolVersionOutput('azurite');
    const azVer = parseSemver(azOut);
    if (!azVer) {
      throw new Error('azurite CLI required for native azurite');
    }
  }

  // Docker API needs SQL + Azurite reachable (docker mode or already healthy host).
  if (runtime.api.mode === 'docker') {
    if (runtime.sql.mode === 'off') {
      console.warn(
        '  WARN api is docker and sql is off — ensure SQL is reachable (host.docker.internal / compose)',
      );
    }
    if (runtime.azurite.mode === 'off') {
      console.warn(
        '  WARN api is docker and azurite is off — ensure blob storage is reachable',
      );
    }
  }

  console.log('  OK  mode/tooling guards');
}

/**
 * @returns {Array<{ name: string, action: 'skip-off'|'skip-healthy'|'start', cfg: object }>}
 */
async function planActions(runtime) {
  const plan = [];
  for (const name of START_ORDER) {
    const cfg = runtime[name];
    if (cfg.mode === 'off') {
      plan.push({ name, action: 'skip-off', cfg });
      continue;
    }
    const healthy = await isServiceHealthy(name, cfg);
    if (healthy && !cfg.rebuild) {
      plan.push({ name, action: 'skip-healthy', cfg });
      continue;
    }
    if (!healthy && !cfg.rebuild && (await portConflict(name))) {
      throw new Error(
        `${name}: port in use but service not healthy. Set rebuild: true or free the port.`,
      );
    }
    plan.push({ name, action: 'start', cfg });
  }
  return plan;
}

async function ensureAzuriteNative(state, rebuild) {
  if (!rebuild) {
    const up =
      (await portOpen(PORTS.azuriteBlob)) &&
      (await portOpen(PORTS.azuriteQueue)) &&
      (await portOpen(PORTS.azuriteTable));
    if (up) {
      if (!(await probeAzuriteApiVersion())) {
        throw new Error(
          'Azurite on :10000 rejects Storage API 2024-11-04. Stop it, ensure azurite >= 3.34, restart.',
        );
      }
      console.log('Azurite already listening — reusing (API 2024-11-04 OK)');
      return;
    }
  }

  stopNativeService('azurite', state);
  ensureDir(AZURITE_DATA_DIR);
  const hostPortArgs = [
    '--silent',
    '--blobHost',
    '127.0.0.1',
    '--queueHost',
    '127.0.0.1',
    '--tableHost',
    '127.0.0.1',
    '--blobPort',
    String(PORTS.azuriteBlob),
    '--queuePort',
    String(PORTS.azuriteQueue),
    '--tablePort',
    String(PORTS.azuriteTable),
  ];
  const diskArgs = [...hostPortArgs, '--location', AZURITE_DATA_DIR];
  const memoryArgs = [...hostPortArgs, '--inMemoryPersistence'];

  state.native.azurite = spawnManaged('azurite', 'azurite', diskArgs, REPO_ROOT);
  if (await waitForPort(PORTS.azuriteBlob, 'Azurite blob', 20_000)) {
    if (await probeAzuriteApiVersion()) return;
    console.warn('Azurite rejected API 2024-11-04 — restarting in-memory (no --location)');
    stopPid(state.native.azurite);
    delete state.native.azurite;
  } else {
    console.warn('Azurite disk start failed — retrying --inMemoryPersistence (no --location)');
    stopPid(state.native.azurite);
    delete state.native.azurite;
  }

  state.native.azurite = spawnManaged('azurite', 'azurite', memoryArgs, REPO_ROOT);
  if (!(await waitForPort(PORTS.azuriteBlob, 'Azurite blob (in-memory)', 30_000))) {
    throw new Error('Azurite did not open :10000 in time');
  }
  if (!(await probeAzuriteApiVersion())) {
    throw new Error('Azurite still rejects Storage API 2024-11-04 after in-memory start');
  }
}

async function startNativeApi(state, cfg, secretEnv) {
  stopNativeService('api', state);
  const shouldBuild = cfg.rebuild || !cfg.hotReload;
  if (shouldBuild) {
    console.log('[api] dotnet build…');
    const build = spawnSync('dotnet', ['build', PATHS.apiProject, '--nologo'], {
      cwd: PATHS.apiCwd,
      env: nativeEnv(secretEnv),
      stdio: 'inherit',
      shell: process.platform === 'win32',
    });
    if (build.status !== 0) throw new Error('api: dotnet build failed');
  }

  const args = cfg.hotReload
    ? [
        'watch',
        'run',
        '--project',
        PATHS.apiProject,
        '--no-restore',
        '--',
        '--urls',
        `http://localhost:${PORTS.api}`,
      ]
    : [
        'run',
        '--project',
        PATHS.apiProject,
        '--no-restore',
        '--no-build',
        '--',
        '--urls',
        `http://localhost:${PORTS.api}`,
      ];

  state.native.api = spawnManaged('api', 'dotnet', args, PATHS.apiCwd, secretEnv);
}

async function startNativeFrontend(kind, state, cfg) {
  const clientDir = kind === 'web' ? PATHS.webClient : PATHS.appClient;
  const ngPort = kind === 'web' ? PORTS.webNg : PORTS.appNg;
  const swaPort = kind === 'web' ? PORTS.web : PORTS.app;
  const ngKey = kind === 'web' ? 'webNg' : 'appNg';

  stopNativeService(kind, state);

  const ng = localBin(clientDir, 'ng');
  if (!ng) {
    throw new Error(`${kind}: ng not found — run npm run ${kind}:install`);
  }

  if (cfg.hotReload) {
    state.native[ngKey] = spawnManaged(
      ngKey,
      ng,
      ['serve', '--port', String(ngPort), '--host', '127.0.0.1'],
      clientDir,
    );
    if (!(await waitForPort(ngPort, `${kind} ng serve`, 180_000))) {
      throw new Error(`${kind}: ng serve did not open :${ngPort} — not starting SWA`);
    }
    state.native[kind] = spawnManaged(
      kind,
      'swa',
      [
        'start',
        `http://127.0.0.1:${ngPort}`,
        '--port',
        String(swaPort),
        '--devserver-timeout',
        '120',
      ],
      clientDir,
    );
  } else {
    console.log(`[${kind}] ng build (hotReload: false)…`);
    const build = spawnSync(ng, ['build'], {
      cwd: clientDir,
      env: nativeEnv(),
      stdio: 'inherit',
      shell: process.platform === 'win32',
    });
    if (build.status !== 0) throw new Error(`${kind}: ng build failed`);
    const dist = path.join(clientDir, 'dist');
    if (!fs.existsSync(dist)) throw new Error(`${kind}: missing dist after build`);
    state.native[kind] = spawnManaged(
      kind,
      'swa',
      ['start', dist, '--port', String(swaPort)],
      clientDir,
    );
  }
}

async function startNativeImageResize(state, cfg) {
  stopNativeService('imageResize', state);
  // Always build when rebuild, or when script-root missing (WorkerExtensions dual-csproj).
  const needBuild = cfg.rebuild || !fs.existsSync(PATHS.functionsScriptRoot);
  if (needBuild) {
    console.log('[imageResize] building DertInfoImageResizeV4…');
    const build = spawnSync(
      'dotnet',
      ['build', PATHS.functionsProject, '--configuration', 'Debug', '--nologo'],
      {
        cwd: PATHS.functionsDir,
        env: nativeEnv(),
        stdio: 'inherit',
        shell: process.platform === 'win32',
      },
    );
    if (build.status !== 0) throw new Error('imageResize: dotnet build failed');
  }
  if (!fs.existsSync(PATHS.functionsScriptRoot)) {
    throw new Error(`imageResize: missing script root ${PATHS.functionsScriptRoot}`);
  }
  state.native.imageResize = spawnManaged(
    'imageResize',
    'func',
    ['start', '--port', String(PORTS.imageResize), '--script-root', PATHS.functionsScriptRoot],
    PATHS.functionsDir,
  );
}

async function startDockerService(name, cfg, runtime, state) {
  stopNativeService(name, state);
  const composeName = composeServiceName(name);
  if (state.docker.includes(composeName) || cfg.rebuild) {
    composeStop(composeName);
  }

  const bridge = dockerBridgeEnv(runtime);
  if (cfg.rebuild) {
    composeBuild(composeName);
  }
  composeUp(composeName, {
    env: bridge,
    noDeps: true,
    forceRecreate: Boolean(cfg.rebuild),
  });
  if (!state.docker.includes(composeName)) {
    state.docker.push(composeName);
  }
}

async function applyAction(item, runtime, state, secretEnv) {
  const { name, action, cfg } = item;
  if (action === 'skip-off') {
    console.log(`Skipping ${name} (mode: off)`);
    return;
  }
  if (action === 'skip-healthy') {
    console.log(`Skipping ${name} (already healthy, rebuild: false)`);
    return;
  }

  console.log(`\n→ Starting ${name} (mode: ${cfg.mode}, rebuild: ${cfg.rebuild}${cfg.hotReload != null ? `, hotReload: ${cfg.hotReload}` : ''})`);

  if (cfg.mode === 'docker') {
    if (name === 'sql' || name === 'azurite') {
      await startDockerService(name, cfg, runtime, state);
    } else {
      await startDockerService(name, cfg, runtime, state);
    }
  } else if (cfg.mode === 'native') {
    switch (name) {
      case 'azurite':
        await ensureAzuriteNative(state, cfg.rebuild);
        break;
      case 'sql':
        console.log(
          'sql mode native — expecting SQL Express from api.env (not started by npm). Verify with doctor.',
        );
        if (!(await isServiceHealthy('sql', cfg))) {
          throw new Error('sql mode native but sqlcmd cannot connect — start SQL Express / fix api.env');
        }
        break;
      case 'api':
        await startNativeApi(state, cfg, secretEnv);
        break;
      case 'web':
      case 'app':
        await startNativeFrontend(name, state, cfg);
        break;
      case 'imageResize':
        await startNativeImageResize(state, cfg);
        break;
      default:
        throw new Error(`Unknown service ${name}`);
    }
  }

  // Wait for availability (metric / correctness)
  if (name === 'sql' && cfg.mode === 'native') {
    return; // already verified
  }
  if (name === 'azurite' && cfg.mode === 'native') {
    return; // ensureAzurite already waited
  }
  const timeout = name === 'web' || name === 'app' || name === 'api' ? 300_000 : 120_000;
  if (!(await waitUntilHealthy(name, cfg, timeout))) {
    throw new Error(`${name} did not become healthy in time`);
  }
}

function printSummary(runtime) {
  const line = (label, port, cfg) => {
    if (cfg.mode === 'off') return `  ${label.padEnd(14)}(off)`;
    return `  ${label.padEnd(14)}http://localhost:${port}  [${cfg.mode}]`;
  };
  console.log(`
Estate ready (pids → ${path.relative(REPO_ROOT, PIDS_PATH)})

${line('API', PORTS.api, runtime.api)}
${line('Website', PORTS.web, runtime.web)}
${line('App (PWA)', PORTS.app, runtime.app)}
${line('Image resize', PORTS.imageResize, runtime.imageResize)}
  Azurite       ${runtime.azurite.mode === 'off' ? '(off)' : `http://127.0.0.1:${PORTS.azuriteBlob}  [${runtime.azurite.mode}]`}
  SQL           ${runtime.sql.mode === 'off' ? '(off — use existing)' : `[${runtime.sql.mode}]`}

  npm run status
  npm run stop
`);
}

async function main() {
  if (process.argv.includes('--help') || process.argv.includes('-h')) {
    console.log(`Usage: node infra/dev/start.mjs
Starts services per infra/dev/runtime.json after doctor + guards.
Copy ${path.relative(REPO_ROOT, RUNTIME_EXAMPLE_PATH)} → runtime.json
Each service: { "mode": "native"|"docker"|"off", "rebuild": bool, "hotReload"?: bool }`);
    process.exit(0);
  }

  const startedAt = Date.now();
  const runtime = loadRuntime();
  const runtimeSource = fs.existsSync(RUNTIME_PATH)
    ? path.relative(REPO_ROOT, RUNTIME_PATH)
    : `defaults (copy ${path.relative(REPO_ROOT, RUNTIME_EXAMPLE_PATH)} → runtime.json)`;

  console.log(`Runtime: ${runtimeSource}`);
  for (const key of RUNTIME_SERVICE_KEYS) {
    const c = runtime[key];
    const hr = c.hotReload != null ? ` hotReload=${c.hotReload}` : '';
    console.log(`  ${key}: mode=${c.mode} rebuild=${c.rebuild}${hr}`);
  }

  runDoctor();
  runGuards(runtime);

  const secretEnv = envFileToProcessEnv(API_ENV_PATH);
  const state = readPids();
  const plan = await planActions(runtime);

  console.log('\nPlan');
  for (const item of plan) {
    console.log(`  ${item.name}: ${item.action}`);
  }

  for (const item of plan) {
    await applyAction(item, runtime, state, secretEnv);
  }

  writePids(state);
  printSummary(runtime);
  console.log(`Start completed in ${((Date.now() - startedAt) / 1000).toFixed(1)}s`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
