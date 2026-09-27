import { WEB_BASE, openFabItem, refreshPersonaSession, withPersonaPage } from '../helpers.mjs';

export const id = 'groups.manage.group-admin-creates';

export async function run(ctx) {
  await refreshPersonaSession('group-admin');
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await openFabItem(page, 'group_work');
    await page.locator('input[formcontrolname="groupName"]').fill('The Simpsons');
    await page.locator('input[formcontrolname="groupBio"]').fill('The family from Springfield');
    await page.getByRole('button', { name: /Add Group/ }).click();
    await page.waitForURL(/\/group-configure\/[^/]+\/\d+/, { timeout: 120000, waitUntil: 'commit' });
    const match = page.url().match(/\/group-configure\/[^/]+\/(\d+)/);
    if (!match) throw new Error(`Create group did not open configure (${page.url()})`);
    ctx.provide('createdGroupId', match[1]);
  });
  await refreshPersonaSession('group-admin');
}
