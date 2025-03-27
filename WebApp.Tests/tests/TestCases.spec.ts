import { test, expect, Page } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);

});

test.afterEach('Logout User',async ({ page }) => {
    await actions.click_button(page, 'logout');
    const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
    await expect(guest_user).toBeVisible();
});

async function filterTestCase(page: Page, testcaseName: string) {
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByRole('textbox', { name: 'Name filter value' }).click();
    await page.getByRole('textbox', { name: 'Name filter value' }).fill(testcaseName);
    await page.getByRole('button', { name: 'Apply' }).click();
    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }
    await expect(page.getByRole('table')).toContainText(testcaseName);
    await page.waitForLoadState('load');
}

test('Create New Test Case With Main Information', async ({ page })=> {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);
    const testcase_name = 'Test Case Playwright' + Random;
    await actions.LaunchProject(page, 'Demo Project Without Data' );

    await actions.click_button(page, 'd_testcases');

    await actions.click_button(page, 'create_testcase');

    await actions.fill_input(page, 'testcase_name', testcase_name);

    await actions.fill_input(page, 'testcase_description', 'Test Test Cases Playwright Description' + Random);

    await actions.select_dropdown_option(page, 'testcase_priority', 'Medium');


    await actions.select_dropdown_option(page,'testcase_status', 'New');

    await actions.select_dropdown_option(page,'testcase_assignedto', 'user@example.com');

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_testcase');

    await filterTestCase(page, testcase_name);

    await actions.click_button(page, 'delete');

    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText(testcase_name);
});