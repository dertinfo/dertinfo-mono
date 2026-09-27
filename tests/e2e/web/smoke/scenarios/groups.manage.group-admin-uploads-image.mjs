import { FIXTURE_IMAGE, WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';

export const id = 'groups.manage.group-admin-uploads-image';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  const groupPath = `${WEB_BASE}/group/${encodeURIComponent('The Simpsons')}/${groupId}/gallery`;
  await withPersonaPage('group-admin', async (page) => {
    await page.goto(groupPath, { waitUntil: 'domcontentloaded', timeout: 60000 });
    await page.getByText('Uploaded Photos', { exact: true }).waitFor({ state: 'visible', timeout: 30000 });
    const before = await page.locator('mat-grid-tile').count();
    await page.locator('#app-addition-header button').click();
    await page.getByText('Upload Images', { exact: true }).waitFor({ state: 'visible', timeout: 15000 });
    await page.locator('input[type="file"]').setInputFiles(FIXTURE_IMAGE);
    await page.locator('button', { hasText: 'Upload' }).filter({ has: page.locator('mat-icon') }).click();
    const deadline = Date.now() + 90000;
    let seen = false;
    while (Date.now() < deadline) {
      const count = await page.locator('mat-grid-tile').count();
      const nameVisible = await page.getByText('smoke-upload.jpg').count();
      if (count > before || nameVisible > 0) {
        seen = true;
        break;
      }
      await page.waitForTimeout(1000);
    }
    if (!seen) throw new Error('Uploaded photo did not appear under Uploaded Photos');
  });
}
