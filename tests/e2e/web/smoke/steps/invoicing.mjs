import { team } from '../config/load.mjs';
import { normalizeToken } from '../helpers.mjs';
import { bodyText, eventAdminUrl, groupAdminUrl } from '../pages.mjs';

export async function reviewEventInvoice(page, ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await page.goto(eventAdminUrl(eventId, 'invoices'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.locator('app-addition-header').filter({ hasText: 'Invoices & Payments' }).waitFor({ state: 'visible', timeout: 30000 });
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
  if (!detail.includes(team().TeamName)) {
    throw new Error('Invoice does not show the team');
  }
}

export async function reviewGroupInvoice(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'invoices'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.locator('app-addition-header').filter({ hasText: 'Invoices & Payments' }).waitFor({ state: 'visible', timeout: 30000 });
  await page.getByText('Not yet received').waitFor({ state: 'visible', timeout: 30000 });
  const listing = await bodyText(page);
  if (!listing.includes('Not yet received')) {
    throw new Error('Group invoice is not marked Not yet received');
  }
}
