import { WEB_BASE, normalizeToken, waitForDashboardFab, withPersonaPage } from '../helpers.mjs';
import { EVENT_NAME, openCardMenu } from '../pages.mjs';

export const id = 'events.create.event-admin-sees-unconfigured';

export async function run(ctx) {
  const eventId = ctx.chain[normalizeToken('createdEventId')];
  await withPersonaPage('event-admin', async (page) => {
    await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await waitForDashboardFab(page);
    await page.getByRole('tab', { name: /Events/ }).click();
    const card = page.locator('mat-card', { hasText: EVENT_NAME }).first();
    await card.getByText('Needs more information').waitFor({ state: 'visible', timeout: 30000 });
    await openCardMenu(page, EVENT_NAME);
    await page.getByRole('menuitem', { name: 'Configure' }).click();
    await page.waitForURL(new RegExp(`/event-configure/.*/${eventId}/`), { timeout: 60000, waitUntil: 'commit' });
  });
}
