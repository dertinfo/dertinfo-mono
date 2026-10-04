import { refreshPersonaSession, withPersonaPage } from '../helpers.mjs';
import { addEventActivities, configureEvent, createEvent, seeUnconfiguredEvent } from '../steps/event.mjs';
import { part } from '../steps/log.mjs';

export const id = 'event.eventadmin.createandconfigure-scenario1';

export async function run(ctx) {
  await part(id, 'sign in', async () => {
    ctx.provide('session:event-admin', await refreshPersonaSession('event-admin'));
  });
  await withPersonaPage('event-admin', async (page) => {
    await part(id, 'create', () => createEvent(page, ctx));
    await part(id, 'unconfigured', () => seeUnconfiguredEvent(page, ctx));
    await part(id, 'configure', () => configureEvent(page, ctx));
    await part(id, 'add activities', () => addEventActivities(page, ctx));
  });
  await refreshPersonaSession('event-admin');
}
