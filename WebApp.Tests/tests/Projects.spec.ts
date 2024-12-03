import { test, expect } from '@playwright/test';


const URL = 'https://localhost:7089';

test.beforeEach('Login User',async ({ page }) => {
    await page.goto(URL + '/Account/Login');
    await page.getByTestId('login_emailform').fill('admin@example.com');
    await page.getByTestId('login_passwordform').fill('Admin123!');
    await page.getByTestId('login_submitform').click();
    const heading = page.locator('strong', { hasText: 'Welcome, admin@example.com!'});
    await expect(heading).toBeVisible();
});

test.afterEach('Logout User',async ({ page }) => {
        await page.getByTestId('logout').click();
        const guest_user = page.locator('strong', { hasText: 'Welcome, Guest User!'});
        await expect(guest_user).toBeVisible();
});


test('test', async ({ page }) => {
    const Random = Math.floor(Math.random() * 1000);
    
    await page.getByTestId('create_project').click();
    await page.getByTestId('project_name').click();
    await page.getByTestId('project_name').fill('Test Project Playwright' + Random);
    await page.getByTestId('project_name').press('Tab');
    await expect(page.getByTestId('project_name')).toHaveValue('Test Project Playwright' + Random);
    await page.getByTestId('project_description').click();
    await page.getByTestId('project_description').fill('Sample my description');
    await page.getByTestId('project_description').press('Tab');
    await expect(page.getByTestId('project_description')).toHaveValue('Sample my description');
    await page.getByTestId('submit').click();
    await page.getByRole('columnheader', { name: 'Name filter_alt' }).locator('i').click();
    await page.getByLabel('Name filter value').click();
    await page.getByLabel('Name filter value').fill('Test Project Playwright' + Random);
    await page.getByRole('button', { name: 'Apply' }).click();
    await expect(page.locator('tr', { hasText: 'Test Project Playwright'+ Random })).toBeVisible();
    await page.getByRole('button', { name: 'delete' }).first().click();
    await expect(page.locator('#rz-dialog-0-label')).toContainText('All underlying Data will be lost');
    await page.getByRole('button', { name: 'Ok' }).click();
});

