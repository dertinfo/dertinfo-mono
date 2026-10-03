/**
 * Website smoke runner.
 * Order comes from scenario contracts on the capability feature pages.
 * A scenario checks prerequisites first and does not run its own steps when they are missing.
 * Each run recreates the Docker SQL volume and restarts the API before scenarios.
 */
import fs from 'fs';
import path from 'path';
import { pathToFileURL } from 'url';
import { fileURLToPath } from 'url';
import { loadE2eEnv, normalizeRef, normalizeToken } from '../helpers.mjs';

const smokeDir = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const repoRoot = path.resolve(smokeDir, '../../../..');
const featuresDir = path.join(repoRoot, 'docs', 'capabilities', 'features');
const stateDir = path.join(smokeDir, 'state');
const chainPath = path.join(stateDir, 'chain.json');
const scenariosDir = path.dirname(fileURLToPath(import.meta.url));

loadE2eEnv();

function parseSimpleYaml(source) {
  const root = {};
  const stack = [{ indent: -1, container: root, key: null }];
  for (const raw of source.split(/\r?\n/)) {
    if (!raw.trim() || raw.trim().startsWith('#')) continue;
    const indent = raw.match(/^ */)[0].length;
    const line = raw.trim();
    while (stack.length > 1 && indent <= stack[stack.length - 1].indent) stack.pop();
    const frame = stack[stack.length - 1];
    if (line.startsWith('- ')) {
      const item = line.slice(2).trim();
      if (!Array.isArray(frame.container[frame.key])) frame.container[frame.key] = [];
      const objectItem = item.match(/^([A-Za-z0-9_-]+):\s+(.*)$/);
      if (objectItem) {
        const obj = {};
        obj[objectItem[1]] = coerce(objectItem[2]);
        frame.container[frame.key].push(obj);
        stack.push({ indent, container: obj, key: objectItem[1] });
      } else {
        frame.container[frame.key].push(coerce(item));
      }
      continue;
    }
    const [key, value] = splitKey(line);
    if (value === '') {
      frame.container[key] = {};
      stack.push({ indent, container: frame.container, key });
    } else {
      frame.container[key] = coerce(value);
    }
  }
  return root;
}

function splitKey(line) {
  const index = line.indexOf(':');
  return [line.slice(0, index).trim(), line.slice(index + 1).trim()];
}

function coerce(value) {
  if (value === 'true') return true;
  if (value === 'false') return false;
  if (value === '[]') return [];
  if (/^\d+$/.test(value)) return Number(value);
  if ((value.startsWith('"') && value.endsWith('"')) || (value.startsWith("'") && value.endsWith("'"))) {
    return value.slice(1, -1);
  }
  return value;
}

function contractsFromMarkdown(markdown) {
  const rolesMatch = markdown.match(/^roles:\s*\[([^\]]*)\]/m);
  const roles = rolesMatch
    ? rolesMatch[1].split(',').map((role) => role.trim()).filter(Boolean)
    : [];
  const contracts = [];
  for (const match of markdown.matchAll(/```yaml\r?\n([\s\S]*?)```/g)) {
    const parsed = parseSimpleYaml(match[1]);
    const listed = Array.isArray(parsed.scenarios) ? parsed.scenarios : parsed.id ? [parsed] : [];
    for (const contract of listed) {
      contracts.push({
        ...contract,
        requires: contract.requires || [],
        provides: contract.provides || [],
        roles,
      });
    }
  }
  return contracts;
}

function loadContracts() {
  const contracts = [];
  for (const fileName of fs.readdirSync(featuresDir).filter((name) => name.endsWith('.md'))) {
    const markdown = fs.readFileSync(path.join(featuresDir, fileName), 'utf8');
    contracts.push(...contractsFromMarkdown(markdown));
  }
  return contracts;
}

function loadChain() {
  if (!fs.existsSync(chainPath)) return {};
  return JSON.parse(fs.readFileSync(chainPath, 'utf8'));
}

function saveChain(chain) {
  fs.mkdirSync(stateDir, { recursive: true });
  fs.writeFileSync(chainPath, `${JSON.stringify(chain, null, 2)}\n`);
}

function scenarioKey(id, instance = 1) {
  return instance === 1 ? id : `${id}[${instance}]`;
}

function sortReady(ready, byId) {
  return [...ready].sort((a, b) => (byId.get(a).sequence ?? 0) - (byId.get(b).sequence ?? 0));
}

function executionOrder(contracts) {
  const byId = new Map(contracts.map((contract) => [contract.id, contract]));
  const incoming = new Map(contracts.map((contract) => [contract.id, 0]));
  const dependents = new Map(contracts.map((contract) => [contract.id, []]));
  for (const contract of contracts) {
    for (const requirement of contract.requires) {
      const { id } = normalizeRef(requirement);
      if (!byId.has(id)) {
        throw new Error(`${contract.id} requires unknown scenario ${requirement}`);
      }
      incoming.set(contract.id, incoming.get(contract.id) + 1);
      dependents.get(id).push(contract.id);
    }
  }
  const ready = contracts.filter((contract) => incoming.get(contract.id) === 0).map((contract) => contract.id);
  const order = [];
  while (ready.length > 0) {
    const next = sortReady(ready, byId)[0];
    ready.splice(ready.indexOf(next), 1);
    order.push(byId.get(next));
    for (const dependent of dependents.get(next)) {
      incoming.set(dependent, incoming.get(dependent) - 1);
      if (incoming.get(dependent) === 0) ready.push(dependent);
    }
  }
  if (order.length !== contracts.length) {
    throw new Error('Scenario requirements contain a cycle');
  }
  return order;
}

