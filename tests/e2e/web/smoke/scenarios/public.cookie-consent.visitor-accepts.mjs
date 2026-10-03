import { chromium } from 'playwright';
import { SMOKE_CONTEXT, WEB_BASE, acceptCookieConsentIfVisible, saveCookieConsent } from '../helpers.mjs';

export const id = 'public.cookie-consent.visitor-accepts';

export async function run() {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext(SMOKE_CONTEXT);
  const page = await context.newPage();
  try {
    await page.goto(WEB_BASE, { waitUntil: 'domcontentloaded', timeout: 60000 });
    const accepted = await acceptCookieConsentIfVisible(page, { timeoutMs: 15000 });
    const cookies = await context.cookies();
    const consented = cookies.some((cookie) => cookie.name === 'cookie-consent' && cookie.value === 'consented');
    if (accepted && !consented) throw new Error('Accept Cookies did not store cookie-consent');
    if (!accepted && !consented) throw new Error('Cookie consent was not shown and has not been accepted');
    await saveCookieConsent(context);
  } finally {
    await browser.close();
  }
}
