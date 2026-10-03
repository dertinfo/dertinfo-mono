import { team } from '../config/load.mjs';
import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { clickPageAdd, fillControl, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-adds-team';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'teams'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await clickPageAdd(page);
    await page.getByRole('tab', { name: 'Add New Team' }).click();
    const groupTeam = team();
    await fillControl(page, 'teamName', groupTeam.TeamName);
    await fillControl(page, 'teamBio', groupTeam.TeamBio);
    await page.getByRole('button', { name: /Add Team/ }).click();
    await page.locator('app-group-teams').getByText(groupTeam.TeamName, { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
    ctx.provide('addedTeamId', groupTeam.TeamName);
  });
}
