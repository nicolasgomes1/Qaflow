import { test, expect } from '@playwright/test';
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


test('Create a new Project', async ({ page }) => {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);
    
    await actions.click_button(page, 'create_project');

    const project_name = 'Test Project Playwright' + Random;
    await actions.fill_input(page, 'project_name', project_name);

    await actions.fill_input(page, 'project_description', 'Sample my description');

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }

    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill(project_name);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: project_name })).toBeVisible();
    
    // Delete the project
    await page.getByRole('button', { name: 'delete' }).first().hover();
    await page.getByRole('button', { name: 'delete' }).first().click();
    await expect(page.locator('#rz-dialog-0-label')).toContainText('All underlying Data will be lost');
    await page.getByRole('button', { name: 'Ok' }).click();
    await expect(page.getByRole('table')).not.toContainText(project_name);

});

test('View a new Project', async ({ page }) => {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await actions.click_button(page, 'create_project');

    const project_name = 'Test Project Playwright' + Random;
    await actions.fill_input(page, 'project_name', project_name);

    const description = 'Sample my description';
    await actions.fill_input(page, 'project_description', description);

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }

    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill(project_name);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: project_name })).toBeVisible();

    await actions.click_button(page, 'view');

    expect(page.getByText(project_name)).toBeVisible();
    expect(page.getByText(description)).toBeVisible();
    
});

test('Edit a new Project', async ({ page }) => {
    test.slow();
    const Random = Math.floor(Math.random() * 1000);

    await actions.click_button(page, 'create_project');

    const project_name = 'Test Project Playwright' + Random;
    await actions.fill_input(page, 'project_name', project_name);

    const description = 'Sample my description';
    await actions.fill_input(page, 'project_description', description);

    await actions.submit_form(page);

    await actions.validate_button(page, 'create_project');

    if (await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()) {
        await page.locator('.rz-notification-item > div:nth-child(2)').click();
    }

    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').hover();
    await page.getByRole('columnheader', { name: 'Name sort filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill(project_name);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: project_name })).toBeVisible();

    await actions.click_button(page, 'edit');

    await actions.validate_input(page, 'project_name', project_name);
    await actions.validate_input(page, 'project_description', description);
    

    await actions.submit_form(page);
    await page.locator('.rz-notification-item > div:nth-child(2)').isVisible()
    await page.locator('.rz-notification-item > div:nth-child(2)').click()

});

