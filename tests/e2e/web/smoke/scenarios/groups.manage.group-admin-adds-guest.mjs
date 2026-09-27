import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { fillControl, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-adds-guest';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'members'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.locator('#app-addition-header button').click();
    await page.getByRole('tab', { name: 'Add New Person' }).click();
    await fillControl(page, 'name', 'Marge Simpson');
    await page.locator('mat-radio-button', { hasText: 'Guest' }).click();
    await page.getByRole('button', { name: /Add Member/ }).click();
    await page.getByRole('tab', { name: 'Guests' }).click();
    await page.getByText('Marge Simpson').waitFor({ state: 'visible', timeout: 30000 });
    ctx.provide('addedGuestId', 'Marge Simpson');
  });
}
