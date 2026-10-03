import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { expectFlowState, groupAdminUrl, registrationRow } from '../pages.mjs';

export const id = 'registration.submit.group-admin-submits';

export async function run(ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'registrations'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.getByRole('tab', { name: 'Active' }).click();
    await registrationRow(page, registrationId).click();
    await page.waitForURL(new RegExp(`/group-registration/${registrationId}`), { timeout: 60000, waitUntil: 'commit' });
    await page.getByRole('button', { name: 'Submit' }).click();
    await expectFlowState(page, 'Submitted');
    ctx.provide('submittedRegistrationId', registrationId);
  });
}
