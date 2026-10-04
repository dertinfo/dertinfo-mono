import { withPersonaPage } from '../helpers.mjs';
import { reviewEventInvoice, reviewGroupInvoice } from '../steps/invoicing.mjs';
import { part } from '../steps/log.mjs';
import { amendSubmittedRegistration, confirmRegistration, seeConfirmedRegistration } from '../steps/registration.mjs';

export const id = 'registration.groupadmin.checkregistrationandedit-scenario1';

export async function run(ctx) {
  await withPersonaPage('group-admin', async (page) => {
    await part(id, 'amend submitted', () => amendSubmittedRegistration(page, ctx));
  });
  await withPersonaPage('event-admin', async (page) => {
    await part(id, 'confirm', () => confirmRegistration(page, ctx));
  });
  await withPersonaPage('group-admin', async (page) => {
    await part(id, 'confirmed for group', () => seeConfirmedRegistration(page, ctx));
  });
  await withPersonaPage('event-admin', async (page) => {
    await part(id, 'event invoice', () => reviewEventInvoice(page, ctx));
  });
  await withPersonaPage('group-admin', async (page) => {
    await part(id, 'group invoice', () => reviewGroupInvoice(page, ctx));
  });
}
