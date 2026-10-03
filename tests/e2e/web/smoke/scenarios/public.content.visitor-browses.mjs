import { chromium } from 'playwright';
import { SMOKE_CONTEXT, WEB_BASE, applyCookieConsent, openContent } from '../helpers.mjs';

export const id = 'public.content.visitor-browses';

export async function run() {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext(SMOKE_CONTEXT);
  await applyCookieConsent(context);
  const page = await context.newPage();
  try {
    const homeErrors = await openContent(page, '/', { testId: 'public-home' });
    if (!page.url().startsWith(`${WEB_BASE}/home`)) {
      throw new Error(`/ did not land on /home (${page.url()})`);
    }
    const brand = await page.getByTestId('public-brand').innerText();
    if (!brand.replace(/\s+/g, '').includes('DertInfo')) {
      throw new Error('Home header did not show DertInfo');
    }
    await page.getByTestId('public-dashboard').waitFor({ state: 'visible' });
    if (homeErrors.length > 0) throw new Error(`Home had an uncaught page error: ${homeErrors[0]}`);

    await openContent(page, '/content/results', { testId: 'public-results' });
    await openContent(page, '/content/history', { testId: 'public-history', timeoutMs: 30000 });
    await openContent(page, '/content/community', { testId: 'public-community' });
    await openContent(page, '/content/notations', { testId: 'public-notations' });
    await openContent(page, '/dertofderts', { testId: 'public-dertofderts', timeoutMs: 45000 });
  } finally {
    await browser.close();
  }
}
