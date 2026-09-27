import { ensurePersonaSession } from '../helpers.mjs';

export const id = 'auth.login.group-admin';

export async function run(ctx) {
  const file = await ensurePersonaSession('group-admin');
  ctx.provide('session:group-admin', file);
}
