import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { ensureAttendance } from '../pages.mjs';

export const id = 'registration.submit.group-admin-adds-member';

export async function run(ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await ensureAttendance(page, 'Homer Simpson');
  });
}
