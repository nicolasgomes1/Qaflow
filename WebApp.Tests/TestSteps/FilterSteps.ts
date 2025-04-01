import { expect, Page } from '@playwright/test';


async function filterTableModel(page: Page, modelName: string) {
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(modelName);
    await page.getByRole('button', { name: 'Apply' }).click();
    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }
    await expect(page.getByRole('table')).toContainText(modelName);
    await page.waitForLoadState('load');
}

export {filterTableModel}