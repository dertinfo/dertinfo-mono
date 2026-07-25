/**
 * Shared health probes for native and docker-managed services.
 */
import http from 'node:http';
import { spawnSync } from 'node:child_process';
import {
  API_ENV_PATH,
  PORTS,
  commandExists,
  parseEnvFile,
  portOpen,
} from './paths.mjs';

export function httpOk(url, timeoutMs = 2000) {
  return new Promise((resolve) => {
    const req = http.get(url, { timeout: timeoutMs }, (res) => {
      res.resume();
      resolve(Boolean(res.statusCode && res.statusCode < 500));
    });
    req.on('timeout', () => {
      req.destroy();
      resolve(false);
    });
    req.on('error', () => resolve(false));
  });
}

/** True if Azurite accepts x-ms-version 2024-11-04 without "not supported". */
export function probeAzuriteApiVersion() {
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

export async function isAzuriteHealthy() {
  const up =
    (await portOpen(PORTS.azuriteBlob)) &&
    (await portOpen(PORTS.azuriteQueue)) &&
    (await portOpen(PORTS.azuriteTable));
  if (!up) return false;
  return probeAzuriteApiVersion();
}

export async function isApiHealthy() {
  if (!(await portOpen(PORTS.api))) return false;
  return httpOk(`http://localhost:${PORTS.api}/swagger/index.html`);
}

export async function isWebHealthy() {
  if (!(await portOpen(PORTS.web))) return false;
  return httpOk(`http://localhost:${PORTS.web}/`);
}

export async function isAppHealthy() {
  if (!(await portOpen(PORTS.app))) return false;
  return httpOk(`http://localhost:${PORTS.app}/`);
}

export async function isImageResizeHealthy() {
  return portOpen(PORTS.imageResize);
}

/** Docker SQL published on :44000, or native SQL Express via sqlcmd + api.env. */
export async function isSqlHealthy(mode = 'docker') {
  if (mode === 'docker') {
    if (!(await portOpen(PORTS.sql))) return false;
    if (!commandExists('sqlcmd')) return true;
    const r = spawnSync(
      'sqlcmd',
      [
        '-S',
        `127.0.0.1,${PORTS.sql}`,
        '-U',
        'sa',
        '-P',
        'myUn53cur3Pa55w0rd!',
        '-C',
        '-Q',
        'SELECT 1',
        '-h',
        '-1',
        '-W',
      ],
      { encoding: 'utf8' },
    );
    return r.status === 0;
  }

  if (mode === 'off') {
    if (await portOpen(PORTS.sql)) {
      return isSqlHealthy('docker');
    }
  }

  // native (or off fallback): sqlcmd against api.env
  if (!commandExists('sqlcmd')) return false;
  const env = parseEnvFile(API_ENV_PATH);
  const server = env.SqlConnection__ServerName;
  const user = env.SqlConnection__ServerAdminName;
  const db = env.SqlConnection__DatabaseName;
  const password = env.SqlConnection__ServerAdminPassword;
  if (!server || !user || !db || !password) return false;
  const r = spawnSync(
    'sqlcmd',
    ['-S', server, '-U', user, '-P', password, '-d', db, '-C', '-Q', 'SELECT 1', '-h', '-1', '-W'],
    { encoding: 'utf8' },
  );
  return r.status === 0;
}

export async function isServiceHealthy(name, runtimeCfg) {
  switch (name) {
    case 'api':
      return isApiHealthy();
    case 'web':
      return isWebHealthy();
    case 'app':
      return isAppHealthy();
    case 'imageResize':
      return isImageResizeHealthy();
    case 'azurite':
      return isAzuriteHealthy();
    case 'sql':
      return isSqlHealthy(runtimeCfg?.mode ?? 'native');
    default:
      return false;
  }
}

/** Port occupied but service not healthy — conflict for start with rebuild:false. */
export async function portConflict(name) {
  const portMap = {
    api: PORTS.api,
    web: PORTS.web,
    app: PORTS.app,
    imageResize: PORTS.imageResize,
    azurite: PORTS.azuriteBlob,
    sql: PORTS.sql,
  };
  const port = portMap[name];
  if (port == null) return false;
  if (name === 'azurite') {
    const listening =
      (await portOpen(PORTS.azuriteBlob)) ||
      (await portOpen(PORTS.azuriteQueue)) ||
      (await portOpen(PORTS.azuriteTable));
    if (!listening) return false;
    return !(await isAzuriteHealthy());
  }
  if (name === 'sql') {
    // Native SQL does not use PORTS.sql; only docker does.
    return false;
  }
  if (!(await portOpen(port))) return false;
  return !(await isServiceHealthy(name));
}
