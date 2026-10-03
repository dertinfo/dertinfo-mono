import { defaultActivity, member, team, teamActivity } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { attendanceCard, clickSelectOption, expectFlowState, selectOption } from '../pages.mjs';

export const id = 'registration.submit.group-admin-assigns-activities';

async function openActivities(page, person) {
  const card = attendanceCard(page, person);
  await card.getByRole('button', { name: /Add Activities/ }).click();
  await page.getByText('Select Tickets').waitFor({ state: 'visible', timeout: 15000 });
  return card;
}

export async function run(ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    const memberName = member('activeMember').Name;
    const guestName = member('guest').Name;
    const teamName = team().TeamName;
    const individualTitle = defaultActivity().Title;
    const teamTitle = teamActivity().Title;

    const homer = attendanceCard(page, memberName);
    await homer.getByText(individualTitle).waitFor({ state: 'visible', timeout: 30000 });
    await openActivities(page, memberName);
    await selectOption(page, individualTitle).waitFor({ state: 'visible' });
    await page.getByRole('button', { name: 'Cancel' }).click();

    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/guests`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    const marge = attendanceCard(page, guestName);
    await marge.getByText(individualTitle).waitFor({ state: 'visible', timeout: 30000 });
    await openActivities(page, guestName);
    await selectOption(page, individualTitle).waitFor({ state: 'visible' });
    await page.getByRole('button', { name: 'Cancel' }).click();

    await page.goto(`${WEB_BASE}/group-registration/${registrationId}/teams`, {
      waitUntil: 'domcontentloaded',
      timeout: 60000,
    });
    await openActivities(page, teamName);
    await clickSelectOption(page, teamTitle);
    await page.getByRole('button', { name: /Save Changes/ }).click();
    const teamCard = attendanceCard(page, teamName);
    await teamCard.getByText(teamTitle).waitFor({ state: 'visible', timeout: 30000 });
    await expectFlowState(page, 'New');
  });
}
