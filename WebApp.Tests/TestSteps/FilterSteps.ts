import { expect, Page } from '@playwright/test';


async function filterTableModel(page: Page, modelName: string) {
    // Wait for the column header to be visible before interacting
    const columnHeader = page.getByRole('columnheader', { name: 'Name sort filter_alt' });
    await columnHeader.waitFor({ state: 'visible' });

    // Hover and click with proper waiting for each step
    const icon = columnHeader.locator('i');
    await icon.hover();
    await icon.click();

    // Wait for the textbox to appear before interacting with it
    const filterTextbox = page.getByRole('textbox', { name: 'Name filter value' });
    await filterTextbox.waitFor({ state: 'visible' });

    // Clear any existing value and fill in the new model name
    await filterTextbox.click();
    await filterTextbox.fill(modelName);

    // Wait for the Apply button to be visible and clickable
    const applyButton = page.getByRole('button', { name: 'Apply' });
    await applyButton.waitFor({ state: 'visible' });
    await applyButton.click();

    // Handle potential notifications that may pop up after applying the filter
    const notification = page.locator('.rz-notification-item > div:nth-child(2)');
    if (await notification.isVisible()) {
        await notification.click();
    }

    // Ensure that the table has been updated with the filtered result
    const table = page.getByRole('table');
    await table.waitFor({ state: 'visible' });

    // Wait for the table to contain the model name after filtering
    await expect(table).toContainText(modelName);

    // Wait for the page to finish loading
    await page.waitForLoadState('load');
}




export {filterTableModel}