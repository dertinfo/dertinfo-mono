#!/usr/bin/env node
/**
 * Probe local-native ports (informational).
 */
import http from 'node:http';
import { PORTS, portOpen } from './paths.mjs';

async function httpOk(url, timeoutMs = 2000) {
  return new Promise((resolve) => {
    const req = http.get(url, { timeout: timeoutMs }, (res) => {
      res.resume();
      resolve(res.statusCode && res.statusCode < 500);
    });
    req.on('timeout', () => {
      req.destroy();
      resolve(false);
    });
    req.on('error', () => resolve(false));
  });
}

const checks = [
  ['api', PORTS.api, `http://localhost:${PORTS.api}/swagger/index.html`],
  ['web', PORTS.web, `http://localhost:${PORTS.web}/`],
  ['app', PORTS.app, `http://localhost:${PORTS.app}/`],
  ['imageResize', PORTS.imageResize, null],
  ['azuriteBlob', PORTS.azuriteBlob, null],
];

console.log('DertInfo local-native status\n');
for (const [name, port, url] of checks) {
  const tcp = await portOpen(port);
  let httpStatus = '';
  if (tcp && url) {
    httpStatus = (await httpOk(url)) ? ' http:ok' : ' http:down';
  }
  console.log(`  ${tcp ? 'UP  ' : 'DOWN'} ${name.padEnd(12)} :${port}${httpStatus}`);
}
