#!/usr/bin/env node
/**
 * Fail-fast preflight for local-native development.
 * Does not install packages or start SQL Express.
 */
import fs from 'node:fs';
import https from 'node:https';
import path from 'node:path';
import { spawnSync } from 'node:child_process';
import {
  API_ENV_PATH,
  API_ENV_EXAMPLE_PATH,
  MIN_AZURITE_VERSION,
  MIN_FUNC_CORE_TOOLS_MAJOR,
  PATHS,
  PORTS,
  REQUIRED_SECRET_KEYS,
  REPO_ROOT,
  commandExists,
  parseEnvFile,
  parseSemver,
  portOpen,
  semverGte,
  toolVersionOutput,
} from './paths.mjs';

const results = [];

function pass(name, detail = '') {
  results.push({ ok: true, name, detail });
  console.log(`  OK  ${name}${detail ? ` — ${detail}` : ''}`);
}

function fail(name, detail) {
  results.push({ ok: false, name, detail });
  console.log(` FAIL ${name} — ${detail}`);
}

function checkTooling() {
  console.log('\nTooling (present only; no install)');
  if (commandExists('dotnet')) {
    const v = spawnSync('dotnet', ['--version'], { encoding: 'utf8' });
    pass('dotnet', (v.stdout || '').trim());
  } else {
    fail('dotnet', 'not found on PATH');
  }

  if (commandExists('node')) {
    const v = spawnSync('node', ['--version'], { encoding: 'utf8' });
    const ver = (v.stdout || '').trim();
    const [major, minor] = ver.replace(/^v/, '').split('.').map(Number);
    if (major > 16 || (major === 16 && minor >= 10)) {
      pass('node', ver);
    } else {
      fail('node', `${ver} (need >= 16.10 — Angular 14 / monorepo floor)`);
    }
  } else {
    fail('node', 'not found on PATH');
  }

  if (commandExists('func')) {
    const funcOut = toolVersionOutput('func');
    const funcVer = parseSemver(funcOut);
    if (funcVer && funcVer.major >= MIN_FUNC_CORE_TOOLS_MAJOR) {
      pass('func', `Azure Functions Core Tools ${funcVer.raw} (v${MIN_FUNC_CORE_TOOLS_MAJOR}+)`);
    } else if (funcVer) {
      fail(
        'func',
        `${funcVer.raw} — need Core Tools v${MIN_FUNC_CORE_TOOLS_MAJOR}+ (winget upgrade Microsoft.Azure.FunctionsCoreTools)`,
      );
    } else {
      pass('func', 'Azure Functions Core Tools (version string not parsed)');
    }
  } else {
    fail('func', 'Azure Functions Core Tools not on PATH');
  }

  if (commandExists('swa')) {
    pass('swa', 'Azure Static Web Apps CLI');
  } else {
    fail(
      'swa',
      'Azure Static Web Apps CLI not on PATH — install: npm install -g @azure/static-web-apps-cli',
    );
  }

  if (commandExists('azurite')) {
    const azOut = toolVersionOutput('azurite');
    const azVer = parseSemver(azOut);
    if (azVer && semverGte(azVer, MIN_AZURITE_VERSION)) {
      pass('azurite', `${azVer.raw} (>= ${MIN_AZURITE_VERSION})`);
    } else if (azVer) {
      fail(
        'azurite',
        `${azVer.raw} is too old for Functions Core Tools 4.12+ (needs Storage API 2024-11-04). Upgrade: npm install -g azurite@latest then stop any running Azurite (npm run stop / kill :10000) before restart. Minimum ${MIN_AZURITE_VERSION}.`,
      );
    } else {
      fail('azurite', `CLI on PATH but version not parsed from: ${azOut.slice(0, 80)}`);
    }
  } else {
    fail('azurite', 'not on PATH — install: npm install -g azurite@latest');
  }

  if (!fs.existsSync(path.join(PATHS.webClient, 'node_modules'))) {
    fail('web node_modules', 'missing — run: npm run web:install (from repo root)');
  } else {
    pass('web node_modules', PATHS.webClient);
  }

  if (!fs.existsSync(path.join(PATHS.appClient, 'node_modules'))) {
    fail('app node_modules', 'missing — run: npm run app:install (from repo root)');
  } else {
    pass('app node_modules', PATHS.appClient);
  }

  if (!fs.existsSync(PATHS.functionsSettings)) {
    fail(
      'functions local.settings.json',
      `missing — copy ${path.relative(REPO_ROOT, PATHS.functionsSettingsExample)}`,
    );
  } else {
    pass('functions local.settings.json');
  }
}

