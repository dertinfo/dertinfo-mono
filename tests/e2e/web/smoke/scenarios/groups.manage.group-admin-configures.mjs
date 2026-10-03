import { group } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { GROUP_NAME, clickLabelledCheckbox, clickNext, entityCard, fillControl, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-configures';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-configure/${encodeURIComponent(GROUP_NAME)}/${groupId}/start`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await fillControl(page, 'groupBio', group.GroupBio);
    await fillControl(page, 'originTown', group.OriginTown);
    await fillControl(page, 'originPostcode', group.OriginPostcode);
    await clickNext(page);
    await fillControl(page, 'contactName', group.PrimaryContactName);
    await fillControl(page, 'contactEmail', group.PrimaryContactEmail);
    await fillControl(page, 'contactTelephone', group.PrimaryContactNumber);
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
    const card = entityCard(page, GROUP_NAME);
    await card.waitFor({ state: 'visible', timeout: 30000 });
    if (await card.getByText('Needs more information').isVisible()) {
      throw new Error('Configured group still shows Needs more information');
    }
    await page.goto(groupAdminUrl(groupId), { waitUntil: 'domcontentloaded', timeout: 60000 });
  });
}
