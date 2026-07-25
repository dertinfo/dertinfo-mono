/**
 * Docker Compose helpers for hybrid start/stop.
 */
import { spawnSync } from 'node:child_process';
import {
  COMPOSE_SERVICES,
  REPO_ROOT,
  API_ENV_PATH,
  commandExists,
  parseEnvFile,
} from './paths.mjs';

export function assertDockerAvailable() {
  if (!commandExists('docker')) {
    throw new Error('docker not on PATH — required when any service has mode "docker"');
  }
  const r = spawnSync('docker', ['compose', 'version'], {
    encoding: 'utf8',
    shell: process.platform === 'win32',
  });
  if (r.status !== 0) {
    throw new Error('docker compose not available — install Docker Compose V2');
  }
}

/**
 * @param {string} composeService
 * @param {{ env?: Record<string, string>, noDeps?: boolean, forceRecreate?: boolean }} opts
 */
export function composeUp(composeService, opts = {}) {
  const { env = {}, noDeps = true, forceRecreate = false } = opts;
  const args = ['compose', 'up', '-d'];
  if (noDeps) args.push('--no-deps');
  if (forceRecreate) args.push('--force-recreate');
  args.push(composeService);

  console.log(`[docker] docker ${args.join(' ')}`);
  const r = spawnSync('docker', args, {
    cwd: REPO_ROOT,
    env: { ...process.env, ...env },
    stdio: 'inherit',
    shell: process.platform === 'win32',
  });
  if (r.status !== 0) {
    throw new Error(`docker compose up failed for ${composeService} (exit ${r.status})`);
  }
}

export function composeBuild(composeService) {
  console.log(`[docker] docker compose build ${composeService}`);
  const r = spawnSync('docker', ['compose', 'build', composeService], {
    cwd: REPO_ROOT,
    stdio: 'inherit',
    shell: process.platform === 'win32',
  });
  if (r.status !== 0) {
    throw new Error(`docker compose build failed for ${composeService} (exit ${r.status})`);
  }
}

export function composeStop(composeService) {
  console.log(`[docker] docker compose stop ${composeService}`);
  spawnSync('docker', ['compose', 'stop', composeService], {
    cwd: REPO_ROOT,
    stdio: 'inherit',
    shell: process.platform === 'win32',
  });
}

export function composeServiceName(logicalName) {
  const name = COMPOSE_SERVICES[logicalName];
  if (!name) throw new Error(`No compose service for ${logicalName}`);
  return name;
}

/** Env substitutions for hybrid docker API / functions (see docker-compose.yml). */
export function dockerBridgeEnv(runtime) {
  const env = {};
  const sqlMode = runtime.sql?.mode;
  const azMode = runtime.azurite?.mode;
  const secrets = parseEnvFile(API_ENV_PATH);

  if (sqlMode === 'native' || sqlMode === 'off') {
    const server = String(secrets.SqlConnection__ServerName || '');
    // Linux containers cannot use named pipes / local named instances (.\SQLEXPRESS).
    // Use host.docker.internal TCP — SQL Express must listen on a fixed TCP port (often 1433).
    if (!server || /^(\.|localhost|127\.0\.0\.1)/i.test(server) || server.includes('\\')) {
      env.SQL_SERVER_HOST = 'host.docker.internal,1433';
    } else {
      env.SQL_SERVER_HOST = server
        .replace(/^localhost/i, 'host.docker.internal')
        .replace(/^127\.0\.0\.1/i, 'host.docker.internal');
    }
    if (secrets.SqlConnection__ServerAdminPassword) {
      env.SQL_SA_PASSWORD = secrets.SqlConnection__ServerAdminPassword;
    }
    if (secrets.SqlConnection__ServerAdminName) {
      env.SQL_SA_USER = secrets.SqlConnection__ServerAdminName;
    }
    if (secrets.SqlConnection__DatabaseName) {
      env.SQL_DATABASE = secrets.SqlConnection__DatabaseName;
    }
  } else {
    env.SQL_SERVER_HOST = 'sqlserver';
  }

  if (azMode === 'native' || azMode === 'off') {
    env.AZURITE_BLOB_ENDPOINT = 'http://host.docker.internal:10000/devstoreaccount1';
    env.AZURITE_QUEUE_ENDPOINT = 'http://host.docker.internal:10001/devstoreaccount1';
  } else {
    env.AZURITE_BLOB_ENDPOINT = 'http://azstorage:10000/devstoreaccount1';
    env.AZURITE_QUEUE_ENDPOINT = 'http://azstorage:10001/devstoreaccount1';
  }
  return env;
}
