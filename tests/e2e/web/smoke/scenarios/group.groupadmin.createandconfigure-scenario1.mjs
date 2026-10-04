import { refreshPersonaSession, withPersonaPage } from '../helpers.mjs';
import { addGroupGuest, addGroupMember, addGroupTeam, changeGroupImage, configureGroup, createGroup, seeDefaultTeam, seeUnconfiguredGroup } from '../steps/group.mjs';
import { part } from '../steps/log.mjs';

export const id = 'group.groupadmin.createandconfigure-scenario1';

export async function run(ctx) {
  await part(id, 'sign in', async () => {
    ctx.provide('session:group-admin', await refreshPersonaSession('group-admin'));
  });
  await withPersonaPage('group-admin', async (page) => {
    await part(id, 'create', () => createGroup(page, ctx));
    await part(id, 'unconfigured', () => seeUnconfiguredGroup(page, ctx));
    await part(id, 'configure', () => configureGroup(page, ctx));
    await part(id, 'default team', () => seeDefaultTeam(page, ctx));
    await part(id, 'add member', () => addGroupMember(page, ctx));
    await part(id, 'add guest', () => addGroupGuest(page, ctx));
    await part(id, 'add team', () => addGroupTeam(page, ctx));
    await part(id, 'change image', () => changeGroupImage(page, ctx));
  });
  await refreshPersonaSession('group-admin');
}
