#!/usr/bin/env node
/**
 * Free port 44000, delete the SQL data volume that was mounted there,
 * start the Compose SQL Server, then restart the API.
 * The API applies EF migrations when it starts, so the database is empty.
 * Smoke runs call this before scenarios. The website and Auth0 sessions stay up.
 */
import crossSpawn from 'cross-spawn';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { pathToFileURL } from 'node:url';
import {
  assertDockerAvailable,
  composeUp,
  dockerBridgeEnv,
} from './docker.mjs';
import { isApiHealthy, isSqlHealthy } from './health.mjs';
import {
  API_ENV_PATH,
  COMPOSE_SERVICES,
  DEV_DIR,
  PATHS,
  PIDS_PATH,
  PORTS,
  REPO_ROOT,
  ensureDir,
  envFileToProcessEnv,
  loadRuntime,
  portOpen,
} from './paths.mjs';

const spawnSync = crossSpawn.sync;
const spawn = crossSpawn;
const SQL_SERVICE = COMPOSE_SERVICES.sql;
const API_SERVICE = COMPOSE_SERVICES.api;

function sameDir(left, right) {
  if (!left || !right) return false;
  const normal = (value) => path.resolve(value).replace(/\\/g, '/').toLowerCase();
  return normal(left) === normal(right);
}

function docker(args, { allowFail = false } = {}) {
  const result = spawnSync('docker', args, {
    cwd: REPO_ROOT,
    encoding: 'utf8',
    stdio: allowFail ? 'pipe' : 'inherit',
  });
  if (!allowFail && result.status !== 0) {
    throw new Error(`docker ${args.join(' ')} failed (exit ${result.status})`);
  }
  return result;
}

function readPids() {
  if (!fs.existsSync(PIDS_PATH)) return { native: {}, docker: [] };
  try {
    const raw = JSON.parse(fs.readFileSync(PIDS_PATH, 'utf8'));
    if (raw.native || raw.docker) {
      return { native: raw.native || {}, docker: raw.docker || [] };
    }
    return { native: raw, docker: [] };
  } catch {
    return { native: {}, docker: [] };
  }
}

function writePids(state) {
  fs.writeFileSync(PIDS_PATH, `${JSON.stringify(state, null, 2)}\n`, 'utf8');
}

function stopPid(pid) {
  if (!pid) return;
  if (process.platform === 'win32') {
    spawnSync('taskkill', ['/PID', String(pid), '/T', '/F'], { stdio: 'ignore' });
    return;
  }
  try {
    process.kill(pid, 'SIGTERM');
  } catch {
    // already gone
  }
}

function listeningPids(port) {
  if (process.platform === 'win32') {
    const result = spawnSync('netstat', ['-ano', '-p', 'tcp'], { encoding: 'utf8' });
    const pids = new Set();
    const marker = new RegExp(`:${port}\\s`);
    for (const line of (result.stdout || '').split(/\r?\n/)) {
      if (!marker.test(line) || !line.includes('LISTENING')) continue;
      const pid = line.trim().split(/\s+/).at(-1);
      if (pid && pid !== '0') pids.add(pid);
    }
    return [...pids];
  }
  const result = spawnSync('lsof', ['-ti', `tcp:${port}`, '-sTCP:LISTEN'], { encoding: 'utf8' });
  return (result.stdout || '').split(/\s+/).filter(Boolean);
}

async function waitFor(label, probe, timeoutMs) {
  const started = Date.now();
  console.log(`Waiting for ${label} (up to ${Math.round(timeoutMs / 1000)}s)…`);
  while (Date.now() - started < timeoutMs) {
    if (await probe()) {
      console.log(`${label} after ${Date.now() - started}ms`);
      return;
    }
    await new Promise((resolve) => setTimeout(resolve, 2000));
  }
  throw new Error(`${label} did not succeed within ${Math.round(timeoutMs / 1000)}s`);
}

function containersPublishing(port) {
  const listed = docker(['ps', '-a', '--format', '{{.ID}}\t{{.Names}}\t{{.Ports}}'], { allowFail: true });
  const marker = new RegExp(`:${port}->`);
  const found = [];
  for (const line of (listed.stdout || '').split(/\r?\n/)) {
    if (!line.trim()) continue;
    const [id, name, ports = ''] = line.split('\t');
    if (id && marker.test(ports)) found.push({ id, name });
  }
  return found;
}

function mssqlVolumeNames(containerId) {
  const inspect = docker(
    ['inspect', '--format', '{{range .Mounts}}{{.Type}}|{{.Name}}|{{.Destination}}\n{{end}}', containerId],
    { allowFail: true },
  );
  const names = [];
  for (const line of (inspect.stdout || '').split(/\r?\n/)) {
    const [type, name, destination] = line.split('|');
    if (type === 'volume' && destination === '/var/opt/mssql' && name) names.push(name);
  }
  return names;
}

function sqlVolumeNames() {
  const listed = docker(
    ['volume', 'ls', '-q', '--filter', 'label=com.docker.compose.volume=sqlserver-data'],
    { allowFail: true },
  );
  const names = (listed.stdout || '').split(/\r?\n/).map((line) => line.trim()).filter(Boolean);
  const matched = [];
  for (const name of names) {
    const inspect = docker(
      ['volume', 'inspect', '--format', '{{ index .Labels "com.docker.compose.project.working_dir" }}', name],
      { allowFail: true },
    );
    if (sameDir((inspect.stdout || '').trim(), REPO_ROOT)) matched.push(name);
  }
  const fallback = `${path.basename(REPO_ROOT).toLowerCase()}_sqlserver-data`;
  const all = docker(['volume', 'ls', '-q'], { allowFail: true });
  const known = (all.stdout || '').split(/\r?\n/).map((line) => line.trim());
  if (known.includes(fallback) && !matched.includes(fallback)) matched.push(fallback);
  return matched;
}

