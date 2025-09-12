import { test, expect } from '@playwright/test';
import * as actions from '../../TestSteps/ReusableTestSteps';
import * as login from '../../TestSteps/LoginbyRole';

test.beforeEach('Login User', async ({ page }) => {
    await login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User', async ({ page }) => {
    await actions.click_button(page, 'logout');
});

test('Enable Own Integration', async ({ page }) => {
    await page.waitForLoadState('networkidle');

    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_settings');

    const row = page.locator('tr', { hasText: 'OwnIntegrations' });
    await expect(row).toBeVisible();
    await expect(row).toContainText('Disabled');

    // Click edit
    await row.getByTestId('edit_button').click();

    // Custom checkbox: click instead of setChecked
    const checkbox = row.getByTestId('checkbox');
    await expect(checkbox).toBeVisible();
    await checkbox.click(); // <-- click instead of setChecked

    // Save
    await row.getByTestId('save_button').click();
    await expect(row).toContainText('Enabled');

    // Disable again
    await row.getByTestId('edit_button').click();
    await expect(checkbox).toBeVisible();
    await checkbox.click(); // click to uncheck
    await row.getByTestId('save_button').click();
    await expect(row).toContainText('Disabled');
});