function checkSecrets() {
  console.log('\nSecrets (infra/secrets/api.env)');
  if (!fs.existsSync(API_ENV_PATH)) {
    fail(
      'api.env',
      `missing — copy ${path.relative(REPO_ROOT, API_ENV_EXAMPLE_PATH)} to api.env and fill values`,
    );
    return null;
  }
  const env = parseEnvFile(API_ENV_PATH);
  let allOk = true;
  for (const key of REQUIRED_SECRET_KEYS) {
    if (!env[key] || !String(env[key]).trim()) {
      fail(key, 'empty or missing');
      allOk = false;
    } else {
      pass(key, 'set');
    }
  }
  return allOk ? env : null;
}

function checkSql(env) {
  console.log('\nSQL Server');
  if (!env) {
    fail('sql', 'skipped (secrets missing)');
    return;
  }
  const server = env.SqlConnection__ServerName;
  const user = env.SqlConnection__ServerAdminName;
  const db = env.SqlConnection__DatabaseName;
  const password = env.SqlConnection__ServerAdminPassword;

  if (!commandExists('sqlcmd')) {
    fail(
      'sqlcmd',
      'not on PATH — install SQL Server Command Line Utilities to verify the database',
    );
    return;
  }

  const args = [
    '-S',
    server,
    '-U',
    user,
    '-P',
    password,
    '-d',
    db,
    '-C',
    '-Q',
    "SET NOCOUNT ON; IF OBJECT_ID('dbo.__EFMigrationsHistory') IS NULL SELECT 'NO_HISTORY' ELSE SELECT CAST(COUNT(*) AS varchar(20)) FROM dbo.__EFMigrationsHistory;",
    '-h',
    '-1',
    '-W',
  ];
  const r = spawnSync('sqlcmd', args, { encoding: 'utf8' });
  const out = `${r.stdout || ''}${r.stderr || ''}`.trim();
  if (r.status !== 0) {
    fail(
      'sql connect',
      `cannot reach ${server} / ${db} as ${user}. Ensure SQL is running and the database exists (create an empty database if needed). ${out.slice(0, 200)}`,
    );
    return;
  }
  pass('sql connect', `${server} / ${db}`);
  if (out.includes('NO_HISTORY')) {
    pass(
      'sql schema',
      `__EFMigrationsHistory missing — API will apply migrations on startup`,
    );
    return;
  }
  const count = Number((out.match(/\d+/) || [])[0]);
  if (!Number.isFinite(count) || count < 1) {
    pass(
      'sql schema',
      `no migrations recorded yet — API will apply migrations on startup`,
    );
    return;
  }
  pass('sql schema', `${count} migration(s) in __EFMigrationsHistory`);
}

async function checkAzurite() {
  console.log('\nAzurite');
  const blob = await portOpen(PORTS.azuriteBlob);
  const queue = await portOpen(PORTS.azuriteQueue);
  const table = await portOpen(PORTS.azuriteTable);
  if (blob && queue && table) {
    pass('azurite ports', `${PORTS.azuriteBlob}/${PORTS.azuriteQueue}/${PORTS.azuriteTable}`);
    return;
  }
  if (commandExists('azurite')) {
    pass(
      'azurite ports',
      'not listening yet — npm run start will launch Azurite (CLI available)',
    );
  } else {
    fail(
      'azurite ports',
      `ports ${PORTS.azuriteBlob}-${PORTS.azuriteTable} closed and azurite CLI missing`,
    );
  }
}

function checkAuth0(env) {
  console.log('\nAuth0');
  if (!env?.Auth0__Domain) {
    fail('Auth0 OIDC', 'skipped (Auth0__Domain missing)');
    return Promise.resolve();
  }
  const domain = String(env.Auth0__Domain).replace(/^https?:\/\//, '').replace(/\/$/, '');
  const url = `https://${domain}/.well-known/openid-configuration`;
  return new Promise((resolve) => {
    const req = https.get(url, { timeout: 10000 }, (res) => {
      res.resume();
      if (res.statusCode && res.statusCode >= 200 && res.statusCode < 300) {
        pass('Auth0 OIDC', `${domain} reachable`);
      } else {
        fail('Auth0 OIDC', `HTTP ${res.statusCode} for ${domain}`);
      }
      resolve();
    });
    req.on('timeout', () => {
      req.destroy();
      fail('Auth0 OIDC', `timeout contacting ${domain}`);
      resolve();
    });
    req.on('error', (err) => {
      fail('Auth0 OIDC', err.message);
      resolve();
    });
  });
}

async function main() {
  console.log('DertInfo local-native doctor');
  console.log(`Repo: ${REPO_ROOT}`);
  checkTooling();
  const env = checkSecrets();
  checkSql(env);
  await checkAzurite();
  await checkAuth0(env);

  const failed = results.filter((r) => !r.ok);
  console.log('');
  if (failed.length) {
    console.log(`Doctor failed (${failed.length} check(s)).`);
    process.exit(1);
  }
  console.log('Doctor passed.');
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
