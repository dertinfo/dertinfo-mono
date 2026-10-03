import { defaultActivity, member } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { attendanceCard, clickSelectOption, expectFlowState } from '../pages.mjs';

export const id = 'registration.submit.group-admin-amends-submitted';

export async function run(ctx) {
  const registrationId = ctx.chain[normalizeToken('submittedRegistrationId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    const person = member('activeMember');
    const title = defaultActivity().Title;
    const card = attendanceCard(page, person.Name);
    await card.getByRole('button', { name: /Add Activities/ }).click();
    await clickSelectOption(page, title);
    await page.getByRole('button', { name: /Save Changes/ }).click();
    await page.reload({ waitUntil: 'domcontentloaded' });
    await card.getByText(title).waitFor({ state: 'visible', timeout: 30000 });
    await expectFlowState(page, 'Submitted');
  });
}
