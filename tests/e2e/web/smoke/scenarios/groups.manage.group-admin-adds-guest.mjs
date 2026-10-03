import { member } from '../config/load.mjs';
import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { clickLabelledRadio, clickPageAdd, fillControl, groupAdminUrl } from '../pages.mjs';

export const id = 'groups.manage.group-admin-adds-guest';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'members'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await clickPageAdd(page);
    await page.getByRole('tab', { name: 'Add New Person' }).click();
    const person = member('guest');
    await fillControl(page, 'name', person.Name);
    await clickLabelledRadio(page, 'Guest');
    await page.getByRole('button', { name: /Add Member/ }).click();
    await page.getByRole('tab', { name: 'Guests' }).click();
    await page.getByText(person.Name).waitFor({ state: 'visible', timeout: 30000 });
    ctx.provide('addedGuestId', person.Name);
  });
}