function removeSqlContainer() {
  console.log(`[docker] docker compose rm -f -s ${SQL_SERVICE}`);
  docker(['compose', 'rm', '-f', '-s', SQL_SERVICE], { allowFail: true });
}

async function removeVolume(name) {
  for (let attempt = 1; attempt <= 10; attempt += 1) {
    console.log(`[docker] docker volume rm ${name}`);
    const result = docker(['volume', 'rm', name], { allowFail: true });
    if (result.status === 0) return;
    const detail = `${result.stderr || ''}${result.stdout || ''}`.trim();
    if (attempt === 10) {
      throw new Error(`docker volume rm ${name} failed: ${detail}`);
    }
    console.log(`Volume still in use, retrying (${detail})`);
    await new Promise((resolve) => setTimeout(resolve, 1000));
  }
}

async function recreateSqlVolume() {
  const holders = containersPublishing(PORTS.sql);
  const volumes = new Set(sqlVolumeNames());
  for (const holder of holders) {
    console.log(`SQL on :${PORTS.sql} is ${holder.name} (${holder.id}).`);
    for (const name of mssqlVolumeNames(holder.id)) volumes.add(name);
  }
  removeSqlContainer();
  for (const holder of holders) {
    console.log(`[docker] docker rm -f ${holder.name}`);
    docker(['rm', '-f', holder.id], { allowFail: true });
  }
  docker(['rm', '-f', SQL_SERVICE], { allowFail: true });
  await waitFor(`port ${PORTS.sql} to close`, async () => !(await portOpen(PORTS.sql)), 30_000);
  if (volumes.size === 0) {
    console.log('No existing SQL data volume — a new empty volume will be created.');
    return;
  }
  for (const name of volumes) {
    await removeVolume(name);
  }
}

function rememberDockerService(state, service) {
  if (!state.docker.includes(service)) state.docker.push(service);
}

async function restartNativeApi(state, runtime) {
  const tracked = state.native.api;
  if (tracked) {
    console.log(`Stopping native api (pid ${tracked})`);
    stopPid(tracked);
    delete state.native.api;
  }
  for (const pid of listeningPids(PORTS.api)) {
    console.log(`Stopping process listening on :${PORTS.api} (pid ${pid})`);
    stopPid(pid);
  }
  await waitFor('API port to close', async () => !(await portOpen(PORTS.api)), 30_000);

  const cfg = runtime.api;
  const secretEnv = envFileToProcessEnv(API_ENV_PATH);
  const nugetPackages = path.join(os.homedir(), '.nuget', 'packages');
  const args = cfg.hotReload
    ? ['watch', 'run', '--project', PATHS.apiProject, '--no-restore', '--', '--urls', `http://localhost:${PORTS.api}`]
    : ['run', '--project', PATHS.apiProject, '--no-restore', '--no-build', '--', '--urls', `http://localhost:${PORTS.api}`];

  const logDir = path.join(DEV_DIR, 'logs');
  ensureDir(logDir);
  const outFd = fs.openSync(path.join(logDir, 'api.log'), 'a');
  const errFd = fs.openSync(path.join(logDir, 'api.err.log'), 'a');
  console.log(`[api] dotnet ${args.join(' ')}`);
  const child = spawn('dotnet', args, {
    cwd: PATHS.apiCwd,
    env: { ...process.env, NUGET_PACKAGES: nugetPackages, ...secretEnv },
    detached: true,
    stdio: ['ignore', outFd, errFd],
    windowsHide: true,
  });
  child.unref();
  state.native.api = child.pid;
  writePids(state);
}

async function restartDockerApi(state, runtime) {
  console.log(`[docker] docker compose stop ${API_SERVICE}`);
  docker(['compose', 'stop', API_SERVICE], { allowFail: true });
  composeUp(API_SERVICE, {
    env: dockerBridgeEnv(runtime),
    noDeps: true,
    forceRecreate: true,
  });
  rememberDockerService(state, API_SERVICE);
  writePids(state);
}

/** Stop SQL, delete its Compose volume, start a new empty server, then restart the API. */
export async function resetSmokeDatabase() {
  const runtime = loadRuntime();
  if (runtime.sql?.mode !== 'docker') {
    throw new Error(
      'Smoke tests need sql.mode "docker" in infra/dev/runtime.json so the sqlserver-data volume can be recreated.',
    );
  }
  if (runtime.api?.mode !== 'native' && runtime.api?.mode !== 'docker') {
    throw new Error(
      'Smoke tests restart the API after the database is recreated so migrations run. Set api.mode to native or docker.',
    );
  }

  assertDockerAvailable();
  console.log('Recreating the Docker SQL volume for a clean smoke database…');
  await recreateSqlVolume();
  composeUp(SQL_SERVICE, { noDeps: true, forceRecreate: true });
  const state = readPids();
  rememberDockerService(state, SQL_SERVICE);
  writePids(state);
  await waitFor('SQL', () => isSqlHealthy('docker'), 180_000);

  console.log('Restarting the API so it migrates the empty database…');
  if (runtime.api.mode === 'docker') {
    await restartDockerApi(state, runtime);
  } else {
    await restartNativeApi(state, runtime);
  }
  await waitFor('API', () => isApiHealthy(), 300_000);
  console.log('Clean database is ready.');
}

const invokedDirectly = process.argv[1] && import.meta.url === pathToFileURL(path.resolve(process.argv[1])).href;
if (invokedDirectly) {
  resetSmokeDatabase().catch((error) => {
    console.error(error.message);
    process.exit(1);
  });
}
