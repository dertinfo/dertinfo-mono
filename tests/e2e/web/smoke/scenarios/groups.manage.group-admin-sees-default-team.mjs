import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { GROUP_NAME, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-sees-default-team';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'teams'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.locator('app-group-teams').getByText(GROUP_NAME, { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
  });
}
