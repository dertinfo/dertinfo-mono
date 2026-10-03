import { activitiesByAudience, audienceLabel, defaultActivity, event, teamActivity } from '../config/load.mjs';
import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { clickPageAdd, eventAdminUrl } from '../pages.mjs';

export const id = 'events.create.event-admin-adds-activities';

async function addActivity(page, activity) {
  await clickPageAdd(page);
  const dialog = page.getByRole('dialog').last();
  await dialog.getByText('Add a new activity').waitFor({ state: 'visible', timeout: 15000 });
  await dialog.locator('input[formcontrolname="title"]').fill(activity.Title);
  await dialog.locator('input[formcontrolname="description"], textarea[formcontrolname="description"]').fill(activity.Description);
  await dialog.locator('input[formcontrolname="price"]').fill(String(activity.Price));
  await dialog.getByRole('combobox', { name: /Target Audience/ }).click();
  await page.getByRole('option', { name: audienceLabel(activity) }).click();
  if (activity.IsDefault) await dialog.locator('mat-checkbox').filter({ hasText: 'Set as Default' }).click();
  await dialog.getByRole('button', { name: /Add Activity/ }).click();
  await dialog.waitFor({ state: 'hidden', timeout: 30000 });
  await page.getByText(activity.Title, { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
}

export async function run(ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await withPersonaPage('event-admin', async (page) => {
    await page.goto(eventAdminUrl(eventId, 'activities'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    for (const activity of activitiesByAudience(event, 'INDIVIDUAL')) await addActivity(page, activity);
    await page.getByRole('tab', { name: 'Team Activities' }).click();
    for (const activity of activitiesByAudience(event, 'TEAM')) await addActivity(page, activity);
    ctx.provide('addedIndividualActivityId', defaultActivity().Title);
    ctx.provide('addedTeamActivityId', teamActivity().Title);
  });
}
