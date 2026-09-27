import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';
import { EVENT_NAME, clickLabelledCheckbox, clickNext, eventAdminUrl, fillControl } from '../pages.mjs';

async function fillDate(page, name, value) {
  const input = page.locator('.mat-step:has(mat-step-header[aria-selected="true"])').locator(`input[formcontrolname="${name}"]`);
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
    await fillControl(page, 'eventSynopsis', 'A day at the zoo with the animals');
    await fillDate(page, 'eventStartDate', '01/06/2027');
    await fillDate(page, 'eventEndDate', '02/06/2027');
    await fillControl(page, 'locationTown', 'Springfield');
    await fillControl(page, 'locationPostcode', 'SP1 1AA');
    await clickNext(page);
    await fillControl(page, 'contactName', 'Event Keeper');
    await fillControl(page, 'contactEmail', 'event-admin-1@dertinfo.co.uk');
    await fillControl(page, 'contactTelephone', '01234567891');
    await clickNext(page);
    await page.waitForFunction(() => {
      const input = document.querySelector('input[formcontrolname="eventPictureUrl"]');
      return !!(input && input.value);
    }, null, { timeout: 30000 });
    await clickNext(page);
    await fillDate(page, 'registrationOpenDate', '01/01/2026');
    await fillDate(page, 'registrationCloseDate', '01/05/2027');
    await clickNext(page);
    await page.locator('mat-radio-button', { hasText: 'Dancing England Rapper Tournament(Standard)' }).click();
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
