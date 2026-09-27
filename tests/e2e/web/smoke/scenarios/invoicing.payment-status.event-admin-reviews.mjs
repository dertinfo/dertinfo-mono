import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { bodyText, eventAdminUrl } from '../pages.mjs';

export const id = 'invoicing.payment-status.event-admin-reviews';

export async function run(ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await withPersonaPage('event-admin', async (page) => {
    await page.goto(eventAdminUrl(eventId, 'invoices'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.getByText('Invoices & Payments', { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
    await page.getByText('Registration Total').waitFor({ state: 'visible', timeout: 30000 });
    const listing = await bodyText(page);
    if (!listing.includes('Registration Total') || !listing.includes('Invoice Total')) {
      throw new Error('Invoice is missing the registration total or the invoice total');
    }
    if (!listing.includes('Not yet received')) {
      throw new Error('Invoice is not marked Not yet received');
    }
    await page.locator('mat-list-item', { hasText: 'Registration Total' }).first().click();
    await page.getByText('on behalf of').waitFor({ state: 'visible', timeout: 30000 });
    const detail = await bodyText(page);
    if (!detail.includes('Springfield')) {
      throw new Error('Invoice does not show the team');
    }
  });
}
