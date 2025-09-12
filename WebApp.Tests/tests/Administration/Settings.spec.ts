import { test, expect } from '@playwright/test';
import * as actions from '../../TestSteps/ReusableTestSteps';
import * as login from '../../TestSteps/LoginbyRole';
import * as filter from '../../TestSteps/FilterSteps';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
});

test('Enable Own Integration', async ({ page }) => {
    await actions.click_element(page, 'admin_menu');
    await actions.click_element(page, 'manage_settings');

    const row = page.locator('tr', {hasText: 'OwnIntegrations'});
    await expect(row).toContainText('Disabled');
    row.getByTestId("edit_button").click();
    await page.waitForTimeout(1000);
    row.getByTestId("checkbox").click();
    row.getByTestId("save_button").click();
    await page.waitForTimeout(1000);

    await expect(row).toContainText('Enabled');
    await page.waitForTimeout(1000);

    row.getByTestId("edit_button").click();
    await page.waitForTimeout(1000);
    row.getByTestId("checkbox").click();
    await page.waitForTimeout(1000);

    row.getByTestId("save_button").click();

    await expect(row).toContainText('Disabled');


});