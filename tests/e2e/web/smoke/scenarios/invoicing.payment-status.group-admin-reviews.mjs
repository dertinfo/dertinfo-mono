import { normalizeToken, withPersonaPage } from '../helpers.mjs';
import { bodyText, groupAdminUrl } from '../pages.mjs';

export const id = 'invoicing.payment-status.group-admin-reviews';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupAdminUrl(groupId, 'invoices'), { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.getByText('Invoices & Payments', { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
    await page.getByText('Not yet received').waitFor({ state: 'visible', timeout: 30000 });
    const listing = await bodyText(page);
    if (!listing.includes('Not yet received')) {
      throw new Error('Group invoice is not marked Not yet received');
    }
  });
}
