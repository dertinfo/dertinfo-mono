import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { clickLabelledCheckbox, clickNext, expectFlowState } from '../pages.mjs';

export const id = 'registration.submit.group-admin-opens';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-register/${groupId}/${eventId}/start`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await clickNext(page);
    await clickNext(page);
    await clickLabelledCheckbox(page, 'I agree to the terms and conditions');
    await page.getByRole('button', { name: 'Submit' }).click();
    await page.waitForURL(/\/group-registration\/\d+/, { timeout: 120000, waitUntil: 'commit' });
    const match = page.url().match(/\/group-registration\/(\d+)/);
    if (!match) throw new Error(`Registration did not open (${page.url()})`);
    await expectFlowState(page, 'New');
    ctx.provide('pendingRegistrationId', match[1]);
  });
}
