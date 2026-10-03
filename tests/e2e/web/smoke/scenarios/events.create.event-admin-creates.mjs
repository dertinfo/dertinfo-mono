import { event } from '../config/load.mjs';
import { WEB_BASE, openFabItem, refreshPersonaSession, withPersonaPage } from '../helpers.mjs';

export const id = 'events.create.event-admin-creates';

export async function run(ctx) {
  await refreshPersonaSession('event-admin');
  await withPersonaPage('event-admin', async (page) => {
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await openFabItem(page, 'create-event');
    await page.locator('input[formcontrolname="eventName"]').fill(event.Name);
    await page.locator('input[formcontrolname="eventSynopsis"]').fill(event.EventSynopsis);
    await page.getByRole('button', { name: /Add Event/ }).click();
    await page.waitForURL(/\/event-configure\/[^/]+\/\d+/, { timeout: 120000, waitUntil: 'commit' });
    const match = page.url().match(/\/event-configure\/[^/]+\/(\d+)/);
    if (!match) throw new Error(`Create event did not open configure (${page.url()})`);
    ctx.provide('createdEventId', match[1]);
  });
  await refreshPersonaSession('event-admin');
}
