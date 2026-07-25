#!/usr/bin/env node
/**
 * Stop managed native processes and docker services started by npm run start.
 */
import fs from 'node:fs';
import { spawnSync } from 'node:child_process';
import { PIDS_PATH, REPO_ROOT } from './paths.mjs';

function stopPid(pid, name) {
  try {
    if (process.platform === 'win32') {
      spawnSync('taskkill', ['/PID', String(pid), '/T', '/F'], { stdio: 'ignore' });
    } else {
      process.kill(pid, 'SIGTERM');
    }
    console.log(`Stopped ${name} (pid ${pid})`);
  } catch {
    console.log(`Already gone: ${name} (pid ${pid})`);
  }
}

function stopCompose(service) {
  console.log(`Stopping docker ${service}`);
  spawnSync('docker', ['compose', 'stop', service], {
    cwd: REPO_ROOT,
    stdio: 'inherit',
    shell: process.platform === 'win32',
  });
}

if (!fs.existsSync(PIDS_PATH)) {
  console.log('No managed pid file — nothing to stop.');
  process.exit(0);
}

let raw;
try {
  raw = JSON.parse(fs.readFileSync(PIDS_PATH, 'utf8'));
} catch {
  console.log('Could not read pid file.');
  process.exit(1);
}

const native = raw.native || (raw.docker ? {} : raw);
const docker = raw.docker || [];

for (const [name, pid] of Object.entries(native)) {
  if (name === 'docker' || Array.isArray(pid)) continue;
  stopPid(pid, name);
}

for (const service of docker) {
  stopCompose(service);
}

fs.unlinkSync(PIDS_PATH);
console.log('Stop complete.');
