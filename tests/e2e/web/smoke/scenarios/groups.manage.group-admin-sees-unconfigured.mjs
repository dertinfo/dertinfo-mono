import { WEB_BASE, normalizeToken, waitForDashboardFab, withPersonaPage } from '../helpers.mjs';
import { GROUP_NAME, openCardMenu } from '../pages.mjs';

export const id = 'groups.manage.group-admin-sees-unconfigured';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await waitForDashboardFab(page);
    const card = page.locator('mat-card', { hasText: GROUP_NAME }).first();
    await card.getByText('Needs more information').waitFor({ state: 'visible', timeout: 30000 });
    await openCardMenu(page, GROUP_NAME);
    await page.getByRole('menuitem', { name: 'Configure' }).click();
    await page.waitForURL(new RegExp(`/group-configure/.*/${groupId}/`), { timeout: 60000, waitUntil: 'commit' });
  });
}
