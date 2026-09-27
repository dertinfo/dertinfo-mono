import { ensurePersonaSession } from '../helpers.mjs';

export const id = 'auth.login.event-admin';

export async function run(ctx) {
  const file = await ensurePersonaSession('event-admin');
  ctx.provide('session:event-admin', file);
}
