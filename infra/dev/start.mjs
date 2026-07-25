#!/usr/bin/env node
/**
 * Start the local-native estate according to infra/dev/runtime.json.
 * Runs doctor first. Does not npm install / dotnet restore.
 */
import { spawn, spawnSync } from 'node:child_process';
import fs from 'node:fs';
import http from 'node:http';
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
  ensureDir,
  envFileToProcessEnv,
  loadRuntime,
  parseSemver,
  portOpen,
  toolVersionOutput,
} from './paths.mjs';

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

function stopManagedPids() {
  if (!fs.existsSync(PIDS_PATH)) return;
  let pids;
  try {
    pids = JSON.parse(fs.readFileSync(PIDS_PATH, 'utf8'));
  } catch {
    return;
  }
  for (const [name, pid] of Object.entries(pids)) {
    try {
      if (process.platform === 'win32') {
        spawnSync('taskkill', ['/PID', String(pid), '/T', '/F'], { stdio: 'ignore' });
      } else {
        process.kill(pid, 'SIGTERM');
      }
      console.log(`Stopped ${name} (pid ${pid})`);
    } catch {
      // already gone
    }
  }
  fs.unlinkSync(PIDS_PATH);
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
  // openSync so stdio FDs are valid immediately. Do not close them after spawn —
  // the detached child still needs them (closing early empties logs on Windows).
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

function skip(name, reason) {
  console.log(`Skipping ${name} (${reason})`);
}

/** Poll until TCP port accepts connections (ng / Azurite readiness). */
async function waitForPort(port, label, timeoutMs = 180_000) {
  const started = Date.now();
  console.log(`Waiting for ${label} on :${port} (up to ${Math.round(timeoutMs / 1000)}s)…`);
  while (Date.now() - started < timeoutMs) {
    if (await portOpen(port, '127.0.0.1', 500)) {
      const waitedMs = Date.now() - started;
      console.log(`${label} ready on :${port} after ${waitedMs}ms`);
      return true;
    }
    await new Promise((r) => setTimeout(r, 1000));
  }
  return false;
}

async function ensureAzurite(pids) {
  const azOut = toolVersionOutput('azurite');
  parseSemver(azOut); // ensure CLI responds

  const up =
    (await portOpen(PORTS.azuriteBlob)) &&
    (await portOpen(PORTS.azuriteQueue)) &&
    (await portOpen(PORTS.azuriteTable));
  if (up) {
    const compatible = await probeAzuriteApiVersion();
    if (!compatible) {
      console.error(
        'Azurite on :10000 rejects Storage API 2024-11-04 (likely an old process). Stop it (npm run stop / kill ports 10000-10002), ensure azurite --version is >= 3.34.0, then npm run start again.',
      );
      process.exit(1);
    }
    console.log('Azurite already listening — reusing (API 2024-11-04 OK)');
    return;
  }

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
  // Azurite 3.36+: --inMemoryPersistence cannot be combined with --location
  const memoryArgs = [...hostPortArgs, '--inMemoryPersistence'];

  pids.azurite = spawnManaged('azurite', 'azurite', diskArgs, REPO_ROOT);
  if (await waitForPort(PORTS.azuriteBlob, 'Azurite blob', 20_000)) {
    if (await probeAzuriteApiVersion()) return;
    console.warn(
      'Azurite opened ports but rejected API 2024-11-04 — restarting with --inMemoryPersistence (no --location)',
    );
    stopPid(pids.azurite);
    delete pids.azurite;
  } else {
    console.warn(
      'Azurite disk location failed to stay up — retrying with --inMemoryPersistence (without --location; required by Azurite 3.36+)',
    );
    stopPid(pids.azurite);
    delete pids.azurite;
  }

  pids.azurite = spawnManaged('azurite', 'azurite', memoryArgs, REPO_ROOT);
  if (!(await waitForPort(PORTS.azuriteBlob, 'Azurite blob (in-memory)', 30_000))) {
    console.error('Azurite did not open :10000 in time');
    process.exit(1);
  }
  if (!(await probeAzuriteApiVersion())) {
    console.error('Azurite still rejects Storage API 2024-11-04 after in-memory start');
    process.exit(1);
  }
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

/** True if Azurite accepts (or at least understands) x-ms-version 2024-11-04 without "not supported". */
function probeAzuriteApiVersion() {
  return new Promise((resolve) => {
    const req = http.request(
      {
        host: '127.0.0.1',
        port: PORTS.azuriteBlob,
        path: '/devstoreaccount1?restype=service&comp=properties',
        method: 'GET',
        headers: { 'x-ms-version': '2024-11-04' },
        timeout: 2000,
      },
      (res) => {
        let body = '';
        res.on('data', (c) => {
          body += c;
        });
        res.on('end', () => {
          const bad = /API version 2024-11-04 is not supported/i.test(body);
          resolve(!bad);
        });
      },
    );
    req.on('timeout', () => {
      req.destroy();
      resolve(false);
    });
    req.on('error', () => resolve(false));
    req.end();
  });
}

async function main() {
  if (process.argv.includes('--help') || process.argv.includes('-h')) {
    console.log(`Usage: node infra/dev/start.mjs
Starts selected services after doctor passes (see infra/dev/runtime.json).
Copy ${path.relative(REPO_ROOT, RUNTIME_EXAMPLE_PATH)} → runtime.json to customise.
Does not install npm/dotnet packages.`);
    process.exit(0);
  }

  const runtime = loadRuntime();
  const runtimeSource = fs.existsSync(RUNTIME_PATH)
    ? path.relative(REPO_ROOT, RUNTIME_PATH)
    : `defaults (copy ${path.relative(REPO_ROOT, RUNTIME_EXAMPLE_PATH)} → runtime.json to customise)`;
  console.log(`Runtime: ${runtimeSource}`);
  console.log(
    `  start: ${Object.entries(runtime)
      .filter(([, on]) => on)
      .map(([k]) => k)
      .join(', ') || '(none)'}`,
  );
  const skipped = Object.entries(runtime)
    .filter(([, on]) => !on)
    .map(([k]) => k);
  if (skipped.length) {
    console.log(`  skip:  ${skipped.join(', ')}`);
  }

  runDoctor();
  stopManagedPids();

  const secretEnv = envFileToProcessEnv(API_ENV_PATH);
  const pids = {};

  if (runtime.azurite) {
    await ensureAzurite(pids);
  } else {
    skip('azurite', 'runtime.json');
  }

  if (runtime.api) {
    pids.api = spawnManaged(
      'api',
      'dotnet',
      [
        'watch',
        'run',
        '--project',
        PATHS.apiProject,
        '--no-restore',
        '--',
        '--urls',
        `http://localhost:${PORTS.api}`,
      ],
      PATHS.apiCwd,
      secretEnv,
    );
  } else {
    skip('api', 'runtime.json — set "api": true to start via npm');
  }

  if (runtime.web) {
    const webNg = localBin(PATHS.webClient, 'ng');
    if (!webNg) {
      console.error('web: ng not found under node_modules/.bin — run npm run web:install');
      process.exit(1);
    }
    pids.webNg = spawnManaged(
      'webNg',
      webNg,
      ['serve', '--port', String(PORTS.webNg), '--host', '127.0.0.1'],
      PATHS.webClient,
    );
    if (!(await waitForPort(PORTS.webNg, 'web ng serve', 180_000))) {
      console.error(`web: ng serve did not open :${PORTS.webNg} in time — not starting SWA`);
      process.exit(1);
    }
    pids.web = spawnManaged(
      'web',
      'swa',
      [
        'start',
        `http://127.0.0.1:${PORTS.webNg}`,
        '--port',
        String(PORTS.web),
        '--devserver-timeout',
        '120',
      ],
      PATHS.webClient,
    );
  } else {
    skip('web', 'runtime.json');
  }

  if (runtime.app) {
    const appNg = localBin(PATHS.appClient, 'ng');
    if (!appNg) {
      console.error('app: ng not found under node_modules/.bin — run npm run app:install');
      process.exit(1);
    }
    pids.appNg = spawnManaged(
      'appNg',
      appNg,
      ['serve', '--port', String(PORTS.appNg), '--host', '127.0.0.1'],
      PATHS.appClient,
    );
    if (!(await waitForPort(PORTS.appNg, 'app ng serve', 180_000))) {
      console.error(`app: ng serve did not open :${PORTS.appNg} in time — not starting SWA`);
      process.exit(1);
    }
    pids.app = spawnManaged(
      'app',
      'swa',
      [
        'start',
        `http://127.0.0.1:${PORTS.appNg}`,
        '--port',
        String(PORTS.app),
        '--devserver-timeout',
        '120',
      ],
      PATHS.appClient,
    );
  } else {
    skip('app', 'runtime.json');
  }

  if (runtime.imageResize) {
    // After a VS/dotnet build, obj/.../WorkerExtensions.csproj appears beside the
    // real project. `func start` in the source dir then fails with "found 2".
    // Build explicitly, then point Core Tools at the output folder (same as a
    // known-good local recipe; Azurite must already be up for blob triggers).
    console.log('[imageResize] building DertInfoImageResizeV4…');
    const build = spawnSync(
      'dotnet',
      ['build', PATHS.functionsProject, '--configuration', 'Debug', '--nologo'],
      { cwd: PATHS.functionsDir, env: nativeEnv(), stdio: 'inherit', shell: process.platform === 'win32' },
    );
    if (build.status !== 0) {
      console.error('imageResize: dotnet build failed');
      process.exit(build.status ?? 1);
    }
    if (!fs.existsSync(PATHS.functionsScriptRoot)) {
      console.error(`imageResize: missing script root ${PATHS.functionsScriptRoot}`);
      process.exit(1);
    }
    pids.imageResize = spawnManaged(
      'imageResize',
      'func',
      [
        'start',
        '--port',
        String(PORTS.imageResize),
        '--script-root',
        PATHS.functionsScriptRoot,
      ],
      PATHS.functionsDir,
    );
  } else {
    skip('imageResize', 'runtime.json');
  }

  fs.writeFileSync(PIDS_PATH, JSON.stringify(pids, null, 2) + '\n', 'utf8');

  const line = (label, port, on) =>
    `  ${label.padEnd(14)}${on ? `http://localhost:${port}` : '(skipped)'}`;

  console.log(`
Local-native estate (managed pids → ${path.relative(REPO_ROOT, PIDS_PATH)})

${line('API', PORTS.api, runtime.api)}
${line('Website', PORTS.web, runtime.web)}
${line('App (PWA)', PORTS.app, runtime.app)}
${line('Image resize', PORTS.imageResize, runtime.imageResize)}
  Azurite       ${runtime.azurite ? `http://127.0.0.1:${PORTS.azuriteBlob}` : '(skipped)'}

  npm run status
  npm run stop
`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
