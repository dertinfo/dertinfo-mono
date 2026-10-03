import { chromium } from 'playwright';
import { API_BASE, VIEWPORT, WEB_BASE, applyCookieConsent } from '../helpers.mjs';

export const id = 'auth.session-continuity.rejected-cache-returns-to-sign-in';

function plantSession(config) {
  const scope = 'openid profile email offline_access';
  const base64Url = (value) => btoa(JSON.stringify(value)).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '');
  const now = Math.floor(Date.now() / 1000);
  const claims = {
    sub: 'auth0|stale-session-test',
    email: 'stale-session@example.com',
    name: 'Stale Session',
    iss: `https://${config.auth0TenantDomain}/`,
    aud: config.auth0ClientId,
    iat: now - 3600,
    exp: now + 3600,
  };
  const idToken = `${base64Url({ alg: 'RS256', typ: 'JWT' })}.${base64Url(claims)}.c3RhbGU`;
  const decodedToken = {
    user: { sub: claims.sub, email: claims.email, name: claims.name },
    claims,
  };
  const userKey = `@@auth0spajs@@::${config.auth0ClientId}::@@user@@`;
  const tokenKey = `@@auth0spajs@@::${config.auth0ClientId}::${config.auth0Audience}::${scope}`;
  localStorage.setItem(userKey, JSON.stringify({ id_token: idToken, decodedToken }));
  localStorage.setItem(tokenKey, JSON.stringify({
    body: {
      client_id: config.auth0ClientId,
      access_token: 'stale-access-token',
      id_token: idToken,
      refresh_token: 'stale-refresh-token-not-valid',
      scope,
      expires_in: 7200,
      token_type: 'Bearer',
      audience: config.auth0Audience,
      decodedToken,
    },
    expiresAt: now - 120,
  }));
  localStorage.setItem('user_data', JSON.stringify({
    email: claims.email,
    name: claims.name,
    gdprConsentGained: true,
  }));
  localStorage.setItem('dertinfo_access_token', 'stale-access-token');
  sessionStorage.removeItem('sessionwarm');
}

function isSignIn(url) {
  return url.includes('/authorize') || url.includes('/u/login') || url.includes('/auth/signin');
}

export async function run() {
  const configResponse = await fetch(`${API_BASE}/clientconfiguration/web`);
  if (!configResponse.ok) throw new Error(`Client configuration failed: HTTP ${configResponse.status}`);
  const config = await configResponse.json();
  if (!config.auth0ClientId || !config.auth0Audience || !config.auth0TenantDomain) {
    throw new Error('Client configuration is missing Auth0 client id, audience, or domain.');
  }

  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: VIEWPORT });
  try {
    await applyCookieConsent(page.context());
    await page.addInitScript(plantSession, config);
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    const deadline = Date.now() + 15000;
    let url = page.url();
    while (Date.now() < deadline) {
      url = page.url();
      if (!url.startsWith(WEB_BASE) && isSignIn(url)) break;
      await page.waitForTimeout(1000);
    }
    const storage = await page.context().storageState();
    const origin = storage.origins.find((entry) => entry.origin === WEB_BASE);
    const cacheKeys = (origin?.localStorage ?? [])
      .map((entry) => entry.name)
      .filter((key) => key.startsWith('@@auth0spajs@@') || key === 'user_data' || key === 'dertinfo_access_token');
    const text = url.startsWith(WEB_BASE) ? await page.locator('body').innerText() : '';
    if (text.includes('Warming up') || cacheKeys.length > 0 || !isSignIn(url)) {
      throw new Error(`Rejected cache did not return to sign-in (${url})`);
    }
  } finally {
    await browser.close();
  }
}
