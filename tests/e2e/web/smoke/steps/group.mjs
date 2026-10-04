import path from 'path';
import { GROUP_NAME, fixtureImage, group, member, team } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, openFabItem, waitForDashboardFab } from '../helpers.mjs';
import { clickLabelledCheckbox, clickLabelledRadio, clickNext, clickPageAdd, entityCard, fillControl, groupAdminUrl, openCardMenu } from '../pages.mjs';

export async function createGroup(page, ctx) {
  await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
  await openFabItem(page, 'create-group');
  await page.locator('input[formcontrolname="groupName"]').fill(group.GroupName);
  await page.locator('input[formcontrolname="groupBio"]').fill(group.GroupBio);
  await page.getByRole('button', { name: /Add Group/ }).click();
  await page.waitForURL(/\/group-configure\/[^/]+\/\d+/, { timeout: 120000, waitUntil: 'commit' });
  const match = page.url().match(/\/group-configure\/[^/]+\/(\d+)/);
  if (!match) throw new Error(`Create group did not open configure (${page.url()})`);
  ctx.provide('createdGroupId', match[1]);
}

export async function seeUnconfiguredGroup(page, ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
  await waitForDashboardFab(page);
  const card = entityCard(page, GROUP_NAME);
  await card.getByText('Needs more information').waitFor({ state: 'visible', timeout: 30000 });
  await openCardMenu(page, GROUP_NAME);
  await page.getByRole('menuitem', { name: 'Configure' }).click();
  await page.waitForURL(new RegExp(`/group-configure/.*/${groupId}/`), { timeout: 60000, waitUntil: 'commit' });
}

export async function configureGroup(page, ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  await page.goto(`${WEB_BASE}/group-configure/${encodeURIComponent(GROUP_NAME)}/${groupId}/start`, {
    waitUntil: 'domcontentloaded',
    timeout: 60000,
  });
  await fillControl(page, 'groupBio', group.GroupBio);
  await fillControl(page, 'originTown', group.OriginTown);
  await fillControl(page, 'originPostcode', group.OriginPostcode);
  await clickNext(page);
  await fillControl(page, 'contactName', group.PrimaryContactName);
  await fillControl(page, 'contactEmail', group.PrimaryContactEmail);
  await fillControl(page, 'contactTelephone', group.PrimaryContactNumber);
  await clickNext(page);
  await clickNext(page);
  await clickNext(page);
  await clickLabelledCheckbox(page, 'I agree to the terms and conditions');
  await clickNext(page);
  await page.getByRole('button', { name: 'Submit' }).click();
  await page.waitForURL(new RegExp(`/group/${encodeURIComponent(GROUP_NAME)}/${groupId}`), {
    timeout: 120000,
    waitUntil: 'commit',
  });
  ctx.provide('configuredGroupId', groupId);
  await page.goto(`${WEB_BASE}/dashboard`, { waitUntil: 'domcontentloaded', timeout: 60000 });
  const card = entityCard(page, GROUP_NAME);
  await card.waitFor({ state: 'visible', timeout: 30000 });
  if (await card.getByText('Needs more information').isVisible()) {
    throw new Error('Configured group still shows Needs more information');
  }
  await page.goto(groupAdminUrl(groupId), { waitUntil: 'domcontentloaded', timeout: 60000 });
}

export async function seeDefaultTeam(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'teams'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.locator('app-group-teams').getByText(GROUP_NAME, { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
}

export async function addGroupMember(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'members'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await clickPageAdd(page);
  await page.getByRole('tab', { name: 'Add New Person' }).click();
  const person = member('activeMember');
  await fillControl(page, 'name', person.Name);
  await clickLabelledRadio(page, 'Member');
  await page.getByRole('button', { name: /Add Member/ }).click();
  await page.getByText(person.Name).waitFor({ state: 'visible', timeout: 30000 });
  ctx.provide('addedMemberId', person.Name);
}

export async function addGroupGuest(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'members'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await clickPageAdd(page);
  await page.getByRole('tab', { name: 'Add New Person' }).click();
  const person = member('guest');
  await fillControl(page, 'name', person.Name);
  await clickLabelledRadio(page, 'Guest');
  await page.getByRole('button', { name: /Add Member/ }).click();
  await page.getByRole('tab', { name: 'Guests' }).click();
  await page.getByText(person.Name).waitFor({ state: 'visible', timeout: 30000 });
  ctx.provide('addedGuestId', person.Name);
}

export async function addGroupTeam(page, ctx) {
  const groupId = ctx.chain[normalizeToken('configuredGroupId')];
  await page.goto(groupAdminUrl(groupId, 'teams'), { waitUntil: 'domcontentloaded', timeout: 60000 });
  await clickPageAdd(page);
  await page.getByRole('tab', { name: 'Add New Team' }).click();
  const groupTeam = team();
  await fillControl(page, 'teamName', groupTeam.TeamName);
  await fillControl(page, 'teamBio', groupTeam.TeamBio);
  await page.getByRole('button', { name: /Add Team/ }).click();
  await page.locator('app-group-teams').getByText(groupTeam.TeamName, { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
  ctx.provide('addedTeamId', groupTeam.TeamName);
}

export async function changeGroupImage(page, ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  const imagePath = fixtureImage(group);
  const fileName = path.basename(imagePath);
  const groupPath = `${WEB_BASE}/group/${encodeURIComponent(GROUP_NAME)}/${groupId}/gallery`;
  await page.goto(groupPath, { waitUntil: 'domcontentloaded', timeout: 60000 });
  await page.getByText('Uploaded Photos', { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
  const before = await page.getByTestId('gallery-photo').locator('img').evaluateAll((images) => (
    images.map((image) => image.getAttribute('src')).filter(Boolean)
  ));
  await page.getByTestId('page-add').click();
  const dialog = page.getByRole('dialog');
  await dialog.getByText('Upload Images', { exact: true }).waitFor({ state: 'visible', timeout: 15000 });
  await dialog.getByTestId('upload-file').setInputFiles(imagePath);
  await dialog.getByText(fileName, { exact: true }).waitFor({ state: 'visible', timeout: 15000 });
  await dialog.getByTestId('upload-all').click();
  await dialog.waitFor({ state: 'hidden', timeout: 90000 });
  try {
    await page.waitForFunction((previous) => {
      const sources = [...document.querySelectorAll('[data-testid="gallery-photo"] img')]
        .map((image) => image.getAttribute('src'))
        .filter(Boolean);
      return sources.some((source) => !previous.includes(source));
    }, before, { timeout: 30000 });
  } catch {
    throw new Error('Uploaded photo did not replace the gallery image');
  }
}
