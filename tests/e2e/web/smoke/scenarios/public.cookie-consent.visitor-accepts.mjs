import { WEB_BASE, acceptCookieConsentIfVisible, saveCookieConsent, withSmokePage } from '../helpers.mjs';

export const id = 'public.cookie-consent.visitor-accepts';

export async function run() {
  await withSmokePage(async (page) => {
    await page.goto(WEB_BASE, { waitUntil: 'domcontentloaded', timeout: 60000 });
    const accepted = await acceptCookieConsentIfVisible(page, { timeoutMs: 15000 });
    const cookies = await page.context().cookies();
    const consented = cookies.some((cookie) => cookie.name === 'cookie-consent' && cookie.value === 'consented');
    if (accepted && !consented) throw new Error('Accept Cookies did not store cookie-consent');
    if (!accepted && !consented) throw new Error('Cookie consent was not shown and has not been accepted');
    await saveCookieConsent(page.context());
  }, { label: 'cookie-consent' });
}
