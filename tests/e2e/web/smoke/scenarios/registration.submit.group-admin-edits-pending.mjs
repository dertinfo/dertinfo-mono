import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { expectFlowState } from '../pages.mjs';

export const id = 'registration.submit.group-admin-edits-pending';

export async function run(ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    const card = page.locator('mat-card', { hasText: 'Homer Simpson' }).first();
    await card.getByRole('button', { name: /Add Activities/ }).click();
    await page.locator('.activity-select', { hasText: 'Zoo Ticket' }).locator('button').click();
    await page.getByRole('button', { name: /Save Changes/ }).click();
    await page.reload({ waitUntil: 'domcontentloaded' });
    await card.getByText('Zoo Ticket').waitFor({ state: 'hidden', timeout: 30000 });
    await expectFlowState(page, 'New');
  });
}
