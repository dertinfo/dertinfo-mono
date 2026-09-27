import { WEB_BASE } from './helpers.mjs';

export const GROUP_NAME = 'The Simpsons';
export const EVENT_NAME = 'City Zoo Gathering';

export function groupAdminUrl(groupId, child = 'overview') {
  return `${WEB_BASE}/group/${encodeURIComponent(GROUP_NAME)}/${groupId}/${child}`;
}

export function eventAdminUrl(eventId, child = 'overview') {
  return `${WEB_BASE}/event/${encodeURIComponent(EVENT_NAME)}/${eventId}/${child}`;
}

export async function fillControl(page, name, value) {
  await page.locator(`input[formcontrolname="${name}"], textarea[formcontrolname="${name}"]`).fill(value);
}

export async function clickLabelledCheckbox(page, label) {
  await page.locator('mat-checkbox', { hasText: label }).click();
}

export async function clickNext(page) {
  const header = page.locator('mat-step-header[aria-selected="true"]');
  const before = Number(await header.getAttribute('aria-posinset'));
  await page.locator('.mat-step:has(mat-step-header[aria-selected="true"]) button.mat-stepper-next').click({ force: true });
  await page.locator(`mat-step-header[aria-selected="true"][aria-posinset="${before + 1}"]`).waitFor({
    state: 'visible',
    timeout: 15000,
  });
}

export async function openCardMenu(page, title) {
  const card = page.locator('mat-card', { hasText: title }).first();
  await card.locator('button').first().click();
  return card;
}

export async function bodyText(page) {
  return (await page.locator('body').innerText()).replace(/\u00a0/g, ' ');
}

export async function expectFlowState(page, name) {
  const radio = page.getByRole('radio', { name, exact: true });
  await radio.waitFor({ state: 'visible', timeout: 30000 });
  const deadline = Date.now() + 30000;
  while (Date.now() < deadline) {
    if (await radio.isChecked()) return;
    await page.waitForTimeout(250);
  }
  throw new Error(`Registration flow state is not ${name}`);
}

export async function ensureAttendance(page, name) {
  await page.locator('#app-addition-header button').click();
  const dialog = page.locator('mat-dialog-container');
  const item = dialog.locator('.item-select', { hasText: name });
  await item.waitFor({ state: 'visible', timeout: 15000 });
  const selected = await item.evaluate((el) => el.classList.contains('item-selected'));
  if (selected) {
    await dialog.getByRole('button', { name: 'Cancel' }).click();
  } else {
    await item.locator('button').click();
    await dialog.getByRole('button', { name: /Save Changes/ }).click();
  }
  await dialog.waitFor({ state: 'hidden', timeout: 30000 });
  await page.locator('mat-card-title', { hasText: name }).first().waitFor({ state: 'visible', timeout: 30000 });
}
