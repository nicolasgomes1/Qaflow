import { test, expect } from '@playwright/test';
import * as actions from '../TestSteps/ReusableTestSteps';
import * as login from '../TestSteps/LoginbyRole';

test.beforeEach('Login User',async ({ page }) => {
    login.LoginbyRole(page, login.Users.Admin);
});

test.afterEach('Logout User',async ({ page }) => {
        await page.getByTestId('logout').click();
        const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
        await expect(guest_user).toBeVisible();
});


test('CRUD a Project', async ({ page }) => {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);
    
    await actions.click_button(page, 'create_project');

    await actions.fill_input(page, 'project_name', 'Test Project Playwright' + Random);

    await actions.fill_input(page, 'project_description', 'Sample my description');

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }

    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill('Test Project Playwright' + Random);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: 'Test Project Playwright'+ Random })).toBeVisible();
    await page.getByRole('button', { name: 'delete' }).first().hover();
    await page.getByRole('button', { name: 'delete' }).first().click();
    await expect(page.locator('#rz-dialog-0-label')).toContainText('All underlying Data will be lost');
    await page.getByRole('button', { name: 'Ok' }).click();
});

