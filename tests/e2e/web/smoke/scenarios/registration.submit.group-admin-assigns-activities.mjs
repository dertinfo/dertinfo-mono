import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { expectFlowState } from '../pages.mjs';

export const id = 'registration.submit.group-admin-assigns-activities';

async function openActivities(page, person) {
  const card = page.locator('mat-card', { hasText: person }).first();
  await card.getByRole('button', { name: /Add Activities/ }).click();
  await page.getByText('Select Tickets').waitFor({ state: 'visible', timeout: 15000 });
  return card;
}

export async function run(ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    const homer = page.locator('mat-card', { hasText: 'Homer Simpson' }).first();
    await homer.getByText('Zoo Ticket').waitFor({ state: 'visible', timeout: 30000 });
    await openActivities(page, 'Homer Simpson');
    await page.locator('.activity-select', { hasText: 'Zoo Ticket' }).waitFor({ state: 'visible' });
    await page.getByRole('button', { name: 'Cancel' }).click();

    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/guests`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    const marge = page.locator('mat-card', { hasText: 'Marge Simpson' }).first();
    await marge.getByText('Zoo Ticket').waitFor({ state: 'visible', timeout: 30000 });
    await openActivities(page, 'Marge Simpson');
    await page.locator('.activity-select', { hasText: 'Zoo Ticket' }).waitFor({ state: 'visible' });
    await page.getByRole('button', { name: 'Cancel' }).click();

    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/teams`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await openActivities(page, 'Springfield');
    await page.locator('.activity-select', { hasText: 'Zoo Dance' }).locator('button').click();
    await page.getByRole('button', { name: /Save Changes/ }).click();
    const team = page.locator('mat-card', { hasText: 'Springfield' }).first();
    await team.getByText('Zoo Dance').waitFor({ state: 'visible', timeout: 30000 });
    await expectFlowState(page, 'New');
  });
}
