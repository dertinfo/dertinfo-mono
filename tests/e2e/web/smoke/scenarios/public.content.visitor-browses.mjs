import { chromium } from 'playwright';
import { VIEWPORT, WEB_BASE, openContent } from '../helpers.mjs';

export const id = 'public.content.visitor-browses';

export async function run() {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: VIEWPORT });
  try {
    const homeErrors = await openContent(page, '/', { text: 'My Dashboard' });
    if (!page.url().startsWith(`${WEB_BASE}/home`)) {
      throw new Error(`/ did not land on /home (${page.url()})`);
    }
    const brand = await page.locator('#home-header .navbar-brand').innerText();
    if (!brand.replace(/\s+/g, '').includes('DertInfo')) {
      throw new Error('Home header did not show DertInfo');
    }
    if ((await page.locator('#dashboard-topbtn').count()) < 1) {
      throw new Error('Home is missing the My Dashboard link');
    }
    if (homeErrors.length > 0) throw new Error(`Home had an uncaught page error: ${homeErrors[0]}`);

    await openContent(page, '/content/results', { text: 'Select Competition' });
    await openContent(page, '/content/history', { text: 'DertInfo', timeoutMs: 30000 });
    if ((await page.locator('#home-header .navbar-brand').count()) < 1) {
      throw new Error('/content/history did not render the public header');
    }
    if ((await page.locator('.section-header').count()) < 1) {
      throw new Error('/content/history did not render');
    }
    await openContent(page, '/content/community', { text: 'Permenant Members' });
    await openContent(page, '/content/notations', { text: 'AMBLE' });
    await openContent(page, '/dertofderts', { text: 'DERT of DERTs', timeoutMs: 45000 });
  } finally {
    await browser.close();
  }
}
