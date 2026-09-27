/**
 * Shared checks for the website smoke suite.
 * The site must already be running. Credentials stay in the process environment.
 */
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import { chromium } from 'playwright';

export const WEB_BASE = 'http://localhost:44200';
export const API_BASE = 'http://localhost:44100/api';
export const VIEWPORT = { width: 1400, height: 900 };

const smokeDir = path.dirname(fileURLToPath(import.meta.url));

export const FIXTURE_IMAGE = path.join(smokeDir, 'fixtures', 'smoke-upload.jpg');

export const EXIT = {
  ok: 0,
  fail: 1,
  challenged: 2,
  blocked: 3,
  noGroup: 4,
};

const e2eRoot = path.resolve(smokeDir, '../..');

/** Load tests/e2e/.env into the process environment. Existing variables win. */
export function loadE2eEnv() {
  const envPath = path.join(e2eRoot, '.env');
  if (!fs.existsSync(envPath)) return;
  for (const line of fs.readFileSync(envPath, 'utf8').split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith('#')) continue;
    const index = trimmed.indexOf('=');
    if (index < 1) continue;
    const key = trimmed.slice(0, index).trim();
    const value = trimmed.slice(index + 1).trim();
    if (process.env[key] == null || process.env[key] === '') process.env[key] = value;
  }
}

/** `createdEventId` is instance 1. `createdEventId[2]` stays as written. */
export function normalizeToken(name) {
  return /\[\d+\]$/.test(name) ? name : `${name}[1]`;
}

/** A requirement with no brackets is instance 1 of that scenario. */
export function normalizeRef(requirement) {
  const match = String(requirement).match(/^(.*)\[(\d+)\]$/);
  if (!match) return { id: requirement, instance: 1 };
  return { id: match[1], instance: Number(match[2]) };
}

export function isSessionError(url, bodyText) {
  const pathName = new URL(url).pathname;
  if (['/session/404', '/session/error', '/session/403', '/session/401'].includes(pathName)) {
    return true;
  }
  const text = bodyText || '';
  if (text.includes('Server Error!') || text.includes('Forbidden!')) {
    return true;
  }
  if (/^404\b/m.test(text) && text.includes('Not Found')) {
    return true;
  }
  return false;
}

export async function bodyText(page) {
  return page.locator('body').innerText();
}

/**
 * @param {import('playwright').Page} page
 * @param {string} routePath
 * @param {{ text: string, timeoutMs?: number }} expect
 */
export async function openContent(page, routePath, expect) {
  const errors = [];
  const onError = (error) => errors.push(error.message);
  page.on('pageerror', onError);
  await page.goto(`${WEB_BASE}${routePath}`, { waitUntil: 'domcontentloaded', timeout: 60000 });
  const acceptCookies = page.getByRole('button', { name: 'Accept Cookies' });
  if (await acceptCookies.isVisible().catch(() => false)) {
    await acceptCookies.click();
  }
  const timeoutMs = expect.timeoutMs ?? 30000;
  const deadline = Date.now() + timeoutMs;
  let text = '';
  let url = page.url();
  while (Date.now() < deadline) {
    url = page.url();
    text = await bodyText(page);
    const warming = text.includes('Warming up');
    const ready = text.includes(expect.text) && !warming && !isSessionError(url, text);
    if (ready) {
      break;
    }
    if (!warming && isSessionError(url, text)) {
      break;
    }
    await page.waitForTimeout(500);
  }
  page.off('pageerror', onError);
  url = page.url();
  text = await bodyText(page);
  if (isSessionError(url, text)) {
    throw new Error(`${routePath} rendered the session error page (${url})`);
  }
  if (!text.includes(expect.text)) {
    throw new Error(`${routePath} did not show "${expect.text}"`);
  }
  if (!text.trim()) {
    throw new Error(`${routePath} rendered an empty body`);
  }
  return errors;
}

export function sessionFile(persona) {
  return path.join(smokeDir, 'state', 'sessions', `${persona}.json`);
}

/** Email and password for a persona, from tests/e2e/.env. */
export function personaCredentials(persona) {
  const envName = persona.toUpperCase().replace(/-/g, '_');
  const email = process.env[`DERTINFO_E2E_${envName}_EMAIL`];
  const password = process.env[`DERTINFO_E2E_${envName}_PASSWORD`];
  if (!email || !password) {
    throw new Error(`${persona} email and password must be set in tests/e2e/.env`);
  }
  return { email, password };
}

async function loginWall(page) {
  const text = (await bodyText(page).catch(() => '')).toLowerCase();
  const captchaFrame = await page.locator('iframe[src*="recaptcha"], iframe[src*="hcaptcha"], iframe[title*="captcha" i]').count();
  if (
    captchaFrame > 0
    || text.includes('captcha')
    || text.includes("i'm not a robot")
    || text.includes('verify you are human')
  ) {
    return 'captcha';
  }
  if (
    text.includes('access denied')
    || text.includes('blocked')
    || text.includes('unusual activity')
    || text.includes('something went wrong')
  ) {
    return 'blocked';
  }
  return null;
}

