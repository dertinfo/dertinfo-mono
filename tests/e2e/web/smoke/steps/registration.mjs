import { defaultActivity, member, team, teamActivity } from '../config/load.mjs';
import { WEB_BASE, normalizeToken } from '../helpers.mjs';
import { EVENT_NAME, attendanceCard, clickLabelledCheckbox, clickNext, clickSelectOption, ensureAttendance, expectFlowState, groupAdminUrl, registrationRow, selectOption } from '../pages.mjs';

export async function seeAvailableEvent(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'registrations'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.getByRole('tab', { name: 'Available / New' }).click();
  await page.getByText(EVENT_NAME).waitFor({ state: 'visible', timeout: 30000 });
}

export async function openRegistration(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  await page.goto(`${WEB_BASE}/group-register/${groupId}/${eventId}/start`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await clickNext(page);
  await clickNext(page);
  await clickLabelledCheckbox(page, 'I agree to the terms and conditions');
  await page.getByRole('button', { name: 'Submit' }).click();
  await page.waitForURL(/\/group-registration\/\d+/, { timeout: 120000, waitUntil: 'commit' });
  const match = page.url().match(/\/group-registration\/(\d+)/);
  if (!match) throw new Error(`Registration did not open (${page.url()})`);
  await expectFlowState(page, 'New');
  ctx.provide('pendingRegistrationId', match[1]);
}

export async function addRegistrationMember(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await ensureAttendance(page, member('activeMember').Name);
}

export async function addRegistrationGuest(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/guests`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await ensureAttendance(page, member('guest').Name);
}

export async function addRegistrationTeam(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/teams`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await ensureAttendance(page, team().TeamName);
}

async function openActivities(page, person) {
  const card = attendanceCard(page, person);
  await card.getByRole('button', { name: /Add Activities/ }).click();
  await page.getByText('Select Tickets').waitFor({ state: 'visible', timeout: 15000 });
  return card;
}

export async function assignRegistrationActivities(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  const memberName = member('activeMember').Name;
  const guestName = member('guest').Name;
  const teamName = team().TeamName;
  const individualTitle = defaultActivity().Title;
  const teamTitle = teamActivity().Title;

  const homer = attendanceCard(page, memberName);
  await homer.getByText(individualTitle).waitFor({ state: 'visible', timeout: 30000 });
  await openActivities(page, memberName);
  await selectOption(page, individualTitle).waitFor({ state: 'visible' });
  await page.getByRole('button', { name: 'Cancel' }).click();

  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/guests`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  const marge = attendanceCard(page, guestName);
  await marge.getByText(individualTitle).waitFor({ state: 'visible', timeout: 30000 });
  await openActivities(page, guestName);
  await selectOption(page, individualTitle).waitFor({ state: 'visible' });
  await page.getByRole('button', { name: 'Cancel' }).click();

  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/teams`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await openActivities(page, teamName);
  await clickSelectOption(page, teamTitle);
  await page.getByRole('button', { name: /Save Changes/ }).click();
  const teamCard = attendanceCard(page, teamName);
  await teamCard.getByText(teamTitle).waitFor({ state: 'visible', timeout: 30000 });
  await expectFlowState(page, 'New');
}

export async function editPendingRegistration(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  const person = member('activeMember');
  const title = defaultActivity().Title;
  const card = attendanceCard(page, person.Name);
  await card.getByRole('button', { name: /Add Activities/ }).click();
  await clickSelectOption(page, title);
  await page.getByRole('button', { name: /Save Changes/ }).click();
  await page.getByText('Select Tickets').waitFor({ state: 'hidden', timeout: 30000 });
  await page.reload({ waitUntil: 'domcontentloaded' });
  await card.getByText('Ticket Type').waitFor({ state: 'visible', timeout: 30000 });
  await card.getByText(title).waitFor({ state: 'hidden', timeout: 30000 });
  await expectFlowState(page, 'New');
}

export async function submitRegistration(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('pendingRegistrationId')];
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'registrations'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.getByRole('tab', { name: 'Active' }).click();
  await registrationRow(page, registrationId).click();
  await page.waitForURL(new RegExp(`/group-registration/${registrationId}`), { timeout: 60000, waitUntil: 'commit' });
  await page.getByRole('button', { name: 'Submit' }).click();
  await expectFlowState(page, 'Submitted');
  ctx.provide('submittedRegistrationId', registrationId);
}

export async function amendSubmittedRegistration(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('submittedRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  const person = member('activeMember');
  const title = defaultActivity().Title;
  const card = attendanceCard(page, person.Name);
  await card.getByRole('button', { name: /Add Activities/ }).click();
  await clickSelectOption(page, title);
  await page.getByRole('button', { name: /Save Changes/ }).click();
  await page.getByText('Select Tickets').waitFor({ state: 'hidden', timeout: 30000 });
  await page.reload({ waitUntil: 'domcontentloaded' });
  await card.getByText(title).waitFor({ state: 'visible', timeout: 30000 });
  await expectFlowState(page, 'Submitted');
}

export async function confirmRegistration(page, ctx) {
  const eventId = ctx.chain[normalizeToken('configuredEventId')];
  const registrationId = ctx.chain[normalizeToken('submittedRegistrationId')];
  await page.goto(`${WEB_BASE}/event/${encodeURIComponent(EVENT_NAME)}/${eventId}/registrations`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await registrationRow(page, registrationId).click();
  await page.waitForURL(new RegExp(`/event-registration/${registrationId}`), { timeout: 60000, waitUntil: 'commit' });
  await page.getByRole('button', { name: 'Confirm', exact: true }).click();
  await expectFlowState(page, 'Confirmed');
}

export async function seeConfirmedRegistration(page, ctx) {
  const registrationId = ctx.chain[normalizeToken('submittedRegistrationId')];
  await page.goto(`${WEB_BASE}/group-registration/${registrationId}/members`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await expectFlowState(page, 'Confirmed');
  if (await page.getByRole('button', { name: /Add Activities/ }).count()) {
    throw new Error('Group administrator can still edit a confirmed registration');
  }
}
