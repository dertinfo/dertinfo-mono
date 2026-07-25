#!/usr/bin/env node
/**
 * Stop managed native processes started by npm run start.
 */
import fs from 'node:fs';
import { spawnSync } from 'node:child_process';
import { PIDS_PATH } from './paths.mjs';

function stopManagedPids() {
  if (!fs.existsSync(PIDS_PATH)) {
    console.log('No managed pid file — nothing to stop.');
    return;
  }
  let pids;
  try {
    pids = JSON.parse(fs.readFileSync(PIDS_PATH, 'utf8'));
  } catch {
    console.log('Could not read pid file.');
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
      console.log(`Already gone: ${name} (pid ${pid})`);
    }
  }
  fs.unlinkSync(PIDS_PATH);
}

stopManagedPids();
