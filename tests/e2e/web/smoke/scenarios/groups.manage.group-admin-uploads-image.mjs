import path from 'path';
import { GROUP_NAME, fixtureImage, group } from '../config/load.mjs';
import { WEB_BASE, normalizeToken, withPersonaPage } from '../helpers.mjs';

export const id = 'groups.manage.group-admin-uploads-image';

export async function run(ctx) {
  const groupId = ctx.chain[normalizeToken('createdGroupId')];
  const imagePath = fixtureImage(group);
  const fileName = path.basename(imagePath);
  const groupPath = `${WEB_BASE}/group/${encodeURIComponent(GROUP_NAME)}/${groupId}/gallery`;
  await withPersonaPage('group-admin', async (page) => {
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
  });
}