async function completeUniversalLogin(page, email, password, headed) {
  await page.waitForURL((url) => !url.href.startsWith(WEB_BASE) || url.pathname.includes('/auth/signin'), { timeout: 30000 });
  const leftApp = Date.now() + 20000;
  while (Date.now() < leftApp && page.url().startsWith(WEB_BASE)) {
    await page.waitForTimeout(250);
  }
  if (page.url().startsWith(WEB_BASE)) {
    throw Object.assign(new Error('Sign-in did not leave the app for Auth0'), { code: EXIT.challenged });
  }
  const wall = await loginWall(page);
  if (wall) throw Object.assign(new Error(`Auth0 ${wall} the automated login`), { code: EXIT.blocked });

  const username = page.locator('input[name="username"], input[name="email"], input#username').first();
  await username.waitFor({ state: 'visible', timeout: 20000 });
  await username.fill(email);

  let passwordBox = page.locator('input[name="password"], input#password').first();
  if (!(await passwordBox.isVisible().catch(() => false))) {
    await page.locator('button[type="submit"], button[data-action-button-primary="true"], button[name="action"]').first().click();
    passwordBox = page.locator('input[name="password"], input#password').first();
    try {
      await passwordBox.waitFor({ state: 'visible', timeout: 15000 });
    } catch {
      const after = await loginWall(page);
      if (after) throw Object.assign(new Error(`Auth0 ${after} the automated login`), { code: EXIT.blocked });
      throw Object.assign(new Error('Auth0 did not show the password field'), { code: headed ? EXIT.blocked : EXIT.challenged });
    }
  }
  await passwordBox.fill(password);
  await page.locator('button[type="submit"], button[data-action-button-primary="true"], button[name="action"]').first().click();

  const consent = page.getByRole('button', { name: /accept|allow/i });
  const backHome = Date.now() + 90000;
  while (Date.now() < backHome) {
    const current = page.url();
    if (current.startsWith(`${WEB_BASE}/dashboard`)) return;
    const wallNow = await loginWall(page);
    if (wallNow && !current.startsWith(WEB_BASE)) {
      throw Object.assign(new Error(`Auth0 ${wallNow} the automated login`), { code: EXIT.blocked });
    }
    if (await consent.isVisible().catch(() => false)) await consent.click();
    await page.waitForTimeout(500);
  }
  if (!headed && !page.url().startsWith(WEB_BASE)) {
    throw Object.assign(new Error('Headless login did not return to the app'), { code: EXIT.challenged });
  }
  throw new Error(`Login did not reach /dashboard (${page.url()})`);
}

async function waitForDashboard(page) {
  const deadline = Date.now() + 60000;
  while (Date.now() < deadline) {
    const url = page.url();
    const text = await bodyText(page);
    if (url.startsWith(`${WEB_BASE}/dashboard`) && !text.includes('Warming up') && !isSessionError(url, text)) {
      return;
    }
    await page.waitForTimeout(500);
  }
  const url = page.url();
  const text = await bodyText(page);
  if (!url.startsWith(`${WEB_BASE}/dashboard`) || text.includes('Warming up') || isSessionError(url, text)) {
    throw new Error(`Dashboard was not the signed-in dashboard (${url})`);
  }
}

async function acceptGdpr(page) {
  const button = page.getByRole('button', { name: 'Accept Terms & Provide Consent' });
  if (await button.isVisible().catch(() => false)) await button.click();
}

async function dashboardLoads(storageState) {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ storageState, viewport: VIEWPORT });
  const page = await context.newPage();
  try {
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await waitForDashboard(page);
    return true;
  } catch {
    return false;
  } finally {
    await browser.close();
  }
}

async function performLogin(persona, email, password, headed) {
  const browser = await chromium.launch({ headless: !headed });
  const context = await browser.newContext({ viewport: VIEWPORT });
  const page = await context.newPage();
  try {
    await page.goto(`${WEB_BASE}/auth/signin`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await completeUniversalLogin(page, email, password, headed);
    await waitForDashboard(page);
    await acceptGdpr(page);
    const file = sessionFile(persona);
    fs.mkdirSync(path.dirname(file), { recursive: true });
    await context.storageState({ path: file });
    return file;
  } finally {
    await browser.close();
  }
}

/** Sign in again and replace the saved session, so a new group or event id is on the token. */
export async function refreshPersonaSession(persona) {
  const { email, password } = personaCredentials(persona);
  try {
    return await performLogin(persona, email, password, false);
  } catch (error) {
    if (error.code !== EXIT.challenged) throw error;
    return await performLogin(persona, email, password, true);
  }
}

/** Reuse a saved session when the dashboard still loads. Otherwise sign in once. */
export async function ensurePersonaSession(persona) {
  const file = sessionFile(persona);
  if (fs.existsSync(file) && await dashboardLoads(file)) return file;
  const { email, password } = personaCredentials(persona);
  try {
    return await performLogin(persona, email, password, false);
  } catch (error) {
    if (error.code !== EXIT.challenged) throw error;
    return await performLogin(persona, email, password, true);
  }
}

/** Open a fresh browser signed in as persona. Caller closes the browser. */
export async function withPersonaPage(persona, run) {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ storageState: sessionFile(persona), viewport: VIEWPORT });
  const page = await context.newPage();
  try {
    await run(page);
  } finally {
    await browser.close();
  }
}

/** Wait until the signed-in dashboard has rendered the add button. */
export async function waitForDashboardFab(page) {
  const deadline = Date.now() + 60000;
  while (Date.now() < deadline) {
    const url = page.url();
    const text = await bodyText(page);
    const fab = await page.locator('#app-fabmenu button').count();
    if (url.startsWith(`${WEB_BASE}/dashboard`) && !text.includes('Warming up') && fab > 0 && !isSessionError(url, text)) {
      return;
    }
    await page.waitForTimeout(500);
  }
  const text = await bodyText(page);
  throw new Error(`Dashboard add button was not available (${page.url()}): ${text.replace(/\s+/g, ' ').slice(0, 240)}`);
}

/** Open the dashboard add menu and choose the flyout item with this Material icon name. */
export async function openFabItem(page, iconName) {
  await waitForDashboardFab(page);
  await page.locator('#app-fabmenu button').last().click();
  const item = page.locator('#app-fabmenu .flyout button', { has: page.locator('mat-icon', { hasText: iconName }) });
  await item.click();
}
