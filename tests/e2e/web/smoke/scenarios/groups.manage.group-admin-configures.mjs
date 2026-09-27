import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { GROUP_NAME, clickLabelledCheckbox, clickNext, fillControl, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-configures';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-configure/${encodeURIComponent(GROUP_NAME)}/${groupId}/start`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await fillControl(page, 'groupBio', 'The family from Springfield');
    await fillControl(page, 'originTown', 'Springfield');
    await fillControl(page, 'originPostcode', 'SP1 1AA');
    await clickNext(page);
    await fillControl(page, 'contactName', 'Homer Simpson');
    await fillControl(page, 'contactEmail', 'group-admin-1@dertinfo.co.uk');
    await fillControl(page, 'contactTelephone', '01234567890');
    await clickNext(page);
    await clickNext(page);
    await clickNext(page);
    await clickLabelledCheckbox(page, 'I agree to the terms and conditions');
    await clickNext(page);
    await page.getByRole('button', { name: 'Submit' }).click();
    await page.waitForURL(new RegExp(`/group/${encodeURIComponent(GROUP_NAME)}/${groupId}`), {
      timeout: 120000,
      waitUntil: 'commit',
    });
    ctx.provide('configuredGroupId', groupId);
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    const card = page.locator('mat-card', { hasText: GROUP_NAME }).first();
    if (await card.getByText('Needs more information').count()) {
      throw new Error('Configured group still shows Needs more information');
    }
    await page.goto(groupAdminUrl(groupId), { waitUntil: 'domcontentloaded', timeout: 60000 });
  });
}
