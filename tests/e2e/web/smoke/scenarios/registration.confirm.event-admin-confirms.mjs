import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { EVENT_NAME, expectFlowState } from '../pages.mjs';

export const id = 'registration.confirm.event-admin-confirms';

export async function run(ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  const registrationId = ctx.chain[normalizeToken('submittedRegistrationId')];
  await withPersonaPage('event-admin', async (page) => {
    await page.goto(`${WEB_BASE}/event/${encodeURIComponent(EVENT_NAME)}/${eventId}/registrations`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await page.locator(`mat-list-item[ng-reflect-router-link*="event-registration/${registrationId}"]`).click();
    await page.waitForURL(new RegExp(`/event-registration/${registrationId}`), { timeout: 60000, waitUntil: 'commit' });
    await page.getByRole('button', { name: 'Confirm', exact: true }).click();
    await expectFlowState(page, 'Confirmed');
  });
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await expectFlowState(page, 'Confirmed');
    if (await page.getByRole('button', { name: /Add Activities/ }).count()) {
      throw new Error('Group administrator can still edit a confirmed registration');
    }
  });
}
