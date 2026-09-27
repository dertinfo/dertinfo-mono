import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { fillControl, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-adds-team';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'teams'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.locator('#app-addition-header button').click();
    await page.getByRole('tab', { name: 'Add New Team' }).click();
    await fillControl(page, 'teamName', 'Springfield');
    await fillControl(page, 'teamBio', 'A team from Springfield');
    await page.getByRole('button', { name: /Add Team/ }).click();
    await page.locator('app-group-teams').getByText('Springfield', { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
    ctx.provide('addedTeamId', 'Springfield');
  });
}
