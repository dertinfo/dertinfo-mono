/**
 * Runs all scripts in this folder in the recommended investigation order.
 * Run: npm run test:login:warmup  (from tests/e2e)
 */
import { spawn } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const scripts = [
  'baseline-smoke.mjs',
  'mid-warmup-race.mjs',
  'delay-success-control.mjs',
  'refresh-trap.mjs',
  'url-history.mjs',
  'resolver-hang-vs-auth.mjs',
];

for (const script of scripts) {
  const scriptPath = path.join(__dirname, script);
  console.log('\n' + '='.repeat(72));
  console.log(`Running ${script}`);
  console.log('='.repeat(72));

  await new Promise((resolve, reject) => {
    const child = spawn(process.execPath, [scriptPath], {
      stdio: 'inherit',
      cwd: path.join(__dirname, '../../..'),
    });
    child.on('close', code => (code === 0 ? resolve() : reject(new Error(`${script} exited ${code}`))));
  });
}

console.log('\nAll warmup login tests completed.');