function missingPrerequisites(contract, chain, passed) {
  const missing = [];
  for (const requirement of contract.requires) {
    const { id, instance } = normalizeRef(requirement);
    const key = scenarioKey(id, instance);
    if (!passed.has(key)) {
      missing.push(requirement);
      continue;
    }
    const provider = passed.get(key);
    for (const token of provider.provides) {
      const named = normalizeToken(token);
      const instanceToken = named.replace(/\[\d+\]$/, `[${instance}]`);
      if (!Object.prototype.hasOwnProperty.call(chain, instanceToken) && !Object.prototype.hasOwnProperty.call(chain, named)) {
        missing.push(token);
      }
    }
  }
  return missing;
}

const selected = process.argv.slice(2);
const contracts = loadContracts();
const modules = new Map();
for (const fileName of fs.readdirSync(scenariosDir).filter((name) => name.endsWith('.mjs') && name !== 'run.mjs')) {
  const imported = await import(pathToFileURL(path.join(scenariosDir, fileName)).href);
  if (!imported.id || typeof imported.run !== 'function') {
    throw new Error(`${fileName} must export id and run`);
  }
  modules.set(imported.id, imported);
}

const smokeContracts = contracts.filter((contract) => modules.has(contract.id));
for (const [id, imported] of modules) {
  const contract = contracts.find((item) => item.id === id);
  if (!contract) throw new Error(`${id} has no scenario contract on a feature page`);
  if (contract.persona && !contract.roles.includes(contract.persona)) {
    throw new Error(`${id} persona ${contract.persona} is not in the feature roles`);
  }
  modules.set(id, { ...imported, contract });
}

let planned = executionOrder(smokeContracts);
const wanted = new Set(selected);
if (selected.length > 0) {
  const included = new Set();
  function include(id) {
    if (included.has(id)) return;
    const contract = smokeContracts.find((item) => item.id === id);
    if (!contract) throw new Error(`Unknown scenario ${id}`);
    for (const requirement of contract.requires) include(normalizeRef(requirement).id);
    included.add(id);
  }
  for (const id of wanted) include(id);
  planned = planned.filter((contract) => included.has(contract.id));
}

const resetSql = await import(pathToFileURL(path.join(repoRoot, 'infra', 'dev', 'reset-sql.mjs')).href);
try {
  await resetSql.resetSmokeDatabase();
} catch (error) {
  console.error(error.message);
  process.exit(1);
}

fs.mkdirSync(stateDir, { recursive: true });
fs.rmSync(chainPath, { force: true });
let chain = loadChain();
const passed = new Map();
let failed = false;

function tokensReady(contract) {
  return contract.provides.length > 0 && contract.provides.every((token) => (
    Object.prototype.hasOwnProperty.call(chain, normalizeToken(token))
  ));
}

function covered(id, seen = new Set()) {
  if (seen.has(id)) return false;
  seen.add(id);
  const contract = smokeContracts.find((item) => item.id === id);
  if (!contract) return false;
  if (tokensReady(contract)) return true;
  const dependents = smokeContracts.filter((item) => item.requires.some((requirement) => normalizeRef(requirement).id === id));
  return dependents.length > 0 && dependents.every((dependent) => covered(dependent.id, seen));
}

if (selected.length > 0) {
  planned = planned.filter((contract) => {
    if (wanted.has(contract.id)) return true;
    if (chain[`done:${contract.id}`]) {
      passed.set(contract.id, contract);
      passed.set(scenarioKey(contract.id, 1), contract);
      return false;
    }
    if (!tokensReady(contract) && !covered(contract.id)) return true;
    passed.set(contract.id, contract);
    passed.set(scenarioKey(contract.id, 1), contract);
    return false;
  });
}

for (const contract of planned) {
  const missing = missingPrerequisites(contract, chain, passed);
  if (missing.length > 0) {
    console.error(`FAIL ${contract.id}`);
    console.error(`prerequisites not available: ${missing.join(', ')}`);
    failed = true;
    continue;
  }
  try {
    const imported = modules.get(contract.id);
    await imported.run({
      chain,
      provide(name, value) {
        chain[normalizeToken(name)] = value;
        saveChain(chain);
      },
    });
    for (const token of contract.provides) {
      const key = normalizeToken(token);
      if (!Object.prototype.hasOwnProperty.call(chain, key)) {
        chain[key] = 'ready';
        saveChain(chain);
      }
    }
    passed.set(contract.id, contract);
    passed.set(scenarioKey(contract.id, 1), contract);
    chain[`done:${contract.id}`] = true;
    saveChain(chain);
    console.log(`PASS ${contract.id}`);
  } catch (error) {
    console.error(`FAIL ${contract.id}`);
    console.error(error.message);
    failed = true;
  }
}

process.exit(failed ? 1 : 0);
