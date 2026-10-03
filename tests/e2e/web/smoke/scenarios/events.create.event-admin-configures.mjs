import { event, fixtureImage, formDate } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { EVENT_NAME, clickLabelledCheckbox, clickNext, eventAdminUrl, fillControl } from '../pages.mjs';

async function fillDate(page, name, value) {
  const header = page.locator('mat-step-header[aria-selected="true"]');
  const input = header.locator(`xpath=following::input[@formcontrolname="${name}"][1]`);
  await input.click();
  await input.fill(value);
  await input.press('Tab');
}

export const id = 'events.create.event-admin-configures';

export async function run(ctx) {
  const eventId = ctx.chain[normalizeToken('createdEventId')];
  await withPersonaPage('event-admin', async (page) => {
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
    await page.waitForFunction(() => {
      const input = document.querySelector('input[formcontrolname="eventPictureUrl"]');
      return !!(input && input.value);
    }, null, { timeout: 30000 });
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
  });
}
