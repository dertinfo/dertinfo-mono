import { withPersonaPage } from '../helpers.mjs';
import { part } from '../steps/log.mjs';
import { addRegistrationGuest, addRegistrationMember, addRegistrationTeam, assignRegistrationActivities, editPendingRegistration, openRegistration, seeAvailableEvent, submitRegistration } from '../steps/registration.mjs';

export const id = 'registration.groupadmin.registerforevent-scenario1';

export async function run(ctx) {
  await withPersonaPage('group-admin', async (page) => {
    await part(id, 'available event', () => seeAvailableEvent(page, ctx));
    await part(id, 'open registration', () => openRegistration(page, ctx));
    await part(id, 'add member', () => addRegistrationMember(page, ctx));
    await part(id, 'add guest', () => addRegistrationGuest(page, ctx));
    await part(id, 'add team', () => addRegistrationTeam(page, ctx));
    await part(id, 'assign activities', () => assignRegistrationActivities(page, ctx));
    await part(id, 'edit pending', () => editPendingRegistration(page, ctx));
    await part(id, 'submit', () => submitRegistration(page, ctx));
  });
}
