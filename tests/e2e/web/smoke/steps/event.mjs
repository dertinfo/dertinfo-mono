import { activitiesByAudience, audienceLabel, defaultActivity, event, fixtureImage, formDate, teamActivity } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, openFabItem, waitForDashboardFab } from '../helpers.mjs';
import { EVENT_NAME, clickLabelledCheckbox, clickNext, clickPageAdd, entityCard, eventAdminUrl, fillControl, openCardMenu, selectedStepHeader } from '../pages.mjs';

async function fillDate(page, name, value) {
  const header = selectedStepHeader(page);
  const input = header.locator(`xpath=following::input[@formcontrolname="${name}"][1]`);
  // The step header is selected before the step body is visible.
  await input.waitFor({ state: 'visible', timeout: 15000 });
  // A resting mat-label covers the input, so a normal click lands on the label.
  await input.click({ force: true });
  await input.fill(value, { force: true });
  await input.press('Tab');
}

export async function createEvent(page, ctx) {
  await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
  await openFabItem(page, 'create-event');
  await page.locator('input[formcontrolname="eventName"]').fill(event.Name);
  await page.locator('input[formcontrolname="eventSynopsis"]').fill(event.EventSynopsis);
  await page.getByRole('button', { name: /Add Event/ }).click();
  await page.waitForURL(/\/event-configure\/[^/]+\/\d+/, { timeout: 120000, waitUntil: 'commit' });
  const match = page.url().match(/\/event-configure\/[^/]+\/(\d+)/);
  if (!match) throw new Error(`Create event did not open configure (${page.url()})`);
  ctx.provide('createdEventId', match[1]);
}

export async function seeUnconfiguredEvent(page, ctx) {
  const eventId = ctx.chain[normalizeToken('createdEventId')];
  await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
  await waitForDashboardFab(page);
  await page.getByRole('tab', { name: /Events/ }).click();
  const card = entityCard(page, EVENT_NAME);
  await card.getByText('Needs more information').waitFor({ state: 'visible', timeout: 30000 });
  await openCardMenu(page, EVENT_NAME);
  await page.getByRole('menuitem', { name: 'Configure' }).click();
  await page.waitForURL(new RegExp(`/event-configure/.*/${eventId}/`), { timeout: 60000, waitUntil: 'commit' });
}

export async function configureEvent(page, ctx) {
  const eventId = ctx.chain[normalizeToken('createdEventId')];
  await page.goto(`${WEB_BASE}/event-configure/${encodeURIComponent(EVENT_NAME)}/${eventId}/start`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await fillControl(page, 'eventSynopsis', event.EventSynopsis);
  await fillDate(page, 'eventStartDate', formDate(event.EventStartDate));
  await fillDate(page, 'eventEndDate', formDate(event.EventEndDate));
  await fillControl(page, 'locationTown', event.LocationTown);
  await fillControl(page, 'locationPostcode', event.LocationPostcode);
  await clickNext(page);
  await fillControl(page, 'contactName', event.ContactName);
  await fillControl(page, 'contactEmail', event.ContactEmail);
  await fillControl(page, 'contactTelephone', event.ContactTelephone);
  await clickNext(page);
  await page.getByRole('button', { name: 'Change Image' }).click();
  await page.getByText('Upload Images', { exact: true }).waitFor({ state: 'visible', timeout: 15000 });
  await page.getByTestId('upload-file').setInputFiles(fixtureImage(event));
  await page.getByTestId('upload-all').click();
  await page.getByText('Upload Images', { exact: true }).waitFor({ state: 'hidden', timeout: 90000 });
  const preview = page.locator('app-image-retry img');
  try {
    await page.waitForFunction(() => {
      const input = document.querySelector('input[formcontrolname="eventPictureUrl"]');
      const image = document.querySelector('app-image-retry img');
      const src = image && image.getAttribute('src');
      return !!(input && input.value && src && src.includes('480x360'));
    }, null, { timeout: 30000 });
  } catch {
    const stored = await page.locator('input[formcontrolname="eventPictureUrl"]').inputValue().catch(() => '');
    const shown = await preview.first().getAttribute('src').catch(() => '');
    throw new Error(`Event image was not resized to 480x360 (stored ${stored}, shown ${shown})`);
  }
  await clickNext(page);
  await fillDate(page, 'registrationOpenDate', formDate(event.RegistrationOpenDate));
  await fillDate(page, 'registrationCloseDate', formDate(event.RegistrationCloseDate));
  await clickNext(page);
  await page.locator(`[data-testid="event-type"][data-event-type="${event.EventTemplateType}"]`).click();
  await clickNext(page);
  await clickLabelledCheckbox(page, 'I agree to the terms and conditions');
  await clickNext(page);
  await page.getByRole('button', { name: 'Submit' }).click();
  await page.waitForURL(new RegExp(`/event/${encodeURIComponent(EVENT_NAME)}/${eventId}`), {
    timeout: 120000,
    waitUntil: 'commit',
  });
  ctx.provide('configuredEventId', eventId);
  await page.goto(eventAdminUrl(eventId), { waitUntil: 'domcontentloaded', timeout: 60000 });
}

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

export async function addEventActivities(page, ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await page.goto(eventAdminUrl(eventId, 'activities'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  for (const activity of activitiesByAudience(event, 'INDIVIDUAL')) await addActivity(page, activity);
  await page.getByRole('tab', { name: 'Team Activities' }).click();
  for (const activity of activitiesByAudience(event, 'TEAM')) await addActivity(page, activity);
  ctx.provide('addedIndividualActivityId', defaultActivity().Title);
  ctx.provide('addedTeamActivityId', teamActivity().Title);
}
