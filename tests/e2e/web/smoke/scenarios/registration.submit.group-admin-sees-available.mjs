import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { EVENT_NAME, groupAdminUrl } from '../pages.mjs';

export const id = 'registration.submit.group-admin-sees-available';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'registrations'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.getByRole('tab', { name: 'Available / New' }).click();
    await page.getByText(EVENT_NAME).waitFor({ state: 'visible', timeout: 30000 });
  });
}
