#!/usr/bin/env node
/**
 * Probe local estate ports / health (informational).
 */
import { PORTS, loadRuntime, portOpen } from './paths.mjs';
import {
  isApiHealthy,
  isAppHealthy,
  isAzuriteHealthy,
  isImageResizeHealthy,
  isSqlHealthy,
  isWebHealthy,
} from './health.mjs';

const runtime = loadRuntime();

const rows = [
  ['api', PORTS.api, () => isApiHealthy()],
  ['web', PORTS.web, () => isWebHealthy()],
  ['app', PORTS.app, () => isAppHealthy()],
  ['imageResize', PORTS.imageResize, () => isImageResizeHealthy()],
  ['azurite', PORTS.azuriteBlob, () => isAzuriteHealthy()],
  ['sql', PORTS.sql, () => isSqlHealthy(runtime.sql?.mode ?? 'native')],
];

console.log('DertInfo local estate status\n');
for (const [name, port, check] of rows) {
  const cfg = runtime[name];
  const mode = cfg?.mode ?? '?';
  const tcp = await portOpen(port);
  const healthy = await check();
  const flag = healthy ? 'UP  ' : tcp ? 'WEAK' : 'DOWN';
  console.log(
    `  ${flag} ${name.padEnd(12)} :${String(port).padEnd(5)} mode=${mode}${healthy ? ' healthy' : ''}`,
  );
}
