import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { clickLabelledCheckbox, eventAdminUrl, fillControl } from '../pages.mjs';

export const id = 'events.create.event-admin-adds-activities';

async function addActivity(page, title, audience, setDefault) {
  await page.locator('#app-addition-header button').click();
  await page.getByText('Add a new activity').waitFor({ state: 'visible', timeout: 15000 });
  await fillControl(page, 'title', title);
  await fillControl(page, 'price', '10');
  await page.getByRole('combobox', { name: /Target Audience/ }).click();
  await page.getByRole('option', { name: audience }).click();
  if (setDefault) await clickLabelledCheckbox(page, 'Set as Default');
  await page.getByRole('button', { name: /Add Activity/ }).click();
  await page.getByText(title, { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
}

export async function run(ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await withPersonaPage('event-admin', async (page) => {
    await page.goto(eventAdminUrl(eventId, 'activities'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await addActivity(page, 'Zoo Ticket', 'Individual', true);
    await page.getByRole('tab', { name: 'Team Activities' }).click();
    await addActivity(page, 'Zoo Dance', 'Team', false);
    ctx.provide('addedIndividualActivityId', 'Zoo Ticket');
    ctx.provide('addedTeamActivityId', 'Zoo Dance');
  });
}
